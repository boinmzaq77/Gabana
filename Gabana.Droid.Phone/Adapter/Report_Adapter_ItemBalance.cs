using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_ItemBalance : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<Item> listitem;
        public string positionClick;
        List<ItemOnBranch> itemOnBranch;

        public Report_Adapter_ItemBalance(List<ItemOnBranch> s, List<Item> l)
        {
            itemOnBranch = s;
            listitem = l;
        }

        public override int ItemCount
        {
            get { return itemOnBranch == null ? 0 : itemOnBranch.Count; }
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewBestSaleHolder vh = holder as ListViewBestSaleHolder;

            var item = listitem.Where(x => x.SysItemID == itemOnBranch[position].SysItemID).FirstOrDefault();
            if (item == null)
            {
                var color = Android.Graphics.Color.ParseColor("#0095DA");
                vh.ImageItem.SetBackgroundColor(color);
                vh.ShortName.Visibility = ViewStates.Visible;
            }
            else
            {
                var paths = item.PicturePath;
                if (!string.IsNullOrEmpty(paths))
                {
                    if (await GabanaAPI.CheckSpeedConnection())
                    {
                        Utils.SetImage(vh.ImageItem, paths);
                    }
                    else
                    {
                        var folder = DataCashingAll.PathThumnailFolderImage;
                        var filesList = Directory.GetFiles(folder);

                        string pathPicture = item.ThumbnailLocalPath;
                        string path = Utils.SplitPath(pathPicture);
                        string pathImage = folder + path;

                        foreach (var file in filesList)
                        {
                            var filename = System.IO.Path.GetFileName(file);
                            if (filename == path)
                            {
                                Android.Net.Uri uri = Android.Net.Uri.Parse(pathImage);
                                vh.ImageItem.SetImageURI(uri);
                            }
                        }
                    }
                    vh.ShortName.Visibility = ViewStates.Gone;
                }
                else
                {
                    string conColor = Utils.SetBackground(Convert.ToInt32(item.Colors));
                    var color = Android.Graphics.Color.ParseColor(conColor);
                    vh.ImageItem.SetBackgroundColor(color);
                    vh.ShortName.Visibility = ViewStates.Visible;
                }
            }

            vh.ShortName.Text = item.ItemName?.ToString();
            vh.Name.Text = item.ItemName?.ToString();

            vh.TotalSale.Text = itemOnBranch[position].BalanceStock.ToString("#,##0");


        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_item, parent, false);
            ListViewBestSaleHolder vh = new ListViewBestSaleHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}