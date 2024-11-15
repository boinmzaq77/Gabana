using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;
using System.Threading.Tasks;
using static Gabana.iOS.CustomerController;
using Gabana3.JAM.Report;
using Gabana.ShareSource;
using System.IO;

namespace Gabana.iOS
{
    public class ReportItemTopSaleDataSource : UICollectionViewDataSource
    {
        public List<Item> listitem;
        private int BestSellBy;
        bool flag = false;
        public string positionClick;
        long? itemId;
        List<Gabana3.JAM.Report.SummaryItemModel> summaryItems = new List<Gabana3.JAM.Report.SummaryItemModel>();
        private string CURRENCYSYMBOLS;

        public ReportItemTopSaleDataSource(List<Gabana3.JAM.Report.SummaryItemModel> s, List<Item> l, int b)
        {
            summaryItems = s;
            listitem = l;
            BestSellBy = b;
        }

        public void ReloadData(List<Gabana3.JAM.Report.SummaryItemModel> s)
        {
            summaryItems = s;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ItemTopSaleReportDataViewCell", indexPath) as ItemTopSaleReportDataViewCell;
            var item = listitem.Where(x => x.ItemName == summaryItems[(int)indexPath.Row].ItemName).FirstOrDefault();
            if(item!=null)
            {
                cell.ShortName = "";
                cell.Colors = 0;
                cell.Image = null;
                if (item.ThumbnailLocalPath == null || item.ThumbnailLocalPath == "")
                {

                    cell.Image = null;
                    if (item.Colors != null)
                    {
                        cell.Colors = (long)item.Colors;
                        cell.ShortName = item.ShortName;
                    }
                    else
                    {
                        cell.Colors = 0;
                        cell.ShortName = null;
                    }
                }
                else
                {
                    cell.ShortName = null;
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    cell.Image = Path.Combine(docFolder, item.ThumbnailLocalPath);
                }

                //if (item.Colors != null)
                //{
                //    cell.Colors = (long)item.Colors;
                //    cell.ShortName = item.ShortName;
                //}
                //else
                //{
                //    cell.Colors = 0;
                //    cell.ShortName = null;
                //    var paths = item.PicturePath;
                //    if (!string.IsNullOrEmpty(paths))
                //    {
                //        if (DataCashingAll.CheckConnectInternet)
                //        {
                //            cell.Image = paths;
                //        }
                //        else
                //        {
                //            cell.Image = item.PictureLocalPath;
                //        }
                //    }
                //}
            }
           
            cell.Name = summaryItems[(int)indexPath.Row].ItemName?.ToString();

            switch (BestSellBy)
            {
                case 0:
                    cell.Total = CURRENCYSYMBOLS + " " + summaryItems[(int)indexPath.Row].SumTotalAmount.ToString("##,###.00");
                    break;
                case 1:
                    cell.Total = summaryItems[(int)indexPath.Row].SumQuantity.ToString("#,###");
                    break;
                default:
                    break;
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.summaryItems.Count;
        }
    }
}