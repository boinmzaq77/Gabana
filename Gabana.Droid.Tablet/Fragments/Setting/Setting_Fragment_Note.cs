using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AutoMapper.Configuration.Conventions;
using Gabana.Droid.Helper;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Droid.Tablet.Helper;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
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

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Note : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_Note NewInstance()
        {
            Setting_Fragment_Note frag = new Setting_Fragment_Note();
            return frag;
        }
        View view;
        public static Setting_Fragment_Note fragment_main;
        string LoginType, UserLogin;
        public static ORM.MerchantDB.NoteCategory EditNoteCategory;
        public string SelectNoteCategory;
        public List<ORM.MerchantDB.Note> DefaultNote;
        NoteManage noteManage = new NoteManage();
        public static long FocusNoteCategeryID = 0;
        public static ORM.MerchantDB.Note FocusNote;
        internal ListNoteCategory listNoteCate;
        ListNoteData ListdataNote;
        public static bool checkManinRole;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_note, container, false);
            try
            {
                fragment_main = this;
                CheckJwt();
                ComBineUI();
                SetUIEvent();
                LoginType = Preferences.Get("LoginType", "");
                UserLogin = Preferences.Get("User", "");
                var Width = 130;
                MySwipeHelper mySwipe = new MyImplementSwipeHelper(this.Activity, rcvNote, (int)Width);
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Note");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        private void SetUIEvent()
        {
            editSearchNote.TextChanged += EditSearchNote_TextChanged;
            editSearchNote.KeyPress += EditSearchNote_KeyPress;
            editSearchNote.FocusChange += EditSearchNote_FocusChange;
            lnBack.Click += LnBack_Click;
            addNote.Click += AddNote_Click;
            btnAddCategoryNote.Click += LnAddCategoryNote_Click;
            lnAddCategoryNote.Click += LnAddCategoryNote_Click;
            btnSearch.Click += BtnSearch_Click;
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
            }
            View view = this.Activity.CurrentFocus;
            if (view != null)
            {
                MainActivity.main_activity.CloseKeyboard(view);
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

        private void SetFilterNote()
        {
            try
            {
                lstSearchNote = new List<ORM.MerchantDB.Note>();
                if (FocusNoteCategeryID == 0)
                {
                    lstSearchNote = DefaultNote.Where(x => x.Message.ToLower().Contains(searchNote.ToLower())).ToList();
                }
                else
                {
                    lstSearchNote = DefaultNote.Where(x => x.Message.ToLower().Contains(searchNote.ToLower()) && x.SysNoteCategoryID == FocusNoteCategeryID).ToList();
                }

                ListdataNote = new ListNoteData(lstSearchNote);
                setting_adapter_note = new Setting_Adapter_Note(ListdataNote);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvNote.SetLayoutManager(gridLayoutItem);
                rcvNote.HasFixedSize = true;
                rcvNote.SetItemViewCacheSize(50);
                rcvNote.SetAdapter(setting_adapter_note);
                setting_adapter_note.ItemClick += Setting_Adapter_Note_ItemClick;

                if (setting_adapter_note.ItemCount == 0)
                {
                    if (!string.IsNullOrEmpty(searchNote))
                    {
                        //NodataSearch ไม่มี
                        lnNonote.Visibility = ViewStates.Visible;
                        rcvNote.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnNonote.Visibility = ViewStates.Gone;
                        rcvNote.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    lnNonote.Visibility = ViewStates.Gone;
                    rcvNote.Visibility = ViewStates.Visible;
                }
                SetBtnSearchItem();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetFilterItemData at Item");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SetClearSearchText();
            editSearchNote.ClearFocus();
            OnResume();
        }

        private void SetClearSearchText()
        {
            searchNote = "";
            editSearchNote.Text = string.Empty;
            SetBtnSearchItem();
        }

        ListNoteData lstShowNote;
        public static List<ORM.MerchantDB.Note> lstNote, lstSearchNote;

        public void RefreshData()
        {
            fragment_main.OnResume();
        }

        async Task SetDataNote()
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
                setting_adapter_note = new Setting_Adapter_Note(ListdataNote);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvNote.SetLayoutManager(gridLayoutItem);
                rcvNote.HasFixedSize = true;
                rcvNote.SetItemViewCacheSize(50);
                rcvNote.SetAdapter(setting_adapter_note);
                setting_adapter_note.ItemClick += Setting_Adapter_Note_ItemClick;

                if (setting_adapter_note.ItemCount == 0)
                {
                    lnNonote.Visibility = ViewStates.Visible;
                    rcvNote.Visibility = ViewStates.Gone;
                }
                else
                {
                    lnNonote.Visibility = ViewStates.Gone;
                    rcvNote.Visibility = ViewStates.Visible;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task<List<ORM.MerchantDB.Note>> GetDataNote()
        {
            try
            {
                DefaultNote = new List<ORM.MerchantDB.Note>();
                DefaultNote = await noteManage.GetAllNote(DataCashingAll.MerchantId);
                if (DefaultNote == null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    DefaultNote = new List<ORM.MerchantDB.Note>();
                    return DefaultNote;
                }

                return DefaultNote;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataNote at Note");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
                return new List<ORM.MerchantDB.Note>();
            }
        }

        public static void SetFocusItem(long category, ORM.MerchantDB.Note note)
        {
            FocusNoteCategeryID = category;
            FocusNote = note;
        }


        private async void Setting_Adapter_Note_ItemClick(object sender, int e)
        {
            try
            {
                var noteUpdate = lstNote[e];
                DataCashing.EditNote = noteUpdate;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addnote");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Item_Adapter_Item_ItemClick at Note");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                await ShowDetail();
                FocusNoteCategory();
                FocusNewNote();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Note");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task ShowDetail()
        {
            SetBtnSearchItem();
            await GetNoteCategoryData();
            SetNoteCategoryData();

            await GetDataNote();
            if (DefaultNote != null)
            {
                lstNote = DefaultNote;
            }
            await SetDataNote();
        }

        private async void FocusNewNote()
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
                            await SetDataNote();
                            FocusNote = null;
                            return;
                        }

                        index = lstNote.FindIndex(x => x.SysNoteID == FocusNote.SysNoteID);
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
                        lstNote.Insert(0, FocusNote);
                    }
                    if (lstSearchNote?.Count > 0)
                    {
                        index = lstSearchNote.FindIndex(x => x.SysNoteID == FocusNote.SysNoteID);
                        if (index != -1)
                        {
                            lstSearchNote.RemoveAt(index);
                        }
                        lstSearchNote.Insert(0, FocusNote);
                    }
                    setting_adapter_note.NotifyDataSetChanged();
                    FocusNote = null;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FocusNoteCategory at Note");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void FocusNoteCategory()
        {
            try
            {
                if (FocusNoteCategeryID > 0)
                {
                    int index = -1;
                    if (lstCategory != null)
                    {
                        lstCategory = new List<ORM.MerchantDB.NoteCategory>();
                        await GetNoteCategoryData();
                        SetNoteCategoryData();

                        index = lstCategory.FindIndex(x => x.SysNoteCategoryID == FocusNoteCategeryID);
                        rcvCategoryNote.ScrollToPosition(index);
                        //set เผื่อมีการสร้างโน้ตที่หมวดมหมุ่ที่สร้างใหม่
                        //แสดงลิสต์ของโน้ตให้จรงกับหมวดหมู่ที่มีการเพิ่มล่าสุด                        
                        lstNote = new List<ORM.MerchantDB.Note>();
                        lstNote = DefaultNote.Where(x => x.SysNoteCategoryID == FocusNoteCategeryID && x.MerchantID == DataCashingAll.MerchantId
                                                && x.DataStatus != 'D').OrderBy(x => x.Message).ToList();
                        await SetDataNote();
                    }
                    setting_adapter_category.NotifyDataSetChanged();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FocusNoteCategory at Note");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetNoteCategoryData()
        {
            try
            {
                LinearLayoutManager layoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
                rcvCategoryNote.HasFixedSize = true;
                rcvCategoryNote.SetLayoutManager(layoutManager);
                setting_adapter_category = new Setting_Adapter_NoteCategory(listNoteCate);
                rcvCategoryNote.SetAdapter(setting_adapter_category);
                setting_adapter_category.ItemClick += Setting_adapter_category_ItemClick;

                ListViewCategoryHolder categoryHolder;
                if (FocusNoteCategeryID > 0)
                {
                    int index = lstCategory.FindIndex(x => x.SysNoteCategoryID == FocusNoteCategeryID);
                    if (index == -1)
                    {
                        //return;
                        //เปลี่ยนเป็นถ้าไปหาไม่เจอให้แสดงทั้งหมดแทน
                        FocusNoteCategeryID = 0;
                        index = 0;
                    }
                    categoryHolder = rcvCategoryNote.FindViewHolderForAdapterPosition(index) as ListViewCategoryHolder;
                }
                else
                {
                    categoryHolder = rcvCategoryNote.FindViewHolderForAdapterPosition(0) as ListViewCategoryHolder;
                }
                if (categoryHolder != null)
                {
                    categoryHolder.Select();
                    Setting_Adapter_NoteCategory.vhselect = categoryHolder;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetCategoryMenuShow at Note");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
            }
        }

        private async void Setting_adapter_category_ItemClick(object sender, int e)
        {
            try
            {
                if (e == -1)
                {
                    return;
                }

                Setting_Adapter_NoteCategory.vhselect.NotSelect();
                if (e == 0)
                {
                    FocusNoteCategeryID = 0;
                }
                else
                {
                    var category = lstCategory.Where(x => x.SysNoteCategoryID == (int)listNoteCate[e].SysNoteCategoryID).FirstOrDefault();
                    if (category != null)
                    {
                        FocusNoteCategeryID = category.SysNoteCategoryID;
                    }
                    else
                    {
                        FocusNoteCategeryID = 0;
                    }
                }
                var viewCategoryHolder = rcvCategoryNote.FindViewHolderForAdapterPosition(e) as ListViewCategoryHolder;
                if (viewCategoryHolder != null)
                {
                    viewCategoryHolder.Select();
                    Setting_Adapter_NoteCategory.vhselect = viewCategoryHolder;
                }
                await SetDataNote();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
            }
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

                lstCategory = await category.GetAllNoteCategory();
                if (lstCategory != null)
                {
                    firstCategory.AddRange(lstCategory);
                }

                listNoteCate = new ListNoteCategory(firstCategory);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                Log.Debug("error", ex.Message);
            }
        }

        public static List<ORM.MerchantDB.NoteCategory> lstCategory = new List<ORM.MerchantDB.NoteCategory>();
        string searchNote;
        private void SetBtnSearchItem()
        {
            if (string.IsNullOrEmpty(searchNote))
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.Search);
                btnSearch.Enabled = false;
            }
            else
            {
                btnSearch.SetBackgroundResource(Resource.Mipmap.DelTxt);
                btnSearch.Enabled = true;
            }
        }

        Setting_Adapter_NoteCategory setting_adapter_category;
        Setting_Adapter_Note setting_adapter_note;

        public static async void NoteCategoryLongClick(ORM.MerchantDB.NoteCategory n)
        {
            try
            {
                EditNoteCategory = n;
                if (EditNoteCategory == null)
                {
                    return;
                }

                if (EditNoteCategory?.Name != "All")
                {
                    var fragment = new Setting_Dialog_AddNote();
                    fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Setting_Dialog_AddNote));
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("NoteCategoryLongClick at Note");
                Log.Debug("error", ex.Message);
            }
        }

        private async void EditSearchNote_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            searchNote = editSearchNote.Text.Trim();
            if (string.IsNullOrEmpty(searchNote))
            {
                await SetDataNote();
            }
            SetBtnSearchItem();
        }

        LinearLayout lnAddCategoryNote;
        RecyclerView rcvCategoryNote, rcvNote;
        ImageButton btnSearch;
        EditText editSearchNote;
        SwipeRefreshLayout refreshlayout;
        LinearLayout lnNonote;
        Button addNote;
        LinearLayout lnBack;
        ImageButton btnAddCategoryNote;

        private void ComBineUI()
        {
            lnAddCategoryNote = view.FindViewById<LinearLayout>(Resource.Id.lnAddCategoryNote);
            rcvCategoryNote = view.FindViewById<RecyclerView>(Resource.Id.rcvCategoryNote);
            btnSearch = view.FindViewById<ImageButton>(Resource.Id.btnSearch);
            editSearchNote = view.FindViewById<EditText>(Resource.Id.editSearchNote);
            refreshlayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refreshlayout);
            rcvNote = view.FindViewById<RecyclerView>(Resource.Id.rcvNote);
            lnNonote = view.FindViewById<LinearLayout>(Resource.Id.lnNonote);
            addNote = view.FindViewById<Button>(Resource.Id.addNote);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            btnAddCategoryNote = view.FindViewById<ImageButton>(Resource.Id.btnAddCategoryNote);


            refreshlayout.Refresh += async (sender, e) =>
            {
                DataCashingAll.flagNoteCategoryChange = true;
                DataCashingAll.flagNoteChange = true;
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
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
            EditNoteCategory = null;
            var fragment = new Setting_Dialog_AddNote();
            fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Setting_Dialog_AddNote));
        }

        private async void AddNote_Click(object sender, EventArgs e)
        {
            DataCashing.EditNote = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "addnote");
        }

        private async void LnBack_Click(object sender, EventArgs e)
        {
            DataCashing.EditNote = null;
            FocusNoteCategeryID = 0;
            FocusNote = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
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

        public void DeleteNote(ORM.MerchantDB.Note _DeleteNote)
        {
            try
            {
                int index = 0;
                index = lstNote.FindIndex(x => x.SysNoteID == _DeleteNote.SysNoteID);
                if (index == -1)
                {
                    return;
                }

                lstNote.RemoveAt(index);
                setting_adapter_note.NotifyItemRemoved(index);
                OnResume();

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteCustomer at Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        List<ORM.MerchantDB.SystemRevisionNo> listRivision = new List<ORM.MerchantDB.SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage = new SystemRevisionNoManage();
        int maxNoteRevision = 0;
        int maxNoteCategoryRevision = 0;
        ORM.MerchantDB.Note note = new ORM.MerchantDB.Note();
        NoteCategoryManage noteCategoryManage = new NoteCategoryManage();
        ORM.MerchantDB.NoteCategory noteCategory = new ORM.MerchantDB.NoteCategory();

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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                    Resource.Mipmap.DeleteBt2,
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
                    try
                    {
                        DataCashing.EditNote = lstNote[position];
                        var fragment = new Setting_Dialog_DeleteNote();
                        fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Setting_Dialog_DeleteNote));
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("MyImplementSwipeHelper at giftvoucher");
                        Toast.MakeText(myImplementSwipeHelper.context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
            }
        }

    }
    public class ListNoteCategory
    {
        public List<ORM.MerchantDB.NoteCategory> notecategory;
        static List<ORM.MerchantDB.NoteCategory> builitem;
        public ListNoteCategory(List<ORM.MerchantDB.NoteCategory> notecategory)
        {
            builitem = notecategory;
            this.notecategory = builitem;
        }
        public int Count
        {
            get
            {
                return notecategory == null ? 0 : notecategory.Count;
            }
        }
        public ORM.MerchantDB.NoteCategory this[int i]
        {
            get { return notecategory == null ? null : notecategory[i]; }
        }


    }
}