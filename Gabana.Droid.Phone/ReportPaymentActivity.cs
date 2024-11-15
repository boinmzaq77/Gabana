using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views.InputMethods;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyInsightsLib;
using Android.Content;
using Gabana.Droid.ListData;
using System.Linq;

namespace Gabana.Droid
{
    [Activity(Theme = "@style/AppTheme.Main", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Icon = "@mipmap/GabanaLogIn", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ReportPaymentActivity : AppCompatActivity
    {
        public static ReportPaymentActivity reportPaymentActivity;
        private static ListPaymentType listPayment;
        LinearLayout lnBack, lnNoDataSearch;
        RecyclerView mRecycleView;
        private static List<PaymentType> lstPayment;
        public static List<PaymentType> listChoosePayment = new List<PaymentType>();
        Button btnAll, btnApply;
        public static string paymentSelect;
        EditText textSearch;
        string SearchName;
        ImageButton btnSearch;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.report_activity_payment);
                reportPaymentActivity = this;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnNoDataSearch = FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
                lnBack.Click += LnBack_Click;
                btnAll = FindViewById<Button>(Resource.Id.btnAll);
                btnAll.Click += BtnAll_Click;
                mRecycleView = FindViewById<RecyclerView>(Resource.Id.recyclerview_listpayment);
                btnApply = FindViewById<Button>(Resource.Id.btnApply);
                btnApply.Click += BtnApply_Click;
                textSearch = FindViewById<EditText>(Resource.Id.textSearch);
                textSearch.TextChanged += TextSearch_TextChanged; 
                textSearch.KeyPress += TextSearch_KeyPress ;
                textSearch.FocusChange += TxtSearch_FocusChange;
                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearch);
                btnSearch.Click += BtnSearch_Click;

                //paymentSelect = "All";
                CheckJwt();
                SetPaymentData();
                SetShowButton();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : ReportPaymentActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Report_Adapter_Customer_ItemClick at ReportPayment");
                Log.Debug("Error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtSearch_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(textSearch.Text.Trim()))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            OnResume();
        }

        private void SetBtnSearch()
        {
            if (string.IsNullOrEmpty(SearchName))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearch.Enabled = false;
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearch.Enabled = true;
            }
        }
        private void TextSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();

            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterBranchData();
            }
            View view = this.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(view.WindowToken, 0);
                }
            }
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del)
            {
                e.Handled = false;
            }

            e.Handled = false;
            if (e.Handled)
            {
                string input = string.Empty;
                switch (e.KeyCode)
                {
                    case Keycode.Num0:
                        input += "0";
                        break;
                    case Keycode.Num1:
                        input += "1";
                        break;
                    case Keycode.Num2:
                        input += "2";
                        break;
                    case Keycode.Num3:
                        input += "3";
                        break;
                    case Keycode.Num4:
                        input += "4";
                        break;
                    case Keycode.Num5:
                        input += "5";
                        break;
                    case Keycode.Num6:
                        input += "6";
                        break;
                    case Keycode.Num7:
                        input += "7";
                        break;
                    case Keycode.Num8:
                        input += "8";
                        break;
                    case Keycode.Num9:
                        input += "9";
                        break;
                    default:
                        break;
                }
                //e.Handled = false;
                textSearch.Text += input;
                textSearch.SetSelection(textSearch.Text.Length);
                return;
            }
        }
        private async void SetFilterBranchData()
        {
            try
            {
                lstPayment = lstPayment.Where(x => x.Detail.ToLower().Contains(SearchName)).ToList();
                listPayment = new ListPaymentType(lstPayment);

                Report_Adapter_Payment report_adapter_customer = new Report_Adapter_Payment(listPayment);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetAdapter(report_adapter_customer);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;

                if (report_adapter_customer.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    mRecycleView.Visibility = Android.Views.ViewStates.Gone;
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                }
                else
                {
                    mRecycleView.Visibility = Android.Views.ViewStates.Visible;
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                }
                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterBranchData at chooseBranch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void TextSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SearchName = textSearch.Text.Trim();
            if (string.IsNullOrEmpty(SearchName))
            {
                SetPaymentData();
            }
            SetBtnSearch();
        }
        private void SetClearSearchText()
        {
            SearchName = "";
            textSearch.Text = string.Empty;
            SetBtnSearch();
        }
        private async void SetPaymentData()
        {

            lstPayment = await SetListPayment();
            listPayment = new ListPaymentType(lstPayment);

            if (listChoosePayment.Count == 0)
            {
                listChoosePayment = lstPayment;
            }

            Report_Adapter_Payment report_adapter_customer = new Report_Adapter_Payment(listPayment);
            GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
            mRecycleView.SetAdapter(report_adapter_customer);
            mRecycleView.HasFixedSize = true;
            mRecycleView.SetLayoutManager(mLayoutManager);
            report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;

            if (report_adapter_customer.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
            {
                mRecycleView.Visibility = Android.Views.ViewStates.Gone;
                lnNoDataSearch.Visibility = ViewStates.Visible;
            }
            else
            {
                mRecycleView.Visibility = Android.Views.ViewStates.Visible;
                lnNoDataSearch.Visibility = ViewStates.Gone;
            }


        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<List<PaymentType>> SetListPayment()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            List<Gabana.Model.PaymentType> listPayments = new List<Gabana.Model.PaymentType>
            {
                new Model.PaymentType(){ Type ="CH" ,Detail = "Cash เงินสด", Logo = Resource.Mipmap.RPaymentCash , color = "#0095DA"},
                new Model.PaymentType(){ Type ="Cr" ,Detail = "Credit Card บัตรเคดิต", Logo = Resource.Mipmap.RPaymentCredit, color = "#F8971D"},
                new Model.PaymentType(){ Type ="Dr" ,Detail = "Debit Card บัตรเดบิต" , Logo = Resource.Mipmap.RPaymentDebit, color = "#E32D49"},
                new Model.PaymentType(){ Type ="GV" ,Detail = "Gift Voucher บัตรของขวัญ" , Logo = Resource.Mipmap.RPaymentGiftvoucher , color = "#37AA52" },
                new Model.PaymentType(){ Type ="MYQR",Detail = "myQR คิวอาร์ของฉัน", Logo = Resource.Mipmap.RPaymentMyQR , color = "#F75600"},
                new Model.PaymentType(){ Type ="QRCH",Detail = "QR Cash คิวอาร์เงินสด", Logo = Resource.Mipmap.RPaymentQrCash , color = "#3F51B5"},
                new Model.PaymentType(){ Type ="QRCR",Detail = "QR Credit คิวอาร์เครดิต", Logo = Resource.Mipmap.RPaymentQrCredit , color = "#00796B"},
                new Model.PaymentType(){ Type ="WECHAT",Detail = "WECHAT วีแชท",Logo = Resource.Mipmap.RPaymentWechat , color = "#8BC34A"}
            };
            return listPayments;
        }
        private void SetShowButton()
        {
            if (paymentSelect != "All Payment")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }

            if (listChoosePayment.Count > 0)
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

        }

        private async void BtnAll_Click(object sender, EventArgs e)
        {
            if (paymentSelect != "All Payment" && paymentSelect == "")
            {
                paymentSelect = "All Payment";
                listChoosePayment = new List<PaymentType>();
                listChoosePayment = lstPayment;
            }
            else
            {
                listChoosePayment = new List<PaymentType>();
                paymentSelect = "";
            }

            SetShowButton();

            lstPayment = await SetListPayment();
            listPayment = new ListPaymentType(lstPayment);

            Report_Adapter_Payment report_adapter_customer = new Report_Adapter_Payment(listPayment);
            GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
            mRecycleView.SetAdapter(report_adapter_customer);
            mRecycleView.HasFixedSize = true;
            mRecycleView.SetLayoutManager(mLayoutManager);
            report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;



        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (paymentSelect != string.Empty)
            {
                ReportDailySaleActivity.listChoosePayment = listChoosePayment;
                this.Finish();
            }
            else
            {
                Toast.MakeText(this, "กรุณาเลือกรูปแบบชำระเงิน", ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        private async void Report_Adapter_Customer_ItemClick(object sender, int e)
        {
            try
            {
                var payment = listPayment[e];
                var search = listChoosePayment.FindIndex(x => x.Type == payment.Type);
                if (search == -1)
                {
                    listChoosePayment.Add(payment);
                }
                else
                {
                    listChoosePayment.RemoveAt(search);
                }

                paymentSelect = "";

                if (listPayment.Count == listChoosePayment.Count)
                {
                    paymentSelect = "All Payment";
                }
                else
                {
                    foreach (var item in listChoosePayment)
                    {
                        if (paymentSelect != "")
                        {
                            paymentSelect += "," + item.Type;
                        }
                        else
                        {
                            paymentSelect = item.Type;
                        }
                    }
                }

                lstPayment = await SetListPayment();
                listPayment = new ListPaymentType(lstPayment);
                SetShowButton();

                Report_Adapter_Payment report_adapter_customer = new Report_Adapter_Payment(listPayment);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this, 1, 1, false);
                mRecycleView.SetAdapter(report_adapter_customer);
                mRecycleView.HasFixedSize = true;
                mRecycleView.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Report_Adapter_Customer_ItemClick at ReportPayment");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }
        internal static void SetSelectPayment(List<PaymentType> l, string t)
        {
            listChoosePayment = l;
            paymentSelect = t;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'ReportPaymentActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'ReportPaymentActivity.openPage' is assigned but its value is never used
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

