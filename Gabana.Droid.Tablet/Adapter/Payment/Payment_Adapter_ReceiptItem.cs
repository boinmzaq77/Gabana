using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Pos;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Payment
{
    internal class Payment_Adapter_ReceiptItem : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        private static RecyclerView rcvlistTopping;
        private TranWithDetailsLocal tranWithDetails;

        public Payment_Adapter_ReceiptItem(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
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
                rcvlistTopping.SetAdapter(cart_Adapter_Option);
                rcvlistTopping.HasFixedSize = true;
                rcvlistTopping.SetItemViewCacheSize(20);
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.payment_adapter_recieptitem, parent, false);
            ListViewReceiptHolder vh = new ListViewReceiptHolder(itemView, OnClick);
            rcvlistTopping = itemView.FindViewById<RecyclerView>(Resource.Id.rcvlistTopping);
            var layoutManager = new LinearLayoutManager(parent.Context, 1, false);
            rcvlistTopping.SetLayoutManager(layoutManager);

            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }

    public class ListViewReceiptHolder : RecyclerView.ViewHolder
    {
        public TextView textCountITem { get; set; }
        public TextView txtNameItem { get; set; }
        public TextView txtPrice { get; set; }
        public TextView txtDiscount { get; set; }
        public LinearLayout lnOptionlist { get; set; }
        public LinearLayout lnDiscountItem { get; set; }
        public LinearLayout lnCommetnItem { get; set; }
        public TextView CommentItem { get; set; }
        public ListViewReceiptHolder(View itemview, Action<int> listener) : base(itemview)
        {
            textCountITem = itemview.FindViewById<TextView>(Resource.Id.textCountITem);
            txtNameItem = itemview.FindViewById<TextView>(Resource.Id.txtNameItem);
            txtPrice = itemview.FindViewById<TextView>(Resource.Id.txtPrice);
            txtDiscount = itemview.FindViewById<TextView>(Resource.Id.textItemDiscount);
            lnOptionlist = itemview.FindViewById<LinearLayout>(Resource.Id.lnOptionlist);
            lnDiscountItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnDiscountItem);
            lnCommetnItem = itemview.FindViewById<LinearLayout>(Resource.Id.lnCommentItem);
            CommentItem = itemview.FindViewById<TextView>(Resource.Id.txtCommentItem);

            itemview.Click += (sender, e) => listener(base.Position);
        }
    }

}