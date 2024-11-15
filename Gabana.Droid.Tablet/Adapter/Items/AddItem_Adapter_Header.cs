using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Items
{
    internal class AddItem_Adapter_Header : RecyclerView.Adapter
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
            ListViewMenu vh = holder as ListViewMenu;
            //vh.CategoryName.Text = ListMenu[position].NameMenuEn;
            if (DataCashing.Language == "th")
            {
                vh.Name.Text = ListMenu[position].NameMenuTh;
            }
            else
            {
                vh.Name.Text = ListMenu[position].NameMenuEn;

            }
            vh.Line.Visibility = ViewStates.Gone;
            string tabSelected = "";

            if (ipage == "item")
            {
                tabSelected = Item_Fragment_AddItem.tabSelected;
            }
            if (ipage == "extra")
            {
                tabSelected = Item_Fragment_AddTopping.tabSelected;
            }
            if (ipage == "POS_item")
            {
                tabSelected = POS_Dialog_AddItem.tabSelected;
            }
            if (ipage == "POS_extra")
            {
                tabSelected = POS_Dialog_AddTopping.tabSelected;
            }

            if (tabSelected == ListMenu[position].NameMenuEn)
            {
                vh.Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Line.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
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


}