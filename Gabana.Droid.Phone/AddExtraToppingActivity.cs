using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]

    public class AddExtraToppingActivity : AppCompatActivity
    {
        public static AddExtraToppingActivity addExtra;
        LinearLayout linearShowProduct, linearShowStock, lnDetails, lnSwithStcok, lnOnhand, lnStockMoveMent;
        internal Button /*buttonAddProduct, */btnadditem;
        Spinner spinnerCategory;
        FrameLayout btnDelete;
        EditText textInsertProduct, textInsertPrice, txtcost, txtminimumStock;
        TextView txtTitle, txtViewItemnameTitle, txtItemNamePic, txtPricePic, txtStock;
        ImageButton btnCategory, imgBack, btncolor1, btncolor2, btncolor3, btncolor4, btncolor5, btncolor6, btncolor7, btncolor8, btncolor9, btnaddpic, btnShowDetail;
        ImageView imageViewItem, colorViewItem, imgFavorite;
        Switch switchStock;
        NoteCategoryManage noteManage = new NoteCategoryManage();
        ItemManage itemManage = new ItemManage();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        Item AddToppping = new Item();
        Item EditToppping = new Item();
        List<string> itemID = new List<string>();
        List<string> SysNoteCategoryID = new List<string>();
        int SysItemId;
        long? SyscategoryID;
        string ToppingName, sys, colorSelected, DecimalDisplay, LoginType;
        decimal showStock;
        public static Item ExtraToppping;

        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        Android.Net.Uri keepCropedUri;
        Android.Graphics.Bitmap bitmap;
        string path;

        RecyclerView recyclerHeaderItem;
        public static string tabSelected = "";
        public List<MenuTab> MenuTab { get; set; }
        private bool showdetail, offline = false;
        string CURRENCYSYMBOLS, usernamelogin;
        bool favorite, HavePicture = false;
        public static bool checkManageStock = false;
        ItemOnBranch itemOnBranch, GetDataStock, getBalance;
        static string stockOnhabd;
        string pathThumnailFolder, pathFolderPicture;
        static bool EditStock = false;
        bool first = true, flagdatachange = false, CheckNet = false;
        FrameLayout btnFavorite;
        LinearLayout lnCategory;
        DialogLoading dialogLoading = new DialogLoading();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.addextratopping_activity);

                addExtra = this;
                recyclerHeaderItem = FindViewById<RecyclerView>(Resource.Id.recyclerHeaderItem);
                linearShowProduct = FindViewById<LinearLayout>(Resource.Id.lnShowProduct);
                linearShowStock = FindViewById<LinearLayout>(Resource.Id.lnShowStock);
                textInsertProduct = FindViewById<EditText>(Resource.Id.textInsertProduct);
                textInsertPrice = FindViewById<EditText>(Resource.Id.textInsertPrice);
                txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
                txtcost = FindViewById<EditText>(Resource.Id.txtcost);
                txtStock = FindViewById<TextView>(Resource.Id.txtStock);
                txtminimumStock = FindViewById<EditText>(Resource.Id.txtMinimumStock);
                txtStock.TextChanged += TxtStock_TextChanged;
                txtminimumStock.TextChanged += TxtminimumStock_TextChanged;
                txtminimumStock.FocusChange += TxtminimumStock_FocusChange;
                spinnerCategory = FindViewById<Spinner>(Resource.Id.spinnerCategory);
                btnadditem = FindViewById<Button>(Resource.Id.btnAdditem);
                btnCategory = FindViewById<ImageButton>(Resource.Id.btnCategory);
                txtViewItemnameTitle = FindViewById<TextView>(Resource.Id.txtViewItemnameTitle);
                imageViewItem = FindViewById<ImageView>(Resource.Id.imageViewItem);
                imageViewItem.Click += ImageViewItem_Click;
                colorViewItem = FindViewById<ImageView>(Resource.Id.colorViewItem);
                btncolor1 = FindViewById<ImageButton>(Resource.Id.btncolor1);
                btncolor2 = FindViewById<ImageButton>(Resource.Id.btncolor2);
                btncolor3 = FindViewById<ImageButton>(Resource.Id.btncolor3);
                btncolor4 = FindViewById<ImageButton>(Resource.Id.btncolor4);
                btncolor5 = FindViewById<ImageButton>(Resource.Id.btncolor5);
                btncolor6 = FindViewById<ImageButton>(Resource.Id.btncolor6);
                btncolor7 = FindViewById<ImageButton>(Resource.Id.btncolor7);
                btncolor8 = FindViewById<ImageButton>(Resource.Id.btncolor8);
                btncolor9 = FindViewById<ImageButton>(Resource.Id.btncolor9);
                btnaddpic = FindViewById<ImageButton>(Resource.Id.btnaddpic);
                btnShowDetail = FindViewById<ImageButton>(Resource.Id.btnShowDetail);
                txtItemNamePic = FindViewById<TextView>(Resource.Id.txtItemNamePic);
                txtPricePic = FindViewById<TextView>(Resource.Id.txtPricePic);
                txtPricePic.Hint = Utils.DisplayDecimal(0);
                lnDetails = FindViewById<LinearLayout>(Resource.Id.lnDetails);
                lnSwithStcok = FindViewById<LinearLayout>(Resource.Id.lnSwithStcok);
                imgBack = FindViewById<ImageButton>(Resource.Id.imgBack);
                FrameLayout lnBack = FindViewById<FrameLayout>(Resource.Id.lnBack);
                lnCategory = FindViewById<LinearLayout>(Resource.Id.lnCategory);
                lnBack.Click += ImgBack_Click;
                imgBack.Click += ImgBack_Click;
                FrameLayout lnBtnShowDetail = FindViewById<FrameLayout>(Resource.Id.lnBtnShowDetail);
                lnBtnShowDetail.Click += BtnShowDetail_Click;
                textInsertProduct.TextChanged += TextInsertProduct_TextChanged;
                switchStock = FindViewById<Switch>(Resource.Id.switchStock);
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
                textInsertPrice.Hint = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(0);
                textInsertPrice.TextChanged += TextInsertPrice_TextChanged;
                textInsertPrice.KeyPress += TextInsertPrice_KeyPress;
                txtcost.Hint = Utils.DisplayDecimal(0);
                txtcost.TextChanged += Txtcost_TextChanged;
                txtcost.KeyPress += Txtcost_KeyPress;
                btnCategory.Click += BtnCategory_Click;
                lnCategory.Click += BtnCategory_Click;
                btncolor1.Click += Btncolor1_Click;
                btncolor2.Click += Btncolor2_Click;
                btncolor3.Click += Btncolor3_Click;
                btncolor4.Click += Btncolor4_Click;
                btncolor5.Click += Btncolor5_Click;
                btncolor6.Click += Btncolor6_Click;
                btncolor7.Click += Btncolor7_Click;
                btncolor8.Click += Btncolor8_Click;
                btncolor9.Click += Btncolor9_Click;
                btnaddpic.Click += Btnaddpic_Click;
                imgBack.Click += ImgBack_Click;
                switchStock.CheckedChange += SwitchStock_CheckedChange;
                lnOnhand = FindViewById<LinearLayout>(Resource.Id.lnOnhand);
                lnOnhand.Click += LnOnhand_Click;
                lnStockMoveMent = FindViewById<LinearLayout>(Resource.Id.lnStockMoveMent);
                lnStockMoveMent.Click += LnStockMoveMent_Click;
                btnFavorite = FindViewById<FrameLayout>(Resource.Id.btnFavorite);
                imgFavorite = FindViewById<ImageView>(Resource.Id.imgFavorite);
                favorite = false;
                btnFavorite.Click += BtnFavorite_Click;
                pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                pathFolderPicture = DataCashingAll.PathFolderImage;
                SetColorItemView();
                CheckJwt();
                CheckPermission();
                await SpinnerCategory();

                if (dialogLoading != null & dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                CheckNet = await GabanaAPI.CheckSpeedConnection();

                DecimalDisplay = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;
                colorSelected = "#0095DA";
                string colorReplace = SetAlphaColor(colorSelected);
                imgBack.RequestFocus();
               
                btnShowDetail.Click += BtnShowDetail_Click;
                showdetail = false;  
                usernamelogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                btnDelete = FindViewById<FrameLayout>(Resource.Id.btnDelete);                

                if (ExtraToppping == null)
                {
                    txtTitle.Text = GetString(Resource.String.item_activity_addtopping);
                    btnadditem.Text = GetString(Resource.String.item_activity_addtopping);
                    btnadditem.Click += Additem_Click;
                    btnDelete.Visibility = ViewStates.Gone;
                    SetColorItemView();
                }
                else
                {
                    txtTitle.Text = GetString(Resource.String.extra_activity_Edit);
                    btnadditem.Text = GetString(Resource.String.textsave);
                    SysItemId = (int)ExtraToppping.SysItemID;
                    await ShowToppingBeforEdit();
                    await GetStockData();                   
                    btnadditem.Click += Edititem_Click;                    
                    btnDelete.Visibility = ViewStates.Visible;
                    btnDelete.Click += BtnDelete_Click;
                }
                await GetExtraList();
                SetTabMenu();
                SetTabShowMenu();

                SetFavorite();
                ShowDetailItem();               
                

                first = false;
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddEmployeeActivity");
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at add Extra");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtminimumStock_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                //เคส ถ้า minimum = 0 ให้ลบ 0 ออกก่อนจะกดตัวเลขอื่น
                if (e.HasFocus)
                {
                    if (txtminimumStock.Text.Length == 1 && txtminimumStock.Text == "0")
                    {
                        txtminimumStock.Text = String.Empty;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SetUIFromMainRole(string loginType)
        {
            var check = UtilsAll.CheckPermissionRoleUser(loginType, "insert", "item");
            if (check && CheckNet)
            {
                txtViewItemnameTitle.Enabled = true;
                btncolor1.Enabled = true;
                btncolor2.Enabled = true;
                btncolor3.Enabled = true;
                btncolor4.Enabled = true;
                btncolor5.Enabled = true;
                btncolor6.Enabled = true;
                btncolor7.Enabled = true;
                btncolor8.Enabled = true;
                btncolor9.Enabled = true;
                btnaddpic.Enabled = true;
                btnFavorite.Enabled = true;
                textInsertProduct.Enabled = true;
                textInsertProduct.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textInsertProduct.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textInsertPrice.Enabled = true;
                textInsertPrice.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textInsertPrice.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcost.Enabled = true;
                txtcost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                lnCategory.Enabled = true;
                spinnerCategory.Enabled = true;
                btnCategory.Enabled = true;
                btnCategory.SetBackgroundResource(Resource.Mipmap.Next);
                switchStock.Enabled = true;
                txtStock.Enabled = true;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnadditem.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnadditem.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else if (check && !CheckNet)
            {
                txtViewItemnameTitle.Enabled = true;
                btncolor1.Enabled = true;
                btncolor2.Enabled = true;
                btncolor3.Enabled = true;
                btncolor4.Enabled = true;
                btncolor5.Enabled = true;
                btncolor6.Enabled = true;
                btncolor7.Enabled = true;
                btncolor8.Enabled = true;
                btncolor9.Enabled = true;
                btnaddpic.Enabled = true;
                btnFavorite.Enabled = true;
                textInsertProduct.Enabled = true;
                textInsertProduct.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textInsertProduct.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textInsertPrice.Enabled = true;
                textInsertPrice.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textInsertPrice.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcost.Enabled = true;
                txtcost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                lnCategory.Enabled = true;
                spinnerCategory.Enabled = true;
                btnCategory.Enabled = true;
                btnCategory.SetBackgroundResource(Resource.Mipmap.Next);
                switchStock.Enabled = false;
                txtStock.Enabled = false;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                lnOnhand.Enabled = false;
            }
            else
            {
                txtViewItemnameTitle.Enabled = false;
                btncolor1.Enabled = false;
                btncolor2.Enabled = false;
                btncolor3.Enabled = false;
                btncolor4.Enabled = false;
                btncolor5.Enabled = false;
                btncolor6.Enabled = false;
                btncolor7.Enabled = false;
                btncolor8.Enabled = false;
                btncolor9.Enabled = false;
                btnaddpic.Enabled = false;
                btnFavorite.Enabled = false;
                textInsertProduct.Enabled = false;
                textInsertProduct.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                textInsertProduct.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                textInsertPrice.Enabled = false;
                textInsertPrice.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                textInsertPrice.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtcost.Enabled = false;
                txtcost.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtcost.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                lnCategory.Enabled = false;
                spinnerCategory.Enabled = false;
                btnCategory.Enabled = false;
                btnCategory.SetBackgroundResource(Resource.Mipmap.NextG);
                switchStock.Enabled = false;
                txtStock.Enabled = false;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtminimumStock.Enabled = false;
                txtminimumStock.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtminimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnadditem.SetBackgroundResource(Resource.Drawable.btnbordergray);
                btnadditem.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
            }

            check = UtilsAll.CheckPermissionRoleUser(loginType, "delete", "item");
            if (check)
            {
                btnDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                btnDelete.Visibility = ViewStates.Gone;

            }
        }

        private void ImageViewItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = "";
                if (ExtraToppping != null)
                {
                    //MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.dialog_item.ToString();
                    bundle.PutString("message", myMessage);
                    if (!string.IsNullOrEmpty(ExtraToppping.PicturePath))
                    {
                        bundle.PutString("OpenCloudPicture", ExtraToppping.PicturePath);
                        path = ExtraToppping.PicturePath;
                    }
                    else
                    {
                        bundle.PutString("OpenCloudPicture", ExtraToppping.PictureLocalPath);
                        path = ExtraToppping.PictureLocalPath;
                    }
                    //dialog.Arguments = bundle;
                    //dialog.Show(SupportFragmentManager, myMessage);

                    Show_Dialog_Item dialog_Item = Show_Dialog_Item.NewInstance(path);
                    dialog_Item.Show(SupportFragmentManager, myMessage);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgProfile_Click at add add Topping");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtminimumStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            int max = 999999;
            var remove = Utils.CheckLenghtValue(txtminimumStock.Text);
            int value = string.IsNullOrEmpty(remove) ? 0 : int.Parse(remove);
            if (max < value)
            {
                Toast.MakeText(this, GetString(Resource.String.maxitem) + " " + max.ToString("#,###"), ToastLength.Short).Show();
                txtminimumStock.Text = max.ToString("#,###");
                txtminimumStock.SetSelection(txtminimumStock.Text.Length);
                CheckDataChange();
                return;
            }
            //EditStock = true;
            CheckDataChange();
        }

        private void BtnFavorite_Click(object sender, EventArgs e)
        {
            if (favorite)
            {
                favorite = false;
            }
            else
            {
                favorite = true;
            }
            SetFavorite();

            CheckDataChange();

        }

        private void SetFavorite()
        {
            if (favorite)
            {
                imgFavorite.SetBackgroundResource(Resource.Mipmap.Fav);
            }
            else
            {
                imgFavorite.SetBackgroundResource(Resource.Mipmap.Unfav);
            }
            CheckDataChange();
        }

        internal static void SetOnhand(string text)
        {
            addExtra.txtStock.Text = text;
            stockOnhabd = text;

            if (checkManageStock)
            {
                //open stock
                addExtra.lnSwithStcok.Visibility = ViewStates.Visible;
                if (!string.IsNullOrEmpty(stockOnhabd))
                {
                    addExtra.txtStock.Text = Convert.ToInt32(Utils.CheckLenghtValue(stockOnhabd)).ToString("#,###");
                    stockOnhabd = string.Empty;
                }
            }
        }

        private void LnStockMoveMent_Click(object sender, EventArgs e)
        {
            if (SysItemId != 0)
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(StockMoveMentActivity)));
                StockMoveMentActivity.SetPageView(ExtraToppping);
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notdata), ToastLength.Short).Show();
            }
        }

        private void LnOnhand_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(OnhandActivity)));
            OnhandActivity.SetPageView("AddExtra", txtStock.Text);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            string Role = LoginType;
            bool check = UtilsAll.CheckPermissionRoleUser(Role, "delete", "topping");
            if (check)
            {
                DeleteExtraTopping();
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        private void DeleteExtraTopping()
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutInt("systemID", (int)SysItemId);
                bundle.PutString("deleteType", "topping");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteExtraTopping at add Extra");
                Toast.MakeText(this, $"Can't delete{ex.Message}", ToastLength.Short).Show();
                return;
            }
        }

        public string SetAlphaColor(string colorSelected)
        {
            string[] color = colorSelected.Split("#");
            string colorReplace = "#80" + color[1];
            return colorReplace;
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async void BtnShowDetail_Click(object sender, EventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (showdetail)
            {
                showdetail = false;
            }
            else
            {
                showdetail = true;
            }
            ShowDetailItem();
            SetUIFromMainRole(LoginType);
            CheckDataChange();
        }

        private void ShowDetailItem()
        {
            if (showdetail)
            {
                lnDetails.Visibility = ViewStates.Visible;
                btnShowDetail.SetBackgroundResource(Resource.Mipmap.DetailShow);
            }
            else
            {
                lnDetails.Visibility = ViewStates.Gone;
                btnShowDetail.SetBackgroundResource(Resource.Mipmap.DetailNotShow);
            }
        }
        private void BtnCategory_Click(object sender, EventArgs e)
        {
            spinnerCategory.PerformClick();
        }

        public async Task SpinnerCategory()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                itemID = new List<string>();
                CategoryManage categoryManage = new CategoryManage();
                Category addcategory = new Category();
                List<Category> category = new List<Category>();
                List<Category> getallCategory = new List<Category>();

                addcategory = new Category()
                {
                    Name = "None Category",
                    SysCategoryID = 0
                };
                category.Add(addcategory);
                getallCategory = await categoryManage.GetAllCategory();
                category.AddRange(getallCategory);

                for (int i = 0; i < category.Count; i++)
                {
                    temp = category[i].Name.ToString();
                    temp2 = category[i].SysCategoryID.ToString();
                    items.Add(temp);
                    itemID.Add(temp2);
                }

                spinnerCategory.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerCategory_ItemSelected);
                var adapterCategory = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerCategory.Adapter = adapterCategory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SpinnerCategory at add Extra");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void spinnerCategory_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (itemID == null)
            {
                return;
            }

            SyscategoryID = Convert.ToInt32(itemID[e.Position].ToString());

            CheckDataChange();

        }

        private async void Additem_Click(object sender, EventArgs e)
        {           
            int count = itemManage.CountItem();
            if (count <= 10000)
            {
                await InsertToppping();
            }
        }

        private async void Edititem_Click(object sender, EventArgs e)
        {
            btnadditem.Enabled = false;
            string Role = LoginType;
            bool check = UtilsAll.CheckPermissionRoleUser(Role, "update", "topping");
            if (!check)
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                btnadditem.Enabled = true;
                return;
            }
            await UpdateToppping();
            btnadditem.Enabled = true;
        }

        async Task ShowToppingBeforEdit()
        {
            try
            {
                textInsertProduct.Text = ExtraToppping.ItemName;
                textInsertPrice.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(ExtraToppping.Price);
                txtcost.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(ExtraToppping.EstimateCost);

                var cloudpath = ExtraToppping.PicturePath == null ? string.Empty : ExtraToppping.PicturePath;
                var localpath = ExtraToppping.ThumbnailLocalPath == null ? string.Empty : ExtraToppping.ThumbnailLocalPath;

                if (CheckNet)
                {
                    if (string.IsNullOrEmpty(localpath))
                    {
                        if (string.IsNullOrEmpty(cloudpath))
                        {
                            //defalut
                            imageViewItem.SetImageURI(null);
                            string conColor = Utils.SetBackground(Convert.ToInt32(ExtraToppping.Colors));
                            var color = Android.Graphics.Color.ParseColor(conColor);
                            colorSelected = conColor;
                            colorViewItem.SetBackgroundColor(color);
                        }
                        else
                        {
                            //cloud
                            Utils.SetImage(imageViewItem, cloudpath);
                        }
                    }
                    else
                    {
                        //local
                        colorSelected = "#A2A2A2";
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imageViewItem.SetImageURI(uri);
                        imageViewItem.Visibility = ViewStates.Visible;
                        txtViewItemnameTitle.Visibility = ViewStates.Gone;
                        var color = Android.Graphics.Color.ParseColor(colorSelected);
                        colorViewItem.SetBackgroundColor(color);
                        HavePicture = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        colorSelected = "#A2A2A2";
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imageViewItem.SetImageURI(uri);
                        imageViewItem.Visibility = ViewStates.Visible;
                        txtViewItemnameTitle.Visibility = ViewStates.Gone;
                        var color = Android.Graphics.Color.ParseColor(colorSelected);
                        colorViewItem.SetBackgroundColor(color);
                        HavePicture = true;
                    }
                    else
                    {
                        imageViewItem.SetImageURI(null);
                        string conColor = Utils.SetBackground(Convert.ToInt32(ExtraToppping.Colors));
                        var color = Android.Graphics.Color.ParseColor(conColor);
                        colorSelected = conColor;
                        colorViewItem.SetBackgroundColor(color);
                    }
                }

                if (ExtraToppping.FavoriteNo != 0)
                {
                    favorite = true;
                }
                else
                {
                    favorite = false;
                }

                if (ExtraToppping.FTrackStock == 1)
                {
                    switchStock.Checked = true;
                    showStock = 1;
                }
                else
                {
                    switchStock.Checked = false;
                    showStock = 0;
                }

                SetFavorite();
                SpinnerSelectCategory();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowToppingBeforEdit at add Extra");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public async Task InsertToppping()
        {
            try
            {
                int colorItem = 0;
                if (colorSelected == "#A2A2A2")
                {
                    colorItem = 0;
                }
                else
                {
                    string color = colorSelected;
                    string[] scolor = color.Split("#");
                    colorItem = int.Parse(scolor[1], System.Globalization.NumberStyles.HexNumber);
                }

                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 30);
                sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                AddToppping.MerchantID = DataCashingAll.MerchantId;
                AddToppping.SysItemID = Convert.ToInt64(sys);
                AddToppping.ItemName = ToppingName;
                AddToppping.Ordinary = 0;

                if (SyscategoryID == 0)
                {
                    AddToppping.SysCategoryID = null;
                }
                else
                {
                    AddToppping.SysCategoryID = SyscategoryID;
                }
                AddToppping.ItemCode = null;

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResult = await Utils.InsertImageToThumbnail(path, bitmap, "item");
                    if (checkResult)
                    {
                        AddToppping.ThumbnailPath = pathThumnailFolder + path;
                        AddToppping.ThumbnailLocalPath = pathThumnailFolder + path;
                    }

                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    if (checkResultPicture)
                    {
                        AddToppping.PictureLocalPath = pathFolderPicture + path;
                    }
                    //Utils.streamImage(bitmap);
                    AddToppping.PicturePath = keepCropedUri.ToString();
                }
                else
                {
                    AddToppping.ThumbnailPath = null;
                    AddToppping.PicturePath = null;
                    AddToppping.PictureLocalPath = null;
                    AddToppping.ThumbnailLocalPath = null;
                }

                if (textInsertProduct.Text.Length > 5)
                {
                    AddToppping.ShortName = textInsertProduct.Text.Substring(0, 5);
                }
                else
                {
                    AddToppping.ShortName = textInsertProduct.Text;
                }

                AddToppping.Colors = colorItem;
                AddToppping.UnitName = null;
                AddToppping.RegularSizeName = null;

                if (favorite)
                {
                    AddToppping.FavoriteNo = 1;
                }
                else
                {
                    AddToppping.FavoriteNo = 0;
                }
                decimal minimumStock = 0;
                if (string.IsNullOrEmpty(txtminimumStock.Text))
                {
                    minimumStock = 0;
                }
                else
                {
                    minimumStock = ConvertToDecimal(txtminimumStock.Text.Trim());
                }


                var Price = textInsertPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textInsertPrice.Text;
                }
                AddToppping.Price = ConvertToDecimal(Price);

                //Topping มี cost ไหม
                if (string.IsNullOrEmpty(txtcost.Text))
                {
                    //ถ้า User ไม่กำหนด ต้นทุน ระบบจะนำเอาราคาขายที่ตั้งมาเป็นต้นทุน
                    AddToppping.EstimateCost = ConvertToDecimal(Price);
                }
                else
                {
                    var Cost = txtcost.Text.Trim();
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        Cost = txtcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        Cost = txtcost.Text;
                    }
                    AddToppping.EstimateCost = ConvertToDecimal(Cost);
                }
                AddToppping.OptSalePrice = 'F';
                AddToppping.TaxType = 'N';
                AddToppping.SellBy = 'T';
                AddToppping.FTrackStock = 0;
                AddToppping.FDisplayOption = 0;
                AddToppping.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                AddToppping.SaleItemType = 'T';
                AddToppping.Comments = null;
                AddToppping.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                AddToppping.UserLastModified = usernamelogin;
                AddToppping.DataStatus = 'I';
                AddToppping.FWaitSending = 2;
                AddToppping.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                //Check ชื่อสินค้า 
                var checkname = itemExtra.FindIndex(x => x.ItemName.Equals(AddToppping.ItemName));
                if (checkname != -1)
                {
                    try
                    {
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                        InsertRepeatItem insertRepeat = new InsertRepeatItem();
                        insertRepeat.checkManageStock = checkManageStock;
                        insertRepeat.DetailITem = AddToppping;
                        insertRepeat.Stock = txtStock.Text;
                        insertRepeat.minimumstock = minimumStock.ToString("#,##0");
                        var json = JsonConvert.SerializeObject(insertRepeat);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "insertrepeat");
                        bundle.PutString("detailitem", AddToppping.ItemName);
                        bundle.PutString("fromPage", "itemname");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "insert");
                        bundle.PutString("itemtype", "topping");
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("checkName at add Extra");
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                if (checkManageStock)
                {
                    if (AddToppping.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(txtStock.Text))
                        {
                            Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                            return;
                        }

                        AddToppping.FTrackStock = 1;
                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = AddToppping.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = AddToppping.SysItemID,
                            BalanceStock = Convert.ToDecimal(Utils.CheckLenghtValue(txtStock.Text)),
                            MinimumStock = minimumStock,
                        };
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.insertdataitem), ToastLength.Long).Show();
                        return;
                    }
                }
                else
                {
                    itemOnBranch = null;
                }

                var result = await itemManage.InsertItem(AddToppping, itemOnBranch, null);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                }                

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)AddToppping.MerchantID, (int)AddToppping.SysItemID);
                }
                else
                {
                    AddToppping.FWaitSending = 2;
                    await itemManage.UpdateItem(AddToppping);
                }
                if (checkManageStock)
                {
                    DataCashingAll.flagItemOnBranchChange = true;
                }
                flagdatachange = false;
                EditStock = false;
                checkManageStock = false;
                tabSelected = string.Empty;

                ItemActivity.SetFocusNewItem(AddToppping);
                DataCashingAll.flagItemChange = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertToppping at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
            }
        }
        async Task GetExtraList()
        {

            //DialogLoading dialogLoading = new DialogLoading();
            //if (dialogLoading.Cancelable != false)
            //{
            //    dialogLoading.Cancelable = false;
            //    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            //}
            try
            {
                itemExtra = new List<Item>();
                ItemManage itemManage = new ItemManage();
                itemExtra = await itemManage.GetToppingItem();
                if (itemExtra == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    itemExtra = new List<Item>();
                }
                //if (dialogLoading != null)
                //{
                //    dialogLoading.DismissAllowingStateLoss();
                //    dialogLoading.Dismiss();
                //}
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("item_Adapter_Extra_ItemClick at Item");
                //dialogLoading.Dismiss();
            }
        }
        public async Task UpdateToppping()
        {
            try
            {
                EditToppping.MerchantID = ExtraToppping.MerchantID;
                EditToppping.SysItemID = ExtraToppping.SysItemID;
                EditToppping.ItemName = ToppingName;
                EditToppping.Ordinary = ExtraToppping.Ordinary;

                if (SyscategoryID == ExtraToppping.SysCategoryID)
                {
                    EditToppping.SysCategoryID = ExtraToppping.SysCategoryID;
                }
                else if (SyscategoryID == 0)
                {
                    EditToppping.SysCategoryID = null;
                }
                else
                {
                    EditToppping.SysCategoryID = SyscategoryID;
                }
                EditToppping.ItemCode = ExtraToppping.ItemCode;
                EditToppping.ShortName = ExtraToppping.ShortName;

                int colorItem = 0;
                if (colorSelected == "#A2A2A2" || colorSelected == "0")
                {
                    colorItem = 0;
                }
                else
                {
                    string color = colorSelected;
                    string[] scolor = color.Split("#");
                    colorItem = int.Parse(scolor[1], System.Globalization.NumberStyles.HexNumber);
                }

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResult = await Utils.InsertImageToThumbnail(path, bitmap, "item");
                    if (checkResult)
                    {
                        EditToppping.ThumbnailPath = pathThumnailFolder + path;
                        EditToppping.ThumbnailLocalPath = pathThumnailFolder + path;
                    }

                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    if (checkResultPicture)
                    {
                        EditToppping.PictureLocalPath = pathFolderPicture + path;
                    }

                    if (!string.IsNullOrEmpty(ExtraToppping.ThumbnailLocalPath))
                    {
                        Java.IO.File imgFile = new Java.IO.File(ExtraToppping.ThumbnailLocalPath);
                        if (System.IO.File.Exists(imgFile.AbsolutePath))
                        {
                            System.IO.File.Delete(imgFile.AbsolutePath);
                        }
                    }

                    EditToppping.PicturePath = ExtraToppping.PicturePath;
                    EditToppping.Colors = 0;
                }
                else
                {
                    EditToppping.ThumbnailPath = ExtraToppping.ThumbnailPath;
                    EditToppping.PicturePath = ExtraToppping.PicturePath;
                    EditToppping.PictureLocalPath = ExtraToppping.PictureLocalPath;
                    EditToppping.ThumbnailLocalPath = ExtraToppping.ThumbnailLocalPath;
                    EditToppping.Colors = ExtraToppping.Colors;
                }

                if (colorItem != ExtraToppping.Colors)
                {
                    EditToppping.Colors = colorItem;

                    //กรณีเปลี่ยนสี แต่เคยมีรูปอยู่
                    if (!string.IsNullOrEmpty(ExtraToppping.ThumbnailLocalPath))
                    {
                        Java.IO.File imgFiletThumnail = new Java.IO.File(ExtraToppping.ThumbnailLocalPath);
                        if (System.IO.File.Exists(imgFiletThumnail.AbsolutePath))
                        {
                            System.IO.File.Delete(imgFiletThumnail.AbsolutePath);
                        }
                        EditToppping.PicturePath = null;
                        EditToppping.PictureLocalPath = null;
                        EditToppping.ThumbnailPath = null;
                        EditToppping.ThumbnailLocalPath = null;
                    }
                }
                else
                {
                    EditToppping.Colors = ExtraToppping.Colors;
                }

                EditToppping.UnitName = ExtraToppping.UnitName;
                EditToppping.RegularSizeName = ExtraToppping.RegularSizeName;

                if (favorite)
                {
                    EditToppping.FavoriteNo = 1;
                }
                else
                {
                    EditToppping.FavoriteNo = 0;
                }

                var Price = textInsertPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textInsertPrice.Text;
                }
                EditToppping.Price = ConvertToDecimal(Price);

                //Topping มี cost ไหม
                if (string.IsNullOrEmpty(txtcost.Text))
                {
                    //ถ้า User ไม่กำหนด ต้นทุน ระบบจะนำเอาราคาขายที่ตั้งมาเป็นต้นทุน
                    EditToppping.EstimateCost = ConvertToDecimal(Price);
                }
                else
                {
                    var Cost = txtcost.Text.Trim();
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        Cost = txtcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        Cost = txtcost.Text;
                    }
                    EditToppping.EstimateCost = ConvertToDecimal(Cost);
                }

                EditToppping.OptSalePrice = ExtraToppping.OptSalePrice;
                EditToppping.TaxType = ExtraToppping.TaxType;
                EditToppping.SellBy = ExtraToppping.SellBy;
                if (ExtraToppping.FTrackStock != showStock || ExtraToppping.FTrackStock == 1 & EditStock)
                {
                    EditToppping.FTrackStock = showStock;
                    if (await GabanaAPI.CheckNetWork())
                    {
                        if (await GabanaAPI.CheckSpeedConnection())
                        {
                            await UpdateStock();
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                        }
                    }
                    EditToppping.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                }
                else
                {
                    EditToppping.FTrackStock = ExtraToppping.FTrackStock;
                    EditToppping.TrackStockDateTime = Utils.GetTranDate(ExtraToppping.TrackStockDateTime);
                }
                EditToppping.FDisplayOption = ExtraToppping.FDisplayOption;
                EditToppping.SaleItemType = ExtraToppping.SaleItemType;
                EditToppping.Comments = ExtraToppping.Comments;
                EditToppping.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                EditToppping.UserLastModified = usernamelogin;
                EditToppping.DataStatus = 'M';
                EditToppping.FWaitSending = 2;
                EditToppping.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);

                if (textInsertProduct.Text.Length > 5)
                {
                    EditToppping.ShortName = textInsertProduct.Text.Substring(0, 5);
                }
                else
                {
                    EditToppping.ShortName = textInsertProduct.Text;
                }

                //Check ชื่อสินค้า                 
                if (textInsertProduct.Text.Trim() != ExtraToppping.ItemName && textInsertProduct.Text.Trim() != string.Empty)
                {
                    var checkname = itemExtra.FindIndex(x => x.ItemName.Equals(EditToppping.ItemName));
                    if (checkname != -1)
                    {
                        //Toast.MakeText(this, "ชื่อสินค้านี้มีอยู่ในระบบแล้ว ต้องการจะบันทึกหรือไม่", ToastLength.Short).Show();
                        try
                        {
                            //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                            InsertRepeatItem insertRepeat = new InsertRepeatItem();
                            insertRepeat.DetailITem = EditToppping;
                            var json = JsonConvert.SerializeObject(insertRepeat);

                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("insertRepeat", "insertrepeat");
                            bundle.PutString("detailitem", EditToppping.ItemName);
                            bundle.PutString("fromPage", "itemname");
                            bundle.PutString("detailnnsert", json);
                            bundle.PutString("event", "update");
                            bundle.PutString("itemtype", "topping");
                            dialog.Arguments = bundle;
                            dialog.Show(SupportFragmentManager, myMessage);
                            return;
                        }
                        catch (Exception ex)
                        {
                            await TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("checkName Update at add Extra");
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                            return;
                        }
                    }
                }

                var result = await itemManage.UpdateItem(EditToppping);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                }                

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)EditToppping.MerchantID, (int)EditToppping.SysItemID);
                }
                else
                {
                    EditToppping.FWaitSending = 2;
                    await itemManage.UpdateItem(EditToppping);
                }
                if (EditStock)
                {
                    ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                    var updateStock = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranch);
                }
                if (EditToppping.FTrackStock != showStock | EditToppping.FTrackStock == 1 & EditStock)
                {
                    DataCashingAll.flagItemOnBranchChange = true;
                }
                flagdatachange = false;
                EditStock = false;
                checkManageStock = false;
                tabSelected = string.Empty;

                ItemActivity.SetFocusNewItem(EditToppping);
                DataCashingAll.flagItemChange = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                //fragment.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateToppping at add Extra");
                Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
            }
        }

        private async Task UpdateStock()
        {
            bool resultstock = false;
            if (showStock == 1)
            {
                //Open Stock
                if (string.IsNullOrEmpty(txtStock.Text))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }
                decimal minimumStock = 0;
                if (string.IsNullOrEmpty(txtminimumStock.Text))
                {
                    minimumStock = 0;
                }
                else
                {
                    minimumStock = Convert.ToDecimal(txtminimumStock.Text.Trim());
                }

                //(int)DataCashingAll.SysBranchId, sysitem, (int)DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)), ConvertToDecimal(Utils.CheckLenghtValue(txtminimumStock.Text))
                resultstock = await UpdateOpenStock((int)DataCashingAll.SysBranchId, (int)EditToppping.SysItemID, (int)DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)), minimumStock);
                if (!resultstock)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = EditToppping.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = EditToppping.SysItemID,
                    BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)),
                    MinimumStock = minimumStock,
                    LastDateBalanceStock = DateTime.UtcNow
                };
            }
            else
            {
                //Close Stock
                resultstock = await UpdateClosetock((int)EditToppping.SysItemID);
                if (!resultstock)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = EditToppping.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = EditToppping.SysItemID,
                    BalanceStock = 0,
                    MinimumStock = 0,
                    LastDateBalanceStock = DateTime.UtcNow
                };
            }
        }

        async void SpinnerSelectCategory()
        {
            try
            {
                CategoryManage categoryManage = new CategoryManage();

                var lstcategory = new List<Category>();
                var getallCategory = new List<Category>();
                var addcategory = new Category();

                addcategory = new Category()
                {
                    Name = "None",
                    SysCategoryID = 0
                };
                lstcategory.Add(addcategory);
                getallCategory = await categoryManage.GetAllCategory();
                lstcategory.AddRange(getallCategory);

                string[] category_array = lstcategory.Select(i => i.Name.ToString()).ToArray();
                var adapterCategory = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, category_array);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerCategory.Adapter = adapterCategory;

                long? category = ExtraToppping.SysCategoryID;
                if (category != null)
                {
                    var data = lstcategory.Where(x => x.SysCategoryID == category).FirstOrDefault();
                    if (data != null)
                    {
                        int position = adapterCategory.GetPosition(data.Name);
                        spinnerCategory.SetSelection(position);
                    }
                    else
                    {
                        int position = adapterCategory.GetPosition("None");
                        spinnerCategory.SetSelection(position);
                    }
                }
                else
                {
                    int position = adapterCategory.GetPosition("None");
                    spinnerCategory.SetSelection(position);
                }
                SyscategoryID = category == null ? 0 : Convert.ToInt32(category);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SpinnerSelectCategory at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void Txtcost_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (txtcost.Text.Length == 0)
                {
                    return;
                }

                if (txtcost.Text == ".")
                {
                    return;
                }

                string[] txt = new string[] { };
                int checkIndex = 0;
                if (txtcost.Text.Contains('.'))
                {
                    checkIndex = txtcost.Text.IndexOf('.');
                    if (checkIndex == -1)
                    {
                        return;
                    }

                    txt = txtcost.Text.Split('.');
                    if (!string.IsNullOrEmpty(txt[1]))
                    {
                        if (txt[1].Length > Convert.ToInt32(DecimalDisplay))
                        {
                            string Amount = txtcost.Text;
                            txtcost.Text = Amount.Remove(Amount.Length - 1);
                            txtcost.SetSelection(txtcost.Text.Length);
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txt[0]))
                    {
                        if ((txt[0].Length) > 13)
                        {
                            txtcost.Text = txt[0].Remove(13, 1);
                            txtcost.SetSelection(txtcost.Text.Length);
                            return;
                        }
                    }

                    var strConbine = txt[0].ToString() + (txt[1] == null ? "" : txt[1].ToString());
                }
                else
                {
                    //เพิ่มเงื่อนไขการลบ Currrency ก่อนเข้าฟังก์ชัน กัน Error
                    string cost;
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        cost = txtcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        cost = txtcost.Text;
                    }

                    if (cost.Trim().Length == 0)
                    {
                        return;
                    }
                }
                CheckDataChange();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Txtcost_TextChanged at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void Txtcost_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    //เพิ่มเงื่อนไขการลบ Currrency ก่อนเข้าฟังก์ชัน กัน Error
                    string txtCost;
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        txtCost = txtcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtCost = txtcost.Text;
                    }

                    if (txtCost.Trim().Length == 0)
                    {
                        return;
                    }
                    var Price = Convert.ToDecimal(txtCost);
                    //textCurrency.Visibility = ViewStates.Visible;
                    txtcost.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(Price);
                    txtcost.SetSelection(txtcost.Text.Length);
                }
                else
                {
                    e.Handled = false; //if you want that character appeared on the screen
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
                    txtcost.Text += input;
                    txtcost.SetSelection(txtcost.Text.Length);
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("Txtcost_KeyPress at add Extra");
            }
        }

        private void TextInsertPrice_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (textInsertPrice.Text.Length == 0)
                {
                    return;
                }

                if (textInsertPrice.Text == ".")
                {
                    return;
                }

                string[] txt = new string[] { };
                int checkIndex = 0;
                if (textInsertPrice.Text.Contains('.'))
                {
                    checkIndex = textInsertPrice.Text.IndexOf('.');
                    if (checkIndex == -1)
                    {
                        return;
                    }

                    txt = textInsertPrice.Text.Split('.');
                    if (!string.IsNullOrEmpty(txt[1]))
                    {
                        if (txt[1].Length > Convert.ToInt32(DecimalDisplay))
                        {
                            string Amount = textInsertPrice.Text;
                            textInsertPrice.Text = Amount.Remove(Amount.Length - 1);
                            textInsertPrice.SetSelection(textInsertPrice.Text.Length);
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txt[0]))
                    {
                        if ((txt[0].Length) > 13)
                        {
                            textInsertPrice.Text = txt[0].Remove(13, 1);
                            textInsertPrice.SetSelection(textInsertPrice.Text.Length);
                            return;
                        }
                    }

                    var strConbine = txt[0].ToString() + (txt[1] == null ? "" : txt[1].ToString());
                }
                else
                {
                    string maxdata;
                    if (DecimalDisplay == "4")
                    {
                        maxdata = Utils.DisplayDecimal((decimal)9999999999.9999);
                    }
                    else
                    {
                        maxdata = Utils.DisplayDecimal((decimal)9999999999.99);
                    }

                    //เพิ่มเงื่อนไขการลบ Currrency ก่อนเข้าฟังก์ชัน กัน Error
                    string txtPrice;
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        txtPrice = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtPrice = textInsertPrice.Text;
                    }

                    if (txtPrice.Trim().Length == 0)
                    {
                        return;
                    }

                    if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(txtPrice))
                    {
                        Toast.MakeText(this, GetString(Resource.String.maxamount) + " " + maxdata, ToastLength.Short).Show();
                        textInsertPrice.Text = maxdata;
                        textInsertPrice.SetSelection(textInsertPrice.Text.Length);
                        return;
                    }
                }

                SetColorItemView();
                CheckDataChange();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_TextChanged at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async void CheckDataChange()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (ExtraToppping == null)
            {
                if (switchStock.Checked)
                {
                    flagdatachange = true;
                }
                if (string.IsNullOrEmpty(textInsertProduct.Text))
                {
                    SetButtonAdd(false);
                    return;
                }
                string txtPrice;
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    txtPrice = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    txtPrice = textInsertPrice.Text;
                }
                if (string.IsNullOrEmpty(txtPrice))
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
                if (textInsertProduct.Text != ExtraToppping.ItemName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                decimal insertPrice = 0;
                string txtPrice;
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    txtPrice = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    txtPrice = textInsertPrice.Text;
                }
                decimal.TryParse(txtPrice, out insertPrice);
                if (insertPrice != (decimal)ExtraToppping.Price)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }

                string txtCost;
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    txtCost = txtcost.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    txtCost = txtcost.Text;
                }
                decimal Cost = 0;
                decimal.TryParse(txtCost, out Cost);
                if (Cost != (decimal)ExtraToppping.EstimateCost)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (keepCropedUri != null)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                string conColor = string.Empty;
                if (ExtraToppping.Colors == 0)
                {
                    conColor = "0";
                }
                else
                {
                    conColor = Utils.SetBackground(Convert.ToInt32(ExtraToppping.Colors));
                }
                if (colorSelected == "#A2A2A2" || colorSelected == null)
                {
                    colorSelected = "0";
                }
                if (colorSelected != conColor)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                int numFav = 0;
                if (favorite)
                {
                    numFav = 1;
                }
                if (numFav != ExtraToppping.FavoriteNo)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                int swStock = 0;
                if (switchStock.Checked)
                {
                    swStock = 1;
                }
                if (ExtraToppping.FTrackStock != swStock)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }

                if (CheckNet)
                {
                    if (GetDataStock != null)
                    {
                        var balancestock = ConvertToDecimal(txtStock.Text);
                        if (balancestock != GetDataStock.BalanceStock)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            EditStock = true;
                            return;
                        }
                        decimal minist;
                        decimal.TryParse(txtminimumStock.Text, out minist);
                        if (minist != GetDataStock.MinimumStock)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            EditStock = true;
                            return;
                        }
                    }
                    else
                    {
                        var balancestock = ConvertToDecimal(txtStock.Text);
                        if (balancestock != 0)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            EditStock = true;
                            return;
                        }
                        decimal minist;
                        decimal.TryParse(txtminimumStock.Text, out minist);
                        if (minist != 0)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            EditStock = true;
                            return;
                        }
                    }
                }
                else
                {
                    if (getBalance != null)
                    {
                        var balancestock = ConvertToDecimal(txtStock.Text);
                        if (balancestock != getBalance.BalanceStock)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            EditStock = true;
                            return;
                        }
                        decimal minist;
                        decimal.TryParse(txtminimumStock.Text, out minist);
                        if (minist != getBalance.MinimumStock)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            EditStock = true;
                            return;
                        }
                    }
                }
                   

                if (ExtraToppping.SysCategoryID != null && SyscategoryID != ExtraToppping.SysCategoryID)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (ExtraToppping.SysCategoryID == null && SyscategoryID != 0)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);
            }
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnadditem.SetBackgroundResource(Resource.Drawable.btnblue);
                btnadditem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnadditem.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnadditem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnadditem.Enabled = enable;
        }

        private void TextInsertPrice_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;

                    //เพิ่มเงื่อนไขการลบ Currrency ก่อนเข้าฟังก์ชัน กัน Error
                    string txtPrice;
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        txtPrice = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtPrice = textInsertPrice.Text;
                    }

                    if (txtPrice.Trim().Length == 0)
                    {
                        return;
                    }
                    var Price = Convert.ToDecimal(txtPrice);
                    //textCurrency.Visibility = ViewStates.Visible;
                    textInsertPrice.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(Price);
                    textInsertPrice.SetSelection(textInsertPrice.Text.Length);
                }
                else
                {
                    e.Handled = false; //if you want that character appeared on the screen
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
                    textInsertPrice.Text += input;
                    textInsertPrice.SetSelection(textInsertPrice.Text.Length);
                    return;
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_KeyPress at add Extra");
            }
        }

        private void TextInsertProduct_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            ToppingName = textInsertProduct.Text.Trim();
            SetColorItemView();
            CheckDataChange();
        }

        public static void SetExtraTopping(Item i)
        {
            ExtraToppping = i;
        }

        private void Btncolor9_Click(object sender, EventArgs e)
        {
            colorSelected = "#DD527E";
            SetColorItemView();
        }

        private void Btncolor8_Click(object sender, EventArgs e)
        {
            colorSelected = "#8BC34A";
            SetColorItemView();
        }

        private void Btncolor7_Click(object sender, EventArgs e)
        {
            colorSelected = "#00796B";
            SetColorItemView();
        }

        private void Btncolor6_Click(object sender, EventArgs e)
        {
            colorSelected = "#3F51B5";
            SetColorItemView();
        }

        private void Btncolor5_Click(object sender, EventArgs e)
        {
            colorSelected = "#F75600";
            SetColorItemView();
        }

        private void Btncolor4_Click(object sender, EventArgs e)
        {
            colorSelected = "#37AA52";
            SetColorItemView();
        }

        private void Btncolor3_Click(object sender, EventArgs e)
        {
            colorSelected = "#E32D49";
            SetColorItemView();
        }

        private void Btncolor2_Click(object sender, EventArgs e)
        {
            colorSelected = "#F8971D";
            SetColorItemView();
        }

        private void Btncolor1_Click(object sender, EventArgs e)
        {
            colorSelected = "#0095DA";
            SetColorItemView();
        }

        private async void SetColorItemView()
        {
            try
            {
                imageViewItem.Visibility = ViewStates.Gone;
                txtViewItemnameTitle.Visibility = ViewStates.Visible;

                if ((colorSelected != null && colorSelected != "#A2A2A2") && (colorSelected != null && colorSelected != "0"))
                {
                    btncolor1.SetBackgroundResource(Resource.Mipmap.Color1);
                    btncolor2.SetBackgroundResource(Resource.Mipmap.Color2);
                    btncolor3.SetBackgroundResource(Resource.Mipmap.Color3);
                    btncolor4.SetBackgroundResource(Resource.Mipmap.Color4);
                    btncolor5.SetBackgroundResource(Resource.Mipmap.Color5);
                    btncolor6.SetBackgroundResource(Resource.Mipmap.Color6);
                    btncolor7.SetBackgroundResource(Resource.Mipmap.Color7);
                    btncolor8.SetBackgroundResource(Resource.Mipmap.Color8);
                    btncolor9.SetBackgroundResource(Resource.Mipmap.Color9);

                    HavePicture = false;
                    keepCropedUri = null;

                    switch (colorSelected)
                    {
                        case "#0095DA":
                            btncolor1.SetBackgroundResource(Resource.Mipmap.Color1B);
                            break;
                        case "#F8971D":
                            btncolor2.SetBackgroundResource(Resource.Mipmap.Color2B);
                            break;
                        case "#E32D49":
                            btncolor3.SetBackgroundResource(Resource.Mipmap.Color3B);
                            break;
                        case "#37AA52":
                            btncolor4.SetBackgroundResource(Resource.Mipmap.Color4B);
                            break;
                        case "#F75600":
                            btncolor5.SetBackgroundResource(Resource.Mipmap.Color5B);
                            break;
                        case "#3F51B5":
                            btncolor6.SetBackgroundResource(Resource.Mipmap.Color6B);
                            break;
                        case "#00796B":
                            btncolor7.SetBackgroundResource(Resource.Mipmap.Color7B);
                            break;
                        case "#8BC34A":
                            btncolor8.SetBackgroundResource(Resource.Mipmap.Color8B);
                            break;
                        case "#DD527E":
                            btncolor9.SetBackgroundResource(Resource.Mipmap.Color9B);
                            break;
                        default:
                            break;
                    }
                    colorViewItem.SetBackgroundColor(Android.Graphics.Color.ParseColor(colorSelected));
                }

                var itemname = textInsertProduct?.Text.Trim();
                if (itemname != "")
                {
                    txtItemNamePic.Text = itemname;
                    txtViewItemnameTitle.Text = itemname;
                }
                else
                {
                    txtItemNamePic.Text = "Item Name";
                    txtViewItemnameTitle.Text = "Item Name";
                }

                var itemprice = textInsertPrice?.Text.Trim();
                if (itemprice != "" && itemprice != "." && itemprice != "0")
                {
                    string value;
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        value = itemprice.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        value = itemprice;
                    }
                    decimal numitemprice = ConvertToDecimal(value);
                    txtPricePic.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(numitemprice);
                }
                else
                {
                    txtPricePic.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(0);
                }

                SetPicture();
                CheckDataChange();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetColorItemView at add Extra");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        void SetPicture()
        {
            //ตัวแปรสำหรับเช็คว่ามีรูปไหม
            if (HavePicture)
            {
                imageViewItem.Visibility = ViewStates.Visible;
                txtViewItemnameTitle.Visibility = ViewStates.Gone;
                colorSelected = "#A2A2A2";
                var color = Android.Graphics.Color.ParseColor(colorSelected);
                colorViewItem.SetBackgroundColor(color);
            }
            CheckDataChange();
        }

        private void ImgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            try
            {
                if (ExtraToppping == null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "topping");
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
                    bundle.PutString("fromPage", "topping");
                    bundle.PutString("PassValue", ExtraToppping.SysItemID.ToString());
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

        private void Btnaddpic_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.addcustomer_dialog_addimage.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("OpenPicture", "topping");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btnaddpic_Click at add Extra");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async void GalleryOpen()
        {
            try
            {
                string action;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    action = Intent.ActionOpenDocument;
                }
                else
                {
                    action = Intent.ActionPick;
                }
                Intent GalIntent = new Intent(action, MediaStore.Images.Media.ExternalContentUri);
                GalIntent.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(GalIntent, "Select image from gallery"), GALLERY_PICTURE);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GalleryOpen at add Extra");
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        //Android.Net.Uri keepCropedUri;    // เก็บเอาไว้ใช้งานที่ OnActionResult  ของการ Crop เพราะ Androd ที่ตำกว่า Android.N จะไม่มีชื่อไฟล์กลับไป

        private async void CropImage(Android.Net.Uri imageUri)
        {
            try
            {

                Intent CropIntent = new Intent("com.android.camera.action.CROP");
                CropIntent.SetDataAndType(imageUri, "image/*");
                CropIntent.AddFlags(ActivityFlags.GrantReadUriPermission); // **** ต้อง อนุญาติให้อ่าน ได้ด้วยนะครับ สำคัญ มันจะสามารถอ่านไฟล์ที่ได้จากการ CaptureImage ได้ ****

                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 448);
                CropIntent.PutExtra("outputY", 336);
                CropIntent.PutExtra("aspectX", 4);
                CropIntent.PutExtra("aspectY", 3);
                CropIntent.PutExtra("scaleUpIfNeeded", true);
                CropIntent.PutExtra("return-data", false);

#pragma warning disable CS0618 // Type or member is obsolete
                string cropedFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                         Android.OS.Environment.DirectoryPictures,
                                                         "file_" + Guid.NewGuid().ToString() + ".jpg");
#pragma warning restore CS0618 // Type or member is obsolete
                Java.IO.File cropedFile = new Java.IO.File(cropedFilePath);
                // create new file handle to get full resolution crop
                Android.Net.Uri cropedUri;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    cropedUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", cropedFile);

                    //this is the stuff that was missing - but only if you get the uri from FileProvider
                    CropIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                    //กำหนดสิทธิให้ Inten อื่นสามารถ เขียน Uri ได้
                    List<ResolveInfo> resolvedIntentActivities = Application.Context.PackageManager.QueryIntentActivities(CropIntent, PackageInfoFlags.MatchDefaultOnly).ToList();
                    foreach (ResolveInfo resolvedIntentInfo in resolvedIntentActivities)
                    {
                        String packageName = resolvedIntentInfo.ActivityInfo.PackageName;
                        Application.Context.GrantUriPermission(packageName, cropedUri, ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    }
                }
                else
                {
                    cropedUri = Android.Net.Uri.FromFile(cropedFile);
                }
                keepCropedUri = cropedUri;  // เก็บเอาไว้ใช้งานที่ OnActionResult เพราะ Android ที่ต่ำกว่า Android.N จะ Get เอาจาก ค่าที่ส่งไปใน Functio ไม่ได้

                CropIntent.PutExtra(MediaStore.ExtraOutput, cropedUri);
                StartActivityForResult(CropIntent, CROP_REQUEST);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CropImage at add extratopping");
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        Android.Net.Uri cameraTakePictureUri;
        private List<Item> itemExtra;


        public void CameraTakePicture()
        {
            try
            {
                Intent CamIntent = new Intent(MediaStore.ActionImageCapture);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                //ถ้ากำหนดชื่อชื่อไฟล์ มันจะ Save ลงที่ไฟล์นี้แล้วส่งไปให้ OnActivityResult
                string filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                         Android.OS.Environment.DirectoryPictures,
                                                         "file_" + Guid.NewGuid().ToString() + ".jpg");
                Java.IO.File tempFile = new Java.IO.File(filePath);
                Android.Net.Uri tempURI;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tempURI = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", tempFile);

                    //this is the stuff that was missing - but only if you get the uri from FileProvider
                    CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                }
                else
                {
                    tempURI = Android.Net.Uri.FromFile(tempFile);
                }
                cameraTakePictureUri = tempURI;
                CamIntent.PutExtra(MediaStore.ExtraOutput, tempURI);
                CamIntent.PutExtra("return-data", false);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                StartActivityForResult(CamIntent, CAMERA_REQUEST);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Take Picture at add extratopping");
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }


        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == CAMERA_REQUEST && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // Solution 1 : เอาชื่อไฟล์ที่ได้ส่งไป crop
                    CropImage(cameraTakePictureUri);

                    // Solution 2 : เอา Data ที่เป็น Bitmap Save ลง Temp โรสำ แล้ว ชื่อไฟล์ที่ได้ส่งไป crop
                    //            : แบบนี้ ภาพไม่ชัด
                    //Bundle bundle = data.Extras;
                    //Bitmap bitmap = (Bitmap)bundle.GetParcelable("data");
                }
                else if (requestCode == GALLERY_PICTURE && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // มาจาก User เลื่อกรุปจาก Gallory : (case นี้จะมี uri)
                    if (data != null)
                    {
                        Android.Net.Uri selectPictureUri = data.Data;
                        CropImage(selectPictureUri);
                    }
                    else
                    {
                        Toast.MakeText(this, "error : GALLERY_PICTURE data is null", ToastLength.Short).Show();
                        return;
                    }
                }
                else if (requestCode == CROP_REQUEST && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // มาจาก User เลื่อกถ่ายรูป หรือ เลื่อกรุปจาก Gallory แล้วผ่าน function CropImage();
                    if (data != null)
                    {

                        Bundle bundle = data.Extras;

                        // Solution 1 : เอาค่า BitMap มาจัดการเลย (ok) แต่ใช้กับ Android 10 ไม่ได้ครับ
                        //Bitmap bitmap = (Bitmap)bundle.GetParcelable("data");

                        // Solution 2 : อ่าน BitMap จากไฟล์ (ok)
                        Android.Net.Uri cropImageURI = keepCropedUri;
#pragma warning disable CS0618 // Type or member is obsolete
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
#pragma warning restore CS0618 // Type or member is obsolete

                        // Solution 3 : อ่าน BitMap จากไฟล์ โดยเอาค่าไฟล์จาก bundle.GetParcelable(MediaStore.ExtraOutput) : จะ error กับ Andord ที่ต่ำกว่า Android.N
                        //Android.Net.Uri cropImageURI = (Android.Net.Uri)bundle.GetParcelable(MediaStore.ExtraOutput); // ใช้กับ Andord ที่ต่ำกว่า Android.N ไม่ได้ เพราะจะมีค่าเป็น Null
                        //Bitmap bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        imageViewItem.SetImageBitmap(bitmap);
                        HavePicture = true;
                        SetPicture();
                    }
                    else
                    {
                        Toast.MakeText(this, "error : CROP_REQUEST data is null", ToastLength.Short).Show();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at add Extra");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private void SetTabMenu()
        {
            MenuTab = new List<MenuTab>
            {
                new MenuTab() { NameMenuEn = "Extra Topping" , NameMenuTh = "ท็อปปิ้งเสริม" },
                new MenuTab() { NameMenuEn = "Stock" , NameMenuTh = "สต็อก" }
            };
        }

        private void SetTabShowMenu()
        {
            if (tabSelected == "")
            {
                tabSelected = "Extra Topping";
            }

            GridLayoutManager menuLayoutManager = new GridLayoutManager(this, 2, 1, false);
            recyclerHeaderItem.HasFixedSize = true;
            recyclerHeaderItem.SetLayoutManager(menuLayoutManager);
            AddItem_Adapter_Header additem_Adapter_Header = new AddItem_Adapter_Header(MenuTab, "extra");
            recyclerHeaderItem.SetAdapter(additem_Adapter_Header);
            additem_Adapter_Header.ItemClick += AddItem_Adapter_Header_ItemClick;
            linearShowProduct.Visibility = ViewStates.Gone;
            switch (tabSelected)
            {
                case "Extra Topping":
                    linearShowProduct.Visibility = ViewStates.Visible;
                    break;
                case "Stock":
                    linearShowStock.Visibility = ViewStates.Visible;
                    if (GetDataStock == null & CheckNet | ExtraToppping?.FTrackStock == 0 | getBalance == null & !CheckNet)
                    {
                        lnSwithStcok.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lnSwithStcok.Visibility = ViewStates.Visible;
                    }

                    if (checkManageStock)
                    {
                        lnSwithStcok.Visibility = ViewStates.Visible;
                    }

                    //กรณีที่เปลี่ยนสาขาแล้วสินค้าไม่ได้ตั้งสต๊อกที่สาขาใหม่ ให้แสดง balance และ minimum เป็น 0 เงื่อนไข คือ Ftrackstock = 1 และ DataStock ,getBalance  = null
                    if ((ExtraToppping?.FTrackStock == 1 && GetDataStock == null & getBalance == null) && !EditStock)
                    {
                        lnSwithStcok.Visibility = ViewStates.Visible;
                        txtStock.Text = "0";
                        txtminimumStock.Text = "0";
                        switchStock.Checked = true;
                    }

                    offline = true;
                    if (offline & !CheckNet)
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.offline_dialog_main.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("stockoffline", "stockoffline");
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                    }
                    break;
                default:
                    linearShowProduct.Visibility = ViewStates.Gone;
                    linearShowStock.Visibility = ViewStates.Gone;
                    break;
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async void AddItem_Adapter_Header_ItemClick(object sender, int e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (ExtraToppping == null & !CheckNet)
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.offline_dialog_main.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("InsertOffline", "InsertOffline");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
                return;
            }

            tabSelected = MenuTab[e].NameMenuEn;
            SetTabShowMenu();
            SetUIFromMainRole(LoginType);
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                CheckNet = await GabanaAPI.CheckSpeedConnection();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }

        async Task GetStockData()
        {
            //DialogLoading dialogLoading = new DialogLoading();
            //if (dialogLoading.Cancelable != false)
            //{
            //    dialogLoading.Cancelable = false;
            //    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            //}
            try
            {               
                ItemOnBranchManage onBranchManage = new ItemOnBranchManage();                
                //BalanceStock กับ MinimumStock
                if (SysItemId != 0 & CheckNet)
                {
                    GetDataStock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, (int)SysItemId);
                    if (GetDataStock != null)
                    {
                        ItemOnBranch itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = GetDataStock.MerchantID,
                            SysBranchID = GetDataStock.SysBranchID,
                            SysItemID = GetDataStock.SysItemID,
                            BalanceStock = GetDataStock.BalanceStock,
                            MinimumStock = GetDataStock.MinimumStock
                        };
                        var insert = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranch);

                        txtStock.Text = GetDataStock.BalanceStock.ToString("#,###");
                        txtminimumStock.Text = GetDataStock.MinimumStock.ToString("#,##0");

                        if (ExtraToppping.FTrackStock == 0)
                        {
                            switchStock.Checked = false;
                            lnSwithStcok.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            switchStock.Checked = true;
                            lnSwithStcok.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        lnSwithStcok.Visibility = ViewStates.Gone;
                    }
                }
                else if (SysItemId != 0 & !CheckNet)
                {
                    getBalance = await onBranchManage.GetItemOnBranch((int)DataCashingAll.MerchantId, (int)DataCashingAll.SysBranchId, (int)SysItemId);
                    if (getBalance != null)
                    {
                        txtStock.Text = getBalance.BalanceStock.ToString();
                        txtminimumStock.Text = getBalance.MinimumStock.ToString("#,##0");

                        if (ExtraToppping.FTrackStock == 0)
                        {
                            switchStock.Checked = false;
                            lnSwithStcok.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            switchStock.Checked = true;
                            lnSwithStcok.Visibility = ViewStates.Visible;
                            linearShowStock.Visibility = ViewStates.Visible;
                            linearShowStock.Enabled = false;
                            lnSwithStcok.Enabled = false;              
                            switchStock.Enabled = false;
                            lnOnhand.Enabled = false;
                            txtminimumStock.Enabled = false;
                            lnStockMoveMent.Enabled = false;
                        }
                    }
                    else
                    {
                        lnSwithStcok.Visibility = ViewStates.Gone;
                    }
                }
                else //SysItemId == 0 
                {
                    lnSwithStcok.Visibility = ViewStates.Gone;
                }

                CheckDataChange();
                //if (dialogLoading != null)
                //{
                //    dialogLoading.DismissAllowingStateLoss();
                //    dialogLoading.Dismiss();
                //}
            }
            catch (Exception ex)
            {
                //dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetStockData at add Extra");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void SwitchStock_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (await GabanaAPI.CheckNetWork())
            {
                if (await GabanaAPI.CheckSpeedConnection())
                {
                    if (switchStock.Checked)
                    {
                        //open stock
                        showStock = 1;
                        lnSwithStcok.Visibility = ViewStates.Visible;
                        checkManageStock = true;
                    }
                    else
                    {
                        //close
                        showStock = 0;
                        lnSwithStcok.Visibility = ViewStates.Gone;
                        checkManageStock = true;
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    switchStock.Enabled = false;
                }
            }
            else
            {
                //Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                switchStock.Enabled = false;
            }
            CheckDataChange();
        }

        async Task<bool> UpdateOpenStock(int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock)
        {
            //Post/Open การเปิดระบบ Track Stock
            var PostDataTrackStockOpen = await GabanaAPI.PostDataTrackStockOpen(sysItemID, deviceNo);
            if (PostDataTrackStockOpen.Status)
            {
                //Post/Adjust เป็นการ update BalanceStock หรือ MinimumStock
                var PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(sysBranchID, sysItemID, deviceNo, balanceStock, minimumStock);
                if (PostDataTrackStockAdjust.Status)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if ("Item has stock tracking already." == PostDataTrackStockOpen.Message)
            {
                //Post/Adjust เป็นการ update BalanceStock หรือ MinimumStock
                var PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(sysBranchID, sysItemID, deviceNo, balanceStock, minimumStock);
                if (PostDataTrackStockAdjust.Status)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Toast.MakeText(this, PostDataTrackStockOpen.Message, ToastLength.Long).Show();
                return false;
            }
        }

        async Task<bool> UpdateClosetock(int sysitem)
        {
            //Post/Close เป็นการปิดระบบ Track Stock
            var PostDataTrackStockClose = await GabanaAPI.PostDataTrackStockClose(sysitem, (int)DataCashingAll.DeviceNo);
            if (PostDataTrackStockClose.Status)
            {
                lnSwithStcok.Visibility = ViewStates.Gone;
                return true;
            }
            else
            {
                Toast.MakeText(this, PostDataTrackStockClose.Message, ToastLength.Long).Show();
                return false;
            }
        }

        decimal ConvertToDecimal(string txt)
        {
            decimal decimalValue = 0;
            decimal.TryParse(txt, out decimalValue);
            return decimalValue;
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            tabSelected = "Extra Topping";
            offline = false;
            checkManageStock = false;
            EditStock = false;
            flagdatachange = false;
        }

        public void CheckPermission()
        {
            string[] PERMISSIONS;


            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                PERMISSIONS = new string[]
                {
                    "android.permission.POST_NOTIFICATION",
                    "android.permission.READ_MEDIA_IMAGES",
                    "android.permission.CAMERA",
                    "android.permission.ACTION_OPEN_DOCUMENT",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT",
                    "android.permission.BLUETOOTH_SCAN",
                    "android.permission.INTERNET",
                    "android.permission.LOCATION_HARDWARE",
                    "android.permission.ACCESS_LOCATION_EXTRA_COMMANDS",
                    "android.permission.ACCESS_MOCK_LOCATION",
                    "android.permission.ACCESS_NETWORK_STATE",
                    "android.permission.ACCESS_WIFI_STATE",
                    "android.permission.INTERNET",

                };
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                PERMISSIONS = new string[]
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT"
                };
            }
            else
            {
                PERMISSIONS = new string[]
               {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH"
                };
            }

            int RequestLocationId = 0;

            foreach (var item in PERMISSIONS)
            {
                if (CheckSelfPermission(item) != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, RequestLocationId);
                }
                ShouldShowRequestPermissionRationale(item);
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