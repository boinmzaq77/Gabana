using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Report;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    internal class Report_Dialog_Payment : AndroidX.Fragment.App.DialogFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Report_Dialog_Customer NewInstance()
        {
            var frag = new Report_Dialog_Customer { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.report_dialog_payment, container, false);
            try
            {
                CombinUI();
                SetUIEvent();

                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Report Dialog Customer");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            btnAll.Click += BtnAll_Click;
            btnApply.Click += BtnApply_Click;

            textSearch.TextChanged += TextSearch_TextChanged;
            textSearch.KeyPress += TextSearch_KeyPress;
            textSearch.FocusChange += TxtSearch_FocusChange;
            btnSearch.Click += BtnSearch_Click;

            SetPaymentData();
            SetShowButton();

        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
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

            Report_Adapter_ChoosePayment report_adapter_customer = new Report_Adapter_ChoosePayment(listPayment);
            GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
            rcvPayment.SetAdapter(report_adapter_customer);
            rcvPayment.HasFixedSize = true;
            rcvPayment.SetLayoutManager(mLayoutManager);
            report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;
        }
        internal static void SetSelectPayment(List<PaymentType> l, string t)
        {
            listChoosePayment = l;
            paymentSelect = t;
        }
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            if (paymentSelect != string.Empty)
            {
                Report_Dialog_Custom.listChoosePayment = listChoosePayment;
                await Report_Dialog_Custom.dialog_Custom.SetDataPayment();
                this.Dialog.Dismiss();
            }
            else
            {
                Toast.MakeText(this.Activity, "กรุณาเลือกรูปแบบชำระเงิน", ToastLength.Short).Show();
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
        private void TextSearch_KeyPress(object sender, Android.Views.View.KeyEventArgs e)
        {
            SetBtnSearch();

            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterBranchData();
            }
            View view = this.Activity.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    MainActivity.main_activity.CloseKeyboard(view);
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

                Report_Adapter_ChoosePayment report_adapter_customer = new Report_Adapter_ChoosePayment(listPayment);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvPayment.SetAdapter(report_adapter_customer);
                rcvPayment.HasFixedSize = true;
                rcvPayment.SetLayoutManager(mLayoutManager);
                report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;

                if (report_adapter_customer.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
                {
                    rcvPayment.Visibility = Android.Views.ViewStates.Gone;
                    lnNoDataSearch.Visibility = ViewStates.Visible;
                }
                else
                {
                    rcvPayment.Visibility = Android.Views.ViewStates.Visible;
                    lnNoDataSearch.Visibility = ViewStates.Gone;
                }
                SetBtnSearch();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterBranchData at chooseBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
        private void SetClearSearchText()
        {
            SearchName = "";
            textSearch.Text = string.Empty;
            SetBtnSearch();
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
        private static List<PaymentType> lstPayment;
        private static ListPaymentType listPayment;
        string SearchName;
        private async void SetPaymentData()
        {

            lstPayment = await SetListPayment();
            listPayment = new ListPaymentType(lstPayment);

            if (listChoosePayment.Count == 0)
            {
                listChoosePayment = lstPayment;
            }

            Report_Adapter_ChoosePayment report_adapter_customer = new Report_Adapter_ChoosePayment(listPayment);
            GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
            rcvPayment.SetAdapter(report_adapter_customer);
            rcvPayment.HasFixedSize = true;
            rcvPayment.SetLayoutManager(mLayoutManager);
            report_adapter_customer.ItemClick += Report_Adapter_Customer_ItemClick;

            if (report_adapter_customer.ItemCount == 0 && !string.IsNullOrEmpty(SearchName))
            {
                rcvPayment.Visibility = Android.Views.ViewStates.Gone;
                lnNoDataSearch.Visibility = ViewStates.Visible;
            }
            else
            {
                rcvPayment.Visibility = Android.Views.ViewStates.Visible;
                lnNoDataSearch.Visibility = ViewStates.Gone;
            }


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

                Report_Adapter_ChoosePayment report_adapter_customer = new Report_Adapter_ChoosePayment(listPayment);
                GridLayoutManager mLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvPayment.SetAdapter(report_adapter_customer);
                rcvPayment.HasFixedSize = true;
                rcvPayment.SetLayoutManager(mLayoutManager);
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

        private async Task<List<PaymentType>> SetListPayment()
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
        public static string paymentSelect;
        public static List<PaymentType> listChoosePayment = new List<PaymentType>();
        private void SetShowButton()
        {
            if (paymentSelect != "All Payment")
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnAll.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAll.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }

            if (listChoosePayment.Count > 0)
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApply.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApply.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }

        }
        LinearLayout lnBack;
        FrameLayout lnSearch;
        ImageButton btnSearch;
        EditText textSearch;
        Button btnAll;
        RecyclerView rcvPayment;
        LinearLayout lnNoDataSearch;
        Button btnApply;

        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnSearch = view.FindViewById<FrameLayout>(Resource.Id.lnSearch);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            textSearch = view.FindViewById<EditText>(Resource.Id.textSearch);
            btnAll = view.FindViewById<Button>(Resource.Id.btnAll);
            rcvPayment = view.FindViewById<RecyclerView>(Resource.Id.rcvPayment);
            lnNoDataSearch = view.FindViewById<LinearLayout>(Resource.Id.lnNoDataSearch);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
        }
    }


}