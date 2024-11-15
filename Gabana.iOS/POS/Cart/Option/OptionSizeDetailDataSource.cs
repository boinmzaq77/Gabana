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
    public class OptionSizeDetailDataSource : UICollectionViewDataSource
    {
        public List<ItemExSize> allExtra = new List<ItemExSize>();
        public OptionSizeCollectionViewCell cellchoose ;
        TranDetailItemWithTopping item =new  TranDetailItemWithTopping();
        public OptionSizeDetailDataSource(List<ItemExSize> all, TranDetailItemWithTopping item)
        {
            this.allExtra = all;
            this.item = item;
        }
        public void ReloadData(List<ItemExSize> all)
        {
            this.allExtra = all;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("optionSizeCollectionViewCell", indexPath) as OptionSizeCollectionViewCell;
            cell.SizeName = allExtra[(int)indexPath.Row].ExSizeName ?? "";
            cell.Price = "";
            if (allExtra[(int)indexPath.Row].Price!=0)
            {
                cell.Price = Utils.DisplayDecimal(allExtra[(int)indexPath.Row].Price);
            }
            if (string.IsNullOrEmpty( item.tranDetailItem.SizeName) )
            {
                if (allExtra[(int)indexPath.Row].ExSizeNo == 999 )
                {
                    cell.SelectSize = true;
                    cellchoose = cell;
                }
            }
            else
            {
                if (item.tranDetailItem.SizeName == allExtra[(int)indexPath.Row].ExSizeName)
                {
                    cell.SelectSize = true;
                    cellchoose = cell;
                }
            }
            

           
            cell.OnSizeCellSelectBtn += (extraCollectionCell) =>
            {
                if (cellchoose != null)
                {
                    cellchoose.SelectSize = false;
                }
                cellchoose = cell;
                cell.SelectSize = true; 
                NSIndexPath indexcode = collectionView.IndexPathForCell(extraCollectionCell);
                Console.WriteLine("indexcode = " + indexcode.ToString());
                OnExtraSizeSelectIndex?.Invoke(indexcode);
            };
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return allExtra.Count;
        }
        #region Events
        public delegate void ExtraSizeSelectIndexDelegate(NSIndexPath indexPath);
        public event ExtraSizeSelectIndexDelegate OnExtraSizeSelectIndex;

        #endregion
    }
}