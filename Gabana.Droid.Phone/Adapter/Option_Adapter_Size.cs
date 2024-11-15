using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using System;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Option_Adapter_Size : RecyclerView.Adapter
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

                if (OptionActivity.POSDataItem != null) //Insert to cart
                {
                    if (OptionActivity.flagLoadSize == false)
                    {
                        if ("Default Size" == lssize[position].ExSizeName)
                        {
                            vh.SelectSize.SetBackgroundResource(Resource.Mipmap.RadioCheck);
                        }
                        OptionActivity.flagLoadSize = true;
                    }
                }

                //Update Cart
                //click Select Size
                if (OptionActivity.SelectSize.ExSizeNo != 0)
                {
                    if (OptionActivity.ExSizeNo == lssize[position].ExSizeNo)
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

                    if (CartActivity.CurrentActivity)
                    {
                        Detail = OptionActivity.tranWithDetails.tranDetailItemWithToppings.Where(x => !string.IsNullOrEmpty(x.tranDetailItem.SizeName) && x.tranDetailItem.DetailNo == CartActivity.DetailNo).FirstOrDefault();
                    }
                    else
                    {
                        Detail = OptionActivity.tranWithDetails.tranDetailItemWithToppings.Where(x => !string.IsNullOrEmpty(x.tranDetailItem.SizeName) && x.tranDetailItem.DetailNo == CartScanActivity.detailNoClickOption).FirstOrDefault();
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
}