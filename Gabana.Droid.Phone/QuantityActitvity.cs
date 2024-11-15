using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class QuantityActitvity : Activity
    {
        ImageButton imgBack, btndeletenumber, imgDecrease, imgIncrease;
        Button btnOK, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0;
        TextView editValue;
        int getValue;
        string strcurrent;
        public QuantityActitvity quantity;
        public int setQuanity;
        //public int? seValuetQuantity;
        private static int? backtoQuantity;
        string DECIMALPOINTDISPLAY;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.quantity_activity_main);

                imgBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);

                imgDecrease = FindViewById<ImageButton>(Resource.Id.imageButtonDecrease);
                imgIncrease = FindViewById<ImageButton>(Resource.Id.imageButtonIncrease);
                editValue = FindViewById<TextView>(Resource.Id.editTextQuantity);
                btnOK = FindViewById<Button>(Resource.Id.buttonOK);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
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

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImgBack_Click;
                imgBack.Click += ImgBack_Click;
                //editValue.TextChanged += EditValue_TextChanged;
                btnOK.Click += BtnOK_Click;
                imgDecrease.Click += ImgDecrease_Click;
                imgIncrease.Click += ImgIncrease_Click;
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

                CheckJwt();

                setQuanity = backtoQuantity == null ? 0 : backtoQuantity.Value;
                if (setQuanity != 0)
                {
                    strcurrent = setQuanity.ToString();
                }
                else
                {
                    strcurrent = "1";
                }
                strcurrent = Utils.CheckLenghtValue(strcurrent);
                editValue.Text = (Convert.ToInt32(strcurrent)).ToString("#,##0");
                btnOK.SetBackgroundResource(Resource.Drawable.btnblue);
                btnOK.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;

                editValue.Focusable = true;
                editValue.TextChanged += EditValue_TextChanged1;
                strcurrent = string.Empty;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Quantity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void EditValue_TextChanged1(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                decimal value = 0;
                if (Decimal.TryParse(editValue.Text, out value))
                {
                    if (value > 0)
                    {
                        btnOK.Enabled = true;
                        btnOK.SetBackgroundResource(Resource.Drawable.btnblue);
                        btnOK.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    }
                    else
                    {
                        btnOK.Enabled = false;
                        btnOK.SetBackgroundResource(Resource.Drawable.btnborderblue);
                        btnOK.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber0);
        }

        private void ImgIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                int temp;
                strcurrent = editValue.Text;
                if (strcurrent == string.Empty)
                {
                    strcurrent = "1";
                    editValue.Text = Convert.ToInt32(strcurrent).ToString("#,##0");
                }
                else
                {
                    strcurrent = Utils.CheckLenghtValue(strcurrent);
                    temp = int.Parse(strcurrent);
                    strcurrent = temp + 1 + "";
                    if (Convert.ToInt32(strcurrent) > 999999)
                    {
                        editValue.Text = 999999.ToString("#,##0");
                        strcurrent = "999999";
                    }
                    else
                    {
                        editValue.Text = Convert.ToInt32(strcurrent).ToString("#,##0");
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgIncrease_Click at Quantity");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        private async void ImgDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                int temp;
                strcurrent = editValue.Text;
                strcurrent = Utils.CheckLenghtValue(strcurrent);
                switch (strcurrent)
                {
                    case "":
                        editValue.Text = string.Empty;
                        break;
                    case "0":
                        editValue.Text = "0";
                        break;
                    case "1":
                        editValue.Text = "1";
                        break;
                    default:
                        temp = int.Parse(strcurrent);
                        strcurrent = temp - 1 + "";
                        editValue.Text = Convert.ToInt64(strcurrent).ToString("#,##0");
                        break;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgDecrease_Click at Quantity");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                decimal value = 0, data = 0;
                if (Decimal.TryParse(editValue.Text, out value))
                {
                    if (value > 0)
                    {
                        data = value;
                    }
                    else
                    {
                        data = 0;
                    }
                }

                if (DataCashing.flagEditQuantity)
                {
                    setQuanity = (int)data;
                    DataCashing.EditQuantity = setQuanity;
                    this.Finish();
                }
                else
                {
                    setQuanity = (int)data;
                    PosActivity.setQuantity = setQuanity;
                    DataCashing.setQuantityToCart = setQuanity;
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnOK_Click at Quantity");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        internal static void SetBackQuantity(int? i)
        {
            backtoQuantity = i;
        }

        private async void EditValue_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                strcurrent = editValue.Text;
                if (strcurrent == string.Empty)
                {
                    editValue.Text = "0";
                    editValue.Focusable = true;
                    Toast.MakeText(this, "กรุณากรอกจำนวน", ToastLength.Long).Show();
                }
                else
                {
                    getValue = Int32.Parse(editValue.Text);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("EditValue_TextChanged at Quantity");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        private void ImgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            strcurrent = editValue.Text.Trim();
            if (strcurrent != string.Empty && strcurrent.Length > 1)
            {
                strcurrent = Utils.CheckLenghtValue(strcurrent);
                strcurrent = strcurrent.Remove(strcurrent.Length - 1);
                editValue.Text = Convert.ToInt64(Utils.CheckLenghtValue(strcurrent)).ToString("#,##0");
                editValue.Focusable = true;
                return;
            }

            //กรณีกดลบจนเหลือตัวสุดท้าย ถ้าลบอีกครั้งให้เป็น 1 //ui ให้เปลี่ยนเป็นเหลือ 0
            if (strcurrent != string.Empty && strcurrent.Length == 1)
            {
                strcurrent = "0";
                editValue.Text = strcurrent;
                editValue.Focusable = true;
                return;
            }
        }

        public async void SetQuantity(Button btn)
        {
            try
            {
                string amount = "0";
                if (!string.IsNullOrEmpty(editValue.Text))
                {
                    amount = editValue.Text;
                }
                else
                {
                    amount = "0";
                }

                btn.RequestFocus();
                var num = btn.Text.ToString();
                switch (num)
                {
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
                        break;
                }

                decimal data = 0, checkValue = 0;
                //if (Int32.TryParse(amount.Replace(",", ""), out data))
                if (Decimal.TryParse(amount, out data))
                {
                    if (data > 0)
                    {
                        checkValue = data;
                    }
                    else
                    {
                        checkValue = 0;
                    }
                }

                //var data = Convert.ToInt32(amount);
                if (checkValue > 999999)
                {
                    Toast.MakeText(this, Application.Context.GetString(Resource.String.maxitem) + " 999,999", ToastLength.Short).Show();
                    editValue.Text = "999,999";
                    strcurrent = editValue.Text;
                    decimal value1 = 0;
                    //if (Int32.TryParse(strcurrent.Replace(",", ""), out value1))
                    if (Decimal.TryParse(strcurrent, out value1))
                    {
                        if (value1 > 0)
                        {
                            setQuanity = (int)value1;
                        }
                        else
                        {
                            setQuanity = 0;
                        }
                    }
                    return;
                }

                editValue.Text = checkValue.ToString("#,##0");
                strcurrent = editValue.Text;
                decimal value = 0;
                //if (Int32.TryParse(strcurrent.Replace(",",""), out value))
                if (Decimal.TryParse(strcurrent, out value))
                {
                    if (value > 0)
                    {
                        setQuanity = (int)value;
                    }
                    else
                    {
                        setQuanity = 0;
                    }
                }
                editValue.Focusable = true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetQuantity at Quantity");
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                return;
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            PosActivity.setQuantity = 1;
            DataCashing.setQuantityToCart = 1;
            DataCashing.flagEditQuantity = false;
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'QuantityActitvity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'QuantityActitvity.openPage' is assigned but its value is never used
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

            CheckJwt();
        }
    }
}