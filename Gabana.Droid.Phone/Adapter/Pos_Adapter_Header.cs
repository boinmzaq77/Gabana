using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class Pos_Adapter_Header : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<MenuTabwithSysCategory> ListMenu;
        public static ListViewCategoryHolder vhselect;

        public Pos_Adapter_Header(List<MenuTabwithSysCategory> l)
        {
            ListMenu = l;
        }
        public override int ItemCount
        {
            get { return ListMenu == null ? 0 : ListMenu.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewCategoryHolder vh = holder as ListViewCategoryHolder;
            vh.CategoryName.Text = DataCashing.Language == "th" ? ListMenu[position].NameMenuTh : ListMenu[position].NameMenuEn;
            vh.Lineblue.Visibility = ViewStates.Gone;

            if (PosActivity.tabSelected == ListMenu[position].NameMenuEn)
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Lineblue.Visibility = ViewStates.Visible;
                vhselect = vh;
                vh.ItemView.RequestFocus();
            }
            else
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
                vh.Lineblue.Visibility = ViewStates.Gone;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.pos_adapter_menuheader, parent, false);
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