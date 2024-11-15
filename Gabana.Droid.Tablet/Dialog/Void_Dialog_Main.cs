using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabana.Droid.Tablet.Fragments.Bill;
using Xamarin.Essentials;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Void_Dialog_Main : AndroidX.Fragment.App.DialogFragment
    {
        Button btn_cancel;
        Button btn_save;
        string emplogin;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Void_Dialog_Main NewInstance()
        {
            var frag = new Void_Dialog_Main { Arguments = new Bundle() };
            return frag;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.void_dialog_main, container, false);
            try
            {
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);
                btn_save.Click += BtnOK_Click;
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_cancel.Click += BtnCancle_Click; 
                emplogin = Preferences.Get("User", "");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            Dismiss(); 
        }

        private async void BtnOK_Click(object sender, EventArgs e)
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
                btn_save.Enabled = false;
                //Update Fcancel = 1 -> void bill
                TransManage transManage = new TransManage();
                var BillVoid = BillHistory_Fragment_Detail.tranhistory;
                string tranNo = BillVoid.tranNo;

                //บิลเก่า จะสามาถ cancel ได้ต้องไม่เกิน 6 เดือน
                var DateNow = DateTime.UtcNow;

                int month = (DateNow.Month - BillVoid.tranDate.Month);
                if (month > 6)
                {
                    Dismiss();
                    Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotvoid), ToastLength.Short).Show();
                    return;
                }

                if (await GabanaAPI.CheckNetWork())
                {
                    //online
                    #region online
                    var result = await GabanaAPI.DeleteDataTran(DataCashingAll.SysBranchId, tranNo, UtilsAll.ChangeDateTimeUS(BillVoid.tranDate));
                    if (result.Status)
                    {
                        BillVoid.fCancel = 1;
                        var getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranNo);
                        if (getTran != null)
                        {
                            getTran.FCancel = 1;
                            getTran.FWaitSending = 2;
                            getTran.WaitSendingTime = DateTime.UtcNow;
                            getTran.LastDateModified = DateTime.UtcNow;
                            getTran.LastUserModified = emplogin;
                            getTran.TranDate = Utils.GetTranDate(getTran.TranDate);

                            var updatetran = await transManage.UpdateTran(getTran);
                            if (!updatetran)
                            {
                                MainDialog.CloseDialog();
                                Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotsave), ToastLength.Long).Show();
                                return;
                            }
                        }
                        ;
                        DataCashing.billHistory = BillVoid;
                        BillHistory_Fragment_Detail.fragment_detail.OnResume();
                        //BillHistory_Fragment_Main.fragment_main.OnResume();
                        BillHistory_Fragment_Main.fragment_main.BillHistoryFocus(BillVoid);
                        Dismiss();
                        Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Long).Show();
                    }
                    else
                    {
                        Dismiss();
                        Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotedit), ToastLength.Long).Show();
                        return;
                    }
                    #endregion
                }
                else
                {
                    //offine
                    #region offine                    
                    var getTran = await transManage.GetTran(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranNo);
                    if (getTran != null)
                    {
                        getTran.FCancel = 1;
                        getTran.FWaitSending = 2;
                        getTran.WaitSendingTime = DateTime.UtcNow;
                        getTran.LastDateModified = DateTime.UtcNow;
                        getTran.LastUserModified = emplogin;
                        getTran.TranDate = Utils.GetTranDate(getTran.TranDate);

                        var updatetran = await transManage.UpdateTran(getTran);
                        if (!updatetran)
                        {
                            MainDialog.CloseDialog();
                            Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotsave), ToastLength.Long).Show();
                            return;
                        }

                        Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.savesucess), ToastLength.Long).Show();
                        BillVoid.fCancel = 1; ;
                        DataCashing.billHistory = BillVoid;
                        BillHistory_Fragment_Detail.fragment_detail.OnResume();
                        //BillHistory_Fragment_Main.fragment_main.OnResume();
                        BillHistory_Fragment_Main.fragment_main.BillHistoryFocus(BillVoid);
                        Dismiss();
                    }
                    else
                    {
                        Dismiss();
                        Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotedit), ToastLength.Long).Show();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnOK_Click at void_dialog_main");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Long).Show();
            }
        }
    }
}