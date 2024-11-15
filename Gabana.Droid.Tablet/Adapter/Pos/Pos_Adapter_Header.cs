using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.POS
{
    public class Pos_Adapter_Header : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<MenuTabwithSysCategory> ListMenu;
        public static ListViewCategoryHolder vh;
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
            vh = holder as ListViewCategoryHolder;

            if (DataCashing.Language == "th")
            {
                vh.CategoryName.Text = ListMenu[position].NameMenuTh;
            }
            else
            {
                vh.CategoryName.Text = ListMenu[position].NameMenuEn;

            }

            vh.Lineblue.Visibility = ViewStates.Gone;
            if (POS_Fragment_Main.tabSelected == ListMenu[position].NameMenuEn)
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Lineblue.Visibility = ViewStates.Visible;
                vhselect = vh;
                vh.ItemView.RequestFocus();
            }
            else
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                vh.Lineblue.Visibility = ViewStates.Gone;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.pos_adapter_menuheader, parent, false);
            vh = new ListViewCategoryHolder(itemView, OnClick);
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
        public View Lineblue { get; set; }
        public ListViewCategoryHolder(View itemview, Action<int> listener) : base(itemview)
        {
            CategoryName = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);
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