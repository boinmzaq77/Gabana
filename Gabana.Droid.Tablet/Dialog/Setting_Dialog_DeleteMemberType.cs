
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dialog_DeleteMemberType : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Dialog_DeleteMemberType NewInstance()
        {
            var frag = new Setting_Dialog_DeleteMemberType { Arguments = new Bundle() };
            return frag;
        }
        View view;
        Button btnCancel, btnSave;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_dialog_deletemembertype, container, false);
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
            this.Dialog.Dismiss();
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var lstMemberType = Setting_Fragment_AddMemberType.lstMemberType;
                var DeleteonCloud = await GabanaAPI.DeleteDataMemberType(lstMemberType);               
                if (!DeleteonCloud.Status)
                {
                    Toast.MakeText(this.Activity, DeleteonCloud.Message, ToastLength.Short).Show();
                    this.Dialog.Dismiss();
                }

                //Set Null ที่ customer ที่มีการใช้ membertype 
                CustomerManage customerManage = new CustomerManage();
                var check = await customerManage.UpdateNullCustomerandDeleteMembeytype(DataCashingAll.MerchantId, lstMemberType[0].MemberTypeNo);
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "membertype");
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


    }
}