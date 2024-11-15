
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
    public class Report_Adapter_Category : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListCategory listCategory;
        public string positionClick;
        public List<Item> listItem;
        private List<Category> listChooseCategory;

        public Report_Adapter_Category(ListCategory l, List<Item> item, List<Category> c)
        {
            listCategory = l;
            listItem = item;
            listChooseCategory = c;
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
                    if (itemcategory.Count <= 1)
                    {
                        vh.CategoryItem.Text = Totalitem + " Item";

                    }
                    else
                    {
                        vh.CategoryItem.Text = Totalitem + " Items";
                    }
                }
                else
                {
                    itemcategory = null;
                    vh.CategoryItem.Text = 0 + " Item";
                }

                var index = listChooseCategory.FindIndex(x => x.SysCategoryID == listCategory[position].SysCategoryID);
                if (index == -1)
                {
                    vh.Check.Visibility = ViewStates.Invisible;
                }
                else
                {
                    vh.Check.Visibility = ViewStates.Visible;
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_choosecategory, parent, false);
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