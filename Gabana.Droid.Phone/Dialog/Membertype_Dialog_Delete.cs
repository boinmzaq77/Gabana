using Android.App;
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
    public class Membertype_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static string membertypeData;
        static string Page;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Membertype_Dialog_Delete NewInstance(string _membertypeData, string _page)
        {
            membertypeData = _membertypeData;
            Page = _page;
            var frag = new Membertype_Dialog_Delete { Arguments = new Bundle() };
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
                textconfirm1.Text = Application.Context.GetString(Resource.String.dialog_delete_membertype1);
                textconfirm2.Text = Application.Context.GetString(Resource.String.dialog_delete_membertype2);
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
                var lstmemberType = JsonConvert.DeserializeObject<List<Gabana.ORM.Master.MemberType>>(membertypeData);
                if (lstmemberType == null)
                {
                    btn_save.Enabled = true;
                    fntElse();
                }
                if (lstmemberType != null)
                {
                    var DeleteonCloud = await GabanaAPI.DeleteDataMemberType(lstmemberType);                    
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

                    //Set Null ที่ customer ที่มีการใช้ membertype 
                    CustomerManage customerManage = new CustomerManage();
                    var check = await customerManage.UpdateNullCustomerandDeleteMembeytype(DataCashingAll.MerchantId, lstmemberType[0].MemberTypeNo);
                    if (!check)
                    {
                        btn_save.Enabled = true;
                        fntElse();
                    }

                    MemberTypeActivity.flagData = true;
                    if (Page != "main")
                    {
                        //หน้า add
                        this.Activity.Finish();
                    }
                    else
                    {
                        MemberTypeActivity.flagData = true;
                        MemberTypeActivity.membertype.Resume();
                    }

                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                    MainDialog.CloseDialog();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
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
