using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.ORM.MerchantDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Option
{
    internal class Option_Adapter_Size : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListItemExSize lssize;
        public string positionClick;

        public Option_Adapter_Size(ListItemExSize l)
        {
            lssize = l;
        }
        public override int ItemCount
        {
            get { return lssize == null ? 0 : lssize.Count; }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewOptionSizeHolder vh = holder as ListViewOptionSizeHolder;
                vh.SizeName.Text = lssize[position].ExSizeName?.ToString();
                vh.Price.Text = Utils.DisplayDecimal(lssize[position].Price);
                vh.SelectSize.SetBackgroundResource(Resource.Mipmap.RadioBlank);
                vh.ItemView.Focusable = false;
                vh.ItemView.FocusableInTouchMode = false;
                vh.ItemView.Clickable = true;


                //Load Show check Default Size
                //Update POSItem POSItem == null => เพิ่ม option ที่ตะกร้า

                if (POS_Dialog_Option.POSDataItem != null) //Insert to cart
                {
                    if (POS_Dialog_Option.flagLoadSize == false)
                    {
                        if ("Default Size" == lssize[position].ExSizeName)
                        {
                            vh.SelectSize.SetBackgroundResource(Resource.Mipmap.RadioCheck);
                        }
                        POS_Dialog_Option.flagLoadSize = true;
                    }
                }

                //Update Cart
                //click Select Size
                if (POS_Dialog_Option.SelectSize.ExSizeNo != 0)
                {
                    if (POS_Dialog_Option.ExSizeNo == lssize[position].ExSizeNo)
                    {
                        vh.SelectSize.SetBackgroundResource(Resource.Mipmap.RadioCheck);
                    }
                    else
                    {
                        vh.SelectSize.SetBackgroundResource(Resource.Mipmap.RadioBlank);
                    }
                }

                //Edit Size
                //click option ที่ cart แสดงค่าที่เลือกครั้งก่อน
                if (DataCashing.flagEditOptionSize)
                {
                    //Find Sizename
                    Model.TranDetailItemWithTopping Detail = new Model.TranDetailItemWithTopping();
                    if (POS_Dialog_Scan.scan  != null)
                    {
                        Detail = POS_Dialog_Option.tranWithDetails.tranDetailItemWithToppings.Where(x => !string.IsNullOrEmpty(x.tranDetailItem.SizeName) && x.tranDetailItem.DetailNo == POS_Dialog_Scan.detailNoClickOption).FirstOrDefault();
                    }
                    else
                    {
                        Detail = POS_Dialog_Option.tranWithDetails.tranDetailItemWithToppings.Where(x => !string.IsNullOrEmpty(x.tranDetailItem.SizeName) && x.tranDetailItem.DetailNo == POS_Fragment_Cart.DetailNo).FirstOrDefault();
                    }

                    if (Detail == null)
                    {
                        if ("Default Size" == lssize[position].ExSizeName)
                        {
                            vh.SelectSize.SetBackgroundResource(Resource.Mipmap.RadioCheck);
                            DataCashing.flagEditOptionSize = false;
                        }
                    }
                    else
                    {
                        if (Detail.tranDetailItem.SizeName == lssize[position].ExSizeName)
                        {
                            vh.SelectSize.SetBackgroundResource(Resource.Mipmap.RadioCheck);
                            DataCashing.flagEditOptionSize = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.option_adapter_size, parent, false);
            ListViewOptionSizeHolder vh = new ListViewOptionSizeHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
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
    public class ListViewOptionSizeHolder : RecyclerView.ViewHolder
    {
        public ImageButton SelectSize { get; set; }
        public TextView SizeName { get; set; }
        public TextView Price { get; set; }

        public ImageButton SelectCustomer { get; set; }
        public ListViewOptionSizeHolder(View itemview, Action<int> listener) : base(itemview)
        {
            SelectSize = itemview.FindViewById<ImageButton>(Resource.Id.btnSelectSize);
            SizeName = itemview.FindViewById<TextView>(Resource.Id.textSizeName);
            Price = itemview.FindViewById<TextView>(Resource.Id.textSizePrice);


            itemview.Click += (sender, e) => listener(base.Position);

        }
    }

}