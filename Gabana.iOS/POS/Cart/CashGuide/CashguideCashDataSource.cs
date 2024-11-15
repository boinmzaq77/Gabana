using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;
using System.Threading.Tasks;

namespace Gabana.iOS
{
    public class CashguideCashDataSource : UICollectionViewDataSource
    {
        public List<CashTemplate> cash = new List<CashTemplate>();
        
        public CashguideCashDataSource(List<CashTemplate> all)
        {
            this.cash = all;
           
        }
        public void ReloadData(List<CashTemplate> all)
        {
            this.cash = all;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("CashguideCashCollectionViewCell", indexPath) as CashguideCashCollectionViewCell;
            //cell.Name = "+"+cash[(int)indexPath.Row].Amount.ToString("#,##0.00");
            string cashstring; 
            int indexpoint = cash[(int)indexPath.Row].Amount.ToString().LastIndexOf(".");
            if (indexpoint != -1)
            {
                var check = cash[(int)indexPath.Row].Amount.ToString().Split(".");
                if (check[1].Length == 2)
                {
                    cashstring = cash[(int)indexPath.Row].Amount.ToString("#,###.00");
                }
                else if (check[1].Length == 1)
                {
                    cashstring = cash[(int)indexPath.Row].Amount.ToString("#,###.0");
                }
                else
                {
                    cashstring = cash[(int)indexPath.Row].Amount.ToString("#,###") + ".";
                }
            }
            else
            {
                cashstring = cash[(int)indexPath.Row].Amount.ToString("#,###");
            }
            cell.Name = "+"+cashstring;
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return cash.Count;
        }

        #region Events
        public delegate void NoteSelectIndexDelegate(NSIndexPath indexPath);
        public event NoteSelectIndexDelegate OnNoteSelectIndex;
        #endregion
    }
}