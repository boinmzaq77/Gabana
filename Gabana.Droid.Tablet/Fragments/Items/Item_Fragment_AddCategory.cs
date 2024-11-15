using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using System.Threading.Tasks;
using Gabana.Droid.Tablet.Dialog;

namespace Gabana.Droid.Tablet.Fragments.Items
{
    public class Item_Fragment_AddCategory : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Item_Fragment_AddCategory NewInstance()
        {
            Item_Fragment_AddCategory frag = new Item_Fragment_AddCategory();
            return frag;
        }
        public static Item_Fragment_AddCategory fragment_addcategory;
        View view;

        Gabana.ORM.MerchantDB.Category addCategory = new Gabana.ORM.MerchantDB.Category();
        public static Gabana.ORM.MerchantDB.Category editCategory = new Gabana.ORM.MerchantDB.Category();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.item_fragment_addcategory, container, false);
            try
            {
                fragment_addcategory = this;
                editCategory = DataCashing.EditCategory;
                CheckJwt();
                CombineUI();
                SetEventUI();
                SetDetail();
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        private void SetEventUI()
        {
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            txtNameCategory.TextChanged += TxtNameCategory_TextChanged;
            lnBack.Click += BtnBack_Click;
        }

        private async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    MainActivity.main_activity.Finish();
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

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            CheckJwt();
            editCategory = DataCashing.EditCategory;

            if (DataCashing.EditCategory == null)
            {
                btnDelete.Visibility = ViewStates.Gone;
                txtNameCategory.Text = string.Empty;
                btnAdd.Text = GetString(Resource.String.addcategory_activity_title);
                textTitle.Text = GetString(Resource.String.addcategory_activity_title);
                UINewCategory();
            }
            else
            {
                txtNameCategory.Text = DataCashing.EditCategory.Name;
                btnDelete.Visibility = ViewStates.Visible;
                btnAdd.Text = GetString(Resource.String.textsave);
                textTitle.Text = GetString(Resource.String.editcategory_activity_title);
            }
        }

        private void SetDetail()
        {
            try
            {
                if (editCategory == null)
                {
                    btnAdd.Text = GetString(Resource.String.addcategory_activity_title);
                    textTitle.Text = GetString(Resource.String.addcategory_activity_title);
                    btnAdd.Text = GetString(Resource.String.textsave);
                    textTitle.Text = GetString(Resource.String.editcategory_activity_title);
                    btnDelete.Visibility = ViewStates.Gone;
                    UINewCategory();
                }
                else
                {
                    btnAdd.Text = GetString(Resource.String.textsave);
                    textTitle.Text = GetString(Resource.String.editcategory_activity_title);
                    txtNameCategory.Text = DataCashing.EditCategory.Name;
                    btnDelete.Visibility = ViewStates.Visible;
                }
                SetButtonAdd(false);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                bool CheckDup = false;
                CheckDup = await CheckDuplicateData(txtNameCategory.Text);
                if (!CheckDup)
                {
                    btnAdd.Enabled = true;
                    var fragmenta = new Category_Dialog_Dublicate();
                    fragmenta.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Category_Dialog_Dublicate));
                    return;
                }
                ManageCategory();
                btnAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnAdd_Click at addcategory");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task<bool> CheckDuplicateData(string categoryName)
        {
            try
            {
                var checkName =  await categoryManage.CheckCategoryName(categoryName);
                if (checkName)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("CheckDuplicateData addcategory");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public async void ManageCategory()
        {
            try
            {
                bool check = false;
                if (editCategory == null)
                {
                    check = await InsertCatagory();
                    if (!check) return;
                }
                else
                {
                    check = await UpdateCategory();
                    if (!check) return;
                }
                SetClearData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ManageCategory");
            }
        }

        public void SetClearData()
        {
            UINewCategory();
            DataCashing.EditItem = null;
            editCategory = null;
            flagdatachange = false;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "default");
        }

        public void UINewCategory()
        {
            txtNameCategory.Text = string.Empty;
        }

        private async Task<bool> UpdateCategory()
        {
            try
            {
                editCategory.Name = txtNameCategory.Text.Trim();
                editCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                editCategory.FWaitSending = 2;
                editCategory.DataStatus = 'M';
                editCategory.DateCreated = Utils.GetTranDate(editCategory.DateCreated);
                editCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                var result = await categoryManage.UpdateCategory(editCategory);
                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)DataCashingAll.MerchantId, (int)DataCashing.EditSysCategoryID);
                }
                else
                {
                    editCategory.FWaitSending = 2;
                    await categoryManage.UpdateCategory(editCategory);
                }
                Item_Fragment_Main.fragment_main.ReloadCategory(editCategory);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> InsertCatagory()
        {
            try
            {
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

                result = await categoryManage.InsertCategory(addCategory);
                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }
                Toast.MakeText(this.Activity, GetString(Resource.String.insertsucess), ToastLength.Short).Show();

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

                Item_Fragment_Main.fragment_main.ReloadCategory(addCategory);
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DetailInsert at Item_Fragment_AddCategory");
                return false;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnDelete.Enabled = false;
                MainDialog dialog = new MainDialog() { Cancelable = false };
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.addcategory_dialog_delete.ToString();
                bundle.PutString("message", myMessage);
                dialog.Arguments = bundle;
                dialog.Show(Activity.SupportFragmentManager, myMessage);
                btnDelete.Enabled = true;
            }
            catch (Exception ex)
            {
                btnDelete.Enabled = true;
                Toast.MakeText(this.Activity, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at add category");
                return;
            }
        }

        private async void DeleteCategory()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Android.App.Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
                    return;
                }
                
                ItemManage itemManage = new ItemManage();
                Category cateDelte = new Category();
                cateDelte = DataCashing.EditCategory;

                var UpdateItem = await itemManage.GetItembyCategory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                if (UpdateItem != null)
                {
                    foreach (var update in UpdateItem)
                    {
                        update.SysCategoryID = null;
                        var resultUpdate = await itemManage.UpdateItem(update);
                    }
                }
                cateDelte.DataStatus = 'D';
                cateDelte.FWaitSending = 2;
                cateDelte.DateModified = DateTime.UtcNow;
                var updateCate = await categoryManage.UpdateCategory(cateDelte);
                if (!updateCate)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotdelete), ToastLength.Short).Show();                    
                    return;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.deletesucess), ToastLength.Short).Show();
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                }
                else
                {
                    cateDelte.FWaitSending = 2;
                    await categoryManage.UpdateCategory(cateDelte);
                }

                DataCashing.EditCategory = null;
                editCategory = null;
                flagdatachange = false;
                Item_Fragment_Main.fragment_main.DeleteCategory(cateDelte);
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "default");
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
            }
        }

        CategoryManage categoryManage = new CategoryManage();  
        
        public static bool flagdatachange = false;
        private void BtnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flagdatachange)
                {
                    SetClearData(); return;
                }

                if (DataCashing.EditCategory == null)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Add_Dialog_Back.SetPage("category");
                    Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();
                    add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Edit_Dialog_Back.SetPage("category");
                    Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                    edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void TxtNameCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void CheckDataChange()
        {
            if (DataCashing.EditCategory == null)
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
                if (txtNameCategory.Text != DataCashing.EditCategory.Name)
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

        TextView textTitle;
        FrameLayout lnBack, btnDelete;
        public static EditText txtNameCategory;
        internal Button btnAdd;
        private void CombineUI()
        {
            textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
            lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
            txtNameCategory = view.FindViewById<EditText>(Resource.Id.txtNameCategory);
            btnDelete = view.FindViewById<FrameLayout>(Resource.Id.btnDelete);
            btnAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
        }
    }
}