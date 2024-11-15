using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.More
{
    internal class ChangeBranch_Adapter_Main : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListBranch listitem;
        public string positionClick;
        public string BranchID, BranchSelect;

        public ChangeBranch_Adapter_Main(ListBranch l)
        {
            listitem = l;
        }
        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewBranchHolder vh = holder as ListViewBranchHolder;
            vh.BranchName.Text = listitem[position].BranchName?.ToString();
            if (listitem[position].BranchID == ChangeBranchActivity.branchSelect)
            {
                vh.BtnCheck.Visibility = ViewStates.Visible;
                positionClick = listitem[position].BranchID?.ToString();
                BranchID = listitem[position].BranchID?.ToString();
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.login_adapter_selectbranch, parent, false);
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