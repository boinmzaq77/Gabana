using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class OptionToppingCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            OptionToppingCollectionViewCell cell = collectionView.CellForItem(indexPath) as OptionToppingCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void OptionToppingSelectedDelegate(NSIndexPath indexPath);
        public event OptionToppingSelectedDelegate OnItemSelected;
        #endregion

    }
}