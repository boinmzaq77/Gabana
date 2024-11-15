using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public class NoteMenuCollectionDelegate : UICollectionViewDelegate
    {
        public NoteMenuCollectionViewCell lastSelectedCell = null;
        public void POSMenu(UICollectionView collectionView)
        {
            NSIndexPath nSIndexPath = NSIndexPath.FromItemSection(0, 0);
            NoteMenuCollectionViewCell cell = collectionView.CellForItem(nSIndexPath) as NoteMenuCollectionViewCell;
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
                NoteMenuCollectionViewCell cell2 = collectionView.CellForItem(nSIndexPath) as NoteMenuCollectionViewCell;
                if (cell2 != null)
                {
                    cell2.ShowSelected(false);
                }
                
            }
            if (itemNoteController.Last != null)
            {
                NoteMenuCollectionViewCell cell2 = collectionView.CellForItem(itemNoteController.Last) as NoteMenuCollectionViewCell;
                if (cell2 != null)
                {
                    cell2.ShowSelected(false);
                }
            }

            NoteMenuCollectionViewCell cell = collectionView.CellForItem(indexPath) as NoteMenuCollectionViewCell;
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
        public delegate void NoteMenuSelectedDelegate(NSIndexPath indexPath);
        public event NoteMenuSelectedDelegate OnItemSelected;
        #endregion

    }
}