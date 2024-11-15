using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_ChooseBranch : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListBranch listitem;
        private List<Branch> listChooseBranch;
        public string positionClick;
        public string BranchID;

        public Report_Adapter_ChooseBranch(ListBranch l, List<Branch> b)
        {
            listitem = l;
            listChooseBranch = b;
        }
        public override int ItemCount
        {
            get { return listitem == null ? 0 : listitem.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewBranchHolder vh = holder as ListViewBranchHolder;
            vh.BranchName.Text = listitem[position].BranchName?.ToString();

            var index = listChooseBranch.FindIndex(x => x.SysBranchID == listitem[position].SysBranchID);
            if (index == -1)
            {
                vh.BtnCheck.Visibility = ViewStates.Invisible;
            }
            else
            {
                vh.BtnCheck.Visibility = ViewStates.Visible;
            }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_choosebranch, parent, false);
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