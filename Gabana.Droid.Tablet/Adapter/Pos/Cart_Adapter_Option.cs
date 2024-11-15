using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Pos
{
    public class Cart_Adapter_Option : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListOption ListOption;
        public string positionClick;

        public Cart_Adapter_Option(ListOption l)
        {
            ListOption = l;
        }
        public override int ItemCount
        {
            get { return ListOption == null ? 0 : ListOption.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewOptionHolder vh = holder as ListViewOptionHolder;
            vh.Name.Text = ListOption[position].ItemName?.ToString();
            var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

            vh.Price.Text = "(" + Utils.DisplayDecimal((ListOption[position].Quantity * ListOption[position].ToppingPrice)) + ")";
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cart_adapter_option, parent, false);
            ListViewOptionHolder vh = new ListViewOptionHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
    public class ListViewOptionHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public TextView Price { get; set; }


        public ListViewOptionHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Name = itemview.FindViewById<TextView>(Resource.Id.textName);
            Price = itemview.FindViewById<TextView>(Resource.Id.textPrice);

            itemview.Click += (sender, e) => listener(base.Position);

        }
    }

}