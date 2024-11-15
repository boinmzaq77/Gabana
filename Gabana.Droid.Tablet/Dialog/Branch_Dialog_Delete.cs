using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gabana.ORM.MerchantDB;
using Gabana.Droid.Tablet.Fragments.Setting;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Branch_Dialog_Delete : AndroidX.Fragment.App.DialogFragment
    {
        Button BtnDeleteBranch;
        static string Page;
        static int SysbranchID;
        TextView textNameBranch;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Branch_Dialog_Delete NewInstance()
        {
            var frag = new Branch_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }
        TextView txtDetail;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.branch_dialog_delete, container, false);
            try
            {

                textNameBranch = view.FindViewById<TextView>(Resource.Id.textNameBranch);
                BtnDeleteBranch = view.FindViewById<Button>(Resource.Id.BtnDeleteBranch);
                BtnDeleteBranch.Click += BtnDeleteBranch_Click;
                textNameBranch.TextChanged += TextNameBranch_TextChanged;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }


        private void TextNameBranch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(textNameBranch.Text))
            {
                BtnDeleteBranch.Enabled = false;
                BtnDeleteBranch.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                BtnDeleteBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                BtnDeleteBranch.Enabled = true;
                BtnDeleteBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                BtnDeleteBranch.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
        }

        private async void BtnDeleteBranch_Click(object sender, EventArgs e)
        {
            try
            {
                SysbranchID = (int)DataCashing.EditBranch.SysBranchID;
                BtnDeleteBranch.Enabled = false;
                if (string.IsNullOrEmpty(textNameBranch.Text))
                {
                    BtnDeleteBranch.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    return;
                }

                if (SysbranchID == 1)
                {
                    BtnDeleteBranch.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdeleteho), ToastLength.Short).Show();
                    return;
                }
                else if (SysbranchID == DataCashingAll.SysBranchId)
                {
                    BtnDeleteBranch.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    BranchManage branchManage = new BranchManage();
                    Branch branchdata = await branchManage.GetBranch(DataCashingAll.MerchantId, SysbranchID);
                    if (branchdata.BranchName.ToLower() != textNameBranch.Text.Trim().ToLower())
                    {
                        BtnDeleteBranch.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                        return;
                    }

                    var DeleteonCloud = await GabanaAPI.DeleteDataBranch(SysbranchID);
                    if (!DeleteonCloud.Status)
                    {
                        BtnDeleteBranch.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                        MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "note");
                        this.Dialog.Dismiss();

                        return;
                    }

                    var DeleteonLocal = await branchManage.DeleteBranch(DataCashingAll.MerchantId, SysbranchID);
                    if (!DeleteonLocal)
                    {
                        BtnDeleteBranch.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();

                        MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "note");
                        this.Dialog.Dismiss();
                        return;
                    }

                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "branch");

                    BtnDeleteBranch.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                        
                    this.Dialog.Dismiss();
                }
            }
            catch (Exception ex)
            {
                BtnDeleteBranch.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

    }
}