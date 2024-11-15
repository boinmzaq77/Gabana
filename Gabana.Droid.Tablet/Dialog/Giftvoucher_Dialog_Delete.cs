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
using Gabana.ORM.MerchantDB;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Giftvoucher_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Giftvoucher_Dialog_Delete NewInstance()
        {
            var frag = new Giftvoucher_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }
        Button btnCancle, btnOK;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.giftvoucher_dialog_delete, container, false);
            try
            {
                btnCancle = view.FindViewById<Button>(Resource.Id.btnCancle);
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                btnOK.Click += BtnOK_Click;
                btnCancle.Click += BtnCancle_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }
        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnOK.Enabled = false;
                GiftVoucher giftVoucher = new GiftVoucher();
                if (DataCashing.EditGiftVoucher != null)
                {
                    giftVoucher = DataCashing.EditGiftVoucher;
                }
                var vouchercode = giftVoucher.GiftVoucherCode;
                var result = await GabanaAPI.DeleteDataGiftVoucher(vouchercode);
                if (!result.Status)
                {                    
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    btnOK.Enabled = true;
                    return;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                var deletelocal = await giftVoucherManage.DeleteGiftVoucher(DataCashingAll.MerchantId, vouchercode);
                DataCashing.flagAmountGiftVoucher = false;
                Setting_Fragment_AddGiftVoucher.flagdatachange = false;
                DataCashing.EditGiftVoucher = null;
                Setting_Fragment_GiftVoucher.fragment_giftvoucher.DeleteGiftVoucher(giftVoucher);
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "giftvoucher");
                btnOK.Enabled = true;
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                btnOK.Enabled = true;
            }
        }
    }
}