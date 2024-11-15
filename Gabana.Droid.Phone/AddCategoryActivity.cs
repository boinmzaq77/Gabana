using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddCategoryActivity : AppCompatActivity
    {
        public static AddCategoryActivity addcategory;
        internal Button btnAdd;
        ImageButton BtnBack;
        TextView textTitle;
        EditText txtNameCategory;
        Gabana.ORM.MerchantDB.Category addCategory = new Gabana.ORM.MerchantDB.Category();
        Gabana.ORM.MerchantDB.Category editCategory = new Gabana.ORM.MerchantDB.Category();
        CategoryManage categoryManage = new CategoryManage();
        bool first = true, flagdatachange = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addcategory_activity_main);

                addcategory = this;

                btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
                BtnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                txtNameCategory = FindViewById<EditText>(Resource.Id.txtNameCategory);
                txtNameCategory.TextChanged += TxtNameCategory_TextChanged;
                textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                FrameLayout lnBack = FindViewById<FrameLayout>(Resource.Id.lnBack);
                lnBack.Click += BtnBack_Click;
                BtnBack.Click += BtnBack_Click;
                FrameLayout btnDelete = FindViewById<FrameLayout>(Resource.Id.btnDelete);

                CheckJwt();

                if (DataCashing.EditSysCategory == "EditCategory")
                {
                    btnAdd.Click += BtnEdit_Click;
                    UpdateCategory();
                    btnDelete.Visibility = ViewStates.Visible;
                    btnDelete.Click += BtnDelete_Click;
                    btnAdd.Text = GetString(Resource.String.textsave);
                    textTitle.Text = GetString(Resource.String.editcategory_activity_title);
                }
                else
                {
                    btnAdd.Click += BtnAdd_Click;
                    btnDelete.Visibility = ViewStates.Gone;
                    btnAdd.Text = GetString(Resource.String.addcategory_activity_title);
                    textTitle.Text = GetString(Resource.String.addcategory_activity_title);

                }
                first = false;
                SetButtonAdd(false);
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddCategoryActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("OnCreate at add category");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        private void TxtNameCategory_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (string.IsNullOrEmpty(DataCashing.EditSysCategory))
            {
                if (!string.IsNullOrEmpty(txtNameCategory.Text))
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
            }
            else
            {
                if (txtNameCategory.Text != editCategory.Name)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
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

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("deleteType", "category");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at add category");
                return;
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            DataCashing.EditSysCategory = null;
        }

        public override void OnBackPressed()
        {
            try
            {
                if (string.IsNullOrEmpty(DataCashing.EditSysCategory))
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "category");
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
                    bundle.PutString("fromPage", "category");
                    bundle.PutString("PassValue", DataCashing.EditSysCategoryID.ToString());
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

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                DeviceSystemSeqNo deviceSystemSeq = new DeviceSystemSeqNo();
                DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();

                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 20);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                var result = false;
                // CultureInfo.CurrentCulture = new CultureInfo("en-US");
                addCategory.MerchantID = DataCashingAll.MerchantId;
                addCategory.SysCategoryID = long.Parse(sys);
                addCategory.Ordinary = null;
                addCategory.Name = txtNameCategory.Text.Trim();
                addCategory.DateCreated = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.DataStatus = 'I';
                addCategory.FWaitSending = 2;
                addCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.LinkProMaxxID = null;

                var checkName = await categoryManage.CheckCategoryName(addCategory.Name);
                if (checkName)
                {
                    try
                    {
                        btnAdd.Enabled = true;
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                        var json = JsonConvert.SerializeObject(addCategory);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "inserCategory");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "insert");
                        bundle.PutString("detailitem", addCategory.Name);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        btnAdd.Enabled = true;
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add Category");
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                result = await categoryManage.InsertCategory(addCategory);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }
                Toast.MakeText(this, GetString(Resource.String.insertsucess), ToastLength.Short).Show();

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)addCategory.MerchantID, (int)addCategory.SysCategoryID);
                }
                else
                {
                    addCategory.FWaitSending = 2;
                    await categoryManage.UpdateCategory(addCategory);
                }

                DataCashingAll.flagCategoryChange = true;
                ItemActivity.SetFocusCategory(addCategory);
                btnAdd.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnAdd_Click at add category");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                //CultureInfo.CurrentCulture = new CultureInfo("en-US");
                editCategory.Name = txtNameCategory.Text.Trim();
                editCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                editCategory.FWaitSending = 2;
                editCategory.DataStatus = 'M';
                editCategory.DateCreated = Utils.GetTranDate(editCategory.DateCreated);
                editCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                var checkName = await categoryManage.CheckCategoryName(editCategory.Name);
                if (checkName)
                {
                    try
                    {
                        btnAdd.Enabled = true;
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                        var json = JsonConvert.SerializeObject(editCategory);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "inserCategory");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "update");
                        bundle.PutString("detailitem", editCategory.Name);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        btnAdd.Enabled = true;
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add Category");
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                var result = await categoryManage.UpdateCategory(editCategory);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)DataCashingAll.MerchantId, (int)DataCashing.EditSysCategoryID);
                }
                else
                {
                    editCategory.FWaitSending = 2;
                    await categoryManage.UpdateCategory(editCategory);
                }

                DataCashingAll.flagCategoryChange = true;                
                ItemActivity.SetFocusCategory(editCategory);
                btnAdd.Enabled = true;
                this.Finish();                
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnEdit_Click at add category");
            }
        }

        public async void UpdateCategory()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {

                editCategory = await categoryManage.GetCategory((int)DataCashingAll.MerchantId, (int)DataCashing.EditSysCategoryID);
                txtNameCategory.Text = editCategory.Name;
                txtNameCategory.SetSelection(editCategory.Name.Length);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("UpdateCategory at add category");
                return;
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

