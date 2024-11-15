using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Gabana.ios.Phone
{
    public class MainitemCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            mainitemViewCell cell = collectionView.CellForItem(indexPath) as mainitemViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnmainItemSelected?.Invoke(indexPath);
            return true;
        }
        

        #region Events
        public delegate void MainitemSelectedDelegate(NSIndexPath indexPath);
        public event MainitemSelectedDelegate OnmainItemSelected;
        #endregion

    }
}