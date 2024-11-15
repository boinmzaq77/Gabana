using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Option
{
    internal class Option_Adapter_NoteCategory : RecyclerView.Adapter
    {

        Context context;
        public event EventHandler<int> ItemClick;
        public ListNoteCategory listNoteCategory;
        public Option_Adapter_NoteCategory(ListNoteCategory l)
        {
            listNoteCategory = l;
        }

        public override int ItemCount
        {
            get { return listNoteCategory == null ? 0 : listNoteCategory.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewCategoryHolder vh = holder as ListViewCategoryHolder;
            vh.CategoryName.Text = listNoteCategory[position].Name;
            vh.Lineblue.Visibility = ViewStates.Gone;
            if (POS_Dialog_Option.nameCategoryNote == listNoteCategory[position].Name && POS_Dialog_Option.sysCategoryNote == listNoteCategory[position].SysNoteCategoryID)
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Lineblue.Visibility = ViewStates.Visible;
                vh.ItemView.RequestFocus();
            }
            else
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.eclipse, null));
                vh.Lineblue.Visibility = ViewStates.Gone;
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cart_adapter_notecategory, parent, false);
            ListViewCategoryHolder vh = new ListViewCategoryHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }

    public class ListNoteCategory
    {
        public List<ORM.MerchantDB.NoteCategory> notecategory;
        static List<ORM.MerchantDB.NoteCategory> builitem;
        public ListNoteCategory(List<ORM.MerchantDB.NoteCategory> notecategory)
        {
            builitem = notecategory;
            this.notecategory = builitem;
        }
        public int Count
        {
            get
            {
                return notecategory == null ? 0 : notecategory.Count;
            }
        }
        public ORM.MerchantDB.NoteCategory this[int i]
        {
            get { return notecategory == null ? null : notecategory[i]; }
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