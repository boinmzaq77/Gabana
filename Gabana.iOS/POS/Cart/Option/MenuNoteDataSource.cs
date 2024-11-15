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
    public class MenuNoteDataSource : UICollectionViewDataSource
    {
        public List<NoteCategory> Menu;
        public MenuNoteDataSource(List<NoteCategory> menuPos)
        {
            Menu = menuPos;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            POSMenuCollectionViewCell cell = collectionView.DequeueReusableCell("menuNoteCell", indexPath) as POSMenuCollectionViewCell;
            cell.Name = this.Menu[(int)indexPath.Row].Name;
            if (OptionController.Select == (int)indexPath.Row)
            {
                cell.ShowSelected(true);
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.Menu.Count;
        }
        public string GetItem(int row)
        {
            return this.Menu[row].Name;
        }
    }
}