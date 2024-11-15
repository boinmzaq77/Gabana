using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Items;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.Model;
using Gabana.ORM.Master;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using Category = Gabana.ORM.MerchantDB.Category;
using Android;
using Android.Webkit;

namespace Gabana.Droid.Tablet.Dialog
{
    public class POS_Dialog_AddItem : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static POS_Dialog_AddItem NewInstance()
        {
            var frag = new POS_Dialog_AddItem { Arguments = new Bundle() };
            return frag;
        }
        public static POS_Dialog_AddItem dialog_additem;
        View view;
        public static bool first = true, flagdatachange = false;
        public static bool favorite, favoritefromPOS;
        ORM.MerchantDB.ItemOnBranch itemOnBranch, DataStock, getBalance;
        static bool EditStock = false;
        long SyscategoryID;
        decimal showDisplay, showStock;
        char TaxType;
        ItemManage ItemManage = new ItemManage();
        ItemExSizeManage ItemExSizeManage = new ItemExSizeManage();
        List<ORM.MerchantDB.ItemExSize> lstExSize = new List<ORM.MerchantDB.ItemExSize>();
        List<ORM.MerchantDB.ItemExSize> newlsItemExSize = new List<ORM.MerchantDB.ItemExSize>();
        List<ORM.MerchantDB.ItemExSize> TemplsItemExSize = new List<ORM.MerchantDB.ItemExSize>();
        string usernamelogin, LoginType;
        string colorSelected;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_additem, container, false);
            try
            {
                dialog_additem = this;
                usernamelogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                CombinUI();
                SetUIEvent();
                SetUIFromMainRole(LoginType);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

            _ = TinyInsights.TrackPageViewAsync("OnCreateView : POS_Dialog_AddItem");
            return view;
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
            txtPricePic.Hint = Utils.DisplayDecimal(0);
            textCost.TextChanged += TextCost_TextChanged;
            lnAddSize.Click += LnAddSize_Click;
            btnAddSize.Click += LnAddSize_Click;
            btnShowDetail.Click += BtnShowDetail_Click;
            lnScanItem.Click += LnScanItem_Click;
            lnFavorite.Click += LnFavorite_Click;
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

        private void LnBack_Click(object sender, EventArgs e)
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
                    Add_Dialog_Back.SetPage("POS_item");
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
                    Edit_Dialog_Back.SetPage("POS_item");
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

        ImageButton btncolor1, btncolor2, btncolor3, btncolor4, btncolor5, btncolor6, btncolor7, btncolor8, btncolor9, btnaddpic;
        TextView txtTitle;
        FrameLayout lnBack;
        RecyclerView rcvHeaderItem;
        LinearLayout lnShowItem;
        ScrollView scvItem;
        ImageView colorViewItem;
        internal static ImageView imageViewItem;
        EditText txtViewItemnameTitle;
        TextView txtItemNamePic, txtPricePic;
        ImageView imgFavorite;
        FrameLayout lnFavorite;
        EditText textItemPrice;
        public static EditText textItemName;
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
        public static RecyclerView rcvSize;
        public static Switch switchShowOption;
        Switch switchStock;
        LinearLayout lnShowStock, lnSwithStcok, lnOnhand, lnStockMoveMent;
        TextView txtStock, txtMinimumStock;
        internal Button btnAdditem;
        private void CombinUI()
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
            btnAdditem = view.FindViewById<Button>(Resource.Id.btnAdditem);
        }

        static string stockOnhabd;
        internal static void SetOnhand(string text)
        {
            dialog_additem.txtStock.Text = text;
            stockOnhabd = text;

            if (checkManageStock)
            {
                //open stock
                dialog_additem.lnSwithStcok.Visibility = ViewStates.Visible;
                if (!string.IsNullOrEmpty(stockOnhabd))
                {
                    dialog_additem.txtStock.Text = Convert.ToInt32(Utils.CheckLenghtValue(stockOnhabd)).ToString("#,###");
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

        public void CheckDataChange()
        {
            try
            {
                if (first)
                {
                    SetButtonAdd(false);
                    return;
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
            }           

            await GetItemList();
            SetTabMenu();
            SetTabShowMenu();
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
                AddItem_Adapter_Header additem_adapter_header = new AddItem_Adapter_Header(menuTab, "POS_item");
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
                        if (checkManageStock)
                        {
                            lnSwithStcok.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            lnSwithStcok.Visibility = ViewStates.Gone;
                        }
                        break;
                    default:
                        lnShowItem.Visibility = ViewStates.Gone;
                        lnShowStock.Visibility = ViewStates.Gone;
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

        public bool EditItemExSize()
        {
            try
            {
                itemExSizes = new List<ORM.MerchantDB.ItemExSize>();
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
                Toast.MakeText(this.Activity, "EditItemExSize" + ex.Message, ToastLength.Short).Show();
                return false;
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
                newlsItemExSize = new List<ORM.MerchantDB.ItemExSize>();
                lstExSize = new List<ORM.MerchantDB.ItemExSize>();
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
        private void BtnAdditem_Click(object sender, EventArgs e)
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
                    Item_Dialog_Dublicate.SetPage("POS_item");
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
                ORM.MerchantDB.DeviceSystemSeqNo deviceSystemSeq = new ORM.MerchantDB.DeviceSystemSeqNo();
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
                        itemOnBranch = new ORM.MerchantDB.ItemOnBranch()
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
                Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                return false;
            }
        }

        public List<ORM.MerchantDB.ItemExSize> itemExSizes = new List<ORM.MerchantDB.ItemExSize>();
        RecyclerView.ViewHolder viewHolder;
        ListViewItemExSizeHolder vh;
        public async Task<bool> AddItemExSize()
        {
            try
            {
                itemExSizes = new List<ORM.MerchantDB.ItemExSize>();
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
        private List<ORM.MerchantDB.Item> items;
        async Task GetItemList()
        {
            try
            {
                items = new List<ORM.MerchantDB.Item>();
                items = MainActivity.allData.DefaultDataItem;
            }
            catch (Exception ex)
            {
                Log.Debug("stateStep", "AddItem" + "GetItemList");
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
                AddItem_Dialog_OnHand.SetPage("POS_item");
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
                //StartActivity(new Android.Content.Intent(Application.Context, typeof(ItemCodeScanActivity)));



            }
            catch (Exception)
            {

            }
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
            SetUIFromMainRole(LoginType);
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

        private void LnAddSize_Click(object sender, EventArgs e)
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
                    newlsItemExSize.Add(new ORM.MerchantDB.ItemExSize { MerchantID = DataCashingAll.MerchantId, SysItemID = SysItemId, ExSizeNo = i, ExSizeName = "", Price = 0, EstimateCost = 0 });
                }
                AddItem_Adapter_Size.SetPage("POS_item");
                ListItemExSize lstItemExSize = new ListItemExSize(newlsItemExSize);
                AddItem_Adapter_Size addItem_adapter_size = new AddItem_Adapter_Size(lstItemExSize);
                LinearLayoutManager layoutManager = new LinearLayoutManager(this.Activity);
                rcvSize.SetLayoutManager(layoutManager);
                rcvSize.LayoutFrozen = true;
                rcvSize.SetAdapter(addItem_adapter_size);
                var layout = addItem_adapter_size.ItemCount;
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

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
            flagdatachange = false;
            dialog_additem = null;
            POS_Fragment_Main.pos_dialog_additem = null;
            this.Dialog.Dismiss();            
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
            //SetUIFromMainRole(LoginType);

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
        private bool CheckDuplicateData(string itemname, string itemcode)
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public static void SetCategoryFromPOS(long SyscategoryID)
        {
            SyscategoryIDfromPOS = SyscategoryID;
        }

        public void DeleteExSize(ORM.MerchantDB.ItemExSize PositionDelete)
        {
            try
            {
                dialog_additem.EditItemExSize();

                var item = dialog_additem.newlsItemExSize.Find(x => x.ExSizeNo == PositionDelete.ExSizeNo);
                dialog_additem.newlsItemExSize.Remove(item);

                ListItemExSize lstItemExSize = new ListItemExSize(dialog_additem.newlsItemExSize);
                AddItem_Adapter_Size addItem_adapter_size = new AddItem_Adapter_Size(lstItemExSize);
                GridLayoutManager gridLayoutManager = new GridLayoutManager(MainActivity.main_activity, 1, 1, false);
                rcvSize.HasFixedSize = true;
                rcvSize.SetLayoutManager(gridLayoutManager);
                rcvSize.SetAdapter(addItem_adapter_size);
                var layout = addItem_adapter_size.ItemCount;
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                //lnExtrasize.LayoutParameters.Height = Convert.ToInt32(190* layout);
                //fragment_additem.OnResume();
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
    }
}