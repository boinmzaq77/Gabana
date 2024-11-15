using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.Phone;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class CartActivity : AppCompatActivity
    {
        public static CartActivity cart;
        private static RecyclerView recyclerview_listitemcart;
        public static TranWithDetailsLocal tranWithDetails;
        static LinearLayout lnNoCustomer, lnHaveCustomer, lnShowcart, lnCartdetail, lnShowDiscount, frameEmptyCart, lnDiscountMember, lnShowService, lnVat;
        static TextView txtNameCustomer, textCartService;
        static ImageButton btnCustomer;
        public static long DetailNo = 0; // มีการเรียกใช้ข้างนอก
        public static long DetailNoOld = 0; // มีการเรียกใช้ข้างนอก
        static ImageView imgShowOption, imgShowOption2;
        private FrameLayout frameShowOption, frameClickOption;
        public FrameLayout  lnRemark; // มีการเรียกใช้ข้างนอก
        static TextView textRemark;
        static Button btnCheckOut;
        private bool flagShowEditOption = true;
        public bool flagEditQuantity;
        public int? EditQuantity;
        public bool clearcart;
        public static bool  addDiscount;
        public TextView textTotal, textNumVat, textVat, textNumService;
        TextView textMember, textNumMember, textCartDiscount, textNumDiscount;
        public string CartDiscount = "";
        static Customer selectCus;
        private string CURRENCYSYMBOLS;
        public static bool CurrentActivity, openlstTopping = false, IsActive = false;
        public static List<string> lstSysItemIdStatusD = new List<string>();
        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;
        ImageButton btnBack;
        LinearLayout lnBack;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.cart_activity_main);

                InitialUIElement();

                cart = this; 
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS ?? "฿";

                await CheckJwt();
                Log.Debug("swipapp", "Cart OnCreate");
                SetCustomer();
                SetITemInCart();
                CurrentActivity = true;

                #region Update 08/09/2565 ถ้าไม่มีสินค้าไม่ต้องแสดง
                //Update 08/09/2565 ถ้าไม่มีสินค้าไม่ต้องแสดง
                //if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                //{
                //frameEmptyCart.Visibility = ViewStates.Visible;
                //lnShowcart.Visibility = ViewStates.Gone;
                //lnCartdetail.Visibility = ViewStates.Gone;
                //btnCheckOut.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                //btnCheckOut.SetBackgroundResource(Resource.Drawable.btnborderblue);
                //} 
                #endregion

                if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                {
                    frameEmptyCart.Visibility = ViewStates.Visible;
                    lnShowcart.Visibility = ViewStates.Gone;
                    lnCartdetail.Visibility = ViewStates.Gone;
                    frameShowOption.Visibility = ViewStates.Invisible;
                }

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
                _ = TinyInsights.TrackPageViewAsync("OnCreate : CartActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Cart");
            }
        }

        private void InitialUIElement()
        {
            textMember = FindViewById<TextView>(Resource.Id.textMember);
            textNumMember = FindViewById<TextView>(Resource.Id.textNumMember);
            textCartDiscount = FindViewById<TextView>(Resource.Id.textCartDiscount);
            textNumDiscount = FindViewById<TextView>(Resource.Id.textNumDiscount);
            textVat = FindViewById<TextView>(Resource.Id.textVat);
            textNumVat = FindViewById<TextView>(Resource.Id.textNumVat);
            textTotal = FindViewById<TextView>(Resource.Id.textTotal);

            btnCheckOut = FindViewById<Button>(Resource.Id.btnCheckOut);
            btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
            lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnCartdetail = FindViewById<LinearLayout>(Resource.Id.lnCartdetail);

            frameShowOption = FindViewById<FrameLayout>(Resource.Id.frameShowOption);
            frameEmptyCart = FindViewById<LinearLayout>(Resource.Id.frameEmptyCart);
            frameClickOption = FindViewById<FrameLayout>(Resource.Id.frameClickOption);
            imgShowOption = FindViewById<ImageView>(Resource.Id.imgShowOption);
            imgShowOption2 = FindViewById<ImageView>(Resource.Id.imgShowOption2);

            btnCustomer = FindViewById<ImageButton>(Resource.Id.btnCustomer);
            lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
            lnShowcart = FindViewById<LinearLayout>(Resource.Id.lnShowcart);
            lnRemark = FindViewById<FrameLayout>(Resource.Id.lnRemark);
            lnDiscountMember = FindViewById<LinearLayout>(Resource.Id.lnDiscountMember);
            lnShowService = FindViewById<LinearLayout>(Resource.Id.lnShowService);
            lnVat = FindViewById<LinearLayout>(Resource.Id.lnVat);
            lnShowDiscount = FindViewById<LinearLayout>(Resource.Id.lnShowDiscount);
            txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
            textRemark = FindViewById<TextView>(Resource.Id.textRemark);
            textCartService = FindViewById<TextView>(Resource.Id.textCartService);
            textNumService = FindViewById<TextView>(Resource.Id.textNumService);
            recyclerview_listitemcart = FindViewById<RecyclerView>(Resource.Id.recyclerview_listitemcart);
            btnCustomer.Click += BtnCustomer_Click;
            lnNoCustomer.Click += BtnCustomer_Click;
            lnHaveCustomer.Click += BtnCustomer_Click;

            btnCheckOut.Click += BtnCheckOut_Click;
            btnBack.Click += BtnBack_Click;
            lnBack.Click += BtnBack_Click;

            frameClickOption.Click += FrameClickOption_Click;
            imgShowOption.Click += FrameClickOption_Click;
            imgShowOption2.Click += FrameClickOption_Click;
        }

        private async Task SetCustomer()
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

                    UpdateDiscountinfomation();

                    UpdateVatinfomation();

                    UpdateServicechargeinfomation();

                    UpdateRemarkInfomation();

                    UpdateCartAdapter();

                    UpdateFrameandCheckoutButton();
                }
                else
                {
                    Cart_Adapter_Item  cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
                    SetupRecyclerView(cart_Adapter_Item);

                    frameEmptyCart.Visibility = ViewStates.Visible;
                    lnShowcart.Visibility = ViewStates.Gone;
                    lnCartdetail.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SetITemInCart at Cart");
            }
        }

        private void UpdateFrameandCheckoutButton()
        {
            if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
            {
                frameEmptyCart.Visibility = ViewStates.Visible;
                lnShowcart.Visibility = ViewStates.Gone;
                lnCartdetail.Visibility = ViewStates.Gone;
                frameShowOption.Visibility = ViewStates.Invisible;
            }
            else
            {
                frameEmptyCart.Visibility = ViewStates.Gone;
                frameShowOption.Visibility = ViewStates.Visible;
                lnShowcart.Visibility = ViewStates.Visible;
                lnCartdetail.Visibility = ViewStates.Visible;

                if (lstSysItemIdStatusD.Count > 0)
                {
                    btnCheckOut.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                    btnCheckOut.SetBackgroundResource(Resource.Drawable.btnborderblue);
                }
                else
                {
                    btnCheckOut.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
                    btnCheckOut.SetBackgroundResource(Resource.Drawable.btnblue);
                }
            }
        }

        private void UpdateCartAdapter()
        {
            Cart_Adapter_Item cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
            SetupRecyclerView(cart_Adapter_Item);
        }

        private void SetupRecyclerView(Cart_Adapter_Item cart_Adapter_Item)
        {
            GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
            recyclerview_listitemcart.SetLayoutManager(gridLayoutManager);
            recyclerview_listitemcart.HasFixedSize = true;
            recyclerview_listitemcart.SetItemViewCacheSize(20);
            recyclerview_listitemcart.SetAdapter(cart_Adapter_Item);
            cart_Adapter_Item.ItemClick += Cart_Adapter_Item_ItemClick;
        }

        private void UpdateRemarkInfomation()
        {
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

        private void UpdateServicechargeinfomation()
        {
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
        }

        private void UpdateVatinfomation()
        {
            // Vat
            string getvat = Utils.DisplayDouble(tranWithDetails.tran.TaxRate == null ? 0 : tranWithDetails.tran.TaxRate.Value);
            if (tranWithDetails.tran.TotalVat == 0)
            {
                lnVat.Visibility = ViewStates.Gone;
            }

            textVat.Text = GetString(Resource.String.vat) + " " + getvat + "%";

            textNumVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);

            textTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
        }

        private void UpdateDiscountinfomation()
        {
            //Discount From Member 
            decimal disMember = 0;
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
                    disMember = tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat;
                    textNumMember.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                }
                else
                {
                    lnDiscountMember.Visibility = ViewStates.Gone;
                }
            }

            //Discount From Discount
            decimal discount;
            decimal disDiscont = 0;
            var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
            if (tranTradDiscount != null)
            {
                lnShowDiscount.Visibility = ViewStates.Visible;
                var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                if (check == -1)
                {
                    CartDiscount = tranTradDiscount.FmlDiscount;
                    discount = Convert.ToDecimal(CartDiscount);
                    textCartDiscount.Text = GetString(Resource.String.discount) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount));
                }
                else
                {

                    discount = Convert.ToDecimal(tranTradDiscount.FmlDiscount.Remove(check));
                    textCartDiscount.Text = GetString(Resource.String.discount) + " " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount.Remove(check))) + "%";
                    discount = discount / 100;
                }

                disDiscont = tranTradDiscount.TradeDiscHaveVat + tranTradDiscount.TradeDiscNoneVat;
                textNumDiscount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disDiscont));
            }
            else
            {
                lnShowDiscount.Visibility = ViewStates.Gone;
            }
        }

        private async void FrameClickOption_Click(object sender, EventArgs e)
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
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SetITemInCart at Cart");
            }
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
        }

        private void Cart_Adapter_Item_ItemClick(object sender, int e)
        {
            DetailNo = tranWithDetails.tranDetailItemWithToppings[e].tranDetailItem.DetailNo;

            Cart_Adapter_Item cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
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

        public override void OnBackPressed()
        {
            DetailNo = 0;
            CurrentActivity = false;
            PosActivity.SetTranDetail(tranWithDetails);
            IsActive = false;
            this.Finish();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private void BtnCheckOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstSysItemIdStatusD.Count > 0)
                {
                    return;
                }
                                
                decimal amount = Convert.ToDecimal(tranWithDetails.tran.GrandTotal - tranWithDetails.tranPayments.Sum(x => x.PaymentAmount));

                if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                {
                    return;
                }

                HandleCheckout(amount);

                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("BtnCheckOut at Cart");
            }
        }

        private void HandleCheckout(decimal amount)
        {
            if (Decimal.Compare(amount, 0) <= 0)
            {
                StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));
                BalanceActivity.SetTranDetail(tranWithDetails);

            }
            else
            {
                DetailNo = 0;
                StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                PaymentActivity.SetTranDetail(tranWithDetails);
                IsActive = false;
            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                CheckJwt();
                Log.Debug("swipapp", "Cart OnResume");
                IsActive = true;
                CurrentActivity = true;

                SetCustomer();
                SetITemInCart();
                UpdateCartVisibility();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("resume at Cart");                
            }
        }

        private void UpdateCartVisibility()
        {
            if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
            {
                frameEmptyCart.Visibility = ViewStates.Visible;
                lnShowcart.Visibility = ViewStates.Gone;
                lnCartdetail.Visibility = ViewStates.Gone;
                frameShowOption.Visibility = ViewStates.Invisible;
            }
            else
            {
                lnShowcart.Visibility = ViewStates.Visible;
                frameEmptyCart.Visibility = ViewStates.Gone;
            }
        }

        private static void ItemFocus()
        {
            try
            {
                var index = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.DetailNo == DetailNo);
                if (index == -1)
                {
                    return;
                }
                else
                {
                    recyclerview_listitemcart.ScrollToPosition(index);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Item");
                Toast.MakeText(cart, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(Application.Context, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at Cart");
                return;
            }
        }

        async Task CheckJwt()
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

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                textMember?.Dispose();
                textNumMember?.Dispose();
                textCartDiscount?.Dispose();
                textNumDiscount?.Dispose();
                textVat?.Dispose();
                textNumVat?.Dispose();
                textTotal?.Dispose();
                btnCheckOut?.Dispose();
                btnBack?.Dispose();
                lnBack?.Dispose();
                lnCartdetail?.Dispose();
                frameShowOption?.Dispose();
                frameEmptyCart?.Dispose();
                frameClickOption?.Dispose();
                imgShowOption?.Dispose();
                imgShowOption2?.Dispose();
                btnCustomer?.Dispose();
                lnNoCustomer?.Dispose();
                lnHaveCustomer?.Dispose();
                lnShowcart?.Dispose();
                //lnRemark?.Dispose();
                lnDiscountMember?.Dispose();
                lnShowService?.Dispose();
                lnVat?.Dispose();
                lnShowDiscount?.Dispose();
                txtNameCustomer?.Dispose();
                textRemark?.Dispose();
                textCartService?.Dispose();
                textNumService?.Dispose();
                recyclerview_listitemcart?.Dispose();

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnDestroy at Cart");
            }
        }
    }
}

