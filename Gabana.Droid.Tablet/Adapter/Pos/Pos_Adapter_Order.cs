using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Adapter.Pos
{
    internal class Pos_Adapter_Order : RecyclerView.Adapter
    {

        ListOrders listOrders;
        public event EventHandler<int> ItemClick;
        string Currency;
        string myorder, otherdevice, allorder, textheader;
        bool sortOrder;

        public Pos_Adapter_Order(ListOrders l)
        {
            listOrders = l;
        }

        public override int ItemCount
        {
            get { return listOrders == null ? 0 : listOrders.Count; }
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                sortOrder = Pos_Dialog_Order.sort;
                Gabana.Model.OrderNew order = new Model.OrderNew();
                order = listOrders[position];
                ListViewOrderHolder vh = holder as ListViewOrderHolder;
                vh.Name.Text = order.orderName?.ToString();
                if (!string.IsNullOrEmpty(order.comments))
                {
                    vh.Comment.Text = " ," + order.comments?.ToString();
                    vh.Comment.Visibility = ViewStates.Visible;
                }
                else
                {
                    vh.Comment.Visibility = ViewStates.Gone;
                }

                vh.Device.Text = "Device" + order.deviceNo;
                if (order.deviceNo == DataCashingAll.Device.DeviceNo && !sortOrder)
                {
                    if (textheader != myorder)
                    {
                        textheader = myorder;
                        vh.Header.Text = textheader;
                        vh.Header.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        vh.Header.Visibility = ViewStates.Gone;
                    }
                }
                if (order.deviceNo != DataCashingAll.Device.DeviceNo && !sortOrder)
                {
                    if (textheader != otherdevice)
                    {
                        textheader = otherdevice;
                        vh.Header.Text = textheader;
                        vh.Header.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        vh.Header.Visibility = ViewStates.Gone;
                    }
                }
                if (sortOrder)
                {
                    if (textheader != allorder)
                    {
                        textheader = allorder;
                        vh.Header.Text = textheader;
                        vh.Header.Visibility = ViewStates.Visible;
                        vh.Header.Text = allorder;
                    }
                    else
                    {
                        vh.Header.Visibility = ViewStates.Gone;
                    }
                }
                vh.Date.Text = Utils.ShowDate(order.tranDate);
                vh.Amount.Text = Currency + " " + Utils.DisplayDecimal(order.grandTotal);

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBindViewHolder at order_adapter");
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.pos_dapter_oreder, parent, false);
            ListViewOrderHolder vh = new ListViewOrderHolder(itemView, OnClick);
            Android.Content.Res.Resources res = parent.Resources;
            myorder = res.GetString(Resource.String.order_activity_myorders);
            otherdevice = res.GetString(Resource.String.order_activity_otherorders);
            allorder = res.GetString(Resource.String.order_activity_alldevice);
            Currency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
        public delegate void CardCellQRCodeIndexDelegate5();
        public event CardCellQRCodeIndexDelegate5 OnCardCellbtnIndex5;

    }
    public class ListViewOrderHolder : RecyclerView.ViewHolder
    {
        public TextView Header { get; set; }
        public TextView Name { get; set; }
        public TextView Comment { get; set; }
        public TextView Device { get; set; }
        public TextView Date { get; set; }
        public TextView Amount { get; set; }

        public ListViewOrderHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Header = itemview.FindViewById<TextView>(Resource.Id.textHead);
            Name = itemview.FindViewById<TextView>(Resource.Id.txtName);
            Comment = itemview.FindViewById<TextView>(Resource.Id.txtComment);
            Device = itemview.FindViewById<TextView>(Resource.Id.txtDevice);
            Date = itemview.FindViewById<TextView>(Resource.Id.txtDate);
            Amount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);



            itemview.Click += (sender, e) => listener(base.Position);


        }
    }

}