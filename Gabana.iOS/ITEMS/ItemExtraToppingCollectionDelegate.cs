using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class ItemExtraToppingCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            CellItemToppingList cell = collectionView.CellForItem(indexPath) as CellItemToppingList;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void itemExtraToppingSelectedDelegate(NSIndexPath indexPath);
        public event itemExtraToppingSelectedDelegate OnItemSelected;
        #endregion

    }
}