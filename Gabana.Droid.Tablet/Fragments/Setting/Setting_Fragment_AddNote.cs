using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_AddNote : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static Setting_Fragment_AddNote NewInstance()
        {
            Setting_Fragment_AddNote frag = new Setting_Fragment_AddNote();
            return frag;
        }
        View view;
        public static Setting_Fragment_AddNote fragment_addnote;
        string UserLogin;
        public static Note Editnote = new Note();
        bool flagdatachange = false;
        string OldNameNote;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_addnote, container, false);
            try
            {
                fragment_addnote = this;
                CheckJwt();
                CombineUI();
                SetUIEvent();
                SetButtonAdd(false);
                UserLogin = Preferences.Get("User", "");
                return view;
            }
            catch (Exception)
            {
                return view;
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

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            framNoteCate.Click += FramNoteCate_Click;
            textNote.TextChanged += TextNote_TextChanged;
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
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
                var adapterCategory = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, category_array);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnNoteCategory.Adapter = adapterCategory;

                long? category = Editnote.SysNoteCategoryID;

                if (category != null)
                {
                    var data = getallCategory.Where(x => x.SysNoteCategoryID == category).FirstOrDefault();
                    if (data != null)
                    {
                        int position = adapterCategory.GetPosition(data.Name);
                        spnNoteCategory.SetSelection(position);
                    }
                    else
                    {
                        int position = adapterCategory.GetPosition("None");
                        spnNoteCategory.SetSelection(position);
                    }
                }
                else
                {
                    int position = adapterCategory.GetPosition("None");
                    spnNoteCategory.SetSelection(position);
                }

                textNote.Text = Editnote.Message;
                OldNameNote = Editnote.Message;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("ShowDataEdit at Add Note");
                return;
            }
        }

        List<string> SysNoteCategoryID;
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

                spnNoteCategory.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spnNoteCategory_ItemSelected);
                var adapterCategory = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnNoteCategory.Adapter = adapterCategory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SpinnerCategory at Add Note");
            }
        }

        private void spnNoteCategory_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (SysNoteCategoryID == null)
            {
                return;
            }
            SysNoteCateID = Convert.ToInt32(SysNoteCategoryID[e.Position].ToString());
            CheckDataChange();
        }

        LinearLayout lnBack;
        Spinner spnNoteCategory;
        FrameLayout framNoteCate;
        public EditText textNote;
        FrameLayout btnDelete;
        public Button btnAdd;
        TextView textTitle;

        private void CombineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            spnNoteCategory = view.FindViewById<Spinner>(Resource.Id.spnNoteCategory);
            framNoteCate = view.FindViewById<FrameLayout>(Resource.Id.framNoteCate);
            textNote = view.FindViewById<EditText>(Resource.Id.textNote);
            btnDelete = view.FindViewById<FrameLayout>(Resource.Id.btnDelete);
            btnAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
            textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var fragment = new Setting_Dialog_DeleteNote();
                fragment.Show(Activity.SupportFragmentManager, nameof(Setting_Dialog_DeleteNote));
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at Add Note");
                return;
            }
        }

        private void TextNote_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            NoteName = textNote.Text.Trim();
            CheckDataChange();
        }

        private void SetButtonAdd(bool enable)
        {

            if (enable)
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAdd.Enabled = enable;
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
                var adapterCategory = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, category_array);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnNoteCategory.Adapter = adapterCategory;

                NoteCategory category = getallCategory.Where(i => i.SysNoteCategoryID == sysCategoryID).FirstOrDefault();
                if (category != null)
                {
                    long? categoryID = category.SysNoteCategoryID;

                    foreach (var i in getallCategory)
                    {
                        if (categoryID == i.SysNoteCategoryID)
                        {
                            int position = adapterCategory.GetPosition(i.Name);
                            spnNoteCategory.SetSelection(position);
                            SysNoteCateID = (int)i.SysNoteCategoryID;
                        }
                    }
                }
                else
                {

                    int position = adapterCategory.GetPosition("None");
                    spnNoteCategory.SetSelection(position);
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SpinnerSelectNoteCategory at Add Note");
            }
        }

        private void CheckDataChange()
        {
            if (DataCashing.EditNote == null)
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
                Editnote = DataCashing.EditNote;
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

        private void FramNoteCate_Click(object sender, EventArgs e)
        {
            spnNoteCategory.PerformClick();
        }

        private async void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "note");
            MainActivity.main_activity.CloseKeyboard(view);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            bool CheckDup = false;
            CheckDup = CheckDuplicateData(textNote.Text);
            if (!CheckDup)
            {
                btnAdd.Enabled = true;
                var fragmenta = new Setting_Dialog_DublicateNote();
                fragmenta.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Setting_Dialog_DublicateNote));
                return;
            }
            ManageNote();
            btnAdd.Enabled = true;            
        }

        private bool CheckDuplicateData(string NoteName)
        {
            try
            {
                if (string.IsNullOrEmpty(NoteName))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                if (DataCashing.EditNote != null)
                {
                    if (NoteName == OldNameNote)
                    {
                        return true;
                    }
                }                    

                var checkname = lstnote.FindIndex(x => x.Message.ToLower().Equals(NoteName.ToLower()));
                if (checkname != -1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async void ManageNote()
        {
            bool check = false;
            if (DataCashing.EditNote == null)
            {
                check = await InsertNote();
                if (!check) return;
            }
            else
            {
                check = await UpdateNote();
                if (!check) return;
            }
            SetClearData();
        }

        public async void SetClearData()
        {
            try
            {
                UINewNote();
                DataCashing.EditNote = null;
                Editnote = null;
                flagdatachange = false;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "note");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetClearData at Add Gift");
            }
        }
                

        NoteManage noteManage = new NoteManage();
        public async Task<bool> UpdateNote()
        {
            try
            {
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
                              
                var result = await noteManage.UpdateNote(Editnote);
                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                //JobQueue
                if (DataCashing.CheckNet)
                {
                    JobQueue.Default.AddJobSendNote((int)Editnote.MerchantID, (int)Editnote.SysNoteID);
                }
                else
                {
                    Editnote.FWaitSending = 2;
                    await noteManage.UpdateNote(Editnote);
                }
                DataCashing.EditNote = Editnote;
                long fogusCate = 0;
                if (Editnote.SysNoteCategoryID == null)
                {
                    fogusCate = 0;
                }
                else
                {
                    fogusCate = (long)Editnote.SysNoteCategoryID;
                }
                DataCashingAll.flagNoteChange = true;
                Setting_Fragment_Note.SetFocusItem(fogusCate, Editnote);
                MainActivity.main_activity.CloseKeyboard(view);
                return true;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateNote at Add Note");
                return false;
            }
        }

        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        string sys;
        public static Note Insertnote = new Note();
        string NoteName;
        int systemID = 70;
        int SysNoteCateID, sysCategoryID;
        public async Task<bool> InsertNote()
        {
            try
            {
                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, systemID);
                sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                Insertnote.MerchantID = DataCashingAll.MerchantId;
                Insertnote.SysNoteID = Convert.ToInt64(sys);
                Insertnote.Ordinary = null;

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

                var result = await noteManage.InsertNote(Insertnote);
                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                DataCashing.EditNote = Insertnote;

                //JobQueue
                if (DataCashing.CheckNet)
                {
                    JobQueue.Default.AddJobSendNote((int)Insertnote.MerchantID, (int)Insertnote.SysNoteID);
                }
                else
                {
                    Insertnote.FWaitSending = 2;
                    await noteManage.UpdateNote(Insertnote);
                }

                long fogusCate = 0;
                if (Insertnote.SysNoteCategoryID == null)
                {
                    fogusCate = 0;
                }
                else
                {
                    fogusCate = (long)Insertnote.SysNoteCategoryID;
                }
                DataCashingAll.flagNoteChange = true;
                Setting_Fragment_Note.SetFocusItem(fogusCate, Insertnote);                
                MainActivity.main_activity.CloseKeyboard(view);
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertNote at Add Note");
                return false;
            }
        }

        public override async void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                UINewNote();
                await SetDetailNote();
                flagdatachange = false;
                SetButtonAdd(false);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task  SetDetailNote()
        {
            try
            {
                if (DataCashing.EditNote == null)
                {
                    textTitle.Text = GetString(Resource.String.addnote_activity_title);
                    btnAdd.Text = GetString(Resource.String.addnote_activity_title);
                    btnDelete.Visibility = ViewStates.Gone;                   
                    sysCategoryID = (int)Setting_Fragment_Note.FocusNoteCategeryID;
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
                    btnDelete.Visibility = ViewStates.Visible;
                    SpinnerCategory();
                    Editnote = DataCashing.EditNote;
                    ShowDataEdit();                    
                }

                await GetListNote();
                await GetListNoteCategory();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public  List<NoteCategory> lstnotecategory;
        private async Task GetListNoteCategory()
        {
            try
            {
                lstnotecategory = new List<NoteCategory>();
                lstnotecategory = Setting_Fragment_Note.lstCategory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("GetListNoteCategory at addnote");
            }
        }

        private List<Note> lstnote;
        private async Task GetListNote()
        {
            try
            {
                lstnote = new List<Note>();
                lstnote = Setting_Fragment_Note.fragment_main.DefaultNote;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("GetListNote at addnote");
            }
        }

        private void UINewNote()
        {
            spnNoteCategory.SetSelection(0);
            textNote.Text = string.Empty;
            textTitle.Text = string.Empty;
        }
    }
}