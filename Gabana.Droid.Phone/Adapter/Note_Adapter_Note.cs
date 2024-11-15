using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using System;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Note_Adapter_Note : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListNoteData listNote;
        public string positionClick;
        ORM.MerchantDB.NoteCategory NoteCategory;

        public Note_Adapter_Note(ListNoteData l)
        {
            listNote = l;
        }
        public override int ItemCount
        {
            get { return listNote == null ? 0 : listNote.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewNoteDataHolder vh = holder as ListViewNoteDataHolder;
                vh.NoteName.Text = listNote[position].Message;

                if (listNote[position].SysNoteCategoryID != null)
                {
                    var NotecategoryID = listNote[position].SysNoteCategoryID?.ToString();
                    NoteCategory = NoteActivity.lstNoteCategory.Where(x => x.SysNoteCategoryID.ToString() == NotecategoryID).FirstOrDefault();
                    if (NoteCategory == null)
                    {
                        vh.NoteCategory.Text = "None Category";
                    }
                    else
                    {
                        vh.NoteCategory.Text = NoteCategory.Name;
                    }
                }
                else
                {
                    NoteCategory = null;
                    vh.NoteCategory.Text = "None Category";
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.note_adapter_note, parent, false);
            ListViewNoteDataHolder vh = new ListViewNoteDataHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
}