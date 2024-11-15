using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using System.Threading.Tasks;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class ItemNoteCategoryDataSource : UICollectionViewDataSource
    {
        public UICollectionView collectionView;
        public List<NoteCategory> NoteCatagory;
        public ItemNoteCategoryDataSource(List<NoteCategory> menuPos)
        {
              NoteCatagory = menuPos;
        }
        public void ReloadData(List<NoteCategory> item)
        {
            this.NoteCatagory = item;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.collectionView = collectionView;
            NoteMenuCollectionViewCell cell = collectionView.DequeueReusableCell("menuPosCell", indexPath) as NoteMenuCollectionViewCell;
            if((int)indexPath.Row == 0)
            {
                cell.Name = Utils.TextBundle("all","ALL");
                
            }
            else
            {
                cell.Name = this.NoteCatagory[(int)indexPath.Row].Name;
            }
            if (itemNoteController.Select == (int)indexPath.Row)
            {
                cell.ShowSelected(true);
            }
            else
            {
                cell.ShowSelected(false);
            }
            cell.OnLongClick -= Cell_OnLongClick;
            cell.OnLongClick += Cell_OnLongClick;

            return cell;
        }

        private void Cell_OnLongClick(NoteMenuCollectionViewCell indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnLong?.Invoke(indexPathQRcode);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.NoteCatagory.Count;
        }
        public string GetItem(int row)
        {
            return this.NoteCatagory[row].Name;
        }
        public delegate void NoteLongDelete(NSIndexPath indexPath);
        public event NoteLongDelete OnLong;
    }
}