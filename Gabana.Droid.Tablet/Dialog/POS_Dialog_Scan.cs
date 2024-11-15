using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using EDMTDev.ZXingXamarinAndroid;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Java.Lang.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class POS_Dialog_Scan :  AndroidX.Fragment.App.DialogFragment, IPermissionListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static POS_Dialog_Scan NewInstance()
        {
            var frag = new POS_Dialog_Scan { Arguments = new Bundle() };
            return frag;
        }

        View view;
        public static POS_Dialog_Scan scan;
        public static long DetailNoOld = 0;
        public static bool  openlstTopping = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_scan, container, false);
            try
            {
                scan = this;
                tranWithDetails = MainActivity.tranWithDetails;
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();

                //Request Permission
                Dexter.WithActivity(this.Activity)
                      .WithPermission(Manifest.Permission.Camera)
                      .WithListener(scan)
                      .Check();

                _ = TinyInsights.TrackPageViewAsync("OnCreateView : POS_Dialog_Scan");
                return view;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at POS Dialog Scan");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }


        public static ZXingScannerView ScannerView;
        LinearLayout lnBack, lnBody;
        RecyclerView rcvListItem;
        LinearLayout lnCartdetail;
        FrameLayout lnRemark;
        public TextView textRemark, textTotal;
        LinearLayout lnDiscountMember;
        TextView textMember, textNumMember;
        LinearLayout lnShowDiscount;
        TextView textCartDiscount, textNumDiscount;
        LinearLayout lnVat;
        public TextView textVat, textNumVat;
        LinearLayout lnShowService;
        public TextView textCartService, textNumService;
        FrameLayout frameShowOption, lnOption;
        LinearLayout lnPayment;
        TextView txtSumary, txtNoItem;
        private void CombinUI()
        {
            ScannerView = view.FindViewById<ZXingScannerView>(Resource.Id.zxscan);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBody = view.FindViewById<LinearLayout>(Resource.Id.lnBody);
            rcvListItem = view.FindViewById<RecyclerView>(Resource.Id.rcvListItem);
            lnCartdetail = view.FindViewById<LinearLayout>(Resource.Id.lnCartdetail);
            lnRemark = view.FindViewById<FrameLayout>(Resource.Id.lnRemark);
            textRemark = view.FindViewById<TextView>(Resource.Id.textRemark);
            textTotal = view.FindViewById<TextView>(Resource.Id.textTotal);
            lnDiscountMember = view.FindViewById<LinearLayout>(Resource.Id.lnDiscountMember);
            textMember = view.FindViewById<TextView>(Resource.Id.textMember);
            textNumMember = view.FindViewById<TextView>(Resource.Id.textNumMember);
            lnShowDiscount = view.FindViewById<LinearLayout>(Resource.Id.lnShowDiscount);
            textCartDiscount = view.FindViewById<TextView>(Resource.Id.textCartDiscount);
            textNumDiscount = view.FindViewById<TextView>(Resource.Id.textNumDiscount);
            lnVat = view.FindViewById<LinearLayout>(Resource.Id.lnVat);
            textVat = view.FindViewById<TextView>(Resource.Id.textVat);
            textNumVat = view.FindViewById<TextView>(Resource.Id.textNumVat);
            lnShowService = view.FindViewById<LinearLayout>(Resource.Id.lnShowService);
            textCartService = view.FindViewById<TextView>(Resource.Id.textCartService);
            textNumService = view.FindViewById<TextView>(Resource.Id.textNumService);
            frameShowOption = view.FindViewById<FrameLayout>(Resource.Id.frameShowOption);
            lnOption = view.FindViewById<FrameLayout>(Resource.Id.lnOption);
            lnPayment = view.FindViewById<LinearLayout>(Resource.Id.lnPayment);
            txtSumary = view.FindViewById<TextView>(Resource.Id.txtSumary);
            txtNoItem = view.FindViewById<TextView>(Resource.Id.txtNoItem);

            lnBack.Click += LnBack_Click;
            lnPayment.Click += BtnPayment_Click;
            lnOption.Click += LnOption_Click;
        }  

        private void LnOption_Click(object sender, EventArgs e)
        {
            try
            {
                if (tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    Cart_Dialog_Option dialog = new Cart_Dialog_Option();
                    var fragment = new Cart_Dialog_Option();
                    fragment.Show(this.Activity.SupportFragmentManager, nameof(Cart_Dialog_Option));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FrameClickOption_Click at Cartscan");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, "ไม่มียอดที่ต้องชำระ", ToastLength.Short).Show();
                    return;
                }

                if (tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    detailNoClickOption = 0;
                    StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                    this.Dismiss();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPayment_Click at Cartscan");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnDismiss(this.Dialog);
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            try
            {
                detailNoClickOption = 0;
                MainActivity.tranWithDetails = tranWithDetails;
                this.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPayment_Click at Cartscan");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            ScannerView.SetResultHandler(new MyResultHandler(this));
            ScannerView.SetAutoFocus(true);
            ScannerView.SetLaserColor(Color.LightBlue);
            ScannerView.SetBorderColor(Color.Transparent);
            ScannerView.StartCamera();
        }
        private class MyResultHandler : AppCompatActivity, IResultHandler
        {
            private POS_Dialog_Scan dialog_scan;

            public MyResultHandler(POS_Dialog_Scan dialog_Scan)
            {
                this.dialog_scan = dialog_Scan;
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
                        Toast.MakeText(Application.Context, this.dialog_scan.GetString(Resource.String.notdata), ToastLength.Short).Show();
                        //Toast.MakeText(Application.Context, "testtt", ToastLength.Short).Show();
                        Dexter.WithActivity(scan.Activity)
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
                    ScannerView.SetResultHandler(new MyResultHandler(scan));
                    //แสดงข้อมูลที่หน้าจอ
                    //scan.ReloadData(Itemresult[0]);
                    //เพิ่มข้อมูลไปที่ Tran
                    scan.ScanItemtoCart(Itemresult[0]);

                }
                catch (Exception ex)
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                    _ = TinyInsights.TrackPageViewAsync("HandleResult at Cartscan");
                    Toast.MakeText(this.Application, ex.Message, ToastLength.Short).Show();
                    Dexter.WithActivity(this)
                        .WithPermission(Manifest.Permission.Camera)
                        .WithListener(scan)
                        .Check();
                }
            }
        }
        public static long SysItemFocus = 0;
        public static TranWithDetailsLocal tranWithDetails;
        public static int detailNo = 0;

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

                    var fragment = new POS_Dialog_Option();
                    POS_Dialog_Option dialog = new POS_Dialog_Option();
                    fragment.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_Option));

                    POS_Dialog_Option.POSDataItem = itemscan;
                    POS_Dialog_Option.ItemShopOption = itemscan.SysItemID;

                    int index = POS_Fragment_Main.listItem.items.FindIndex(x => x.SysItemID == itemscan.SysItemID);
                    if (index != -1)
                    {
                        POS_Dialog_Option.PositionClick = POS_Fragment_Main.listItem.items.FindIndex(x => x.SysItemID == itemscan.SysItemID);
                    }
                    else
                    {
                        POS_Dialog_Option.PositionClick = 0;
                    }
                    POS_Dialog_Option.dialog_Option.OnResume();
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
                    txtSumary.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    lnPayment.SetBackgroundResource(Resource.Drawable.btnBluerectangle);

                    //คำนวณค่า
                    //SelectItemtoCart();
                    SetITemInCart();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        string CURRENCYSYMBOLS;
        public double total = 0.00;
        public static bool addDiscount;
        public string CartDiscount = "";
        Cart_Adapter_Item cart_Adapter_Item; //Body Show Row Menu
        public static List<string> lstSysItemIdStatusD = new List<string>();

        public async void SetITemInCart()
        {
            try
            {
                if (tranWithDetails.tran != null)
                {
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                    {
                        txtSumary.Text = "0.00 " + CURRENCYSYMBOLS;
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
                        txtSumary.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        lnPayment.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                    }
                    else
                    {
                        var quantity = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Quantity);
                        txtSumary.Text = Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.SubAmount)) + " " + CURRENCYSYMBOLS;

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
                        txtSumary.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                        lnPayment.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
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
                        if (disDiscont > 0)
                        {
                            lnShowDiscount.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            lnShowDiscount.Visibility = ViewStates.Gone;
                        }
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
                    else
                    {
                        lnVat.Visibility = ViewStates.Visible;
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
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListItem.SetLayoutManager(gridLayoutManager);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(20);
                rcvListItem.SetAdapter(cart_Adapter_Item);
                cart_Adapter_Item.ItemClick += Cart_Adapter_Item_ItemClick;

                if (lstSysItemIdStatusD.Count > 0 && tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    txtSumary.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    lnPayment.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                }
                else
                {
                    txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    txtSumary.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    lnPayment.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                }

                //scrolltoposition
                if (cart_Adapter_Item.ItemCount > 0)
                {
                    //int index = tranWithDetails.tranDetailItemWithToppings.FindIndex(tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == ));
                    //rcvlistitemcart.SmoothScrollToPosition(cart_Adapter_Item.ItemCount - 1);
                    rcvListItem.ScrollToPosition(cart_Adapter_Item.ItemCount - 1);
                }

                ItemFocus(SysItemFocus);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SetITemInCart at Cart");
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
                        rcvListItem.ScrollToPosition(index);
                        SysItemID = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public static int detailNoClickOption = 0;

        private async void Cart_Adapter_Item_ItemClick(object sender, int e)
        {
            try
            {
                if (e == -1)
                {
                    return;
                }
                detailNoClickOption = (int)tranWithDetails.tranDetailItemWithToppings[e].tranDetailItem.DetailNo;

                cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvListItem.SetLayoutManager(gridLayoutManager);
                rcvListItem.HasFixedSize = true;
                rcvListItem.SetItemViewCacheSize(20);
                rcvListItem.SetAdapter(cart_Adapter_Item);
                cart_Adapter_Item.ItemClick += Cart_Adapter_Item_ItemClick;
                if (e > 4)
                {
                    rcvListItem.ScrollToPosition(e);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Cart_Adapter_Item_ItemClick at Cartscan");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void CallDailog(List<Item> itemresult)
        {
            try
            {
                var fragment = new POS_Dialog_ScanItem();
                POS_Dialog_ScanItem dialog = new POS_Dialog_ScanItem();
                fragment.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_ScanItem));
                dialog.Cancelable = false;
                POS_Dialog_ScanItem.SetListItem(itemresult);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CallDailog at Cartscan");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                                             

                Dexter.WithActivity(this.Activity)
                        .WithPermission(Manifest.Permission.Camera)
                        .WithListener(scan)
                        .Check();

                tranWithDetails = MainActivity.tranWithDetails;
                List<string> lstSysItemIdStatusD = new List<string>();
                lstSysItemIdStatusD = Utils.CheckStatusIteminCart(tranWithDetails);
                if (lstSysItemIdStatusD.Count > 0)
                {
                    Toast.MakeText(this.Context, "lstSysItemIdStatusD.Count > 0", ToastLength.Short).Show();
                }

                SetITemInCart();                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Cartscan");
            }
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this.Activity, "You Must Enable Permission", ToastLength.Long).Show();
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {

        }
    }

}