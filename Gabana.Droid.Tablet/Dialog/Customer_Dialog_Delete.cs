using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Customer_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Customer_Dialog_Delete NewInstance()
        {
            var frag = new Customer_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }
        Button btn_save, btn_cancel;
        private Customer cusdelete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.customer_dialog_delete, container, false);
            try
            {
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += Btn_save_Click;

                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += Btn_cancel_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            //Delete Customer ที่เลือก 
            try
            {
                if (DataCashing.EditCus != null)
                {
                    cusdelete = DataCashing.EditCus;
                }

                btn_save.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    btn_save.Enabled = true;
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                CustomerManage customerManage = new CustomerManage();
                cusdelete.DataStatus = 'D';
                cusdelete.FWaitSending = 2;
                cusdelete.LastDateModified = DateTime.UtcNow;
                var result = await customerManage.UpdateCustomer(cusdelete);
                if (!result)
                {
                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    if (Customer_Fragment_Main.fragment_main != null)
                    {
                        DataCashingAll.flagCustomerChange = true;
                        Customer_Fragment_Main.fragment_main.OnResume();
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

                Toast.MakeText(Application.Context, GetString(Resource.String.deletesucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCustomer(DataCashingAll.MerchantId, Convert.ToInt32(DataCashing.EditCus.SysCustomerID));
                }
                else
                {
                    cusdelete.FWaitSending = 2;
                    await customerManage.UpdateCustomer(cusdelete);
                }

                DataCashingAll.flagCustomerChange = true;
                DataCashing.EditCus = null;
                Customer_Fragment_AddCustomer.customerEdit = null;                
                Customer_Fragment_AddCustomer.keepCropedUri = null;
                Customer_Fragment_Main.fragment_main.DeleteCustomer(cusdelete);
                Dismiss();
                btn_save.Enabled = true;
            }
            catch (Exception ex)
            {
                btn_save.Enabled = true;
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }



    }
}