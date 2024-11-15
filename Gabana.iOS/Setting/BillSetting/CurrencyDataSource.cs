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
    public class CurrencyDataSource : UICollectionViewDataSource
    {
        List<Currency> currencySource = new List<Currency>();

        public CurrencyDataSource(List<Currency> cur)
        {
            this.currencySource = cur;
        }
        public void ReloadData(List<Currency> cur)
        {
            this.currencySource = cur;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("CurrencyViewCell", indexPath) as CurrencyViewCell;
            cell.Name = this.currencySource[(int)indexPath.Row].Name;
            cell.show = this.currencySource[(int)indexPath.Row].Choose;
            if (this.currencySource[(int)indexPath.Row].Choose)
            {
                cell.Image = this.currencySource[(int)indexPath.Row].Imagechoose;
            }
            else
            {
                cell.Image = this.currencySource[(int)indexPath.Row].Image;
            }
            
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.currencySource.Count;
        }
    }

}