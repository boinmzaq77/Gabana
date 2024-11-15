﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Dashboard;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Adapter.Dashboard
{
    internal class Dashboard_Adapter_Item : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public List<Item> listitem;
        public string positionClick;
        ListBestSallItem bestSellingItems;
        public Dashboard_Adapter_Item(List<Item> l, ListBestSallItem b)
        {
            listitem = l;
            bestSellingItems = b;
        }

        public override int ItemCount
        {
            get { return bestSellingItems == null ? 0 : bestSellingItems.Count; }
        }
        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewBestSaleHolder vh = holder as ListViewBestSaleHolder;
                Item item = listitem.Where(x => x.ItemName.Replace("\u200b", "") == bestSellingItems[position].itemName.Replace("\u200b", "")).FirstOrDefault();
                if (item == null)
                {
                    var color = Color.ParseColor("#0095DA");
                    vh.ImageItem.SetBackgroundColor(color);
                    vh.ShortName.Visibility = ViewStates.Visible;
                }
                else
                {
                    var cloudpath = item.PicturePath == null ? string.Empty : item.PicturePath;
                    var localpath = item.ThumbnailLocalPath == null ? string.Empty : item.ThumbnailLocalPath;

                    if (string.IsNullOrEmpty(localpath))
                    {
                        if (string.IsNullOrEmpty(cloudpath))
                        {
                            //defalut
                            string conColor = Utils.SetBackground(Convert.ToInt32(item.Colors));
                            var color = Android.Graphics.Color.ParseColor(conColor);
                            vh.ShortName.SetBackgroundColor(color);
                            vh.ShortName.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            //cloud
                            Utils.SetImage(vh.ImageItem, cloudpath);
                            vh.ShortName.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        //local
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        vh.ImageItem.SetImageURI(uri);
                        vh.ShortName.Visibility = ViewStates.Gone;
                    }
                }

                vh.ShortName.Text = bestSellingItems[position].itemName?.ToString();
                vh.Name.Text = bestSellingItems[position].itemName?.ToString();

                vh.TotalSale.Text = bestSellingItems[position].totalAmount.ToString("#,##0.00");
                switch (bestSellingItems[position].movementFlag)
                {
                    case -1:
                        vh.ImageMoveMent.SetBackgroundResource(Resource.Mipmap.DbDown);
                        vh.ImageMoveMent.Visibility = ViewStates.Visible;

                        break;
                    case 0:
                        vh.ImageMoveMent.Visibility = ViewStates.Invisible;
                        break;
                    case 1:
                        vh.ImageMoveMent.SetBackgroundResource(Resource.Mipmap.DbUp);
                        vh.ImageMoveMent.Visibility = ViewStates.Visible;
                        break;
                    default:
                        vh.ImageMoveMent.Visibility = ViewStates.Invisible;
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBindViewHolder at Dashboard");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.dashboard_adapter_item, parent, false);
            ListViewBestSaleHolder vh = new ListViewBestSaleHolder(itemView, OnClick);

            TextView textSignMoney = itemView.FindViewById<TextView>(Resource.Id.textSignMoney);
            var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            textSignMoney.Text = CURRENCYSYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
    public class ListViewBestSaleHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageItem { get; set; }
        public TextView ShortName { get; set; }
        public TextView Name { get; set; }
        public TextView TotalSale { get; set; }
        public ImageButton ImageMoveMent { get; set; }

        public ListViewBestSaleHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImageItem = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            ShortName = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            Name = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            TotalSale = itemview.FindViewById<TextView>(Resource.Id.txtTotalSale);
            ImageMoveMent = itemview.FindViewById<ImageButton>(Resource.Id.imageMoveMent);


            itemview.Click += (sender, e) => listener(base.Position);


        }
    }

}