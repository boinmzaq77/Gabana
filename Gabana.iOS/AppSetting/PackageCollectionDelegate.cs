using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class PackageCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            PackageCollectionViewCell cell = collectionView.CellForItem(indexPath) as PackageCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }
            OnItemSelected?.Invoke(indexPath);
            return true;
        }
        
         
        #region Events
        public delegate void itemPOSSelectedDelegate(NSIndexPath indexPath);
        public event itemPOSSelectedDelegate OnItemSelected;
        #endregion

    }
}