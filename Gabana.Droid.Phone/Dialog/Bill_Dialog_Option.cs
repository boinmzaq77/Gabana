using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid.Phone
{
    public class Bill_Dialog_Option : Android.Support.V4.App.DialogFragment
    {
        TextView txtcancel;
        LinearLayout lnVoid;

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static Bill_Dialog_Option NewInstance()
        {
            var frag = new Bill_Dialog_Option { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.bill_dialog_option, container, false);
            try
            {
                Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);

                txtcancel = view.FindViewById<TextView>(Resource.Id.txtcancel);
                txtcancel.Click += Txtcancel_Click;

                lnVoid = view.FindViewById<LinearLayout>(Resource.Id.lnVoid);
                lnVoid.Click += LnVoid_Click;


            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();

            }
            return view;
        }

        private void LnVoid_Click(object sender, EventArgs e)
        {
            this.Dismiss();

            Android.Support.V4.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
            MainDialog dialog = new MainDialog();
            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.void_dialog_main.ToString();
            bundle.PutString("message", myMessage);
            dialog.Arguments = bundle;
            dialog.Show(transaction, myMessage);
            //Dismiss();
            //Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
            //Void_Dialog_Main dialog = new Void_Dialog_Main();
            //var transactionId = dialog.Show(ft, "VoidDialog");
        }

        private void Txtcancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

    }
}