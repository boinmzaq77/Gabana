using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Customers
{
    internal class Customer_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCustomer listcustomer;
        bool CheckNet;
        public Customer_Adapter_Main(ListCustomer l, bool c)
        {
            listcustomer = l;
            //CheckNet = c;
        }
        public Customer_Adapter_Main(ListCustomer l)
        {
            listcustomer = l;
        }
        public override int ItemCount
        {
            get { return listcustomer == null ? 0 : listcustomer.Count; }
        }
        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewCustomerHolder vh = holder as ListViewCustomerHolder;
                vh.CustomerName.Text = listcustomer[position].CustomerName?.ToString();
                var cloudpath = listcustomer[position].PicturePath == null ? string.Empty : listcustomer[position].PicturePath;
                var localpath = listcustomer[position].ThumbnailLocalPath == null ? string.Empty : listcustomer[position].ThumbnailLocalPath;

                //if (CheckNet)
                if (await GabanaAPI.CheckNetWork())
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
                        string path = "";
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.customer_fragment_main.ToString();
                        bundle.PutString("message", myMessage);
                        if (!string.IsNullOrEmpty(listcustomer[position].PicturePath))
                        {
                            if (listcustomer[position].PicturePath.Contains("http"))
                            {
                                bundle.PutString("OpenCloudPicture", listcustomer[position].PicturePath);
                                path = listcustomer[position].PicturePath;
                            }
                            else
                            {
                                Java.IO.File imgFile = new Java.IO.File(listcustomer[position].PictureLocalPath);
                                if (imgFile != null)
                                {
                                    bundle.PutString("OpenCloudPicture", imgFile.AbsolutePath);
                                    path = imgFile.AbsolutePath;
                                }
                            }
                        }
                        Customer_Dialog_ShowImage dialog_Item = Customer_Dialog_ShowImage.NewInstance(path);
                        dialog_Item.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.customer_adapter_main, parent, false);
            ListViewCustomerHolder vh = new ListViewCustomerHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
    public class ListViewCustomerHolder : RecyclerView.ViewHolder
    {
        public TextView CustomerName { get; set; }
        public ImageView imageCustomer { get; set; }
        public ImageButton SelectCustomer { get; set; }

        public ListViewCustomerHolder(View itemview, Action<int> listener) : base(itemview)
        {
            imageCustomer = itemview.FindViewById<ImageView>(Resource.Id.imageCustomer);
            CustomerName = itemview.FindViewById<TextView>(Resource.Id.txtNameCustomer);
            SelectCustomer = itemview.FindViewById<ImageButton>(Resource.Id.imgSelect);


            itemview.Click += (sender, e) => listener(base.Position);

        }
    }



}