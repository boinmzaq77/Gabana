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
using Gabana.ORM.MerchantDB;
using static Java.Util.Jar.Attributes;
using Gabana.Droid.Tablet.Fragments.Customers;
using TinyInsightsLib;
using Gabana.Model;
using Newtonsoft.Json;
using Android.Icu.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class AddCustomer_Dialog_Dubicate : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static AddCustomer_Dialog_Dubicate NewInstance()
        {
            var frag = new AddCustomer_Dialog_Dubicate { Arguments = new Bundle() };
            return frag;
        }
        Button btn_cancel, btn_save;
        TextView txtDetail;
        static string CustomerName = "";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.addcustomer_dialog_dubicate, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);
                txtDetail.Text = string.Empty;

                string txtdialog = string.Empty;
                var textItemName = CustomerName;
                var text1 = GetText(Resource.String.dialog_addcustomer);
                var text2 = GetText(Resource.String.dialog_additem2);

                txtdialog = text1 + " " + textItemName + " " + text2;
                txtdialog += " " + GetString(Resource.String.dialog_additem3);
                 
                txtDetail.Text += txtdialog;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        public static void SetCustomerName(string _customername)
        {
            CustomerName = _customername;
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

                Customer_Fragment_AddCustomer.fragment_main.ManageCustomer();
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
    }
}