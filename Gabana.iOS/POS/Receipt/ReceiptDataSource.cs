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
    public class ReceiptDataSource : UICollectionViewDataSource
    {
        CoreGraphics.CGRect frame;
        private List<TranDetailItemWithTopping> tranDetailItems;

        public ReceiptDataSource(List<TranDetailItemWithTopping> tranDetailItems, CoreGraphics.CGRect frame)
        {
            this.frame = frame;
            this.tranDetailItems = tranDetailItems;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell;
            var cell2 = collectionView.DequeueReusableCell("receiptCollectionViewCell", indexPath) as ReceiptCollectionViewCell;
            cell2.Name = tranDetailItems[indexPath.Row].tranDetailItem.ItemName + " " + tranDetailItems[indexPath.Row].tranDetailItem.SizeName;
            cell2.toppinglist = tranDetailItems[indexPath.Row].tranDetailItemToppings;
            cell2.amount = tranDetailItems[indexPath.Row].tranDetailItem.Quantity.ToString("#,##0");
            cell2.price = Utils.DisplayDecimal(tranDetailItems[indexPath.Row].tranDetailItem.Amount);
            cell2.Height = tranDetailItems[indexPath.Row].tranDetailItemToppings.Count * 25;
            if (tranDetailItems[indexPath.Row].tranDetailItem.Discount>0)
            {
                cell2.discount = Utils.TextBundle("discount", "Discount") +" (" + Utils.DisplayDecimal(tranDetailItems[indexPath.Row].tranDetailItem.Discount) + ")";
            }
            else
            {
                cell2.discount = "";
            }
            
            cell2.comment = tranDetailItems[indexPath.Row].tranDetailItem.Comments;
            cell2.size = frame.Width;
            cell = cell2;
            
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return tranDetailItems.Count;
        }
    }
}