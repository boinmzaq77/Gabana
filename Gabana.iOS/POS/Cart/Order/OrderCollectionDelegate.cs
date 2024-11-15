﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class OrderCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            OrderCollectionViewCell cell = collectionView.CellForItem(indexPath) as OrderCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void OrderSelectedDelegate(NSIndexPath indexPath);
        public event OrderSelectedDelegate OnItemSelected;
        #endregion

    }
}