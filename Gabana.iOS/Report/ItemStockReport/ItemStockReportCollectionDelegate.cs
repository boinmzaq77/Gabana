using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class ItemStockReportCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemStockReportViewCell cell = collectionView.CellForItem(indexPath) as ItemStockReportViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }
            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void itemSelectedDelegate(NSIndexPath indexPath);
        public event itemSelectedDelegate OnItemSelected;
        #endregion

    }
}