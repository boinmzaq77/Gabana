using Android.App;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class OnhandActivity : Activity
    {
        ImageButton imagebtnBack, btndeletenumber;
        TextView txtAmount;
        Button btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, buttonCharge, btnClear;
        string strValue = "0"; //strValue คือ จำนวนเงินที่จะจ่าย
        int count = 0;
        LinearLayout btnAdd5, btnAdd10, btnAdd50, btnAdd100;
        private static string ipage;
        private static string balanceStock;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.onhand_activity_main);
                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtAmount = FindViewById<TextView>(Resource.Id.txtAmount);
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
                btnAdd5 = FindViewById<LinearLayout>(Resource.Id.btnAdd5);
                btnAdd50 = FindViewById<LinearLayout>(Resource.Id.btnAdd50);
                btnAdd10 = FindViewById<LinearLayout>(Resource.Id.btnAdd10);
                btnAdd100 = FindViewById<LinearLayout>(Resource.Id.btnAdd100);
                btnClear = FindViewById<Button>(Resource.Id.btnClear);

                imagebtnBack.Click += ImagebtnBack_Click;
                btndeletenumber.Click += Btndeletenumber_Click;
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

                btnAdd5.Click += BtnAdd5_Click;
                btnAdd10.Click += BtnAdd10_Click;
                btnAdd50.Click += BtnAdd50_Click;
                btnAdd100.Click += BtnAdd100_Click;
                btnClear.Click += BtnClear_Click;

                txtAmount.Text = balanceStock;
                CheckJwt();
                SetBtnCharge();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : OnhandActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtAmount.Text = "0";
            strValue = txtAmount.Text;
            SetBtnCharge();
        }

        private void BtnAdd100_Click(object sender, EventArgs e)
        {
            txtAmount.Text = "100";
            strValue = txtAmount.Text;
            SetBtnCharge();
        }

        private void BtnAdd50_Click(object sender, EventArgs e)
        {
            txtAmount.Text = "50";
            strValue = txtAmount.Text;
            SetBtnCharge();
        }

        private void BtnAdd10_Click(object sender, EventArgs e)
        {
            txtAmount.Text = "10";
            strValue = txtAmount.Text;
            SetBtnCharge();
        }

        private void BtnAdd5_Click(object sender, EventArgs e)
        {
            txtAmount.Text = "5";
            strValue = txtAmount.Text;
            SetBtnCharge();
        }

        private void SetBtnCharge()
        {
            if (txtAmount.Text != "")
            {
                btndeletenumber.Enabled = true;
                buttonCharge.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                buttonCharge.SetBackgroundResource(Resource.Drawable.btnblue);
            }
            else
            {
                btndeletenumber.Enabled = false;
                buttonCharge.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                buttonCharge.SetBackgroundResource(Resource.Drawable.btnborderblue);

            }
        }

        private void ButtonCharge_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAmount.Text != string.Empty)
                {

                    switch (ipage)
                    {
                        case "Additem":
                            AddItemActivity.SetOnhand(txtAmount.Text);
                            this.Finish();
                            break;
                        case "AddExtra":
                            AddExtraToppingActivity.SetOnhand(txtAmount.Text);
                            this.Finish();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                buttonCharge.Enabled = true;
                return;
            }
        }

        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber0);
        }

        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                decimal damount;
                string amount;
                int indexclear = 0;
                strValue = txtAmount.Text;

                if (strValue.Contains('-'))
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    if (amount == "-")
                    {
                        damount = 0;
                    }
                    else
                    {
                        damount = Convert.ToInt32(amount);
                    }
                    txtAmount.Text = damount.ToString("#,##0");
                    strValue = txtAmount.Text;
                    txtAmount.Focusable = true;
                    indexclear = txtAmount.Text.LastIndexOf(".");
                    return;
                }

                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = Utils.CheckLenghtValue(strValue);
                    strValue = strValue.Remove(strValue.Length - 1);
                    txtAmount.Text = Convert.ToInt64(Utils.CheckLenghtValue(strValue)).ToString("#,##0");
                    txtAmount.Focusable = true;
                    return;
                }

                //กรณีกดลบจนเหลือตัวสุดท้าย ถ้าลบอีกครั้งให้เป็น 1 //ui ให้เปลี่ยนเป็นเหลือ 0
                if (strValue != string.Empty && strValue.Length == 1)
                {
                    strValue = "0";
                    txtAmount.Text = strValue;
                    return;
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
            OnBackPressed();
        }

        public void SetValue(Button btn)
        {
            string amount = "0";
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                var datas = Utils.CheckLenghtValue(txtAmount.Text);
                amount = (Convert.ToInt64(datas)).ToString("#,##0");
            }
            else
            {
                amount = "0";
            }
            var num = btn.Text.ToString();
            btn.RequestFocus();

            switch (num)
            {
                case "0":
                    amount += num;
                    break;
                case "1":
                    amount += num;
                    break;
                case "2":
                    amount += num;
                    break;
                case "3":
                    amount += num;
                    break;
                case "4":
                    amount += num;
                    break;
                case "5":
                    amount += num;
                    break;
                case "6":
                    amount += num;
                    break;
                case "7":
                    amount += num;
                    break;
                case "8":
                    amount += num;
                    break;
                case "9":
                    amount += num;
                    break;
                default:
                    amount += num;
                    count++;
                    break;
            }

            var data = Utils.CheckLenghtValue(amount);
            if (data.Length > 6)
            {
                Toast.MakeText(this, GetString(Resource.String.maxitem) + " 999,999", ToastLength.Short).Show();
                txtAmount.Text = "999,999";
                strValue = txtAmount.Text;
                return;
            }
            txtAmount.Text = (Convert.ToInt64(data)).ToString("#,##0");
            strValue = txtAmount.Text;
            SetBtnCharge();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        internal static void SetPageView(string v, string t)
        {
            ipage = v;
            balanceStock = t;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'OnhandActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'OnhandActivity.openPage' is assigned but its value is never used
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