using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Employee_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static string Page, Username;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Employee_Dialog_Delete NewInstance(string _username, string _page)
        {
            Username = _username;
            Page = _page;
            var frag = new Employee_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_deleteitem, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

                textconfirm1 = view.FindViewById<TextView>(Resource.Id.textconfirm1);
                textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);
                textconfirm1.Text = GetString(Resource.String.dialog_delete_employee_1);
                textconfirm2.Text = GetString(Resource.String.dialog_delete_employee_2);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                btn_save.Enabled = false;
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                BranchPolicyManage policyManage = new BranchPolicyManage();

                var mainrole = DataCashingAll.UserAccountInfo.Where(x => x.UserName == Username & x.MainRoles.ToLower() == "owner").FirstOrDefault();
                if (mainrole != null)
                {
                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdeleteowner), ToastLength.Short).Show();
                    return;
                }

                var result = await GabanaAPI.DeleteSeAuthDataUserAccount(Username);
                if (!result.Status)
                {
                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, result.Message, ToastLength.Short).Show();
                    return;
                }

                var resultGabana = await GabanaAPI.DeleteDataUserAccount(Username);
                if (!resultGabana.Status)
                {
                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, resultGabana.Message, ToastLength.Short).Show();
                    return;
                }

                //Delete BranchPolicy ของ employee 
                var lstbranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, Username);
                foreach (var item in lstbranchPolicy)
                {
                    var delete = await policyManage.DeleteBranch(DataCashingAll.MerchantId, Username);
                }

                //Delete useraccount
                var resultLocal = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, Username);
                if (!resultLocal)
                {
                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    return;
                }

                var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName == Username);
                DataCashingAll.UserAccountInfo.Remove(data);
                EmployeeActivity.flagEmployeeChange = true;
                if (Page != "main")
                {
                    //หน้า add                       
                    this.Activity.Finish();
                }
                else
                {
                    EmployeeActivity.employeeActivity.Resume();
                }

                btn_save.Enabled = true;
                Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                MainDialog.CloseDialog();
            }
            catch (Exception ex)
            {
                btn_save.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUBtn_save_Clickpdate at employee_dialog");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}
