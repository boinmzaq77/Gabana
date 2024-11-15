using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AutoMapper.Configuration.Conventions;
using Gabana.Droid.Adapter;
using Gabana.Droid.Helper;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.Master;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using static Java.Text.Normalizer;

namespace Gabana.Droid
{
    [Activity(Label = "NoteActivityNew")]
    public class NoteActivity : AppCompatActivity
    {
        public static NoteActivity noteActivity;
        RecyclerView recyclerViewCategory;
        FrameLayout FrameSearchNote;
        EditText editSearchNote;
        RecyclerView recyclerViewNote;
        LinearLayout lnNonote;
        ListNoteCategory listNoteCate;
        public static List<ORM.MerchantDB.NoteCategory> lstNoteCategory = new List<ORM.MerchantDB.NoteCategory>();
        public static List<ORM.MerchantDB.Note> lstNote,lstSearchNote;
        List<ORM.MerchantDB.Note> DefaultNote;
        ListNoteData ListdataNote;
        public static ORM.MerchantDB.NoteCategory EditNoteCategory;
        ORM.MerchantDB.Note Editnote = new ORM.MerchantDB.Note();
        SwipeRefreshLayout refreshlayout;
        string searchNote;
        ImageButton btnSearch;
        Note_Adapter_Header note_adapter_header;
        List<ORM.MerchantDB.SystemRevisionNo> listRivision = new List<ORM.MerchantDB.SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        int maxNoteRevision = 0;
        int maxNoteCategoryRevision = 0;
        NoteManage noteManage = new NoteManage();
        ORM.MerchantDB.Note note = new ORM.MerchantDB.Note();
        NoteCategoryManage noteCategoryManage = new NoteCategoryManage();
        ORM.MerchantDB.NoteCategory noteCategory = new ORM.MerchantDB.NoteCategory();
        public static bool checkManinRole;
        private string LoginType;
        Button addNote;
        Note_Adapter_Note note_Adapter_note;
        public static long FocusNoteCategeryID = 0; 
        public static ORM.MerchantDB.Note FocusNote;
        DialogLoading dialogLoading = new DialogLoading();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.note_activity);

                noteActivity = this;
                recyclerViewCategory = FindViewById<RecyclerView>(Resource.Id.recyclerViewCategoryNote);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                lnNonote = FindViewById<LinearLayout>(Resource.Id.lnNonote);
                LinearLayout lnAddCategoryNote = FindViewById<LinearLayout>(Resource.Id.lnAddCategoryNote);
                ImageButton btnAddCategoryNote = FindViewById<ImageButton>(Resource.Id.btnAddCategoryNote);
                addNote = FindViewById<Button>(Resource.Id.addNote);
                btnSearch = FindViewById<ImageButton>(Resource.Id.btnSearch);
                btnSearch.Click += BtnSearch_Click;

                FrameSearchNote = FindViewById<FrameLayout>(Resource.Id.lnSearchDiscount);
                editSearchNote = FindViewById<EditText>(Resource.Id.editSearchDiscount);
                recyclerViewNote = FindViewById<RecyclerView>(Resource.Id.recyclerview_list);

                editSearchNote.TextChanged += EditSearchNote_TextChanged;
                editSearchNote.KeyPress += EditSearchNote_KeyPress;
                editSearchNote.FocusChange += EditSearchNote_FocusChange;

                addNote.Click += AddNote_Click;
                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;
                lnAddCategoryNote.Click += LnAddCategoryNote_Click;
                btnAddCategoryNote.Click += LnAddCategoryNote_Click;

