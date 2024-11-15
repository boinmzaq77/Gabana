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
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class ItemStockReportDataSource : UICollectionViewDataSource
    {

        public ListItem listitem;
        public List<Item> listChooseItem;

        public ItemStockReportDataSource(ListItem l, List<Item> c)
        {
            listitem = l;
            listChooseItem = c;
        }
        public void ReloadData(List<Item> item)
        {
            this.listChooseItem = item;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ItemStockReportViewCell", indexPath) as ItemStockReportViewCell;

            var paths = this.listitem.items[(int)indexPath.Row].PicturePath;
            if (!string.IsNullOrEmpty(paths))
            {
                if (DataCashingAll.CheckConnectInternet)
                {
                    cell.Image = paths;
                }
                else
                {
                    //load local
                    cell.Image = this.listitem.items[(int)indexPath.Row].PictureLocalPath;
                }
                cell.ShortName = "";
             }
            else
            {
                cell.Colors = (long)this.listitem.items[(int)indexPath.Row].Colors;
            }
            cell.ShortName = listitem.items[(int)indexPath.Row].ShortName?.ToString();
            cell.Name = listitem.items[(int)indexPath.Row].ItemName?.ToString();

            var index = listChooseItem.FindIndex(x => x.SysItemID == listitem.items[(int)indexPath.Row].SysItemID);
            if (index == -1)
            {
                cell.Select = false;
            }
            else
            {
                cell.Select = true;
            }

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.listitem != null)
                return this.listitem.items.Count;
            else
                return 0;
        }

    }
}