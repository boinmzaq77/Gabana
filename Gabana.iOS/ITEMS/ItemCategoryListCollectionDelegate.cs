﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class ItemCategoryListCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            itemCatagoryCollectionViewCellList cell = collectionView.CellForItem(indexPath) as itemCatagoryCollectionViewCellList;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void itemCategoryListSelectedDelegate(NSIndexPath indexPath);
        public event itemCategoryListSelectedDelegate OnItemSelected;
        #endregion

    }
}