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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Setting_Dialog_AddNote : AndroidX.Fragment.App.DialogFragment
    {
        Button BtnAddCategory;
        ImageButton btnBack;
        public EditText textNameCategory;
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
        public static Setting_Dialog_AddNote addNotecate;
        string OldNameNoteCategory;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Setting_Dialog_AddNote NewInstance()
        {
            var frag = new Setting_Dialog_AddNote { Arguments = new Bundle() };
            return frag;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_dialog_addcate, container, false);
            try
            {
                addNotecate = this;
                CheckJwt();
                CombineUI();
                SetEventUI();              

                NoteCategoryDetail = Setting_Fragment_Note.EditNoteCategory;
                if (NoteCategoryDetail == null)
                {
                    lnBtnAddNoteCategory.Visibility = ViewStates.Visible;
                    lnEdit.Visibility = ViewStates.Gone;                    
                }
                else
                {
                    lnBtnAddNoteCategory.Visibility = ViewStates.Gone;
                    lnEdit.Visibility = ViewStates.Visible;
                    ShowData();                    
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
            return view;
        }

        private void SetEventUI()
        {
            btnBack.Click += BtnBack_Click;
            textNameCategory.TextChanged += TextNameCategory_TextChanged;
            frameClickDelete.Click += FrameClickDelete_Click;
            BtnAddCategory.Click += BtnAddCategory_Click;
            btnSave.Click += BtnAddCategory_Click;
        }

        private void CombineUI()
        {
            lnEdit = view.FindViewById<LinearLayout>(Resource.Id.lnEdit);
            lnBtnAddNoteCategory = view.FindViewById<LinearLayout>(Resource.Id.lnBtnAddNoteCategory);
            frameClickDelete = view.FindViewById<FrameLayout>(Resource.Id.frameClickDelete);
            btnBack = view.FindViewById<ImageButton>(Resource.Id.btnBack);
            BtnAddCategory = view.FindViewById<Button>(Resource.Id.BtnAddCategory);
            textNameCategory = view.FindViewById<EditText>(Resource.Id.textNameCategory);
            txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
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

        public void ShowData()
        {
            try
            {
                txtTitle.Text = GetString(Resource.String.editnotecategory_activity_title);
                textNameCategory.Text = NoteCategoryDetail.Name;
                textNameCategory.Focusable = true;
                textNameCategory.SetSelection(textNameCategory.Text.Length);
                OldNameNoteCategory = textNameCategory.Text;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnAddCategory_Click(object sender, EventArgs e)
        {
            try
            {
                BtnAddCategory.Enabled = false;
                bool CheckDup = false;
                CheckDup = CheckDuplicateData(textNameCategory.Text);
                if (!CheckDup)
                {
                    BtnAddCategory.Enabled = true;
                    var fragmenta = new Setting_Dialog_DublicateNoteCate();
                    fragmenta.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Setting_Dialog_DublicateNoteCate));

                    Toast.MakeText(this.Activity, "CheckDuplicateData", ToastLength.Short).Show();
                    return;
                }
                ManageNoteCategory();
                BtnAddCategory.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private bool CheckDuplicateData(string txtNotecategory)
        {
            try
            {
                if (string.IsNullOrEmpty(NoteCategoryName))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                if (NoteCategoryDetail != null)
                {
                    if (txtNotecategory == OldNameNoteCategory)
                    {
                        return true;
                    }
                }

                var checkname = Setting_Fragment_Note.lstCategory.FindIndex(x => x.Name.ToLower().Equals(txtNotecategory.ToLower()));
                if (checkname != -1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public async void ManageNoteCategory()
        {
            bool check = false;
            if (NoteCategoryDetail == null)
            {
                check = await InsertNoteCategory();
                if (!check) return;
            }
            else
            {
                check = await UpdateNoteCategory();
                if (!check) return;
            }
            SetClearData();   
        }

        private void SetClearData()
        {
            try
            {
                UINewNoteCategory();
                NoteCategoryDetail = null;
                addNotecate.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetClearData at Add Gift");
            }
        }

        private void UINewNoteCategory()
        {
            textNameCategory.Text = string.Empty;
        }

        private async Task<bool> UpdateNoteCategory()
        {
            try
            {
                EditNoteCategory.MerchantID = NoteCategoryDetail.MerchantID;
                EditNoteCategory.SysNoteCategoryID = NoteCategoryDetail.SysNoteCategoryID;
                EditNoteCategory.DataStatus = 'M';
                EditNoteCategory.FWaitSending = 2;
                EditNoteCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                EditNoteCategory.Name = NoteCategoryName;
                EditNoteCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                EditNoteCategory.DateCreated = Utils.GetTranDate(NoteCategoryDetail.DateCreated);

                //NoteCategory = 'M'
                NoteCategoryManage NoteCategoryManage = new NoteCategoryManage();
                var check = await NoteCategoryManage.UpdateNoteCategory(EditNoteCategory);
                if (!check)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    addNotecate.Dismiss();
                    return false;
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
                DataCashingAll.flagNoteCategoryChange = true;
                Setting_Fragment_Note.SetFocusItem(NoteCategoryDetail.SysNoteCategoryID, null);
                Setting_Fragment_Note.fragment_main.RefreshData();
                
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private async Task<bool> InsertNoteCategory()
        {
            try
            {
                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, systemID);
                sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                string NoteCategory = textNameCategory.Text.Trim();

                AddNoteCategory.MerchantID = DataCashingAll.MerchantId;
                AddNoteCategory.SysNoteCategoryID = Convert.ToInt64(sys);
                AddNoteCategory.Ordinary = null;  
                AddNoteCategory.Name = NoteCategoryName;
                AddNoteCategory.DateCreated = Utils.GetTranDate(DateTime.UtcNow);
                AddNoteCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                AddNoteCategory.DataStatus = 'I';
                AddNoteCategory.FWaitSending = 2;
                AddNoteCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);                

                var result = await noteCategoryManage.InsertNoteCategory(AddNoteCategory);
                Log.Debug("Insert", result.ToString());

                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
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
                DataCashingAll.flagNoteCategoryChange = true;
                Setting_Fragment_Note.SetFocusItem(AddNoteCategory.SysNoteCategoryID, null);
                Setting_Fragment_Note.fragment_main.RefreshData();
                addNotecate.Dismiss();
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Setting dialog at add Note");
                return false;
            }
        }

        private void FrameClickDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.setting_dialog_deletecatenote.ToString();
                bundle.PutString("message", myMessage);
                Setting_Dialog_DeleteNoteCate setting_Dialog = Setting_Dialog_DeleteNoteCate.NewInstance();
                setting_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("FrameClickDelete_Click at addnote");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                addNotecate.Dismiss();
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
                    BtnAddCategory.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                    BtnAddCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }
                else
                {
                    BtnAddCategory.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                    BtnAddCategory.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                }
                BtnAddCategory.Enabled = enable;
            }
            else
            {
                if (enable)
                {
                    btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                }
                else
                {
                    btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                    btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                }
                btnSave.Enabled = enable;
            }
        }
    }
}