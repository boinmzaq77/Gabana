using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Chip;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Option;
using Gabana.Droid.Tablet.Adapter.Pos;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class POS_Dialog_Option : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static POS_Dialog_Option NewInstance()
        {
            var frag = new POS_Dialog_Option { Arguments = new Bundle() };
            return frag;
        }

        public static POS_Dialog_Option dialog_Option;
        View view;
        public static List<TranDetailItemTopping> lstTranSelectTopping;
        public static List<NoteCategory> lstCategoryNote = new List<NoteCategory>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_option, container, false);
            try
            {
                dialog_Option = this;
                CombinUI();

                IsActive = true;
                tranWithDetails = MainActivity.tranWithDetails;

                lstTranSelectTopping = new List<TranDetailItemTopping>();
                lstTempTopping = new List<TranDetailItemTopping>();
                SelectSize = new ItemExSize();
                //noteCategories 
                listCategoryNote = new ListNoteCategory(lstCategoryNote);
                showData();
                return view;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Option");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }
        public static void SetlistCategoryNote(List<NoteCategory> noteCategories)
        {
            lstCategoryNote = noteCategories;
        }
        public async void showData()
        {
            try
            {
                if (tranWithDetails.tran != null)
                {
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

                    int row = -1;
                    if (DataCashing.flagEditOptionNote)
                    {
                        if (POS_Dialog_Scan.scan != null)
                        {
                            row = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == sysitemId & x.tranDetailItem.DetailNo == POS_Dialog_Scan.detailNoClickOption);
                            if (row != -1)
                            {
                                textInsertNote.Text = tranWithDetails.tranDetailItemWithToppings[row].tranDetailItem.Comments;
                                DataCashing.flagEditOptionNote = false;
                            }
                        }
                        else
                        {
                            row = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == sysitemId & x.tranDetailItem.DetailNo == POS_Fragment_Cart.DetailNo);
                            if (row != -1)
                            {
                                textInsertNote.Text = tranWithDetails.tranDetailItemWithToppings[row].tranDetailItem.Comments;
                                DataCashing.flagEditOptionNote = false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("showData at Dialog Option");
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

        public static TranDetailItemNew ItemEditTopping;
        public static void SetItemDetail(TranDetailItemNew item)
        {
            ItemEditTopping = item;
        }

        List<ItemExSize> lstExSize = new List<ItemExSize>();
        ItemExSizeManage itemexsizemanage;
        List<ItemExSize> exSizes = new List<ItemExSize>();
        private async Task GetSizeItem()
        {
            try
            {
                List<ItemExSize> exSizes = new List<ItemExSize>();
                lstExSize = new List<ItemExSize>();
                itemexsizemanage = new ItemExSizeManage();
                decimal price = 0;

                //ItemPrice จาก Item
                if (CartDetaItem != null && POSDataItem == null) //แก้ไข option ที่ cart
                {
                    price = CartDetaItem.Price;
                    exSizes = await itemexsizemanage.GetItemSize(DataCashingAll.MerchantId, (int)sysitemId);
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
                    exSizes = await itemexsizemanage.GetItemSize(DataCashingAll.MerchantId, (int)ItemShopOption);
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
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetSizeItem at OptionDialog");
                return;
            }
        }
        public List<Category> itemExtraCategory = new List<Category>();
        private ListCategory listExtraCategory;
        Option_Adapter_CategoryExtra option_adapter_categoryextra;
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
                noneCategory.AddRange(itemExtraCategory);

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

                listExtraCategory = new ListCategory(noneCategory);
                if (listExtraCategory.Count > 0)
                {
                    recyclerViewExtraCategory.Visibility = ViewStates.Visible;
                    nameCategory = listExtraCategory[0].Name;
                    sysCategory = listExtraCategory[0].SysCategoryID;

                    Showfilter((int)sysCategory);
                }
                else
                {
                    recyclerViewExtraCategory.Visibility = ViewStates.Invisible;
                    Showfilter(0);
                }

                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false);
                recyclerViewExtraCategory.HasFixedSize = true;
                recyclerViewExtraCategory.SetLayoutManager(mLayoutManager);
                option_adapter_categoryextra = new Option_Adapter_CategoryExtra(listExtraCategory);
                recyclerViewExtraCategory.SetAdapter(option_adapter_categoryextra);
                option_adapter_categoryextra.ItemClick += Option_adapter_categoryextra_ItemClick; ;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetExtraCategory at Option");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Option_adapter_categoryextra_ItemClick(object sender, int e)
        {
            try
            {
                LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false);
                recyclerViewExtraCategory.HasFixedSize = true;
                recyclerViewExtraCategory.SetLayoutManager(mLayoutManager);
                option_adapter_categoryextra = new Option_Adapter_CategoryExtra(listExtraCategory);
                recyclerViewExtraCategory.SetAdapter(option_adapter_categoryextra);
                option_adapter_categoryextra.ItemClick += Option_adapter_categoryextra_ItemClick;

                nameCategory = listExtraCategory[e].Name;
                sysCategory = listExtraCategory[e].SysCategoryID;
                if (sysCategory == 0)
                {
                    Showfilter(0);
                }
                else
                {
                    Showfilter((int)sysCategory);
                }

                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Context, 1, 1, false);
                recyclerViewExtra.SetLayoutManager(gridLayoutItem);
                recyclerViewExtra.HasFixedSize = true;
                recyclerViewExtra.SetItemViewCacheSize(20);
                option_adapter_extra = new Option_Adapter_Extra(listExtraItem);
                recyclerViewExtra.SetAdapter(option_adapter_extra);
                option_adapter_extra.ItemClick += Option_adapter_extra_ItemClick; ;

                if (e > 5)
                {
                    recyclerViewExtraCategory.ScrollToPosition(e);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetExtra at Option");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void Option_adapter_extra_ItemClick(object sender, int e)
        {
            try
            {
                sysitemIDToppping = listExtraItem[e].SysItemID;
                listExtraItem = new ListItem(itemExtra);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Context, 1, 1, false);
                recyclerViewExtra.SetLayoutManager(gridLayoutItem);
                recyclerViewExtra.HasFixedSize = true;
                recyclerViewExtra.SetItemViewCacheSize(20);
                option_adapter_extra = new Option_Adapter_Extra(listExtraItem);
                recyclerViewExtra.SetAdapter(option_adapter_extra);

                //RunOnUiThread(() => option_Adapter_Extra.NotifyDataSetChanged());

                option_adapter_extra.ItemClick += Option_adapter_extra_ItemClick;

                if (e > 7)
                {
                    recyclerViewListsize.ScrollToPosition(e);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Option_Adapter_Extra_ItemClick at Option");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        Option_Adapter_Extra option_adapter_extra;
        ListItemExSize lstItemExSize;
        Option_Adapter_Size option_adapter_size;
        private async void SetSizeItem()
        {
            try
            {
                if (lstExSize.Count > 1)
                {
                    lnSize.Visibility = ViewStates.Visible;
                    lstItemExSize = new ListItemExSize(lstExSize);
                    GridLayoutManager gridLayoutManagerSize = new GridLayoutManager(this.Context, 1, 1, false);
                    recyclerViewListsize.HasFixedSize = true;
                    recyclerViewListsize.SetLayoutManager(gridLayoutManagerSize);
                    option_adapter_size = new Option_Adapter_Size(lstItemExSize);
                    recyclerViewListsize.SetAdapter(option_adapter_size);
                    option_adapter_size.ItemClick += Option_adapter_size_ItemClick; ;
                }
                else
                {
                    lnSize.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
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
                GridLayoutManager gridLayoutManagerSize = new GridLayoutManager(this.Context, 1, 1, false);
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
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetExtra()
        {
            try
            {
                if (itemExtra.Count > 0)
                {
                    listExtraItem = new ListItem(itemExtra);
                    GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Context, 1, 1, false);
                    recyclerViewExtra.SetLayoutManager(gridLayoutItem);
                    recyclerViewExtra.HasFixedSize = true;
                    recyclerViewExtra.SetItemViewCacheSize(20);
                    option_adapter_extra = new Option_Adapter_Extra(listExtraItem);
                    recyclerViewExtra.SetAdapter(option_adapter_extra);
                    option_adapter_extra.ItemClick += Option_adapter_extra_ItemClick;
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
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        int fillter;
        public static List<Item> itemExtra;
        private static ListItem listExtraItem;
        public async void Showfilter(int fillterId)
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
                    Toast.MakeText(this.Context, "เรียกข้อมูลไอเท็มไม่ได้", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Debug("error", ex.Message);
            }
        }


        public static TranWithDetailsLocal tranWithDetails;
        public static long ItemShopOption = 0;
        public static int PositionClick = 0;

        LinearLayout lnBack, lnSize, lnNote, lnExtra;
        EditText textInsertNote;
        RecyclerView recyclerViewListsize, recyclerViewCategoryNote, recyclerViewNote, recyclerViewExtraCategory, recyclerViewExtra;
        Button btnAddtoCart;
        ChipGroup chipGroup;
        ListNoteCategory listCategoryNote ;
        public static List<Note> SelectNote = new List<Note>();
        public static long? sysitemId;

        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnSize = view.FindViewById<LinearLayout>(Resource.Id.lnSize);
            lnNote = view.FindViewById<LinearLayout>(Resource.Id.lnNote);
            lnExtra = view.FindViewById<LinearLayout>(Resource.Id.lnExtra);
            ImageButton btnBack = view.FindViewById<ImageButton>(Resource.Id.btnBack);
            textInsertNote = view.FindViewById<EditText>(Resource.Id.textComment);
            recyclerViewListsize = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewListsize);
            recyclerViewCategoryNote = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewNoteCategory);
            recyclerViewNote = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewNote);
            recyclerViewExtraCategory = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewExtraCategory);
            recyclerViewExtra = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewExtra);
            btnAddtoCart = view.FindViewById<Button>(Resource.Id.btnAddtoCart);

            chipGroup = view.FindViewById<ChipGroup>(Resource.Id.chipGroup);

            lnBack.Click += LnBack_Click;
            btnBack.Click += LnBack_Click;
            btnAddtoCart.Click += BtnAddtoCart_Click;
            textInsertNote.Text = string.Empty;
        }
        public static bool flagLoadSize = false;
        private void BtnAddtoCart_Click(object sender, EventArgs e)
        {
            try
            {
                DialogLoading dialogLoading   = new DialogLoading();
                dialogLoading.Cancelable = false;
                dialogLoading.Show(Activity.SupportFragmentManager, nameof(DialogLoading));
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
                POS_Fragment_Main.fragment_option = null;
                dialogLoading.Dismiss();
                this.Dialog.Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("BtnAddtoCart_Click at Option");
                _= TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        public static ItemExSize SelectSize;
        public static ItemExSize defaultSize;
        TranDetailItemWithTopping detailItemWithTopping;

        public static long sysitemIDToppping = 0;
        public static long ExSizeNo = 0;
        public static long NoteID = 0;
        public static bool IsActive = false ;
        public static Item CartDetaItem;

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

                lstTranSelectTopping = new List<TranDetailItemTopping>();
                SelectSize = new ItemExSize();
                SelectNote = new List<Note>();
                ExSizeNo = 0;
                sysitemIDToppping = 0;
                NoteID = 0;
                POSDataItem = null;
                ItemShopOption = 0;
                CartDetaItem = null;
                flagLoadSize = false;
                IsActive = false;

                POS_Fragment_Main.tranDetailItemWithTopping = detailItemWithTopping;
                POS_Fragment_Main.SelectItemtoCart(PositionClick);

                decimal quantuty = MainActivity.tranWithDetails.tranDetailItemWithToppings.ToList().Where(x => x.tranDetailItem.SysItemID == POS_Fragment_Main.listItem[PositionClick].SysItemID).Sum(x => x.tranDetailItem.Quantity);
                POS_Fragment_Main.ItemName.Text = (int)quantuty + "x " + POS_Fragment_Main.listItem[PositionClick].ItemName?.ToString();

                MainActivity.tranWithDetails = tranWithDetails;
                //POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();

                if (POS_Dialog_Scan.scan != null)
                {
                    POS_Dialog_Scan.scan.OnResume();
                }
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("InsertTotran at Option");
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        TranDetailItemWithTopping tranDetailItem = new TranDetailItemWithTopping();
        void UpdateTotran()
        {
            try
            {
                //Item
                //Size
                //SelectSize
                string size = string.Empty;
                decimal itemPrice = 0;
                List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
                TranDetailItemNew DetailItem = new TranDetailItemNew();

                long detailNo = 0;
                if (POS_Dialog_Scan.scan != null)
                {
                    tranDetailItem = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == POS_Dialog_Scan.detailNoClickOption).FirstOrDefault();
                }
                else
                {
                    tranDetailItem = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == POS_Fragment_Cart.DetailNo).FirstOrDefault();
                }

                if (tranDetailItem != null)
                {
                    detailNo = tranDetailItem.tranDetailItem.DetailNo;
                    DetailItem = tranDetailItem.tranDetailItem;

                    //Note เดิม
                    DetailItem.Comments = textInsertNote.Text;
                }

                int current = 0;
                if (POS_Dialog_Scan.scan != null)
                {
                    current = POS_Dialog_Scan.detailNoClickOption;
                }
                else
                {
                    current = (int)POS_Fragment_Cart.DetailNo;
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
                flagLoadSize = false;

                MainActivity.tranWithDetails = tranWithDetails;
                //POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();
                //POS_Fragment_Cart.DetailNo = 0;

                if (POS_Dialog_Scan.scan != null)
                {
                     POS_Dialog_Scan.scan.OnResume();
                }
                IsActive = false;
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateTotran at Option");
            }
        }

        LinearLayoutManager LayoutManager;
        Option_Adapter_NoteCategory option_adapter_notecategory;
        private async void ShowCategoryNote()
        {
            try
            {
                if (listCategoryNote == null)
                {
                    lnNote.Visibility = ViewStates.Gone;
                }
                if (listCategoryNote.Count > 0)
                {
                    lnNote.Visibility = ViewStates.Visible;
                    try
                    {
                        var Id = (int)listCategoryNote[0].SysNoteCategoryID;
                        nameCategoryNote = listCategoryNote[0].Name;
                        sysCategoryNote = listCategoryNote[0].SysNoteCategoryID;
                        LayoutManager = new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false);
                        recyclerViewCategoryNote.HasFixedSize = true;
                        recyclerViewCategoryNote.SetLayoutManager(LayoutManager);
                        option_adapter_notecategory = new Option_Adapter_NoteCategory(listCategoryNote);
                        recyclerViewCategoryNote.SetAdapter(option_adapter_notecategory);
                        option_adapter_notecategory.ItemClick += Option_adapter_notecategory_ItemClick; ;

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
                        Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                    }
                }
                else
                {
                    lnNote.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowCategoryNote at Option");
                return;
            }
        }

        private async void Option_adapter_notecategory_ItemClick(object sender, int e)
        {
            try
            {
                nameCategoryNote = listCategoryNote[e].Name;
                sysCategoryNote = listCategoryNote[e].SysNoteCategoryID;
                var category = lstCategoryNote.Where(x => x.Name == nameCategoryNote & x.SysNoteCategoryID == sysCategoryNote).FirstOrDefault();
                if (category != null)
                {
                    var ID = category.SysNoteCategoryID;
                    await ShowDetail(Convert.ToInt32(ID));
                }
                else
                {
                    await ShowDetail(Convert.ToInt32(0));
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

                LayoutManager = new LinearLayoutManager(this.Context, LinearLayoutManager.Horizontal, false);
                recyclerViewCategoryNote.HasFixedSize = true;
                recyclerViewCategoryNote.SetLayoutManager(LayoutManager);
                option_adapter_notecategory = new Option_Adapter_NoteCategory(listCategoryNote);
                recyclerViewCategoryNote.SetAdapter(option_adapter_notecategory);
                option_adapter_notecategory.ItemClick += Option_adapter_notecategory_ItemClick;

                if (e > 4)
                {
                    recyclerViewCategoryNote.ScrollToPosition(e);
                }
            }
            catch (Exception)
            {

                
            }
        }

        static List<Note> lstdatanote;
        ListNoteData lstShowNote;
        Option_Adapter_Note option_adapter_note;
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
                        Toast.MakeText(this.Context, "ไม่สามารถเรียกข้อมูลได้", ToastLength.Short).Show();
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
                option_adapter_note = new Option_Adapter_Note(lstShowNote);
                recyclerViewNote.SetAdapter(option_adapter_note);
                option_adapter_note.ItemClick += Option_adapter_note_ItemClick;

                CreateChip(lstdatanote);


            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("ShowDetail at Option");
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        public static string nameNote = "";
        private async void Option_adapter_note_ItemClick(object sender, int e)
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
                option_adapter_note = new Option_Adapter_Note(lstShowNote);
                recyclerViewNote.SetAdapter(option_adapter_note);
                option_adapter_note.ItemClick += Option_adapter_note_ItemClick;
                textInsertNote.Text += lstShowNote[e].Message + " ";
                textInsertNote.SetSelection(textInsertNote.Text.Length);

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("Option_Adapter_Note_ItemClick at Option");
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }

        }
        private void CreateChip(List<Note> notes)
        {
            chipGroup.RemoveAllViews();
            foreach (var item in notes)
            {
                var inflater = LayoutInflater.From(this.Context);
                var chip = inflater.Inflate(Resource.Layout.chip_note, null, false) as Chip;
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
        public static string nameCategoryNote = "";
        public static string nameCategory = "";
        public static long sysCategoryNote = 0;
        public static long sysCategory = 0;
        private void LnBack_Click(object sender, EventArgs e)
        {            
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
            flagLoadSize = false;
            POSDataItem = null;
            POS_Fragment_Main.fragment_option = null;
            Dismiss();
        }
        public static List<TranDetailItemTopping> lstTempTopping;
        void ClickBack()
        {
            TranDetailItemWithTopping tranDetailItemWithToppings = new TranDetailItemWithTopping();
            if (POS_Dialog_Scan.scan != null)
            {
                tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == POS_Dialog_Scan.detailNoClickOption).FirstOrDefault();
            }
            else
            {
                tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings.Where(x => x.tranDetailItem.SysItemID == sysitemId && x.tranDetailItem.DetailNo == POS_Fragment_Cart.DetailNo).FirstOrDefault();
            }

            if (tranDetailItemWithToppings != null)
            {
                tranDetailItemWithToppings.tranDetailItemToppings.AddRange(lstTempTopping);
                MainActivity.tranWithDetails = tranWithDetails;
            }
        }

        public static Item POSDataItem;
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
    }


}