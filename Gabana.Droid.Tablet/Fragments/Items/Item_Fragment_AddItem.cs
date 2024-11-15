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
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.DataModel.Items;
using Gabana3.JAM.Items;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqToDB.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Items
{
    public class Item_Fragment_AddItem : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Item_Fragment_AddItem NewInstance()
        {
            Item_Fragment_AddItem frag = new Item_Fragment_AddItem();
            return frag;
        }
        public static Item_Fragment_AddItem fragment_additem;
        View view;
        public static bool checkNet = false;
        string usernamelogin, LoginType;
        string colorSelected;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.item_fragment_additem, container, false);
            try
            {
                fragment_additem = this;
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
        static string stockOnhabd;
        internal static void SetOnhand(string text)
        {
            fragment_additem.txtStock.Text = text;
            stockOnhabd = text;

            if (checkManageStock)
            {
                //open stock
                fragment_additem.lnSwithStcok.Visibility = ViewStates.Visible;
                if (!string.IsNullOrEmpty(stockOnhabd))
                {
                    fragment_additem.txtStock.Text = Convert.ToInt32(Utils.CheckLenghtValue(stockOnhabd)).ToString("#,###");
                    stockOnhabd = string.Empty;
                }
            }
        }

        bool HavePicture = false;
        internal static Android.Net.Uri keepCropedUri;
        string CURRENCYSYMBOLS;
        private void SetItemView()
        {
            try
            {
                Utils.AddNullValue();
                imageViewItem.Visibility = ViewStates.Gone;
                txtViewItemnameTitle.Visibility = ViewStates.Visible;

                btnAdditem.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
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

                var itemprice = textItemPrice?.Text.Trim();
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
                Toast.MakeText(this.Activity, "SetItemView" + ex.Message, ToastLength.Short).Show();
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
        public static  bool first = true, flagdatachange = false;
        public static Item itemEdit = new Item();
        public static bool favorite, favoritefromPOS; 
        ItemOnBranch itemOnBranch, DataStock, getBalance;
        static bool EditStock = false;
        long SyscategoryID;
        decimal showDisplay, showStock;
        char TaxType;
        ItemManage ItemManage = new ItemManage();
        ItemExSizeManage ItemExSizeManage = new ItemExSizeManage();
        List<ItemExSize> lstExSize = new List<ItemExSize>();
        List<ItemExSize> newlsItemExSize = new List<ItemExSize>();
        List<ItemExSize> TemplsItemExSize = new List<ItemExSize>();
        public void CheckDataChange()
        {
            try
            {
                if (first)
                {
                    SetButtonAdd(false);
                    return ;
                }
                if (DataCashing.EditItem == null)
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
                    if (string.IsNullOrEmpty(textItemName.Text) || string.IsNullOrEmpty(textItemPrice.Text))
                    {
                        SetButtonAdd(false);
                        flagdatachange = false;
                        return;
                    }
                    if (!string.IsNullOrEmpty(textItemName.Text))
                    {
                        if (textItemName.Text != itemEdit.ItemName)
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
                        txtPrice = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtPrice = textItemPrice.Text;
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
                    else
                    {
                        SetButtonAdd(false);
                        flagdatachange = false;
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
                    if (DataCashing.CheckNet)
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
                    var category = DataCashing.EditItem.SysCategoryID == null ? 0 : Convert.ToInt32(DataCashing.EditItem.SysCategoryID);                   
                    if (SyscategoryID != category)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    string ItemEdit = string.IsNullOrEmpty(itemEdit.ItemCode) ? "" : itemEdit.ItemCode;
                    string ItemCodeitem = string.IsNullOrEmpty(textItemCode.Text) ? "" : textItemCode.Text;
                    if (ItemEdit != ItemCodeitem)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    string txtCost;
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        txtCost = textCost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtCost = textCost.Text;
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
                        switchStock.Enabled = false;
                        switchStock.Checked = false;
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
                }
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "CheckDataChange " + ex.Message);
            }
        }
        private void SetUIFromMainRole(string loginType)
        {
            var checkRole = UtilsAll.CheckPermissionRoleUser(loginType, "insert", "item");
            if (checkRole && DataCashing.CheckNet)
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
                textItemCode.Enabled = true;
                textItemCode.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textItemCode.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textCost.Enabled = true;
                textCost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textCost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                spnCategory.Enabled = true;
                lnCategory.Enabled = true;
                btnCategory.SetBackgroundResource(Resource.Mipmap.Next);
                spnVat.Enabled = true;
                btnVat.SetBackgroundResource(Resource.Mipmap.Next);
                lnVat.Enabled = true;
                btnAddSize.Enabled = true;
                btnAddSize.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAddSize.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
                switchStock.Enabled = true;
                switchStock.Enabled = true;
                txtStock.Enabled = true;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtMinimumStock.Enabled = true;
                txtMinimumStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtMinimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAdditem.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                imgScanItem.SetBackgroundResource(Resource.Mipmap.ScanItem);
                lnScanItem.Enabled = true;
            }
            else if (checkRole && !DataCashing.CheckNet)
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
                textItemCode.Enabled = true;
                textItemCode.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textItemCode.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                textCost.Enabled = true;
                textCost.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                textCost.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                spnCategory.Enabled = true;
                lnCategory.Enabled = true;
                btnCategory.SetBackgroundResource(Resource.Mipmap.Next);
                spnVat.Enabled = true;
                btnVat.SetBackgroundResource(Resource.Mipmap.Next);
                lnVat.Enabled = true;
                btnAddSize.Enabled = true;
                btnAddSize.SetBackgroundResource(Resource.Drawable.btnWhiteBorderGray);
                btnAddSize.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
                switchStock.Enabled = true;
                switchStock.Enabled = false;
                txtStock.Enabled = false;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtMinimumStock.Enabled = false;
                txtMinimumStock.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtMinimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAdditem.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
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
                lnFavorite.Enabled = false;
                textItemName.Enabled = false;
                textItemName.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                textItemName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                textItemPrice.Enabled = false;
                textItemPrice.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                textItemPrice.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                textItemCode.Enabled = false;
                textItemCode.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                textItemCode.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                textCost.Enabled = false;
                textCost.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                textCost.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                spnCategory.Enabled = false;
                lnCategory.Enabled = false;
                btnCategory.SetBackgroundResource(Resource.Mipmap.NextG);
                spnVat.Enabled = false;
                btnVat.SetBackgroundResource(Resource.Mipmap.NextG);
                lnVat.Enabled = false;
                btnAddSize.Enabled = false;
                btnAddSize.SetBackgroundResource(Resource.Drawable.btnWhiteBorderGrayOval);
                btnAddSize.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                switchStock.Enabled = false;
                switchStock.Enabled = false;
                txtStock.Enabled = false;
                txtStock.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtMinimumStock.Enabled = false;
                txtMinimumStock.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtMinimumStock.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnWhiteBorderGrayRD5);
                btnAdditem.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                imgScanItem.SetBackgroundResource(Resource.Mipmap.ScanCodeG);
                lnScanItem.Enabled = false;
            }


            checkRole = UtilsAll.CheckPermissionRoleUser(loginType, "delete", "item");
            if (checkRole)
            {
                if (DataCashing.EditItem == null)
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
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAdditem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdditem.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAdditem.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAdditem.Enabled = enable;
        }
        decimal ConvertToDecimal(string txt)
        {
            decimal decimalValue = 0;
            decimal.TryParse(txt, out decimalValue);
            return decimalValue;
        }

        string pathThumnailFolder, pathFolderPicture;
        string path, DecimalDisplay;
        public long SysItemId;
        private async Task SetDetailItem()
        {
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
            if (CURRENCYSYMBOLS == null) CURRENCYSYMBOLS = "฿";
            textItemPrice.Hint = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(0);

            pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
            pathFolderPicture = DataCashingAll.PathFolderImage;

            spnVat.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerVat_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinnervat, Resource.Layout.spinner_item);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnVat.Adapter = adapter;

            DecimalDisplay = DataCashingAll.setmerchantConfig?.DECIMAL_POINT_DISPLAY;
            if (DecimalDisplay == null) DecimalDisplay = "2";
            if (DataCashing.EditItem == null)
            {
                txtTitle.Text = GetString(Resource.String.additem_activity_title);
                btnAdditem.Text = GetString(Resource.String.additem_activity_additem);
                btnDelete.Visibility = ViewStates.Gone;                
            }
            else
            {
                txtTitle.Text = GetString(Resource.String.edititem_activity_title);
                btnAdditem.Text = GetString(Resource.String.textsave); 
                SysItemId = DataCashing.EditItem.SysItemID;                
                ShowItemForEdit();
                await GetStockData();
                btnDelete.Visibility = ViewStates.Visible;
            }

            await GetItemList();
            SetTabMenu();
            SetTabShowMenu();

            await ShowItemExSize();
            SetFavorite();
            ShowDetailItem();
        }

        private void spinnerVat_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string selectVAT = spnVat.SelectedItem.ToString();
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
                GridLayoutManager gridLayoutManager = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvSize.HasFixedSize = true;
                rcvSize.SetLayoutManager(gridLayoutManager);

                rcvSize.SetAdapter(addItem_adapter_size);

                if (addItem_adapter_size.ItemCount > 0)
                {
                    btnAddSize.Visibility = ViewStates.Gone;
                    lnAddSize.Visibility = ViewStates.Visible;
                }
                else
                {
                    btnAddSize.Visibility = ViewStates.Visible;
                    lnAddSize.Visibility = ViewStates.Gone;
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
                Toast.MakeText(this.Activity, "ShowItemExSize" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public  void DeleteExSize(ItemExSize PositionDelete)
        {
            try
            {
                fragment_additem.EditItemExSize();

                var item = fragment_additem.newlsItemExSize.Find(x => x.ExSizeNo == PositionDelete.ExSizeNo);
                fragment_additem.newlsItemExSize.Remove(item);

                ListItemExSize lstItemExSize = new ListItemExSize(fragment_additem.newlsItemExSize);
                AddItem_Adapter_Size addItem_adapter_size = new AddItem_Adapter_Size(lstItemExSize);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(MainActivity.main_activity, 1, 1, false);
                rcvSize.HasFixedSize = true;
                rcvSize.SetLayoutManager(gridLayoutManager);
                rcvSize.SetAdapter(addItem_adapter_size);
                var layout = addItem_adapter_size.ItemCount;
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                //lnExtrasize.LayoutParameters.Height = Convert.ToInt32(190* layout);
                fragment_additem.OnResume();
                rcvSize.ScrollToPosition(layout - 1);

                if (addItem_adapter_size.ItemCount > 0)
                {
                    btnAddSize.Visibility = ViewStates.Gone;
                    lnAddSize.Visibility = ViewStates.Visible;
                    switchStock.Enabled = false;
                    switchStock.Checked = false;
                }
                else
                {
                    btnAddSize.Visibility = ViewStates.Visible;
                    lnAddSize.Visibility = ViewStates.Gone;
                    switchStock.Enabled = true;
                    switchStock.Checked = true;
                }

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("DeleteExSize at add Item");
                Log.Debug("error", ex.Message);
                Toast.MakeText(MainActivity.main_activity, "DeleteExSize" + ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetTabShowMenu()
        {
            try
            {
                if (string.IsNullOrEmpty(tabSelected))
                {
                    tabSelected = "Item";
                }
                GridLayoutManager menuLayoutManager = new GridLayoutManager(this.Activity, 2, 1, false);
                rcvHeaderItem.HasFixedSize = true;
                rcvHeaderItem.SetLayoutManager(menuLayoutManager);
                AddItem_Adapter_Header additem_adapter_header = new AddItem_Adapter_Header(menuTab, "item");
                rcvHeaderItem.SetAdapter(additem_adapter_header);
                additem_adapter_header.ItemClick += Additem_adapter_header_ItemClick; 
                lnShowItem.Visibility = ViewStates.Gone;
                switch (tabSelected)
                {
                    case "Item":
                        lnShowItem.Visibility = ViewStates.Visible;
                        break;
                    case "Stock":
                        lnShowStock.Visibility = ViewStates.Visible;
                        if ((DataStock == null & DataCashing.CheckNet) | (itemEdit?.FTrackStock == 0) | (getBalance == null & !DataCashing.CheckNet))
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
                        lnShowItem.Visibility = ViewStates.Gone;
                        break;
                }
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "Error AddItem" + "SetTabShowMenu");
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetTabShowMenu at add Item");
                Toast.MakeText(this.Activity, "SetTabShowMenu" + ex.Message, ToastLength.Short).Show();
            }
        }
        private void Additem_adapter_header_ItemClick(object sender, int e)
        {
            try
            {
                if (DataCashing.EditItem == null & !DataCashing.CheckNet)
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
        public List<MenuTab> menuTab { get; set; }
        private void SetTabMenu()
        {
            menuTab = new List<MenuTab>
            {
                new MenuTab() { NameMenuEn = "Item" , NameMenuTh = "สินค้า" },
                new MenuTab() { NameMenuEn = "Stock" , NameMenuTh = "สต็อก" }
            };
        }

        public static string tabSelected;
        bool showdetail;
       
        public async Task<bool> UpdateItem()
        {
            try
            {
                Gabana.ORM.MerchantDB.Item editItem = new Gabana.ORM.MerchantDB.Item();
                Item getItems = new Item();
                getItems = DataCashing.EditItem;

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
                if (textItemName.Text.Trim() != getItems.ItemName && textItemName.Text.Trim() != string.Empty)
                {
                    editItem.ItemName = textItemName.Text.Trim();
                }
                else
                {
                    editItem.ItemName = getItems.ItemName;
                }

                //editItem.ShortName = txtViewItemnameTitle.Text?.ToString();
                if (string.IsNullOrEmpty(txtViewItemnameTitle.Text))
                {
                    if (textItemName.Text.Length > 6)
                    {
                        editItem.ShortName = textItemName.Text.Substring(0, 6);
                    }
                    else
                    {
                        editItem.ShortName = textItemName.Text;
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

                editItem.ItemCode = textItemCode?.Text.Trim() ?? getItems.ItemCode;
                editItem.FavoriteNo = getItems.FavoriteNo;
                editItem.UnitName = getItems.UnitName;
                editItem.RegularSizeName = getItems.RegularSizeName;
                var Price = textItemPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textItemPrice.Text;
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

                if (getItems.FTrackStock != showStock || getItems.FTrackStock == 1 & EditStock)
                {
                    //case save stock                    
                    editItem.FTrackStock = showStock;
                    if (DataCashing.CheckNet)
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
                    editItem.TrackStockDateTime = Utils.GetTranDate(DateTime.UtcNow);
                }
                else
                {
                    editItem.FTrackStock = getItems.FTrackStock;
                    editItem.TrackStockDateTime = Utils.GetTranDate(getItems.TrackStockDateTime);
                }
                editItem.SaleItemType = getItems.SaleItemType;
                editItem.Comments = getItems.Comments;
                editItem.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                editItem.UserLastModified = usernamelogin;
                editItem.DataStatus = 'M';
                editItem.FWaitSending = 2;
                editItem.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                editItem.LinkProMaxxItemID = getItems.LinkProMaxxItemID;
                editItem.LinkProMaxxItemUnit = getItems.LinkProMaxxItemUnit;

                var Cost = textCost.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Cost = textCost.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Cost = textCost.Text;
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

                //เช็คว่าสินค้ามี size ซ้ำกันหรือไม่                
                if (showdetail)
                {
                    var resultUpdateSize = EditItemExSize();
                    if (!resultUpdateSize)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.repeatnameexsize), ToastLength.Short).Show();
                        return false;
                    }
                }

                UpdateItemExsize();

                var result = await ItemManage.UpdateItem(editItem);
                if (result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
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
                Item_Fragment_Main.fragment_main.ReloadItem(editItem);
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Update at edit Item");
                Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                return false;
            }
        }
        public bool EditItemExSize()
        {
            try
            {
                itemExSizes = new List<ItemExSize>();
                for (int i = 0; i < rcvSize.ChildCount; i++)
                {
                    View child = rcvSize.GetChildAt(i);
                    viewHolder = rcvSize.GetChildViewHolder(child);
                    vh = viewHolder as ListViewItemExSizeHolder;
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
                        return false ;
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
                Toast.MakeText(this.Activity, "EditItemExSize" + ex.Message, ToastLength.Short).Show();
                return false;
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
                resultstock = await UpdateOpenStock(DataCashingAll.SysBranchId, (int)DataCashing.EditItem.SysItemID, DataCashingAll.DeviceNo, ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)), minimumStock);
                if (!resultstock)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = DataCashing.EditItem.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = DataCashing.EditItem.SysItemID,
                    BalanceStock = ConvertToDecimal(Utils.CheckLenghtValue(txtStock.Text)),
                    MinimumStock = minimumStock,
                    LastDateBalanceStock = DateTime.UtcNow
                };
            }
            else
            {
                //Close Stock
                resultstock = await UpdateClosetock((int)DataCashing.EditItem.SysItemID);
                if (!resultstock)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                itemOnBranch = new ItemOnBranch()
                {
                    MerchantID = DataCashing.EditItem.MerchantID,
                    SysBranchID = DataCashingAll.SysBranchId,
                    SysItemID = DataCashing.EditItem.SysItemID,
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
                Toast.MakeText(this.Activity, "UpdateClosetock" + PostDataTrackStockClose.Message, ToastLength.Long).Show();
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
                if (SysItemId != 0 & DataCashing.CheckNet)
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
                else if (SysItemId != 0 & !DataCashing.CheckNet)
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
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "GetStockData" + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetStockData at add Item");
                Toast.MakeText(this.Activity, "GetStockData" + ex.Message, ToastLength.Short).Show();
            }
        }

        public void UINewItem()
        {
            try
            {
                txtTitle.Text = string.Empty;
                textItemName.Text = string.Empty;
                textItemCode.Text = string.Empty;
                textItemPrice.Text = string.Empty;
                textCost.Text = string.Empty; 
                imageViewItem.SetImageURI(null);
                colorSelected = "#0095DA";
                SysItemId = 0;
                txtItemNamePic.Text = "Item Name";
                txtViewItemnameTitle.Text = "Item Name";
                txtStock.Text = "0";
                txtMinimumStock.Text = "0";
                SetItemView();
                spnVat.SetSelection(0);
                tabSelected = "Item";                                
                spnCategory.SetSelection(0);
                SyscategoryID = 0;
                switchShowOption.Checked = false;
                switchShowOption.Enabled = true;
                favorite = false;
                showStock = 0;
                switchStock.Checked = false;
                switchStock.Enabled = true;
                showdetail = false;
                newlsItemExSize = new List<ItemExSize>();
                lstExSize = new List<ItemExSize>();               
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem Erro 5 - " + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowItemForEdit at add Item");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, "ShowItemForEdit" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        //Show Item in App (Edit)        
        public void ShowItemForEdit()
        {
            try
            {
                if (DataCashing.EditItem != null)
                {
                    itemEdit = DataCashing.EditItem;
                    textItemName.Text = itemEdit.ItemName;
                    textItemName.SetSelection(itemEdit.ItemName.Length);
                    if (!string.IsNullOrEmpty(itemEdit.ItemCode))
                    {
                        textItemCode.Text = itemEdit.ItemCode;
                        textItemCode.SetSelectAllOnFocus(true);
                    }
                    textItemPrice.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(itemEdit.Price);
                    textItemPrice.SetSelection(textItemPrice.Text.Length);
                    textCost.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(itemEdit.EstimateCost);
                    txtViewItemnameTitle.Text = itemEdit.ShortName?.ToString();

                    var cloudpath = itemEdit.PicturePath == null ? string.Empty : itemEdit.PicturePath;
                    var localpath = itemEdit.ThumbnailLocalPath == null ? string.Empty : itemEdit.ThumbnailLocalPath;
                    if (DataCashing.CheckNet)
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
                        spnVat.SetSelection(0);
                    }
                    else
                    {
                        spnVat.SetSelection(1);
                    }

                    List<Category> lstcategory = new List<Category>();
                    List<Category> getallCategory = new List<Category>();
                    Category addcategory = new Category();

                    addcategory = new Category()
                    {
                        Name = "None",
                        SysCategoryID = 0
                    };
                    lstcategory.Add(addcategory);
                    getallCategory = MainActivity.allData.DefaultDataCategory;
                    lstcategory.AddRange(getallCategory);

                    string[] category_array = lstcategory.Select(i => i.Name.ToString()).ToArray();
                    var adapterCategory = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, category_array);
                    adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spnCategory.Adapter = adapterCategory;

                    long? category = itemEdit.SysCategoryID;

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

                    SysItemId = itemEdit.SysItemID;

                    if (itemEdit.FDisplayOption == 1)
                    {
                        switchShowOption.Checked = true;
                    }
                    else
                    {
                        switchShowOption.Checked = false;
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
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowItemForEdit at add Item");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this.Activity, "ShowItemForEdit" + ex.Message, ToastLength.Short).Show();
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
        private  void BtnAdditem_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdditem.Enabled = false;
                if (string.IsNullOrEmpty(textItemName.Text))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.inputname), ToastLength.Short).Show();
                    btnAdditem.Enabled = true;
                    return;
                }

                if (string.IsNullOrEmpty(textItemPrice.Text))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.inputprice), ToastLength.Short).Show();
                    btnAdditem.Enabled = true;
                    return;
                }

                bool CheckDup = false;
                CheckDup = CheckDuplicateData(textItemName.Text, textItemCode.Text);
                if (!CheckDup)
                {
                    btnAdditem.Enabled = true;
                    var fragmenta = new Item_Dialog_Dublicate();
                    fragmenta.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Item_Dialog_Dublicate));                    
                    return;
                }
                ManageItem();
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

        public async void ManageItem()
        {
            try
            {
                bool check = false;
                if (DataCashing.EditItem == null)
                {
                    //limit Item != 'D' = 10000
                    int count = ItemManage.CountItem();
                    if (count <= 10000)
                    {
                        check = await InsertItem();
                        if (!check) return;
                    }
                }
                else
                {
                    check = await UpdateItem();
                    if (!check) return;
                }
                SetClearData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ManageItem at addItem");
            }
        }

        string sys;
        public static long SyscategoryIDfromPOS;
        Android.Graphics.Bitmap bitmap;
        public static bool checkManageStock = false;
        

        public async Task<bool> InsertItem()
        {
            try
            {
                Gabana.ORM.MerchantDB.Item addItem = new Gabana.ORM.MerchantDB.Item();
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
                addItem.ItemName = textItemName.Text.Trim();
                addItem.ShortName = txtViewItemnameTitle.Text?.Trim().ToString();
                addItem.Ordinary = 2;
                addItem.SysCategoryID = category;
                addItem.ItemCode = textItemCode?.Text.Trim() ?? "";
                addItem.PicturePath = "";
                addItem.UnitName = null;
                addItem.RegularSizeName = null;
                var Price = textItemPrice.Text.Trim();
                if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                {
                    Price = textItemPrice.Text.Replace(CURRENCYSYMBOLS, "");
                }
                else
                {
                    Price = textItemPrice.Text;
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
                if (textCost.Text == string.Empty)
                {
                    //ถ้า User ไม่กำหนด ต้นทุน ระบบจะนำเอาราคาขายที่ตั้งมาเป็นต้นทุน
                    addItem.EstimateCost = ConvertToDecimal(textItemPrice.Text.Trim());
                }
                else
                {
                    var Cost = textCost.Text.Trim();
                    if (!string.IsNullOrEmpty(CURRENCYSYMBOLS))
                    {
                        Cost = textCost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        Cost = textCost.Text;
                    }
                    addItem.EstimateCost = ConvertToDecimal(Cost);
                }
                if (string.IsNullOrEmpty(txtViewItemnameTitle.Text))
                {
                    if (textItemName.Text.Length > 6)
                    {
                        addItem.ShortName = textItemName.Text.Substring(0, 6);
                    }
                    else
                    {
                        addItem.ShortName = textItemName.Text;
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
                
                if (checkManageStock)
                {
                    if (addItem.SysItemID != 0)
                    {
                        if (string.IsNullOrEmpty(txtStock.Text))
                        {
                            Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                            return false;
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.insertdataitem), ToastLength.Long).Show();
                        return false; 
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.repeatnameexsize), ToastLength.Short).Show();
                    return false;
                }

                var result = await ItemManage.InsertItem(addItem, itemOnBranch, itemExSizes);
                if (!result)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
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

                Item_Fragment_Main.fragment_main.ReloadItem(addItem);
                return true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Insert at add Item");
                Toast.MakeText(this.Activity, "insertitem : " + ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public List<ItemExSize> itemExSizes = new List<ItemExSize>();
        RecyclerView.ViewHolder viewHolder;
        ListViewItemExSizeHolder vh ;
        public async Task<bool> AddItemExSize()
        {
            try
            {
                itemExSizes = new List<ItemExSize>();
                for (int i = 0; i < rcvSize.ChildCount; i++)
                {
                    View child = rcvSize.GetChildAt(i);
                    viewHolder = rcvSize.GetChildViewHolder(child);
                    vh = viewHolder as ListViewItemExSizeHolder;
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
                Toast.MakeText(this.Activity, "AddItemExSize" + ex.Message, ToastLength.Short).Show();
                return false;
            }
        }
        private List<Item> items;
        async Task GetItemList()
        {
            try
            {
                items = new List<Item>();
                items = MainActivity.allData.DefaultDataItem;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("GetItemList at Item");
            }
        }
        List<String> itemID;
        public void SpinnerCategory()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                itemID = new List<string>();

                Category addcategory = new Category();
                var category = new List<Category>();
                var getallCategory = new List<Category>();

                addcategory = new Category()
                {
                    Name = "None",
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

                spnCategory.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spnCategory_ItemSelected);
                var adapterCategory = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
                adapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spnCategory.Adapter = adapterCategory;

                if (SyscategoryIDfromPOS != 0)
                {
                    int index = category.FindIndex(x => x.SysCategoryID == SyscategoryIDfromPOS);
                    if (index != -1)
                    {
                        spnCategory.SetSelection(index);
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
        private void spnCategory_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (itemID == null)
            {
                return;
            }

            SyscategoryID = Convert.ToInt32(itemID[e.Position].ToString());
            CheckDataChange();
        }
        
        public async void UpdateItemExsize()
        {
            //กรณีที่แก้ไขข้อมูล ExSize ตอนกดแก้ไขข้อมูล Item 
            //ลบ ItemSize เดิมออกแล้ว Insert ใหม่

            var getItemSize = await ItemExSizeManage.GetItemSize((int)DataCashing.EditItem.MerchantID, (int)DataCashing.EditItem.SysItemID);
            if (getItemSize.Count > 0)
            {
                bool checkResult = await ItemExSizeManage.DeleteItemsize((int)DataCashing.EditItem.MerchantID, (int)DataCashing.EditItem.SysItemID);
                if (!checkResult)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }
            }

            bool checkInsert = await ItemExSizeManage.InsertListItemsize(newlsItemExSize);
            if (!checkInsert)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();
            }
        }
        TextView txtTitle;
        FrameLayout lnBack;
        LinearLayout lnShowItem;
        RecyclerView rcvHeaderItem;
        ScrollView scvItem;
        ImageView colorViewItem;
        internal static ImageView imageViewItem;
        EditText txtViewItemnameTitle;
        TextView txtItemNamePic, txtPricePic;
        ImageButton btncolor1, btncolor2, btncolor3, btncolor4, btncolor5, btncolor6, btncolor7, btncolor8, btncolor9, btnaddpic;
        ImageView imgFavorite;
        FrameLayout lnFavorite;
        public static EditText textItemName;
        EditText textItemPrice;
        ImageButton btnShowDetail;
        LinearLayout lnDetails;
        public static EditText textItemCode;
        ImageView imgScanItem;
        FrameLayout lnScanItem;
        EditText textCost;
        Spinner spnCategory, spnVat;
        Button btnCategory, btnVat;
        public static Button btnAddSize;
        LinearLayout lnCategory, lnVat;
        public static LinearLayout lnAddSize;
        public static  RecyclerView rcvSize;
        public static Switch switchShowOption;
        Switch switchStock;
        LinearLayout lnShowStock, lnSwithStcok, lnOnhand, lnStockMoveMent;
        TextView txtStock, txtMinimumStock;
        FrameLayout btnDelete;
        internal Button btnAdditem;
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
            textItemPrice = view.FindViewById<EditText>(Resource.Id.textItemPrice);
            btnShowDetail = view.FindViewById<ImageButton>(Resource.Id.btnShowDetail);
            lnDetails = view.FindViewById<LinearLayout>(Resource.Id.lnDetails);
            textItemCode = view.FindViewById<EditText>(Resource.Id.textItemCode);
            imgScanItem = view.FindViewById<ImageView>(Resource.Id.imgScanItem);
            lnScanItem = view.FindViewById<FrameLayout>(Resource.Id.lnScanItem);
            textCost = view.FindViewById<EditText>(Resource.Id.textCost);
            spnCategory = view.FindViewById<Spinner>(Resource.Id.spnCategory);
            btnCategory = view.FindViewById<Button>(Resource.Id.btnCategory);
            lnCategory = view.FindViewById<LinearLayout>(Resource.Id.lnCategory);
            spnVat = view.FindViewById<Spinner>(Resource.Id.spnVat);
            btnVat = view.FindViewById<Button>(Resource.Id.btnVat);
            lnVat = view.FindViewById<LinearLayout>(Resource.Id.lnVat);
            btnAddSize = view.FindViewById<Button>(Resource.Id.btnAddSize);
            rcvSize = view.FindViewById<RecyclerView>(Resource.Id.rcvSize);
            lnAddSize = view.FindViewById<LinearLayout>(Resource.Id.lnAddsize);
            switchShowOption = view.FindViewById<Switch>(Resource.Id.switchShowOption);
            lnShowStock = view.FindViewById<LinearLayout>(Resource.Id.lnShowStock);
            switchStock = view.FindViewById<Switch>(Resource.Id.switchStock);
            lnSwithStcok = view.FindViewById<LinearLayout>(Resource.Id.lnSwithStcok);
            lnOnhand = view.FindViewById<LinearLayout>(Resource.Id.lnOnhand);
            txtStock = view.FindViewById<TextView>(Resource.Id.txtStock);
            txtMinimumStock = view.FindViewById<TextView>(Resource.Id.txtMinimumStock);
            lnStockMoveMent = view.FindViewById<LinearLayout>(Resource.Id.lnStockMoveMent);
            btnDelete = view.FindViewById<FrameLayout>(Resource.Id.btnDelete);
            btnAdditem = view.FindViewById<Button>(Resource.Id.btnAdditem);
        }
        private void SetUIEvent()
        {
            if (favoritefromPOS)
            {
                favorite = true;
            }
            else
            {
                favorite = false;
            }
            lnBack.Click += LnBack_Click;
            lnDetails.Visibility = ViewStates.Visible;
            textItemCode.TextChanged += TextItemCode_TextChanged; 
            imageViewItem.Click += ImageViewItem_Click;
            txtPricePic.Hint = Utils.DisplayDecimal(0);
            textCost.TextChanged += TextCost_TextChanged; 
            lnAddSize.Click += LnAddSize_Click;
            btnAddSize.Click += LnAddSize_Click;
            btnShowDetail.Click += BtnShowDetail_Click; 
            lnScanItem.Click += LnScanItem_Click;
            lnFavorite.Click += LnFavorite_Click; 
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
            textItemName.TextChanged += TextItemName_TextChanged;
            textItemPrice.TextChanged += TextItemPrice_TextChanged;
            textItemPrice.KeyPress += TextItemPrice_KeyPress;
            textCost.Hint = Utils.DisplayDecimal(0);
            textCost.KeyPress += TextCost_KeyPress;
            switchShowOption.CheckedChange += SwitchShowOption_CheckedChange;
            txtStock.TextChanged += TxtStock_TextChanged;
            lnOnhand.Click += LnOnhand_Click;
            txtStock.Click += LnOnhand_Click;
            switchStock.CheckedChange += SwitchStock_CheckedChange;
            lnStockMoveMent.Click += LnStockMoveMent_Click;
            btnAdditem.Click += BtnAdditem_Click;
            btnCategory.Click += BtnCategory_Click;
            btnVat.Click += BtnVat_Click;
            txtMinimumStock.FocusChange += TxtMinimumStock_FocusChange;
            txtMinimumStock.TextChanged += TxtMinimumStock_TextChanged;
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
                //txtMinimumStock.SetSelection(txtMinimumStock.Text.Length);
                CheckDataChange();
                return;
            }
            //EditStock = true;
            CheckDataChange();
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

        private void BtnVat_Click(object sender, EventArgs e)
        {
            spnVat.PerformClick();
        }

        private void BtnCategory_Click(object sender, EventArgs e)
        {
            spnCategory.PerformClick();
        }

        private void LnOnhand_Click(object sender, EventArgs e)
        {
            try
            {
                var fragment = new AddItem_Dialog_OnHand();
                AddItem_Dialog_OnHand.SetPage("additem");
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

        private void LnStockMoveMent_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataCashing.EditItem == null)
                {
                    return;
                }

                var fragment = new AddItem_Dialog_MoveMent();
                AddItem_Dialog_MoveMent.SetItem(DataCashing.EditItem);
                fragment.Show(this.Activity.SupportFragmentManager, nameof(AddItem_Dialog_MoveMent));
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

        private void TxtStock_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void SwitchShowOption_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (switchShowOption.Checked)
            {
                showDisplay = 1;
            }
            else
            {
                showDisplay = 0;
            }

            CheckDataChange();
        }

        private void TextCost_KeyPress(object sender, View.KeyEventArgs e)
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
                        txtCost = textCost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        txtCost = textCost.Text;
                    }

                    if (txtCost.Trim().Length == 0)
                    {
                        return;
                    }
                    var Price = Convert.ToDecimal(txtCost);
                    //textCurrency.Visibility = ViewStates.Visible;
                    textCost.Text = CURRENCYSYMBOLS + " " + Utils.DisplayDecimal(Price);
                    textCost.SetSelection(textCost.Text.Length);
                }
                else
                {
                    e.Handled = false; //if you want that character appeared on the screen
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, "Txtcost_KeyPress" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Txtcost_KeyPress at Add Item");
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
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, "TextInsertPrice_KeyPress" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_KeyPress at Add Item");
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
                Toast.MakeText(this.Activity, "TextInsertPrice_TextChanged" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TextInsertPrice_TextChanged at Add Item");
            }

        }

        private void TextItemName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var itemname = textItemName?.Text.Trim();
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

        private void Btnaddpic_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.flagChooseMedia = true;
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

        private void LnVat_Click(object sender, EventArgs e)
        {
            try
            {
                spnVat.PerformClick();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnVat_Click at add Item");
                Toast.MakeText(this.Activity, "LnVat_Click" + ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnCategory_Click(object sender, EventArgs e)
        {
            try
            {
                spnCategory.PerformClick();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnCategory_Click at add Item");
                Toast.MakeText(this.Activity, "LnCategory_Click" + ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnDelete.Enabled = false;
                string Role = LoginType;
                bool check = UtilsAll.CheckPermissionRoleUser(Role, "delete", "item");
                if (!check)
                {
                    btnDelete.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;                    
                }

                MainDialog dialog = new MainDialog() { Cancelable = false };
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.additem_dialog_delete.ToString();
                bundle.PutString("message", myMessage);
                dialog.Arguments = bundle;
                dialog.Show(Activity.SupportFragmentManager, myMessage);
                btnDelete.Enabled = true;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at add Item");
                Toast.MakeText(this.Activity, "BtnDelete_Click" + ex.Message, ToastLength.Short).Show();
                return;
            }

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

        private void LnScanItem_Click(object sender, EventArgs e)
        {
            try
            {
                bool check = MainActivity.main_activity.CheckPermission();
                if (check)
                {
                    Item_Dialog_Scan dialog = Item_Dialog_Scan.NewInstance("additem");
                    var fragment = new Item_Dialog_Scan() { Cancelable = false };
                    fragment.Show(Activity.SupportFragmentManager, nameof(Item_Dialog_Scan));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnScanItem_Click at Item Dialog AddItem");
            }
        }

        public static void SetItemCode(string itemCode)
        {
            textItemCode.Text = itemCode;
        }

        private void BtnShowDetail_Click(object sender, EventArgs e)
        {
            if (showdetail)
            {
                EditItemExSize();
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

        private void TextCost_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (textCost.Text.Length == 0)
                {
                    return;
                }

                if (textCost.Text == ".")
                {
                    return;
                }

                string[] txt = new string[] { };
                int checkIndex = 0;
                if (textCost.Text.Contains('.'))
                {
                    checkIndex = textCost.Text.IndexOf('.');
                    if (checkIndex == -1)
                    {
                        return;
                    }

                    txt = textCost.Text.Split('.');
                    if (!string.IsNullOrEmpty(txt[1]))
                    {
                        if (txt[1].Length > Convert.ToInt32(DecimalDisplay))
                        {
                            string Amount = textCost.Text;
                            textCost.Text = Amount.Remove(Amount.Length - 1);
                            textCost.SetSelection(textCost.Text.Length);
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(txt[0]))
                    {
                        if ((txt[0].Length) > 13)
                        {
                            textCost.Text = txt[0].Remove(13, 1);
                            textCost.SetSelection(textCost.Text.Length);
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
                        cost = textCost.Text.Replace(CURRENCYSYMBOLS, "");
                    }
                    else
                    {
                        cost = textCost.Text;
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
                Toast.MakeText(this.Activity, "Txtcost_TextChanged" + ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Txtcost_TextChanged at Add Item");
            }

        }

        private void ImageViewItem_Click(object sender, EventArgs e)
        {
            try
            {
                string path = "";
                if (itemEdit != null)
                {
                    //MainDialog dialog = new MainDialog();
                   
                    if (!string.IsNullOrEmpty(itemEdit.PicturePath))
                    {
                        path = itemEdit.PicturePath;
                    }
                    else
                    {
                        path = itemEdit.PictureLocalPath;
                    }
                    //dialog.Arguments = bundle;
                    //dialog.Show(SupportFragmentManager, myMessage);

                    Item_Dialog_ShowImage dialog_Item = Item_Dialog_ShowImage.NewInstance();
                    Item_Dialog_ShowImage.SetPath(path);
                    dialog_Item.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Item_Dialog_ShowImage));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgProfile_Click at add add Item");
                Toast.MakeText(this.Activity, "ImageViewItem_Click" + ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        string itemcode = "";

        private void TextItemCode_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(textItemCode.Text))
            {
                CheckDataChange();
                return;
            }

            if (Regex.IsMatch(textItemCode.Text, @"^[\u0E00-\u0E7Fa-zA-Z0-9']+$"))
            {
                itemcode = textItemCode.Text;
            }
            else
            {
                textItemCode.Text = itemcode;
                Toast.MakeText(this.Activity, "สามารถกรอกได้เฉพาะตัวภาษาอังกฤษหรือภาษาไทยหรือตัวเลขได้", ToastLength.Short).Show();
                return;
            }
            textItemCode.SetSelection(textItemCode.Text.Length);
            CheckDataChange();
        }
        private void SwitchStock_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            //check switch stock ว่า true ไหม ถ้า true จะไม่สามารถใช้งานปุ่มได้            
            // var Nosizename = newlsItemExSize.Where(x=> string.IsNullOrEmpty(x.ExSizeName)).ToList();
            //if ((recyclerViewSize.ChildCount > 0) && (Nosizename.Count > 0))
            if ((rcvSize.ChildCount > 0) && (newlsItemExSize.Count > 0))
            {
                Toast.MakeText(this.Activity, "สินค้ามีขนาดไม่สามารถเพิ่มสต๊อกได้", ToastLength.Short).Show();
                lnSwithStcok.Visibility = ViewStates.Gone;
                switchStock.Checked = false;
                return;
            }

            if (!DataCashing.CheckNet)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                switchStock.Enabled = false;
            }

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
            CheckDataChange();
        }

        private async void LnAddSize_Click(object sender, EventArgs e)
        {
            try
            {
                //check switch stock ว่า true ไหม ถ้า true จะไม่สามารถใช้งานปุ่มได้
                if (switchStock.Checked)
                {
                    Toast.MakeText(this.Activity, "สินค้ามีสต๊อกไม่สามารถเพิ่มขนาดได้", ToastLength.Short).Show();
                    return;
                }

                //Get ค่าจาก textbox มาเก็บลง  List
                if (showdetail)
                {
                    EditItemExSize();
                }

                //insert ItemExSize หลังจากกดเพิ่ม
                if (newlsItemExSize.Count < 5)
                {
                    var i = newlsItemExSize.Count + 1;
                    newlsItemExSize.Add(new ItemExSize { MerchantID = DataCashingAll.MerchantId, SysItemID = SysItemId, ExSizeNo = i, ExSizeName = "", Price = 0, EstimateCost = 0 });
                }

                ListItemExSize lstItemExSize = new ListItemExSize(newlsItemExSize);
                AddItem_Adapter_Size addItem_adapter_size = new AddItem_Adapter_Size(lstItemExSize);
                LinearLayoutManager layoutManager = new LinearLayoutManager(this.Activity);
                rcvSize.SetLayoutManager(layoutManager);
                rcvSize.LayoutFrozen = true;
                rcvSize.SetAdapter(addItem_adapter_size);
                var layout = addItem_adapter_size.ItemCount;
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                //lnExtrasize.LayoutParameters.Height = Convert.ToInt32(190* layout);
                //this.OnResume();


                rcvSize.ScrollToPosition(layout - 1);
                rcvSize.FindFocus();

                CheckDataChange();

                if (addItem_adapter_size.ItemCount > 0)
                {
                    btnAddSize.Visibility = ViewStates.Gone;
                    lnAddSize.Visibility = ViewStates.Visible;
                    switchStock.Enabled = false;
                }
                else
                {
                    btnAddSize.Visibility = ViewStates.Visible;
                    lnAddSize.Visibility = ViewStates.Gone;
                    switchStock.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("AddExSize at add Item");
            }
        }

        public void SetClearData()
        {
            UINewItem();
            DataCashing.flagChooseMedia = false;
            DataCashing.EditItem = null;
            itemEdit = null;
            flagdatachange = false;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnItem,"item","default");
        }

        public void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flagdatachange)
                {
                    SetClearData(); return;
                }

                if (DataCashing.EditItem == null)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Add_Dialog_Back.SetPage("item");
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
                    Edit_Dialog_Back.SetPage("item");
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
                SetImgItem();
                return;
            }

            first = true;
            UINewItem();
            SetItemView();
            SpinnerCategory();
            await SetDetailItem();
            SetUIFromMainRole(LoginType);     

            first = false;
            flagdatachange = false;
            SetButtonAdd(false);
        }

        private void SetImgItem()
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

        public static bool useItemcode = false;
        private bool CheckDuplicateData(string itemname,string itemcode)
        {
            try
            {
                if (DataCashing.EditItem == null)
                {
                    #region Check ชื่อสินค้า และ itemCode                
                    var checkNameandItemCode = items.FindIndex(x => x.ItemName.ToLower().Equals(itemname.ToLower()) && !string.IsNullOrEmpty(itemcode) && x.ItemCode.ToLower().Equals(itemcode.ToLower()));
                    if (checkNameandItemCode != -1)
                    {
                        return false;
                    }

                    //Check ชื่อสินค้า 
                    var checkname = items.FindIndex(x => x.ItemName.ToLower().Equals(itemname.ToLower()));
                    if (checkname != -1)
                    {
                        return false;
                    }

                    // itemCode
                    var checkItemCode = items.FindIndex(x => !string.IsNullOrEmpty(itemcode) && x.ItemCode.ToLower().Equals(itemcode.ToLower()));
                    if (checkItemCode != -1)
                    {
                        useItemcode = true;
                        return false;
                    }
                    #endregion
                }
                else
                {
                    // Check ชื่อสินค้า และ itemCode 
                    #region Check ชื่อสินค้า และ itemCode
                    if ((itemname.Trim() != DataCashing.EditItem.ItemName) && (itemcode.Trim() != DataCashing.EditItem.ItemCode))
                    {
                        var checkNameandItemCode = items.FindIndex(x => x.ItemName.ToLower().Equals(DataCashing.EditItem.ItemName.ToLower()) && !string.IsNullOrEmpty(DataCashing.EditItem.ItemCode) && x.ItemCode.ToLower().Equals(DataCashing.EditItem.ItemCode.ToLower()));
                        if (checkNameandItemCode != -1)
                        {
                            return false;
                        }
                    }


                    //Check ชื่อสินค้า                 
                    if (itemname.Trim() != DataCashing.EditItem.ItemName && itemname.Trim() != string.Empty)
                    {
                        var checkname = items.FindIndex(x => x.ItemName.ToLower().Equals(DataCashing.EditItem.ItemName.ToLower()));
                        if (checkname != -1)
                        {
                            return false;
                        }
                    }

                    // itemCode
                    if (itemcode.Trim() != DataCashing.EditItem.ItemCode)
                    {
                        var checkItemCode = items.FindIndex(x => !string.IsNullOrEmpty(DataCashing.EditItem.ItemCode.ToLower()) && x.ItemCode.ToLower().Equals(DataCashing.EditItem.ItemCode.ToLower()));
                        if (checkItemCode != -1)
                        {
                            return false;
                        }
                    }
                    #endregion
                }
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
                return false;
            }            
        }

        public static void SetCategoryFromPOS(long SyscategoryID)
        {
            SyscategoryIDfromPOS = SyscategoryID;
        }
    }

    public class ListViewItemExSizeHolder : RecyclerView.ViewHolder
    {
        public TextView ExSizeNo { get; set; }
        public EditText ExSizeName { get; set; }
        public EditText Price { get; set; }
        public EditText EstimateCost { get; set; }
        public EditText Comments { get; set; }
        public ImageButton btnDelete { get; set; }

        public ImageButton SelectCustomer { get; set; }
        public ListViewItemExSizeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ExSizeName = itemview.FindViewById<EditText>(Resource.Id.txtExSizeName);
            Price = itemview.FindViewById<EditText>(Resource.Id.txtPrice);
            EstimateCost = itemview.FindViewById<EditText>(Resource.Id.txtEstimate);
            btnDelete = itemview.FindViewById<ImageButton>(Resource.Id.btnDelete);

            itemview.Click += (sender, e) => listener(base.Position);
        }
    }
    public class ListItemExSize
    {
        public List<ItemExSize> itemexsizes;
        static List<ItemExSize> builitem;
        public ListItemExSize(List<ItemExSize> lsitemExSizes)
        {
            builitem = lsitemExSizes;
            this.itemexsizes = builitem;
        }

        public int Count
        {
            get
            {
                return itemexsizes == null ? 0 : itemexsizes.Count;
            }
        }
        public ORM.MerchantDB.ItemExSize this[int i]
        {
            get { return itemexsizes == null ? null : itemexsizes[i]; }
        }
    }

}