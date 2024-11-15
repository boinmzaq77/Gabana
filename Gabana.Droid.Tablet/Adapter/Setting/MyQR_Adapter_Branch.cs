using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    internal class MyQR_Adapter_Branch : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public static ListBranch listitem;
        public static string positionClick;
        public static string BranchID;
        public static ListViewBranchHolder vh;
        private List<ORM.MerchantDB.Branch> listChooseBranch;
        public MyQR_Adapter_Branch(ListBranch b, List<ORM.MerchantDB.Branch> l)
        {
            listitem = b;
            listChooseBranch = l;
        }
        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewBranchHolder vh = holder as ListViewBranchHolder;
            vh.BranchName.Text = listitem[position].BranchName?.ToString();
            var branch = listChooseBranch.FindIndex(x => x.BranchID == listitem[position].BranchID);
            if (branch != -1)
            {
                vh.BtnCheck.Visibility = ViewStates.Visible;
                positionClick = listitem[position].BranchID?.ToString();
                BranchID = listitem[position].BranchID?.ToString();
            }
            else if (Myqr_Dialog_SelectBranch.branchSelect == "All")
            {
                vh.BtnCheck.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.BtnCheck.Visibility = ViewStates.Invisible;
                positionClick = string.Empty;
                BranchID = string.Empty;
            }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.myqr_adapter_selectbranch, parent, false);
            ListViewBranchHolder vh = new ListViewBranchHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }

}