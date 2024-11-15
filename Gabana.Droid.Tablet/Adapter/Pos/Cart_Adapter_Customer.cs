using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Customers;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Pos
{
    public class Cart_Adapter_Customer : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCustomer listcustomer;
        public string positionClick;
        bool CheckNet;

        public Cart_Adapter_Customer(ListCustomer l, bool c)
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
                        vh.imageCustomer.SetBackgroundResource(Resource.Mipmap.LogoDefault);
                    }
                }

                if (Cart_Dailog_Customer.selectCustomer == listcustomer[position].SysCustomerID)
                {
                    vh.SelectCustomer.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.SelectCustomer.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cart_adapter_customer, parent, false);
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