using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.ORM.MerchantDB;
using System;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Note_Adapter_Header : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListNoteCategory listNoteCategory;
        public static NoteCategory EditNoteCategory;
        public static ListViewCategoryHolder vh;
        public static ListViewCategoryHolder vhselect;
        public static int temp = -1;

        public Note_Adapter_Header(ListNoteCategory l)
        {
            listNoteCategory = l;
        }

        public override int ItemCount
        {
            get { return listNoteCategory == null ? 0 : listNoteCategory.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                vh = holder as ListViewCategoryHolder;
                vh.CategoryName.Text = listNoteCategory[position].Name;
                vh.Lineblue.Visibility = ViewStates.Gone;                
                if (NoteActivity.FocusNoteCategeryID == listNoteCategory[position].SysNoteCategoryID)
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

                vh.ItemView.LongClick += ItemView_LongClick;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context,ex.Message,ToastLength.Short).Show();
            }
        }

        private void ItemView_LongClick(object sender, View.LongClickEventArgs e)
        {
            temp =  (int)NoteActivity.FocusNoteCategeryID;
            EditNoteCategory = listNoteCategory.notecategory.Where(x=>x.SysNoteCategoryID == temp).FirstOrDefault();
            NoteActivity.NoteCategoryLongClick(EditNoteCategory);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            ListViewCategoryHolder vh;
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.pos_adapter_menuheader, parent, false);
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
}