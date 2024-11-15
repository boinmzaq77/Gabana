using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class AddItem_Adapter_Header : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        List<MenuTab> ListMenu;
        public string positionClick;
        string ipage;

        public AddItem_Adapter_Header(List<MenuTab> l, string v)
        {
            ListMenu = l;
            ipage = v;
        }
        public override int ItemCount
        {
            get { return ListMenu == null ? 0 : ListMenu.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewCategoryHolder vh = holder as ListViewCategoryHolder;
            //vh.CategoryName.Text = ListMenu[position].NameMenuEn;
            if (DataCashing.Language == "th")
            {
                vh.CategoryName.Text = ListMenu[position].NameMenuTh;
            }
            else
            {
                vh.CategoryName.Text = ListMenu[position].NameMenuEn;

            }
            vh.Lineblue.Visibility = ViewStates.Gone;
            string tabSelected = "";

            if (ipage == "item")
            {
                tabSelected = AddItemActivity.tabSelected;
            }
            if (ipage == "extra")
            {
                tabSelected = AddExtraToppingActivity.tabSelected;
            }

            if (tabSelected == ListMenu[position].NameMenuEn)
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Lineblue.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
                vh.Lineblue.Visibility = ViewStates.Gone;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_menuheader, parent, false);
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