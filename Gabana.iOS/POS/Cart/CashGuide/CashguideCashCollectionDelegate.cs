﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class CashguideCashCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            CashguideCashCollectionViewCell cell = collectionView.CellForItem(indexPath) as CashguideCashCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void OptionNoteSelectedDelegate(NSIndexPath indexPath);
        public event OptionNoteSelectedDelegate OnItemSelected;
        #endregion

    }
}