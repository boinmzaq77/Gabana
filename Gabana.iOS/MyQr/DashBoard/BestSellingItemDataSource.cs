using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using System.Threading.Tasks;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System.Drawing;

namespace Gabana.iOS
{
    public class BestSellingItemDataSource : UICollectionViewDataSource
    {
        public List<Item> itemAll;
        public ListBestSallItem item;
       
        public BestSellingItemDataSource(List<Item> all,ListBestSallItem Top)
           {
            this.itemAll = all;
                this.item = Top;
           }
        public void ReloadData(ListBestSallItem item)
        {
            this.item = item;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell1 = collectionView.DequeueReusableCell("BestSellingItemViewCell", indexPath) as BestSellingItemViewCell;

            var item = this.itemAll.Where(x => x.ItemName == this.item[(int)indexPath.Row].itemName).FirstOrDefault();
            var paths = item.PicturePath;
            cell1.ShortName = "";
            if (!string.IsNullOrEmpty(paths))
            {
                if (DataCashingAll.CheckConnectInternet)
                {
                    Utils utils = new Utils();
                    cell1.Image = paths;
                    cell1.ShortName = "";
                }
                else
                {
                    cell1.Colors = 0;
                    cell1.ShortName = item.ShortName??"";
                }
            }
            else
            {
                if(item.Colors != 0)
                {
                    cell1.Colors = item.Colors??0;
                    cell1.ShortName = item.ShortName ?? "";
                }
            }
           
            cell1.Name = this.item[(int)indexPath.Row].itemName;
            cell1.Total = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS+" "+ Utils.DisplayDecimal(this.item[(int)indexPath.Row].totalAmount);
            switch (this.item[(int)indexPath.Row].movementFlag)
            {
                case -1: //down
                    cell1.Status = "DbDown";
                    break;
                case 0:
                    cell1.Status = "";
                    break;
                case 1: //up
                    cell1.Status = "DbUp";
                    break;
                default:
                    cell1.Status = "";
                    break;
            }
            return cell1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.item != null)
                return this.item.Count;
            else
                return 0;
        }
        public class ListBestSallItem
        {
            public List<Gabana3.JAM.Dashboard.BestSellingItem> bestSellings;
            static List<Gabana3.JAM.Dashboard.BestSellingItem> builitem;
            public ListBestSallItem(List<Gabana3.JAM.Dashboard.BestSellingItem> bestSellings)
            {
                builitem = bestSellings.OrderByDescending(x => x.totalAmount).ToList();
                this.bestSellings = builitem;
            }
            public int Count
            {

                get
                {
                    if (bestSellings.Count <= 5)
                    {
                        return bestSellings == null ? 0 : bestSellings.Count;
                    }
                    else
                    {
                        return bestSellings == null ? 0 : 5;
                    }
                }
            }
            public Gabana3.JAM.Dashboard.BestSellingItem this[int i]
            {
                get { return bestSellings == null ? null : bestSellings[i]; }
            }
        }
    }
}