using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;

namespace Gabana.Droid.Phone
{
    public class Branch_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button BtnDeleteBranch;
        static string Page;
        static int SysbranchID;
        TextView textNameBranch;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Branch_Dialog_Delete NewInstance(int _sysbranchid, string _page)
        {
            SysbranchID = _sysbranchid;
            Page = _page;
            var frag = new Branch_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }

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
                BtnDeleteBranch.SetBackgroundResource(Resource.Drawable.btnborderblue);
                BtnDeleteBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            else
            {
                BtnDeleteBranch.Enabled = true;
                BtnDeleteBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                BtnDeleteBranch.SetBackgroundResource(Resource.Drawable.btnblue);
            }
        }

        private async void BtnDeleteBranch_Click(object sender, EventArgs e)
        {
            try
            {
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
                        MainDialog.CloseDialog();
                        if (Page == "main")
                        {
                            BranchActivity.branchactivity.Resume();
                        }
                        return;
                    }

                    var DeleteonLocal = await branchManage.DeleteBranch(DataCashingAll.MerchantId, SysbranchID);
                    if (!DeleteonLocal)
                    {
                        BtnDeleteBranch.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                        MainDialog.CloseDialog();
                        if (Page == "main")
                        {
                            BranchActivity.branchactivity.Resume();
                        }
                        return;
                    }

                    if (Page != "main")
                    {
                        //หน้า add
                        this.Activity.Finish();
                    }
                    else
                    {
                        BranchActivity.flagLoadData = true;
                        BranchActivity.branchactivity.Resume();
                    }
                    BtnDeleteBranch.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                    MainDialog.CloseDialog();
                }
            }
            catch (Exception ex)
            {
                BtnDeleteBranch.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}
