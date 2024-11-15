using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class BillHistoryCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            BillListViewCell cell = collectionView.CellForItem(indexPath) as BillListViewCell;
            if (cell == null)
            {
                //collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void DateSelectedDelegate(NSIndexPath indexPath);
        public event DateSelectedDelegate OnItemSelected;
        #endregion

    }
}