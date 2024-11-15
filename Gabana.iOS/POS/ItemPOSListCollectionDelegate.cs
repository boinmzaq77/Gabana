using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class ItemPOSListCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //ItemPOSCollectionViewCellList cell = collectionView.CellForItem(indexPath) as ItemPOSCollectionViewCellList;
            //if (cell == null)
            //{
            //    collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
            //    return true;
            //}
            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void itemPOSListSelectedDelegate(NSIndexPath indexPath);
        public event itemPOSListSelectedDelegate OnItemSelected;
        #endregion

    }
}