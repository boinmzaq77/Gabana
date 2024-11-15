using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace Gabana.Droid
{
    public class Note_Dialog_EditNoteCategory : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel;
        Button btn_Delete;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Note_Dialog_EditNoteCategory NewInstance()
        {
            var frag = new Note_Dialog_EditNoteCategory { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.note_dialog_deletenotecategory, container, false);
            try
            {
                Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_Delete = view.FindViewById<Button>(Resource.Id.btn_Delete);

                btn_cancel.Click += Btn_cancel_Click;
                btn_Delete.Click += Btn_Delete_Click;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.StackTrace, ToastLength.Long).Show();
            }
            return view;
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {

        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            Dismiss();
            Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
            AddNote_Dialog_AddCategory dialog = new AddNote_Dialog_AddCategory();
            var transactionId = dialog.Show(ft, "DialogAddCategory");
        }


    }
}