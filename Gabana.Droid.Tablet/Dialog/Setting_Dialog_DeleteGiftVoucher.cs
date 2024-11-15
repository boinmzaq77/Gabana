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

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dialog_DeleteGiftVoucher : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Dialog_DeleteGiftVoucher NewInstance()
        {
            var frag = new Setting_Dialog_DeleteGiftVoucher { Arguments = new Bundle() };
            return frag;
        }
        View view;
        Button btnCancel, btnSave;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_dialog_deletegiftvoucher, container, false);
            try
            {
                btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
                btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

                btnCancel.Click += BtnCancel_Click; ;
                btnSave.Click += BtnSave_Click; ;

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
                if (await GabanaAPI.CheckNetWork())
                {
                    var vouchercode = DataCashing.EditGiftVoucher.GiftVoucherCode;
                    var result = await GabanaAPI.DeleteDataGiftVoucher(vouchercode);
                    if (result.Status)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                        var deletelocal = await giftVoucherManage.DeleteGiftVoucher(DataCashingAll.MerchantId, vouchercode);

                        DataCashing.EditGiftVoucher = null;
                        DataCashingAll.flagGiftVoucherChange = true;

                        Setting_Fragment_GiftVoucher.fragment_giftvoucher.OnResume();
                        MainDialog.CloseDialog();
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUBtn_save_Clickpdate at giftvoucher_dialog");
                return;
            }
        }
    }
}