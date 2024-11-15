using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    public class Option_Adapter_Note : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListNoteData listNote;
        public string positionClick;
        public Option_Adapter_Note(ListNoteData l)
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
                ListViewNoteOptionHolder vh = holder as ListViewNoteOptionHolder;
                vh.textSizeName.Text = listNote[position].Message;
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.option_adapter_note, parent, false);
            ListViewNoteOptionHolder vh = new ListViewNoteOptionHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
}