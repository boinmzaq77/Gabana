using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Phone
{
    public class CashGuide_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static string CashGuidData;
        static string Page;
        List<ORM.Master.CashTemplate> CashTemplateData;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static CashGuide_Dialog_Delete NewInstance(string _CashGuidData, string _page)
        {
            CashGuidData = _CashGuidData;
            Page = _page;
            var frag = new CashGuide_Dialog_Delete { Arguments = new Bundle() };
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
                textconfirm1.Text = GetString(Resource.String.dialog_delete);
                CashTemplateData = JsonConvert.DeserializeObject<List<ORM.Master.CashTemplate>>(CashGuidData);
                textconfirm2.Text = Utils.DisplayDecimal(CashTemplateData[0].Amount).ToString();
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
                if (CashTemplateData == null)
                {
                    btn_save.Enabled = true;
                    fntElse();
                }
                if (CashTemplateData != null)
                {
                    var DeleteonCloud = await GabanaAPI.DeleteDataCashTemplate(CashTemplateData);
                    if (DeleteonCloud == null)
                    {
                        DeleteonCloud = await GabanaAPI.DeleteDataCashTemplate(CashTemplateData);
                    }
                    if (!DeleteonCloud.Status)
                    {
                        btn_save.Enabled = true;
                        Toast.MakeText(this.Activity, DeleteonCloud.Message, ToastLength.Short).Show();
                        MainDialog.CloseDialog();
                        if (Page == "main")
                        {
                            MemberTypeActivity.membertype.Resume();
                        }
                        return;
                    }

                    CashTemplateManage cashTemplateManage = new CashTemplateManage();
                    var data = await cashTemplateManage.DeleteCashTemplate(DataCashingAll.MerchantId, CashTemplateData[0].CashTemplateNo);
                    if (!data)
                    {
                        btn_save.Enabled = true;
                        fntElse();
                    }

                    if (Page != "main")
                    {
                        //หน้า add
                        this.Activity.Finish();
                    }
                    else
                    {
                        CashGuideActivity.flagLoadData = true;
                        CashGuideActivity.cashGuideActivity.Resume();
                    }
                    Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                    btn_save.Enabled = true;
                    MainDialog.CloseDialog();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                btn_save.Enabled = true;
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

        void fntElse()
        {
            try
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                MainDialog.CloseDialog();
                if (Page != "main")
                {
                    //หน้า add
                    this.Activity.Finish();
                }
                else
                {
                    MemberTypeActivity.membertype.Resume();
                }
                return;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

    }
}
