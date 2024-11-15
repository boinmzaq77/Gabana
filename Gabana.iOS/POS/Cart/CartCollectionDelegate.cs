using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class CartCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            
            CartCollectionViewCell2 cell = collectionView.CellForItem(indexPath) as CartCollectionViewCell2;
            if (cell == null)
            {
                CartCollectionViewCell3 cell3 = collectionView.CellForItem(indexPath) as CartCollectionViewCell3;
                if (cell3 == null)
                {
                    return true;
                }
                collectionView.LayoutIfNeeded();
                
                OnItemSelected?.Invoke(indexPath);
                collectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, true);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            collectionView.LayoutIfNeeded();
            collectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredVertically, true);
            return true;
        }

        #region Events
        public delegate void itemPOSSelectedDelegate(NSIndexPath indexPath);
        public event itemPOSSelectedDelegate OnItemSelected;
        #endregion

    }
}