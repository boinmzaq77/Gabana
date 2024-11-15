using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class myQRBranch_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public static ListBranch listitem;
        public static string positionClick;
        public static string BranchID;
        public static ListViewBranchHolder vh;
        private List<ORM.MerchantDB.Branch> listChooseBranch;

        public myQRBranch_Adapter_Main(ListBranch b, List<ORM.MerchantDB.Branch> l)
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
            vh = holder as ListViewBranchHolder;
            //setCheck( position);   
            vh.BranchName.Text = listitem[position].BranchName?.ToString();
            var branch = listChooseBranch.FindIndex(x => x.BranchID == listitem[position].BranchID);
            if (branch != -1)
            {
                vh.BtnCheck.Visibility = ViewStates.Visible;
                positionClick = listitem[position].BranchID?.ToString();
                BranchID = listitem[position].BranchID?.ToString();
            }
            else if (myQRBranchActivity.branchSelect == "All")
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

        public static void setCheck(int position)
        {
            try
            {
                //if (position == -1)
                //{
                //    foreach (var item in listitem.branches)
                //    {
                //        vh.BtnCheck.Visibility = ViewStates.Visible;
                //    }      

                //    return;
                //}

                //if (position == -2)
                //{
                //    vh.BtnCheck.Visibility = ViewStates.Gone;
                //    return;
                //}

                vh.BranchName.Text = listitem[position].BranchName?.ToString();
                if (listitem[position].BranchID == myQRBranchActivity.branchSelect)
                {
                    vh.BtnCheck.Visibility = ViewStates.Visible;
                    positionClick = listitem[position].BranchID?.ToString();
                    BranchID = listitem[position].BranchID?.ToString();
                }
                else if (myQRBranchActivity.branchSelect == "All")
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.selectbranch_adapter_main, parent, false);
            vh = new ListViewBranchHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}