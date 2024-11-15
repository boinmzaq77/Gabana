using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddNoteActivity : AppCompatActivity
    {
        public static AddNoteActivity addNote;
        NoteManage noteManage = new NoteManage();
        Note Insertnote = new Note();
        Note Editnote = new Note();
        string sys;
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        ListCategory ListCategory;
        EditText textNote;
        internal Button btnAdd;
        Spinner spinNoteCategory;
        ImageButton imgNoteCategory, btnBack;
        LinearLayout lnBack;
        TextView textTitle;
        RecyclerView mRecycleViewshowNote;
        FrameLayout btnDelete;
        string NoteName;
        List<string> SysNoteCategoryID;
        int SysNoteCateID, sysCategoryID;
        int systemID = 70;
        string UserLogin;
        public static string tabSelected = "";
        bool first = true, flagdatachange = false;
        DialogLoading dialogLoading = new DialogLoading();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addnote_activity_main);

                addNote = this;

                mRecycleViewshowNote = FindViewById<RecyclerView>(Resource.Id.recyclerview_listSubNote);
                btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                imgNoteCategory = FindViewById<ImageButton>(Resource.Id.imgNoteCategory);
                spinNoteCategory = FindViewById<Spinner>(Resource.Id.spinNoteCategory);
                textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                textNote = FindViewById<EditText>(Resource.Id.textNote);
                btnDelete = FindViewById<FrameLayout>(Resource.Id.btnDelete);

                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;
                imgNoteCategory.Click += ImgNoteCategory_Click;
                textNote.TextChanged += TextNote_TextChanged;
                btnDelete.Click += BtnDelete_Click;
                UserLogin = Preferences.Get("User", "");


                CheckJwt();

                if (dialogLoading != null & dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                if (DataCashing.EditNoteItem == null)
                {
                    textTitle.Text = GetString(Resource.String.addnote_activity_title);
                    btnAdd.Text = GetString(Resource.String.addnote_activity_title);
                    btnDelete.Visibility = Android.Views.ViewStates.Gone;
                    btnAdd.Click += BtnAdd_Click;
                    sysCategoryID = (int)NoteActivity.FocusNoteCategeryID;
                    if (sysCategoryID == 0)
                    {
                        SpinnerCategory();
                    }
                    else
                    {
                        //กรณีเลือกจาก NoteCate แล้วมาเลือก NoteCate ใหม่
                        SpinnerCategory();
                        SpinnerSelectNoteCategory();
                    }
                }
                else
                {
                    textTitle.Text = GetString(Resource.String.editnote_activity_title);
                    btnAdd.Text = GetString(Resource.String.textsave);
                    SpinnerCategory();
                    Editnote = DataCashing.EditNoteItem;
                    ShowDataEdit();
                    btnAdd.Click += BtnEdit_Click;
                }

                Item_Adapter_Subnote item_Adapter_Subnote = new Item_Adapter_Subnote(ListCategory);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                mRecycleViewshowNote.SetLayoutManager(gridLayoutItem);
                mRecycleViewshowNote.HasFixedSize = true;
                mRecycleViewshowNote.SetItemViewCacheSize(20);
                mRecycleViewshowNote.SetAdapter(item_Adapter_Subnote);
                SetButtonAdd(false);
                first = false;

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddNoteActivity");

            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("Add Note");
                return;
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("deleteType", "note");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at Add Note");
                return;
            }
        }

        private void TextNote_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            NoteName = textNote.Text.Trim();
            CheckDataChange();
        }

        private void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (DataCashing.EditNoteItem == null)
            {
                if (string.IsNullOrEmpty(textNote.Text))
                {
                    SetButtonAdd(false);
                    return;
                }
                SetButtonAdd(true);
                flagdatachange = true;
                return;
            }
            else
            {
                if (textNote.Text != Editnote.Message)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (Editnote.SysNoteCategoryID != null && Editnote.SysNoteCategoryID != SysNoteCateID)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (Editnote.SysNoteCategoryID == null && SysNoteCateID != 0)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }

                SetButtonAdd(false);
                flagdatachange = false;
                return;
            }
        }

        private void SetButtonAdd(bool enable)
        {

            if (enable)
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnAdd.Enabled = enable;
        }

        private void ImgNoteCategory_Click(object sender, EventArgs e)
        {
            spinNoteCategory.PerformClick();
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            DataCashing.EditNoteItem = null;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            try
            {
                if (DataCashing.EditNoteItem == null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "note");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "note");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            InsertNote();
            btnAdd.Enabled = true;
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            UpdateNote();
            btnAdd.Enabled = true;
        }

        public async void InsertNote()
        {
            try
            {
                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, systemID);
                sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                Insertnote.MerchantID = DataCashingAll.MerchantId;
                Insertnote.SysNoteID = Convert.ToInt64(sys);
                Insertnote.Ordinary = null;

                if (string.IsNullOrEmpty(NoteName))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }

                switch (SysNoteCateID)
                {
                    case 0:
                        Insertnote.SysNoteCategoryID = null;
                        break;
                    default:
                        Insertnote.SysNoteCategoryID = SysNoteCateID;
                        break;
                }

                Insertnote.Message = NoteName;
                Insertnote.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                Insertnote.UserLastModified = UserLogin;
                Insertnote.DataStatus = 'I';
                Insertnote.FWaitSending = 2;
                Insertnote.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                var checkName = await noteManage.CheckNoteName(Insertnote.Message);
                if (checkName)
                {
                    try
                    {
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                        var json = JsonConvert.SerializeObject(Insertnote);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "inserNote");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "insert");
                        bundle.PutString("detailitem", Insertnote.Message);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add Category");
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                var result = await noteManage.InsertNote(Insertnote);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.insertsucess), ToastLength.Short).Show();

                //JobQueue
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNote((int)Insertnote.MerchantID, (int)Insertnote.SysNoteID);
                }
                else
                {
                    Insertnote.FWaitSending = 2;
                    await noteManage.UpdateNote(Insertnote);
                }

                DataCashing.EditNoteItem = null;
                NoteActivity.SetFocusNew(SysNoteCateID, Insertnote);
                this.Finish();
            }
            catch (Exception ex)
            {
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertNote at Add Note");
            }
        }

        private async void ShowDataEdit()
        {
            try
            {
                NoteCategoryManage categoryManage = new NoteCategoryManage();

                var lstcategory = new List<NoteCategory>();
                var getallCategory = new List<NoteCategory>();
                var addcategory = new NoteCategory();

                addcategory = new NoteCategory()
                {
                    Name = "None",
                    SysNoteCategoryID = 0
                };
                lstcategory.Add(addcategory);
                getallCategory = await categoryManage.GetAllNoteCategory();
                lstcategory.AddRange(getallCategory);

                string[] category_array = lstcategory.Select(i => i.Name.ToString()).ToArray();
                var adapterCategory = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, category_array);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinNoteCategory.Adapter = adapterCategory;

                long? category = Editnote.SysNoteCategoryID;

                if (category != null)
                {
                    var data = getallCategory.Where(x => x.SysNoteCategoryID == category).FirstOrDefault();
                    if (data != null)
                    {
                        int position = adapterCategory.GetPosition(data.Name);
                        spinNoteCategory.SetSelection(position);
                    }
                    else
                    {
                        int position = adapterCategory.GetPosition("None");
                        spinNoteCategory.SetSelection(position);
                    }
                }
                else
                {
                    int position = adapterCategory.GetPosition("None");
                    spinNoteCategory.SetSelection(position);
                }
                textNote.Text = Editnote.Message;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("ShowDataEdit at Add Note");
                return;
            }
        }

        public async void UpdateNote()
        {
            try
            {
                string OldNameNote = Editnote.Message;
                if (string.IsNullOrEmpty(NoteName))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }

                switch (SysNoteCateID)
                {
                    case 0:
                        Editnote.SysNoteCategoryID = null;
                        break;
                    default:
                        Editnote.SysNoteCategoryID = SysNoteCateID;
                        break;
                }

                Editnote.Message = NoteName;
                Editnote.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                Editnote.UserLastModified = UserLogin;
                Editnote.DataStatus = 'M';
                Editnote.FWaitSending = 2;
                Editnote.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                //Check ชื่อ Note
                if (textNote.Text != OldNameNote && textNote.Text != string.Empty)
                {
                    var checkName = await noteManage.CheckNoteName(Editnote.Message);
                    if (checkName)
                    {
                        try
                        {
                            //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                            var json = JsonConvert.SerializeObject(Editnote);

                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("insertRepeat", "inserNote");
                            bundle.PutString("detailnnsert", json);
                            bundle.PutString("event", "update");
                            bundle.PutString("detailitem", Editnote.Message);
                            dialog.Arguments = bundle;
                            dialog.Show(SupportFragmentManager, myMessage);
                            return;
                        }
                        catch (Exception ex)
                        {
                            await TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("InsertItem at add Category");
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                            return;
                        }
                    }
                }

                var result = await noteManage.UpdateNote(Editnote);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();

                //JobQueue
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNote((int)Editnote.MerchantID, (int)Editnote.SysNoteID);
                }
                else
                {
                    Editnote.FWaitSending = 2;
                    await noteManage.UpdateNote(Editnote);
                }
                DataCashing.EditNoteItem = null;
                NoteActivity.SetFocusNew(SysNoteCateID, Editnote);
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("UpdateNote at Add Note");
            }
        }

        public async void SpinnerCategory()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                SysNoteCategoryID = new List<string>();
                NoteCategoryManage noteCategoryManage = new NoteCategoryManage();
                NoteCategory noteCategory = new NoteCategory();
                var category = new List<NoteCategory>();
                var getallCategory = new List<NoteCategory>();

                noteCategory = new NoteCategory()
                {
                    Name = "None Category",
                    SysNoteCategoryID = 0
                };
                category.Add(noteCategory);
                getallCategory = await noteCategoryManage.GetAllNoteCategory();
                category.AddRange(getallCategory);

                for (int i = 0; i < category.Count; i++)
                {
                    temp = category[i].Name.ToString();
                    temp2 = category[i].SysNoteCategoryID.ToString();
                    items.Add(temp);
                    SysNoteCategoryID.Add(temp2);
                }

                spinNoteCategory.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinNoteCategory_ItemSelected);
                var adapterCategory = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinNoteCategory.Adapter = adapterCategory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SpinnerCategory at Add Note");
            }
        }

        private void spinNoteCategory_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (SysNoteCategoryID == null)
            {
                return;
            }

            SysNoteCateID = Convert.ToInt32(SysNoteCategoryID[e.Position].ToString());

            CheckDataChange();

        }

        async void SpinnerSelectNoteCategory()
        {
            try
            {
                NoteCategoryManage categoryManage = new NoteCategoryManage();

                var lstcategory = new List<NoteCategory>();
                var getallCategory = new List<NoteCategory>();
                var addcategory = new NoteCategory();

                addcategory = new NoteCategory()
                {
                    Name = "None",
                    SysNoteCategoryID = 0
                };
                lstcategory.Add(addcategory);
                getallCategory = await categoryManage.GetAllNoteCategory();
                lstcategory.AddRange(getallCategory);

                string[] category_array = lstcategory.Select(i => i.Name.ToString()).ToArray();
                var adapterCategory = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, category_array);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinNoteCategory.Adapter = adapterCategory;

                NoteCategory category = getallCategory.Where(i => i.SysNoteCategoryID == sysCategoryID).FirstOrDefault();
                if (category != null)
                {
                    long? categoryID = category.SysNoteCategoryID;

                    foreach (var i in getallCategory)
                    {
                        if (categoryID == i.SysNoteCategoryID)
                        {
                            int position = adapterCategory.GetPosition(i.Name);
                            spinNoteCategory.SetSelection(position);
                            SysNoteCateID = (int)i.SysNoteCategoryID;
                        }
                    }
                }
                else
                {

                    int position = adapterCategory.GetPosition("None");
                    spinNoteCategory.SetSelection(position);
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SpinnerSelectNoteCategory at Add Note");
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }
    }
}

