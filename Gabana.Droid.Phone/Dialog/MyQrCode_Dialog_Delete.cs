using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class MyQrCode_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static MyQrCode_Dialog_Delete NewInstance()
        {
            var frag = new MyQrCode_Dialog_Delete { Arguments = new Bundle() };
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
                textconfirm1.Text = Application.Context.GetString(Resource.String.dialog_delete_myqrcode_1);
                textconfirm2.Text = Application.Context.GetString(Resource.String.dialog_delete_myqrcode_2);

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

                if (await GabanaAPI.CheckSpeedConnection())
                {
                    string pathImage = "";
                    var QrNo = (int)AddmyQRActivity.MyQrCodeData.MyQrCodeNo;
                    var result = await GabanaAPI.DeleteDataMyQrCode(QrNo);
                    if (result.Status)
                    {
                        Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                        MyQrCodeManage MyQrCodeManage = new MyQrCodeManage();
                        var data = await MyQrCodeManage.GetMyQrCode(DataCashingAll.MerchantId, QrNo);
                        if (data != null)
                        {
                            pathImage = data.PictureLocalPath;
                        }

                        var delete = await MyQrCodeManage.DeleteMyQrCode(DataCashingAll.MerchantId, QrNo);
                        if (delete && !string.IsNullOrEmpty(pathImage))
                        {
                            Java.IO.File imgFile = new Java.IO.File(pathImage);

                            if (System.IO.File.Exists(pathImage))
                            {
                                System.IO.File.Delete(pathImage);
                            }
                        }

                        AddmyQRActivity.MyQrCodeData = null;
                        DataCashingAll.flagMyQrCodeChange = true;
                        MainDialog.CloseDialog();
                        this.Activity.Finish();
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at qr_dialog_delete");
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}
