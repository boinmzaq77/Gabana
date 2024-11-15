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
    public class ItemDiscountDataSourceList : UICollectionViewDataSource
    {
        public List<Gabana.ORM.MerchantDB.DiscountTemplate> discount;
       // Gabana.ShareSource.Manage.CategoryManage category = new Gabana.ShareSource.Manage.CategoryManage();
       public ItemDiscountDataSourceList(List<DiscountTemplate> dis)
       {
            this.discount = dis;
       }
        public void ReloadData(List<DiscountTemplate> item)
        {
            this.discount = item;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell1 = collectionView.DequeueReusableCell("itemDiscountCellList", indexPath) as itemDiscountCollectionViewCellList;
            cell1.DiscountItem = this.discount[(int)indexPath.Row].TemplateName.ToString();
          
            return cell1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.discount != null)
                return this.discount.Count;
            else
                return 0;
        }
    }
}