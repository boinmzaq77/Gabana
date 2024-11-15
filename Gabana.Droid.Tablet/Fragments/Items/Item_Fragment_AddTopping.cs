using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Items;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Items
{
    public class Item_Fragment_AddTopping : AndroidX.Fragment.App.Fragment
    {
        public static string tabSelected = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Item_Fragment_AddTopping NewInstance()
        {
            Item_Fragment_AddTopping frag = new Item_Fragment_AddTopping();
            return frag;
        }

       public static Item_Fragment_AddTopping fragment_addtopping;
        View view;
        string usernamelogin;
        public static bool checkManageStock = false;
        public static Item ExtraToppping = new Item();
        string ToppingName, sys, colorSelected, DecimalDisplay, LoginType;

        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        internal static Android.Net.Uri keepCropedUri;
        Android.Graphics.Bitmap bitmap;
        string path;
        static bool EditStock = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.item_fragment_addtopping, container, false);
            try
            {
                fragment_addtopping = this;                
                usernamelogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                CheckJwt();
                ComBineUI();                
                SetUIEvent();
                SetUIFromMainRole(LoginType);
                return view;
            }
            catch (Exception)
            {
                return view;
            }
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

        private async Task SetDetailToping()
        {
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
            if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
            textItemPrice.Hint = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(0);

            DecimalDisplay = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;
            colorSelected = "#0095DA";
            string colorReplace = SetAlphaColor(colorSelected);

            btnShowDetail.Click += LnBtnShowDetail_Click;
            showdetail = false;
            usernamelogin = Preferences.Get("User", "");
            LoginType = Preferences.Get("LoginType", "");

            pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
            pathFolderPicture = DataCashingAll.PathFolderImage;            

            if (DataCashing.EditTopping == null)
            {
                txtTitle.Text = GetString(Resource.String.item_activity_addtopping);
                btnAddTopping.Text = GetString(Resource.String.item_activity_addtopping);                
                btnDelete.Visibility = ViewStates.Gone;                
            }
            else
            {
                txtTitle.Text = GetString(Resource.String.extra_activity_Edit);
                btnAddTopping.Text = GetString(Resource.String.textsave);
                ShowDetailTopping();
                await GetStockData();               
                btnDelete.Visibility = ViewStates.Visible;                
            }
            //await GetExtraList();
            SetTabMenu();
            SetTabShowMenu();

            SetFavorite();
            ShowDetailItem();
        }
        public override async void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            CheckJwt();
            if (DataCashing.flagChooseMedia)
            {
                SetImgTopping();
                return;
            }

            first = true;
            UINewItem();
            SetItemView();
            SpinnerCategory();
            await SetDetailToping();
            SetUIFromMainRole(LoginType);

            first = false;
            flagdatachange = false;
            SetButtonAdd(false);
        }

        private void SetImgTopping()
        {
            try
            {
                if (keepCropedUri != null)
                {
                    //Clear รูปภาพก่อนทำอะไรใหม่
                    string setpathnull = string.Empty;
                    Android.Net.Uri urisetpathnull = Android.Net.Uri.Parse(setpathnull);
                    imageViewItem.SetImageURI(urisetpathnull);

                    Android.Net.Uri cropImageURI = keepCropedUri;
                    bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
                    imageViewItem.SetImageBitmap(bitmap);

                    HavePicture = true;
                    SetPicture();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetImgProfile at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public List<MenuTab> menuTab { get; set; }
        private void SetTabMenu()
        {
            menuTab = new List<MenuTab>
            {
                new MenuTab() { NameMenuEn = "Item" , NameMenuTh = "สินค้า" },
                new MenuTab() { NameMenuEn = "Stock" , NameMenuTh = "สต็อก" }
            };
        }
        private void SetTabShowMenu()
        {
            if (string.IsNullOrEmpty(tabSelected))
            {
                tabSelected = "Item";
            }

            GridLayoutManager menuLayoutManager = new GridLayoutManager(this.Activity, 2, 1, false);
            rcvHeaderItem.HasFixedSize = true;
            rcvHeaderItem.SetLayoutManager(menuLayoutManager);
            AddItem_Adapter_Header additem_Adapter_Header = new AddItem_Adapter_Header(menuTab, "extra");
            rcvHeaderItem.SetAdapter(additem_Adapter_Header);
            additem_Adapter_Header.ItemClick += Additem_adapter_header_ItemClick;
            lnShowItem.Visibility = ViewStates.Gone;
            switch (tabSelected)
            {
                case "Item":
                    lnShowItem.Visibility = ViewStates.Visible;
                    break;
                case "Stock":
                    lnShowStock.Visibility = ViewStates.Visible;
                    if (GetDataStock == null & DataCashing.CheckNet | ExtraToppping?.FTrackStock == 0 | getBalance == null & !DataCashing.CheckNet)
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
                        txtMinimumStock.Text = "0";
                        switchStock.Checked = true;
                    }

                    if (!DataCashing.CheckNet)
                    {
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.alert_dialog_offline.ToString();
                        bundle.PutString("message", myMessage);
                        dialog.Arguments = bundle;
                        dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    }
                    break;
                default:
                    lnShowItem.Visibility = ViewStates.Gone;
                    lnShowStock.Visibility = ViewStates.Gone;
                    break;
            }
            CheckDataChange();
        }

        private void Additem_adapter_header_ItemClick(object sender, int e)
        {
            try
            {
                if (DataCashing.EditTopping == null & !DataCashing.CheckNet)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.item_dialog_offline.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }

                tabSelected = menuTab[e].NameMenuEn;
                SetTabShowMenu();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, "Additem_adapter_header_ItemClick " + ex.Message, ToastLength.Short).Show();
            }
        }
        public void UINewItem()
        {
            try
            {
                txtTitle.Text = string.Empty;
                textItemName.Text = string.Empty;
                textItemPrice.Text = string.Empty;
                textcost.Text = string.Empty;
                imageViewItem.SetImageURI(null);
                colorSelected = "#0095DA";
                txtItemNamePic.Text = "Item Name";
                txtViewItemnameTitle.Text = "Item Name";
                txtStock.Text = "0";
                txtMinimumStock.Text = "0";
                SetItemView();
                spnCategory.SetSelection(0);
                SyscategoryID = 0;
                switchStock.Checked = false;
                switchStock.Enabled = true;
                favorite = false;
                showStock = 0;
                tabSelected = "Item";
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowItemForEdit at add Item");
                Toast.MakeText(this.Activity, "ShowItemForEdit" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            string Role = LoginType;
            bool check = UtilsAll.CheckPermissionRoleUser(Role, "delete", "topping");
            if (!check)
            {
                btnDelete.Enabled = true;
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                return;                
            }

            MainDialog dialog = new MainDialog() { Cancelable = false };
            Bundle bundle = new Bundle();
            String myMessage = Resource.Layout.addtopping_dialog_delete.ToString();
            bundle.PutString("message", myMessage);
            dialog.Arguments = bundle;
            dialog.Show(Activity.SupportFragmentManager, myMessage);

            btnDelete.Enabled = true;
        }        
        
        public async Task<bool> UpdateToppping()
        {
            try
            {                
                Item EditToppping = new Item();                
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

                var Price = textItemPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textItemPrice.Text;
                }
                EditToppping.Price = ConvertToDecimal(Price);

                //Topping มี cost ไหม
                if (string.IsNullOrEmpty(textcost.Text))
                {
                    //ถ้า User ไม่กำหนด ต้นทุน ระบบจะนำเอาราคาขายที่ตั้งมาเป็นต้นทุน
                    EditToppping.EstimateCost = ConvertToDecimal(Price);
                }
                else
                {
                    var Cost = textcost.Text.Trim();
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        Cost = textcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        Cost = textcost.Text;
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
                            Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                            return false;
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

                if (textItemName.Text.Length > 5)
                {
                    EditToppping.ShortName = textItemName.Text.Substring(0, 5);
                }
                else
                {
                    EditToppping.ShortName = textItemName.Text;
                }                               

                var result = await itemManage.UpdateItem(EditToppping);
                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();

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

                Item_Fragment_Main.fragment_main.ReloadTopping(EditToppping);
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateToppping at add Extra");
                Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                return false;
            }
        }

        Gabana.ORM.MerchantDB.Item editItem = new Gabana.ORM.MerchantDB.Item();
        private async Task UpdateStock()
        {
            bool resultstock = false;
            if (showStock == 1)
            {
                //Open Stock
                if (string.IsNullOrEmpty(txtStock.Text))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
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
                resultstock = await UpdateOpenStock(DataCashingAll.SysBranchId, (int)DataCashing.EditTopping.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)), minimumStock);
                if (!resultstock)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = DataCashing.EditTopping.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = DataCashing.EditTopping.SysItemID,
                    BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)),
                    MinimumStock = minimumStock,
                    LastDateBalanceStock = DateTime.UtcNow
                };
            }
            else
            {
                //Close Stock
                resultstock = await UpdateClosetock((int)DataCashing.EditTopping.SysItemID);
                if (!resultstock)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = DataCashing.EditTopping.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = DataCashing.EditTopping.SysItemID,
                    BalanceStock = 0,
                    MinimumStock = 0,
                    LastDateBalanceStock = DateTime.UtcNow
                };

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
                Toast.MakeText(this.Activity, PostDataTrackStockClose.Message, ToastLength.Long).Show();
                return false;
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
                Toast.MakeText(this.Activity, "UpdateOpenStock" + PostDataTrackStockOpen.Message, ToastLength.Long).Show();
                return false;
            }
        }

        async Task GetStockData()
        {
            try
            {
                ItemOnBranchManage onBranchManage = new ItemOnBranchManage();
                //BalanceStock กับ MinimumStock
                if (DataCashing.EditTopping.SysItemID != 0 & DataCashing.CheckNet)
                {
                    GetDataStock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, (int)DataCashing.EditTopping.SysItemID);
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
                        txtMinimumStock.Text = GetDataStock.MinimumStock.ToString("#,##0");

                        if (DataCashing.EditTopping.FTrackStock == 0)
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
                else if (DataCashing.EditTopping.SysItemID != 0 & !DataCashing.CheckNet)
                {
                    getBalance = await onBranchManage.GetItemOnBranch((int)DataCashingAll.MerchantId, (int)DataCashingAll.SysBranchId, (int)DataCashing.EditTopping.SysItemID);
                    if (getBalance != null)
                    {
                        txtStock.Text = getBalance.BalanceStock.ToString();
                        txtMinimumStock.Text = getBalance.MinimumStock.ToString("#,##0");

                        if (DataCashing.EditTopping.FTrackStock == 0)
                        {
                            switchStock.Checked = false;
                            lnSwithStcok.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            switchStock.Checked = true;
                            lnSwithStcok.Visibility = ViewStates.Visible;
                            lnShowStock.Visibility = ViewStates.Visible;
                            lnShowStock.Enabled = false;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        void ShowDetailTopping()
        {
            try
            {
                if (DataCashing.EditTopping != null)
                {
                    ExtraToppping = DataCashing.EditTopping;
                    textItemName.Text = ExtraToppping.ItemName;
                    textItemPrice.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(ExtraToppping.Price);
                    textcost.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(ExtraToppping.EstimateCost);

                    var cloudpath = ExtraToppping.PicturePath == null ? string.Empty : ExtraToppping.PicturePath;
                    var localpath = ExtraToppping.ThumbnailLocalPath == null ? string.Empty : ExtraToppping.ThumbnailLocalPath;

                    if (DataCashing.CheckNet)
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
                            string conColor = Utils.SetBackground(Convert.ToInt32(DataCashing.EditTopping.Colors));
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
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowToppingBeforEdit at add Extra");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                var adapterCategory = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, category_array);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCategory.Adapter = adapterCategory;

                long? category = DataCashing.EditTopping.SysCategoryID;
                if (category != null)
                {
                    var data = lstcategory.Where(x => x.SysCategoryID == category).FirstOrDefault();
                    if (data != null)
                    {
                        int position = adapterCategory.GetPosition(data.Name);
                        spnCategory.SetSelection(position);
                    }
                    else
                    {
                        int position = adapterCategory.GetPosition("None");
                        spnCategory.SetSelection(position);
                    }
                }
                else
                {
                    int position = adapterCategory.GetPosition("None");
                    spnCategory.SetSelection(position);
                }
                SyscategoryID = category == null ? 0 : Convert.ToInt32(category);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SpinnerSelectCategory at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        ItemManage itemManage = new ItemManage();
        private void Additem_Click(object sender, EventArgs e)
        {
            try
            {
                btnAddTopping.Enabled = false;
                bool CheckDup = false;
                CheckDup = CheckDuplicateData(textItemName.Text.ToLower());
                if (!CheckDup)
                {
                    btnAddTopping.Enabled = true;
                    var fragmenta = new Topping_Dialog_Dublicate();
                    fragmenta.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Topping_Dialog_Dublicate));
                    return;
                }
                ManageTopping();
                btnAddTopping.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAddTopping.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Additem_Click at addtopping");
                Log.Debug("error", ex.Message);
            }
            
        }

        private bool CheckDuplicateData(string itemname)
        {
            if (DataCashing.EditTopping == null)
            {
                //Check ชื่อสินค้า 
                var checkname = Item_Fragment_Main.itemExtra.FindIndex(x => !string.IsNullOrEmpty(x.ItemName) && x.ItemName.ToLower().Equals(itemname));
                if (checkname != -1)
                {
                    return false;
                }
            }
            else
            {
                //Check ชื่อสินค้า                 
                if (itemname.Trim() !=  DataCashing.EditTopping.ItemName && itemname.Trim() != string.Empty)
                {
                    var checkname = Item_Fragment_Main.itemExtra.FindIndex(x => !string.IsNullOrEmpty(x.ItemName) && x.ItemName.Equals(itemname));
                    if (checkname != -1)
                    {
                        return false;
                    }
                }
            }
            return true;    
        }

        public async void ManageTopping()
        {
            try
            {
                bool check = false;
                if (DataCashing.EditTopping == null)
                {
                    int count = itemManage.CountItem();
                    if (count <= 10000)
                    {
                        check = await InsertTopping();
                        if (!check) return;
                    }
                }
                else
                {
                    string Role = LoginType;
                    bool checkpermission = UtilsAll.CheckPermissionRoleUser(Role, "update", "topping");
                    if (!checkpermission)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                        return;
                        
                    }
                    check = await UpdateToppping();
                    if (!check) return;
                }
                SetClearData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ManageTopping at addItem");
            }
        }

        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        Item AddToppping = new Item();

        string pathThumnailFolder, pathFolderPicture;
        public async Task<bool> InsertTopping()
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

                if (textItemName.Text.Length > 5)
                {
                    AddToppping.ShortName = textItemName.Text.Substring(0, 5);
                }
                else
                {
                    AddToppping.ShortName = textItemName.Text;
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
                if (string.IsNullOrEmpty(txtMinimumStock.Text))
                {
                    minimumStock = 0;
                }
                else
                {
                    minimumStock = ConvertToDecimal(txtMinimumStock.Text.Trim());
                }

                var Price = textItemPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textItemPrice.Text;
                }
                AddToppping.Price = ConvertToDecimal(Price);

                //Topping มี cost ไหม
                if (string.IsNullOrEmpty(textcost.Text))
                {
                    //ถ้า User ไม่กำหนด ต้นทุน ระบบจะนำเอาราคาขายที่ตั้งมาเป็นต้นทุน
                    AddToppping.EstimateCost = ConvertToDecimal(Price);
                }
                else
                {
                    var Cost = textcost.Text.Trim();
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        Cost = textcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        Cost = textcost.Text;
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

                if (checkManageStock)
                {
                    if (AddToppping.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(txtStock.Text))
                        {
                            Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                            return false;
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.insertdataitem), ToastLength.Long).Show();
                        return false;
                    }
                }
                else
                {
                    itemOnBranch = null;
                }

                var result = await itemManage.InsertItem(AddToppping, itemOnBranch, null);
                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
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

                Item_Fragment_Main.fragment_main.ReloadTopping(AddToppping);
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertToppping at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, "inserttopping : " + ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public string SetAlphaColor(string colorSelected)
        {
            string[] color = colorSelected.Split("#");
            string colorReplace = "#80" + color[1];
            return colorReplace;
        }

        bool first = true;
        public static  bool flagdatachange = false;
        static string stockOnhabd;
        internal static void SetOnhand(string text)
        {
            fragment_addtopping.txtStock.Text = text;
            stockOnhabd = text;

            if (checkManageStock)
            {
                //open stock
                fragment_addtopping.lnSwithStcok.Visibility = ViewStates.Visible;
                if (!string.IsNullOrEmpty(stockOnhabd))
                {
                    fragment_addtopping.txtStock.Text = Convert.ToInt32(Utils.CheckLenghtValue(stockOnhabd)).ToString("#,###");
                    stockOnhabd = string.Empty;
                }
            }
        }
        private void SetUIFromMainRole(string loginType)
        {
            var check = UtilsAll.CheckPermissionRoleUser(loginType, "insert", "item");
            if (check && DataCashing.CheckNet)
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
                lnFavorite.Enabled = true;
                textItemName.Enabled = true;
                textItemName.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textItemName.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textItemPrice.Enabled = true;
                textItemPrice.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textItemPrice.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textcost.Enabled = true;
                textcost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textcost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                lnCategory.Enabled = true;
                spnCategory.Enabled = true;
                btnCategory.Enabled = true;
                btnCategory.SetBackgroundResource(Resource.Mipmap.Next);
                switchStock.Enabled = true;
                txtStock.Enabled = true;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnAddTopping.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAddTopping.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else if (check && !DataCashing.CheckNet)
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
                lnFavorite.Enabled = true;
                textItemName.Enabled = true;
                textItemName.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textItemName.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textItemPrice.Enabled = true;
                textItemPrice.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textItemPrice.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textcost.Enabled = true;
                textcost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textcost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                lnCategory.Enabled = true;
                spnCategory.Enabled = true;
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
                lnFavorite.Enabled = false;
                textItemName.Enabled = false;
                textItemName.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                textItemName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                textItemPrice.Enabled = false;
                textItemPrice.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                textItemPrice.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                textcost.Enabled = false;
                textcost.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                textcost.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                lnCategory.Enabled = false;
                spnCategory.Enabled = false;
                btnCategory.Enabled = false;
                btnCategory.SetBackgroundResource(Resource.Mipmap.NextG);
                switchStock.Enabled = false;
                txtStock.Enabled = false;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtMinimumStock.Enabled = false;
                txtMinimumStock.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtMinimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnAddTopping.SetBackgroundResource(Resource.Drawable.btnWhiteBorderGrayRD5);
                btnAddTopping.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
            }

            check = UtilsAll.CheckPermissionRoleUser(loginType, "delete", "item");
            if (check)
            {
                btnDelete.Visibility = ViewStates.Visible;
                if (DataCashing.EditTopping == null)
                {
                    btnDelete.Visibility = ViewStates.Gone;
                }
                else
                {
                    btnDelete.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                btnDelete.Visibility = ViewStates.Gone;
            }
        }


        bool favorite, HavePicture = false;
        string CURRENCYSYMBOLS;
        private void SetUIEvent()
        {
            favorite = false;
            txtStock.TextChanged += TxtStock_TextChanged; 
            txtMinimumStock.TextChanged += TxtMinimumStock_TextChanged; 
            txtMinimumStock.FocusChange += TxtMinimumStock_FocusChange; 
            imageViewItem.Click += ImageViewItem_Click; 
            txtPricePic.Hint = Utils.DisplayDecimal(0);
            lnBack.Click += LnBack_Click;
            lnBtnShowDetail.Click += LnBtnShowDetail_Click;
            textItemName.TextChanged += TextItemName_TextChanged;            
            textItemPrice.TextChanged += TextItemPrice_TextChanged;
            textItemPrice.KeyPress += TextItemPrice_KeyPress;
            textcost.Hint = Utils.DisplayDecimal(0);
            textcost.TextChanged += Textcost_TextChanged;
            textcost.KeyPress += Textcost_KeyPress;
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
            switchStock.CheckedChange += SwitchStock_CheckedChange;
            lnOnhand.Click += LnOnhand_Click;
            txtStock.Click += LnOnhand_Click;
            lnStockMoveMent.Click += LnStockMoveMent_Click;
            lnFavorite.Click += LnFavorite_Click;
            btnAddTopping.Click += Additem_Click;
            btnDelete.Click += BtnDelete_Click;
        }
    
        List<string> itemID = new List<string>();
        public async void SpinnerCategory()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                itemID = new List<string>();
               
                Category addcategory = new Category();
                List<Category> category = new List<Category>();
                List<Category> getallCategory = new List<Category>();

                addcategory = new Category()
                {
                    Name = "None Category",
                    SysCategoryID = 0
                };
                category.Add(addcategory);
                getallCategory = MainActivity.allData.DefaultDataCategory;
                category.AddRange(getallCategory);

                for (int i = 0; i < category.Count; i++)
                {
                    temp = category[i].Name.ToString();
                    temp2 = category[i].SysCategoryID.ToString();
                    items.Add(temp);
                    itemID.Add(temp2);
                }

                spnCategory.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerCategory_ItemSelected);
                var adapterCategory = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCategory.Adapter = adapterCategory;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SpinnerCategory at add Extra");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        long? SyscategoryID;

        private void spinnerCategory_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (itemID == null)
            {
                return;
            }

            SyscategoryID = Convert.ToInt32(itemID[e.Position].ToString());

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

        private void LnFavorite_Click(object sender, EventArgs e)
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
        private void LnStockMoveMent_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataCashing.EditTopping == null)
                {
                    return;
                }

                var fragment = new AddItem_Dialog_MoveMent();
                fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(AddItem_Dialog_MoveMent));
                return;
            }
            catch (Exception ex)
            {

                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtStock_Click at add AddItem");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        decimal showStock;
        private void LnOnhand_Click(object sender, EventArgs e)
        {
            try
            {
                var fragment = new AddItem_Dialog_OnHand();
                AddItem_Dialog_OnHand.SetPage("addtopping");
                fragment.Show(this.Activity.SupportFragmentManager, nameof(AddItem_Dialog_OnHand));
            }
            catch (Exception ex)
            {

                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtStock_Click at add AddItem");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
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
                        checkManageStock = false;
                    }
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
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

        private void Btnaddpic_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.flagChooseMedia = true;
                Bundle bundle = new Bundle();
                var fragment = new AddItem_Dialog_SelectMedia();
                fragment.Show(this.Activity.SupportFragmentManager, nameof(AddItem_Dialog_SelectMedia));
            }
            catch (Exception ex)
            {

                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btnaddpic_Click at add AddItem");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
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
        private void BtnCategory_Click(object sender, EventArgs e)
        {
            spnCategory.PerformClick();
        }

        private void Textcost_KeyPress(object sender, View.KeyEventArgs e)
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
                        txtCost = textcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtCost = textcost.Text;
                    }

                    if (txtCost.Trim().Length == 0)
                    {
                        return;
                    }
                    var Price = Convert.ToDecimal(txtCost);
                    //textCurrency.Visibility = ViewStates.Visible;
                    textcost.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(Price);
                    textcost.SetSelection(textcost.Text.Length);
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
                    textcost.Text += input;
                    textcost.SetSelection(textcost.Text.Length);
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("Txtcost_KeyPress at add Extra");
            }

        }

        private void Textcost_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (textcost.Text.Length == 0)
                {
                    return;
                }

                if (textcost.Text == ".")
                {
                    return;
                }

                string[] txt = new string[] { };
                int checkIndex = 0;
                if (textcost.Text.Contains('.'))
                {
                    checkIndex = textcost.Text.IndexOf('.');
                    if (checkIndex == -1)
                    {
                        return;
                    }

                    txt = textcost.Text.Split('.');
                    if (!string.IsNullOrEmpty(txt[1]))
                    {
                        if (txt[1].Length > Convert.ToInt32(DecimalDisplay))
                        {
                            string Amount = textcost.Text;
                            textcost.Text = Amount.Remove(Amount.Length - 1);
                            textcost.SetSelection(textcost.Text.Length);
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txt[0]))
                    {
                        if ((txt[0].Length) > 13)
                        {
                            textcost.Text = txt[0].Remove(13, 1);
                            textcost.SetSelection(textcost.Text.Length);
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
                        cost = textcost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        cost = textcost.Text;
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Txtcost_TextChanged at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void TextItemPrice_KeyPress(object sender, View.KeyEventArgs e)
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
                        txtPrice = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtPrice = textItemPrice.Text;
                    }

                    if (txtPrice.Trim().Length == 0)
                    {
                        return;
                    }
                    var Price = Convert.ToDecimal(txtPrice);
                    //textCurrency.Visibility = ViewStates.Visible;
                    textItemPrice.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(Price);
                    textItemPrice.SetSelection(textItemPrice.Text.Length);
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
                    textItemPrice.Text += input;
                    textItemPrice.SetSelection(textItemPrice.Text.Length);
                    return;
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_KeyPress at add Extra");
            }
        }

        private void TextItemPrice_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (textItemPrice.Text.Length == 0)
                {
                    return;
                }

                if (textItemPrice.Text == ".")
                {
                    return;
                }

                string[] txt = new string[] { };
                int checkIndex = 0;
                if (textItemPrice.Text.Contains('.'))
                {
                    checkIndex = textItemPrice.Text.IndexOf('.');
                    if (checkIndex == -1)
                    {
                        return;
                    }

                    txt = textItemPrice.Text.Split('.');
                    if (!string.IsNullOrEmpty(txt[1]))
                    {
                        if (txt[1].Length > Convert.ToInt32(DecimalDisplay))
                        {
                            string Amount = textItemPrice.Text;
                            textItemPrice.Text = Amount.Remove(Amount.Length - 1);
                            textItemPrice.SetSelection(textItemPrice.Text.Length);
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txt[0]))
                    {
                        if ((txt[0].Length) > 13)
                        {
                            textItemPrice.Text = txt[0].Remove(13, 1);
                            textItemPrice.SetSelection(textItemPrice.Text.Length);
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
                        txtPrice = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtPrice = textItemPrice.Text;
                    }

                    if (txtPrice.Trim().Length == 0)
                    {
                        return;
                    }

                    if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(txtPrice))
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxamount) + " " + maxdata, ToastLength.Short).Show();
                        textItemPrice.Text = maxdata;
                        textItemPrice.SetSelection(textItemPrice.Text.Length);
                        return;
                    }
                }

                //SetItemView();
                //CheckDataChange();

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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_TextChanged at add Extra");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

        }

        private void TextItemName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            ToppingName = textItemName.Text.Trim();
            if (!HavePicture)
            {
                SetItemView();
            }
            else
            {
                CheckDataChange();
            }
        }
        private async void SetItemView()
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

                var itemname = textItemName?.Text.Trim();
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

                var itemprice = textItemPrice?.Text.Trim();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
        }
        decimal ConvertToDecimal(string txt)
        {
            decimal decimalValue = 0;
            decimal.TryParse(txt, out decimalValue);
            return decimalValue;
        }
        bool showdetail;

        private void LnBtnShowDetail_Click(object sender, EventArgs e)
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
            //SetUIFromMainRole(LoginType);
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
       
        private void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flagdatachange)
                {
                    SetClearData(); return;
                }

                if (DataCashing.EditTopping == null)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Add_Dialog_Back.SetPage("topping");
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
                    Edit_Dialog_Back.SetPage("topping");
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

        private void ImageViewItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = "";
                if (ExtraToppping != null)
                {
                    if (!string.IsNullOrEmpty(ExtraToppping.PicturePath))
                    {
                        path = ExtraToppping.PicturePath;
                    }
                    else
                    {
                        path = ExtraToppping.PictureLocalPath;
                    }

                    Item_Dialog_ShowImage dialog_Item = Item_Dialog_ShowImage.NewInstance();
                    Item_Dialog_ShowImage.SetPath(path);
                    dialog_Item.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Item_Dialog_ShowImage));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgProfile_Click at add add Topping");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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

        private void TxtMinimumStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            int max = 999999;
            var remove = Utils.CheckLenghtValue(txtMinimumStock.Text);
            int value = string.IsNullOrEmpty(remove) ? 0 : int.Parse(remove);
            if (max < value)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.maxitem) + " " + max.ToString("#,###"), ToastLength.Short).Show();
                txtMinimumStock.Text = max.ToString("#,###");
                txtMinimumStock.SetSelection(txtMinimumStock.Text.Length);
                CheckDataChange();
                return;
            }
            //EditStock = true;
            CheckDataChange();
        }

        private void TxtStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();

        }
        ItemOnBranch itemOnBranch, GetDataStock, getBalance;

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAddTopping.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAddTopping.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAddTopping.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAddTopping.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAddTopping.Enabled = enable;
        }
        public void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (DataCashing.EditTopping == null)
            {
                if (switchStock.Checked)
                {
                    flagdatachange = true;
                }
                if (string.IsNullOrEmpty(textItemName.Text))
                {
                    SetButtonAdd(false);
                    return;
                }  
                string txtPrice;
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    txtPrice = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    txtPrice = textItemPrice.Text;
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
                if (string.IsNullOrEmpty(textItemName.Text))
                {
                    return;
                }
                if (textItemName.Text != ExtraToppping.ItemName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                decimal insertPrice = 0;
                string txtPrice;
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    txtPrice = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    txtPrice = textItemPrice.Text;
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
                    txtCost = textcost.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    txtCost = textcost.Text;
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

                if (DataCashing.CheckNet)
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
                        decimal.TryParse(txtMinimumStock.Text, out minist);
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

                var category = DataCashing.EditTopping.SysCategoryID == null ? 0 : Convert.ToInt32(DataCashing.EditTopping.SysCategoryID);
                if (SyscategoryID != category)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
                //SetUIFromMainRole(LoginType);
            }
        }

        public void SetClearData()
        {
            UINewItem();
            ExtraToppping = null;
            DataCashing.flagChooseMedia = false;
            DataCashing.EditTopping = null;
            flagdatachange = false;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem, "item", "default");
        }


        TextView txtTitle;
        FrameLayout lnBack;
        RecyclerView rcvHeaderItem;
        LinearLayout lnShowItem;
        ScrollView scvItem;
        ImageView colorViewItem;
        internal static ImageView imageViewItem;
        EditText txtViewItemnameTitle;
        TextView txtItemNamePic, txtPricePic;
        ImageButton btncolor1, btncolor2, btncolor3, btncolor4, btncolor5, btncolor6, btncolor7, btncolor8, btncolor9, btnaddpic;
        ImageView imgFavorite;
        FrameLayout lnFavorite;
        EditText textItemPrice;
        public static EditText textItemName;
        ImageButton btnShowDetail;
        FrameLayout lnBtnShowDetail;
        LinearLayout lnDetails;
        EditText textcost;
        Spinner spnCategory;
        Button btnCategory;
        LinearLayout lnCategory;
        Switch switchStock;
        LinearLayout lnShowStock, lnSwithStcok, lnOnhand, lnStockMoveMent;
        TextView txtStock;
        EditText txtMinimumStock;
        FrameLayout btnDelete;
        internal Button btnAddTopping;
        private void ComBineUI()
        {
            txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
            lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
            rcvHeaderItem = view.FindViewById<RecyclerView>(Resource.Id.rcvHeaderItem);
            lnShowItem = view.FindViewById<LinearLayout>(Resource.Id.lnShowItem);
            scvItem = view.FindViewById<ScrollView>(Resource.Id.scvItem);
            colorViewItem = view.FindViewById<ImageView>(Resource.Id.colorViewItem);
            imageViewItem = view.FindViewById<ImageView>(Resource.Id.imageViewItem);
            txtViewItemnameTitle = view.FindViewById<EditText>(Resource.Id.txtViewItemnameTitle);
            txtItemNamePic = view.FindViewById<TextView>(Resource.Id.txtItemNamePic);
            txtPricePic = view.FindViewById<TextView>(Resource.Id.txtPricePic);
            btncolor1 = view.FindViewById<ImageButton>(Resource.Id.btncolor1);
            btncolor2 = view.FindViewById<ImageButton>(Resource.Id.btncolor2);
            btncolor3 = view.FindViewById<ImageButton>(Resource.Id.btncolor3);
            btncolor4 = view.FindViewById<ImageButton>(Resource.Id.btncolor4);
            btncolor5 = view.FindViewById<ImageButton>(Resource.Id.btncolor5);
            btncolor6 = view.FindViewById<ImageButton>(Resource.Id.btncolor6);
            btncolor7 = view.FindViewById<ImageButton>(Resource.Id.btncolor7);
            btncolor8 = view.FindViewById<ImageButton>(Resource.Id.btncolor8);
            btncolor9 = view.FindViewById<ImageButton>(Resource.Id.btncolor9);
            btnaddpic = view.FindViewById<ImageButton>(Resource.Id.btnaddpic);
            imgFavorite = view.FindViewById<ImageView>(Resource.Id.imgFavorite);
            lnFavorite = view.FindViewById<FrameLayout>(Resource.Id.lnFavorite);
            textItemName = view.FindViewById<EditText>(Resource.Id.textItemName);
            textItemPrice  = view.FindViewById<EditText>(Resource.Id.textItemPrice);
            btnShowDetail = view.FindViewById<ImageButton>(Resource.Id.btnShowDetail);
            lnBtnShowDetail = view.FindViewById<FrameLayout>(Resource.Id.lnBtnShowDetail);
            lnDetails = view.FindViewById<LinearLayout>(Resource.Id.lnDetails);
            textcost = view.FindViewById<EditText>(Resource.Id.textcost);
            lnCategory = view.FindViewById<LinearLayout>(Resource.Id.lnCategory);
            spnCategory = view.FindViewById<Spinner>(Resource.Id.spnCategory);
            btnCategory = view.FindViewById<Button>(Resource.Id.btnCategory);
            lnShowStock = view.FindViewById<LinearLayout>(Resource.Id.lnShowStock);
            switchStock = view.FindViewById<Switch>(Resource.Id.switchStock);
            lnSwithStcok = view.FindViewById<LinearLayout>(Resource.Id.lnSwithStcok);
            lnOnhand = view.FindViewById<LinearLayout>(Resource.Id.lnOnhand);
            txtStock = view.FindViewById<TextView>(Resource.Id.txtStock);
            txtMinimumStock = view.FindViewById<EditText>(Resource.Id.txtMinimumStock);
            lnStockMoveMent = view.FindViewById<LinearLayout>(Resource.Id.lnStockMoveMent);
            btnDelete = view.FindViewById<FrameLayout>(Resource.Id.btnDelete);
            btnAddTopping = view.FindViewById<Button>(Resource.Id.btnAddTopping);

        }
    }
}