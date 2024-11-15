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
using static LinqToDB.Reflection.Methods.LinqToDB;
using LinqToDB.SqlQuery;
using Xamarin.Essentials;
using System.Text.RegularExpressions;

namespace Gabana.iOS
{
    public class ItemPosDataSource : UICollectionViewDataSource
    {
        public List<Item> item;
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        ItemManage itemmanager = new ItemManage();
        
        public ItemPosDataSource(List<Item> item) 
        {
            this.item = item;
        }
        public void ReloadData(List<Item> item)
        {
            this.item = item;
        }
        public void ReloadData()
        {
          
        }


        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("itemPosCell", indexPath) as ItemPOSCollectionViewCell;
            

            if (item[(int)indexPath.Row].SysItemID != -1)
            {
                //cell.Image = item[(int)indexPath.Row].PicturePath;
                cell.Cost = CURRENCYSYMBOLS + Utils.DisplayDecimal(item[(int)indexPath.Row].Price);
                var Item = POSController.tranWithDetails?.tranDetailItemWithToppings.ToList().Where(x => x.tranDetailItem.SysItemID == item[(int)indexPath.Row].SysItemID).FirstOrDefault();
                if (Item != null)
                {
                    cell.Name = (int)Item.tranDetailItem.Quantity  + "x " + item[(int)indexPath.Row].ItemName;

                }
                else
                {
                    cell.Name = item[(int)indexPath.Row].ItemName;
                }

                if (item[(int)indexPath.Row].Colors != null)
                {
                    cell.Colors = (long)item[(int)indexPath.Row].Colors;
                    cell.ShortName = item[(int)indexPath.Row].ShortName;
                }
                else
                {
                    cell.Colors = 0;
                    cell.ShortName = null;
                }
                cell.Image = null;

                if (MainController.checknet)
                {
                    if (string.IsNullOrEmpty(this.item[(int)indexPath.Row].ThumbnailLocalPath))
                    {
                        if (string.IsNullOrEmpty(this.item[(int)indexPath.Row].PicturePath))
                        {
                            cell.Imageghavenet = null;
                            if (this.item[(int)indexPath.Row].Colors != null)
                            {
                                cell.Colors = (long)this.item[(int)indexPath.Row].Colors;
                                cell.ShortName = this.item[(int)indexPath.Row].ShortName;
                            }
                            else
                            {
                                cell.Colors = 0;
                                cell.ShortName = null;
                            }
                        }
                        else
                        {
                            cell.ShortName = null;
                            cell.Imageghavenet = this.item[(int)indexPath.Row];
                            cell.Colors = int.Parse("A2A2A2", System.Globalization.NumberStyles.HexNumber);
                        }
                    }
                    else
                    {
                        cell.ShortName = null;
                        cell.Imageghavenet = this.item[(int)indexPath.Row];
                        cell.Colors = int.Parse("A2A2A2", System.Globalization.NumberStyles.HexNumber);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(this.item[(int)indexPath.Row].ThumbnailLocalPath))
                    {
                        cell.Imagegnothavenet = null;
                        if (this.item[(int)indexPath.Row].Colors != null)
                        {
                            cell.Colors = (long)this.item[(int)indexPath.Row].Colors;
                            cell.ShortName = this.item[(int)indexPath.Row].ShortName;
                        }
                        else
                        {
                            cell.Colors = 0;
                            cell.ShortName = null;
                        }
                    }
                    else
                    {
                        cell.ShortName = null;
                        cell.Imagegnothavenet = this.item[(int)indexPath.Row];
                        cell.Colors = int.Parse("A2A2A2", System.Globalization.NumberStyles.HexNumber);
                    }
                }

                //if (this.item[(int)indexPath.Row].ThumbnailLocalPath == null || this.item[(int)indexPath.Row].ThumbnailLocalPath == "")
                //{

                //    cell.Image = null;
                //    if (this.item[(int)indexPath.Row].Colors != null)
                //    {
                //        cell.Colors = (long)this.item[(int)indexPath.Row].Colors;
                //        cell.ShortName = this.item[(int)indexPath.Row].ShortName;
                //    }
                //    else
                //    {
                //        cell.Colors = 0;
                //        cell.ShortName = null;
                //    }
                //}
                //else
                //{
                //    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //    cell.Image = Path.Combine(docFolder, this.item[(int)indexPath.Row].ThumbnailLocalPath);
                //    cell.Colors = int.Parse("A2A2A2", System.Globalization.NumberStyles.HexNumber);
                //}
            }
            else // add button
            {

                cell.Image = "AddItem";
                cell.Cost = null;
                cell.Name = null;
                cell.Colors = -1;
                cell.ShortName = null;
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return item.Count;
        }
    }
}