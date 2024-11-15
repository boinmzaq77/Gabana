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
    public class Setting_Dialog_DeleteNote : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Dialog_DeleteNote NewInstance()
        {
            var frag = new Setting_Dialog_DeleteNote { Arguments = new Bundle() };
            return frag;
        }
        View view;
        Button btnCancel, btnSave;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.setting_dialog_deletenote, container, false);
            try
            {
                btnCancel = view.FindViewById<Button>(Resource.Id.btnCancel);
                btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

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
                NoteManage noteManage = new NoteManage();
                if (DataCashing.EditNote != null)
                {
                    Note Editnote = new Note();
                    Editnote = DataCashing.EditNote;
                    Note noteDelete = new Note();
                    noteDelete = await noteManage.GetNote((int)Editnote.MerchantID, (int)Editnote.SysNoteID);
                    noteDelete.DataStatus = 'D';
                    noteDelete.FWaitSending = 2;
                    noteDelete.LastDateModified = DateTime.UtcNow;
                    var update = await noteManage.UpdateNote(noteDelete);
                    if (!update)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();
                        return;
                    }

                    Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();

                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendNote((int)noteDelete.MerchantID, (int)noteDelete.SysNoteID);
                    }
                    else
                    {
                        noteDelete.FWaitSending = 2;
                        await noteManage.UpdateNote(noteDelete);
                    }
                    DataCashingAll.flagNoteChange = true;
                    DataCashing.EditNote = null;
                    Setting_Fragment_Note.fragment_main.DeleteNote(noteDelete);
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "note");
                    this.Dialog.Dismiss();
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
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