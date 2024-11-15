using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid.Adapter
{
    public class Cart_Adapter_Item : RecyclerView.Adapter
    {
        //private List<Item> ilistItem;
        private TranWithDetailsLocal tranWithDetails;
        public event EventHandler<int> ItemClick;
        long? detailNo;
        private static Context context;
        private static RecyclerView recyclerview_listTopping;

        public Cart_Adapter_Item(TranWithDetailsLocal tran)
        {
            tranWithDetails = tran;
        }

        public override int ItemCount
        {
            get { return tranWithDetails == null ? 0 : tranWithDetails.tranDetailItemWithToppings.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewItemHolder vh = holder as ListViewItemHolder;
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;

                vh.lnStatusD.Visibility = ViewStates.Gone;

                string sizename = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SizeName;
                vh.ItemName.Text = string.IsNullOrEmpty(sizename) || sizename == "Default Size" ?
                    tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemName?.ToString() :
                    $"{tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemName?.ToString()} {sizename}";


                #region แก้ไขจำนวนสินค้า
                if (DataCashing.flagEditQuantity)
                {
                    int updateQuantity = CartActivity.CurrentActivity ?
                tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == DataCashing.EditItemID && x.tranDetailItem.DetailNo == CartActivity.DetailNo) :
                tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.SysItemID == DataCashing.EditItemID && x.tranDetailItem.DetailNo == CartScanActivity.detailNoClickOption);

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
                        if (CartActivity.CurrentActivity)
                        {
                            CartActivity.cart.textTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
                            CartActivity.cart.textNumVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                            CartActivity.cart.textNumService.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                        }
                        else
                        {
                            CartScanActivity.scan.textTotal.Text = Utils.DisplayDecimal(tranWithDetails.tran.GrandTotal);
                            CartScanActivity.scan.textNumVat.Text = Utils.DisplayDecimal(tranWithDetails.tran.TotalVat);
                            CartScanActivity.scan.textNumService.Text = Utils.DisplayDecimal(tranWithDetails.tran.ServiceCharge);
                        }
                    }
                    else
                    {
                        return;
                    }


                    if (CartActivity.CurrentActivity)
                    {
                        CartActivity.cart.flagEditQuantity = false;
                    }
                    DataCashing.flagEditQuantity = false;
                }
                else
                {
                    vh.CountItem.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Quantity.ToString("#,###");
                }
                #endregion

                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS ?? "฿";
                decimal price = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Price;
                decimal ItemPrice = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemPrice;

                vh.ItemPrice.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(ItemPrice);
                if (price != ItemPrice)
                {
                    vh.ItemPrice.Text = "  " + CURRENCYSYMBOLS + Utils.DisplayDecimal(price);
                    vh.PriceBeforeDis.Visibility = ViewStates.Visible;
                    vh.PriceBeforeDis.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(ItemPrice);
                    vh.PriceBeforeDis.PaintFlags = Android.Graphics.PaintFlags.StrikeThruText;
                }
                else
                {
                    vh.ItemPrice.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(ItemPrice);
                    vh.PriceBeforeDis.Visibility = ViewStates.Gone;
                }


                decimal totalPrice = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID != null ?
            tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Amount :
            tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Amount;
                vh.PriceTotal.Text = Utils.DisplayDecimal(totalPrice);


                var comment = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Comments;
                vh.lnComment.Visibility = string.IsNullOrEmpty(comment) ? ViewStates.Gone : ViewStates.Visible;
                vh.Comment.Text = comment;


                #region saleitemType Dummy
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
                        var itemId = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID;
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
                        var itemId = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID;
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
                        var itemId = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID;
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
                     CURRENCYSYMBOLS + Utils.DisplayDecimal(tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Discount) + ")";

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
                recyclerview_listTopping.SetAdapter(cart_Adapter_Option);
                recyclerview_listTopping.HasFixedSize = true;
                recyclerview_listTopping.SetItemViewCacheSize(20);
                if (cart_Adapter_Option.ItemCount <= 0)
                {
                    vh.OptionList.Visibility = ViewStates.Gone;
                }
                else
                {
                    vh.OptionList.Visibility = ViewStates.Visible;
                }

                #region เคสดึงออเดอร์ แล้วสินค้ามีการเปลี่ยนแปลง Cart
                //1. เคสสินค้าถูกลบ 
                //ตรวจสอบข้อมูลก่อนจะบันทึกว่ามีสินค้าที่ไม่มีที่เครื่องไหม ถ้ามีให้ remove สินค้านั้น แล้ว calTran ใหม่ 
                if (CartActivity.lstSysItemIdStatusD.Count > 0)
                {
                    var Sysitem = CartActivity.lstSysItemIdStatusD.FirstOrDefault(x => x.Contains(tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID.ToString()));
                    if (!string.IsNullOrEmpty(Sysitem))
                    {
                        vh.lnStatusD.Visibility = ViewStates.Visible;
                        vh.lnEditItem.Visibility = ViewStates.Gone;
                        vh.btnDeleteItemD.Click += (s, e) =>
                        {
                            tranWithDetails = Utils.RemoveItem(tranWithDetails, tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID);
                            DataCashing.ModifyTranOrder = true;
                            int index = CartActivity.lstSysItemIdStatusD.IndexOf(Sysitem);
                            if (index != -1)
                            {
                                CartActivity.lstSysItemIdStatusD.RemoveAt(index);
                            }
                            CartActivity.SetTranDetail(tranWithDetails);
                            CartActivity.cart.SetITemInCart();
                        };
                    }
                    else
                    {
                        vh.lnStatusD.Visibility = ViewStates.Gone;
                    }
                }
                #endregion

                #region เคสดึงออเดอร์ แล้วสินค้ามีการเปลี่ยนแปลง CartScan
                //1. เคสสินค้าถูกลบ 
                //ตรวจสอบข้อมูลก่อนจะบันทึกว่ามีสินค้าที่ไม่มีที่เครื่องไหม ถ้ามีให้ remove สินค้านั้น แล้ว calTran ใหม่ 
                if (CartScanActivity.lstSysItemIdStatusD.Count > 0)
                {
                    var Sysitem = CartScanActivity.lstSysItemIdStatusD.FirstOrDefault(x => x.Contains(tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID.ToString()));
                    if (!string.IsNullOrEmpty(Sysitem))
                    {
                        vh.lnStatusD.Visibility = ViewStates.Visible;
                        vh.lnEditItem.Visibility = ViewStates.Gone;
                        vh.btnDeleteItemD.Click += (s, e) =>
                        {
                            tranWithDetails = Utils.RemoveItem(tranWithDetails, tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SysItemID);
                            DataCashing.ModifyTranOrder = true;
                            int index = CartScanActivity.lstSysItemIdStatusD.IndexOf(Sysitem);
                            if (index != -1)
                            {
                                CartScanActivity.lstSysItemIdStatusD.RemoveAt(index);
                            }
                            CartScanActivity.SetTranDetail(tranWithDetails);
                            CartScanActivity.scan.SetITemInCart();
                        };
                    }
                    else
                    {
                        vh.lnStatusD.Visibility = ViewStates.Gone;
                    }
                }
                #endregion

                vh.lnEditItem.Visibility = ViewStates.Gone;

                #region เปิด/ปิด LnEditItem ที่ Cart
                //ปิด LnEditItem ที่ Cart
                if (CartActivity.DetailNoOld == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo & CartActivity.DetailNoOld > 0 & CartActivity.openlstTopping)
                {
                    vh.lnEditItem.Visibility = ViewStates.Gone;
                    CartActivity.openlstTopping = false;
                    return;
                }

                //เปิด LnEditItem ที่ Cart
                // LnEditItem ของ CartActivity
                if (CartActivity.DetailNo == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo && CartActivity.DetailNo != 0)
                {
                    if (CartActivity.DetailNo == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo)
                    {
                        vh.lnEditItem.Visibility = ViewStates.Visible;
                        vh.ItemView.RequestFocus();
                        CartActivity.DetailNoOld = CartActivity.DetailNo;
                        CartActivity.openlstTopping = true;
                    }
                    else
                    {
                        vh.lnEditItem.Visibility = ViewStates.Gone;
                        CartActivity.openlstTopping = false;
                    }
                }
                #endregion                

                #region เปิด/ปิด LnEditItem ที่ CartScanActivity
                //ปิด LnEditItem ที่ CartScanActivity
                if (CartScanActivity.DetailNoOld == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo & CartScanActivity.DetailNoOld > 0 & CartScanActivity.openlstTopping)
                {
                    vh.lnEditItem.Visibility = ViewStates.Gone;
                    CartScanActivity.openlstTopping = false;
                    return;
                }

                //เปิด LnEditItem ที่ CartScanActivity
                //LnEditItem ของ CartScanActivity
                if (CartScanActivity.detailNoClickOption == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo && CartScanActivity.detailNoClickOption != 0)
                {
                    if (CartScanActivity.detailNoClickOption == tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.DetailNo)
                    {
                        vh.lnEditItem.Visibility = ViewStates.Visible;
                        vh.ItemView.RequestFocus();
                        CartScanActivity.DetailNoOld = CartScanActivity.detailNoClickOption;
                        CartScanActivity.openlstTopping = true;
                    }
                    else
                    {
                        vh.lnEditItem.Visibility = ViewStates.Gone;
                        CartScanActivity.openlstTopping = false;
                    }
                }
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBindViewHolder at cart_adapter");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private void LnPriceClick(TranDetailItemNew item, long? detailNo)
        {
            if (CartActivity.CurrentActivity)
            {
                CartActivity.cart.LnPriceClick(item, detailNo);
            }
            else
            {
                CartScanActivity.scan.LnPriceClick(item, detailNo);
            }
        }

        private void LnQuantityClick(long? itemId, decimal quantity)
        {
            if (CartActivity.CurrentActivity)
            {
                CartActivity.cart.LnQuantityClick(itemId, quantity);
            }
            else
            {
                CartScanActivity.scan.LnQuantityClick(itemId, quantity);
            }
        }

        private void LnOptionClick(TranWithDetailsLocal tranWithDetails, long? sysItemID, TranDetailItemNew tranDetailItem, Item item)
        {
            if (CartActivity.CurrentActivity)
            {
                CartActivity.cart.LnOptionClick(tranWithDetails, sysItemID, tranDetailItem, item);
            }
            else
            {
                CartScanActivity.scan.LnOptionClick(tranWithDetails, sysItemID, tranDetailItem, item);
            }
        }

        private void LnDiscountClick(TranDetailItemNew tranDetail)
        {
            if (CartActivity.CurrentActivity)
            {
                CartActivity.cart.LnDiscountClick(tranDetail);
            }
            else
            {
                CartScanActivity.scan.LnDiscountClick(tranDetail);
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

                if (CartActivity.CurrentActivity)
                {
                    CartActivity.cart.SetITemInCart();
                }
                else
                {
                    //CartScanActivity.scan.SelectItemtoCart();
                    CartScanActivity.scan.SetITemInCart();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnDeleteClick at cart_adapter");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
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

            recyclerview_listTopping = itemView.FindViewById<RecyclerView>(Resource.Id.recyclerview_listTopping);
            var layoutManager = new LinearLayoutManager(context, 1, false);
            recyclerview_listTopping.SetLayoutManager(layoutManager);

            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

        private void LnbtnDeleteItemStatusDClick(int positionClick)
        {
            try
            {
                if (CartActivity.CurrentActivity)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnDeleteClick at cart_adapter");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

    }
}