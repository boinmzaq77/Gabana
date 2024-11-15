using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ShareSource;
using System;

namespace Gabana.Droid.Adapter
{
    public class Receipt_Adapter_Item : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private static RecyclerView recyclerview_listTopping;
        private TranWithDetailsLocal tranWithDetails;

        public Receipt_Adapter_Item(TranWithDetailsLocal tran)
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

                ListViewReceiptHolder vh = holder as ListViewReceiptHolder;
                vh.textCountITem.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Quantity.ToString("#,###");
                vh.txtNameItem.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemName?.ToString();

                string sizename = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.SizeName;
                if (string.IsNullOrEmpty(sizename) || sizename == "Default Size")
                {
                    vh.txtNameItem.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemName?.ToString();
                }
                else
                {
                    vh.txtNameItem.Text = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.ItemName?.ToString() + " " + sizename;
                }

                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                decimal price = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Price;
                decimal ItemPrice = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Amount;

                vh.txtPrice.Text = Utils.DisplayDecimal(ItemPrice);

                //var tranDetailItemToppings = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItemToppings;
                ListOption list = new ListOption(tranWithDetails.tranDetailItemWithToppings[position].tranDetailItemToppings);
                Cart_Adapter_Option cart_Adapter_Option = new Cart_Adapter_Option(list);
                recyclerview_listTopping.SetAdapter(cart_Adapter_Option);
                recyclerview_listTopping.HasFixedSize = true;
                recyclerview_listTopping.SetItemViewCacheSize(20);
                if (cart_Adapter_Option.ItemCount <= 0)
                {
                    vh.lnOptionlist.Visibility = ViewStates.Gone;
                }
                else
                {
                    vh.lnOptionlist.Visibility = ViewStates.Visible;
                }

                if (tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Discount == 0)
                {
                    vh.lnDiscountItem.Visibility = ViewStates.Gone;
                }
                else
                {
                    vh.lnDiscountItem.Visibility = ViewStates.Visible;
                    vh.txtDiscount.Text = Application.Context.GetString(Resource.String.receipt_activity_discount) + " (" + Utils.DisplayDecimal((tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Discount)) + ")";
                }
                //comment item
                var comment = tranWithDetails.tranDetailItemWithToppings[position].tranDetailItem.Comments;
                if (String.IsNullOrEmpty(comment))
                {
                    vh.lnCommetnItem.Visibility = ViewStates.Gone;
                    vh.CommentItem.Text = string.Empty;
                }
                else
                {
                    vh.CommentItem.Text = comment;
                    vh.lnCommetnItem.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.receipt_adapter_item, parent, false);
            ListViewReceiptHolder vh = new ListViewReceiptHolder(itemView, OnClick);

            recyclerview_listTopping = itemView.FindViewById<RecyclerView>(Resource.Id.recyclerview_listTopping);
            var layoutManager = new LinearLayoutManager(parent.Context, 1, false);
            recyclerview_listTopping.SetLayoutManager(layoutManager);

            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}