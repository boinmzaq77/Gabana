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

namespace Gabana.iOS
{
    public class GiftVoucherDataSource : UICollectionViewDataSource
    {
        public UICollectionView collectionView;
        public GiftVoucherViewCell choosecell;
        public LstItemGiftVoucher listvoucher;
        public GiftVoucherDataSource()
        {
        }
        public GiftVoucherDataSource(LstItemGiftVoucher cur)
        {
            this.listvoucher = cur;
        }
        public void ReloadData(LstItemGiftVoucher cur)
        {
            this.listvoucher = cur;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.collectionView = collectionView;
            var cell = collectionView.DequeueReusableCell("GiftVoucherViewCell", indexPath) as GiftVoucherViewCell;
            int position = (int)indexPath.Row;
            cell.Code = listvoucher[position].GiftVoucherCode;
            cell.Name = listvoucher[position].GiftVoucherName;
            cell.Cost = listvoucher[position].FmlAmount;

            if (choosecell != null)
            {
                if (choosecell == cell)
                {
                    choosecell = null;
                }
            }
            cell.OnItemSwipe -= Cell_OnItemSwipe;
            cell.OnItemSwipe += Cell_OnItemSwipe;
            cell.OnItemClear += Cell_OnItemClear;
            cell.OnItemClear -= Cell_OnItemClear;

            cell.OnDeleteItem -= Cell_OnDeleteItem;
            cell.OnDeleteItem += Cell_OnDeleteItem;
            return cell;

        }
        private void Cell_OnDeleteItem(GiftVoucherViewCell indexPath)
        {
            NSIndexPath indexPathQRcode = this.collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }

        private void Cell_OnItemSwipe(GiftVoucherViewCell indexPath)
        {
            if (choosecell != null)
            {
                UIView.Animate(0.7, () =>
                {
                    var frame2 = choosecell.Frame;
                    frame2.X = 0;
                    choosecell.showbtndelete = false;
                    choosecell.Frame = frame2;
                });
            };
            choosecell = indexPath;
            UIView.Animate(0.7, () =>
            {
                var frame = indexPath.Frame;
                frame.X = -80;
                choosecell.showbtndelete = true;
                indexPath.Frame = frame;
            });
        }
        private void Cell_OnItemClear(GiftVoucherViewCell indexPath)
        {
            if (choosecell != null)
            {
                var frame2 = choosecell.Frame;
                frame2.X = 0;
                UIView.Animate(0.7, () =>
                {
                    choosecell.showbtndelete = false;
                    choosecell.Frame = frame2;
                });
            }
        }
        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.listvoucher.Count;
        }
    }

}