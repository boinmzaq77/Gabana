using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;

namespace Gabana.Droid.Adapter
{
    public class Item_Adapter_Subnote : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCategory listCategory;
        public string positionClick;

        public Item_Adapter_Subnote(ListCategory l)
        {
            listCategory = l;
        }
        public override int ItemCount
        {
            get { return listCategory == null ? 0 : listCategory.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewCategoryHolder vh = holder as ListViewCategoryHolder;
                vh.CategoryName.Text = listCategory[position].Name?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_addsubnote, parent, false);
            ListViewCategoryHolder vh = new ListViewCategoryHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}