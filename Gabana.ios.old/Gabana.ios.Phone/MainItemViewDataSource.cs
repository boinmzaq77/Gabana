using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFImageLoading;
using Foundation;
using Gabana.Model;
using UIKit;

namespace Gabana.ios.Phone
{
    public class MainItemViewDataSource : UICollectionViewDataSource
    {
        public List<Menuitem> mainItems;

        #region Constructors
        public MainItemViewDataSource(List<Menuitem> mainItem)
        {
            this.mainItems = mainItem;
        }
        #endregion
        public void ReloadData(List<Menuitem> mainItem)
        {
            this.mainItems = mainItem;
        }

        #region Override Methods
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var mainItemnViewCell = collectionView.DequeueReusableCell("mainitemViewCell", indexPath) as mainitemViewCell;
            if (mainItems[(int)indexPath.Item].MenuIcon != null && mainItems[(int)indexPath.Item].MenuIcon != "")
            {
                mainItemnViewCell.Image = mainItems[(int)indexPath.Item].MenuIcon;
                mainItemnViewCell.UserInteractionEnabled = true;
            }
            else
            {
                mainItemnViewCell.Image = "NoIMG.png";
                mainItemnViewCell.UserInteractionEnabled = false;
            }
            mainItemnViewCell.Name = mainItems[(int)indexPath.Item].MenuName;
            mainItemnViewCell.Layer.BorderColor = new UIColor(red: 51f/255f, green: 170f/255f, blue: 225f/255f, alpha: 1.00f).CGColor;
            mainItemnViewCell.Layer.BorderWidth = 1;
            
            return mainItemnViewCell;
        }
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            // We only have one section
            return 1;
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return mainItems.Count;
        }
        public override bool CanMoveItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            // We can always move items
            return false;
        }
        #endregion
    }
}