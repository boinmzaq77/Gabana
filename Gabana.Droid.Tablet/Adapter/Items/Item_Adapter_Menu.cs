using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Items
{
    internal class Item_Adapter_Menu : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<MenuTab> ListMenu;
        public string positionClick;
        public Item_Adapter_Menu(List<MenuTab> l)
        {
            ListMenu = l;
        }
        public override int ItemCount
        {
            get { return ListMenu == null ? 0 : ListMenu.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewMenu vh = holder as ListViewMenu;
            if (DataCashing.Language == "th")
            {
                vh.Name.Text = ListMenu[position].NameMenuTh;
            }
            else
            {
                vh.Name.Text = ListMenu[position].NameMenuEn;

            }

            vh.Line.Visibility = ViewStates.Gone;
            if (Item_Fragment_Main.tabSelected == ListMenu[position].NameMenuEn)
            {
                vh.Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Line.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
                vh.Line.Visibility = ViewStates.Gone;
            }


        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_adapter_menu, parent, false);
            ListViewMenu vh = new ListViewMenu(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewMenu : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public View Line { get; set; }
        public ListViewMenu(View itemview, Action<int> listener) : base(itemview)
        {
            Name = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);
            Line = itemview.FindViewById<View>(Resource.Id.lineblue);
            itemview.Click += (sender, e) => listener(base.Position);
            itemview.LongClick += (sender, e) => listener(base.Position);
        }

        public void Select()
        {
            Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            Line.Visibility = ViewStates.Visible;
        }

        public void NotSelect()
        {
            Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
            Line.Visibility = ViewStates.Gone;
        }
    }
}