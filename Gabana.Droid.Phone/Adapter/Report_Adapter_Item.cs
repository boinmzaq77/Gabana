using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_Item : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<Item> listitem;
        private string BestSellBy;
        public string positionClick;
        List<Gabana3.JAM.Report.SummaryItemModel> summaryItems;
        private string CURRENCYSYMBOLS;

        public Report_Adapter_Item(List<Gabana3.JAM.Report.SummaryItemModel> s, List<Item> l, string b)
        {
            summaryItems = s;
            listitem = l;
            BestSellBy = b;
        }

        public override int ItemCount
        {
            get { return summaryItems == null ? 0 : summaryItems.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewBestSaleHolder vh = holder as ListViewBestSaleHolder;
            var items = listitem.Where(x => x.ItemName.Replace("\u200b","") == summaryItems[position].ItemName.Replace("\u200b", "")).ToList();
            Item item = new Item();
            item = items.OrderBy(x => x.LastDateModified).LastOrDefault();
            if (item == null)
            {
                var color = Android.Graphics.Color.ParseColor("#0095DA");
                vh.ImageItem.SetBackgroundColor(color);
                vh.ShortName.Visibility = ViewStates.Visible;
            }
            else
            {
                var cloudpath = item.PicturePath == null ? string.Empty : item.PicturePath;
                var localpath = item.ThumbnailLocalPath == null ? string.Empty : item.ThumbnailLocalPath;

                if (string.IsNullOrEmpty(localpath))
                {
                    if (string.IsNullOrEmpty(cloudpath))
                    {
                        //defalut
                        string conColor = Utils.SetBackground(Convert.ToInt32(item.Colors));
                        var color = Android.Graphics.Color.ParseColor(conColor);
                        vh.ShortName.SetBackgroundColor(color);
                        vh.ShortName.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        //cloud
                        Utils.SetImage(vh.ImageItem, cloudpath);
                        vh.ShortName.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    //local
                    Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                    vh.ImageItem.SetImageURI(uri);
                    vh.ShortName.Visibility = ViewStates.Gone;
                }
            }

            vh.ShortName.Text = summaryItems[position].ItemName?.ToString();
            vh.Name.Text = summaryItems[position].ItemName?.ToString();

            switch (BestSellBy)
            {
                case "Sell":
                    vh.TotalSale.Text = CURRENCYSYMBOLS + " " + summaryItems[position].SumTotalAmount.ToString("##,###.00");
                    break;
                case "Unit":
                    vh.TotalSale.Text = summaryItems[position].SumQuantity.ToString("#,###");
                    break;
                default:
                    break;
            }


        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_item, parent, false);
            ListViewBestSaleHolder vh = new ListViewBestSaleHolder(itemView, OnClick);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}