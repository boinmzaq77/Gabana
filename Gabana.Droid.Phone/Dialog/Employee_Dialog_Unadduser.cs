using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class Employee_Dialog_Unadduser : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel;
        Button btn_save;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Employee_Dialog_Unadduser NewInstance()
        {
            var frag = new Employee_Dialog_Unadduser { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.employee_dialog_unadduser, container, false);
            try
            {
                TextView textUser = view.FindViewById<TextView>(Resource.Id.textUser);

                var text1 = GetString(Resource.String.employee_dialog_user);
                textUser.Text = text1 + " " + CheckNewUserActivity.Username;

                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += BtnCancle_Click; ;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {

            MainDialog.CloseDialog();

        }
    }
}