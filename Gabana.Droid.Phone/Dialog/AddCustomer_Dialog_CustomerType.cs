using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class AddCustomer_Dialog_CustomerType : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static AddCustomer_Dialog_CustomerType NewInstance()
        {
            var frag = new AddCustomer_Dialog_CustomerType { Arguments = new Bundle() };
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
                textconfirm1.Text = GetString(Resource.String.addcustomer_dialog_customerType_1);
                textconfirm2.Text = GetString(Resource.String.addcustomer_dialog_customerType_2);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }
        private void Btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(MemberTypeActivity)));
                this.Activity.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at addcus");
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