                CheckJwt();
                EditNoteCategory = null;
                refreshlayout = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
                refreshlayout.Refresh += async (sender, e) =>
                {
                    DataCashingAll.flagNoteCategoryChange = true;
                    DataCashingAll.flagNoteChange = true;
                    if (!await GabanaAPI.CheckNetWork())
                    {
                        Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    }
                    else if (!await GabanaAPI.CheckSpeedConnection())
                    {
                        Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                    else
                    {
                        await GetOnlineDataNoteCategory();
                        await GetOnlineDataNote();
                        OnResume();
                    }
                    BackgroundWorker work = new BackgroundWorker();
                    work.DoWork += Work_DoWork;
                    work.RunWorkerCompleted += Work_RunWorkerCompleted;
                    work.RunWorkerAsync();
                };

                LoginType = Preferences.Get("LoginType", "");
                checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "note");
                UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "note");

                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                var w = mainDisplayInfo.Width;
                var Width = w / 5;

                DataCashingAll.flagNoteCategoryChange = true;
                DataCashingAll.flagNoteChange = true;

                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this, recyclerViewNote, (int)Width);
                _ = TinyInsights.TrackPageViewAsync("OnCreate : NoteActivity");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Note");
                return;
            }
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            refreshlayout.Refreshing = false;
        }

        private void LnAddCategoryNote_Click(object sender, EventArgs e)
        {            
            if (!checkManinRole)
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;
            }

            EditNoteCategory = null;
            var fragment = new AddNote_Dialog_AddCategory();
            fragment.Show(SupportFragmentManager, nameof(AddNote_Dialog_AddCategory));
        }

        private async Task GetNoteCategoryData()
        {
            try
            {
                NoteCategoryManage category = new NoteCategoryManage();
                List<ORM.MerchantDB.NoteCategory> firstCategory = new List<ORM.MerchantDB.NoteCategory>();
                if (DataCashing.Language == "th")
                {
                    firstCategory.Add(new ORM.MerchantDB.NoteCategory { MerchantID = DataCashingAll.MerchantId, SysNoteCategoryID = 0, Ordinary = null, Name = "ทั้งหมด", DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 1, WaitSendingTime = DateTime.UtcNow });
                }
                else
                {
                    firstCategory.Add(new ORM.MerchantDB.NoteCategory { MerchantID = DataCashingAll.MerchantId, SysNoteCategoryID = 0, Ordinary = null, Name = "All", DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 1, WaitSendingTime = DateTime.UtcNow });
                }

                lstNoteCategory = await category.GetAllNoteCategory();
                if (lstNoteCategory != null)
                {
                    firstCategory.AddRange(lstNoteCategory);
                }

                listNoteCate = new ListNoteCategory(firstCategory);                              
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
            }
        }

        private void SetNoteCategoryData()
        {
            try
            {
                LinearLayoutManager layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                recyclerViewCategory.HasFixedSize = true;
                recyclerViewCategory.SetLayoutManager(layoutManager);
                note_adapter_header = new Note_Adapter_Header(listNoteCate);
                recyclerViewCategory.SetAdapter(note_adapter_header);
                note_adapter_header.ItemClick += Note_Adapter_Header_ItemClick;

                ListViewCategoryHolder categoryHolder;
                if (FocusNoteCategeryID > 0)
                {
                    int index = lstNoteCategory.FindIndex(x => x.SysNoteCategoryID == FocusNoteCategeryID);
                    if (index == -1) 
                    {
                        //return;
                        //เปลี่ยนเป็นถ้าไปหาไม่เจอให้แสดงทั้งหมดแทน
                        FocusNoteCategeryID = 0;
                        index = 0;
                    }                       
                    categoryHolder = recyclerViewCategory.FindViewHolderForAdapterPosition(index) as ListViewCategoryHolder;                                      
                }
                else
                {
                    categoryHolder = recyclerViewCategory.FindViewHolderForAdapterPosition(0) as ListViewCategoryHolder;                                       
                }
                if (categoryHolder != null)
                {
                    categoryHolder.Select();
                    Note_Adapter_Header.vhselect = categoryHolder;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetCategoryMenuShow at Note");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
            }
        }

        private void Note_Adapter_Header_ItemClick(object sender, int e)
        {
            try
            {
                if (e == -1)
                {
                    return;
                }

                Note_Adapter_Header.vhselect.NotSelect();
                if (e == 0)
                {
                    FocusNoteCategeryID = 0;
                }
                else
                {
                    var category = lstNoteCategory.Where(x => x.SysNoteCategoryID == (int)listNoteCate[e].SysNoteCategoryID).FirstOrDefault();
                    if (category != null)
                    {
                        FocusNoteCategeryID = category.SysNoteCategoryID;                        
                    }
                    else
                    {
                        FocusNoteCategeryID = 0;
                    }
                }
                var viewCategoryHolder = recyclerViewCategory.FindViewHolderForAdapterPosition(e) as ListViewCategoryHolder;
                if (viewCategoryHolder != null) 
                {
                    viewCategoryHolder.Select();
                    Note_Adapter_Header.vhselect = viewCategoryHolder;
                }
                SetDataNote();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
            }
        }

        public static async void NoteCategoryLongClick(ORM.MerchantDB.NoteCategory n)
        {
            try
            {
                EditNoteCategory = n;
                if (EditNoteCategory == null)
                {
                    return;
                }
                 
                if (EditNoteCategory.SysNoteCategoryID > 0)
                {
                    var fragment = new AddNote_Dialog_AddCategory();
                    fragment.Show(noteActivity.SupportFragmentManager, nameof(AddNote_Dialog_AddCategory));
                    fragment.Cancelable = false;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteCategoryLongClick at Note");
                Toast.MakeText(noteActivity, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
            DataCashing.EditNoteItem = null;
            EditNoteCategory = null;
            FocusNoteCategeryID = 0;
            FocusNote = null;
        }

        private void AddNote_Click(object sender, EventArgs e)
        {
            addNote.Enabled = false;
            if (!checkManinRole)
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                addNote.Enabled = true;
                return;
            }
            DataCashing.EditNoteItem = null;
            StartActivity(new Intent(Application.Context, typeof(AddNoteActivity)));
            addNote.Enabled = true;
        }

        private void EditSearchNote_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchNote = editSearchNote.Text.Trim();
            if (string.IsNullOrEmpty(searchNote))
            {
                SetDataNote();
            }
            SetBtnSearchItem();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            editSearchNote.ClearFocus();
            OnResume();
        }

        private void EditSearchNote_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus || !string.IsNullOrEmpty(editSearchNote.Text.Trim()))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
            }
        }

        private void EditSearchNote_KeyPress(object sender, View.KeyEventArgs e)
        {
            SetBtnSearchItem();
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                SetFilterNote();
                SetBtnSearchItem();
            }
            View view = this.CurrentFocus;
            if (view != null)
            {
                if (e.KeyCode != Keycode.Del && e.KeyCode != Keycode.ShiftLeft && e.KeyCode != Keycode.ShiftRight)
                {
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(view.WindowToken, 0);
                }
            }
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del)
            {
                e.Handled = false;
            }

            e.Handled = false;
            if (e.Handled)
            {
                string input = string.Empty;
                switch (e.KeyCode)
                {
                    case Keycode.Num0:
                        input += "0";
                        break;
                    case Keycode.Num1:
                        input += "1";
                        break;
                    case Keycode.Num2:
                        input += "2";
                        break;
                    case Keycode.Num3:
                        input += "3";
                        break;
                    case Keycode.Num4:
                        input += "4";
                        break;
                    case Keycode.Num5:
                        input += "5";
                        break;
                    case Keycode.Num6:
                        input += "6";
                        break;
                    case Keycode.Num7:
                        input += "7";
                        break;
                    case Keycode.Num8:
                        input += "8";
                        break;
                    case Keycode.Num9:
                        input += "9";
                        break;
                    default:
                        break;
                }
                //e.Handled = false;
                editSearchNote.Text += input;
                editSearchNote.SetSelection(editSearchNote.Text.Length);
                return;
            }
        }

        private void note_Adapter_note_ItemClick(object sender, int e)
        {
            try
            {
                if (!checkManinRole)
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;
                }

                var noteUpdate = lstNote[e];
                StartActivity(new Intent(Application.Context, typeof(AddNoteActivity)));
                DataCashing.EditNoteItem = noteUpdate;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("note_Adapter_note_ItemClick at Note");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetFilterNote()
        {
            try
            {
                lstSearchNote = new List<ORM.MerchantDB.Note>();
                if (string.IsNullOrEmpty(searchNote))
                {
                    return;
                }

                if (FocusNoteCategeryID == 0)
                {
                    lstSearchNote = DefaultNote.Where(x => x.Message.ToLower().Contains(searchNote.ToLower())).ToList();
                }
                else
                {
                    lstSearchNote = DefaultNote.Where(x => x.Message.ToLower().Contains(searchNote.ToLower()) && x.SysNoteCategoryID == FocusNoteCategeryID).ToList();
                }
                ListdataNote = new ListNoteData(lstSearchNote);
                Note_Adapter_Note note_Adapter_note = new Note_Adapter_Note(ListdataNote);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerViewNote.SetLayoutManager(gridLayoutItem);
                recyclerViewNote.HasFixedSize = true;
                recyclerViewNote.SetItemViewCacheSize(50);
                recyclerViewNote.SetAdapter(note_Adapter_note);
                note_Adapter_note.ItemClick += note_Adapter_note_ItemClick;
                SetBtnSearchItem();
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("SetFilterItemData at Item");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async void SetDataNote()
        {
            try
            {
                //Show Note ตามหมวดหมู่ที่เลือก
                if (FocusNoteCategeryID == 0)
                {
                    lstNote = DefaultNote;
                }
                else
                {
                    lstNote = DefaultNote.Where(x => x.SysNoteCategoryID == FocusNoteCategeryID).ToList();
                }

                ListdataNote = new ListNoteData(lstNote);
                note_Adapter_note = new Note_Adapter_Note(ListdataNote);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerViewNote.SetLayoutManager(gridLayoutItem);
                recyclerViewNote.HasFixedSize = true;
                recyclerViewNote.SetItemViewCacheSize(50);
                recyclerViewNote.SetAdapter(note_Adapter_note);
                note_Adapter_note.ItemClick += note_Adapter_note_ItemClick;

                if (note_Adapter_note.ItemCount == 0)
                {
                    lnNonote.Visibility = ViewStates.Visible;
                    recyclerViewNote.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNonote.Visibility = ViewStates.Gone;
                    recyclerViewNote.Visibility = ViewStates.Visible;
                }

                if (!checkManinRole)
                {
                    addNote.SetBackgroundResource(Resource.Mipmap.AddMax);
                    addNote.Enabled = false;
                }
                else
                {
                    addNote.SetBackgroundResource(Resource.Mipmap.Add);
                    addNote.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetDataNote at Note");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async Task<List<ORM.MerchantDB.Note>> GetDataNote()
        {
            try
            {
                DefaultNote = new List<ORM.MerchantDB.Note>();
                DefaultNote = await noteManage.GetAllNote(DataCashingAll.MerchantId);
                if (DefaultNote == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    DefaultNote = new List<ORM.MerchantDB.Note>();
                    return DefaultNote;
                }

                //if (sysNoteCateId == 0)
                //{
                //    return DefaultNote;
                //}

                //DefaultNote = DefaultNote.Where(x=>x.SysNoteCategoryID == sysNoteCateId && x.MerchantID == DataCashingAll.MerchantId 
                //                                && x.DataStatus != 'D').OrderBy(x=>x.Message).ToList();
                return DefaultNote;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataNote at Note");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
                return new List<ORM.MerchantDB.Note>();
            }
        }

        private void SetBtnSearchItem()
        {
            if (string.IsNullOrEmpty(searchNote))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
                //btnSearch.Enabled = false;
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                //btnSearch.Enabled = true;
            }
        }

        private void SetClearSearchText()
        {
            searchNote = "";
            editSearchNote.Text = string.Empty;
            SetBtnSearchItem();
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();

                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                SetBtnSearchItem();
                if (DataCashingAll.flagNoteCategoryChange)
                {
                    await GetNoteCategoryData();
                    SetNoteCategoryData();
                    DataCashingAll.flagNoteCategoryChange = false;
                }

                if (DataCashingAll.flagNoteChange)
                {
                    await GetDataNote();
                    if (DefaultNote != null)
                    {
                        lstNote = DefaultNote;
                    }
                    SetDataNote();
                    DataCashingAll.flagNoteChange = false;
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                
                FocusNoteCategory();
                FocusNewNote();
            }
            catch (Exception ex)
            {
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Note");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public void RefreshData()
        {
            OnResume();
        }

        internal static void SetFocusNew(long category,ORM.MerchantDB.Note note)
        {
            try
            {   
                FocusNoteCategeryID = category;
                FocusNote = note;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFocusItem at Item");
            }
        }

        private void FocusNewNote()
        {
            try
            {
                if (FocusNote != null)
                {
                    int index = -1;
                    if (lstNote != null)
                    {
                        if (lstNote.Count == 0)
                        {
                            lstNote.Add(FocusNote);
                            //เพิ่มให้โน้ตมีรายการล่าสุดเข้าไปด้วย
                            DefaultNote.Add(FocusNote);
                            DefaultNote = DefaultNote.OrderBy(x => x.Message).ToList();
                            SetDataNote();                           
                            FocusNote = null;
                            return;
                        }

                        index = lstNote.FindIndex(x=>x.SysNoteID == FocusNote.SysNoteID);
                        if (index != -1)
                        {
                            lstNote.RemoveAt(index);
                        }
                        else
                        {
                            //เพิ่มให้โน้ตมีรายการล่าสุดเข้าไปด้วย แต่ต้องตรวจสอบก่อนว่าเคยมีโน้ตนี้หรือไม่
                            DefaultNote.Add(FocusNote);
                            DefaultNote = DefaultNote.OrderBy(x => x.Message).ToList();
                        }
                        lstNote.Insert(0,FocusNote);
                    }
                    if (lstSearchNote?.Count > 0)
                    {
                        index = lstSearchNote.FindIndex(x=>x.SysNoteID == FocusNote.SysNoteID);
                        if (index != -1)
                        {
                            lstSearchNote.RemoveAt(index);
                        }
                        lstSearchNote.Insert(0,FocusNote);
                    }
                    note_Adapter_note.NotifyDataSetChanged();
                    FocusNote = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FocusNoteCategory at Note");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void FocusNoteCategory()
        {
            try
            {
                if (FocusNoteCategeryID > 0)
                {
                    int index = -1;
                    if (lstNoteCategory != null)
                    {
                        lstNoteCategory = new List<ORM.MerchantDB.NoteCategory>();
                        await GetNoteCategoryData();
                        SetNoteCategoryData();

                        index = lstNoteCategory.FindIndex(x=>x.SysNoteCategoryID == FocusNoteCategeryID);
                        recyclerViewCategory.ScrollToPosition(index);
                        //set เผื่อมีการสร้างโน้ตที่หมวดมหมุ่ที่สร้างใหม่
                        //แสดงลิสต์ของโน้ตให้จรงกับหมวดหมู่ที่มีการเพิ่มล่าสุด                        
                        lstNote = new List<ORM.MerchantDB.Note>();
                        lstNote = DefaultNote.Where(x => x.SysNoteCategoryID == FocusNoteCategeryID && x.MerchantID == DataCashingAll.MerchantId
                                                && x.DataStatus != 'D').OrderBy(x => x.Message).ToList();
                        SetDataNote();
                    }
                    note_adapter_header.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FocusNoteCategory at Note");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private class MyImplementSwipeHelper : MySwipeHelper
        {
            Context context;
            RecyclerView recyclerView;
            int buttonWidth;
            public MyImplementSwipeHelper(Context context, RecyclerView recyclerView, int buttonWidth) : base(context, recyclerView, buttonWidth)
            {
                this.context = context;
                this.recyclerView = recyclerView;
                this.buttonWidth = buttonWidth;
            }

            public override void InstantiateMybutton(RecyclerView.ViewHolder viewHolder, List<MyButton> buffer)
            {
                buffer.Add(new MyButton(context,
                     "Delete",
                     0,
                     Resource.Mipmap.DeleteBt,
                     "#33AAE1",
                     new MyDeleteButtonClick(this)));
            }

            private class MyDeleteButtonClick : MyButtonClickListener
            {
                private MyImplementSwipeHelper myImplementSwipeHelper;

                public MyDeleteButtonClick(MyImplementSwipeHelper myImplementSwipeHelper)
                {
                    this.myImplementSwipeHelper = myImplementSwipeHelper;
                }

                public void OnClick(int position)
                {
                    DataCashing.EditNoteItem = lstNote[position];
                    try
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("deleteType", "note");
                        bundle.PutString("fromPage", "main");
                        dialog.Arguments = bundle;
                        dialog.Show(noteActivity.SupportFragmentManager, myMessage);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("MyImplementSwipeHelper at Note");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }

        private async Task GetOnlineDataNote()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                ORM.MerchantDB.SystemRevisionNo revisionNo = new ORM.MerchantDB.SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 70).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region Note                            
                    try
                    {
                        //Get NoteCategory API
                        var allNote = await GabanaAPI.GetDataNotes((int)revisionNo.LastRevisionNo, 0);

                        if (allNote == null)
                        {
                            return;
                        }

                        if (allNote.totalNotes == 0)
                        {
                            return;
                        }

                        int round = 0, addrount = 0;
                        round = allNote.totalNotes / 100;
                        addrount = round + 1;
                        for (int j = 0; j < addrount; j++)
                        {
                            allNote = await GabanaAPI.GetDataNotes((int)revisionNo.LastRevisionNo, j);

                            if (allNote == null)
                            {
                                break;
                            }

                            if (allNote.totalNotes == 0)
                            {
                                break;
                            }

                            allNote.noteWithNoteStatuses.ToList().OrderBy(x => x.note.RevisionNo);
                            var maxNote = allNote.noteWithNoteStatuses.ToList().Max(x => x.note.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();                                                             

                            //check ว่ามีไหม
                            List<Gabana3.JAM.Notes.NoteWithNoteStatus> UpdateNote = new List<Gabana3.JAM.Notes.NoteWithNoteStatus>();
                            List<Gabana3.JAM.Notes.NoteWithNoteStatus> InsertNote = new List<Gabana3.JAM.Notes.NoteWithNoteStatus>();
                            List<ORM.MerchantDB.Note> GetallNote = await noteManage.GetAllNote(DataCashingAll.MerchantId);
                            UpdateNote.AddRange(allNote.noteWithNoteStatuses.Where(x => GetallNote.Select(y => (long)y.SysNoteID).ToList().Contains(x.note.SysNoteID)).ToList());
                            InsertNote.AddRange(allNote.noteWithNoteStatuses.Where(x => !(GetallNote.Select(y => (long)y.SysNoteID).ToList().Contains(x.note.SysNoteID)) && x.DataStatus != 'D').ToList()); ;

                            //Insert Note
                            if (InsertNote.Count > 0)
                            {
                                List<ORM.MerchantDB.Note> BulkNote = new List<ORM.MerchantDB.Note>();
                                foreach (var Note in InsertNote)
                                {
                                    BulkNote.Add(new ORM.MerchantDB.Note()
                                    {
                                        MerchantID = Note.note.MerchantID,
                                        SysNoteID = Note.note.SysNoteID,
                                        Ordinary = Note.note.Ordinary,
                                        Message = Note.note.Message,
                                        SysNoteCategoryID = Note.note.SysNoteCategoryID,
                                        LastDateModified = Note.note.LastDateModified,
                                        UserLastModified = Note.note.UserLastModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow
                                    });
                                    maxNoteCategoryRevision = Note.note.RevisionNo;
                                }

                                using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                {
                                    try
                                    {
                                        await db.BulkCopyAsync(BulkNote);
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorRevison = InsertNote.Select(x => x.note.RevisionNo).Min();
                                        maxNoteRevision = errorRevison;
                                        Log.Error("connecterror", "BulkNote :" + ex.Message);
                                        throw ex;
                                    }
                                }
                            }

                            //Update Note
                            if (UpdateNote.Count > 0)
                            {
                                foreach (var Note in UpdateNote)
                                {
                                    if (Note.DataStatus == 'D')
                                    {
                                        //delete
                                        var delete = await noteManage.DeleteNote(Note.note.MerchantID, Note.note.SysNoteID);
                                        if (!delete)
                                        {
                                            var data = await noteManage.GetNote(Note.note.MerchantID, Note.note.SysNoteID);
                                            if (data != null)
                                            {
                                                data.DataStatus = 'D';
                                                data.FWaitSending = 0;
                                                await noteManage.UpdateNote(data);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        note = new ORM.MerchantDB.Note()
                                        {
                                            MerchantID = Note.note.MerchantID,
                                            SysNoteID = Note.note.SysNoteID,
                                            Ordinary = Note.note.Ordinary,
                                            Message = Note.note.Message,
                                            SysNoteCategoryID = Note.note.SysNoteCategoryID,
                                            LastDateModified = Note.note.LastDateModified,
                                            UserLastModified = Note.note.UserLastModified,
                                            DataStatus = 'I',
                                            FWaitSending = 0,
                                            WaitSendingTime = DateTime.UtcNow
                                        };
                                        var insertOrreplace = await noteManage.InsertOrReplaceNote(note);
                                    }
                                    maxNoteRevision = Note.note.RevisionNo;
                                }
                            }

                            await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxNote);
                        }

                        Log.Debug("connectpass", "listRivisionNote");
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionNote : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxNoteRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task GetOnlineDataNoteCategory()
        {
            try
            {
                listRivision = await systemRevisionNoManage.GetAllSystemRevisionNo();
                ORM.MerchantDB.SystemRevisionNo revisionNo = new ORM.MerchantDB.SystemRevisionNo();
                revisionNo = listRivision.Where(x => x.SystemID == 60).FirstOrDefault();
                if (revisionNo != null)
                {
                    #region NoteCategory
                    try
                    {
                        var allNoteCategory = await GabanaAPI.GetDataNoteCategory((int)revisionNo.LastRevisionNo);

                        if (allNoteCategory == null)
                        {
                            return;
                        }

                        if (allNoteCategory.NoteCategory.Count == 0 && allNoteCategory.NoteCategoryBin.Count == 0)
                        {
                            return;
                        }

                        int maxNoteCategory = 0;
                        if (allNoteCategory.NoteCategory.Count > 0)
                        {
                            allNoteCategory.NoteCategory.ToList().OrderBy(x => x.RevisionNo);
                            maxNoteCategory = allNoteCategory.NoteCategory.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  

                            //check ว่ามีไหม
                            List<ORM.Master.NoteCategory> UpdateNoteCategory = new List<ORM.Master.NoteCategory>();
                            List<ORM.Master.NoteCategory> InsertNoteCategory = new List<ORM.Master.NoteCategory>();
                            List<ORM.MerchantDB.NoteCategory> GetallNoteCategory = await noteCategoryManage.GetAllNoteCategory();
                            UpdateNoteCategory.AddRange(allNoteCategory.NoteCategory.Where(x => GetallNoteCategory.Select(y => (long)y.SysNoteCategoryID).ToList().Contains(x.SysNoteCategoryID)).ToList());
                            InsertNoteCategory.AddRange(allNoteCategory.NoteCategory.Where(x => !(GetallNoteCategory.Select(y => (long)y.SysNoteCategoryID).ToList().Contains(x.SysNoteCategoryID))).ToList());

                            //Insert NoteCategory
                            if (InsertNoteCategory.Count > 0)
                            {
                                List<ORM.MerchantDB.NoteCategory> BulkNoteCategory = new List<ORM.MerchantDB.NoteCategory>();

                                foreach (var NoteCategory in InsertNoteCategory)
                                {
                                    BulkNoteCategory.Add(new ORM.MerchantDB.NoteCategory()
                                    {
                                        MerchantID = NoteCategory.MerchantID,
                                        SysNoteCategoryID = NoteCategory.SysNoteCategoryID,
                                        Ordinary = NoteCategory.Ordinary,
                                        Name = NoteCategory.Name,
                                        DateCreated = NoteCategory.DateCreated,
                                        DateModified = NoteCategory.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow
                                    });
                                    maxNoteCategoryRevision = NoteCategory.RevisionNo;
                                }

                                using (MerchantDB db = new MerchantDB(DataCashingAll.Pathdb))
                                {
                                    try
                                    {
                                        await db.BulkCopyAsync(BulkNoteCategory);
                                    }
                                    catch (Exception ex)
                                    {
                                        var errorRevison = InsertNoteCategory.Select(x => x.RevisionNo).Min();
                                        maxNoteCategoryRevision = errorRevison;
                                        Log.Error("connecterror", "BulkNoteCategory :" + ex.Message);
                                        throw ex;
                                    }
                                }
                            }

                            //Update NoteCategory
                            if (UpdateNoteCategory.Count > 0)
                            {
                                foreach (var item in UpdateNoteCategory)
                                {
                                    noteCategory = new ORM.MerchantDB.NoteCategory()
                                    {
                                        MerchantID = item.MerchantID,
                                        SysNoteCategoryID = item.SysNoteCategoryID,
                                        Ordinary = item.Ordinary,
                                        Name = item.Name,
                                        DateCreated = item.DateCreated,
                                        DateModified = item.DateModified,
                                        DataStatus = 'I',
                                        FWaitSending = 0,
                                        WaitSendingTime = DateTime.UtcNow
                                    };
                                    var insertOrreplace = await noteCategoryManage.InsertOrReplaceCategory(noteCategory);

                                    maxNoteCategoryRevision = item.RevisionNo;
                                }
                            }
                        }

                        if (allNoteCategory.NoteCategoryBin.Count > 0)
                        {
                            allNoteCategory.NoteCategoryBin.ToList().OrderBy(x => x.RevisionNo);
                            maxNoteCategory = allNoteCategory.NoteCategoryBin.ToList().Max(x => x.RevisionNo);// OrderByDescending(x => x.RevisionNo).First();  
                                                                                                              //delete
                            foreach (var itemDelete in allNoteCategory.NoteCategoryBin)
                            {
                                //UpdateItem
                                var UpdateNote = await noteManage.GetNoteBYCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                if (UpdateNote != null)
                                {
                                    foreach (var update in UpdateNote)
                                    {
                                        update.SysNoteCategoryID = null;
                                        var resultUpdate = await noteManage.UpdateNote(update);
                                    }
                                }
                                var delete = await noteCategoryManage.DeleteNoteCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                if (!delete)
                                {
                                    var data = await noteCategoryManage.GetNoteCategory(itemDelete.MerchantID, itemDelete.SysNoteCategoryID);
                                    if (data != null)
                                    {
                                        data.DataStatus = 'D';
                                        data.FWaitSending = 0;
                                        await noteCategoryManage.UpdateNoteCategory(data);
                                    }
                                }
                                maxNoteCategoryRevision = itemDelete.RevisionNo;
                            }
                        }

                        Log.Debug("connectpass", "listRivisionNoteCategory");
                        await UtilsAll.updateRevisionNo((int)revisionNo.SystemID, maxNoteCategory);
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("connecterror", "listRivisionNoteCategory : " + ex.Message);
                        await UtilsAll.ErrorRevisionNo((int)revisionNo.SystemID, maxNoteCategoryRevision);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteCategoryChange");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'NoteActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'NoteActivity.openPage' is assigned but its value is never used
        public DateTime pauseDate = DateTime.Now;
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

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }
        }
    }
}