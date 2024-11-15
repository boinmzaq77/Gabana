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

namespace Gabana.Droid.Tablet.Adapter.Login
{
    public class Login_Adapter_TypeLogin : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        List<MenuTab> ListMenu;
        private string tabSelected;

        public Login_Adapter_TypeLogin(List<MenuTab> l, string t)
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
            ListViewTypeLoginHolder vh = holder as ListViewTypeLoginHolder;
            if (DataCashing.Language == "th")
            {
                vh.Name.Text = ListMenu[position].NameMenuTh;
            }
            else
            {
                vh.Name.Text = ListMenu[position].NameMenuEn;

            }
            vh.Lineblue.Visibility = ViewStates.Gone;


            if (tabSelected == ListMenu[position].NameMenuEn)
            {
                vh.Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Lineblue.Visibility = ViewStates.Visible;
            }
            else
            {
                vh.Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                vh.Lineblue.Visibility = ViewStates.Gone;
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.login_adapter_typelogin, parent, false);
            ListViewTypeLoginHolder vh = new ListViewTypeLoginHolder(itemView, OnClick);
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
    public class ListViewTypeLoginHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; set; }
        public View Lineblue { get; set; }
        public ListViewTypeLoginHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Name = itemview.FindViewById<TextView>(Resource.Id.txtName);
            Lineblue = itemview.FindViewById<View>(Resource.Id.lineblue);
            itemview.Click += (sender, e) => listener(base.Position);
            itemview.LongClick += (sender, e) => listener(base.Position);
        }

        public void Select()
        {
            Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            Lineblue.Visibility = ViewStates.Visible;
        }

        public void NotSelect()
        {
            Name.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textgray, null));
            Lineblue.Visibility = ViewStates.Gone;
        }
    }
}