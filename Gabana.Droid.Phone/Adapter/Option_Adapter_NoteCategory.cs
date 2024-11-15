using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    public class Option_Adapter_NoteCategory : RecyclerView.Adapter
    {
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
            if (OptionActivity.nameCategoryNote == listNoteCategory[position].Name && OptionActivity.sysCategoryNote == listNoteCategory[position].SysNoteCategoryID)
            {
                vh.CategoryName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                vh.Lineblue.Visibility = ViewStates.Visible;
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.option_adapter_categoryextra, parent, false);
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