using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dialog_DeleteNoteCate : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Dialog_DeleteNoteCate NewInstance()
        {
            var frag = new Setting_Dialog_DeleteNoteCate { Arguments = new Bundle() };
            return frag;
        }
        View view;
        Button btnCancel, btnSave;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.setting_dialog_deletecatenote, container, false);
            try
            {
                btnCancel = view.FindViewById<Button>(Resource.Id.btn_cancel);
                btnSave = view.FindViewById<Button>(Resource.Id.btn_save);

                btnCancel.Click += BtnCancel_Click; 
                btnSave.Click += BtnSave_Click; 
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    btnSave.Enabled = true;
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                NoteCategory noteCategory = Setting_Fragment_Note.EditNoteCategory;
                if (noteCategory == null)
                {
                    btnSave.Enabled = true;
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
                    btnSave.Enabled = true;
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
                Setting_Fragment_Note.FocusNoteCategeryID = 0; //เพื่อให้แสดงข้อมูลทั้งหมด
                Setting_Fragment_Note.fragment_main.RefreshData();
                Setting_Dialog_AddNote.addNotecate.Dismiss();
                this.Dialog.Dismiss();
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btn_save_Click at note_dialog");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Dialog.Dismiss();
        }
    }
}