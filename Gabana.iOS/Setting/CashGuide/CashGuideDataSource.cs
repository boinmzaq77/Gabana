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
    public class CashGuideDataSource : UICollectionViewDataSource
    {
        List<CashTemplate> CashGuideSource = new List<CashTemplate>();
        public UICollectionView collectionView;
        public CashGuideViewCell choosecell;
        public CashGuideDataSource(List<CashTemplate> cur)
        {
            this.CashGuideSource = cur;
        }
        public void ReloadData(List<CashTemplate> cur)
        {
            this.CashGuideSource = cur;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("CashGuideViewCell", indexPath) as CashGuideViewCell;
            this.collectionView = collectionView;
            //cell.Name = this.CashGuideSource[(int)indexPath.Row].Amount.ToString();
            cell.Name = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS+" "+Utils.DisplayDecimal(this.CashGuideSource[(int)indexPath.Row].Amount);
            cell.OnItemSwipe -= Cell_OnItemSwipe;
            cell.OnItemSwipe += Cell_OnItemSwipe;
            cell.OnDeleteItem -= Cell_OnDeleteItem;
            cell.OnDeleteItem += Cell_OnDeleteItem;

            return cell;

        }
        private void Cell_OnDeleteItem(CashGuideViewCell indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }
        private void Cell_OnItemSwipe(CashGuideViewCell indexPath)
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
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.CashGuideSource.Count;
        }
        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;
    }

}