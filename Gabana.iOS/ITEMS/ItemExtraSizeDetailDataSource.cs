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
    public class ItemExtraSizeDetailDataSource : UICollectionViewDataSource
    {
        public UICollectionView collectionView;
        public List<ItemExSize> allExtra = new List<ItemExSize>();
        public ItemExtraSizeDetailDataSource(List<ItemExSize> all)
        {
            this.allExtra = all;
        }
        public void ReloadData(List<ItemExSize> all)
        {
            this.allExtra = all;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.collectionView = collectionView;
            var cell = collectionView.DequeueReusableCell("extraSizeCollectionViewCell", indexPath) as ExtraSizeCollectionViewCell;
            cell.SizeName = allExtra[(int)indexPath.Row].ExSizeName ?? "";
            cell.Price = "";
            cell.EstimateCost = "";
            if (allExtra[(int)indexPath.Row].Price!=0)
            {
                cell.Price = allExtra[(int)indexPath.Row].Price.ToString("N2");
            }
            if (allExtra[(int)indexPath.Row].EstimateCost != 0)
            {
                cell.EstimateCost = allExtra[(int)indexPath.Row].EstimateCost.ToString("N2");
            }
            cell.Nub = Int32.Parse(allExtra[(int)indexPath.Row].Comments);
            cell.OnCardCellDeleteCodeBtn -= Cell_OnCardCellDeleteCodeBtn;
            cell.OnCardCellDeleteCodeBtn += Cell_OnCardCellDeleteCodeBtn;
           // cell.OnCardCellDeleteCodeBtn += (extraCollectionCell) =>
           // {
           //     NSIndexPath indexPathExtra = collectionView.IndexPathForCell(extraCollectionCell);
           ////     Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
           //     OnExtraSizeDeleteIndex?.Invoke(indexPathExtra);
           // };
            return cell;
        }

        private void Cell_OnCardCellDeleteCodeBtn(ExtraSizeCollectionViewCell extraSizeCollectionViewCell)
        {
            NSIndexPath indexPathExtra = collectionView.IndexPathForCell(extraSizeCollectionViewCell);
            //     Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
            OnExtraSizeDeleteIndex?.Invoke(indexPathExtra);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return allExtra.Count;
        }
        #region Events
        public delegate void ExtraSizeDeleteIndexDelegate(NSIndexPath indexPath);
        public event ExtraSizeDeleteIndexDelegate OnExtraSizeDeleteIndex;

        #endregion
    }
}