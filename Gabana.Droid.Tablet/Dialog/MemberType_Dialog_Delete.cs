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

namespace Gabana.Droid.Tablet.Dialog
{
    public class MemberType_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        Button btnCancle, btnOK;
        public static List<ORM.Master.MemberType> lstMemberType = new List<ORM.Master.MemberType>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static MemberType_Dialog_Delete NewInstance()
        {
            var frag = new MemberType_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.membertype_dialog_delete, container, false);
            try
            {
                btnCancle = view.FindViewById<Button>(Resource.Id.btnCancle);
                btnOK = view.FindViewById<Button>(Resource.Id.btnOK);
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
                var lstMemberType = DataCashing.DeleteMemberType;
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

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }

        public static void SetMembertypeDetail(List<ORM.Master.MemberType> _lstmembertype)
        {
            lstMemberType = _lstmembertype;
        }
    }
}