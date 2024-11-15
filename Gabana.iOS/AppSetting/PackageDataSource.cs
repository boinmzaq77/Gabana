using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System.Threading.Tasks;
using Gabana.ShareSource;
using System.IO;
using Gabana.AppSetting;

namespace Gabana.iOS
{
    public class PackageDataSource : UICollectionViewDataSource
    {
        public List<PackageProduce> item;
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        ItemManage itemmanager = new ItemManage();
        
        public PackageDataSource(List<PackageProduce> item) 
        {
            this.item = item;
        }
        public void ReloadData(List<PackageProduce> item)
        {
            this.item = item;
        }
        public void ReloadData()
        {
          
        }


        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("PackageCollectionViewCell", indexPath) as PackageCollectionViewCell;
            var position = (int)indexPath.Row;
            cell.Name = this.item[position].PackageName;
            cell.bracnh = this.item[position].MaxBranch;
            cell.user = this.item[position].MaxUser;
            cell.price = this.item[position].Price;
            cell.use = false;
            if (PackageController.PackageIdSelected == this.item[position].id)
            {
                cell.use = true;
            }
            
            


            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return item.Count;
        }
    }
}