using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class NoteCategory_Dialog_Delete : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        TextView textconfirm1, textconfirm2;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static NoteCategory_Dialog_Delete NewInstance()
        {
            var frag = new NoteCategory_Dialog_Delete { Arguments = new Bundle() };
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
                textconfirm1.Text = Application.Context.GetString(Resource.String.dialog_delete_notecategory_1);
                textconfirm2.Text = Application.Context.GetString(Resource.String.dialog_delete_notecategory_2);

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
                NoteCategory noteCategory = NoteActivity.EditNoteCategory;
                if (noteCategory == null)
                {
                    btn_save.Enabled = true;
                    return;
                }
                noteCategory.FWaitSending = 2;
                noteCategory.WaitSendingTime = DateTime.UtcNow;
                noteCategory.DataStatus = 'D';
                //NoteCategory = 'D'
                NoteCategoryManage NoteCategoryManage = new NoteCategoryManage();
                var check = await NoteCategoryManage.UpdateNoteCategory(noteCategory);
                if (!check)
                {
                    btn_save.Enabled = true;
                    Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                    Dismiss();
                }

                Toast.MakeText(this.Activity, Application.Context.GetString(Resource.String.deletesucess), ToastLength.Short).Show();

                //Note ที่มี sysNoteCategory  = ตัวที่ลบ ก็ต้องอัพเดตให้ datastatus = 'D'
                NoteManage noteManage = new NoteManage();
                var updatetoNote = await noteManage.GetNoteBYCategory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);

                foreach (var note in updatetoNote)
                {
                    note.DataStatus = 'D';
                    await noteManage.UpdateNote(note);
                }

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNoteCatagory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);
                }
                else
                {
                    noteCategory.FWaitSending = 2;
                    await NoteCategoryManage.UpdateNoteCategory(noteCategory);
                }
                DataCashingAll.flagNoteCategoryChange = true;
                DataCashingAll.flagNoteChange = true;
                NoteActivity.FocusNoteCategeryID = 0; //เพื่อให้แสดงข้อมูลทั้งหมด
                NoteActivity.noteActivity.RefreshData();
                AddNote_Dialog_AddCategory.addNotecate.Dismiss();
                MainDialog.CloseDialog();                
                btn_save.Enabled = true;
            }
            catch (Exception ex)
            {
                btn_save.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at notecate");
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
