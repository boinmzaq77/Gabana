using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Item_Adapter_Category : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCategory listCategory;
        public string positionClick;
        public List<Item> listItem;

        public Item_Adapter_Category(ListCategory l, List<Item> item)
        {
            listCategory = l;
            listItem = item;
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
                var categoryid = listCategory[position].SysCategoryID.ToString();
                //Item item = new Item();
                List<Item> itemcategory;

                if (listItem != null)
                {
                    itemcategory = listItem.Where(x => x.SysCategoryID.ToString() == categoryid).ToList();
                    var Totalitem = itemcategory.Count.ToString();
                    vh.CategoryItem.Text = Totalitem + " Items";
                }
                else
                {
                    itemcategory = null;
                    vh.CategoryItem.Text = 0 + " Items";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_category, parent, false);
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