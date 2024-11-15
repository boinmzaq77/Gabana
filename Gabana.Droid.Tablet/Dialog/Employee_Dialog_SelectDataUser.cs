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
    public class Employee_Dialog_SelectDataUser : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Employee_Dialog_SelectDataUser NewInstance()
        {
            var frag = new Employee_Dialog_SelectDataUser { Arguments = new Bundle() };
            return frag;
        }
        Button btn_cancel;
        Button btn_save;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.employee_dialog_selectdatauser, container, false);
            try
            {
                TextView textUser = view.FindViewById<TextView>(Resource.Id.textComfirmUser);

                var text1 = GetString(Resource.String.employee_dialog_text1);
                var text2 = GetString(Resource.String.employee_dialog_text2);

                textUser.Text = text1 + " " + Employee_Fragment_CheckUser.Username + " " + text2;

                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += BtnCancle_Click; 
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }
        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Employee_Fragment_CheckUser.txtemployeeUsername.Text = string.Empty;
            Employee_Fragment_AddEmployee.EmployeeDetail = Employee_Fragment_CheckUser.resultAccount;
            DataCashing.addEmployeefromSeauth = true;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnEmployee, "employee", "addemployee");
            this.Dismiss();
        }
    }
}