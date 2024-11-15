using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Items
{
    internal class AddItem_Adapter_MoveMent : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        ListStockMoveMent listStock;
        public AddItem_Adapter_MoveMent(ListStockMoveMent s)
        {
            listStock = s;
        }

        public async void refresh(ListStockMoveMent l)
        {
            try
            {
                listStock = l;
                if (await GabanaAPI.CheckNetWork())
                {
                    NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public override int ItemCount
        {
            get { return listStock == null ? 0 : listStock.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewStockHolder vh = holder as ListViewStockHolder;


            vh.UserEdit.Text = listStock[position].UserName?.ToString();
            var MovementDate = listStock[position].MovementDate;

            var timezoneslocal = TimeZoneInfo.Local;
            vh.Date.Text = TimeZoneInfo.ConvertTimeFromUtc(MovementDate, timezoneslocal).ToString("dd/MM/yyyy HH:mm", new CultureInfo("en-US"));
            vh.Unit.Text = listStock[position].Quantity.ToString("#,###");

            switch (listStock[position].MovementType)
            {
                case 'S':
                    vh.TypeStock.Text = "Sale";
                    vh.ImageStock.SetBackgroundResource(Resource.Mipmap.StockDecrease);
                    break;
                case 'C':
                    vh.TypeStock.Text = "Canceled Sale";
                    vh.ImageStock.SetBackgroundResource(Resource.Mipmap.StockIncrease);
                    break;
                case 'A':
                    vh.TypeStock.Text = "Add Stock";
                    vh.ImageStock.SetBackgroundResource(Resource.Mipmap.StockIncrease);
                    break;
                case 'R':
                    vh.TypeStock.Text = "Remove Stock";
                    vh.ImageStock.SetBackgroundResource(Resource.Mipmap.StockIncrease);
                    break;
                case 'B':
                    vh.TypeStock.Text = " Begin Balance";
                    vh.ImageStock.SetBackgroundResource(Resource.Mipmap.StockDecrease);
                    break;
                default:
                    break;
            }


            int countItem = listStock.Count - 1;
            if (countItem == position)
            {
                OnCardCellbtnIndex?.Invoke();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.additem_adapter_movement, parent, false);
            ListViewStockHolder vh = new ListViewStockHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

        public delegate void CardCellQRCodeIndexDelegate();
        public event CardCellQRCodeIndexDelegate OnCardCellbtnIndex;

    }
    public class ListViewStockHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageStock { get; set; }
        public TextView TypeStock { get; set; }
        public TextView UserEdit { get; set; }
        public TextView Date { get; set; }
        public TextView Unit { get; set; }

        public ListViewStockHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImageStock = itemview.FindViewById<ImageView>(Resource.Id.imageStock);
            TypeStock = itemview.FindViewById<TextView>(Resource.Id.txtTypeStock);
            UserEdit = itemview.FindViewById<TextView>(Resource.Id.txtUserEdit);
            Date = itemview.FindViewById<TextView>(Resource.Id.txtDate);
            Unit = itemview.FindViewById<TextView>(Resource.Id.txtUnit);



            itemview.Click += (sender, e) => listener(base.Position);


        }
    }

    public class ListStockMoveMent
    {
        public List<ItemMovement> stockMovement;
        static List<ItemMovement> builitem;
        public ListStockMoveMent(List<ItemMovement> stockMovement)
        {
            builitem = stockMovement.OrderByDescending(x => x.MovementDate).ToList();
            this.stockMovement = builitem;
        }
        public int Count
        {
            get
            {
                return stockMovement == null ? 0 : stockMovement.Count;
            }
        }
        public ItemMovement this[int i]
        {
            get { return stockMovement == null ? null : stockMovement[i]; }
        }
    }
}