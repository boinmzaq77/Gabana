using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.ShareSource;
using System;
using System.Globalization;

namespace Gabana.Droid.Adapter
{
    public class Stock_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        ListStockMoveMent listStock;
        public Stock_Adapter_Main(ListStockMoveMent s)
        {
            listStock = s;
        }

        public async void refresh(ListStockMoveMent l)
        {
            try
            {
                listStock = l;
                if (await GabanaAPI.CheckSpeedConnection())
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.stock_adapter_movement, parent, false);
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
}