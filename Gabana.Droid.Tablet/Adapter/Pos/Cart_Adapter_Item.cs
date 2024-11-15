using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Pos;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Adapter
{
    public class Cart_Adapter_Item : RecyclerView.Adapter
    {
        //private List<Item> ilistItem;
        private TranWithDetailsLocal tranWithDetails;
        public event EventHandler<int> ItemClick;
        long? itemId;
        long? detailNo;
        private static Context context;
        private static RecyclerView scvlistTopping;
        

        public Cart_Adapter_Item(TranWithDetailsLocal tran)
        {
            tranWithDetails = tran;
        }

        public override int ItemCount
        {
            get { return tranWithDetails == null ? 0 : tranWithDetails.tranDetailItemWithToppings.Count; }
        }

        public override  void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewItemHolder vh = holder as ListViewItemHolder;
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;

                string sizename = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SizeName;
                if (string.IsNullOrEmpty(sizename) || sizename == "Default Size")
                {
                    vh.ItemName.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemName?.ToString();
                }               
                else
                {                    
                    vh.ItemName.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemName?.ToString() + " " + sizename;
                }

                #region แก้ไขจำนวนสินค้า
                if (DataCashing.flagEditQuantity)
                {
                    int updateQuantity  = -1;
                    if (POS_Dialog_Scan.scan != null)
                    {
                        updateQuantity = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == DataCashing.EditItemID && x.tranDetailItem.DetailNo == POS_Dialog_Scan.detailNoClickOption);
                    }
                    else
                    {
                        updateQuantity = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == DataCashing.EditItemID && x.tranDetailItem.DetailNo == POS_Fragment_Cart.DetailNo);
                    }

                    if (updateQuantity != -1)
                    {
                        //Change Quantity
                        //เมื่อ Change Quantity แล้วถึงจะเข้า  BLTrans.Caltran(tranWithDetails)
                        //Row ที่เลือก
                        var tranDetailItemWithTopping = tranWithDetails.tranDetailItemWithToppings[updateQuantity];
                        tranWithDetails = BLTrans.ChangeQuantity(tranWithDetails, tranDetailItemWithTopping.tranDetailItem, DataCashing.EditQuantity);
                        DataCashing.ModifyTranOrder = true;
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                        CalCartDiscount();

                        vh.CountItem.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Quantity.ToString("#,###");

                        //แก้ไข VAT ,Total,Service Charge ใหม่ หลังจากมีการเปลี่ยนแปลงจำนวนสินค้า
                        
                        POS_Fragment_Cart.textTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
                        POS_Fragment_Cart.textNumVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                        POS_Fragment_Cart.textNumService.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                        
                        if (POS_Dialog_Scan.scan != null)
                        {
                            POS_Dialog_Scan.scan.textTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
                            POS_Dialog_Scan.scan.textNumVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                            POS_Dialog_Scan.scan.textNumService.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                        }
                    }
                    else
                    {
                        return;
                    }

                    POS_Fragment_Cart.flagEditQuantity = false;
                    DataCashing.flagEditQuantity = false;
                }
                else
                {
                    vh.CountItem.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Quantity.ToString("#,###");
                }
                #endregion

                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                decimal price = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Price;
                decimal ItemPrice = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemPrice;

                vh.ItemPrice.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(ItemPrice);
                if (price != ItemPrice)
                {
                    vh.ItemPrice.Text = "  "+ CURRENCYSYMBOLS + Utils.DisplayDecimal(price);
                    vh.PriceBeforeDis.Visibility = ViewStates.Visible;
                    vh.PriceBeforeDis.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(ItemPrice);
                    vh.PriceBeforeDis.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
                }
                else
                {
                    vh.ItemPrice.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(ItemPrice);
                    vh.PriceBeforeDis.Visibility = ViewStates.Gone;
                }
                vh.PriceTotal.Text = Utils.DisplayDecimal(price);

                if (tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID != null)
                {
                    var priceTotal = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Amount.ToString();
                    vh.PriceTotal.Text = Utils.DisplayDecimal(Convert.ToDecimal(priceTotal));
                }
                else
                {
                    //กรณีที่สินค้าเป็น dummy
                    decimal sumItem = 0; 
                    sumItem = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Amount; //ราคาสินค้า  
                    vh.PriceTotal.Text = Utils.DisplayDecimal(sumItem);
                }

                //comment item
                var comment = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Comments;
                if (String.IsNullOrEmpty(comment))
                {
                    vh.lnComment.Visibility = ViewStates.Gone;
                    vh.Comment.Text = string.Empty;
                }
                else
                {
                    vh.Comment.Text = comment;
                    vh.lnComment.Visibility = ViewStates.Visible;
                }

                #region saleitemType
                //saleitemType = 'D'
                var saleItemType = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SaleItemType;
                if (saleItemType == 'D')
                {
                    vh.lnOption.Visibility = ViewStates.Gone;
                }
                #endregion

                vh.lnQuantity.Click += (s, e) =>
                {
                    try
                    {
                        itemId = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID;
                        DataCashing.EditItemID = (int)itemId;
                        LnQuantityClick(itemId, tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Quantity);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("lnQuantity at cart_adapter");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };

                vh.lnOption.Click += async (s, e) =>
                {
                    try
                    {
                        itemId = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID;
                        var Detailitem = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem;
                        ItemManage itemManage = new ItemManage();
                        Item Datdaitem = await itemManage.GetItem(DataCashingAll.MerchantId, (int)itemId);
                        LnOptionClick(tranWithDetails, itemId, Detailitem, Datdaitem);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("lnOption at cart_adapter");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };

                vh.lnChangePrice.Click += (s, e) =>
                {
                    try
                    {
                        itemId = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID;
                        var item = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem;
                        LnPriceClick(item, detailNo);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("lnChangePrice at cart_adapter");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };
                vh.lnDiscount.Click += (s, e) =>
                {
                    try
                    {
                        TranDetailItemNew tranDetail = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem;
                        LnDiscountClick(tranDetail);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("lnDiscount at cart_adapter");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };
                vh.lnDelete.Click += (s, e) =>
                {
                    try
                    {
                        LnDeleteClick(position);
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("lnDelete at cart_adapter");
                        Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                };

                vh.DiscountItem.Text = Application.Context.GetString(Resource.String.discount) + " (" + 
                     CURRENCYSYMBOLS +  Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Discount) + ")";

                if (tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.FmlDiscountRow != "" 
                    && tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.FmlDiscountRow != null
                    && tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Discount > 0)
                {
                    //vh.DiscountRowImage.Visibility = ViewStates.Visible;
                    vh.Discount.Visibility = ViewStates.Visible;
                }
                else
                {
                    //vh.DiscountRowImage.Visibility = ViewStates.Gone;
                    vh.Discount.Visibility = ViewStates.Gone;
                }

                #region Edit Option
                ListOption list = new ListOption(tranWithDetails.tranDetailItemWithToppings[position].tranDetailItemToppings);
                Cart_Adapter_Option cart_Adapter_Option = new Cart_Adapter_Option(list);
                scvlistTopping.SetAdapter(cart_Adapter_Option);
                scvlistTopping.HasFixedSize = true;
                scvlistTopping.SetItemViewCacheSize(20);
                if (cart_Adapter_Option.ItemCount <= 0)
                {
                    vh.OptionList.Visibility = ViewStates.Gone;
                }
                else
                {
                    vh.OptionList.Visibility = ViewStates.Visible;
                }

                vh.lnEditItem.Visibility = ViewStates.Gone;

                if ((POS_Fragment_Cart.DetailNoOld > 0 & POS_Fragment_Cart.openlstTopping) || (POS_Dialog_Scan.DetailNoOld > 0 & POS_Dialog_Scan.openlstTopping))
                {
                    //ถ้ามี option ตรงนี้จะทำให้ตัวที่มีออฟชั่น แสดง edit option เพี้ยน เพราะ  SysItemID = null 
                    if (POS_Fragment_Cart.DetailNoOld == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo  )
                    {
                        vh.lnEditItem.Visibility = ViewStates.Gone;
                        POS_Fragment_Cart.openlstTopping = false;
                    }

                    if (POS_Dialog_Scan.scan != null)
                    {
                        if (POS_Dialog_Scan.DetailNoOld == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo)
                        {
                            vh.lnEditItem.Visibility = ViewStates.Gone;
                            POS_Dialog_Scan.openlstTopping = false;
                        }
                    }

                    return;
                }
                

                // LnEditItem ของ POS_Fragment_Cart
                if (POS_Fragment_Cart.DetailNo == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo && POS_Fragment_Cart.DetailNo != 0)
                {
                    if (POS_Fragment_Cart.DetailNo == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo)
                    {
                        vh.lnEditItem.Visibility = ViewStates.Visible;
                        vh.ItemView.RequestFocus();
                        POS_Fragment_Cart.DetailNoOld = POS_Fragment_Cart.DetailNo;
                        POS_Fragment_Cart.openlstTopping = true;
                    }
                    else
                    {
                        vh.lnEditItem.Visibility = ViewStates.Gone;
                        POS_Fragment_Cart.openlstTopping = false;
                    }
                }                

                //LnEditItem ของ POS_Dialog_Scan
                if (POS_Dialog_Scan.detailNoClickOption == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo && POS_Dialog_Scan.detailNoClickOption != 0)
                {
                    if (POS_Dialog_Scan.detailNoClickOption == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo)
                    {
                        vh.lnEditItem.Visibility = ViewStates.Visible;
                        vh.ItemView.RequestFocus();
                        POS_Dialog_Scan.DetailNoOld = POS_Dialog_Scan.detailNoClickOption;
                        POS_Dialog_Scan.openlstTopping = true;
                    }
                    else
                    {
                        vh.lnEditItem.Visibility = ViewStates.Gone;
                        POS_Dialog_Scan.openlstTopping = false;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBindViewHolder at cart_adapter");
                Toast.MakeText(Application.Context,ex.Message,ToastLength.Short).Show();
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private void LnPriceClick(TranDetailItemNew item, long? detailNo)
        {
            Cart_Dialog_ChangePrice.SetSysItem(item, tranWithDetails);
            var fragment = new Cart_Dialog_ChangePrice();
            fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Cart_Dialog_ChangePrice));
        }

        private void LnQuantityClick(long? itemId, decimal quantity)
        {
            DataCashing.flagEditQuantity = true;
            var fragment = new POS_Dialog_Quantity();
            fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(POS_Dialog_Quantity));
            POS_Dialog_Quantity.SetBackQuantity((Int32)quantity);
        }

        private void LnOptionClick(TranWithDetailsLocal tranWithDetails,long? sysItemID,TranDetailItemNew tranDetailItem,Item item)
        {
            DataCashing.flagEditOptionSize = true;
            DataCashing.flagEditOptionNote = true;
            DataCashing.flagEditOptionExtraTopping = true;

            POS_Dialog_Option.SetSysItem(sysItemID);
            POS_Dialog_Option.SetItemDetail(tranDetailItem);
            POS_Dialog_Option.SetDataItem(item);

            var fragment = new POS_Dialog_Option();
            fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(POS_Dialog_Option));
        }

        //ส่วนลดต่อสินค้า
        private void LnDiscountClick(TranDetailItemNew tranDetail)
        {
            if (tranDetail != null)
            {
                POS_Dialog_Discount.SetTrtanDetailItem(tranDetail);
                POS_Dialog_Discount.CartDiscount = false;
                var fragment = new POS_Dialog_Discount();
                fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(POS_Dialog_Discount));
            }
        }

        private void LnDeleteClick(int positionClick)
        {
            try
            {
                tranWithDetails = BLTrans.RemoveDetailItem(tranWithDetails, tranWithDetails.tranDetailItemWithToppings[positionClick]);
                DataCashing.ModifyTranOrder = true;
                tranWithDetails = BLTrans.Caltran(tranWithDetails);
                CalCartDiscount();
                MainActivity.tranWithDetails = tranWithDetails;

                POS_Fragment_Cart.DetailNo = 0;
                POS_Fragment_Cart.DetailNoOld = 0;
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();                

                if (POS_Dialog_Scan.scan != null)
                {
                    POS_Dialog_Scan.detailNoClickOption = 0;
                    POS_Dialog_Scan.scan.OnResume();                    
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnDeleteClick at cart_adapter");
                Toast.MakeText(Application.Context , ex.Message,ToastLength.Short).Show();      
            }
        }

        private void CalCartDiscount()
        {
            //check discount Tran  ไม่ให้เกินราคาสินค้า 
            decimal discountCart = 0;
            var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
            double total = Convert.ToDouble(tranWithDetails.tran.GrandTotal);
            string newdiscount = "";
            if (tranTradDiscount != null)
            {
                var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                if (check == -1)
                {
                    Decimal.TryParse(tranTradDiscount.FmlDiscount, out discountCart);

                    if (total < 0 && tranWithDetails.tran.GrandTotal < discountCart)
                    {
                        tranWithDetails = BLTrans.RemoveDiscount(tranWithDetails, "MD");
                        newdiscount = tranWithDetails.tran.GrandTotal.ToString();
                        TranTradDiscount dis = new TranTradDiscount()
                        {
                            MerchantID = tranWithDetails.tran.MerchantID,
                            SysBranchID = tranWithDetails.tran.SysBranchID,
                            TranNo = tranWithDetails.tran.TranNo,
                            PriorityNo = 0,
                            FOnTop = 0,
                            DiscountType = "MD",
                            FmlDiscount = newdiscount
                        };
                        tranWithDetails = BLTrans.AddDiscount(tranWithDetails, dis);
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            context = parent.Context;
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cart_adapter_item, parent, false);
            ListViewItemHolder vh = new ListViewItemHolder(itemView, OnClick);

            scvlistTopping = itemView.FindViewById<RecyclerView>(Resource.Id.scvlistTopping);
            var layoutManager = new LinearLayoutManager(context,1 ,false);
            scvlistTopping.SetLayoutManager(layoutManager);

            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewItemHolder : RecyclerView.ViewHolder
    {
        public ImageView ColorItemView { get; set; }
        public ImageView PicItemView { get; set; }
        public ImageView PicItemViewRow { get; set; }
        public TextView ItemNameView { get; set; }
        public TextView ItemName { get; set; }
        public TextView CategoryItem { get; set; }
        public TextView ItemPrice { get; set; }
        public TextView PriceTotal { get; set; }
        public TextView PriceBeforeDis { get; set; }
        public ImageView Alert { get; set; }
        public TextView CountItem { get; set; }
        public LinearLayout lnEditItem { get; set; }
        public FrameLayout frameAdd { get; set; }
        public ImageView imgShowAnime { get; set; }
        public LinearLayout lnQuantity { get; set; }
        public LinearLayout lnOption { get; set; }
        public LinearLayout lnChangePrice { get; set; }
        public LinearLayout lnDiscount { get; set; }
        public LinearLayout lnDelete { get; set; }
        public RelativeLayout OptionList { get; set; }
        public LinearLayout lnFavorite { get; set; }
        public ImageButton Favorite { get; set; }
        public ImageView DiscountRowImage { get; set; }
        public RelativeLayout lnComment { get; set; }
        public LinearLayout Discount { get; set; }
        public TextView Comment { get; set; }
        public TextView DiscountItem { get; set; }
        public ListViewItemHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ColorItemView = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            PicItemView = itemview.FindViewById<ImageView>(Resource.Id.imagePicItem);
            //PicItemViewRow = itemview.FindViewById<ImageView>(Resource.Id.imageViewcolorItem);
            ItemNameView = itemview.FindViewById<TextView>(Resource.Id.textViewItemName);
            ItemName = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            CategoryItem = itemview.FindViewById<TextView>(Resource.Id.txtCategoryItem);
            ItemPrice = itemview.FindViewById<TextView>(Resource.Id.txtItemPrice);
            PriceTotal = itemview.FindViewById<TextView>(Resource.Id.txtPriceTotal);
            PriceBeforeDis = itemview.FindViewById<TextView>(Resource.Id.txtPriceBeforeDis);
            Alert = itemview.FindViewById<ImageView>(Resource.Id.imagealert);
            CountItem = itemview.FindViewById<TextView>(Resource.Id.textCountITem);
            frameAdd = itemview.FindViewById<FrameLayout>(Resource.Id.frameAdd);
            imgShowAnime = itemview.FindViewById<ImageView>(Resource.Id.imgShowAnime);

            lnEditItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnEditItem);
            lnQuantity = itemview.FindViewById<LinearLayout>(Resource.Id.lnQuantity);
            lnOption = itemview.FindViewById<LinearLayout>(Resource.Id.lnOption);
            lnChangePrice = itemview.FindViewById<LinearLayout>(Resource.Id.lnPrice);
            lnDiscount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscount);
            lnDelete = itemview.FindViewById<LinearLayout>(Resource.Id.lnDelete);
            OptionList = itemview.FindViewById<RelativeLayout>(Resource.Id.lnOptionlist);
            Discount = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscountItem);
            DiscountItem = itemview.FindViewById<TextView>(Resource.Id.textItemDiscount);
            DiscountRowImage = itemview.FindViewById<ImageView>(Resource.Id.imageDiscountRow);

            lnComment = itemview.FindViewById<RelativeLayout>(Resource.Id.lnCommentItem);
            Comment = itemview.FindViewById<TextView>(Resource.Id.textCommentItem);

            itemview.Click += (sender, e) => listener(base.Position);
        }

        public void Select()
        {
            Favorite.SetBackgroundResource(Resource.Mipmap.Fav);
        }

        public void NotSelect()
        {
            Favorite.SetBackgroundResource(Resource.Mipmap.Unfav);
        }
    }

    public class ListOption
    {
        public List<TranDetailItemTopping> detailItemToppings;
        static List<TranDetailItemTopping> builitem;
        public ListOption(List<TranDetailItemTopping> detailItemToppings)
        {
            builitem = detailItemToppings;
            this.detailItemToppings = builitem;
        }
        public int Count
        {
            get
            {
                return detailItemToppings == null ? 0 : detailItemToppings.Count;
            }
        }
        public TranDetailItemTopping this[int i]
        {
            get { return detailItemToppings == null ? null : detailItemToppings[i]; }
        }
    }


}