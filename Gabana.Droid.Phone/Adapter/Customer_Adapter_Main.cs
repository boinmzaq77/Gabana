using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;

namespace Gabana.Droid.Adapter
{
    public class Customer_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCustomer listcustomer;
        public int positionClick;
        View itemView;
        bool CheckNet;

        public Customer_Adapter_Main(ListCustomer l, bool c)
        {
            listcustomer = l;
            CheckNet = c;
        }

        public override int ItemCount
        {
            get { return listcustomer == null ? 0 : listcustomer.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewCustomerHolder vh = holder as ListViewCustomerHolder;
                vh.CustomerName.Text = listcustomer[position].CustomerName?.ToString();

                var cloudpath = listcustomer[position].PicturePath == null ? string.Empty : listcustomer[position].PicturePath;
                var localpath = listcustomer[position].ThumbnailLocalPath == null ? string.Empty : listcustomer[position].ThumbnailLocalPath;

                if (CheckNet)
                {
                    if (string.IsNullOrEmpty(localpath))
                    {
                        if (string.IsNullOrEmpty(cloudpath))
                        {
                            //defalut
                            vh.imageCustomer.SetBackgroundResource(Resource.Mipmap.defaultcust);
                        }
                        else
                        {
                            //cloud
                            Utils.SetImage(vh.imageCustomer, cloudpath);
                        }
                    }
                    else
                    {
                        //local
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        vh.imageCustomer.SetImageURI(uri);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        vh.imageCustomer.SetImageURI(uri);
                    }
                    else
                    {
                        vh.imageCustomer.SetBackgroundResource(Resource.Mipmap.defaultcust);
                    }
                }

                vh.imageCustomer.Click += delegate
                {
                    if (!string.IsNullOrEmpty(cloudpath) && !string.IsNullOrEmpty(localpath))
                    {
                        CustomerActivity.OpenDialogImage(listcustomer[position]);
                    }
                };
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.customer_adapter_main, parent, false);
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