using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;

namespace Gabana.Droid.Phone
{
    public class Customer_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static int CustomerID;
        static string Page;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Customer_Dialog_Delete NewInstance(int customerID, string _page)
        {
            CustomerID = customerID;
            Page = _page;
            var frag = new Customer_Dialog_Delete { Arguments = new Bundle() };
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
                textconfirm1.Text = string.Empty;
                textconfirm2.Text = string.Empty;
                textconfirm1.Text = GetString(Resource.String.dialog_delete_customer_1);
                textconfirm2.Text = GetString(Resource.String.dialog_delete_customer_2);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            //Delete Customer ที่เลือก 
            try
            {
                btn_save.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    btn_save.Enabled = true;
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                CustomerManage CustomerManage = new CustomerManage();
                var cusdelete = await CustomerManage.GetCustomer(DataCashingAll.MerchantId, Convert.ToInt32(CustomerID));
                cusdelete.DataStatus = 'D';
                cusdelete.FWaitSending = 2;
                cusdelete.LastDateModified = DateTime.UtcNow;
                var result = await CustomerManage.UpdateCustomer(cusdelete);                
                if (!result)
                {
                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    if (Page == "main")
                    {
                        DataCashingAll.flagCustomerChange = true;
                        CustomerActivity.customerActivity.Resume();
                    }
                    return;
                }

                if (!string.IsNullOrEmpty(cusdelete.ThumbnailLocalPath))
                {
                    Java.IO.File imgFile = new Java.IO.File(cusdelete.ThumbnailLocalPath);
                    if (System.IO.File.Exists(imgFile.AbsolutePath))
                    {
                        System.IO.File.Delete(imgFile.AbsolutePath);
                    }
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCustomer(DataCashingAll.MerchantId, Convert.ToInt32(CustomerID));
                }
                else
                {
                    cusdelete.FWaitSending = 2;
                    await CustomerManage.UpdateCustomer(cusdelete);
                }
                AddCustomerActivity.iSysCustomerID = 0;
                DataCashingAll.flagCustomerChange = true;
                if (Page != "main")
                {
                    //หน้า add                       
                    this.Activity.Finish();
                }
                else
                {
                    CustomerActivity.customerActivity.Resume();
                }
                MainDialog.CloseDialog();
                btn_save.Enabled = true;
            }
            catch (Exception ex)
            {
                btn_save.Enabled = true;
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
