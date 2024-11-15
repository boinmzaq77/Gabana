using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddItemActivity : AppCompatActivity
    {
        public static AddItemActivity createItem;
        LinearLayout linearShowProduct, linearShowStock, lnSwithStcok;
        ImageButton imgBack;
        internal Button btnAdditem;
        ImageView imageViewItem, colorViewItem, imgFavorite;
        List<String> itemID;
        public long SysItemId;
        long SyscategoryID;
        public static long SyscategoryIDfromPOS;
        char TaxType;
        string colorSelected, LoginType;
        ImageButton btncolor1, btncolor2, btncolor3, btncolor4, btncolor5, btncolor6, btncolor7, btncolor8, btncolor9, btnaddpic;
        EditText textInsertProduct, textInsertPrice, txtcost, txtMinimumStock, txtCodeitem, txtViewItemnameTitle;
        TextView txtItemNamePic, txtPricePic, txtTitle, txtStock;
        LinearLayout lnDetails, lnExtrasize, lnCategory, lnVat, lnOnhand, lnStockMoveMent;
        ImageButton btnShowDetail;
        public static Switch switchShowDisplay, switchStock;
        Spinner spinnerVat, spinnerCategory;
        private static Button btnAddSize;
        bool showdetail, HavePicture = false;
        public static bool checkManageStock = false;
        public static bool favorite, favoritefromPOS;
        FrameLayout btnDelete;
        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        ItemManage ItemManage = new ItemManage();
        ItemExSizeManage ItemExSizeManage = new ItemExSizeManage();
        List<ItemExSize> lstExSize = new List<ItemExSize>();
        List<ItemExSize> newlsItemExSize = new List<ItemExSize>();
        List<ItemExSize> TemplsItemExSize = new List<ItemExSize>();
        public List<ItemExSize> itemExSizes = new List<ItemExSize>();
        Gabana.ORM.MerchantDB.Item addItem = new Gabana.ORM.MerchantDB.Item();
        Gabana.ORM.MerchantDB.Item editItem = new Gabana.ORM.MerchantDB.Item();
        string sys, usernamelogin;
        List<ItemExSize> lstUpdateExSize = new List<ItemExSize>(); //ข้อมูลใหม่จากผู้ใช้  
        public static RecyclerView recyclerViewSize, recyclerHeaderItem;
        Android.Net.Uri keepCropedUri;
        Android.Graphics.Bitmap bitmap;
        string path, DecimalDisplay;
        public static string tabSelected;
        public List<MenuTab> MenuTab { get; set; }
        decimal showDisplay, showStock;
        string CURRENCYSYMBOLS;
        public static Item itemEdit;
        ItemOnBranch itemOnBranch, DataStock, getBalance;
        static string stockOnhabd;
        string pathThumnailFolder, pathFolderPicture;
        static bool EditStock = false;
        bool first = true, offline = false, flagdatachange = false, CheckNet = false;
        private static LinearLayout lnAddsize;
        FrameLayout btnFavorite;
        Button btnCategory, btnVat;
        ImageButton btnStockMovement;
        FrameLayout lnScanItem;
        ImageView imgScanItem;
        private List<Item> items;
        DialogLoading dialogLoading = new DialogLoading();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.additem_activity);
                createItem = this;
                recyclerHeaderItem = FindViewById<RecyclerView>(Resource.Id.recyclerHeaderItem);
                linearShowProduct = FindViewById<LinearLayout>(Resource.Id.lnShowProduct);
                linearShowStock = FindViewById<LinearLayout>(Resource.Id.lnShowStock);
                imgBack = FindViewById<ImageButton>(Resource.Id.imgBack);
                lnAddsize = FindViewById<LinearLayout>(Resource.Id.lnAddsize);
                lnAddsize.Click += BtnAddSize_Click;
                lnDetails = FindViewById<LinearLayout>(Resource.Id.lnDetails);
                lnDetails.Visibility = ViewStates.Visible;
                lnExtrasize = FindViewById<LinearLayout>(Resource.Id.lnExtrasize);
                lnVat = FindViewById<LinearLayout>(Resource.Id.lnVat);
                lnCategory = FindViewById<LinearLayout>(Resource.Id.lnCategory);
                btnCategory = FindViewById<Button>(Resource.Id.btnCategory);
                btnVat = FindViewById<Button>(Resource.Id.btnVat);
                txtCodeitem = FindViewById<EditText>(Resource.Id.txtCodeitem);
                txtCodeitem.TextChanged += TxtCodeitem_TextChanged;
                btnAdditem = FindViewById<Button>(Resource.Id.btnAdditem);
                imageViewItem = FindViewById<ImageView>(Resource.Id.imageViewItem);
                imageViewItem.Click += ImageViewItem_Click;
                colorViewItem = FindViewById<ImageView>(Resource.Id.colorViewItem);
                txtItemNamePic = FindViewById<TextView>(Resource.Id.txtItemNamePic);
                txtPricePic = FindViewById<TextView>(Resource.Id.txtPricePic);
                txtPricePic.Hint = Utils.DisplayDecimal(0);
                txtViewItemnameTitle = FindViewById<EditText>(Resource.Id.txtViewItemnameTitle);
                txtcost = FindViewById<EditText>(Resource.Id.txtcost);
                txtcost.TextChanged += Txtcost_TextChanged;
                txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
                textInsertProduct = FindViewById<EditText>(Resource.Id.textInsertProduct);
                textInsertPrice = FindViewById<EditText>(Resource.Id.textInsertPrice);
                switchShowDisplay = FindViewById<Switch>(Resource.Id.switchShowDisplay);
                spinnerVat = FindViewById<Spinner>(Resource.Id.spinnerVat);
                spinnerCategory = FindViewById<Spinner>(Resource.Id.spinnerCategory);
                btnAddSize = FindViewById<Button>(Resource.Id.btnAddSize);
                btnAddSize.Click += BtnAddSize_Click;
                recyclerViewSize = FindViewById<RecyclerView>(Resource.Id.recyclerViewSize);
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
                FrameLayout lnBtnShowDetail = FindViewById<FrameLayout>(Resource.Id.lnBtnShowDetail);
                lnBtnShowDetail.Click += BtnShowDetail_Click;
                FrameLayout lnBack = FindViewById<FrameLayout>(Resource.Id.lnBack);
                lnBack.Click += ImgBack_Click;
                imgBack.Click += ImgBack_Click;
                btnFavorite = FindViewById<FrameLayout>(Resource.Id.btnFavorite);
                imgFavorite = FindViewById<ImageView>(Resource.Id.imgFavorite);
                btnStockMovement = FindViewById<ImageButton>(Resource.Id.btnStockMovement);
                lnScanItem = FindViewById<FrameLayout>(Resource.Id.lnScanItem);
                imgScanItem = FindViewById<ImageView>(Resource.Id.imgScanItem);
                lnScanItem.Click += LnScanItem_Click;
                if (favoritefromPOS)
                {
                    favorite = true;
                }
                else
                {
                    favorite = false;
                }

                btnFavorite.Click += BtnFavorite_Click;
                btnDelete = FindViewById<FrameLayout>(Resource.Id.btnDelete);
                btnDelete.Click += BtnDelete_Click;
                lnCategory.Click += LnCategory_Click;
                lnVat.Click += LnVat_Click;
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

                textInsertProduct.TextChanged += TextInsertProduct_TextChanged;
                textInsertPrice.TextChanged += TextInsertPrice_TextChanged;
                textInsertPrice.KeyPress += TextInsertPrice_KeyPress;
                txtcost.Hint = Utils.DisplayDecimal(0);
                txtcost.KeyPress += Txtcost_KeyPress;
                switchShowDisplay.CheckedChange += SwitchShowDisplay_CheckedChange;
                switchStock = FindViewById<Switch>(Resource.Id.switchStock);
                txtStock = FindViewById<TextView>(Resource.Id.txtStock);
                txtStock.TextChanged += TxtStock_TextChanged;
                txtStock.Click += TxtStock_Click;
                txtMinimumStock = FindViewById<EditText>(Resource.Id.txtMinimumStock);
                txtMinimumStock.TextChanged += TxtMinimumStock_TextChanged;
                txtMinimumStock.FocusChange += TxtMinimumStock_FocusChange;

                lnSwithStcok = FindViewById<LinearLayout>(Resource.Id.lnSwithStcok);
                switchStock.CheckedChange += SwitchStock_CheckedChange;
                lnOnhand = FindViewById<LinearLayout>(Resource.Id.lnOnhand);
                lnOnhand.Click += LnOnhand_Click;
                lnStockMoveMent = FindViewById<LinearLayout>(Resource.Id.lnStockMoveMent);
                lnStockMoveMent.Click += LnStockMoveMent_Click;
                txtViewItemnameTitle.ClearFocus();
                txtViewItemnameTitle.TextChanged += TxtViewItemnameTitle_TextChanged;

                linearShowProduct.Visibility = ViewStates.Visible;
                linearShowStock.Visibility = ViewStates.Gone;

                spinnerVat.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerVat_ItemSelected);
                var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.spinnervat, Resource.Layout.spinner_item);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerVat.Adapter = adapter;

                usernamelogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");

                if (dialogLoading != null & dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                SetItemView();

                #region Data
                //เพิ่มมาเนื่องจาก มือถือบางเครื่อง ถ้าไปเลือกรูปแล้ว จะเคลียร์หน้าออกทำให้ข้อมูลของ Datacashing หายไป
                //Utils.AddNullValue();

                CheckJwt();
                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
                textInsertPrice.Hint = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(0);

                pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                pathFolderPicture = DataCashingAll.PathFolderImage;

                CheckPermission();
                SetTextColor();
                SpinnerCategory();

                CheckNet = await GabanaAPI.CheckSpeedConnection();

                DecimalDisplay = DataCashingAll.setmerchantConfig?.DECIMAL_POINT_DISPLAY;
                if (DecimalDisplay == null) DecimalDisplay = "2";
                //DataCashing.EditItemID = Preferences.Get("EditItemID", DataCashing.EditItemID);
                if (DataCashing.EditItemID == 0)
                {
                    txtTitle.Text = GetString(Resource.String.additem_activity_title);
                    btnAdditem.Text = GetString(Resource.String.textsave);
                    colorSelected = "#0095DA";
                    SysItemId = 0;
                    btnAdditem.Click += BtnAdditem_Click;
                    txtItemNamePic.Text = "Item Name";
                    txtViewItemnameTitle.Text = "Item Name";
                    SetItemView();
                    txtStock.Text = "0";
                    txtMinimumStock.Text = "0";
                }
                else
                {
                    txtTitle.Text = GetString(Resource.String.edititem_activity_title);
                    btnAdditem.Text = GetString(Resource.String.edititem_activity_title);
                    SysItemId = DataCashing.EditItemID;
                    await ShowItemForEdit();
                    await GetStockData();
                    btnAdditem.Click += BtnEditProduct_Click;
                }

                await GetItemList();
                SetTabMenu();
                SetTabShowMenu();

                await ShowItemExSize();
                SetFavorite();
                ShowDetailItem();
                #endregion

                first = false;
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddItemActivity");
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Log.Debug("stateStep", "AddItem" + "OnCreate" + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at add Item");
                Toast.MakeText(this, "oncreate add item " + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtMinimumStock_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                //เคส ถ้า minimum = 0 ให้ลบ 0 ออกก่อนจะกดตัวเลขอื่น
                if (e.HasFocus)
                {
                    if (txtMinimumStock.Text.Length == 1 && txtMinimumStock.Text == "0")
                    {
                        txtMinimumStock.Text = String.Empty;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        internal static void SetItemCode(string itemCodeResult)
        {
            createItem.txtCodeitem.Text = itemCodeResult;
        }

        private void LnScanItem_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(ItemCodeScanActivity)));
        }

        private void SetUIFromMainRole(string loginType)
        {
            var checkRole = UtilsAll.CheckPermissionRoleUser(loginType, "insert", "item");
            if (checkRole && CheckNet)
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
                txtCodeitem.Enabled = true;
                txtCodeitem.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtCodeitem.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcost.Enabled = true;
                txtcost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                spinnerCategory.Enabled = true;
                lnCategory.Enabled = true;
                btnCategory.SetBackgroundResource(Resource.Mipmap.Next);
                spinnerVat.Enabled = true;
                btnVat.SetBackgroundResource(Resource.Mipmap.Next);
                lnVat.Enabled = true;
                btnAddSize.Enabled = true;
                btnAddSize.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAddSize.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
                switchShowDisplay.Enabled = true;
                switchStock.Enabled = true;
                txtStock.Enabled = true;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtMinimumStock.Enabled = true;
                txtMinimumStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtMinimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAdditem.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                imgScanItem.SetBackgroundResource(Resource.Mipmap.ScanItem);
                lnScanItem.Enabled = true;
            }
            else if (checkRole && !CheckNet)
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
                txtCodeitem.Enabled = true;
                txtCodeitem.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtCodeitem.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcost.Enabled = true;
                txtcost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                spinnerCategory.Enabled = true;
                lnCategory.Enabled = true;
                btnCategory.SetBackgroundResource(Resource.Mipmap.Next);
                spinnerVat.Enabled = true;
                btnVat.SetBackgroundResource(Resource.Mipmap.Next);
                lnVat.Enabled = true;
                btnAddSize.Enabled = true;
                btnAddSize.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAddSize.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
                switchShowDisplay.Enabled = true;
                switchStock.Enabled = false;
                txtStock.Enabled = false;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtMinimumStock.Enabled = false;
                txtMinimumStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtMinimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                //btnAdditem.SetBackgroundResource(Resource.Drawable.btnborderblue);
                //btnAdditem.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                imgScanItem.SetBackgroundResource(Resource.Mipmap.ScanItem);
                lnScanItem.Enabled = true;
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
                txtCodeitem.Enabled = false;
                txtCodeitem.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtCodeitem.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtcost.Enabled = false;
                txtcost.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtcost.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                spinnerCategory.Enabled = false;
                lnCategory.Enabled = false;
                btnCategory.SetBackgroundResource(Resource.Mipmap.NextG);
                spinnerVat.Enabled = false;
                btnVat.SetBackgroundResource(Resource.Mipmap.NextG);
                lnVat.Enabled = false;
                btnAddSize.Enabled = false;
                btnAddSize.SetBackgroundResource(Resource.Drawable.btnbordergray);
                btnAddSize.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                switchShowDisplay.Enabled = false;
                switchStock.Enabled = false;
                txtStock.Enabled = false;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtMinimumStock.Enabled = false;
                txtMinimumStock.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtMinimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnbordergray);
                btnAdditem.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                imgScanItem.SetBackgroundResource(Resource.Mipmap.ScanCodeG);
                lnScanItem.Enabled = false;
            }


            checkRole = UtilsAll.CheckPermissionRoleUser(loginType, "delete", "item");
            if (checkRole)
            {
                btnDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                btnDelete.Visibility = ViewStates.Gone;

            }
        }

        private void TxtStock_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(OnhandActivity)));
            OnhandActivity.SetPageView("Additem", txtStock.Text);
        }

        private void TxtViewItemnameTitle_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void ImageViewItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = "";
                if (itemEdit != null)
                {
                    //MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.dialog_item.ToString();
                    bundle.PutString("message", myMessage);
                    if (!string.IsNullOrEmpty(itemEdit.PicturePath))
                    {
                        bundle.PutString("OpenCloudPicture", itemEdit.PicturePath);
                        path = itemEdit.PicturePath;
                    }
                    else
                    {
                        bundle.PutString("OpenCloudPicture", itemEdit.PictureLocalPath);
                        path = itemEdit.PictureLocalPath;
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
                _ = TinyInsights.TrackPageViewAsync("ImgProfile_Click at add add Item");
                Toast.MakeText(this, "ImageViewItem_Click" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtMinimumStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                int max = 999999;
                var remove = Utils.CheckLenghtValue(txtMinimumStock.Text);
                int.TryParse(remove, out int value);
                //int value = string.IsNullOrEmpty(remove) ? 0 : int.Parse(remove);
                if (max < value)
                {
                    Toast.MakeText(this, GetString(Resource.String.maxitem) + " " + max.ToString("#,###"), ToastLength.Short).Show();
                    txtMinimumStock.Text = max.ToString("#,###");
                    txtMinimumStock.SetSelection(txtMinimumStock.Text.Length);
                    CheckDataChange();
                    return;
                }
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void TxtStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void Txtcost_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
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
                Toast.MakeText(this, "Txtcost_TextChanged" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Txtcost_TextChanged at Add Item");
            }
        }

        string itemcode = "";
        private void TxtCodeitem_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodeitem.Text))
            {
                CheckDataChange();
                return;
            }

            if (Regex.IsMatch(txtCodeitem.Text, @"^[\u0E00-\u0E7Fa-zA-Z0-9']+$"))
            {
                itemcode = txtCodeitem.Text;
            }
            else
            {
                txtCodeitem.Text = itemcode;
                Toast.MakeText(this, "สามารถกรอกได้เฉพาะตัวภาษาอังกฤษหรือภาษาไทยหรือตัวเลขได้", ToastLength.Short).Show();
                return;
            }
            txtCodeitem.SetSelection(txtCodeitem.Text.Length);
            CheckDataChange();
        }

        public void CheckDataChange()
        {
            try
            {
                if (first)
                {
                    SetButtonAdd(false);
                    return;
                }
                if ((DataCashing.EditItemID == 0))
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
                    //itemEdit;
                    if (string.IsNullOrEmpty(textInsertProduct.Text) && string.IsNullOrEmpty(textInsertPrice.Text))
                    {
                        SetButtonAdd(false);
                        return;
                    }
                    if (!string.IsNullOrEmpty(textInsertProduct.Text))
                    {
                        if (textInsertProduct.Text != itemEdit.ItemName)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            return;
                        }
                        if (txtViewItemnameTitle.Text != itemEdit.ShortName)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            return;
                        }
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
                    if (insertPrice != (decimal)itemEdit.Price)
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
                    if (itemEdit.Colors == 0)
                    {
                        conColor = "0";
                    }
                    else
                    {
                        conColor = Utils.SetBackground(Convert.ToInt32(itemEdit.Colors));
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
                    if (numFav != itemEdit.FavoriteNo)
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
                    if (itemEdit.FTrackStock != swStock)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (CheckNet)
                    {
                        if (DataStock != null)
                        {
                            var balancestock = ConvertToDecimal(txtStock.Text);
                            if (balancestock != DataStock.BalanceStock)
                            {
                                SetButtonAdd(true);
                                flagdatachange = true;
                                EditStock = true;
                                return;
                            }
                            decimal minist;
                            decimal.TryParse(txtMinimumStock.Text, out minist);
                            if (minist != DataStock.MinimumStock)
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
                            decimal.TryParse(txtMinimumStock.Text, out minist);
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
                            decimal.TryParse(txtMinimumStock.Text, out minist);
                            if (minist != getBalance.MinimumStock)
                            {
                                SetButtonAdd(true);
                                flagdatachange = true;
                                EditStock = true;
                                return;
                            }
                        }
                    }

                    int catid;
                    int.TryParse(itemEdit.SysCategoryID.ToString(), out catid);
                    if ((int)SyscategoryID != catid)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    int itemid;
                    int.TryParse(itemEdit.SysItemID.ToString(), out itemid);
                    if ((int)SysItemId != itemid)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    string ItemEdit = string.IsNullOrEmpty(itemEdit.ItemCode) ? "" : itemEdit.ItemCode;
                    string ItemCodeitem = string.IsNullOrEmpty(txtCodeitem.Text) ? "" : txtCodeitem.Text;
                    if (ItemEdit != ItemCodeitem)
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
                    if (Cost != (decimal)itemEdit.EstimateCost)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    if (showDisplay != itemEdit.FDisplayOption)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (TaxType != itemEdit.TaxType)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    if (lstExSize.Count > 0)
                    {
                        switchShowDisplay.Enabled = false;
                        switchShowDisplay.Checked = true;
                    }

                    //เพิ่มการเช็ค ถ้ามีการเปลี่ยนแปลงชื่อของขนาด
                    //true คือ เหมือนเดิม
                    HashSet<decimal> Price = new HashSet<decimal>(TemplsItemExSize.Select(s => s.Price));
                    var results = newlsItemExSize.Where(m => !Price.Contains(m.Price)).ToList();

                    HashSet<decimal> EstimateCost = new HashSet<decimal>(TemplsItemExSize.Select(s => s.EstimateCost));
                    var results2 = newlsItemExSize.Where(m => !EstimateCost.Contains(m.EstimateCost)).ToList();

                    HashSet<string> ExSizeName = new HashSet<string>(TemplsItemExSize.Select(s => s.ExSizeName));
                    var results3 = newlsItemExSize.Where(m => !ExSizeName.Contains(m.ExSizeName)).ToList();

                    if (results.Count > 0 || results2.Count > 0 || results3.Count > 0)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    if (!lstExSize.SequenceEqual(newlsItemExSize))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    SetButtonAdd(false);
                    SetUIFromMainRole(LoginType);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "CheckDataChange " + ex.Message);
            }
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAdditem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAdditem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnAdditem.Enabled = enable;
        }

        private void LnStockMoveMent_Click(object sender, EventArgs e)
        {
            if (SysItemId != 0)
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(StockMoveMentActivity)));
                StockMoveMentActivity.SetPageView(itemEdit);
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notdata), ToastLength.Short).Show();
            }
        }

        internal static void SetOnhand(string text)
        {
            createItem.txtStock.Text = text;
            stockOnhabd = text;
        }

        private void LnOnhand_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(OnhandActivity)));
            OnhandActivity.SetPageView("Additem", txtStock.Text);
        }

        private async void SwitchStock_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            //check switch stock ว่า true ไหม ถ้า true จะไม่สามารถใช้งานปุ่มได้            
            // var Nosizename = newlsItemExSize.Where(x=> string.IsNullOrEmpty(x.ExSizeName)).ToList();
            //if ((recyclerViewSize.ChildCount > 0) && (Nosizename.Count > 0))
            if ((recyclerViewSize.ChildCount > 0) && (newlsItemExSize.Count > 0))
            {
                Toast.MakeText(this, "สินค้ามีขนาดไม่สามารถเพิ่มสต๊อกได้", ToastLength.Short).Show();
                lnSwithStcok.Visibility = ViewStates.Gone;
                switchStock.Checked = false;
                return;
            }

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
                        checkManageStock = false;
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

        private void LnVat_Click(object sender, EventArgs e)
        {
            try
            {
                spinnerVat.PerformClick();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnVat_Click at add Item");
                Toast.MakeText(this, "LnVat_Click" + ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnCategory_Click(object sender, EventArgs e)
        {
            try
            {
                spinnerCategory.PerformClick();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnCategory_Click at add Item");
                Toast.MakeText(this, "LnCategory_Click" + ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string Role = LoginType;
                bool check = UtilsAll.CheckPermissionRoleUser(Role, "delete", "item");
                if (check)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutInt("systemID", (int)SysItemId);
                    bundle.PutString("deleteType", "item");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at add Item");
                Toast.MakeText(this, "BtnDelete_Click" + ex.Message, ToastLength.Short).Show();
                return;
            }
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

        private void SwitchShowDisplay_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (switchShowDisplay.Checked)
            {
                showDisplay = 1;
            }
            else
            {
                showDisplay = 0;
            }

            CheckDataChange();
        }

        private async void BtnAdditem_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdditem.Enabled = false;
                if (string.IsNullOrEmpty(textInsertProduct.Text))
                {
                    Toast.MakeText(this, GetString(Resource.String.inputname), ToastLength.Short).Show();
                    btnAdditem.Enabled = true;
                    return;
                }

                if (string.IsNullOrEmpty(textInsertPrice.Text))
                {
                    Toast.MakeText(this, GetString(Resource.String.inputprice), ToastLength.Short).Show();
                    btnAdditem.Enabled = true;
                    return;
                }

                //limit Item != 'D' = 10000
                int count = ItemManage.CountItem();
                if (count <= 10000)
                {
                    await InsertItem();
                }

                btnAdditem.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAdditem.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdditem_Click at add Item");
                Log.Debug("error", ex.Message);
            }
        }

        private async void BtnAddSize_Click(object sender, EventArgs e)
        {
            try
            {
                //check switch stock ว่า true ไหม ถ้า true จะไม่สามารถใช้งานปุ่มได้
                if (switchStock.Checked)
                {
                    Toast.MakeText(this, "สินค้ามีสต๊อกไม่สามารถเพิ่มขนาดได้", ToastLength.Short).Show();
                    //switchStock.Checked = false;
                    //lnSwithStcok.Visibility = ViewStates.Gone;
                    return;
                }

                //Get ค่าจาก textbox มาเก็บลง  List
                if (showdetail)
                {
                    await EditItemExSize();
                }

                //insert ItemExSize หลังจากกดเพิ่ม
                if (newlsItemExSize.Count < 5)
                {
                    var i = newlsItemExSize.Count + 1;
                    newlsItemExSize.Add(new ItemExSize { MerchantID = DataCashingAll.MerchantId, SysItemID = SysItemId, ExSizeNo = i, ExSizeName = "", Price = 0, EstimateCost = 0 });
                }

                ListItemExSize lstItemExSize = new ListItemExSize(newlsItemExSize);
                AddItem_Adapter_Size addItem_adapter_size = new AddItem_Adapter_Size(lstItemExSize);
                recyclerViewSize = FindViewById<RecyclerView>(Resource.Id.recyclerViewSize);
                LinearLayoutManager layoutManager = new LinearLayoutManager(this);
                recyclerViewSize.SetLayoutManager(layoutManager);
                recyclerViewSize.LayoutFrozen = true;
                recyclerViewSize.SetAdapter(addItem_adapter_size);
                var layout = addItem_adapter_size.ItemCount;
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                //lnExtrasize.LayoutParameters.Height = Convert.ToInt32(190* layout);
                this.OnResume();
                recyclerViewSize.ScrollToPosition(layout - 1);
                recyclerViewSize.FindFocus();

                CheckDataChange();

                if (addItem_adapter_size.ItemCount > 0)
                {
                    btnAddSize.Visibility = ViewStates.Gone;
                    lnAddsize.Visibility = ViewStates.Visible;
                    switchStock.Enabled = false;
                    switchStock.Checked = false;
                }
                else
                {
                    btnAddSize.Visibility = ViewStates.Visible;
                    lnAddsize.Visibility = ViewStates.Gone;
                    switchStock.Enabled = true;
                    switchStock.Checked = false;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddExSize at add Item");
            }
        }

        public async void DeleteExSize(ItemExSize PositionDelete)
        {
            try
            {
                await createItem.EditItemExSize();

                var item = createItem.newlsItemExSize.Find(x => x.ExSizeNo == PositionDelete.ExSizeNo);
                createItem.newlsItemExSize.Remove(item);

                ListItemExSize lstItemExSize = new ListItemExSize(createItem.newlsItemExSize);
                AddItem_Adapter_Size addItem_adapter_size = new AddItem_Adapter_Size(lstItemExSize);
                recyclerViewSize = createItem.FindViewById<RecyclerView>(Resource.Id.recyclerViewSize);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(createItem, 1, 1, false);
                recyclerViewSize.HasFixedSize = true;
                recyclerViewSize.SetLayoutManager(gridLayoutManager);
                recyclerViewSize.SetAdapter(addItem_adapter_size);
                var layout = addItem_adapter_size.ItemCount;
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                //lnExtrasize.LayoutParameters.Height = Convert.ToInt32(190* layout);
                createItem.OnResume();
                recyclerViewSize.ScrollToPosition(layout - 1);

                if (addItem_adapter_size.ItemCount > 0)
                {
                    btnAddSize.Visibility = ViewStates.Gone;
                    lnAddsize.Visibility = ViewStates.Visible;
                    switchStock.Enabled = false;
                    switchStock.Checked = false;
                }
                else
                {
                    btnAddSize.Visibility = ViewStates.Visible;
                    lnAddsize.Visibility = ViewStates.Gone;
                    switchStock.Enabled = true;
                    switchStock.Checked = false;
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteExSize at add Item");
                Log.Debug("error", ex.Message);
                Toast.MakeText(createItem, "DeleteExSize" + ex.Message, ToastLength.Short).Show();
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

        private void spinnerVat_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string selectVAT = spinnerVat.SelectedItem.ToString();
            if (selectVAT.ToLower() == "vat" || selectVAT == "ภาษีมูลค่าเพิ่ม")
            {
                TaxType = 'V';
            }
            else
            {
                TaxType = 'N';
            }
            CheckDataChange();

        }

        private async void BtnShowDetail_Click(object sender, EventArgs e)
        {
            if (showdetail)
            {
                await EditItemExSize();
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
                if (!HavePicture)
                {
                    SetItemView();
                }
                else
                {
                    CheckDataChange();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "TextInsertPrice_TextChanged" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_TextChanged at Add Item");
            }
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
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "TextInsertPrice_KeyPress" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_KeyPress at Add Item");
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
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Txtcost_KeyPress" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Txtcost_KeyPress at Add Item");
            }
        }

        private void TextInsertProduct_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var itemname = textInsertProduct?.Text.Trim();
            if (!string.IsNullOrEmpty(itemname))
            {
                txtItemNamePic.Text = itemname;
                txtViewItemnameTitle.Text = itemname;
            }
            else
            {
                txtItemNamePic.Text = "Item Name";
                txtViewItemnameTitle.Text = "Item Name";
            }
            if (!HavePicture)
            {
                SetItemView();
            }
            else
            {
                CheckDataChange();
            }
        }

        private void SetItemView()
        {
            try
            {
                imageViewItem.Visibility = ViewStates.Gone;
                txtViewItemnameTitle.Visibility = ViewStates.Visible;

                btnAdditem.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAdditem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));


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

                var itemprice = textInsertPrice?.Text.Trim();
                if (itemprice != "" && itemprice != "0")
                {
                    //CURRENCYSYMBOLS
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
                //SetPicture();
                CheckDataChange();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetItemView at add Item");
                Toast.MakeText(this, "SetItemView" + ex.Message, ToastLength.Short).Show();
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

        private void Btnaddpic_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.addcustomer_dialog_addimage.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("OpenPicture", "item");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btnaddpic_Click at add Item");
                Toast.MakeText(this, "addpick" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Btncolor9_Click(object sender, EventArgs e)
        {
            colorSelected = "#DD527E";
            SetItemView();
        }

        private void Btncolor8_Click(object sender, EventArgs e)
        {
            colorSelected = "#8BC34A";
            SetItemView();
        }

        private void Btncolor7_Click(object sender, EventArgs e)
        {
            colorSelected = "#00796B";
            SetItemView();
        }

        private void Btncolor6_Click(object sender, EventArgs e)
        {
            colorSelected = "#3F51B5";
            SetItemView();
        }

        private void Btncolor5_Click(object sender, EventArgs e)
        {
            colorSelected = "#F75600";
            SetItemView();
        }

        private void Btncolor4_Click(object sender, EventArgs e)
        {
            colorSelected = "#37AA52";
            SetItemView();
        }

        private void Btncolor3_Click(object sender, EventArgs e)
        {
            colorSelected = "#E32D49";
            SetItemView();
        }

        private void Btncolor2_Click(object sender, EventArgs e)
        {
            colorSelected = "#F8971D";
            SetItemView();
        }

        private void Btncolor1_Click(object sender, EventArgs e)
        {
            colorSelected = "#0095DA";
            SetItemView();
        }

        #region Picture
        public void GalleryOpen()
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GalleryOpen at add Item");
                Toast.MakeText(this, "error GalleryOpen : " + ex.Message, ToastLength.Short).Show(); return;
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

                string cropedFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                         Android.OS.Environment.DirectoryPictures,
                                                         "file_" + Guid.NewGuid().ToString() + ".jpg");
                Java.IO.File cropedFile = new Java.IO.File(cropedFilePath);
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
            catch (ActivityNotFoundException e)
            {
                Log.Debug("stateStep", "AddItem" + "CropImage ActivityNotFoundException " + e.Message);
                String errorMessage = "Your device doesn't support the crop action!";
                Toast.MakeText(this, e.Message + "-" + errorMessage, ToastLength.Short).Show(); return;
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "CropImage Exception" + ex.Message);
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CropImage at add Item");
                Toast.MakeText(this, "error : CropImage " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        Android.Net.Uri cameraTakePictureUri;
        public async void CameraTakePicture()
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
                    CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                }
                else
                {
                    tempURI = Android.Net.Uri.FromFile(tempFile);
                }
                cameraTakePictureUri = tempURI;
                CamIntent.PutExtra(MediaStore.ExtraOutput, tempURI);
                CamIntent.PutExtra("return-data", false);
                StartActivityForResult(CamIntent, CAMERA_REQUEST);
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "CameraTakePicture" + ex.Message);
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Tack Picture at add item");
                Toast.MakeText(this, "error  :  CameraTakePicture" + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == CAMERA_REQUEST && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    CropImage(cameraTakePictureUri);
                }
                else if (requestCode == GALLERY_PICTURE && Convert.ToInt32(resultCode) == RESULT_OK)
                {
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
                    if (data != null)
                    {
                        Bundle bundle = data.Extras;
                        Android.Net.Uri cropImageURI = null;
                        cropImageURI = keepCropedUri;
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
                        imageViewItem.SetImageBitmap(bitmap);
                        HavePicture = true;
                        SetPicture();
                    }
                    else
                    {
                        Log.Debug("stateStep", "AddItem" + "OnActivityResult  CROP_REQUEST data is null ");
                        Toast.MakeText(this, "error : CROP_REQUEST data is null", ToastLength.Short).Show();
                        return;
                    }
                }
                else
                {
                    Log.Debug("stateStep", "AddItem" + "OnActivityResult  else requestCode " + requestCode + " resultCode " + resultCode);
                }
            }
            catch (ActivityNotFoundException e)
            {
                Log.Debug("stateStep", "AddItem" + "OnActivityResult  ActivityNotFoundException" + e.Message);
                Toast.MakeText(this, "OnActivityResult at add Item" + " " + e.Message, ToastLength.Short).Show(); return;
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem OnActivityResult " + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at add Item");
                Toast.MakeText(this, "OnActivityResult" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        #endregion

        private void ImgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            tabSelected = "Item";
            DataCashing.EditItemID = 0;
            offline = false;
            checkManageStock = false;
            EditStock = false;
            flagdatachange = false;
        }

        public override void OnBackPressed()
        {
            try
            {
                if (DataCashing.EditItemID == 0)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "item");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack();
                        return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "item");
                    bundle.PutString("PassValue", DataCashing.EditItemID.ToString());
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

        public string SetAlphaColor(string colorSelected)
        {
            string[] color = colorSelected.Split("#");
            string colorReplace = "#80" + color[1];
            return colorReplace;
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async Task InsertItem()
        {
            try
            {
                DeviceSystemSeqNo deviceSystemSeq = new DeviceSystemSeqNo();
                DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();

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

                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 30);
                sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                long? category;
                if (SyscategoryID == 0 & SyscategoryIDfromPOS == 0)
                {
                    category = null;
                }
                else if (SyscategoryID != 0 & SyscategoryIDfromPOS == 0)
                {
                    category = SyscategoryID;
                }
                else
                {
                    category = SyscategoryIDfromPOS;
                }

                if (TaxType == '\0')
                {
                    TaxType = 'V';
                }

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResult = await Utils.InsertImageToThumbnail(path, bitmap, "item");
                    if (checkResult)
                    {
                        addItem.ThumbnailPath = pathThumnailFolder + path;
                        addItem.ThumbnailLocalPath = pathThumnailFolder + path;
                    }

                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    if (checkResultPicture)
                    {
                        addItem.PictureLocalPath = pathFolderPicture + path;
                    }
                    //Utils.streamImage(bitmap);
                    addItem.PicturePath = keepCropedUri.ToString();
                }
                else
                {
                    addItem.ThumbnailPath = null;
                    addItem.PicturePath = null;
                    addItem.PictureLocalPath = null;
                    addItem.ThumbnailLocalPath = null;
                }
                addItem.Colors = colorItem;
                addItem.MerchantID = DataCashingAll.MerchantId;
                addItem.SysItemID = long.Parse(sys);
                addItem.ItemName = textInsertProduct.Text.Trim();
                addItem.ShortName = txtViewItemnameTitle.Text?.Trim().ToString();
                addItem.Ordinary = 2;
                addItem.SysCategoryID = category;
                addItem.ItemCode = txtCodeitem?.Text.Trim() ?? "";
                addItem.PicturePath = "";
                //addItem.Colors = colorItem;
                //addItem.FavoriteNo = 0; //0 : (Default) n: เป็นสินค้าโปรด ซึ่ง ค่าตัวเลขจะเป็นลำดับในการ ที่หน้า UI Layout ซึ่งสามารถปรับขึ้นลงได้
                addItem.UnitName = null;
                addItem.RegularSizeName = null;
                var Price = textInsertPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textInsertPrice.Text;
                }
                addItem.Price = ConvertToDecimal(Price);
                addItem.OptSalePrice = 'F';
                addItem.TaxType = TaxType;
                addItem.SellBy = 'U';
                addItem.FTrackStock = 0; //เปิดการใช้งานระบบ การติดตามสินค้าคงคลัง (Track Stock)      ; 0 : ปิดใช้งานการติดตามสินค้าคงคลัง(Default)        ; 1 : เปิดใช้งานการติดตามสินค้าคงคลัง
                addItem.TrackStockDateTime = DateTime.UtcNow;
                addItem.SaleItemType = 'U';
                addItem.Comments = null;
                addItem.LastDateModified = DateTime.UtcNow;
                addItem.UserLastModified = usernamelogin;
                addItem.DataStatus = 'I';
                addItem.FWaitSending = 2; 
                addItem.WaitSendingTime = DateTime.UtcNow;
                addItem.LinkProMaxxItemID = null;
                addItem.LinkProMaxxItemUnit = null;
                if (txtcost.Text == string.Empty)
                {
                    //ถ้า User ไม่กำหนด ต้นทุน ระบบจะนำเอาราคาขายที่ตั้งมาเป็นต้นทุน
                    addItem.EstimateCost = ConvertToDecimal(textInsertPrice.Text.Trim());
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
                    addItem.EstimateCost = ConvertToDecimal(Cost);
                }
                if (string.IsNullOrEmpty(txtViewItemnameTitle.Text))
                {
                    if (textInsertProduct.Text.Length > 6)
                    {
                        addItem.ShortName = textInsertProduct.Text.Substring(0, 6);
                    }
                    else
                    {
                        addItem.ShortName = textInsertProduct.Text;
                    }
                }
                else
                {
                    addItem.ShortName = txtViewItemnameTitle.Text;
                }


                addItem.FDisplayOption = showDisplay;

                if (favorite)
                {
                    addItem.FavoriteNo = 1;
                }
                else
                {
                    addItem.FavoriteNo = 0;
                }

                decimal.TryParse(txtMinimumStock.Text.Trim(), out decimal minimumStock);

                #region Check ชื่อสินค้า และ itemCode
                //if ((textInsertProduct.Text.Trim() != getItems.ItemName) && (txtCodeitem.Text.Trim() != getItems.ItemCode))
                //{
                var checkNameandItemCode = items.FindIndex(x => x.ItemName.ToLower().Equals(addItem.ItemName.ToLower()) && !string.IsNullOrEmpty(addItem.ItemCode) && x.ItemCode.ToLower().Equals(addItem.ItemCode.ToLower()));
                if (checkNameandItemCode != -1)
                {
                    try
                    {
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                        InsertRepeatItem insertRepeat = new InsertRepeatItem();
                        insertRepeat.checkManageStock = checkManageStock;
                        insertRepeat.DetailITem = addItem;
                        insertRepeat.Stock = txtStock.Text;
                        insertRepeat.minimumstock = minimumStock.ToString("#,##0");
                        var json = JsonConvert.SerializeObject(insertRepeat);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.item_dialog_same_id_name.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "insertitemitemcodetrepeat");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("ItemName", addItem.ItemName);
                        bundle.PutString("ItemCode", addItem.ItemCode);
                        bundle.PutString("event", "insert");
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("checkItewmCode update at add Item");
                        Toast.MakeText(this, "Check ชื่อสินค้า และ itemCode" + ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }
                //}
                #endregion
                //Check ชื่อสินค้า 
                var checkname = items.FindIndex(x => x.ItemName.ToLower().Equals(addItem.ItemName.ToLower()));
                if (checkname != -1)
                {
                    try
                    {
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                        InsertRepeatItem insertRepeat = new InsertRepeatItem();
                        insertRepeat.checkManageStock = checkManageStock;
                        insertRepeat.DetailITem = addItem;
                        insertRepeat.Stock = txtStock.Text;
                        insertRepeat.minimumstock = minimumStock.ToString("#,##0");
                        var json = JsonConvert.SerializeObject(insertRepeat);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "insertrepeat");
                        bundle.PutString("detailitem", addItem.ItemName);
                        bundle.PutString("fromPage", "itemname");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "insert");
                        bundle.PutString("itemtype", "item");
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add Item");
                        Toast.MakeText(this, "Check ชื่อสินค้า " + ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                // itemCode
                var checkItemCode = items.FindIndex(x => !string.IsNullOrEmpty(addItem.ItemCode) && x.ItemCode.ToLower().Equals(addItem.ItemCode.ToLower()));
                if (checkItemCode != -1)
                {
                    try
                    {
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                        InsertRepeatItem insertRepeat = new InsertRepeatItem();
                        insertRepeat.checkManageStock = checkManageStock;
                        insertRepeat.DetailITem = addItem;
                        insertRepeat.Stock = txtStock.Text;
                        insertRepeat.minimumstock = minimumStock.ToString("#,##0");
                        var json = JsonConvert.SerializeObject(insertRepeat);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "inseritemcodetrepeat");
                        bundle.PutString("detailitem", addItem.ItemCode);
                        bundle.PutString("fromPage", "itemcode");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "insert");
                        bundle.PutString("itemtype", "item");
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("checkItewmCode at add Item");
                        Toast.MakeText(this, "itemCode" + ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                if (checkManageStock)
                {
                    if (addItem.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(txtStock.Text))
                        {
                            Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                            return;
                        }

                        addItem.FTrackStock = 1;
                        itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = addItem.MerchantID,
                            SysBranchID = DataCashingAll.SysBranchId,
                            SysItemID = addItem.SysItemID,
                            BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)),
                            MinimumStock = minimumStock,
                            LastDateBalanceStock = DateTime.UtcNow
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

                SysItemId = addItem.SysItemID;
                //เช็คว่าสินค้ามี size ซ้ำกันหรือไม่
                var resultAddSize = await AddItemExSize();
                if (!resultAddSize)
                {
                    Toast.MakeText(this, GetString(Resource.String.repeatnameexsize), ToastLength.Short).Show();
                    return;
                }

                var result = await ItemManage.InsertItem(addItem, itemOnBranch, itemExSizes);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                }

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)addItem.MerchantID, (int)addItem.SysItemID);
                }
                else
                {
                    addItem.FWaitSending = 2;
                    await ItemManage.UpdateItem(addItem);
                }

                if (checkManageStock)
                {
                    DataCashingAll.flagItemOnBranchChange = true;
                }
                flagdatachange = false;
                EditStock = false;
                SyscategoryIDfromPOS = 0;
                favoritefromPOS = false;
                checkManageStock = false;
                tabSelected = string.Empty;

                //NotifyItemInsert
                ItemActivity.SetFocusNewItem(addItem);
                DataCashingAll.flagItemChange = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Insert at add Item");
                Toast.MakeText(this, "inseritem : " + ex.Message, ToastLength.Short).Show();
                Log.Debug("insert", "inseritem ex : " + ex.Message);
            }
        }

        async Task GetItemList()
        {
            try
            {
                items = new List<Item>();
                ItemManage itemManage = new ItemManage();
                items = await itemManage.GetAllItem();
                if (items == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.notcalldata), ToastLength.Short).Show();
                    items = new List<Item>();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "GetItemList");
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("GetItemList at Item");
            }
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> AddItemExSize()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                itemExSizes = new List<ItemExSize>();
                for (int i = 0; i < recyclerViewSize.ChildCount; i++)
                {
                    View child = recyclerViewSize.GetChildAt(i);
                    RecyclerView.ViewHolder viewHolder = recyclerViewSize.GetChildViewHolder(child);
                    ListViewItemExSizeHolder vh = viewHolder as ListViewItemExSizeHolder;
                    var newitemsize = new ORM.MerchantDB.ItemExSize()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        SysItemID = SysItemId,
                        ExSizeNo = i + 1,
                        ExSizeName = vh.ExSizeName.Text,
                        Price = ConvertToDecimal(vh.Price.Text.Trim()),
                        EstimateCost = ConvertToDecimal(vh.EstimateCost.Text),
                        Comments = ""
                    };
                    itemExSizes.Add(newitemsize);
                }

                itemExSizes = itemExSizes.Where(s => !string.IsNullOrEmpty(s.ExSizeName)).Distinct().ToList();

                //Check SizeName ห้ามซ้ำกันภายในสินค้าตัวเดียวกัน
                //true คือมีข้อมูลซ้ำกัน
                if (itemExSizes.Count > 1)
                {
                    var SameNames = itemExSizes.All(x => itemExSizes.All(y => y.ExSizeName.ToLower().Equals(x.ExSizeName.ToLower())));
                    if (SameNames)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "AddItemExSize" + ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        //Show Item in App (Edit)        
        public async Task ShowItemForEdit()
        {
            try
            {
                SetTextColor();

                //where item ด้วย sysItem เพื่อแสดงข้อมูลที่ Textbox
                ItemExSizeManage = new ItemExSizeManage();
                ItemManage = new ItemManage();
                itemEdit = new Item();
                itemEdit = await ItemManage.GetItem(DataCashingAll.MerchantId, (int)SysItemId);

                if (itemEdit != null)
                {
                    textInsertProduct.Text = itemEdit.ItemName;
                    textInsertProduct.SetSelection(itemEdit.ItemName.Length);
                    if (!string.IsNullOrEmpty(itemEdit.ItemCode))
                    {
                        txtCodeitem.Text = itemEdit.ItemCode;
                        txtCodeitem.SetSelectAllOnFocus(true);
                    }
                    textInsertPrice.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(itemEdit.Price);
                    textInsertPrice.SetSelection(textInsertPrice.Text.Length);
                    txtcost.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(itemEdit.EstimateCost);
                    txtViewItemnameTitle.Text = itemEdit.ShortName?.ToString();

                    var cloudpath = itemEdit.PicturePath == null ? string.Empty : itemEdit.PicturePath;
                    var localpath = itemEdit.ThumbnailLocalPath == null ? string.Empty : itemEdit.ThumbnailLocalPath;
                    if (CheckNet)
                    {
                        if (string.IsNullOrEmpty(localpath))
                        {
                            if (string.IsNullOrEmpty(cloudpath))
                            {
                                //defalut
                                imageViewItem.SetImageURI(null);
                                string conColor = Utils.SetBackground(Convert.ToInt32(itemEdit.Colors));
                                var color = Android.Graphics.Color.ParseColor(conColor);
                                colorSelected = conColor;
                                colorViewItem.SetBackgroundColor(color);
                                HavePicture = false;
                            }
                            else
                            {
                                //cloud
                                Utils.SetImage(imageViewItem, cloudpath);
                                txtViewItemnameTitle.Visibility = ViewStates.Gone;
                                imageViewItem.Visibility = ViewStates.Visible;
                                HavePicture = true;
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
                            string conColor = Utils.SetBackground(Convert.ToInt32(itemEdit.Colors));
                            var color = Android.Graphics.Color.ParseColor(conColor);
                            colorSelected = conColor;
                            colorViewItem.SetBackgroundColor(color);
                            HavePicture = false;
                        }
                    }

                    SetItemView();

                    SetPicture();

                    char selectVAT = itemEdit.TaxType;
                    TaxType = itemEdit.TaxType;
                    if (selectVAT == 'V')
                    {
                        spinnerVat.SetSelection(0);
                    }
                    else
                    {
                        spinnerVat.SetSelection(1);
                    }

                    CategoryManage categoryManage = new CategoryManage();
                    List<Category> lstcategory = new List<Category>();
                    List<Category> getallCategory = new List<Category>();
                    Category addcategory = new Category();

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

                    long? category = itemEdit.SysCategoryID;

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

                    SysItemId = itemEdit.SysItemID;

                    if (itemEdit.FDisplayOption == 1)
                    {
                        switchShowDisplay.Checked = true;
                    }
                    else
                    {
                        switchShowDisplay.Checked = false;
                    }
                    if (itemEdit.FavoriteNo != 0)
                    {
                        favorite = true;
                    }
                    else
                    {
                        favorite = false;
                    }
                    if (itemEdit.FTrackStock == 1)
                    {
                        switchStock.Checked = true;
                        showStock = 1;
                    }
                    else
                    {
                        switchStock.Checked = false;
                        showStock = 0;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem Erro 5 - " + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowItemForEdit at add Item");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, "ShowItemForEdit" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        //Show ItemExSize in App (Edit)
        //get ค่าเดิมจากฐานข้อมูล
        public async Task ShowItemExSize()
        {
            try
            {
                if (SysItemId != 0)
                {
                    lstExSize = await ItemExSizeManage.GetItemSize(DataCashingAll.MerchantId, (int)SysItemId);
                    if (lstExSize != null)
                    {
                        newlsItemExSize = lstExSize;
                        //TemplsItemExSize = lstExSize;

                        foreach (var item in lstExSize)
                        {
                            var itemsize = new ORM.MerchantDB.ItemExSize()
                            {
                                MerchantID = item.MerchantID,
                                SysItemID = item.SysItemID,
                                ExSizeNo = item.ExSizeNo,
                                ExSizeName = item.ExSizeName,
                                Price = Convert.ToDecimal(item.Price),
                                EstimateCost = Convert.ToDecimal(item.EstimateCost),
                                Comments = item.Comments
                            };
                            TemplsItemExSize.Add(itemsize);
                        }
                    }
                }
                var i = newlsItemExSize.Count + 1;
                //newlsItemExSize.Add(new ItemExSize { MerchantID = DataCashingAll.MerchantId, SysItemID = i, ExSizeNo = 0, ExSizeName = "", Price = 0, EstimateCost = 0 });

                ListItemExSize lstItemExSize = new ListItemExSize(newlsItemExSize);
                AddItem_Adapter_Size addItem_adapter_size = new AddItem_Adapter_Size(lstItemExSize);
                recyclerViewSize = FindViewById<RecyclerView>(Resource.Id.recyclerViewSize);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 1, 1, false);
                recyclerViewSize.HasFixedSize = true;
                recyclerViewSize.SetLayoutManager(gridLayoutManager);

                recyclerViewSize.SetAdapter(addItem_adapter_size);

                if (addItem_adapter_size.ItemCount > 0)
                {
                    btnAddSize.Visibility = ViewStates.Gone;
                    lnAddsize.Visibility = ViewStates.Visible;
                }
                else
                {
                    btnAddSize.Visibility = ViewStates.Visible;
                    lnAddsize.Visibility = ViewStates.Gone;
                }
                if (lstExSize.Count > 0)
                {
                    switchStock.Enabled = false;
                    switchStock.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem error" + "ShowItemExSize" + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowItemExSize at add Item");
                Toast.MakeText(this, "ShowItemExSize" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }


        private async void BtnEditProduct_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdditem.Enabled = false;
                await UpdateItem();
                btnAdditem.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAdditem.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public async Task UpdateItem()
        {
            try
            {
                Item getItems = new Item();
                getItems = await ItemManage.GetItem(DataCashingAll.MerchantId, (int)SysItemId);

                if (TaxType == '\0')
                {
                    TaxType = 'V';
                }

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
                        editItem.ThumbnailPath = pathThumnailFolder + path;
                        editItem.ThumbnailLocalPath = pathThumnailFolder + path;
                    }

                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    if (checkResultPicture)
                    {
                        editItem.PictureLocalPath = pathFolderPicture + path;
                    }

                    if (!string.IsNullOrEmpty(getItems.ThumbnailLocalPath))
                    {
                        Java.IO.File imgFile = new Java.IO.File(getItems.ThumbnailLocalPath);
                        if (System.IO.File.Exists(imgFile.AbsolutePath))
                        {
                            System.IO.File.Delete(imgFile.AbsolutePath);
                        }
                    }
                    editItem.PicturePath = getItems.PicturePath;
                    editItem.Colors = 0;
                }
                else
                {
                    editItem.PictureLocalPath = getItems.PictureLocalPath;
                    editItem.PicturePath = getItems.PicturePath;
                    editItem.ThumbnailLocalPath = getItems.ThumbnailLocalPath;
                    editItem.ThumbnailPath = getItems.ThumbnailPath;
                    editItem.Colors = getItems.Colors;
                }

                editItem.MerchantID = getItems.MerchantID;
                editItem.SysItemID = getItems.SysItemID;
                if (textInsertProduct.Text.Trim() != getItems.ItemName && textInsertProduct.Text.Trim() != string.Empty)
                {
                    editItem.ItemName = textInsertProduct.Text.Trim();
                }
                else
                {
                    editItem.ItemName = getItems.ItemName;
                }

                //editItem.ShortName = txtViewItemnameTitle.Text?.ToString();
                if (string.IsNullOrEmpty(txtViewItemnameTitle.Text))
                {
                    if (textInsertProduct.Text.Length > 6)
                    {
                        editItem.ShortName = textInsertProduct.Text.Substring(0, 6);
                    }
                    else
                    {
                        editItem.ShortName = textInsertProduct.Text;
                    }
                }
                else
                {
                    editItem.ShortName = txtViewItemnameTitle.Text;
                }
                editItem.Ordinary = getItems.Ordinary;

                if (getItems.SysCategoryID == SyscategoryID)
                {
                    editItem.SysCategoryID = getItems.SysCategoryID;
                }
                else if (SyscategoryID == 0)
                {
                    editItem.SysCategoryID = null;
                }
                else
                {
                    editItem.SysCategoryID = SyscategoryID;
                }

                editItem.ItemCode = txtCodeitem?.Text.Trim() ?? getItems.ItemCode;
                editItem.FavoriteNo = getItems.FavoriteNo;
                editItem.UnitName = getItems.UnitName;
                editItem.RegularSizeName = getItems.RegularSizeName;
                var Price = textInsertPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textInsertPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textInsertPrice.Text;
                }
                if (ConvertToDecimal(Price) != getItems.Price)
                {
                    editItem.Price = ConvertToDecimal(Price);
                }
                else
                {
                    editItem.Price = getItems.Price;
                }
                editItem.OptSalePrice = getItems.OptSalePrice;
                if (getItems.TaxType != TaxType)
                {
                    editItem.TaxType = TaxType;
                }
                else
                {
                    editItem.TaxType = getItems.TaxType;
                }
                editItem.SellBy = getItems.SellBy;

                #region save stock old
                if (getItems.FTrackStock != showStock || getItems.FTrackStock == 1 & EditStock)
                {
                    //case save stock                    
                    editItem.FTrackStock = showStock;
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
                    editItem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                }
                else
                {
                    editItem.FTrackStock = getItems.FTrackStock;
                    editItem.TrackStockDateTime = Utils.GetTranDate(getItems.TrackStockDateTime);
                }
                #endregion
                editItem.SaleItemType = getItems.SaleItemType;
                editItem.Comments = getItems.Comments;
                editItem.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                editItem.UserLastModified = usernamelogin;
                editItem.DataStatus = 'M';
                editItem.FWaitSending = 2;
                editItem.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                editItem.LinkProMaxxItemID = getItems.LinkProMaxxItemID;
                editItem.LinkProMaxxItemUnit = getItems.LinkProMaxxItemUnit;

                var Cost = txtcost.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Cost = txtcost.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Cost = txtcost.Text;
                }

                if ((ConvertToDecimal(Cost) != getItems.EstimateCost) && !string.IsNullOrEmpty(Cost))
                {
                    editItem.EstimateCost = ConvertToDecimal(Cost);
                }
                else if (getItems.EstimateCost == getItems.Price)
                {
                    editItem.EstimateCost = ConvertToDecimal(Price);
                }
                else
                {
                    editItem.EstimateCost = getItems.EstimateCost;
                }

                editItem.ShortName = txtViewItemnameTitle.Text?.ToString();
                editItem.FDisplayOption = showDisplay;

                if (favorite)
                {
                    editItem.FavoriteNo = 1;
                }
                else
                {
                    editItem.FavoriteNo = 0;
                }

                if (colorItem != getItems.Colors)
                {
                    editItem.Colors = colorItem;
                    //กรณีเปลี่ยนสี แต่เคยมีรูปอยู่
                    if (!string.IsNullOrEmpty(getItems.ThumbnailLocalPath))
                    {
                        Java.IO.File imgFiletThumnail = new Java.IO.File(getItems.ThumbnailLocalPath);
                        if (System.IO.File.Exists(imgFiletThumnail.AbsolutePath))
                        {
                            System.IO.File.Delete(imgFiletThumnail.AbsolutePath);
                        }
                        editItem.PicturePath = null;
                        editItem.PictureLocalPath = null;
                        editItem.ThumbnailPath = null;
                        editItem.ThumbnailLocalPath = null;
                    }
                }
                else
                {
                    editItem.Colors = getItems.Colors;
                }

                // Check ชื่อสินค้า และ itemCode 
                #region Check ชื่อสินค้า และ itemCode
                if ((textInsertProduct.Text.Trim() != getItems.ItemName) && (txtCodeitem.Text.Trim() != getItems.ItemCode))
                {
                    var checkNameandItemCode = items.FindIndex(x => x.ItemName.ToLower().Equals(editItem.ItemName.ToLower()) && !string.IsNullOrEmpty(editItem.ItemCode) && x.ItemCode.ToLower().Equals(editItem.ItemCode.ToLower()));
                    if (checkNameandItemCode != -1)
                    {
                        try
                        {
                            //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                            InsertRepeatItem insertRepeat = new InsertRepeatItem();
                            insertRepeat.checkManageStock = checkManageStock;
                            insertRepeat.DetailITem = editItem;
                            insertRepeat.Stock = txtStock.Text;
                            insertRepeat.minimumstock = txtMinimumStock.Text;
                            var json = JsonConvert.SerializeObject(insertRepeat);

                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("insertRepeat", "insertitemitemcodetrepeat");
                            bundle.PutString("detailnnsert", json);
                            bundle.PutString("ItemName", editItem.ItemName);
                            bundle.PutString("ItemCode", editItem.ItemCode);
                            bundle.PutString("event", "update");
                            dialog.Arguments = bundle;
                            dialog.Show(SupportFragmentManager, myMessage);
                            return;
                        }
                        catch (Exception ex)
                        {
                            _ = TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("checkItewmCode update at add Item");
                            Toast.MakeText(this, "Check ชื่อสินค้า และ itemCode  edit" + ex.Message, ToastLength.Short).Show();
                            return;
                        }
                    }
                }
                #endregion

                //Check ชื่อสินค้า                 
                if (textInsertProduct.Text.Trim() != getItems.ItemName && textInsertProduct.Text.Trim() != string.Empty)
                {
                    var checkname = items.FindIndex(x => x.ItemName.ToLower().Equals(editItem.ItemName.ToLower()));
                    if (checkname != -1)
                    {
                        //Toast.MakeText(this, "ชื่อสินค้านี้มีอยู่ในระบบแล้ว ต้องการจะบันทึกหรือไม่", ToastLength.Short).Show();
                        try
                        {
                            //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                            InsertRepeatItem insertRepeat = new InsertRepeatItem();
                            insertRepeat.checkManageStock = checkManageStock;
                            insertRepeat.DetailITem = editItem;
                            insertRepeat.Stock = txtStock.Text;
                            insertRepeat.minimumstock = txtMinimumStock.Text;
                            var json = JsonConvert.SerializeObject(insertRepeat);

                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("insertRepeat", "insertrepeat");
                            bundle.PutString("detailitem", editItem.ItemName);
                            bundle.PutString("fromPage", "itemname");
                            bundle.PutString("detailnnsert", json);
                            bundle.PutString("event", "update");
                            bundle.PutString("itemtype", "item");
                            dialog.Arguments = bundle;
                            dialog.Show(SupportFragmentManager, myMessage);
                            return;
                        }
                        catch (Exception ex)
                        {
                            await TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("checkName update at add Item");
                            Toast.MakeText(this, "Check ชื่อสินค้า " + ex.Message, ToastLength.Short).Show();
                            return;
                        }
                    }
                }

                // itemCode
                if (txtCodeitem.Text.Trim() != getItems.ItemCode)
                {
                    var checkItemCode = items.FindIndex(x => !string.IsNullOrEmpty(editItem.ItemCode.ToLower()) && x.ItemCode.ToLower().Equals(editItem.ItemCode.ToLower()));
                    if (checkItemCode != -1)
                    {
                        try
                        {
                            //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog
                            InsertRepeatItem insertRepeat = new InsertRepeatItem();
                            insertRepeat.checkManageStock = checkManageStock;
                            insertRepeat.DetailITem = editItem;
                            insertRepeat.Stock = txtStock.Text;
                            insertRepeat.minimumstock = txtMinimumStock.Text;
                            var json = JsonConvert.SerializeObject(insertRepeat);

                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("insertRepeat", "inseritemcodetrepeat");
                            bundle.PutString("detailitem", editItem.ItemCode);
                            bundle.PutString("fromPage", "itemcode");
                            bundle.PutString("detailnnsert", json);
                            bundle.PutString("event", "update");
                            bundle.PutString("itemtype", "item");
                            dialog.Arguments = bundle;
                            dialog.Show(SupportFragmentManager, myMessage);
                            return;
                        }
                        catch (Exception ex)
                        {
                            await TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("checkItewmCode update at add Item");
                            Toast.MakeText(this, " itemCode edit" + ex.Message, ToastLength.Short).Show();
                            return;
                        }
                    }
                }

                //case save stock
                //await UpdateStock(getItems);

                //เช็คว่าสินค้ามี size ซ้ำกันหรือไม่                
                if (showdetail)
                {
                    var resultUpdateSize = await EditItemExSize();
                    if (!resultUpdateSize)
                    {
                        Toast.MakeText(this, GetString(Resource.String.repeatnameexsize), ToastLength.Short).Show();
                        return;
                    }
                }

                UpdateItemExsize();

                var result = await ItemManage.UpdateItem(editItem);
                if (!result)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                }

                if (EditStock)
                {
                    ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                    var updateStock = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranch);
                }

                if (getItems.FTrackStock != showStock | getItems.FTrackStock == 1 & EditStock)
                {
                    DataCashingAll.flagItemOnBranchChange = true;
                }

                // senttocloud 
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendItem((int)editItem.MerchantID, (int)editItem.SysItemID);
                }
                else
                {
                    editItem.FWaitSending = 2;
                    await ItemManage.UpdateItem(editItem);
                }

                EditStock = false;
                flagdatachange = false;
                checkManageStock = false;
                tabSelected = string.Empty;


                //NotifyItemInsert
                ItemActivity.SetFocusNewItem(editItem);
                DataCashingAll.flagItemChange = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Update at edit Item");
                Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> EditItemExSize()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                itemExSizes = new List<ItemExSize>();
                for (int i = 0; i < recyclerViewSize.ChildCount; i++)
                {
                    View child = recyclerViewSize.GetChildAt(i);
                    RecyclerView.ViewHolder viewHolder = recyclerViewSize.GetChildViewHolder(child);
                    ListViewItemExSizeHolder vh = viewHolder as ListViewItemExSizeHolder;
                    var newitemsize = new ORM.MerchantDB.ItemExSize()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        SysItemID = SysItemId,
                        ExSizeNo = i + 1,
                        ExSizeName = vh.ExSizeName.Text,
                        //Price = ConvertToDecimal(vh.Price.Text.Trim()),
                        Price = ConvertToDecimal(vh.Price.Text.Trim()),
                        EstimateCost = ConvertToDecimal(vh.EstimateCost.Text),
                        Comments = ""
                    };
                    itemExSizes.Add(newitemsize);
                }
                itemExSizes = itemExSizes.Where(s => !string.IsNullOrEmpty(s.ExSizeName)).Distinct().ToList();
                newlsItemExSize = itemExSizes;

                //Check SizeName ห้ามซ้ำกันภายในสินค้าตัวเดียวกัน
                //true คือมีข้อมูลซ้ำกัน
                if (itemExSizes.Count > 1)
                {
                    var SameNames = itemExSizes.All(x => itemExSizes.All(y => y.ExSizeName.ToLower().Equals(x.ExSizeName.ToLower())));
                    if (SameNames)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("EditItemExSize at add Item");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, "EditItemExSize" + ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public async void UpdateItemExsize()
        {
            //กรณีที่แก้ไขข้อมูล ExSize ตอนกดแก้ไขข้อมูล Item 
            //ลบ ItemSize เดิมออกแล้ว Insert ใหม่

            var getItemSize = await ItemExSizeManage.GetItemSize((int)editItem.MerchantID, (int)editItem.SysItemID);
            if (getItemSize.Count > 0)
            {
                bool checkResult = await ItemExSizeManage.DeleteItemsize((int)editItem.MerchantID, (int)editItem.SysItemID);
                if (!checkResult)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
            }

            bool checkInsert = await ItemExSizeManage.InsertListItemsize(newlsItemExSize);
            if (!checkInsert)
            {
                Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();
            }
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                CheckNet = await GabanaAPI.CheckSpeedConnection();
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "Onresume error" + ex.Message);
                Toast.MakeText(this, "OnResume" + ex.Message, ToastLength.Short).Show();
            }
        }

        public void SetTextColor()
        {
            textInsertProduct.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            textInsertPrice.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            txtCodeitem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            txtcost.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
        }

        public async void SpinnerCategory()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                itemID = new List<string>();
                CategoryManage categoryManage = new CategoryManage();
                Category addcategory = new Category();
                var category = new List<Category>();
                var getallCategory = new List<Category>();

                addcategory = new Category()
                {
                    Name = "None",
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

                if (SyscategoryIDfromPOS != 0)
                {
                    int index = category.FindIndex(x => x.SysCategoryID == SyscategoryIDfromPOS);
                    if (index != -1)
                    {
                        spinnerCategory.SetSelection(index);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SpinnerCategory at add Item");
                Log.Debug("error", ex.Message);
            }
        }
        private void SetTabMenu()
        {
            MenuTab = new List<MenuTab>
            {
                new MenuTab() { NameMenuEn = "Item" , NameMenuTh = "สินค้า" },
                new MenuTab() { NameMenuEn = "Stock" , NameMenuTh = "สต็อก" }
            };
        }

        private void SetTabShowMenu()
        {
            try
            {
                if (tabSelected == "")
                {
                    tabSelected = "Item";
                }
                GridLayoutManager menuLayoutManager = new GridLayoutManager(this, 2, 1, false);
                recyclerHeaderItem.HasFixedSize = true;
                recyclerHeaderItem.SetLayoutManager(menuLayoutManager);
                AddItem_Adapter_Header additem_Adapter_Header = new AddItem_Adapter_Header(MenuTab, "item");
                recyclerHeaderItem.SetAdapter(additem_Adapter_Header);
                additem_Adapter_Header.ItemClick += AddItem_Adapter_Header_ItemClick;
                linearShowProduct.Visibility = ViewStates.Gone;
                switch (tabSelected)
                {
                    case "Item":
                        linearShowProduct.Visibility = ViewStates.Visible;
                        break;
                    case "Stock":
                        linearShowStock.Visibility = ViewStates.Visible;
                        if ((DataStock == null & CheckNet) | (itemEdit?.FTrackStock == 0) | (getBalance == null & !CheckNet))
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
                        if ((itemEdit?.FTrackStock == 1 && DataStock == null & getBalance == null) && !EditStock)
                        {
                            lnSwithStcok.Visibility = ViewStates.Visible;
                            txtStock.Text = "0";
                            txtMinimumStock.Text = "0";
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
                        linearShowProduct.Visibility = ViewStates.Gone;
                        break;
                }
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "Error AddItem" + "SetTabShowMenu");
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at add Item");
                Toast.MakeText(this, "SetTabShowMenu" + ex.Message, ToastLength.Short).Show();
            }
        }

        private void AddItem_Adapter_Header_ItemClick(object sender, int e)
        {
            if (DataCashing.EditItemID == 0 & !CheckNet)
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

        //BalanceStock กับ MinimumStock
        async Task GetStockData()
        {
            try
            {
                ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                if (SysItemId != 0 & CheckNet)
                {
                    DataStock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, (int)SysItemId);
                    if (DataStock != null)
                    {
                        ItemOnBranch itemOnBranch = new ItemOnBranch()
                        {
                            MerchantID = DataStock.MerchantID,
                            SysBranchID = DataStock.SysBranchID,
                            SysItemID = DataStock.SysItemID,
                            BalanceStock = DataStock.BalanceStock,
                            MinimumStock = DataStock.MinimumStock,
                            LastDateBalanceStock = DataStock.LastDateBalanceStock,
                        };
                        var insert = await onBranchManage.InsertorReplaceItemOnBranch(itemOnBranch);

                        ItemOnBranch onBranch = new ItemOnBranch();
                        onBranch = await onBranchManage.GetItemOnBranch(DataCashingAll.MerchantId, DataCashingAll.SysBranchId, (int)SysItemId);
                        if (onBranch != null)
                        {
                            txtStock.Text = onBranch.BalanceStock.ToString("#,###");
                            txtMinimumStock.Text = onBranch.MinimumStock.ToString("#,##0");
                        }

                        if (itemEdit?.FTrackStock == 0)
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
                        txtMinimumStock.Text = getBalance.MinimumStock.ToString("#,##0");

                        if (itemEdit.FTrackStock == 0)
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
                            txtMinimumStock.Enabled = false;
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
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "GetStockData" + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetStockData at add Item");
                Toast.MakeText(this, "GetStockData" + ex.Message, ToastLength.Short).Show();
            }
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
                ResultAPI PostDataTrackStockAdjust = await GabanaAPI.PostDataTrackStockAdjust(sysBranchID, sysItemID, deviceNo, balanceStock, minimumStock);
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
                Toast.MakeText(this, "UpdateOpenStock" + PostDataTrackStockOpen.Message, ToastLength.Long).Show();
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
                Toast.MakeText(this, "UpdateClosetock" + PostDataTrackStockClose.Message, ToastLength.Long).Show();
                return false;
            }
        }

        decimal ConvertToDecimal(string txt)
        {
            decimal decimalValue = 0;
            decimal.TryParse(txt, out decimalValue);
            return decimalValue;
        }

        public static void SetCategoryFromPOS(long SyscategoryID)
        {
            SyscategoryIDfromPOS = SyscategoryID;
        }

        public static void SetFavFromPOS(bool fav)
        {
            favoritefromPOS = fav;
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
                if (string.IsNullOrEmpty(txtMinimumStock.Text))
                {
                    minimumStock = 0;
                }
                else
                {
                    minimumStock = Convert.ToDecimal(txtMinimumStock.Text);
                }

                //int sysBranchID, int sysItemID, int deviceNo, decimal? balanceStock, decimal? minimumStock
                resultstock = await UpdateOpenStock(DataCashingAll.SysBranchId, (int)editItem.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)), minimumStock);
                if (!resultstock)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = editItem.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = editItem.SysItemID,
                    BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)),
                    MinimumStock = minimumStock,
                    LastDateBalanceStock = DateTime.UtcNow
                };
            }
            else
            {
                //Close Stock
                resultstock = await UpdateClosetock((int)editItem.SysItemID);
                if (!resultstock)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = editItem.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = editItem.SysItemID,
                    BalanceStock = 0,
                    MinimumStock = 0,
                    LastDateBalanceStock = DateTime.UtcNow
                };

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