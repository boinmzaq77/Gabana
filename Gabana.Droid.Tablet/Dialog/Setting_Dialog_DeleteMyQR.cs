using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Gabana.Model;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dialog_DeleteMyQR : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Dialog_DeleteMyQR NewInstance()
        {
            var frag = new Setting_Dialog_DeleteMyQR { Arguments = new Bundle() };
            return frag;
        }
        View view;
        Button btnCancel, btnSave;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_dialog_deletemyqr, container, false);
            try
            {
                btnCancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btnSave = view.FindViewById<Button>(Resource.Id.btn_save);
                btnCancel.Click += BtnCancel_Click; 
                btnSave.Click += BtnSave_Click; 
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }
        private async void BtnSave_Click(object sender, EventArgs e)
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

                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return;
                }

                string pathImage = "";
                var QrNo = (int)DataCashing.EditMyQR.MyQrCodeNo;
                var result = await GabanaAPI.DeleteDataMyQrCode(QrNo);
                if (!result.Status)
                {
                    Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    return;
                }

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
                DataCashing.flagChooseMedia = false;
                Setting_Fragment_AddMyQR.flagdatachange = false;
                DataCashing.EditMyQR = null;
                Setting_Fragment_AddMyQR.MyQrCodeData = null;
                Setting_Fragment_AddMyQR.keepCropedUri = null;
                Setting_Fragment_MyQR.SetFocusQR(0);
                MainDialog.CloseDialog();
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "myqr");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at myqrcode_dialog");
                return;
            }
        }
    }
}