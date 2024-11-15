using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_MenuTime : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        List<MenuTab> ListMenu;
        private string tabSelected;
        public string positionClick;
        public Report_Adapter_MenuTime(List<MenuTab> l, string t)
        {
            ListMenu = l;
            tabSelected = t;
        }
        public override int ItemCount
        {
            get { return ListMenu == null ? 0 : ListMenu.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewMenuHolder vh = holder as ListViewMenuHolder;
            if (DataCashing.Language == "th")
            {
                vh.CategoryName.Text = ListMenu[position].NameMenuTh;
            }
            else
            {
                vh.CategoryName.Text = ListMenu[position].NameMenuEn;

            }
            vh.Lineblue.Visibility = ViewStates.Gone;


            if (tabSelected == ListMenu[position].NameMenuEn)
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Lineblue.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                vh.Lineblue.Visibility = ViewStates.Gone;
            }


        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_menutime, parent, false);
            ListViewMenuHolder vh = new ListViewMenuHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewMenuHolder : RecyclerView.ViewHolder
    {
        public TextView CategoryName { get; set; }
        public TextView CategoryItem { get; set; }
        public ImageView Check { get; set; }
        public View Lineblue { get; set; }
        public ListViewMenuHolder(View itemview, Action<int> listener) : base(itemview)
        {
            CategoryName = itemview.FindViewById<TextView>(Resource.Id.txtNameCategory);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryTotal);

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
            CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
            Lineblue.Visibility = ViewStates.Gone;
        }
    }

}