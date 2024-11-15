using Android.Accounts;
using Android.App;
using Android.Gms.Common.Server.Converter;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class DummyActivity : Activity
    {
        public static DummyActivity main;
        ImageButton imageButtonBack, btndeletenumber;
        TextView txtprice;
        EditText txtDescription;
        Button btnAddToCart, btnPoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0;
        string strValue, DummyDescription;
        int DECIMALPOINT = 2;
        public static TranWithDetailsLocal tranWithDetails;
        static int DetailNo;
        TranDetailItemWithTopping tranDetailItemWithTopping;
        private decimal dec;
        string DECIMALPOINTDISPLAY;

        public DummyActivity()
        {
            tranWithDetails = PosActivity.tranWithDetails;
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.dummy_activity_main);

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                imageButtonBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtprice = FindViewById<TextView>(Resource.Id.txtprice);
                txtDescription = FindViewById<EditText>(Resource.Id.txtDescription);
                btnPoint = FindViewById<Button>(Resource.Id.btnPoint);
                btnnumber0 = FindViewById<Button>(Resource.Id.btnnumber0);
                btnnumber1 = FindViewById<Button>(Resource.Id.btnnumber1);
                btnnumber2 = FindViewById<Button>(Resource.Id.btnnumber2);
                btnnumber3 = FindViewById<Button>(Resource.Id.btnnumber3);
                btnnumber4 = FindViewById<Button>(Resource.Id.btnnumber4);
                btnnumber5 = FindViewById<Button>(Resource.Id.btnnumber5);
                btnnumber6 = FindViewById<Button>(Resource.Id.btnnumber6);
                btnnumber7 = FindViewById<Button>(Resource.Id.btnnumber7);
                btnnumber8 = FindViewById<Button>(Resource.Id.btnnumber8);
                btnnumber9 = FindViewById<Button>(Resource.Id.btnnumber9);
                btnAddToCart = FindViewById<Button>(Resource.Id.buttonOK);
                lnBack.Click += ImageButtonBack_Click;
                imageButtonBack.Click += ImageButtonBack_Click;
                txtDescription.TextChanged += TxtDescription_TextChanged;
                btnnumber0.Click += Btnnumber0_Click;
                btnnumber1.Click += Btnnumber1_Click;
                btnnumber2.Click += Btnnumber2_Click;
                btnnumber3.Click += Btnnumber3_Click;
                btnnumber4.Click += Btnnumber4_Click;
                btnnumber5.Click += Btnnumber5_Click;
                btnnumber6.Click += Btnnumber6_Click;
                btnnumber7.Click += Btnnumber7_Click;
                btnnumber8.Click += Btnnumber8_Click;
                btnnumber9.Click += Btnnumber9_Click;
                btnPoint.Click += BtnPoint_Click;
                btndeletenumber.Click += Btndeletenumber_Click;
                btnAddToCart.Click += Addtocart_Click;

                CheckJwt();
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        //dec = (decimal)0.0001;
                        DECIMALPOINT = 4;
                    }
                    else
                    {
                        //dec = (decimal)0.01;
                        DECIMALPOINT = 2;
                    }
                }
                else
                {
                    //dec = (decimal)0.01;
                    DECIMALPOINT = 2;
                }

                txtprice.Hint = Utils.DisplayDecimal(0);
                strValue = txtprice.Text;
                SetBtnAddToCart();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : DummyActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Dummy");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void SetBtnAddToCart()
        {
            if (txtprice.Text != "")
            {
                btndeletenumber.Enabled = true;
                btnAddToCart.Enabled = true;
                btnAddToCart.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAddToCart.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btndeletenumber.Enabled = false;
                btnAddToCart.Enabled = false;
                btnAddToCart.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAddToCart.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
        }
        private void Addtocart_Click(object sender, EventArgs e)
        {
            if (tranWithDetails != null)
            {
                //สินค้าด่วน
                //ชื่อ ,ราคา ,จำนวน ,sysitemID = 0

                string Language = Preferences.Get("Language", "");

                if (string.IsNullOrEmpty(strValue))
                {
                    return;
                }

                if (string.IsNullOrEmpty(DummyDescription))
                {
                    if (Language == "th")
                    {
                        DummyDescription = "สินค้าด่วน";
                    }
                    else
                    {
                        DummyDescription = "Dummy";
                    }
                }

                Item item = new Item()
                {
                    ItemName = DummyDescription,
                    Price = Convert.ToDecimal(strValue),
                    SysItemID = 0
                };

                // add detail Item            
                SelectItemtoCart(item);

                DataCashing.flagDummy = true;

                PosActivity.SetTranDetail(tranWithDetails);
                //StartActivity(typeof(PosActivity));
                this.Finish();
            }
        }

        private void ImageButtonBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private async void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = strValue.Remove(strValue.Length - 1);
                    string str = "";
                    str = strValue;
                    #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                    //txtprice.Text = Utils.DisplayDecimal(Convert.ToDecimal(str)); 
                    #endregion
                    txtprice.Text =  Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                    //txtprice.Text = (Convert.ToDecimal(strValue)).ToString("#,##0.00"); //ใส่เพื่อแสดงจุดทศนิยมและศูนย์ศูนย์ .00 ต่อท้ายตัวเลขที่กรอก
                    //txtprice.Text = Utils.DisplayComma(strValue); //ใส่เพื่อแสดงจุดทศนิยมและศูนย์ศูนย์ .00 ต่อท้ายตัวเลขที่กรอก 
                    #endregion
                    txtprice.Text = "";
                    txtprice.Hint = Utils.DisplayDecimal(0);                    
                    txtprice.Focusable = true;
                }   

                SetBtnAddToCart();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btndeletenumber_Click at Dummy");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        bool clickPoint = false;
        private void BtnPoint_Click(object sender, EventArgs e)
        {
            clickPoint = true;
            SetValue(btnPoint);
        }

        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber0);
        }

        private void TxtDescription_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            DummyDescription = txtDescription.Text;
        }

        public void SetValue(Button btn)
        {
            try
            {
                string str = "";
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtprice.Text = strValue;
                }

                if (clickPoint)
                {
                    #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                    //if (DECIMALPOINT == 4)
                    //{
                    //    if (strValue.Contains(".0000"))
                    //    {
                    //        var val = strValue.Split(".0000");
                    //        strValue = val[0];
                    //    }
                    //}
                    //else
                    //{
                    //    if (strValue.Contains(".00"))
                    //    {
                    //        var val = strValue.Split(".00");
                    //        strValue = val[0];
                    //    }
                    //}      
                    #endregion
                    strValue += ".";
                    var data = Utils.AllIndexOf(strValue, ".", StringComparison.Ordinal);
                    if (data.Count > 1)
                    {
                        strValue = strValue.Substring(0, strValue.Length - 1);
                        clickPoint = false;
                        return;
                    }
                    str = strValue;
                    clickPoint = false;
                }
                else
                {
                    if (strValue.Contains("."))
                    {
                        var val = strValue.Split(".");
                        var sp = val[1];
                        if (sp.Length == DECIMALPOINT)
                        {
                            return;
                        }
                    }
                    str = strValue + btn.Text.ToString();
                }
                strValue = str;

                #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                //txtprice.Text = Utils.DisplayDecimal(Convert.ToDecimal(str)); // (Convert.ToDouble(str)).ToString("#,##0.00"); 
                #endregion

                txtprice.Text = Utils.DisplayComma(str); 

                //Currency
                // 6 หลัก  9,999,999,999,999.displayDecimal  
                string maxdata;
                if (DECIMALPOINTDISPLAY == "4")
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.9999);
                }
                else
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.99);
                }

                if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                {
                    Toast.MakeText(this, GetString(Resource.String.maxservicecharg) + " " + maxdata, ToastLength.Short).Show();
                    txtprice.Text = maxdata;
                    strValue = txtprice.Text;
                    return;
                }

                SetBtnAddToCart();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at Dummy");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        public void SelectItemtoCart(Item ItemSelect)
        {
            try
            {
                //--------------------------
                //เปลี่ยนสีเมื่อมีสินค้าลงตะกร้า
                //lnPayment.SetBackgroundResource(Resource.Drawable.btnblue);
                //lnPayment.SetPadding(0, 5, 0, 5);
                //textSum.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                //txtNoItem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                //-------------------------

                //tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, ItemSelect, DataCashing.setQuantityToCart);

                try
                {
                    DetailNo = tranWithDetails.tranDetailItemWithToppings.Count + 1;
                    //dummy ถ้าเป็น  null แก้ไขจำนวนไม่ได้ ถ้าเป็น 0 แก้ไขได้แต่จะบันทึกข้อมูลไม่ได้
                    TranDetailItemNew DetailItem = new TranDetailItemNew()
                    {
                        SysItemID = ItemSelect.SysItemID,
                        MerchantID = DataCashingAll.MerchantId,
                        SysBranchID = DataCashingAll.SysBranchId,
                        TranNo = tranWithDetails.tran.TranNo,
                        ItemName = ItemSelect.ItemName,
                        SaleItemType = 'D',
                        FProcess = 1,
                        TaxType = 'V',
                        Quantity = (decimal)DataCashing.setQuantityToCart,
                        Price = ItemSelect.Price,
                        ItemPrice = ItemSelect.Price,
                        Discount = 0,
                        EstimateCost = 0,
                        SizeName = null,
                        Comments = null,
                        DetailNo = DetailNo,
                    };

                    List<TranDetailItemTopping> tranDetailItem = new List<TranDetailItemTopping>();
                    tranDetailItemWithTopping = new TranDetailItemWithTopping()
                    {
                        tranDetailItem = DetailItem,
                        tranDetailItemToppings = tranDetailItem,
                    };

                    tranWithDetails = BLTrans.ChooseItemTran(tranWithDetails, tranDetailItemWithTopping);
                    DataCashing.ModifyTranOrder = true;
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);

                    DataCashing.setQuantityToCart = 1;
                    PosActivity.setQuantity = 1;

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'DummyActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'DummyActivity.openPage' is assigned but its value is never used
        public DateTime pauseDate = DateTime.Now;

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

