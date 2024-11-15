using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class POSMenuCollectionDelegate : UICollectionViewDelegate
    {
        public POSMenuCollectionViewCell lastSelectedCell = null;
        public void POSMenu(UICollectionView collectionView)
        {
            NSIndexPath nSIndexPath = NSIndexPath.FromItemSection(0, 0);
            POSMenuCollectionViewCell cell = collectionView.CellForItem(nSIndexPath) as POSMenuCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredHorizontally, true);
            }
            lastSelectedCell = cell;
            //cell.ShowSelected(true);
            collectionView.ScrollToItem(nSIndexPath, UICollectionViewScrollPosition.CenteredHorizontally, true);
            OnItemSelected?.Invoke(nSIndexPath);
        }
        //override 
        public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (lastSelectedCell != null)
            {
                lastSelectedCell.ShowSelected(false);
            }
            if (itemNoteController.Select == 0 && indexPath.Row != 0)
            {
                NSIndexPath nSIndexPath = NSIndexPath.FromItemSection(0, 0);
                POSMenuCollectionViewCell cell2 = collectionView.CellForItem(nSIndexPath) as POSMenuCollectionViewCell;
                if (cell2 != null)
                {
                    cell2.ShowSelected(false);
                }
                
            }

            POSMenuCollectionViewCell cell = collectionView.CellForItem(indexPath) as POSMenuCollectionViewCell;
            if (cell == null)
            {
                collectionView.ScrollToItem(NSIndexPath.FromRowSection(0, 0), UICollectionViewScrollPosition.CenteredHorizontally, true);
                return true;
            }

            lastSelectedCell = cell;
            cell.ShowSelected(true);
            collectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredHorizontally, true);
            OnItemSelected?.Invoke(indexPath);
            return true;
        }
        

        public static void Test() 
        {
            
        }


        #region Events
        public delegate void POSMenuSelectedDelegate(NSIndexPath indexPath);
        public event POSMenuSelectedDelegate OnItemSelected;
        #endregion

    }
}