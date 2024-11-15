using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    internal class Setting_Adapter_NoteCategory : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListNoteCategory listNoteCategory;
        public static  NoteCategory EditNoteCategory;
        public static ListViewCategoryHolder vh;
        public static ListViewCategoryHolder vhselect;
        public static int temp = -1;

        public Setting_Adapter_NoteCategory(ListNoteCategory l)
        {
            listNoteCategory = l;
        }
        public override int ItemCount
        {
            get { return listNoteCategory == null ? 0 : listNoteCategory.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            vh = holder as ListViewCategoryHolder;
            vh.CategoryName.Text = listNoteCategory[position].Name;
            vh.Lineblue.Visibility = ViewStates.Gone;
            if (Setting_Fragment_Note.FocusNoteCategeryID == listNoteCategory[position].SysNoteCategoryID)
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

            vh.ItemView.LongClick += ItemView_LongClick;
        }

        private void ItemView_LongClick(object sender, View.LongClickEventArgs e)
        {
            temp = (int)Setting_Fragment_Note.FocusNoteCategeryID;
            EditNoteCategory = listNoteCategory.notecategory.Where(x => x.SysNoteCategoryID == temp).FirstOrDefault();
            Setting_Fragment_Note.NoteCategoryLongClick(EditNoteCategory);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            ListViewCategoryHolder vh;
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_category, parent, false);
            vh = new ListViewCategoryHolder(itemView, OnClick);
            vh = new ListViewCategoryHolder(itemView, OnLongClick);

            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

        private void OnLongClick(int obj)
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
            CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
            Lineblue.Visibility = ViewStates.Gone;
        }
    }

}