using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using System.Threading.Tasks;

namespace Gabana.iOS
{
    public class ItemSubNoteDataSource : UICollectionViewDataSource
    {
        public List<Gabana.ORM.MerchantDB.Item> note;
        public ItemSubNoteDataSource(List<Item> item)
       {
            this.note = item;
       }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell1 = collectionView.DequeueReusableCell("subNoteCollectionViewCell", indexPath) as SubNoteCollectionViewCell;
            if(this.note!=null)
            {
                cell1.SubnoteName = this.note[(int)indexPath.Row].ItemName ?? "";
            }
           else
            {
                cell1.SubnoteName = "";
            }
            cell1.OnCardCellDeleteCodeBtn += (subNoteCollectionCell) =>
            {
                NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(subNoteCollectionCell);
                Console.WriteLine("indexPathSubNote = " + indexPathQRcode.ToString());
                OnSubNoteDeleteIndex?.Invoke(indexPathQRcode);
            };
            return cell1;
        }
        #region Events
        public delegate void SubNoteDeleteIndexDelegate(NSIndexPath indexPath);
        public event SubNoteDeleteIndexDelegate OnSubNoteDeleteIndex;

        #endregion
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if(this.note != null)
            {
                return this.note.Count;
            }
            else
            {
                return 1;
            }
          
        }
    }
}