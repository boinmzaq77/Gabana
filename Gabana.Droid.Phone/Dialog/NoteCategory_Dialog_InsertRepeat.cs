using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using TinyInsightsLib;

namespace Gabana.Droid.Phone
{
    public class NoteCategory_Dialog_InsertRepeat : Android.Support.V4.App.DialogFragment
    {
        Button btn_cancel, btn_save;
        static string name, Detail, Event;
        TextView textconfirm1, textconfirm2;
        NoteCategoryManage noteCateManage = new NoteCategoryManage();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static NoteCategory_Dialog_InsertRepeat NewInstance(string _name, string _detailinsert, string _event)
        {
            name = _name;
            Detail = _detailinsert;
            Event = _event;
            var frag = new NoteCategory_Dialog_InsertRepeat { Arguments = new Bundle() };
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

                var textItemName = name;
                var text1 = GetText(Resource.String.dialog_addnotecategory1);
                var text2 = GetText(Resource.String.dialog_additem2);

                textconfirm1.Text = text1 + " " + textItemName + " " + text2;
                textconfirm2.Text = Application.Context.GetString(Resource.String.dialog_additem3);
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
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                var DetailNote = JsonConvert.DeserializeObject<NoteCategory>(Detail);
                if (DetailNote != null)
                {
                    if (Event == "insert")
                    {
                        DetailInsert(DetailNote);
                    }
                    else
                    {
                        DetailUpdate(DetailNote);
                    }
                    MainDialog.CloseDialog();
                }
                btn_save.Enabled = true;
            }
            catch (Exception ex)
            {
                btn_save.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            MainDialog.CloseDialog();
        }


        async void DetailInsert(NoteCategory noteCategory)
        {
            try
            {
                var result = await noteCateManage.InsertNoteCategory(noteCategory);
                if (!result)
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.insertsucess), ToastLength.Short).Show();

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNoteCatagory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);
                }
                else
                {
                    noteCategory.FWaitSending = 2;
                    await noteCateManage.UpdateNoteCategory(noteCategory);
                }

                MainDialog.CloseDialog();
                AddNote_Dialog_AddCategory.addNotecate.Dismiss();
                NoteActivity.SetFocusNew(noteCategory.SysNoteCategoryID,null);
                NoteActivity.noteActivity.RefreshData();
                this.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailInsert at notecategory_dialog");
            }
        }

        async void DetailUpdate(NoteCategory noteCategory)
        {
            try
            {
                var result = await noteCateManage.UpdateNoteCategory(noteCategory);
                if (!result)
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.insertsucess), ToastLength.Short).Show();

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNoteCatagory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);
                }
                else
                {
                    noteCategory.FWaitSending = 2;
                    await noteCateManage.UpdateNoteCategory(noteCategory);
                }

                MainDialog.CloseDialog();
                AddNote_Dialog_AddCategory.addNotecate.Dismiss();                
                NoteActivity.SetFocusNew(noteCategory.SysNoteCategoryID, null);
                NoteActivity.noteActivity.RefreshData();
                this.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailUpdate at notecategory_dialog");
            }
        }
    }
}
