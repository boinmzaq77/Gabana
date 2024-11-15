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
    public class ItemDetailDataSource : UICollectionViewDataSource
    {
        public Gabana.ORM.MerchantDB.Item itemData = new Gabana.ORM.MerchantDB.Item();
        Gabana.ShareSource.Manage.ItemManage itemManager = new ItemManage();
        public ItemDetailDataSource(){}
        public ItemDetailDataSource(long sysID) {
            ItemDetailSource(sysID);
        }
        public async Task ItemDetailSource(long sysID)
        {
            this.itemData = await itemManager.GetItem((int)MainController.merchantlocal.MerchantID, (int)sysID);
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ItemDetailDataSource", indexPath) as ItemAddDatailViewCell;
            if(this.itemData != null && this.itemData.ItemName!= null && this.itemData.ItemName!="")
            {
                cell.ItemName = this.itemData.ItemName;
                cell.ItemPrice = this.itemData.Price.ToString();
                cell.ItemCode = this.itemData.ItemCode;
                if (this.itemData.TaxType == 'N')
                {
                    cell.ItemVatMode = "None Vat";
                }
                if (this.itemData.TaxType == 'V')
                {
                    cell.ItemVatMode = "Include Vat";
                }
                cell.Colors = (long)this.itemData.Colors;
                cell.Image = this.itemData.PictureLocalPath;
                cell.ItemCardShortName = this.itemData.ShortName;
                cell.ItemCardPrice = this.itemData.Price.ToString("N2");
                cell.ItemCost = this.itemData.EstimateCost.ToString();
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return 1;
        }
    }
}