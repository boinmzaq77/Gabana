using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Gabana.ios.Phone
{
    public class PaymentCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            PaymentViewCell cell = collectionView.CellForItem(indexPath) as PaymentViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnPaymentselected?.Invoke(indexPath);
            return true;
        }
        

        #region Events
        public delegate void OnPaymentSelectedDelegate(NSIndexPath indexPath);
        public event OnPaymentSelectedDelegate OnPaymentselected;
        #endregion

    }
}