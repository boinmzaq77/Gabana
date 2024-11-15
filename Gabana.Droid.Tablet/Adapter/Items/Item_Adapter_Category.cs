using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Items
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
    public class ListViewCategoryHolder : RecyclerView.ViewHolder
    {
        public TextView CategoryName { get; set; }
        public TextView CategoryItem { get; set; }
        public ImageView Check { get; set; }
        public View Lineblue { get; set; }
        public ListViewCategoryHolder(View itemview, Action<int> listener) : base(itemview)
        {
            CategoryName = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryTotal);
            //Check = itemview.FindViewById<ImageView>(Resource.Id.imgCheck);

            Lineblue = itemview.FindViewById<View>(Resource.Id.lineblue);
            itemview.Click += (sender, e) => listener(base.Position);
            itemview.LongClick += (sender, e) => listener(base.Position);
        }

        public void Select()
        {
            CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            Lineblue.Visibility = ViewStates.Visible;
        }

        public void NotSelect()
        {
            CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
            Lineblue.Visibility = ViewStates.Gone;
        }
    }
}