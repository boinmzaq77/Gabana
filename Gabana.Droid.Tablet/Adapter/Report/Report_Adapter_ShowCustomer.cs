using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_ShowCustomer : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<SalesByCustomerResponse> listTime;
        public string positionClick;
        private string CURRENCYSYMBOLS;
        List<ORM.MerchantDB.Customer> customers;
        public Report_Adapter_ShowCustomer(List<SalesByCustomerResponse> l, List<ORM.MerchantDB.Customer> listCustomer)
        {
            listTime = l;
            customers = listCustomer;
        }


        public override int ItemCount
        {
            get { return listTime == null ? 0 : listTime.Count; }
        }

        public List<ReportHourly> ReportHourlies { get; }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewCustomerReporHolder vh = holder as ListViewCustomerReporHolder;

                vh.txtNameCustomer.Text = listTime[position].customerName;
                vh.Amount.Text = CURRENCYSYMBOLS + " " + listTime[position].sumTotalAmount.ToString("#,##0.00");

                var customer = customers.Where(x => x.CustomerName.Replace("\u200b", "") == listTime[position].customerName.Replace("\u200b", "")).FirstOrDefault();
                if (customer == null)
                {
                    return;
                }

                var cloudpath = customer.PicturePath == null ? string.Empty : customer.PicturePath;
                var localpath = customer.PictureLocalPath == null ? string.Empty : customer.PictureLocalPath;

                if (string.IsNullOrEmpty(localpath))
                {
                    if (string.IsNullOrEmpty(cloudpath))
                    {
                        //defalut
                        vh.Profile.SetBackgroundResource(Resource.Mipmap.defaultcust);
                    }
                    else
                    {
                        //cloud
                        Utils.SetImage(vh.Profile, cloudpath);
                    }
                }
                else
                {
                    //local
                    Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                    vh.Profile.SetImageURI(uri);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_customer, parent, false);
            ListViewCustomerReporHolder vh = new ListViewCustomerReporHolder(itemView, OnClick);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }

    public class ListViewCustomerReporHolder : RecyclerView.ViewHolder
    {
        public TextView txtNameCustomer { get; set; }
        public TextView Amount { get; set; }
        public ImageView Profile { get; set; }
        public ImageView Position { get; set; }
        public TextView TextPosition { get; set; }

        public ListViewCustomerReporHolder(View itemview, Action<int> listener) : base(itemview)
        {
            txtNameCustomer = itemview.FindViewById<TextView>(Resource.Id.txtNameCustomer);
            Amount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);
            Profile = itemview.FindViewById<ImageView>(Resource.Id.imageCustomer);
            Position = itemview.FindViewById<ImageView>(Resource.Id.imagePosition);
            TextPosition = itemview.FindViewById<TextView>(Resource.Id.txtPositionEmp);



            itemview.Click += (sender, e) => listener(base.Position);


        }
    }
}