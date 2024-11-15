﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class MemberTypeSettingCollectionDelegate : UICollectionViewDelegate
    {
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            MemberTypeSettingViewCell cell = collectionView.CellForItem(indexPath) as MemberTypeSettingViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredVertically, false);
                return true;
            }

            OnItemSelected?.Invoke(indexPath);
            return true;
        }

        #region Events
        public delegate void BranchSettingSelectedDelegate(NSIndexPath indexPath);
        public event BranchSettingSelectedDelegate OnItemSelected;
        #endregion

    }
}