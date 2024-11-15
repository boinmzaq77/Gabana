using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Employee
{
    public class Employee_Fragment_CheckUser : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Employee_Fragment_CheckUser NewInstance()
        {
            Employee_Fragment_CheckUser frag = new Employee_Fragment_CheckUser();
            return frag;
        }
        Employee_Fragment_CheckUser fragment_CheckUser;
        View view;
        string SearchName, LoginType;
        public static bool CheckNet = false;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.employee_fragment_checkuser, container, false);
            try
            {
                CombineUI();
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        LinearLayout lnback;
        public static EditText txtemployeeUsername;
        Button btnNext;
        private async void CombineUI()
        {
            lnback = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            txtemployeeUsername = view.FindViewById<EditText>(Resource.Id.txtemployeeUsername);
            btnNext = view.FindViewById<Button>(Resource.Id.btnNext);

            lnback.Click += Lnback_Click; 
            btnNext.Click += BtnNext_Click; 

            txtemployeeUsername.TextChanged += TxtemployeeUsername_TextChanged; ;
            TokenResult res = await TokenServiceBase.GetToken();
            if (!res.status)
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                return;
            }

        }
        public static string Username;
        public static UserAccount resultAccount;

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                btnNext.Enabled = false;
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }
                
                if (!DataCashing.CheckNet)
                {
                    dialogLoading.Dismiss();
                    btnNext.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Long).Show();
                    return;
                }

                if (!Regex.IsMatch(txtemployeeUsername.Text, @"^[a-z0-9_@]+$"))
                {
                    dialogLoading.Dismiss();
                    btnNext.Enabled = true;
                    Toast.MakeText(this.Activity, "สามารถกรอกได้เฉพาะตัวภาษาอังกฤษ (เฉพาะตัวเล็กเท่านั้น), ตัวเลข, มี '_' หรือ '@' ได้", ToastLength.Short).Show();
                    return;
                }

                Username = txtemployeeUsername.Text;

                if (string.IsNullOrEmpty(Username))
                {
                    dialogLoading.Dismiss();
                    btnNext.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }

                var getUserfromGabana = await GabanaAPI.GetDataUserAccount(Username);
                if (getUserfromGabana != null)
                {
                    // pop up แสดงว่ามีข้อมูลแล้วว
                    dialogLoading.Dismiss();
                    btnNext.Enabled = true;
                    var fragment = new Employee_Dailog_UserDublicate();
                    fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Employee_Dailog_UserDublicate));
                    return;
                }

                //Seauth
                resultAccount = await GabanaAPI.GetSeAuthDataUserAccount(Username);
                if (resultAccount != null) 
                {
                    dialogLoading.Dismiss();
                    btnNext.Enabled = true;
                    var fragment = new Employee_Dialog_SelectDataUser();
                    fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Employee_Dialog_SelectDataUser));
                    return;
                }

                Employee_Fragment_AddEmployee.EmployeeUsername = Username;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnEmployee, "employee", "addemployee");
                txtemployeeUsername.Text = string.Empty;
                btnNext.Enabled = true;

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                btnNext.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnNext_Click");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Lnback_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnEmployee, "employee", "default");
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
                btnNext.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnNext.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
            else
            {
                btnNext.Enabled = true;
                btnNext.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnNext.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
        }

    }
}