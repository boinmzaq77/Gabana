using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace Gabana.iOS
{
    public class BluetoothCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            BluetoothViewCell cell = collectionView.CellForItem(indexPath) as BluetoothViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnCardSelected?.Invoke(indexPath);
            return true;
        }

        

        #region Events
        public delegate void CardSelectedDelegate(NSIndexPath indexPath);
        public event CardSelectedDelegate OnCardSelected;  
        #endregion

    }
}