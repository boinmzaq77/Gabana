using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Adapter
{
    class Order_Adapter_Main : RecyclerView.Adapter
    {
        ListOrders listOrders;
        public event EventHandler<int> ItemClick;
        string Currency;
        string myorder, otherdevice, allorder, textheader;
        bool sortOrder;

        public Order_Adapter_Main(ListOrders l)
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
                sortOrder = OrderActivity.sort;
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.orders_adapter_main, parent, false);
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
#pragma warning disable CS0067 // The event 'Order_Adapter_Main.OnCardCellbtnIndex5' is never used
        public event CardCellQRCodeIndexDelegate5 OnCardCellbtnIndex5;
#pragma warning restore CS0067 // The event 'Order_Adapter_Main.OnCardCellbtnIndex5' is never used
    }
}