using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Gabana.ios.Phone
{
    public class POSMainitemCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            POSheaderViewCell cell = collectionView.CellForItem(indexPath) as POSheaderViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredHorizontally, true);
                return true;
            }

            OnPOSmainItemSelected?.Invoke(indexPath);
            return true;
        }
        

        #region Events
        public delegate void POSMainitemSelectedDelegate(NSIndexPath indexPath);
        public event POSMainitemSelectedDelegate OnPOSmainItemSelected;
        #endregion

    }
}