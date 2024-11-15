using Android.App;
using Android.OS;
using Android.Support.Design.Chip;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class OptionActivity : AppCompatActivity
    {
        public static OptionActivity option;
        public static long? sysitemId;
        LinearLayout lnBack, lnSize, lnNote, lnExtra;
        RecyclerView recyclerViewListsize, recyclerViewCategoryNote, recyclerViewNote, recyclerViewExtraCategory, recyclerViewExtra;
        Button btnAddtoCart;
        EditText textInsertNote;
        public static List<NoteCategory> lstCategoryNote = new List<NoteCategory>();
        ListNoteCategory listCategoryNote;
        public static string nameCategoryNote = "";
        public static long sysCategoryNote = 0;
        static List<Note> lstdatanote;
        ListNoteData lstShowNote;
        public static TranWithDetailsLocal tranWithDetails;
        List<ItemExSize> lstExSize = new List<ItemExSize>();
        ItemExSizeManage ItemExSizeManage;
        TranDetailItemWithTopping detailItemWithTopping;
        public static List<Item> itemExtra;
        private static ListItem listExtraItem;
        public List<Category> itemExtraCategory = new List<Category>();
        private ListCategory listExtraCategory;
        RecyclerView.LayoutManager mLayoutManager;
        Option_Adapter_CategoryExtra option_adapter_categoryextra;
        GridLayoutManager gridLayoutManagerSize;
        GridLayoutManager gridLayoutItem;
        LinearLayoutManager LayoutManager;
        Option_Adapter_Note option_Adapter_Note;
        Option_Adapter_Size option_adapter_size;
        Option_Adapter_NoteCategory option_Adapter_NoteCategory;
        public static Item POSDataItem;
        public static TranDetailItemNew ItemEditTopping;
        public static string nameCategory = "";
        public static long sysCategory = 0;
        int fillter;
        public static long ItemShopOption = 0;
        public static int PositionClick = 0;

        //Size
        public static ItemExSize SelectSize;
        ListItemExSize lstItemExSize;
        public static long ExSizeNo = 0;

        //Extra Topping
        public static List<TranDetailItemTopping> lstTranSelectTopping;
        public static long sysitemIDToppping = 0;
        Option_Adapter_Extra option_Adapter_Extra;

        //Note
        public static string nameNote = "";
        public static long NoteID = 0;
        public static List<Note> SelectNote = new List<Note>();        
        public static ItemExSize defaultSize;
        TranDetailItemWithTopping tranDetailItem = new TranDetailItemWithTopping();
        public static List<TranDetailItemTopping> lstTempTopping;
        public static Item CartDetaItem; // = new Item();
        public static bool flagLoadSize = false;

        ChipGroup chipGroup;
        public static bool IsActive = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.option_activity);
                
                option = this;
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnSize = FindViewById<LinearLayout>(Resource.Id.lnSize);
                lnNote = FindViewById<LinearLayout>(Resource.Id.lnNote);
                lnExtra = FindViewById<LinearLayout>(Resource.Id.lnExtra);
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                textInsertNote = FindViewById<EditText>(Resource.Id.textComment);
                recyclerViewListsize = FindViewById<RecyclerView>(Resource.Id.recyclerViewListsize);
                recyclerViewCategoryNote = FindViewById<RecyclerView>(Resource.Id.recyclerViewNoteCategory);
                recyclerViewNote = FindViewById<RecyclerView>(Resource.Id.recyclerViewNote);
                recyclerViewExtraCategory = FindViewById<RecyclerView>(Resource.Id.recyclerViewExtraCategory);
                recyclerViewExtra = FindViewById<RecyclerView>(Resource.Id.recyclerViewExtra);
                btnAddtoCart = FindViewById<Button>(Resource.Id.btnAddtoCart);
                chipGroup = FindViewById<ChipGroup>(Resource.Id.chipGroup);

                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;
                btnAddtoCart.Click += BtnAddtoCart_Click;
                textInsertNote.Text = string.Empty;

                //showData();
                lstTranSelectTopping = new List<TranDetailItemTopping>();
                lstTempTopping = new List<TranDetailItemTopping>();
                SelectSize = new ItemExSize();
                //noteCategories 
                listCategoryNote = new ListNoteCategory(lstCategoryNote);

                _ = TinyInsights.TrackPageViewAsync("OnCreate : OptionActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Option");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void CreateChip(List<Note> notes)
        {
            chipGroup.RemoveAllViews();

            foreach (var item in notes)
            {
                var inflater = LayoutInflater.From(this);
                var chip = inflater.Inflate(Resource.Layout.option_chip_note, null, false) as Chip;
                chip.Text = item.Message;

                chipGroup.AddView(chip);

                chip.Click += (s, e) =>
                {
                    if (string.IsNullOrEmpty(textInsertNote.Text))
                    {
                        textInsertNote.Text = chip.Text;
                    }
                    else
                    {
                        textInsertNote.Text += " " + chip.Text;
                    }
                };
            }
        }

        public async Task showData()
        {

            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                if (tranWithDetails.tran != null)
                {
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    await GetSizeItem();
                    SetSizeItem();

                    //Category Note
                    //GetCategoryNote(); //ย้ายมาเรียกหน้าหน้า pos
                    ShowCategoryNote();

                    //ExtraTopping Category
                    await SetExtraCategory();
                    SetExtra();

                    DataCashingAll.flagItemChange = false;
                    DataCashingAll.flagCategoryChange = false;


                    if (DataCashing.flagEditOptionNote)
                    {
                        if (CartActivity.CurrentActivity)
                        {
                            var row = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == sysitemId & x.tranDetailItem.DetailNo == CartActivity.DetailNo);
                            if (row != -1)
                            {
                                textInsertNote.Text = tranWithDetails.tranDetailItemWithToppings[row].tranDetailItem.Comments;
                                DataCashing.flagEditOptionNote = false;
                            }
                        }
                        else
                        {
                            var row = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == sysitemId & x.tranDetailItem.DetailNo == CartScanActivity.detailNoClickOption);
                            if (row != -1)
                            {
                                textInsertNote.Text = tranWithDetails.tranDetailItemWithToppings[row].tranDetailItem.Comments;
                                DataCashing.flagEditOptionNote = false;
                            }
                        }
                    }
                }

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
                _ = TinyInsights.TrackPageViewAsync("showData at Option");
            }
        }

        private async void BtnAddtoCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (tranWithDetails.tran != null)
                {
                    if (POSDataItem != null)  //POSDataItem != null => เพิ่ม option ที่มาจากหน้า POS
                    {
                        InsertTotran();     
                    }
                    else //POSDataItem == null => เพิ่ม option ที่มาจากหน้าตะกร้า  
                    {                        
                        UpdateTotran();
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("BtnAddtoCart_Click at Option");
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        void InsertTotran()
        {
            try
            {
                //Item
                //Size
                //SelectSize
                string size = string.Empty;
                decimal itemPrice = 0;
                TranDetailItemNew DetailItem = new TranDetailItemNew();
                DetailItem = new TranDetailItemNew();

                if (SelectSize == null)
                {
                    size = defaultSize.ExSizeName;
                    itemPrice = defaultSize.Price;
                    DetailItem.ItemPrice = POSDataItem.Price;
                    DetailItem.Price = POSDataItem.Price;
                }
                else if (SelectSize.ExSizeNo == 0)
                {
                    size = defaultSize.ExSizeName;
                    itemPrice = defaultSize.Price;
                    DetailItem.ItemPrice = POSDataItem.Price;
                    DetailItem.Price = POSDataItem.Price;
                }
                else
                {
                    size = SelectSize.ExSizeName;
                    itemPrice = SelectSize.Price;
                    DetailItem.ItemPrice = itemPrice;
                    DetailItem.Price = itemPrice;
                }

                DetailItem.SysItemID = POSDataItem.SysItemID;
                DetailItem.MerchantID = DataCashingAll.MerchantId;
                DetailItem.SysBranchID = DataCashingAll.SysBranchId;
                DetailItem.TranNo = tranWithDetails.tran.TranNo;
                DetailItem.ItemName = POSDataItem.ItemName;
                DetailItem.SaleItemType = POSDataItem.SaleItemType;
                DetailItem.FProcess = 1;
                DetailItem.TaxType = POSDataItem.TaxType;
                DetailItem.Quantity = (decimal)DataCashing.setQuantityToCart;
                DetailItem.Discount = 0;
                DetailItem.EstimateCost = POSDataItem.EstimateCost;
                DetailItem.SizeName = size;
                DetailItem.Comments = textInsertNote.Text;
                DetailItem.DetailNo = 0;


                List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                //Topping
                //lstSelectTopping
                for (int i = 0; i < lstTranSelectTopping.Count; i++)
                {
                    TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                    {
                        MerchantID = tranWithDetails.tran.MerchantID,
                        SysBranchID = tranWithDetails.tran.SysBranchID,
                        TranNo = tranWithDetails.tran.TranNo,
                        DetailNo = 0,
                        ToppingNo = lstTranSelectTopping[i].ToppingNo,
                        ItemName = lstTranSelectTopping[i].ItemName,//toppping
                        SysItemID = lstTranSelectTopping[i].SysItemID,
                        UnitName = lstTranSelectTopping[i].UnitName,
                        RegularSizeName = null,
                        Quantity = lstTranSelectTopping[i].Quantity,
                        ToppingPrice = lstTranSelectTopping[i].ToppingPrice,
                        EstimateCost = lstTranSelectTopping[i].EstimateCost,
                        Comments = lstTranSelectTopping[i].Comments
                    };

                    lstitemDetail.Add(itemDetail);
                }

                detailItemWithTopping = new TranDetailItemWithTopping()
                {
                    tranDetailItem = DetailItem,
                    tranDetailItemToppings = lstitemDetail
                };

                PosActivity.tranDetailItemWithTopping = detailItemWithTopping;
                lstTranSelectTopping = new List<TranDetailItemTopping>();
                SelectSize = new ItemExSize();
                SelectNote = new List<Note>();
                ExSizeNo = 0;
                sysitemIDToppping = 0;
                NoteID = 0;
                PosActivity.SetTranDetail(tranWithDetails);
                PosActivity.pos.SelectItemtoCart(PositionClick);
                POSDataItem = null;
                ItemShopOption = 0;
                CartDetaItem = null;
                OptionActivity.flagLoadSize = false;
                IsActive = false;
                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("InsertTotran at Option");
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        void UpdateTotran()
        {
            try
            {
                if (tranWithDetails.tran == null)
                {
                    return;
                }

                //Item
                //Size
                //SelectSize
                string size = string.Empty;
                decimal itemPrice = 0;
                List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                TranDetailItemNew DetailItem = new TranDetailItemNew();

                long detailNo = 0;
                if (CartActivity.CurrentActivity)
                {
                    tranDetailItem = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == CartActivity.DetailNo).FirstOrDefault();
                }
                else
                {
                    tranDetailItem = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == CartScanActivity.detailNoClickOption).FirstOrDefault();
                }

                if (tranDetailItem != null)
                {
                    detailNo = tranDetailItem.tranDetailItem.DetailNo;
                    DetailItem = tranDetailItem.tranDetailItem;

                    //Note เดิม
                    DetailItem.Comments = textInsertNote.Text;
                }

                int current = 0;
                if (CartActivity.CurrentActivity)
                {
                    current = (int)CartActivity.DetailNo;
                }
                else
                {
                    current = CartScanActivity.detailNoClickOption;
                }

                if (tranDetailItem.tranDetailItem.SysItemID == sysitemId && tranDetailItem.tranDetailItem.DetailNo == current)
                {
                    if (SelectSize != null)
                    {
                        if (SelectSize.ExSizeNo == 0) //Default Size = 999
                        {
                            size = tranDetailItem.tranDetailItem.SizeName;
                            itemPrice = tranDetailItem.tranDetailItem.ItemPrice;
                        }
                        else
                        {
                            size = SelectSize.ExSizeName;
                            itemPrice = SelectSize.Price;
                        }
                    }
                }

                if (SelectSize != null)
                {
                    tranDetailItem.tranDetailItem.Price = itemPrice;
                    tranDetailItem.tranDetailItem.ItemPrice = itemPrice;
                    tranDetailItem.tranDetailItem.SizeName = size;
                }
                else
                {
                    tranDetailItem.tranDetailItem.Price = itemPrice;
                    tranDetailItem.tranDetailItem.SizeName = size;
                }

                //มีการแก้ไขท้อปปิ้ง
                List<TranDetailItemTopping> lstNewTopping = new List<TranDetailItemTopping>();
                for (int i = 0; i < lstTranSelectTopping.Count; i++)
                {
                    TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                    {
                        MerchantID = tranWithDetails.tran.MerchantID,
                        SysBranchID = tranWithDetails.tran.SysBranchID,
                        TranNo = tranWithDetails.tran.TranNo,
                        DetailNo = detailNo,
                        ToppingNo = lstTranSelectTopping[i].ToppingNo,
                        ItemName = lstTranSelectTopping[i].ItemName,//toppping
                        SysItemID = lstTranSelectTopping[i].SysItemID,
                        UnitName = lstTranSelectTopping[i].UnitName,
                        RegularSizeName = null,
                        Quantity = lstTranSelectTopping[i].Quantity,
                        ToppingPrice = lstTranSelectTopping[i].ToppingPrice,
                        EstimateCost = lstTranSelectTopping[i].EstimateCost,
                        Comments = lstTranSelectTopping[i].Comments
                    };
                    lstNewTopping.Add(itemDetail);
                }

                List<TranDetailItemTopping> lstOldTopping = new List<TranDetailItemTopping>();
                for (int i = 0; i < tranDetailItem.tranDetailItemToppings.Count; i++)
                {
                    TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                    {
                        MerchantID = tranWithDetails.tran.MerchantID,
                        SysBranchID = tranWithDetails.tran.SysBranchID,
                        TranNo = tranWithDetails.tran.TranNo,
                        DetailNo = detailNo,
                        ToppingNo = tranDetailItem.tranDetailItemToppings[i].ToppingNo,
                        ItemName = tranDetailItem.tranDetailItemToppings[i].ItemName,//toppping
                        SysItemID = tranDetailItem.tranDetailItemToppings[i].SysItemID,
                        UnitName = tranDetailItem.tranDetailItemToppings[i].UnitName,
                        RegularSizeName = null,
                        Quantity = tranDetailItem.tranDetailItemToppings[i].Quantity,
                        ToppingPrice = tranDetailItem.tranDetailItemToppings[i].ToppingPrice,
                        EstimateCost = tranDetailItem.tranDetailItemToppings[i].EstimateCost,
                        Comments = tranDetailItem.tranDetailItemToppings[i].Comments
                    };
                    lstOldTopping.Add(itemDetail);
                }

                //เช็กว่าท้อปปิ้งซ้ำกันไหม
                if (lstNewTopping?.Count > 0 & lstOldTopping?.Count > 0)
                {
                    var check = lstNewTopping.Equals(lstOldTopping);
                    if (check)
                    {
                        lstitemDetail.AddRange(lstNewTopping);
                    }
                    else
                    {
                        lstitemDetail.AddRange(lstOldTopping);
                    }
                }
                else if (lstNewTopping?.Count == 0 & lstOldTopping?.Count > 0)
                {
                    lstitemDetail.AddRange(lstOldTopping);
                }
                else if (lstNewTopping?.Count > 0 & lstOldTopping?.Count == 0)
                {
                    lstitemDetail.AddRange(lstNewTopping);
                }
                else
                {
                    lstitemDetail = new List<TranDetailItemTopping>();
                }

                detailItemWithTopping = new TranDetailItemWithTopping()
                {
                    tranDetailItem = DetailItem,
                    tranDetailItemToppings = lstitemDetail
                };

                tranWithDetails = BLTrans.EditToppingRow(tranWithDetails, detailItemWithTopping);
                DataCashing.ModifyTranOrder = true;
                tranWithDetails = BLTrans.Caltran(tranWithDetails);

                lstTranSelectTopping = new List<TranDetailItemTopping>();
                SelectSize = new ItemExSize();
                SelectNote = new List<Note>();
                ExSizeNo = 0;
                sysitemIDToppping = 0;
                NoteID = 0;
                DataCashing.flagEditOptionSize = false;
                DataCashing.flagEditOptionExtraTopping = false;
                DataCashing.flagEditOptionNote = false;
                OptionActivity.flagLoadSize = false;
                PosActivity.SetTranDetail(tranWithDetails);
                IsActive = false;
                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateTotran at Option");
            }
        }

        private async void ShowCategoryNote()
        {
            try
            {
                if (listCategoryNote.Count > 0)
                {
                    lnNote.Visibility = ViewStates.Visible;
                    try
                    {
                        var Id = (int)listCategoryNote[0].SysNoteCategoryID;
                        nameCategoryNote = listCategoryNote[0].Name;
                        sysCategoryNote = listCategoryNote[0].SysNoteCategoryID;
                        LayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                        recyclerViewCategoryNote.HasFixedSize = true;
                        recyclerViewCategoryNote.SetLayoutManager(LayoutManager);
                        option_Adapter_NoteCategory = new Option_Adapter_NoteCategory(listCategoryNote);
                        recyclerViewCategoryNote.SetAdapter(option_Adapter_NoteCategory);
                        option_Adapter_NoteCategory.ItemClick += Option_adapter_noteCategory_ItemClick;

                        if (Id == 0)
                        {
                            await ShowDetail(0);
                        }
                        else
                        {
                            await ShowDetail(Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("ShowCategoryNote at Option");
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                }
                else
                {
                    lnNote.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowCategoryNote at Option");
                return;
            }
        }

        private void Option_adapter_noteCategory_ItemClick(object sender, int e)
        {
            nameCategoryNote = listCategoryNote[e].Name;
            sysCategoryNote = listCategoryNote[e].SysNoteCategoryID;
            var category = lstCategoryNote.Where(x => x.Name == nameCategoryNote & x.SysNoteCategoryID == sysCategoryNote).FirstOrDefault();
            if (category != null)
            {
                var ID = category.SysNoteCategoryID;
                ShowDetail(Convert.ToInt32(ID));
            }
            else
            {
                ShowDetail(Convert.ToInt32(0));
                if (DataCashing.Language == "th")
                {
                    nameCategoryNote = "ไม่มีกลุ่ม";
                    sysCategoryNote = 0;
                }
                else
                {
                    nameCategoryNote = "None";
                    sysCategoryNote = 0;
                }
            }

            LayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            recyclerViewCategoryNote.HasFixedSize = true;
            recyclerViewCategoryNote.SetLayoutManager(LayoutManager);
            option_Adapter_NoteCategory = new Option_Adapter_NoteCategory(listCategoryNote);
            recyclerViewCategoryNote.SetAdapter(option_Adapter_NoteCategory);
            option_Adapter_NoteCategory.ItemClick += Option_adapter_noteCategory_ItemClick;

            if (e > 4)
            {
                recyclerViewCategoryNote.ScrollToPosition(e);
            }
        }

        //Note
        public async Task ShowDetail(int fillterNote)
        {
            try
            {
                NoteManage noteManage = new NoteManage();
                if (fillterNote == 0)
                {
                    lstdatanote = await noteManage.GetNoteOnlyNoneGroup(DataCashingAll.MerchantId);
                }
                else
                {
                    lstdatanote = await noteManage.GetNoteBYCategory(DataCashingAll.MerchantId, fillterNote);

                    if (lstdatanote == null)
                    {
                        Toast.MakeText(this, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
                    }
                }

                lstShowNote = new ListNoteData(lstdatanote);
                //LayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                int row = 1;
                if (lstShowNote.Count > 4)
                {
                    row = (lstShowNote.Count / 4);
                }
                StaggeredGridLayoutManager layoutManager = new StaggeredGridLayoutManager(row, LinearLayoutManager.Horizontal);
                recyclerViewNote.HasFixedSize = true;
                recyclerViewNote.LayoutDirection = Android.Views.LayoutDirection.Locale;
                recyclerViewNote.SetLayoutManager(layoutManager);
                option_Adapter_Note = new Option_Adapter_Note(lstShowNote);
                recyclerViewNote.SetAdapter(option_Adapter_Note);
                option_Adapter_Note.ItemClick += Option_Adapter_Note_ItemClick;

                CreateChip(lstdatanote);


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("ShowDetail at Option");
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private async void Option_Adapter_Note_ItemClick(object sender, int e)
        {
            try
            {
                nameNote = lstShowNote[e].Message;

                if (NoteID != lstShowNote[e].SysNoteID)
                {
                    NoteID = lstShowNote[e].SysNoteID;
                }
                else
                {
                    NoteID = 0;
                }

                Note note = new Note()
                {
                    MerchantID = lstShowNote[e].MerchantID,
                    SysNoteID = lstShowNote[e].SysNoteID,
                    Ordinary = lstShowNote[e].Ordinary,
                    Message = lstShowNote[e].Message,
                    SysNoteCategoryID = lstShowNote[e].SysNoteCategoryID,
                    LastDateModified = DateTime.UtcNow,
                    UserLastModified = DataCashingAll.MerchantId.ToString()
                };
                SelectNote.Add(note);

                lstShowNote = new ListNoteData(lstdatanote);
                int row = 1;
                if (lstShowNote.Count > 4)
                {
                    row = (lstShowNote.Count / 4);
                }
                StaggeredGridLayoutManager layoutManager = new StaggeredGridLayoutManager(row, LinearLayoutManager.Horizontal);
                recyclerViewNote.HasFixedSize = true;
                recyclerViewNote.LayoutDirection = Android.Views.LayoutDirection.Locale;
                recyclerViewNote.SetLayoutManager(layoutManager);
                option_Adapter_Note = new Option_Adapter_Note(lstShowNote);
                recyclerViewNote.SetAdapter(option_Adapter_Note);
                option_Adapter_Note.ItemClick += Option_Adapter_Note_ItemClick;
                textInsertNote.Text += lstShowNote[e].Message + " ";
                textInsertNote.SetSelection(textInsertNote.Text.Length);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("Option_Adapter_Note_ItemClick at Option");
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        private async Task GetSizeItem()
        {
            try
            {
                List<ItemExSize> exSizes = new List<ItemExSize>();
                lstExSize = new List<ItemExSize>();
                ItemExSizeManage = new ItemExSizeManage();
                decimal price = 0;                

                //ItemPrice จาก Item
                if (CartDetaItem != null && POSDataItem == null) //แก้ไข option ที่ cart
                {
                    price = CartDetaItem.Price;
                    exSizes = await ItemExSizeManage.GetItemSize(DataCashingAll.MerchantId, (int)sysitemId);
                }
                else  //เลือก option ที่ Option // POSDataItem != null
                {
                    if (POSDataItem == null)
                    {
                        Log.Debug("POSActivitypass", " POSDataItem is null");
                    }
                    else
                    {
                        price = POSDataItem.Price;
                    }                    
                    exSizes = await ItemExSizeManage.GetItemSize(DataCashingAll.MerchantId, (int)ItemShopOption);
                }
                
                defaultSize = new ItemExSize() { MerchantID = DataCashingAll.MerchantId, ExSizeNo = 999, ExSizeName = "Default Size", Price = price };
                lstExSize.Add(defaultSize);

                if (exSizes == null)
                {
                    exSizes = new List<ItemExSize>();
                }
                lstExSize.AddRange(exSizes);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ =  TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetSizeItem at Option");
                return;
            }
        }

        private async void SetSizeItem()
        {
            try
            {
                if (lstExSize.Count > 1)
                {
                    lnSize.Visibility = ViewStates.Visible;
                    lstItemExSize = new ListItemExSize(lstExSize);
                    gridLayoutManagerSize = new GridLayoutManager(this, 1, 1, false);
                    recyclerViewListsize.HasFixedSize = true;
                    recyclerViewListsize.SetLayoutManager(gridLayoutManagerSize);
                    option_adapter_size = new Option_Adapter_Size(lstItemExSize);
                    recyclerViewListsize.SetAdapter(option_adapter_size);
                    option_adapter_size.ItemClick += Option_adapter_size_ItemClick;
                }
                else
                {
                    lnSize.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("ListSizeItem at Option");
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        private async void Option_adapter_size_ItemClick(object sender, int e)
        {
            try
            {
                ExSizeNo = lstExSize[e].ExSizeNo;
                SelectSize = new ItemExSize()
                {
                    SysItemID = lstExSize[e].SysItemID,
                    ExSizeName = lstExSize[e].ExSizeName,
                    ExSizeNo = lstExSize[e].ExSizeNo,
                    EstimateCost = lstExSize[e].EstimateCost,
                    MerchantID = lstExSize[e].MerchantID,
                    Price = lstExSize[e].Price,
                    Comments = lstExSize[e].Comments
                };
                gridLayoutManagerSize = new GridLayoutManager(this, 1, 1, false);
                recyclerViewListsize.HasFixedSize = true;
                recyclerViewListsize.SetLayoutManager(gridLayoutManagerSize);
                option_adapter_size = new Option_Adapter_Size(lstItemExSize);
                recyclerViewListsize.SetAdapter(option_adapter_size);
                option_adapter_size.ItemClick += Option_adapter_size_ItemClick;

                if (e > 4)
                {
                    recyclerViewListsize.ScrollToPosition(e);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Option_adapter_size_ItemClick at Option");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        //ExtraTopping Item
        private async void SetExtra()
        {
            try
            {
                if (itemExtra.Count > 0)
                {
                    listExtraItem = new ListItem(itemExtra);
                    gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                    recyclerViewExtra.SetLayoutManager(gridLayoutItem);
                    recyclerViewExtra.HasFixedSize = true;
                    recyclerViewExtra.SetItemViewCacheSize(20);
                    option_Adapter_Extra = new Option_Adapter_Extra(listExtraItem);
                    recyclerViewExtra.SetAdapter(option_Adapter_Extra);
                    option_Adapter_Extra.ItemClick += Option_Adapter_Extra_ItemClick;
                }
                else
                {
                    lnExtra.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetExtra at Option");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Option_Adapter_Extra_ItemClick(object sender, int e)
        {
            try
            {
                sysitemIDToppping = listExtraItem[e].SysItemID;
                listExtraItem = new ListItem(itemExtra);
                gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
                recyclerViewExtra.SetLayoutManager(gridLayoutItem);
                recyclerViewExtra.HasFixedSize = true;
                recyclerViewExtra.SetItemViewCacheSize(20);
                option_Adapter_Extra = new Option_Adapter_Extra(listExtraItem);
                recyclerViewExtra.SetAdapter(option_Adapter_Extra);

                //RunOnUiThread(() => option_Adapter_Extra.NotifyDataSetChanged());

                option_Adapter_Extra.ItemClick += Option_Adapter_Extra_ItemClick;

                if (e > 7)
                {
                    recyclerViewListsize.ScrollToPosition(e);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Option_Adapter_Extra_ItemClick at Option");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async Task<List<Item>> GetExtraList()
        {
            try
            {
                itemExtra = new List<Item>();
                ItemManage itemManage = new ItemManage();
                itemExtra = await itemManage.GetToppingItemByCategory((int)listExtraCategory[0].SysCategoryID);
                if (itemExtra == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                    return null;
                }
                return itemExtra;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetExtraList at Option");
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
                return null;
            }
        }

        //ExtraTopping Category
        private async Task SetExtraCategory()
        {
            try
            {
                CategoryManage categoryManage = new CategoryManage();
                List<Category> noneCategory = new List<Category>();
                ItemManage itemManage = new ItemManage();
                List<Item> lstToppingOnlyNoneGroup = new List<Item>();

                itemExtraCategory = await categoryManage.GetCategoryOption();
                //หมวดหมู่ที่ไม่มีสินค้าจะไม่แสดง
                itemExtraCategory = itemExtraCategory.Where(x => itemExtraCategory.Select(y => (long)y.SysCategoryID).ToList().Contains(x.SysCategoryID)).ToList();

                lstToppingOnlyNoneGroup = await itemManage.GetToppingOnlyNoneGroup();
                if (lstToppingOnlyNoneGroup.Count > 0)
                {
                    if (DataCashing.Language == "th")
                    {
                        noneCategory.Add(new Category { MerchantID = DataCashingAll.MerchantId, SysCategoryID = 0, Name = "ไม่มีกลุ่ม", Ordinary = null, DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 0, WaitSendingTime = DateTime.UtcNow, LinkProMaxxID = null });
                    }
                    else
                    {
                        noneCategory.Add(new Category { MerchantID = DataCashingAll.MerchantId, SysCategoryID = 0, Name = "None", Ordinary = null, DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 0, WaitSendingTime = DateTime.UtcNow, LinkProMaxxID = null });
                    }
                }
                noneCategory.AddRange(itemExtraCategory);

                listExtraCategory = new ListCategory(noneCategory);
                if (listExtraCategory.Count > 0)
                {
                    recyclerViewExtraCategory.Visibility = ViewStates.Visible;
                    nameCategory = listExtraCategory[0].Name;
                    sysCategory = listExtraCategory[0].SysCategoryID;

                    await Showfilter((int)sysCategory);
                }
                else
                {
                    recyclerViewExtraCategory.Visibility = ViewStates.Invisible;
                    await Showfilter(0);
                }

                mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                recyclerViewExtraCategory.HasFixedSize = true;
                recyclerViewExtraCategory.SetLayoutManager(mLayoutManager);
                option_adapter_categoryextra = new Option_Adapter_CategoryExtra(listExtraCategory);
                recyclerViewExtraCategory.SetAdapter(option_adapter_categoryextra);
                option_adapter_categoryextra.ItemClick += Option_adapter_categoryextra_ItemClick;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetExtraCategory at Option");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Option_adapter_categoryextra_ItemClick(object sender, int e)
        {
            mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
            recyclerViewExtraCategory.HasFixedSize = true;
            recyclerViewExtraCategory.SetLayoutManager(mLayoutManager);
            option_adapter_categoryextra = new Option_Adapter_CategoryExtra(listExtraCategory);
            recyclerViewExtraCategory.SetAdapter(option_adapter_categoryextra);
            option_adapter_categoryextra.ItemClick += Option_adapter_categoryextra_ItemClick;

            nameCategory = listExtraCategory[e].Name;
            sysCategory = listExtraCategory[e].SysCategoryID;
            if (sysCategory == 0)
            {
                await Showfilter(0);
            }
            else
            {
                await Showfilter((int)sysCategory);
            }

            gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
            recyclerViewExtra.SetLayoutManager(gridLayoutItem);
            recyclerViewExtra.HasFixedSize = true;
            recyclerViewExtra.SetItemViewCacheSize(20);
            option_Adapter_Extra = new Option_Adapter_Extra(listExtraItem);
            recyclerViewExtra.SetAdapter(option_Adapter_Extra);
            option_Adapter_Extra.ItemClick += Option_Adapter_Extra_ItemClick;

            if (e > 5)
            {
                recyclerViewExtraCategory.ScrollToPosition(e);
            }
        }

        public async Task Showfilter(int fillterId)
        {
            fillter = fillterId;
            itemExtra = new List<Item>();
            ItemManage itemManage = new ItemManage();
            try
            {
                if (fillter == 0)
                {
                    itemExtra = await itemManage.GetToppingOnlyNoneGroup();
                }
                else
                {
                    itemExtra = await itemManage.GetToppingItemByCategory(fillter);
                }
                listExtraItem = new ListItem(itemExtra);
                if (itemExtra == null)
                {
                    Toast.MakeText(this, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
            }
        }


        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
            //ClickBack();
            //if (DataCashing.Language == "th")
            //{
            //    nameCategory = "ไม่มีกลุ่ม";
            //    nameCategoryNote = "ไม่มีกลุ่ม";
            //}
            //else
            //{
            //    nameCategory = "None";
            //    nameCategoryNote = "None";
            //}
            //sysCategoryNote = 0;
            //sysCategory = 0;

            //DataCashing.flagEditOptionSize = false;
            //DataCashing.flagEditOptionExtraTopping = false;
            //DataCashing.flagEditOptionNote = false;
            //OptionActivity.flagLoadSize = false;
            //POSDataItem = null;
            //this.Finish();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            ClickBack();
            if (DataCashing.Language == "th")
            {
                nameCategory = "ไม่มีกลุ่ม";
                nameCategoryNote = "ไม่มีกลุ่ม";
            }
            else
            {
                nameCategory = "None";
                nameCategoryNote = "None";
            }
            sysCategoryNote = 0;
            sysCategory = 0;

            DataCashing.flagEditOptionSize = false;
            DataCashing.flagEditOptionExtraTopping = false;
            DataCashing.flagEditOptionNote = false;
            OptionActivity.flagLoadSize = false;
            POSDataItem = null;
            CartDetaItem = null;
            this.Finish();
        }

        void ClickBack()
        {
            TranDetailItemWithTopping tranDetailItemWithToppings = new TranDetailItemWithTopping();
            if (CartActivity.CurrentActivity)
            {
                tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == CartActivity.DetailNo).FirstOrDefault();
            }
            else
            {
                tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == CartScanActivity.detailNoClickOption).FirstOrDefault();
            }

            if (tranDetailItemWithToppings != null)
            {
                tranDetailItemWithToppings.tranDetailItemToppings.AddRange(lstTempTopping);
                PosActivity.SetTranDetail(tranWithDetails);
            }
        }

        public static void SetSysItem(long? itemId)
        {
            sysitemId = itemId;
        }

        public static void SetDataItem(Item dataitem)
        {
            CartDetaItem = new Item();
            CartDetaItem = dataitem;             
        }

        public static void SetDataItemfromPOS(Item dataitem)
        {           
            POSDataItem = new Item();
            if (dataitem == null)
            {
                Log.Debug("POSActivitypass", "dataitem is null");
                ItemShopOption = 0;
            }
            else
            {
                POSDataItem = dataitem;
                ItemShopOption = POSDataItem.SysItemID;
            } 
        }

        public static void SetItemDetail(TranDetailItemNew item)
        {
            ItemEditTopping = item;
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        public static void SetlistCategoryNote(List<NoteCategory> noteCategories)
        {
            lstCategoryNote = noteCategories;
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();

                CheckJwt();
                IsActive = true;
                await showData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Option");
                return;
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'OptionActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'OptionActivity.openPage' is assigned but its value is never used
        public DateTime pauseDate = DateTime.Now;
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

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

        }
    }
}