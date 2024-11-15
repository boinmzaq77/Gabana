using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using EDMTDev.ZXingXamarinAndroid;
using Gabana.Droid.Adapter;
using Gabana.Droid.Phone;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class CartScanActivity : AppCompatActivity /*Activity*/, IPermissionListener
    {
        public static CartScanActivity scan;
        public CartScanActivity scanbarcode;
        public static ZXingScannerView ScannerView;
        RecyclerView recyclerview_listitemcart;
        Cart_Adapter_Item cart_Adapter_Item; //Body Show Row Menu
        public TextView Total, VatAmount, textTotal, textRemark, textMember, textNumMember, textDiscount, textNumDiscount, textVat, textNumVat, textNumService;
        public Button ButtonQuantity;
        ImageButton imgBack;
        public static TranWithDetailsLocal tranWithDetails;
        public TranDetailItemWithTopping tranDetail;
        public static string totalResult;
        List<Item> lstscanItem = new List<Item>();
        public static int detailNo = 0;
        public static int detailNoClickOption = 0;
        public static long DetailNoOld = 0;
        public static FrameLayout lnRemark, frameClickOption, frameShowOption;
        static LinearLayout lnCartdetail, lnDiscountMember, lnShowDiscount, lnVat, lnShowService;
        public bool clearcart;
        public static bool addDiscount;
        public double total = 0.00;
        public string CartDiscount = "";
        string CURRENCYSYMBOLS;
        public bool flagShowEditOption = true; //Option ของ Item ที่อยู่ใน Cart
        public static bool CurrentActivity, openlstTopping = false;
        TextView textCartService, textCartDiscount;
        static LinearLayout lnNoCustomer, lnHaveCustomer;
        static ImageButton btnCustomer;
        static TextView txtNameCustomer;
        static Customer selectCus;
        public static bool IsActive = false;
        public static long SysItemFocus = 0;
        LinearLayout lnPayment;
        TextView textSum, txtNoItem;
        public static List<string> lstSysItemIdStatusD = new List<string>();
        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.scan_activity_main);
                scan = this;

                //setview
                ScannerView = FindViewById<ZXingScannerView>(Resource.Id.zxscan);
                lnShowService = FindViewById<LinearLayout>(Resource.Id.lnShowService);
                lnVat = FindViewById<LinearLayout>(Resource.Id.lnVat);

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImgBack_Click;
                imgBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                recyclerview_listitemcart = FindViewById<RecyclerView>(Resource.Id.recyclerview_listitemcart);

                lnPayment = FindViewById<LinearLayout>(Resource.Id.linearPayment);
                textSum = FindViewById<TextView>(Resource.Id.txtSumary);
                txtNoItem = FindViewById<TextView>(Resource.Id.txtNoItem);

                textRemark = FindViewById<TextView>(Resource.Id.textRemark);
                textTotal = FindViewById<TextView>(Resource.Id.textTotal);
                textMember = FindViewById<TextView>(Resource.Id.textMember);
                textNumMember = FindViewById<TextView>(Resource.Id.textNumMember);
                textDiscount = FindViewById<TextView>(Resource.Id.textDiscount);
                textCartDiscount = FindViewById<TextView>(Resource.Id.textCartDiscount);
                textNumDiscount = FindViewById<TextView>(Resource.Id.textNumDiscount);
                textVat = FindViewById<TextView>(Resource.Id.textVat);
                textNumVat = FindViewById<TextView>(Resource.Id.textNumVat);
                lnRemark = FindViewById<FrameLayout>(Resource.Id.lnRemark);
                frameShowOption = FindViewById<FrameLayout>(Resource.Id.frameShowOption);
                frameClickOption = FindViewById<FrameLayout>(Resource.Id.frameClickOption);
                lnCartdetail = FindViewById<LinearLayout>(Resource.Id.lnCartdetail);
                lnDiscountMember = FindViewById<LinearLayout>(Resource.Id.lnDiscountMember);
                lnShowDiscount = FindViewById<LinearLayout>(Resource.Id.lnShowDiscount);
                textCartService = FindViewById<TextView>(Resource.Id.textCartService);
                textNumService = FindViewById<TextView>(Resource.Id.textNumService);

                btnCustomer = FindViewById<ImageButton>(Resource.Id.btnCustomer);
                txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
                lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
                lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
                txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
                btnCustomer.Click += BtnCustomer_Click;
                lnNoCustomer.Click += BtnCustomer_Click;
                lnHaveCustomer.Click += BtnCustomer_Click;
                CheckJwt();
                SetCustomer();

                imgBack.Click += ImgBack_Click;
                lnPayment.Click += BtnPayment_Click;
                frameClickOption.Click += FrameClickOption_Click;

                //Request Permission
                Dexter.WithActivity(this)
                    .WithPermission(Manifest.Permission.Camera)
                    .WithListener(this)
                    .Check();

                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";

                bool flagitemstatusd = false, flagcartupdate = false;

                //1. เคสสินค้าถูกลบ 
                lstSysItemIdStatusD = Utils.CheckStatusIteminCart(tranWithDetails);
                if (lstSysItemIdStatusD.Count > 0)
                {
                    flagitemstatusd = true;
                }

                //2. เคสแก้ไขรายละเอียดของ merchantconfig 
                HasChangeinCart hasChange = await Utils.CheckSettingConfiginCart(tranWithDetails);
                if (hasChange.FlagChange)
                {
                    flagcartupdate = true;
                    tranWithDetails = hasChange.tranWithDetailsLocal;
                }

                if (!flagitemstatusd && !flagcartupdate)
                {
                    return;
                }
                else if (flagitemstatusd && !flagcartupdate)
                {
                    DialogShowItemStatusDClick("itemstatusd");
                }
                else if (!flagitemstatusd && flagcartupdate)
                {
                    DialogShowItemStatusDClick("cartupdate");
                }
                else
                {
                    DialogShowItemStatusDClick("updatecart");
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : CartScanActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Cartscan");
                Toast.MakeText(scan, ex.Message, ToastLength.Short).Show();
            }
        }
        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
        }
        private void FrameClickOption_Click(object sender, EventArgs e)
        {
            try
            {
                if (tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    Cart_Dialog_Option.SetTranDetail(tranWithDetails);
                    Cart_Dialog_Option dialog = new Cart_Dialog_Option();
                    var fragment = new Cart_Dialog_Option();
                    fragment.Show(SupportFragmentManager, nameof(Cart_Dialog_Option));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FrameClickOption_Click at Cartscan");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstSysItemIdStatusD.Count > 0)
                {
                    return;
                }

                if (tranWithDetails.tran.GrandTotal < 0)
                {
                    Toast.MakeText(this, "ไม่มียอดที่ต้องชำระ", ToastLength.Short).Show();
                    return;
                }

                if (tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    detailNoClickOption = 0;
                    StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                    PaymentActivity.SetTranDetail(tranWithDetails);
                    IsActive = false;
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPayment_Click at Cartscan");
                Toast.MakeText(scan, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ImgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            try
            {
                detailNoClickOption = 0;
                CurrentActivity = false;
                PosActivity.SetTranDetail(tranWithDetails);
                IsActive = false;
                this.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPayment_Click at Cartscan");
                Toast.MakeText(scan, ex.Message, ToastLength.Short).Show();
            }
        }

        public void DialogClearCart()
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("clearcart", "clearcart");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at Cart");
                Toast.MakeText(this, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                return;
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                ScannerView.StopCamera();
                base.OnDestroy();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPayment_Click at Cartscan");
                Toast.MakeText(scan, ex.Message, ToastLength.Short).Show();
            }
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this, "You Must Enable Permission", ToastLength.Long).Show();
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            ScannerView.SetResultHandler(new MyResultHandler(this));
            ScannerView.SetAutoFocus(true);
            ScannerView.SetLaserColor(Color.LightBlue);
            ScannerView.SetBorderColor(Color.Transparent); 
            ScannerView.StartCamera();
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {

        }

        private class MyResultHandler : AppCompatActivity, IResultHandler
        {
            private CartScanActivity scanbarcodeActivity;

            public MyResultHandler(CartScanActivity scanbarcodeActivity)
            {
                this.scanbarcodeActivity = scanbarcodeActivity;
            }


            public async void HandleResult(ZXing.Result rawResult)
            {
                try
                {
                    Vibration.Vibrate();
                    var duration = TimeSpan.FromMilliseconds(1);
                    Vibration.Vibrate(duration);
                    var scanResult = rawResult.Text;
                    //scanResult = System.Text.RegularExpressions.Regex.Match(scanResult, @"\d+").Value;
                    ItemManage itemManage = new ItemManage();
                    List<Item> Itemresult = new List<Item>();
                    var result = await itemManage.GetItemPOSfScanBarcode(scanResult);
                    if (result != null)
                    {
                        Itemresult = result;
                    }
                    //--------------------------------------
                    //กรณีมี 1 ค่า
                    //เพิ่มข้อมูลลงตะกร้า
                    //กรณีมีหลายค่า
                    //Pop up ให้เลือกค่าก่อนเพิ่มลงตะกร้า
                    //--------------------------------------
                    //scan //this //this.scanbarcodeActivity //Application.Context //scanbarcodeActivity //
                    if (Itemresult.Count == 0)
                    {
                        Toast.MakeText(this.scanbarcodeActivity, this.scanbarcodeActivity.GetString(Resource.String.notdata), ToastLength.Short).Show();
                        //Toast.MakeText(Application.Context, "testtt", ToastLength.Short).Show();
                        Dexter.WithActivity(scan)
                        .WithPermission(Manifest.Permission.Camera)
                        .WithListener(scan)
                        .Check();
                        return;
                    }
                    if (Itemresult.Count == 1)
                    {
                        DataCashing.flagScan = true;
                    }

                    if (Itemresult.Count > 1)
                    {
                        DataCashing.flagScan = true;
                        scan.CallDailog(Itemresult);
                        return;
                    }

                    //เมื่อสแกนเจอสินค้าให้แอดลงตะกร้าเลย
                    ScannerView.StartCamera();
                    ScannerView.SetResultHandler(new MyResultHandler(scanbarcodeActivity));
                    //แสดงข้อมูลที่หน้าจอ
                    //scan.ReloadData(Itemresult[0]);
                    //เพิ่มข้อมูลไปที่ Tran
                    scan.ScanItemtoCart(Itemresult[0]);

                }
                catch (Exception ex)
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                    _ = TinyInsights.TrackPageViewAsync("HandleResult at Cartscan");
                    Toast.MakeText(scan, ex.Message, ToastLength.Short).Show();
                    Dexter.WithActivity(scan)
                        .WithPermission(Manifest.Permission.Camera)
                        .WithListener(scan)
                        .Check();
                }
            }
        }

        private void CallDailog(List<Item> itemresult)
        {
            try
            {
                ScanCart_Dialog_Item.SetListItem(itemresult);
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.cartscan_dialog_items.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("deleteType", "category");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
                dialog.Cancelable = false;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CallDailog at Cartscan");
                Toast.MakeText(scan, ex.Message, ToastLength.Short).Show();
            }
        }

        internal void Onresume()
        {
            OnResume();
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();

                CheckJwt();
                IsActive = true;
                CurrentActivity = true;
                Dexter.WithActivity(this)
                        .WithPermission(Manifest.Permission.Camera)
                        .WithListener(this)
                        .Check();

                SetCustomer();
                SetITemInCart();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Cartscan");
                base.OnRestart();
            }
        }
        public static async void SetCustomer()
        {
            try
            {
                if (DataCashing.SysCustomerID == 999)
                {
                    if (tranWithDetails.tran == null)
                    {
                        return;
                    }

                    if (tranWithDetails.tran.SysCustomerID != 999)
                    {
                        tranWithDetails = await BLTrans.RemovePerson(tranWithDetails);
                    }

                    lnHaveCustomer.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnHaveCustomer.Visibility = ViewStates.Visible;
                    lnNoCustomer.Visibility = ViewStates.Gone;

                    CustomerManage customerManage = new CustomerManage();
                    var listCustomer = new List<Customer>();
                    listCustomer = await customerManage.GetAllCustomer();
                    selectCus = listCustomer.Where(x => x.SysCustomerID == DataCashing.SysCustomerID).FirstOrDefault();
                    if (selectCus == null) return;
                    if (tranWithDetails.tran.SysCustomerID != DataCashing.SysCustomerID)
                    {
                        tranWithDetails.tran.SysCustomerID = selectCus.SysCustomerID;
                        tranWithDetails.tran.CustomerName = selectCus.CustomerName;

                        tranWithDetails = await BLTrans.ChoosePerson(tranWithDetails, selectCus);
                        DataCashing.ModifyTranOrder = true;
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    }
                    txtNameCustomer.Text = tranWithDetails.tran.CustomerName?.ToString();
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        public async void SetITemInCart()
        {
            try
            {
                if (tranWithDetails.tran != null)
                {
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                    {
                        textSum.Text = "0.00 " + CURRENCYSYMBOLS;
                        if (DataCashing.Language == "th")
                        {
                            txtNoItem.Text = "ไม่มีสินค้า";
                        }
                        else
                        {
                            txtNoItem.Text = "No item";
                        }

                        ////ไม่แสดงง
                        lnCartdetail.Visibility = ViewStates.Gone;
                        //frameShowOption.Visibility = ViewStates.Gone;
                        txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        lnPayment.SetBackgroundResource(Resource.Drawable.btnborderblue);
                    }
                    else
                    {
                        var quantity = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity);
                        textSum.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.SubAmount)) + " " + CURRENCYSYMBOLS;

                        if (DataCashing.Language == "th")
                        {
                            txtNoItem.Text = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString("#,###") + " รายการ";
                        }
                        else
                        {
                            if (quantity == 1)
                            {
                                txtNoItem.Text = quantity.ToString("#,###") + " item";
                            }
                            else
                            {
                                txtNoItem.Text = quantity.ToString("#,###") + " items";
                            }

                        }
                        //txtNoItem.Text = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity).ToString() + " items";

                        txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                        textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                        lnPayment.SetBackgroundResource(Resource.Drawable.btnblue);
                        lnCartdetail.Visibility = ViewStates.Visible;
                        //frameShowOption.Visibility = ViewStates.Visible;
                    }
                    total = Convert.ToDouble(tranWithDetails.tran.GrandTotal);

                    //Discount From Member 
                    double disMember = 0;
                    if (DataCashing.SysCustomerID == 999)
                    {
                        lnDiscountMember.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        var tranTradDiscountMember = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "PS").FirstOrDefault();
                        if (tranTradDiscountMember != null)
                        {
                            var check = tranTradDiscountMember.FmlDiscount.IndexOf('%');
                            if (check == -1)
                            {
                                textMember.Text = "Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", "")));
                            }
                            else
                            {
                                textMember.Text = "Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%";
                            }
                            lnDiscountMember.Visibility = ViewStates.Visible;
                            disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                            textNumMember.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                        }
                        else
                        {
                            lnDiscountMember.Visibility = ViewStates.Gone;
                        }
                    }

                    //Discount From Discount
                    double discount;
                    double disDiscont = 0.0;
                    var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                    if (tranTradDiscount != null)
                    {
                        addDiscount = true;

                        var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                        if (check == -1)
                        {
                            CartDiscount = tranTradDiscount.FmlDiscount;
                            discount = Convert.ToDouble(CartDiscount);
                            textCartDiscount.Text = GetString(Resource.String.discount) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount));
                            disDiscont = discount;
                        }
                        else
                        {

                            discount = Convert.ToDouble(tranTradDiscount.FmlDiscount.Remove(check));
                            textCartDiscount.Text = GetString(Resource.String.discount) + " " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount.Remove(check))) + "%";
                            discount = discount / 100;
                            disDiscont = total * discount;
                        }

                        disDiscont = Convert.ToDouble(tranTradDiscount.TradeDiscHaveVat + tranTradDiscount.TradeDiscNoneVat);
                        lnShowDiscount.Visibility = ViewStates.Visible;
                        
                        textNumDiscount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disDiscont));
                    }
                    else
                    {
                        lnShowDiscount.Visibility = ViewStates.Gone;
                        addDiscount = false;
                    }

                    // Vat
                    string getvat = Utils.DisplayDouble(tranWithDetails.tran.TaxRate == null ? 0 : tranWithDetails.tran.TaxRate.Value);
                    if (tranWithDetails.tran.TotalVat == 0)
                    {
                        lnVat.Visibility = ViewStates.Gone;
                    }

                    textVat.Text = GetString(Resource.String.vat) + " " + getvat + "%";

                    textNumVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);

                    textTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);

                    //Service charge
                    string ServiceCharge;
                    if (string.IsNullOrEmpty(tranWithDetails.tran.FmlServiceCharge))
                    {
                        lnShowService.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        if (tranWithDetails.tran.FmlServiceCharge == "0" | tranWithDetails.tran.FmlServiceCharge == "0.00" | tranWithDetails.tran.FmlServiceCharge == "0.0000")
                        {
                            lnShowService.Visibility = ViewStates.Gone;
                        }

                        var checkservice = tranWithDetails.tran.FmlServiceCharge.IndexOf('%');
                        if (checkservice == -1)
                        {
                            ServiceCharge = tranWithDetails.tran.FmlServiceCharge;
                            textCartService.Text = GetString(Resource.String.servicecharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
                        }
                        else
                        {
                            string[] split = tranWithDetails.tran.FmlServiceCharge.Split('%');
                            ServiceCharge = split[0];
                            textCartService.Text = GetString(Resource.String.servicecharge) + " " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
                        }
                        textNumService.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                    }

                    //Remark 
                    if (!string.IsNullOrEmpty(tranWithDetails.tran.Comments))
                    {
                        lnRemark.Visibility = ViewStates.Visible;
                        textRemark.Text = tranWithDetails.tran.Comments;
                    }
                    else
                    {
                        lnRemark.Visibility = ViewStates.Gone;
                    }
                }
                cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                recyclerview_listitemcart.SetLayoutManager(gridLayoutManager);
                recyclerview_listitemcart.HasFixedSize = true;
                recyclerview_listitemcart.SetItemViewCacheSize(20);
                recyclerview_listitemcart.SetAdapter(cart_Adapter_Item);
                cart_Adapter_Item.ItemClick += Cart_Adapter_Item_ItemClick;

                if (lstSysItemIdStatusD.Count > 0 && tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    lnPayment.SetBackgroundResource(Resource.Drawable.btnborderblue);
                }
                else
                {
                    txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    lnPayment.SetBackgroundResource(Resource.Drawable.btnblue);
                }

                ItemFocus(SysItemFocus);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SetITemInCart at Cart");
            }
        }

        private async void Cart_Adapter_Item_ItemClick(object sender, int e)
        {
            try
            {
                detailNoClickOption = (int)tranWithDetails.tranDetailItemWithToppings[e].tranDetailItem.DetailNo;

                cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                recyclerview_listitemcart.SetLayoutManager(gridLayoutManager);
                recyclerview_listitemcart.HasFixedSize = true;
                recyclerview_listitemcart.SetItemViewCacheSize(20);
                recyclerview_listitemcart.SetAdapter(cart_Adapter_Item);
                cart_Adapter_Item.ItemClick += Cart_Adapter_Item_ItemClick;
                if (e > 4)
                {
                    recyclerview_listitemcart.ScrollToPosition(e);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Cart_Adapter_Item_ItemClick at Cartscan");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        public void ScanItemtoCart(Item itemscan)
        {
            try
            {
                SysItemFocus = itemscan.SysItemID;
                // โครงสร้าง item to TranDetailItemWithTopping
                //int quantity = DataCashing.setQuantityToCart;
                detailNo = (int)tranWithDetails.tranDetailItemWithToppings.Count;
                detailNo++;
                if (itemscan.FDisplayOption == 1)
                {
                    StartActivity(typeof(OptionActivity));
                    OptionActivity.POSDataItem = itemscan;
                    OptionActivity.ItemShopOption = itemscan.SysItemID;

                    int index = PosActivity.listItem.items.FindIndex(x => x.SysItemID == itemscan.SysItemID);
                    if (index != -1)
                    {
                        OptionActivity.PositionClick = PosActivity.listItem.items.FindIndex(x => x.SysItemID == itemscan.SysItemID);
                    }
                    else
                    {
                        OptionActivity.PositionClick = 0;
                    }
                    OptionActivity.SetTranDetail(tranWithDetails);
                }
                else
                {
                    TranDetailItemNew DetailItem = new TranDetailItemNew()
                    {
                        SysItemID = itemscan.SysItemID,
                        MerchantID = DataCashingAll.MerchantId,
                        SysBranchID = DataCashingAll.SysBranchId,
                        TranNo = tranWithDetails.tran.TranNo,
                        ItemName = itemscan.ItemName,
                        SaleItemType = itemscan.SaleItemType,
                        FProcess = 1,
                        TaxType = itemscan.TaxType,
                        Quantity = 1,
                        Price = itemscan.Price,
                        ItemPrice = itemscan.Price,
                        Discount = 0,
                        EstimateCost = itemscan.EstimateCost,
                        DetailNo = detailNo,
                        Comments = itemscan.Comments,
                        SizeName = "Default Size"

                    };

                    List<TranDetailItemTopping> tranDetailItem = new List<TranDetailItemTopping>();
                    TranDetailItemWithTopping tranDetailItemWithTopping = new TranDetailItemWithTopping()
                    {
                        tranDetailItem = DetailItem,
                        tranDetailItemToppings = tranDetailItem,
                    };

                    if (itemscan.SysItemID == 0)
                    {
                        return;
                    }

                    var row = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == itemscan.SysItemID);

                    tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, tranDetailItemWithTopping);
                    DataCashing.ModifyTranOrder = true;
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);

                    //เปลี่ยนสีเมื่อมีสินค้าลงตะกร้า
                    txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    lnPayment.SetBackgroundResource(Resource.Drawable.btnblue);

                    //คำนวณค่า
                    //SelectItemtoCart();
                    SetITemInCart();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void LnQuantityClick(long? itemID, decimal quantity)
        {
            try
            {
                DataCashing.flagEditQuantity = true;
                StartActivity(new Intent(Application.Context, typeof(QuantityActitvity)));
                QuantityActitvity.SetBackQuantity((Int32)quantity);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void LnOptionClick(TranWithDetailsLocal tranWithDetails, long? sysitemID, TranDetailItemNew tranDetailItem, Item item)
        {
            try
            {
                StartActivity(new Intent(Application.Context, typeof(OptionActivity)));
                DataCashing.flagEditOptionSize = true;
                DataCashing.flagEditOptionNote = true;
                DataCashing.flagEditOptionExtraTopping = true;
                OptionActivity.SetTranDetail(tranWithDetails);
                OptionActivity.SetSysItem(sysitemID);
                OptionActivity.SetItemDetail(tranDetailItem);
                OptionActivity.SetDataItem(item);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void LnPriceClick(TranDetailItemNew item, long? detailNo)
        {
            try
            {
                StartActivity(new Intent(Application.Context, typeof(ChangePriceActivity)));
                ChangePriceActivity.SetSysItem(item, tranWithDetails);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void LnDiscountClick(TranDetailItemNew tranDetail)
        {
            try
            {
                if (tranDetail != null)
                {
                    StartActivity(new Intent(Application.Context, typeof(AddDiscountActivity)));
                    AddDiscountActivity.SetTrtanDetailItem(tranDetail);
                    AddDiscountActivity.SetTranDetail(tranWithDetails);
                    AddDiscountActivity.CartDiscount = false;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void ItemFocus(long SysItemID)
        {
            try
            {
                if (SysItemID > 0)
                {
                    var index = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == SysItemID);
                    if (index == -1)
                    {
                        return;
                    }
                    else
                    {
                        recyclerview_listitemcart.ScrollToPosition(index);
                        SysItemID = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public void DialogShowItemStatusDClick(string fromEvent)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.cart_dialog_itemstatusd.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("fromPage", fromEvent);
                dialog.Arguments = bundle;
                dialog.Show(this.SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }
        }
    }
}