using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Gabana.ios.Phone
{
    public class itemPOSCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            itemPOSViewCell cell = collectionView.CellForItem(indexPath) as itemPOSViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemPOSSelected?.Invoke(indexPath);
            return true;
        }
        

        #region Events
        public delegate void itemPOSSelectedDelegate(NSIndexPath indexPath);
        public event itemPOSSelectedDelegate OnItemPOSSelected;
        #endregion

    }
}