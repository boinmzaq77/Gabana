using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Employee_Dailog_UserDublicate : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Employee_Dailog_UserDublicate NewInstance()
        {
            var frag = new Employee_Dailog_UserDublicate { Arguments = new Bundle() };
            return frag;
        }
        Button btn_save;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.employee_dialog_userduplicate, container, false);
            try
            {
                TextView textUser = view.FindViewById<TextView>(Resource.Id.textUser);

                var text1 = GetString(Resource.String.employee_dialog_user);
                textUser.Text = text1 + " " + Employee_Fragment_CheckUser.Username;

                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += Btn_save_Click; 

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void Btn_save_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }
    }
}