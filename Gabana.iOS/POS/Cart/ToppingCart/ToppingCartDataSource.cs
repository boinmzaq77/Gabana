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

namespace Gabana.iOS
{
    public class ToppingCartDataSource : UICollectionViewDataSource
    {
        private List<TranDetailItemTopping> item;
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;


        public ToppingCartDataSource(List<TranDetailItemTopping> item)
        {
           
            this.item = item;
        }
        public void ReloadData(List<TranDetailItemTopping> item)
        {
            this.item = item;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell = new UICollectionViewCell();
            try
            {

                var cell2 = collectionView.DequeueReusableCell("ToppingCartCollectionViewCell", indexPath) as ToppingCartCollectionViewCell;
                cell2.Name =  item[indexPath.Row].ItemName + " ("+ DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + Utils.DisplayDecimal((item[indexPath.Row].Quantity * item[indexPath.Row].ToppingPrice)) + ")";
                //cell2.price  = Utils.DisplayDecimal((item[indexPath.Row].Quantity* item[indexPath.Row].ToppingPrice));
                cell2.size = collectionView.Frame.Width;
                return cell2;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return item.Count;
        }
       
    }
}