﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class MainMenuCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            MainMenuCollectionViewCell cell = collectionView.CellForItem(indexPath) as MainMenuCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void MainMenuSelectedDelegate(NSIndexPath indexPath);
        public event MainMenuSelectedDelegate OnItemSelected;
        #endregion

    }
}