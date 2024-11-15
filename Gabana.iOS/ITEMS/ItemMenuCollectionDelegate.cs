using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class ItemMenuCollectionDelegate : UICollectionViewDelegate
    {
        MenuCollectionViewCell lastSelectedCell = null;
        bool firsttime;
        public ItemMenuCollectionDelegate()
        {
            firsttime = true;
        }

        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            MenuCollectionViewCell cell = collectionView.CellForItem(indexPath) as MenuCollectionViewCell;
            if (lastSelectedCell != null)
            {
                lastSelectedCell.ShowSelected(false);
            }
            if (cell == null)
            {
                return true;
            }

            lastSelectedCell = cell;
            cell.ShowSelected(true);
            firsttime = false;
            collectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredHorizontally, false);
            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void ItemMenuSelectedDelegate(NSIndexPath indexPath);
        public event ItemMenuSelectedDelegate OnItemSelected;
        #endregion

    }
}