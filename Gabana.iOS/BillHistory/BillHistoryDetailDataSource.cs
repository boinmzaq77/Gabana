using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System.Threading.Tasks;

namespace Gabana.iOS
{
    public class BillHistoryDetailDataSource : UICollectionViewDataSource
    {
        private List<TranDetailItemWithTopping> tranDetailItems;
        public BillHistoryDetailDataSource()
        {
        }
        public BillHistoryDetailDataSource(List<TranDetailItemWithTopping> tranDetailItems)
        {
            this.tranDetailItems = tranDetailItems;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell;
            var cell2 = collectionView.DequeueReusableCell("BillHistoryDetailCollectionViewCell", indexPath) as BillHistoryDetailCollectionViewCell;
            cell2.Name = tranDetailItems[indexPath.Row].tranDetailItem.ItemName;
            cell2.amount = tranDetailItems[indexPath.Row].tranDetailItem.Quantity.ToString("#,##0");
            cell2.price = Utils.DisplayDecimal(tranDetailItems[indexPath.Row].tranDetailItem.Amount); 
            cell = cell2;
            
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return tranDetailItems.Count;
        }
    }
}