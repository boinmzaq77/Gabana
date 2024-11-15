using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class Note_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;
        static string Page;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Note_Dialog_Delete NewInstance(string _page)
        {
            Page = _page;
            var frag = new Note_Dialog_Delete { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.pos_dialog_deleteitem, container, false);
            try
            {
                btn_cancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btn_save = view.FindViewById<Button>(Resource.Id.btn_save);

                btn_cancel.Click += Btn_cancel_Click;
                btn_save.Click += Btn_save_Click;

                textconfirm1 = view.FindViewById<TextView>(Resource.Id.textconfirm1);
                textconfirm2 = view.FindViewById<TextView>(Resource.Id.textconfirm2);
                textconfirm1.Text = string.Empty;
                textconfirm2.Text = string.Empty;
                textconfirm1.Text = Application.Context.GetString(Resource.String.dialog_delete_note_1);
                textconfirm2.Text = Application.Context.GetString(Resource.String.dialog_delete_note_2);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void Btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                btn_save.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    btn_save.Enabled = true;
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                NoteManage noteManage = new NoteManage();
                if (DataCashing.EditNoteItem != null)
                {
                    Note Editnote = new Note();
                    Editnote = DataCashing.EditNoteItem;
                    Note noteDelete = new Note();
                    noteDelete = await noteManage.GetNote((int)Editnote.MerchantID, (int)Editnote.SysNoteID);
                    noteDelete.DataStatus = 'D';
                    noteDelete.FWaitSending = 2;
                    noteDelete.LastDateModified = DateTime.UtcNow;
                    var update = await noteManage.UpdateNote(noteDelete);
                    if (update)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();

                        if (await GabanaAPI.CheckNetWork())
                        {
                            JobQueue.Default.AddJobSendNote((int)Editnote.MerchantID, (int)Editnote.SysNoteID);
                        }
                        else
                        {
                            noteDelete.FWaitSending = 2;
                            await noteManage.UpdateNote(noteDelete);
                        }
                        DataCashingAll.flagNoteCategoryChange = true;
                        DataCashingAll.flagNoteChange = true;
                        if (Page != "main")
                        {
                            //หน้า add                           
                            this.Activity.Finish();
                        }
                        else
                        {
                            NoteActivity.noteActivity.RefreshData();
                        }
                        MainDialog.CloseDialog();
                    }
                    else
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                        if (Page == "main")
                        {
                            DataCashingAll.flagNoteCategoryChange = true;
                            DataCashingAll.flagNoteChange = true;
                            NoteActivity.noteActivity.RefreshData();
                        }
                        return;
                    }
                    btn_save.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at note_dialog");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }

    }
}
