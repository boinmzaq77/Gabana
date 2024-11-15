using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Chip;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Google.Android.Material.Chip;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class CashActivity : AppCompatActivity
    {
        ImageButton imagebtnBack, btndeletenumber;
        TextView txtAmount, txtpayment_amount;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, buttonCharge;
        CashActivity cash;
        public static LinearLayout lnNoCustomer, lnHaveCustomer;
        static TextView txtNameCustomer;
        ImageButton btnCustomer;
        TransManage transManage = new TransManage();
        public static TranWithDetailsLocal tranWithDetails;
        public static TranPayment tranPayment = new TranPayment();
        double amount; //amount คือ ยอดที่ต้องจ่าย       
        string strValue; //strValue คือ จำนวนเงินที่จะจ่าย
        int count = 0 , DECIMALPOINT = 2;
        int PaymentNo;
        double Change, Cash;//เงินทอน
        string CURRENCYSYMBOLS, DECIMALPOINTDISPLAY;
        static Customer selectCus;
        decimal dec;
        string maxdata;
        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;
        List<ORM.Master.CashTemplate> listcashTemplate = new List<ORM.Master.CashTemplate>();
        //ChipGroup chipGroup;
        DialogLoading dialogLoading = new DialogLoading();
        decimal SumValue = 0;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.cash_activity_main);
                cash = this;
                SetUIElement();
                CheckJwt();
                Log.Debug("swipapp", "Cash OnCreate");
                InitializeDecimalSettings();
                await UpdateTransactionDetails();
                CalculateAmount();
                SetupAmountFields();
                SetBtnCharge();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : CashActivity");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("at CashActivity");
                return;
            }
        }

        private void SetupAmountFields()
        {
            txtAmount.Hint = Utils.DisplayDecimal(Convert.ToDecimal(amount));
            txtpayment_amount.Text = GetString(Resource.String.payment_activity_paymentamount) + " : " + Utils.DisplayDecimal(Convert.ToDecimal(amount)) + " " + CURRENCYSYMBOLS;
            txtAmount.Text = strValue;
            maxdata = Utils.DisplayDecimal(DECIMALPOINT == 4 ? (decimal)9999999999.9999 : (decimal)9999999999.99);
        }

        private void CalculateAmount()
        {
            decimal paymentAmount = tranWithDetails.tranPayments.Sum(item => item.PaymentAmount);
            amount = Convert.ToDouble(tranWithDetails.tran.GrandPayment - paymentAmount);
        }


        private async Task UpdateTransactionDetails()
        {
            if (tranWithDetails != null)
            {
                tranWithDetails = await BLTrans.CalDecimal(tranWithDetails);
            }
        }

        private void InitializeDecimalSettings()
        {
            DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY ?? "2";
            DECIMALPOINT = DECIMALPOINTDISPLAY == "4" ? 4 : 2;
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        }

        private void SetUIElement()
        {
            imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
            btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
            txtAmount = FindViewById<TextView>(Resource.Id.txtAmount);
            txtpayment_amount = FindViewById<TextView>(Resource.Id.txtpayment_amount);
            btnpoint = FindViewById<Button>(Resource.Id.btnpoint);
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
            buttonCharge = FindViewById<Button>(Resource.Id.buttonCharge);
            btnCustomer = FindViewById<ImageButton>(Resource.Id.btnCustomer);
            lnNoCustomer = FindViewById<LinearLayout>(Resource.Id.lnNoCustomer);
            lnHaveCustomer = FindViewById<LinearLayout>(Resource.Id.lnHaveCustomer);
            txtNameCustomer = FindViewById<TextView>(Resource.Id.txtNameCustomer);
            //chipGroup = FindViewById<ChipGroup>(Resource.Id.chipGroup);

            imagebtnBack.Click += ImagebtnBack_Click;
            btndeletenumber.Click += Btndeletenumber_Click;
            btnCustomer.Click += BtnCustomer_Click;
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
            buttonCharge.Click += ButtonCharge_Click;
        }

        private async void SetCashGuide()
        {    
            try
            {
                CashTemplateManage CashTemplateManage = new CashTemplateManage();
                List<CashTemplate> lstcashTemplates = new List<CashTemplate>();

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }
                if (await GabanaAPI.CheckSpeedConnection())
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
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowCash at CashGuide");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void CreateChip(List<CashTemplate> lstcashTemplates)
        {
            try
            {
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var sizechip = Convert.ToInt32((mainDisplayInfo.Width - 100) / 4);

                using (var chipGroup = FindViewById<Android.Support.Design.Chip.ChipGroup>(Resource.Id.chipGroup))
                {
                    chipGroup.RemoveAllViews();
                    chipGroup.SetForegroundGravity(GravityFlags.CenterHorizontal);

                    foreach (var item in lstcashTemplates)
                    {
                        var inflater = LayoutInflater.From(this);
                        var chip = inflater.Inflate(Resource.Layout.cash_chip_cashguid, null, false) as Android.Support.Design.Chip.Chip;
                        chip.Text = "+ " + item.Amount.ToString("#,###.##");
                        chip.SetWidth(sizechip);
                        chip.Gravity = GravityFlags.CenterHorizontal;
                        chipGroup.AddView(chip);

                        chip.Click += (s, e) =>
                        {
                            //กดแล้วเพิ่มค่า 28/03/66
                            string chipvalue = chip.Text.Replace("+", "");
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
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }              

        private void SetBtnCharge()
        {
            if (txtAmount.Text != "")
            {
                btndeletenumber.Enabled = true;
                buttonCharge.Text = GetString(Resource.String.btnCharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDecimal(Convert.ToDecimal(txtAmount.Text));
                buttonCharge.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                buttonCharge.SetBackgroundResource(Resource.Drawable.btnblue);
            }
            else
            {
                btndeletenumber.Enabled = false;
                buttonCharge.Text = GetString(Resource.String.btnCharge) + " " + CURRENCYSYMBOLS + Utils.DisplayDecimal((decimal)amount);
                buttonCharge.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                buttonCharge.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
        }

        public static async void SetCustomer()
        {
            try
            {
                if (DataCashing.SysCustomerID == 999)
                {
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

        private void ButtonCharge_Click(object sender, EventArgs e)
        {
            try
            {
                buttonCharge.Enabled = false;
                Cash = string.IsNullOrEmpty(strValue) ? Convert.ToDouble(txtAmount.Hint) : Convert.ToDouble(strValue);

                AddPaymentToTransaction(Cash);

                StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));
                BalanceActivity.SetTranDetail(tranWithDetails);
                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                buttonCharge.Enabled = true;
                return;
            }
        }

        private void AddPaymentToTransaction(double cash)
        {
            //คำนวณยอดเงินที่ชำระ บันทึกลง tranpayment  cash (strValue) >= amount (ยอดจ่าย)
            initialData();
            int paymentNo = tranWithDetails.tranPayments.Count + 1;
            Change = CalculateAmount(Cash, amount); // Cash เงินที่จ่าย, amount(ยอดจ่าย)
            tranPayment.PaymentNo = PaymentNo;
            tranPayment.PaymentAmount = (decimal)Cash; //เงินที่จ่าย
            tranWithDetails.tranPayments.Add(tranPayment);
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

        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                #region old code
                //int indexpoint = strValue.LastIndexOf(".");
                //int indexclear = 0;
                //decimal damount;
                //string amount;
                ////strValue = damount.ToString();

                //if (strValue != string.Empty && strValue.Length > 1)
                //{
                //    amount = strValue.Remove(strValue.Length - 1);
                //    damount = Convert.ToDecimal(amount);
                //    txtAmount.Text = Utils.DisplayDecimal((decimal)damount);
                //    strValue = txtAmount.Text;
                //    txtAmount.Focusable = true;
                //    indexclear = txtAmount.Text.LastIndexOf(".");
                //}
                //else
                //{
                //    strValue = "";
                //    txtAmount.Text = strValue;
                //    txtAmount.Focusable = true;
                //}

                //if (txtAmount.Text != "")
                //{
                //    damount = Convert.ToDecimal(txtAmount.Text);
                //    if (DECIMALPOINTDISPLAY == "4")
                //    {
                //        amount = (damount * 1000).ToString();
                //    }
                //    else
                //    {
                //        amount = (damount * 10).ToString();
                //    }
                //}
                //else
                //{
                //    amount = "0";
                //}

                //txtAmount.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                //strValue = txtAmount.Text;

                //if (indexpoint > indexclear)
                //{
                //    count = 0;
                //} 
                #endregion
                                
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
            PaymentActivity.SetTranDetail(tranWithDetails);
            this.Finish();
        }

        public void SetValue(Button btn)
        {
            try
            {
                #region old code
                //if (strValue == "0" && btn.Text != ".")
                //{
                //    strValue = string.Empty;
                //    txtAmount.Text = strValue;
                //}

                //string amount;
                ////if (txtAmount.Text != "")
                ////{
                ////    var damount = Convert.ToDouble(txtAmount.Text);
                ////    amount = (damount * 100).ToString();
                ////}
                ////else
                ////{
                ////    amount = "";
                ////}

                //if (txtAmount.Text != "")
                //{
                //    //if (txtAmount.Text.Length > 18)
                //    //{
                //    //    amount = txtAmount.Text;
                //    //}
                //    //else
                //    //{
                //    var damount = Convert.ToDouble(txtAmount.Text);

                //    if (DECIMALPOINTDISPLAY == "4")
                //    {
                //        amount = (damount * 10000).ToString();
                //    }
                //    else
                //    {
                //        amount = (damount * 100).ToString();
                //    }
                //    //}
                //}
                //else
                //{
                //    amount = "0";
                //}

                //var num = btn.Text.ToString();
                //btn.RequestFocus();

                //if (count == 0)
                //{
                //    switch (num)
                //    {
                //        case "0":
                //            amount += num;
                //            break;
                //        case "1":
                //            amount += num;
                //            break;
                //        case "2":
                //            amount += num;
                //            break;
                //        case "3":
                //            amount += num;
                //            break;
                //        case "4":
                //            amount += num;
                //            break;
                //        case "5":
                //            amount += num;
                //            break;
                //        case "6":
                //            amount += num;
                //            break;
                //        case "7":
                //            amount += num;
                //            break;
                //        case "8":
                //            amount += num;
                //            break;
                //        case "9":
                //            amount += num;
                //            break;
                //        default:
                //            amount += num;
                //            count++;
                //            break;
                //    }
                //}
                //else
                //{
                //    switch (num)
                //    {
                //        case "0":
                //            amount += num;
                //            break;
                //        case "1":
                //            amount += num;
                //            break;
                //        case "2":
                //            amount += num;
                //            break;
                //        case "3":
                //            amount += num;
                //            break;
                //        case "4":
                //            amount += num;
                //            break;
                //        case "5":
                //            amount += num;
                //            break;
                //        case "6":
                //            amount += num;
                //            break;
                //        case "7":
                //            amount += num;
                //            break;
                //        case "8":
                //            amount += num;
                //            break;
                //        case "9":
                //            amount += num;
                //            break;
                //        default:
                //            amount += num;
                //            break;

                //    }
                //}
                ////txtAmount.Text = Utils.DisplayDecimal((decimal)(Convert.ToDouble(amount) * 0.01));
                ////strValue = txtAmount.Text;


                ////if (amount.Length > 18)
                ////{
                ////    decimal decimalOne = decimal.Parse(amount); 
                ////    decimal decimalWithDigit = decimalOne + 0.0000m;
                ////    txtAmount.Text = decimalWithDigit.ToString();
                ////}
                ////else
                ////{
                //txtAmount.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                ////}

                //strValue = txtAmount.Text; 
                #endregion

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
                    Toast.MakeText(this, GetString(Resource.String.maxamount) + " " + maxdata, ToastLength.Short).Show();
                    txtAmount.Text = maxdata;
                    strValue = txtAmount.Text;
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at cash");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        private double CalculateAmount(double Cash, double Amount)
        {
            Change = Cash - Amount;
            return Change;
        }

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

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                Log.Debug("swipapp", "Cash OnResume");
                SetCashGuide();
                SetCustomer();
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                //DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();   
            imagebtnBack.PerformClick();
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

        protected override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                imagebtnBack?.Dispose();
                btndeletenumber?.Dispose();
                txtAmount?.Dispose();
                txtpayment_amount?.Dispose();
                btnpoint?.Dispose();
                btnnumber0?.Dispose();
                btnnumber1?.Dispose();
                btnnumber2?.Dispose();
                btnnumber3?.Dispose();
                btnnumber4?.Dispose();
                btnnumber5?.Dispose();
                btnnumber6?.Dispose();
                btnnumber7?.Dispose();
                btnnumber8?.Dispose();
                btnnumber9?.Dispose();
                buttonCharge?.Dispose();
                btnCustomer?.Dispose();
                lnNoCustomer?.Dispose();
                lnHaveCustomer?.Dispose();
                txtNameCustomer?.Dispose();
                //chipGroup?.Dispose();

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnDestroy at Cash");
            }
        }
    }
}