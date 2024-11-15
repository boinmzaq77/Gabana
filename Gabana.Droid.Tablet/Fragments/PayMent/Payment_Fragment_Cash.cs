using Android.Accounts;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Chip;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using iTextSharp.text;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_Cash : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Payment_Fragment_Cash NewInstance()
        {
            Payment_Fragment_Cash frag = new Payment_Fragment_Cash();
            return frag;
        }
        View view;
        public static TranWithDetailsLocal tranWithDetails;
        public static Payment_Fragment_Cash fragment_cash;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_cash, container, false);
            try
            {
                fragment_cash = this;
                tranWithDetails = PaymentActivity.tranWithDetails;
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                ComBineUI();
                btndeletenumber.Click += Btndeletenumber_Click;
                btnpoint.Click += Btnpoint_Click;
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
                btncharge.Click += BtnCharge_Click;
                txtClear.Click += TxtClear_Click;
                
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }

            return view;
        }

        private void TxtClear_Click(object sender, EventArgs e)
        {
            txtAmount.Text = string.Empty;
            strValue = string.Empty;
            ShowDetail();
        }

        double Change, Cash;//เงินทอน
        int PaymentNo;
        public override void OnResume()
        {
            try
            {
                base.OnResume();

                if (!IsAdded)
                {
                    return;
                }

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                tranWithDetails = PaymentActivity.tranWithDetails;
                SetCashGuide();
                txtAmount.Text = string.Empty;
                strValue = string.Empty;
                ShowDetail();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnCharge_Click(object sender, EventArgs e)
        {
            try
            {
                btncharge.Enabled = false;

                if (string.IsNullOrEmpty(strValue))
                {
                    Cash = Convert.ToDouble(txtAmount.Hint);
                }
                else
                {
                    Cash = Convert.ToDouble(strValue);
                }

                //คำนวณยอดเงินที่ชำระ บันทึกลง tranpayment  cash (strValue) >= amount (ยอดจ่าย)
                initialData();

                int PaymentNo =  tranWithDetails.tranPayments.Count();
                PaymentNo++;

                Change = CalculateAmount(Cash, amount); // Cash เงินที่จ่าย, amount(ยอดจ่าย)
                tranPayment.PaymentNo = PaymentNo;
                tranPayment.PaymentAmount = (decimal)Cash; //เงินที่จ่าย
                tranWithDetails.tranPayments.Add(tranPayment);

                CheckBalance();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                btncharge.Enabled = true;
                return;
            }
        }

        private void CheckBalance()
        {
            try
            {
                var list = tranWithDetails.tranPayments.ToList();
                decimal payAmount = 0;
                foreach (var item in list)
                {
                    payAmount += item.PaymentAmount;

                }
                var change = payAmount - tranWithDetails.tran.GrandPayment;
                var index = list.FindLastIndex(x => x.PaymentType == "CH");
                if (index == list.Count - 1)
                {
                    change = payAmount - tranWithDetails.tran.GrandPayment;
                }
                else
                {
                    change = payAmount - tranWithDetails.tran.GrandTotal;
                }

                decimal payByCash = list.Where(x => x.PaymentType == "CH").Sum(x => x.PaymentAmount);
                decimal payByGiftvoucher = list.Where(x => x.PaymentType == "GV").Sum(x => x.PaymentAmount);

                if (payByGiftvoucher > 0 && payAmount < payByGiftvoucher)
                {
                    change = 0;
                }

                if (payByCash < change)
                {
                    change = payByCash;
                }
                if (change < 0)
                {
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.btnChange, "balance", "default");
                }
                else
                {
                    PaymentActivity.payment_main.LoadFragment(Resource.Id.btnChange, "balance", "receipt");
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private double CalculateAmount(double Cash, double Amount)
        {
            Change = Cash - Amount;
            return Change;
        }
        public static TranPayment tranPayment = new TranPayment();
        private void initialData()
        {
            if (tranWithDetails == null)
            {
                return;
            }

            tranPayment = new TranPayment()
            {
                MerchantID = DataCashingAll.MerchantId,
                SysBranchID = DataCashingAll.SysBranchId,
                TranNo = tranWithDetails.tran.TranNo,
                PaymentNo = PaymentNo,
                PaymentType = DataCashing.PaymentType,
                PaymentAmount = (decimal)0, //เงินที่ต้องจ่าย
                CreditCardType = null,
                CardNo = null,
                ExprieDateYYYYMM = null,
                ApproveCode = null,
                TotalRedeemPoint = null,
                //EPaymentType = null,
                RequestNum = null,
                RequestDateTime = null,
                FEPaymentCancel = 0,
                ReferenceNo1 = null,
                ReferenceNo2 = null,
                ReferenceNo3 = null,
                ReferenceNo4 = null,
                Comments = null,
            };
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
        bool clickPoint = false;
        private void Btnpoint_Click(object sender, EventArgs e)
        {
            clickPoint = true;
            SetValue(btnpoint);
        }
        public void SetValue(Button btn)
        {
            try
            {
                string str = "";
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtAmount.Text = strValue;
                }

                if (clickPoint)
                {
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
                txtAmount.Text = Utils.DisplayComma(str);
                SetBtnCharge();

                if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.maxamount) + " " + maxdata, ToastLength.Short).Show();
                    txtAmount.Text = maxdata;
                    strValue = txtAmount.Text;
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at cash");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        int count = 0;
        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(strValue) && strValue.Length > 1)
                {
                    strValue = strValue.Remove(strValue.Length - 1);
                    string str = "";
                    str = strValue;
                    txtAmount.Text = Utils.DisplayComma(str);
                }
                else
                {
                    SumValue = 0;
                    strValue = "0";
                    txtAmount.Text = "";
                    txtAmount.Hint = Utils.DisplayDecimal(0);
                    txtAmount.Focusable = true;
                }
                SetBtnCharge();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        decimal dec;
        string maxdata;
        private async void ShowDetail()
        {
            try
            {
                tranWithDetails = PaymentActivity.tranWithDetails;
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        DECIMALPOINT = 4;
                    }
                    else
                    {
                        DECIMALPOINT = 2;
                    }
                }
                else
                {
                    DECIMALPOINT = 2;
                }
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                if (tranWithDetails != null)
                {
                    tranWithDetails = await BLTrans.CalDecimal(tranWithDetails);
                }
                decimal paymentAmount = 0;
                foreach (var item in tranWithDetails.tranPayments)
                {
                    paymentAmount += item.PaymentAmount;
                }
                //amount คือ ยอดที่ต้องจ่าย      

                amount = Convert.ToDouble(tranWithDetails.tran.GrandPayment - paymentAmount);
                txtAmount.Hint = Utils.DisplayDecimal(Convert.ToDecimal(amount));
                txtPaymentAmount.Text = GetString(Resource.String.payment_activity_paymentamount) + " : " + Utils.DisplayDecimal(Convert.ToDecimal(amount)) + " " + CURRENCYSYMBOLS;

                //txtAmount คือ จำนวนเงินที่จะจ่าย
                txtAmount.Text = strValue;
                SetBtnCharge();

                if (DECIMALPOINTDISPLAY == "4")
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.9999);
                }
                else
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.99);
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreateView : CashActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }


        }
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, btncharge;
        ImageButton btndeletenumber;
        ChipGroup chipGroup;
        TextView txtAmount, txtClear, txtPaymentAmount;
        string strValue; //strValue คือ จำนวนเงินที่จะจ่าย
        double amount; //amount คือ ยอดที่ต้องจ่าย       
        string CURRENCYSYMBOLS, DECIMALPOINTDISPLAY;
        int  DECIMALPOINT = 2;
        decimal SumValue = 0;

        private void ComBineUI()
        {
            btnpoint = view.FindViewById<Button>(Resource.Id.btnpoint);
            btnnumber0 = view.FindViewById<Button>(Resource.Id.btnnumber0);
            btnnumber1 = view.FindViewById<Button>(Resource.Id.btnnumber1);
            btnnumber2 = view.FindViewById<Button>(Resource.Id.btnnumber2);
            btnnumber3 = view.FindViewById<Button>(Resource.Id.btnnumber3);
            btnnumber4 = view.FindViewById<Button>(Resource.Id.btnnumber4);
            btnnumber5 = view.FindViewById<Button>(Resource.Id.btnnumber5);
            btnnumber6 = view.FindViewById<Button>(Resource.Id.btnnumber6);
            btnnumber7 = view.FindViewById<Button>(Resource.Id.btnnumber7);
            btnnumber8 = view.FindViewById<Button>(Resource.Id.btnnumber8);
            btnnumber9 = view.FindViewById<Button>(Resource.Id.btnnumber9);
            btncharge = view.FindViewById<Button>(Resource.Id.btncharge);
            btndeletenumber = view.FindViewById<ImageButton>(Resource.Id.btndeletenumber);
            chipGroup = view.FindViewById<ChipGroup>(Resource.Id.chipGroup);
            txtAmount = view.FindViewById<TextView>(Resource.Id.txtAmount);
            txtClear = view.FindViewById<TextView>(Resource.Id.txtClear);
            txtPaymentAmount = view.FindViewById<TextView>(Resource.Id.txtPaymentAmount); 

        }
        List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();
        CashTemplateManage CashTemplateManage = new CashTemplateManage();
        List<CashTemplate> lstcashTemplates = new List<CashTemplate>();
        private async void SetCashGuide()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(PaymentActivity.payment_main.SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                ResultAPI ResultCashguid = new ResultAPI(false, String.Empty);
                if (await GabanaAPI.CheckNetWork())
                {
                    listcashTemplate = await GabanaAPI.GetDataCashTemplate();
                    if (listcashTemplate == null)
                    {
                        dialogLoading.Dismiss();
                        return;
                    }
                    //ลบข้อมูลทั้งหมด
                    var delete = await CashTemplateManage.DeleteAllCashTemplatee(DataCashingAll.MerchantId);

                    var lst = new List<CashTemplate>();
                    foreach (var item in listcashTemplate)
                    {
                        CashTemplate cashTemplate = new CashTemplate()
                        {
                            Amount = item.Amount,
                            CashTemplateNo = item.CashTemplateNo,
                            DateModified = item.DateModified,
                            MerchantID = item.MerchantID,
                        };
                        var InsertorReplace = await CashTemplateManage.InsertorReplaceCashTemplate(cashTemplate);
                        lst.Add(cashTemplate);
                    }
                    lstcashTemplates = new List<CashTemplate>();
                    lstcashTemplates.AddRange(lst);
                    lstcashTemplates = lstcashTemplates.OrderBy(x => x.CashTemplateNo).ToList();
                }
                else
                {
                    lstcashTemplates = new List<CashTemplate>();
                    lstcashTemplates = await CashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                }

                lstcashTemplates = lstcashTemplates.OrderBy(x => x.Amount).ToList();
                CreateChip(lstcashTemplates);


                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowCash at CashGuide");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void CreateChip(List<CashTemplate> lstcashTemplates)
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var sizechip = Convert.ToInt32((mainDisplayInfo.Width - 100) / 12);

            chipGroup.RemoveAllViews();
            chipGroup.SetForegroundGravity(GravityFlags.CenterHorizontal);
            //var inflater = LayoutInflater.From(Application.Context);

            foreach (var item in lstcashTemplates)
            {
                var inflater = LayoutInflater.From(view.Context);
                Chip chip = inflater.Inflate(Resource.Layout.cash_chip_cashguid, null, false) as Chip;
                chip.Text = "+ " + item.Amount.ToString("#,###.##");
                chip.SetWidth(sizechip);
                chip.Gravity = GravityFlags.CenterHorizontal;
                chipGroup.AddView(chip);
                chip.Click += (s, e) =>
                {
                    string chipvalue = chip.Text.Replace("+","") ;
                    //กดแล้วเพิ่มค่า 28/03/66                    
                    if (string.IsNullOrEmpty(strValue))
                    {
                        SumValue += Convert.ToDecimal(chipvalue);
                    }
                    else
                    {
                        SumValue = Convert.ToDecimal(strValue) + Convert.ToDecimal(chipvalue);
                    }
                    strValue = SumValue.ToString();
                    txtAmount.Text = Utils.DisplayComma(strValue);
                    SetBtnCharge();
                };
            }
        }
        private void SetBtnCharge()
        {
            if (txtAmount.Text != "")
            {
                btndeletenumber.Enabled = true;
                btncharge.Text = GetString(Resource.String.btnCharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDecimal(Convert.ToDecimal(txtAmount.Text));
                btncharge.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btncharge.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
            else
            {
                btndeletenumber.Enabled = false;
                btncharge.Text = GetString(Resource.String.btnCharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDecimal((decimal)amount);
                btncharge.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btncharge.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
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

    }
}