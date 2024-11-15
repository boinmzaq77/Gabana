using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    internal class Setting_Adapter_Note : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListNoteData listNote;
        public string positionClick;
        ORM.MerchantDB.NoteCategory NoteCategory;

        public Setting_Adapter_Note(ListNoteData l)
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
                ListViewNoteHolder vh = holder as ListViewNoteHolder;
                vh.NoteName.Text = listNote[position].Message;

                if (listNote[position].SysNoteCategoryID != null)
                {
                    var NotecategoryID = listNote[position].SysNoteCategoryID?.ToString();
                    NoteCategory = Setting_Fragment_Note.lstCategory.Where(x => x.SysNoteCategoryID.ToString() == NotecategoryID).FirstOrDefault();
                    vh.NoteCategory.Text = NoteCategory.Name;
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_note, parent, false);
            ListViewNoteHolder vh = new ListViewNoteHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
    public class ListViewNoteHolder : RecyclerView.ViewHolder
    {
        public TextView NoteName { get; set; }
        public TextView NoteCategory { get; set; }
        public ListViewNoteHolder(View itemview, Action<int> listener) : base(itemview)
        {
            NoteName = itemview.FindViewById<TextView>(Resource.Id.txtNameNote);
            NoteCategory = itemview.FindViewById<TextView>(Resource.Id.txtSubNote);

            itemview.Click += (sender, e) => listener(base.Position);

        }
    }


    public class ListNoteData
    {
        public List<Note> Note;
        static List<Note> builitem;
        public ListNoteData(List<Note> noteDetails)
        {
            builitem = noteDetails;
            Note = builitem;
        }

        public int Count
        {
            get
            {
                return Note.Count;
            }
        }

        public Note this[int i]
        {
            get { return Note.Count == 0 ? null : Note[i]; }
        }
    }
}