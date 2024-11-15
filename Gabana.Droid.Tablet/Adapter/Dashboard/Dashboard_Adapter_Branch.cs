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

namespace Gabana.Droid.Tablet.Adapter.Dashboard
{
    internal class Dashboard_Adapter_Branch : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListBranch listitem;
        public string positionClick;
        public string BranchID;

        public Dashboard_Adapter_Branch(ListBranch l)
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
            if (listitem[position].BranchID == DashboardBranchActivity.branchSelect)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.dashboard_adapter_branch, parent, false);
            ListViewBranchHolder vh = new ListViewBranchHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
    public class ListViewBranchHolder : RecyclerView.ViewHolder
    {
        public TextView BranchName { get; set; }
        public ImageButton BtnCheck { get; set; }
        public LinearLayout lnSelectBranch { get; set; }

        public ListViewBranchHolder(View itemview, Action<int> listener) : base(itemview)
        {
            BranchName = itemview.FindViewById<TextView>(Resource.Id.txtBranchName);
            BtnCheck = itemview.FindViewById<ImageButton>(Resource.Id.btnCheck);
            lnSelectBranch = itemview.FindViewById<LinearLayout>(Resource.Id.lnSelectBranch);

            itemview.Click += (sender, e) => listener(base.Position);

        }
    }
}