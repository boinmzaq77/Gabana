using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Setting
{
    public class Setting_Adapter_GiftVoucher : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        public ListGiftVoucher listvoucher;
        public string positionClick;
        public Setting_Adapter_GiftVoucher(ListGiftVoucher l)
        {
            listvoucher = l;
        }
        public override int ItemCount
        {
            get { return listvoucher == null ? 0 : listvoucher.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewGiftVoucherHolder vh = holder as ListViewGiftVoucherHolder;
                vh.GiftVoucherID.Text = listvoucher[position].GiftVoucherCode;
                vh.GiftVoucherName.Text = listvoucher[position].GiftVoucherName;
                if (listvoucher[position].FmlAmount.Contains("%"))
                {
                    var value = Convert.ToDecimal(listvoucher[position].FmlAmount.Replace("%", ""));
                    var dec = DataCashingAll.setmerchantConfig?.DECIMAL_POINT_DISPLAY;
                    if (dec == "4")
                    {
                        vh.Discount.Text = value.ToString("#,###.####") + "%";
                    }
                    else
                    {
                        vh.Discount.Text = value.ToString("#,###.##") + "%";
                    }
                }
                else
                {
                    vh.Discount.Text = Utils.DisplayDecimal(Convert.ToDecimal(listvoucher[position].FmlAmount));
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.setting_adapter_giftvoucher, parent, false);
            ListViewGiftVoucherHolder vh = new ListViewGiftVoucherHolder(itemView, OnClick);
            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewGiftVoucherHolder : RecyclerView.ViewHolder
    {
        public TextView Discount { get; set; }
        public TextView GiftVoucherName { get; set; }
        public TextView GiftVoucherID { get; set; }
        public ImageButton Check { get; set; }

        public ListViewGiftVoucherHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Discount = itemview.FindViewById<TextView>(Resource.Id.textDiscount);
            GiftVoucherName = itemview.FindViewById<TextView>(Resource.Id.textGiftName);
            GiftVoucherID = itemview.FindViewById<TextView>(Resource.Id.textGiftID);
            Check = itemview.FindViewById<ImageButton>(Resource.Id.btnCheck);



            itemview.Click += (sender, e) => listener(base.Position);


        }
    }
}