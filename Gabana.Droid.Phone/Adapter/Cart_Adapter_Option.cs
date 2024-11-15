using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.ShareSource;
using System;

namespace Gabana.Droid.Adapter
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
}