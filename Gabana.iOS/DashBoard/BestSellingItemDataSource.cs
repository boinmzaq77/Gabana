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
using System.IO;
using TinyInsightsLib;

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
            try
            {

            
                var cell1 = collectionView.DequeueReusableCell("BestSellingItemViewCell", indexPath) as BestSellingItemViewCell;

                var itemsearch = this.itemAll.Where(x => x.ItemName == this.item[(int)indexPath.Row].itemName).FirstOrDefault();
                if (itemsearch != null)
                {


                    var paths = itemsearch.PicturePath;
                    cell1.ShortName = "";

                    if (itemsearch.ThumbnailLocalPath == null || itemsearch.ThumbnailLocalPath == "")
                    {

                        cell1.Image = null;
                        if (itemsearch.Colors != null)
                        {
                            cell1.Colors = (long)itemsearch.Colors;
                            cell1.ShortName = itemsearch.ShortName;
                        }
                        else
                        {
                            cell1.Colors = 0;
                            cell1.ShortName = null;
                        }
                    }
                    else
                    {
                        var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        cell1.Image = Path.Combine(docFolder, itemsearch.ThumbnailLocalPath);
                    }
                }
                else
                {
                    cell1.Image = null;
                    cell1.Colors = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
                    if (this.item[(int)indexPath.Row].itemName.Length > 5 )
                    {
                        cell1.ShortName = this.item[(int)indexPath.Row].itemName?.Substring(0, 5);
                    }
                    else
                    {
                        cell1.ShortName = this.item[(int)indexPath.Row].itemName;
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
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return null;
            }
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