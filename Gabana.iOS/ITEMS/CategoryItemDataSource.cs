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
    public class CategoryItemDataSource : UICollectionViewDataSource
    {
        public List<Item> Menu;
        ItemManage item = new ItemManage();
        public CategoryItemDataSource(List<Item> item)
        {
            this.Menu = item;
        }
        public void ReloadData(List<Item> item)
        {
            this.Menu = item;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("categoryCollectionViewCell", indexPath) as CategoryCollectionViewCell;
            
                cell.Cost = this.Menu[(int)indexPath.Row].Price.ToString("N2");
                cell.Name = this.Menu[(int)indexPath.Row].ItemName;
                if (this.Menu[(int)indexPath.Row].Colors != null)
                {
                    cell.Colors = (long)this.Menu[(int)indexPath.Row].Colors;
                    cell.ShortName = this.Menu[(int)indexPath.Row].ShortName;
                }
                else
                {
                    cell.Colors = 0;
                    cell.ShortName = null;
                }
                return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.Menu != null)
                return this.Menu.Count;
            else
                return 0;
        }
    }
}