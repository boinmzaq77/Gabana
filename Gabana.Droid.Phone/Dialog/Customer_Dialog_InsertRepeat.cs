using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Customer_Dialog_InsertRepeat : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
#pragma warning disable CS0649 // Field 'Customer_Dialog_InsertRepeat.name' is never assigned to, and will always have its default value null
#pragma warning disable CS0649 // Field 'Customer_Dialog_InsertRepeat.Detail' is never assigned to, and will always have its default value null
#pragma warning disable CS0649 // Field 'Customer_Dialog_InsertRepeat.Event' is never assigned to, and will always have its default value null
        static string name, Detail, Event;
#pragma warning restore CS0649 // Field 'Customer_Dialog_InsertRepeat.Event' is never assigned to, and will always have its default value null
#pragma warning restore CS0649 // Field 'Customer_Dialog_InsertRepeat.Detail' is never assigned to, and will always have its default value null
#pragma warning restore CS0649 // Field 'Customer_Dialog_InsertRepeat.name' is never assigned to, and will always have its default value null
        TextView textconfirm1, textconfirm2;
        CustomerManage customerManage = new CustomerManage();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Customer_Dialog_InsertRepeat NewInstance(string _name, string _detailinsert, string _event)
        {

            var frag = new Customer_Dialog_InsertRepeat { Arguments = new Bundle() };
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

                var textItemName = name;
                var text1 = GetText(Resource.String.dialog_addcustomer);
                var text2 = GetText(Resource.String.dialog_additem2);

                textconfirm1.Text = text1 + " " + textItemName + " " + text2;
                textconfirm2.Text = GetString(Resource.String.dialog_additem3);

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
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                var DetailCustomer = JsonConvert.DeserializeObject<Customer>(Detail);
                if (DetailCustomer != null)
                {
                    if (Event == "insert")
                    {
                        DetailInsert(DetailCustomer);
                    }
                    else
                    {
                        DetailUpdate(DetailCustomer);
                    }
                    MainDialog.CloseDialog();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }


        async void DetailInsert(Customer customer)
        {
            try
            {
                var result = await customerManage.InsertCustomer(customer);
                if (!result)
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                }

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                }
                else
                {
                    customer.FWaitSending = 2;
                    await customerManage.UpdateCustomer(customer);
                }

                //CustomerActivity.SetFocusCustomer(customer.SysCustomerID);
                CustomerActivity.SetFocusCustomer(customer);
                AddCustomerActivity.addCustomer.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailInsert at customer_dialog");
            }
        }

        async void DetailUpdate(Customer customer)
        {
            try
            {
                var result = await customerManage.UpdateCustomer(customer);
                if (!result)
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                }

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                }
                else
                {
                    customer.FWaitSending = 2;
                    await customerManage.UpdateCustomer(customer);
                }
                //CustomerActivity.SetFocusCustomer(customer.SysCustomerID);
                CustomerActivity.SetFocusCustomer(customer);
                AddCustomerActivity.iSysCustomerID = 0;
                AddCustomerActivity.addCustomer.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUpdate at customer_dialog");
            }
        }
    }
}
