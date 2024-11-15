using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    public class Item_Adapter_Discount : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListDiscount listDiscount;
#pragma warning disable CS0414 // The field 'Item_Adapter_Discount.flag' is assigned but its value is never used
        bool flag = false;
#pragma warning restore CS0414 // The field 'Item_Adapter_Discount.flag' is assigned but its value is never used
        public string positionClick;

        public Item_Adapter_Discount(ListDiscount l)
        {
            listDiscount = l;
        }
        public override int ItemCount
        {
            get { return listDiscount == null ? 0 : listDiscount.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewDiscountHolder vh = holder as ListViewDiscountHolder;

            vh.Discount.Text = listDiscount[position].TemplateName?.ToString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_discount, parent, false);
            ListViewDiscountHolder vh = new ListViewDiscountHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}