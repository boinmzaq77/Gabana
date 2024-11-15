using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    public class AddNote_Dialog_AddCategory : Android.Support.V4.App.DialogFragment
    {
        Button BtnAddCategory;
        ImageButton btnBack;
        EditText textNameCategory;
        TextView txtTitle;
        LinearLayout lnEdit, lnBtnAddNoteCategory;
        FrameLayout frameClickDelete;
        Button btnSave;
        NoteCategoryManage noteCategoryManage = new NoteCategoryManage();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        string sys;
        int systemID = 60;
        NoteCategory AddNoteCategory = new NoteCategory();
        NoteCategory EditNoteCategory = new NoteCategory();
        NoteCategory NoteCategoryDetail;
        string NoteCategoryName;
        View view;
        public static AddNote_Dialog_AddCategory addNotecate;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static AddNote_Dialog_AddCategory NewInstance()
        {
            var frag = new AddNote_Dialog_AddCategory { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.addnote_dialog_addcategory, container, false);
            Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            try
            {
                addNotecate = this;
                lnEdit = view.FindViewById<LinearLayout>(Resource.Id.lnEdit);
                lnBtnAddNoteCategory = view.FindViewById<LinearLayout>(Resource.Id.lnBtnAddNoteCategory);
                frameClickDelete = view.FindViewById<FrameLayout>(Resource.Id.frameClickDelete);
                btnBack = view.FindViewById<ImageButton>(Resource.Id.btnBack);
                BtnAddCategory = view.FindViewById<Button>(Resource.Id.BtnAddCategory);
                textNameCategory = view.FindViewById<EditText>(Resource.Id.textNameCategory);
                txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
                btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
                btnBack.Click += BtnBack_Click;
                textNameCategory.TextChanged += TextNameCategory_TextChanged;
                frameClickDelete.Click += FrameClickDelete_Click;

                NoteCategoryDetail = NoteActivity.EditNoteCategory;
                if (NoteCategoryDetail == null)
                {
                    lnBtnAddNoteCategory.Visibility = ViewStates.Visible;
                    lnEdit.Visibility = ViewStates.Gone;
                    BtnAddCategory.Click += BtnAddCategory_Click;
                }
                else
                {
                    lnBtnAddNoteCategory.Visibility = ViewStates.Gone;
                    lnEdit.Visibility = ViewStates.Visible;
                    ShowData();
                    btnSave.Click += BtnSave_Click;
                }
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
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                EditNoteCategory.MerchantID = NoteCategoryDetail.MerchantID;
                EditNoteCategory.SysNoteCategoryID = NoteCategoryDetail.SysNoteCategoryID;
                EditNoteCategory.DataStatus = 'M';
                EditNoteCategory.FWaitSending = 2;
                EditNoteCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                EditNoteCategory.Name = NoteCategoryName;
                EditNoteCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                EditNoteCategory.DateCreated = Utils.GetTranDate(NoteCategoryDetail.DateCreated);

                var checkName = await noteCategoryManage.CheckNoteCategoryName(EditNoteCategory.Name);
                if (checkName)
                {
                    try
                    {
                        btnSave.Enabled = true;
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                        var json = JsonConvert.SerializeObject(EditNoteCategory);

                        Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "inserNoteCategory");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "update");
                        bundle.PutString("detailitem", EditNoteCategory.Name);
                        dialog.Arguments = bundle;
                        var transactionId = dialog.Show(ft, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        btnSave.Enabled = true;
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add NoteCategory");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                //NoteCategory = 'M'
                NoteCategoryManage NoteCategoryManage = new NoteCategoryManage();
                var check = await NoteCategoryManage.UpdateNoteCategory(EditNoteCategory);
                if (!check)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    btnSave.Enabled = true;
                    Dismiss();
                    return;
                }
                Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNoteCatagory((int)EditNoteCategory.MerchantID, (int)EditNoteCategory.SysNoteCategoryID);
                }
                else
                {
                    EditNoteCategory.FWaitSending = 2;
                    await NoteCategoryManage.UpdateNoteCategory(EditNoteCategory);
                }

                NoteActivity.SetFocusNew(EditNoteCategory.SysNoteCategoryID, null);
                NoteActivity.noteActivity.RefreshData();
                btnSave.Enabled = true;
                Dismiss();
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void FrameClickDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("deleteType", "notecategory");
                dialog.Arguments = bundle;
                var transactionId = dialog.Show(ft, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FrameClickDelete_Click at addnote");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void ShowData()
        {
            try
            {
                txtTitle.Text = GetString(Resource.String.editnotecategory_activity_title);
                textNameCategory.Text = NoteCategoryDetail.Name;
                textNameCategory.Focusable = true;
                textNameCategory.SetSelection(textNameCategory.Text.Length);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void TextNameCategory_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                NoteCategoryName = textNameCategory.Text.Trim();
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void BtnAddCategory_Click(object sender, EventArgs e)
        {
            try
            {
                BtnAddCategory.Enabled = false;
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }

                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, systemID);
                sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                //string NoteCategory = textNameCategory.Text.Trim();

                AddNoteCategory.MerchantID = DataCashingAll.MerchantId;
                AddNoteCategory.SysNoteCategoryID = Convert.ToInt64(sys);
                AddNoteCategory.Ordinary = null;

                if (string.IsNullOrEmpty(NoteCategoryName))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    BtnAddCategory.Enabled = true;
                    return;
                }

                AddNoteCategory.Name = NoteCategoryName;
                AddNoteCategory.DateCreated = Utils.GetTranDate(DateTime.UtcNow);
                AddNoteCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                AddNoteCategory.DataStatus = 'I';
                AddNoteCategory.FWaitSending = 2;
                AddNoteCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                var checkName = await noteCategoryManage.CheckNoteCategoryName(AddNoteCategory.Name);
                if (checkName)
                {
                    try
                    {
                        BtnAddCategory.Enabled = true;
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                        var json = JsonConvert.SerializeObject(AddNoteCategory);

                        Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "inserNoteCategory");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "insert");
                        bundle.PutString("detailitem", AddNoteCategory.Name);
                        dialog.Arguments = bundle;
                        var transactionId = dialog.Show(ft, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        BtnAddCategory.Enabled = true;
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add NoteCategory");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                var result = await noteCategoryManage.InsertNoteCategory(AddNoteCategory);
                Log.Debug("Insert", result.ToString());

                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    BtnAddCategory.Enabled = true;
                    return;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.insertsucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNoteCatagory((int)AddNoteCategory.MerchantID, (int)AddNoteCategory.SysNoteCategoryID);
                }
                else
                {
                    AddNoteCategory.FWaitSending = 2;
                    await noteCategoryManage.UpdateNoteCategory(AddNoteCategory);
                }
                NoteActivity.SetFocusNew(AddNoteCategory.SysNoteCategoryID,null);
                NoteActivity.noteActivity.RefreshData();
                BtnAddCategory.Enabled = true;
                this.Dismiss();
            }
            catch (Exception ex)
            {
                BtnAddCategory.Enabled = true;
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Insert at addnote dialog cate");
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void CheckDataChange()
        {
            try
            {
                if (NoteCategoryDetail == null)
                {
                    if (string.IsNullOrEmpty(NoteCategoryName))
                    {
                        SetButtonAdd(false);
                        return;
                    }
                    SetButtonAdd(true);
                    return;
                }
                else
                {
                    if (NoteCategoryName != NoteCategoryDetail.Name)
                    {
                        SetButtonAdd(true);
                        return;
                    }

                    SetButtonAdd(false);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetButtonAdd(bool enable)
        {
            if (NoteCategoryDetail == null)
            {
                if (enable)
                {
                    BtnAddCategory.SetBackgroundResource(Resource.Drawable.btnblue);
                    BtnAddCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }
                else
                {
                    BtnAddCategory.SetBackgroundResource(Resource.Drawable.btnborderblue);
                    BtnAddCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                }
                BtnAddCategory.Enabled = enable;
            }
            else
            {
                if (enable)
                {
                    btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }
                else
                {
                    btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                }
                btnSave.Enabled = enable;
            }
        }
    }
}