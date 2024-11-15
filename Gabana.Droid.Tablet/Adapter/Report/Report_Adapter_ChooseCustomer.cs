using Android.Views;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Customers;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using System;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_ChooseCustomer : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCustomer listCustomer;
        public string positionClick;

        public Report_Adapter_ChooseCustomer(ListCustomer l)
        {
            listCustomer = l;
        }
        public override int ItemCount
        {
            get { return listCustomer == null ? 0 : listCustomer.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewCustomerHolder vh = holder as ListViewCustomerHolder;
                vh.CustomerName.Text = listCustomer[position].CustomerName?.ToString();

                var path = listCustomer[position].ThumbnailLocalPath;
                if (!string.IsNullOrEmpty(path))
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(listCustomer[position]?.ThumbnailLocalPath);
                    vh.imageCustomer.SetImageURI(uri);
                }
                else
                {
                    vh.imageCustomer.SetImageResource(Resource.Mipmap.defaultcust);
                }

                var index = Report_Dialog_Customer.listChooseCustomer.FindIndex(x => x.SysCustomerID == listCustomer[position].SysCustomerID);
                if (index == -1)
                {
                    vh.SelectCustomer.Visibility = ViewStates.Invisible;
                }
                else
                {
                    vh.SelectCustomer.Visibility = ViewStates.Visible;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_choosecustomer, parent, false);
            ListViewCustomerHolder vh = new ListViewCustomerHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }

}