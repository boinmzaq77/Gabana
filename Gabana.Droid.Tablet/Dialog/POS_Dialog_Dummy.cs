using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class POS_Dialog_Dummy : AndroidX.Fragment.App.DialogFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static POS_Dialog_Dummy NewInstance()
        {
            var frag = new POS_Dialog_Dummy { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public static POS_Dialog_Dummy dialog_dummy;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_dummy, container, false);
            try
            {
                dialog_dummy = this;
                CombinUI();
                tranWithDetails = MainActivity.tranWithDetails;
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Option");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }

        LinearLayout lnBack;
        TextView txtprice;
        EditText txtDescription;
        Button btnnumber1, btnnumber2, btnnumber3, btnnumber4,
            btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9,
            btnPoint, btnnumber0;
        LinearLayout btndeletenumber;
        Button btnAddtoCart;
        string DECIMALPOINTDISPLAY;
        string strValue, DummyDescription;
        int DECIMALPOINT = 2;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            txtprice = view.FindViewById<TextView>(Resource.Id.txtprice);
            txtDescription = view.FindViewById<EditText>(Resource.Id.txtDescription);
            btnnumber1 = view.FindViewById<Button>(Resource.Id.btnnumber1);
            btnnumber2 = view.FindViewById<Button>(Resource.Id.btnnumber2);
            btnnumber3 = view.FindViewById<Button>(Resource.Id.btnnumber3);
            btnnumber4 = view.FindViewById<Button>(Resource.Id.btnnumber4);
            btnnumber5 = view.FindViewById<Button>(Resource.Id.btnnumber5);
            btnnumber6 = view.FindViewById<Button>(Resource.Id.btnnumber6);
            btnnumber7 = view.FindViewById<Button>(Resource.Id.btnnumber7);
            btnnumber8 = view.FindViewById<Button>(Resource.Id.btnnumber8);
            btnnumber9 = view.FindViewById<Button>(Resource.Id.btnnumber9);
            btnPoint = view.FindViewById<Button>(Resource.Id.btnPoint);
            btnnumber0 = view.FindViewById<Button>(Resource.Id.btnnumber0);
            btndeletenumber = view.FindViewById<LinearLayout>(Resource.Id.btndeletenumber);
            btnAddtoCart = view.FindViewById<Button>(Resource.Id.btnAddtoCart);

            lnBack.Click += ImageButtonBack_Click;
            lnBack.Click += ImageButtonBack_Click;
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
            btnAddtoCart.Click += Addtocart_Click;

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
        }

        private void ImageButtonBack_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        public static TranWithDetailsLocal tranWithDetails;

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

                MainActivity.tranWithDetails = tranWithDetails;
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();

                this.Dialog.Dismiss();
            }

        }
        static int DetailNo;
        TranDetailItemWithTopping tranDetailItemWithTopping;

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
                    POS_Fragment_Main.setQuantity = 1;

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
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
                    txtprice.Text = Utils.DisplayComma(str);
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.maxservicecharg) + " " + maxdata, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetBtnAddToCart()
        {
            if (txtprice.Text != "")
            {
                btndeletenumber.Enabled = true;
                btnAddtoCart.Enabled = true;
                btnAddtoCart.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAddtoCart.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btndeletenumber.Enabled = false;
                btnAddtoCart.Enabled = false;
                btnAddtoCart.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAddtoCart.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
    }
}