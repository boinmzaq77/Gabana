using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class QRSettingCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            MYQRCollectionViewCellList cell = collectionView.CellForItem(indexPath) as MYQRCollectionViewCellList;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void QRettingSelectedDelegate(NSIndexPath indexPath);
        public event QRettingSelectedDelegate OnItemSelected;
        #endregion

    }
}