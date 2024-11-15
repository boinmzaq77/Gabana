using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Text.RegularExpressions;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class CheckNewUserActivity : AppCompatActivity
    {
        LinearLayout lnback;
        EditText txtemployeeUsername;
        Button btnNext;
        public static string Username;
        public static Gabana.Model.UserAccount resultAccount;
        public static CheckNewUserActivity activity;
        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.checknewuser_activity_main);
                activity = this;
                lnback = FindViewById<LinearLayout>(Resource.Id.lnBack);
                txtemployeeUsername = FindViewById<EditText>(Resource.Id.txtemployeeUsername);
                btnNext = FindViewById<Button>(Resource.Id.btnNext);

                lnback.Click += Lnback_Click;
                btnNext.Click += BtnNext_Click;

                txtemployeeUsername.TextChanged += TxtemployeeUsername_TextChanged;
                CheckJwt();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : CheckNewUserActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at checknewUser");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtemployeeUsername_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SetBtnApply();
        }

        private void SetBtnApply()
        {
            if (String.IsNullOrEmpty(txtemployeeUsername.Text))
            {
                btnNext.Enabled = false;
                btnNext.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnNext.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
            else
            {
                btnNext.Enabled = true;
                btnNext.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnNext.SetBackgroundResource(Resource.Drawable.btnblue);
            }
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Long).Show();
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    return;
                }

                if (!Regex.IsMatch(txtemployeeUsername.Text, @"^[a-z0-9_@]+$"))
                {
                    Toast.MakeText(this, "สามารถกรอกได้เฉพาะตัวภาษาอังกฤษ (เฉพาะตัวเล็กเท่านั้น), ตัวเลข, มี '_' หรือ '@' ได้", ToastLength.Short).Show();
                    return;
                }

                Username = txtemployeeUsername.Text;

                if (string.IsNullOrEmpty(Username))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }

                //ResultAPI resultAPI = await CheckUsername();
                var getUserfromGabana = await GabanaAPI.GetDataUserAccount(Username);
                if (getUserfromGabana != null)
                {
                    // pop up แสดงว่ามีข้อมูลแล้วว
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.employee_dialog_unadduser.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }

                //Seauth
                resultAccount = await GabanaAPI.GetSeAuthDataUserAccount(Username);
                if (resultAccount != null)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.employee_dialog_selectdatauser.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }

                AddEmployeeActivity.EmployeeUsername = Username;
                StartActivity(new Android.Content.Intent(Application.Context, typeof(AddEmployeeActivity)));
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnNext_Click");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnEmpRole_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeRoleActivity)));
        }

        private void Lnback_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            this.Finish();
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
    }
}