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

namespace Gabana.iOS
{
    public class ReportItemStockDataSource : UICollectionViewDataSource
    {
        public List<Item> listitem;
        List<Gabana.ORM.Master.ItemOnBranch> itemOnBranch;

        public ReportItemStockDataSource(List<Gabana.ORM.Master.ItemOnBranch> s, List<Item> l)
        {
            itemOnBranch = s;
            listitem = l;
        }

        public void ReloadData(List<Item> l)
        {
            listitem = l;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ItemTopSaleReportDataViewCell", indexPath) as ItemTopSaleReportDataViewCell;
            int position = (int)indexPath.Row;
            var item = listitem.Where(x => x.SysItemID == itemOnBranch[position].SysItemID).FirstOrDefault();
            if (item != null)
            {
                if (item.Colors != null)
                {
                    cell.Colors = (long)item.Colors;
                    cell.ShortName = item.ShortName;
                }
                else
                {
                    cell.Colors = 0;
                    cell.ShortName = null;
                    var paths = item.PicturePath;
                    if (!string.IsNullOrEmpty(paths))
                    {
                        if (DataCashingAll.CheckConnectInternet)
                        {
                            cell.Image = paths;
                        }
                        else
                        {
                            cell.Image = item.PictureLocalPath;
                        }
                    }
                }
                cell.ShortName = item.ShortName?.ToString();
                cell.Name = item.ItemName?.ToString();
            }

            cell.Total = itemOnBranch[position].BalanceStock.ToString("#,##0")??"";

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.listitem.Count;
        }
    }
}