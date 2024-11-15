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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Cashguide_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        Button btnCancle, btnOK;
        static ORM.Master.CashTemplate cashguild;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Cashguide_Dialog_Delete NewInstance()
        {
            var frag = new Cashguide_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }
        TextView txtDetail;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.cashguide_dialog_delete, container, false);
            try
            {
                btnCancle = view.FindViewById<Button>(Resource.Id.btnCancle);
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
                txtDetail = view.FindViewById<TextView>(Resource.Id.txtDetail);

                txtDetail.Text = Utils.DisplayDecimal(cashguild.Amount).ToString();
                btnCancle.Click += BtnCancle_Click;
                btnOK.Click += BtnOK_Click;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (cashguild == null)
                {
                    this.Dialog.Dismiss();
                    return;
                }

                List<ORM.Master.CashTemplate> lstcash = new List<ORM.Master.CashTemplate>();

                var MasterCashTemplate = new ORM.Master.CashTemplate()
                {
                    Amount = cashguild.Amount,
                    CashTemplateNo = (int)cashguild.CashTemplateNo,
                    MerchantID = (int)cashguild.MerchantID,
                    DateModified = cashguild.DateModified,
                };
                lstcash.Add(MasterCashTemplate);

                var DeleteonCloud = await GabanaAPI.DeleteDataCashTemplate(lstcash);
                if (!DeleteonCloud.Status)
                {
                    Toast.MakeText(this.Activity, DeleteonCloud.Message, ToastLength.Short).Show();
                    this.Dialog.Dismiss();
                    return;
                }

                CashTemplateManage cashTemplateManage = new CashTemplateManage();
                var data = await cashTemplateManage.DeleteCashTemplate(DataCashingAll.MerchantId, (int)cashguild.CashTemplateNo);
                if (!data)
                {
                    this.Dialog.Dismiss();
                    return;
                }

                var CashTemplate = new ORM.MerchantDB.CashTemplate()
                {
                    Amount = cashguild.Amount,
                    CashTemplateNo = (int)cashguild.CashTemplateNo,
                    MerchantID = (int)cashguild.MerchantID,
                    DateModified = cashguild.DateModified,
                };

                Setting_Fragment_CashGuild.fragment_main.DeleteCashGuid(CashTemplate);
                Setting_Fragment_CashGuild.fragment_main.flagLoadData = true;
                //Setting_Fragment_AddCash.fragment_addcash.flagdatachange = false;
                //Setting_Fragment_AddCash.fragment_addcash.cashguide = null;
                //DataCashing.EditCashGuide = null;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "cashguild");
                this.Dialog.Dismiss();
                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

        public static void SetCashguild(ORM.Master.CashTemplate _cashguild)
        {
            cashguild = _cashguild;
        }

    }
}