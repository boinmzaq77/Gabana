using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabana.Droid.Tablet.Fragments.Employee;
using TinyInsightsLib;
using Gabana.ORM.MerchantDB;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Employee_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        Button btnCancle, btnOK;
        public static UserAccountInfo LocalEmployee;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Employee_Dialog_Delete NewInstance()
        {
            var frag = new Employee_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.employee_dialog_delete, container, false);
            try
            {
                btnCancle = view.FindViewById<Button>(Resource.Id.btnCancle);
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnCancle.Click += BtnCancle_Click;
                btnOK.Click += BtnOK_Click; 
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //09/03/66 test delete
                UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                BranchPolicyManage policyManage = new BranchPolicyManage();

                var mainrole = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == LocalEmployee.UserName.ToLower() & x.MainRoles.ToLower() == "owner").FirstOrDefault();
                if (mainrole != null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdeleteowner), ToastLength.Short).Show();
                    this.Dialog.Dismiss();
                    return;
                }

                var result = await GabanaAPI.DeleteSeAuthDataUserAccount(LocalEmployee.UserName.ToLower());
                if (!result.Status)
                {
                    Toast.MakeText(this.Activity, result.Message, ToastLength.Short).Show();
                    this.Dialog.Dismiss();
                    return;
                }

                var resultGabana = await GabanaAPI.DeleteDataUserAccount(LocalEmployee.UserName.ToLower());
                if (!resultGabana.Status)
                {
                    Toast.MakeText(this.Activity, resultGabana.Message, ToastLength.Short).Show();
                    this.Dialog.Dismiss();
                    return;
                }

                //Delete BranchPolicy ของ employee 
                var lstbranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, LocalEmployee.UserName.ToLower());
                foreach (var item in lstbranchPolicy)
                {
                    var delete = await policyManage.DeleteBranch(DataCashingAll.MerchantId, LocalEmployee.UserName.ToLower());
                }

                //Delete useraccount
                var resultLocal = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, LocalEmployee.UserName.ToLower());
                if (!resultLocal)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    this.Dialog.Dismiss();
                    return;
                }

                var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName == LocalEmployee.UserName.ToLower());
                DataCashingAll.UserAccountInfo.Remove(data);
                Employee_Fragment_Main.fragment_main.DeleteEmployee(LocalEmployee);
                Toast.MakeText(Application.Context, "ลบสำเร็จ", ToastLength.Short).Show();
                Employee_Fragment_AddEmployee.fragment_main.SetClearData();
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Employee_Dialog_Delete");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

        public static void SetEmployeeDetail(UserAccountInfo DeleteEmployee)
        {
            LocalEmployee = DeleteEmployee;
        }

    }
}