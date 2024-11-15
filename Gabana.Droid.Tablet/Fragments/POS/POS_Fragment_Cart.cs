using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Adapter;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.POS
{
    public class POS_Fragment_Cart : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static POS_Fragment_Cart NewInstance()
        {
            POS_Fragment_Cart frag = new POS_Fragment_Cart();
            return frag;
        }

        public static POS_Fragment_Cart fragment_cart;
        View view;
        public static TranWithDetailsLocal tranWithDetails ;
        public static bool openlstTopping = false, IsActive = false;
        public static bool addRemark, addDiscount;

        public override  View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_fragment_cart, container, false);
            try
            {
                fragment_cart = this;
                tranWithDetails = MainActivity.tranWithDetails;
                CheckJwt();
                CombineUI();
                SetCustomer();
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                {
                    //ไม่แสดงง
                    frameEmptyCart.Visibility = ViewStates.Visible;
                    lnShowcart.Visibility = ViewStates.Gone;
                    lnCartdetail.Visibility = ViewStates.Gone;
                    frameShowOption.Visibility = ViewStates.Invisible;
                }
                else
                {
                    frameEmptyCart.Visibility = ViewStates.Gone;
                    lnShowcart.Visibility = ViewStates.Visible;
                    lnCartdetail.Visibility = ViewStates.Visible;
                    frameShowOption.Visibility = ViewStates.Visible;
                }

                if (addRemark)
                {
                    lnRemark.Visibility = ViewStates.Visible;
                    textRemark.Text = tranWithDetails.tran.Comments;
                }
                else
                {
                    lnRemark.Visibility = ViewStates.Gone;
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreateView : POS_Fragment_Cart");
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }
        public static TextView textMember, textNumMember, textCartDiscount, textNumDiscount, textVat, textNumVat, textTotal , txtNameCustomer,
                        textRemark, textCartService, textNumService;
        Button btnCheckOut;
        public static LinearLayout lnCartdetail, frameEmptyCart, lnNoCustomer, lnHaveCustomer, lnShowcart, lnDiscountMember, lnShowService,
                        lnVat,lnShowDiscount;
        FrameLayout frameShowOption, lnRemark, frameClickOption;
        ImageView imgShowOption, imgShowOption2;
        ImageButton btnCustomer;
        public static RecyclerView rcvlistitemcart;
        public static long DetailNo = 0;
        public static long DetailNoOld = 0;
        public double total = 0.00;
        static Customer selectCus;
        public string CartDiscount = "";
        public string CartDiscountMember = "", CURRENCYSYMBOLS;
        public static bool flagShowEditOption = true;
        public static bool flagEditQuantity;
        List<string> lstSysItemIdStatusD = new List<string>();

        private void CombineUI()
        {
            textMember = view.FindViewById<TextView>(Resource.Id.textMember);
            textNumMember = view.FindViewById<TextView>(Resource.Id.textNumMember);
            textCartDiscount = view.FindViewById<TextView>(Resource.Id.textCartDiscount);
            textNumDiscount = view.FindViewById<TextView>(Resource.Id.textNumDiscount);
            textVat = view.FindViewById<TextView>(Resource.Id.textVat);
            textNumVat = view.FindViewById<TextView>(Resource.Id.textNumVat);
            textTotal = view.FindViewById<TextView>(Resource.Id.textTotal);

            btnCheckOut = view.FindViewById<Button>(Resource.Id.btnCheckOut);
            lnCartdetail = view.FindViewById<LinearLayout>(Resource.Id.lnCartdetail);

            frameShowOption = view.FindViewById<FrameLayout>(Resource.Id.frameShowOption);
            frameEmptyCart = view.FindViewById<LinearLayout>(Resource.Id.frameEmptyCart);
            frameClickOption = view.FindViewById<FrameLayout>(Resource.Id.frameClickOption);
            imgShowOption = view.FindViewById<ImageView>(Resource.Id.imgShowOption);
            imgShowOption2 = view.FindViewById<ImageView>(Resource.Id.imgShowOption2);

            btnCustomer = view.FindViewById<ImageButton>(Resource.Id.btnCustomer);
            lnNoCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = view.FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
            lnShowcart = view.FindViewById<LinearLayout>(Resource.Id.lnShowcart);
            lnRemark = view.FindViewById<FrameLayout>(Resource.Id.lnRemark);
            lnDiscountMember = view.FindViewById<LinearLayout>(Resource.Id.lnDiscountMember);
            lnShowService = view.FindViewById<LinearLayout>(Resource.Id.lnShowService);
            lnVat = view.FindViewById<LinearLayout>(Resource.Id.lnVat);
            lnShowDiscount = view.FindViewById<LinearLayout>(Resource.Id.lnShowDiscount);
            txtNameCustomer = view.FindViewById<TextView>(Resource.Id.txtNameCustomer);
            textRemark = view.FindViewById<TextView>(Resource.Id.textRemark);
            textCartService = view.FindViewById<TextView>(Resource.Id.textCartService);
            textNumService = view.FindViewById<TextView>(Resource.Id.textNumService);
            rcvlistitemcart = view.FindViewById<RecyclerView>(Resource.Id.rcvlistitemcart);
            btnCustomer.Click += BtnCustomer_Click;
            lnNoCustomer.Click += BtnCustomer_Click;
            lnHaveCustomer.Click += BtnCustomer_Click;
            btnCheckOut.Click += BtnCheckOut_Click;
            frameClickOption.Click += FrameClickOption_Click;
            imgShowOption.Click += FrameClickOption_Click;
            imgShowOption2.Click += FrameClickOption_Click;

        }
        private async void FrameClickOption_Click(object sender, EventArgs e)
        {
            try
            {
                frameClickOption.Enabled = false; 
                if (tranWithDetails.tranDetailItemWithToppings.Count > 0)
                {
                    var fragment = new Cart_Dialog_Option();
                    fragment.Cancelable = false;
                    fragment.Show(Activity.SupportFragmentManager, nameof(Cart_Dialog_Option));
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SetITemInCart at Cart");
            }
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

                DetailNo = 0;
                StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                IsActive = false;

                #region Old Code
                //DetailNo = 0;
                //IsActive = false;
                //if (PaymentActivity.payment_main == null)
                //{
                //    StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                //    return;
                //}
                //else
                //{
                //    decimal amount = 0;
                //    decimal paymentAmount = 0;
                //    paymentAmount = tranWithDetails.tranPayments.Sum(x => x.PaymentAmount);
                //    amount = Convert.ToDecimal(tranWithDetails.tran.GrandTotal - paymentAmount);

                //    if (tranWithDetails.tranDetailItemWithToppings.Count > 0)
                //    {
                //        IsActive = false;
                //        if (amount <= 0)
                //        {
                //            BalanceActivity
                //            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "balance", "default");
                //        }
                //        else
                //        {
                //            PaymentActivity
                //            PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "default");
                //        }
                //    }
                //} 
                #endregion

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                Cart_Dailog_Customer dialog = new Cart_Dailog_Customer();
                Cart_Dailog_Customer.SetTranWithDetail(tranWithDetails);
                var fragment = new Cart_Dailog_Customer();
                fragment.Cancelable = false;
                fragment.Show(Activity.SupportFragmentManager, nameof(Cart_Dailog_Customer));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetCustomer()
        {
            try
            {
                if (DataCashing.SysCustomerID == 999)
                {
                    lnHaveCustomer.Visibility = ViewStates.Gone;
                    lnNoCustomer.Visibility = ViewStates.Visible;
                }
                else
                {
                    lnHaveCustomer.Visibility = ViewStates.Visible;
                    lnNoCustomer.Visibility = ViewStates.Gone;
                    txtNameCustomer.Text = tranWithDetails.tran.CustomerName?.ToString();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }

        public async void SetITemInCart()
        {
            try
            {
                if (tranWithDetails.tran != null)
                {
                    #region old Code
                    //total = Convert.ToDouble(tranWithDetails.tran.GrandTotal);

                    ////Discount From Member 
                    //double disMember = 0;
                    //if (DataCashing.SysCustomerID == 999)
                    //{
                    //    lnDiscountMember.Visibility = ViewStates.Gone;
                    //}
                    //else
                    //{
                    //    var tranTradDiscountMember = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "PS").FirstOrDefault();
                    //    if (tranTradDiscountMember != null)
                    //    {
                    //        var check = tranTradDiscountMember.FmlDiscount.IndexOf('%');
                    //        if (check == -1)
                    //        {
                    //            textMember.Text = "Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Replace("%", "")));
                    //        }
                    //        else
                    //        {
                    //            textMember.Text = "Member " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscountMember.FmlDiscount.Remove(check))) + "%";
                    //        }
                    //        lnDiscountMember.Visibility = ViewStates.Visible;
                    //        disMember = Convert.ToDouble(tranTradDiscountMember.TradeDiscNoneVat + tranTradDiscountMember.TradeDiscHaveVat);
                    //        textNumMember.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disMember));
                    //    }
                    //    else
                    //    {
                    //        lnDiscountMember.Visibility = ViewStates.Gone;
                    //    }
                    //}

                    ////Discount From Discount
                    //double discount;
                    //double disDiscont = 0.0;
                    //var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                    //if (tranTradDiscount != null)
                    //{
                    //    addDiscount = true;

                    //    var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                    //    if (check == -1)
                    //    {
                    //        CartDiscount = tranTradDiscount.FmlDiscount;
                    //        discount = Convert.ToDouble(CartDiscount);
                    //        textCartDiscount.Text = GetString(Resource.String.discount) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount));
                    //        disDiscont = discount;
                    //    }
                    //    else
                    //    {
                    //        discount = Convert.ToDouble(tranTradDiscount.FmlDiscount.Remove(check));
                    //        textCartDiscount.Text = GetString(Resource.String.discount) + " " + Utils.DisplayDouble(Convert.ToDecimal(tranTradDiscount.FmlDiscount.Remove(check))) + "%";
                    //        discount = discount / 100;
                    //        disDiscont = total * discount;
                    //    }

                    //    disDiscont = Convert.ToDouble(tranTradDiscount.TradeDiscHaveVat + tranTradDiscount.TradeDiscNoneVat);
                    //    if (disDiscont > 0)
                    //    {
                    //        lnShowDiscount.Visibility = ViewStates.Visible;
                    //    }
                    //    else
                    //    {
                    //        lnShowDiscount.Visibility = ViewStates.Gone;
                    //    }
                    //    textNumDiscount.Text = "-" + Utils.DisplayDecimal(Convert.ToDecimal(disDiscont));
                    //}
                    //else
                    //{
                    //    lnShowDiscount.Visibility = ViewStates.Gone;
                    //    addDiscount = false;
                    //}

                    //// Vat
                    //string getvat = Utils.DisplayDouble(tranWithDetails.tran.TaxRate == null ? 0 : tranWithDetails.tran.TaxRate.Value);
                    //if (tranWithDetails.tran.TotalVat == 0)
                    //{
                    //    lnVat.Visibility = ViewStates.Gone;
                    //}
                    //else
                    //{
                    //    lnVat.Visibility = ViewStates.Visible;
                    //}

                    //textVat.Text = GetString(Resource.String.vat) + " " + getvat + "%";

                    //var vat = "Include";
                    //total = total - (disMember + disDiscont);
                    //if (vat == "Include")
                    //{
                    //    textNumVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                    //}
                    //else
                    //{
                    //    textNumVat.Text = Utils.DisplayDouble(tranWithDetails.tran.TotalVat);
                    //}

                    //textTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);

                    ////Service charge
                    //string ServiceCharge;
                    //lnShowService.Visibility = ViewStates.Visible;
                    //if (string.IsNullOrEmpty(tranWithDetails.tran.FmlServiceCharge))
                    //{
                    //    lnShowService.Visibility = ViewStates.Gone;
                    //}
                    //else
                    //{
                    //    if (tranWithDetails.tran.FmlServiceCharge == "0" | tranWithDetails.tran.FmlServiceCharge == "0.00" | tranWithDetails.tran.FmlServiceCharge == "0.0000")
                    //    {
                    //        lnShowService.Visibility = ViewStates.Gone;
                    //    }

                    //    var checkservice = tranWithDetails.tran.FmlServiceCharge.IndexOf('%');
                    //    if (checkservice == -1)
                    //    {
                    //        ServiceCharge = tranWithDetails.tran.FmlServiceCharge;
                    //        textCartService.Text = GetString(Resource.String.servicecharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge));
                    //    }
                    //    else
                    //    {
                    //        string[] split = tranWithDetails.tran.FmlServiceCharge.Split('%');
                    //        ServiceCharge = split[0];
                    //        textCartService.Text = GetString(Resource.String.servicecharge) + " " + Utils.DisplayDouble(Convert.ToDecimal(ServiceCharge)) + "%";
                    //    }
                    //    textNumService.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                    //}

                    ////Remark 
                    //if (!string.IsNullOrEmpty(tranWithDetails.tran.Comments))
                    //{
                    //    addRemark = true;
                    //}
                    //else
                    //{
                    //    addRemark = false;
                    //}

                    //cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
                    //GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Context, 1, 1, false);
                    //rcvlistitemcart.SetLayoutManager(gridLayoutManager);
                    //rcvlistitemcart.HasFixedSize = true;
                    //rcvlistitemcart.SetItemViewCacheSize(20);
                    //rcvlistitemcart.SetAdapter(cart_Adapter_Item);
                    //cart_Adapter_Item.ItemClick += Cart_Adapter_Item_ItemClick;

                    //if (tranWithDetails.tranDetailItemWithToppings.Count == 0)
                    //{
                    //    //ไม่แสดงง
                    //    frameEmptyCart.Visibility = ViewStates.Visible;
                    //    lnShowcart.Visibility = ViewStates.Gone;
                    //    lnCartdetail.Visibility = ViewStates.Gone;
                    //    frameShowOption.Visibility = ViewStates.Invisible;
                    //}
                    //else
                    //{
                    //    frameEmptyCart.Visibility = ViewStates.Gone;
                    //    lnShowcart.Visibility = ViewStates.Visible;
                    //    lnCartdetail.Visibility = ViewStates.Visible;
                    //    frameShowOption.Visibility = ViewStates.Visible;
                    //}

                    ////scrolltoposition
                    //if (cart_Adapter_Item.ItemCount > 0)
                    //{
                    //    //int index = tranWithDetails.tranDetailItemWithToppings.FindIndex(tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == ));
                    //    //rcvlistitemcart.SmoothScrollToPosition(cart_Adapter_Item.ItemCount - 1);
                    //    rcvlistitemcart.ScrollToPosition(cart_Adapter_Item.ItemCount - 1);
                    //}
                    // 
                    #endregion

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
                    Cart_Adapter_Item cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
                    SetupRecyclerView(cart_Adapter_Item);

                    frameEmptyCart.Visibility = ViewStates.Visible;
                    lnShowcart.Visibility = ViewStates.Gone;
                    lnCartdetail.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SetITemInCart at Cart");
            }
        }

        private void Cart_Adapter_Item_ItemClick(object sender, int e)
        {
            if (e == -1)
            {
                return;
            }
            DetailNo = tranWithDetails.tranDetailItemWithToppings[e].tranDetailItem.DetailNo;

            Cart_Adapter_Item cart_Adapter_Item = new Cart_Adapter_Item(tranWithDetails);
            SetupRecyclerView(cart_Adapter_Item);
            if (e > 4)
            {
                rcvlistitemcart.ScrollToPosition(e);
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
                    rcvlistitemcart.ScrollToPosition(index);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ItemFocus at Item");
            }
        }
        
        public  override void OnResume()        
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                if (!IsAdded)
                {
                    return;
                }

                CheckJwt();
                tranWithDetails = MainActivity.tranWithDetails;

                #region ถ้ามีการแก้ไขตั้งค่า ตอนเลือกสินค้าใส่ตะกร้า เพิ่มเงื่อนไข check กรณีมีการแก้ไข VAT,Service Charge
                
                //Service Charge
                if (tranWithDetails.tran.FmlServiceCharge.ToString() != DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE.ToString())
                {
                    tranWithDetails.tran.FmlServiceCharge = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE;
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    MainActivity.tranWithDetails = tranWithDetails;
                }

                //vat
                decimal.TryParse(tranWithDetails.tran.TaxRate?.ToString(), out decimal tranvat);
                decimal.TryParse(DataCashingAll.setmerchantConfig.TAXRATE, out decimal merchantvat);
                if (tranvat != merchantvat)
                {
                    tranWithDetails.tran.TaxRate = merchantvat;
                    tranWithDetails.tran.TranTaxType = char.Parse(DataCashingAll.setmerchantConfig.TAXTYPE?.ToString());
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    MainActivity.tranWithDetails = tranWithDetails;
                } 
                #endregion

                //DetailNoOld = 0;
                //DetailNo = 0;
                IsActive = true;
                SetCustomer();
                SetITemInCart();
                UpdateCartVisibility();
                                
                frameClickOption.Enabled = true;

                if (addRemark)
                {
                    lnRemark.Visibility = ViewStates.Visible;
                    textRemark.Text = tranWithDetails.tran.Comments;
                }
                else
                {
                    lnRemark.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
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

        private async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
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
                    btnCheckOut.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                }
                else
                {
                    btnCheckOut.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
                    btnCheckOut.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
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
            GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Context, 1, 1, false);
            rcvlistitemcart.SetLayoutManager(gridLayoutManager);
            rcvlistitemcart.HasFixedSize = true;
            rcvlistitemcart.SetItemViewCacheSize(20);
            rcvlistitemcart.SetAdapter(cart_Adapter_Item);
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

    }
}