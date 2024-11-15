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
    public class GiftVoucher_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static string Page, GiftVoucherCode;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static GiftVoucher_Dialog_Delete NewInstance(string _giftvoucher, string _page)
        {
            GiftVoucherCode = _giftvoucher;
            Page = _page;
            var frag = new GiftVoucher_Dialog_Delete { Arguments = new Bundle() };
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
                textconfirm1.Text = GetString(Resource.String.dialog_delete_giftvoucher_1);
                textconfirm2.Text = GetString(Resource.String.dialog_delete_giftvoucher_2);

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
                btn_save.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    var vouchercode = GiftVoucherCode;
                    var result = await GabanaAPI.DeleteDataGiftVoucher(vouchercode);
                    if (result.Status)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                        var deletelocal = await giftVoucherManage.DeleteGiftVoucher(DataCashingAll.MerchantId, vouchercode);
                        AddGiftvoucherActivity.GiftVoucher = null;

                        DataCashingAll.flagGiftVoucherChange = true;
                        if (Page != "main")
                        {
                            //หน้า add                           
                            this.Activity.Finish();
                        }
                        else
                        {
                            GiftvoucherActivity.activity.Resume();
                        }
                        MainDialog.CloseDialog();
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                        if (Page != "main")
                        {
                            //หน้า add                           
                            DataCashingAll.flagGiftVoucherChange = true;
                            GiftvoucherActivity.activity.Resume();
                        }
                    }
                    btn_save.Enabled = true;
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btn_save.Enabled = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                btn_save.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUBtn_save_Clickpdate at giftvoucher_dialog");
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}
