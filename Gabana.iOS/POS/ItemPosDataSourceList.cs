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
using Gabana.ShareSource;
using System.IO;

namespace Gabana.iOS
{
    public class ItemPosDataSourceList : UICollectionViewDataSource
    {
        public List<Item> Menu;
        CategoryManage catManage = new CategoryManage();
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        public ItemPosDataSourceList(List<Item> item)
        {
            this.Menu = item;
        }
        public void ReloadData(List<Item> item)
        {
            this.Menu = item;
        }
        public async Task<Category> getCat(int index)
        {
            var result = await catManage.GetCategory(Convert.ToInt32(MainController.merchantlocal.MerchantID), (int)this.Menu[index].SysCategoryID);
            return result;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell = new UICollectionViewCell();
            
            if ((int)indexPath.Row != ((Menu.Count) - 1))
            {
                var cell2 = collectionView.DequeueReusableCell("itemPosCellList", indexPath) as ItemPOSCollectionViewCellList;
                //cell2.Image = Menu[(int)indexPath.Row].PicturePath;
                cell2.Cost = CURRENCYSYMBOLS + Utils.DisplayDecimal(Menu[(int)indexPath.Row].Price);
                cell2.Name = Menu[(int)indexPath.Row].ItemName;
                cell2.ShortName = null;
                if (this.Menu[(int)indexPath.Row].ThumbnailLocalPath == null || this.Menu[(int)indexPath.Row].ThumbnailLocalPath == "")
                {

                    cell2.Image = null;
                    if (this.Menu[(int)indexPath.Row].Colors != null)
                    {
                        cell2.Colors = (long)this.Menu[(int)indexPath.Row].Colors;
                        cell2.ShortName = this.Menu[(int)indexPath.Row].ShortName;
                    }
                    else
                    {
                        cell2.Colors = 0;
                        cell2.ShortName = null;
                    }
                }
                else
                {
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    cell2.Image = this.Menu[(int)indexPath.Row];
                    cell2.Colors = int.Parse("A2A2A2", System.Globalization.NumberStyles.HexNumber);
                }
                cell2.Image = null;
                //cell.Image = this.Menu[(int)indexPath.Row];
                if ((string.IsNullOrEmpty(this.Menu[(int)indexPath.Row].PicturePath) && ItemsController.checknet) || (string.IsNullOrEmpty(this.Menu[(int)indexPath.Row].ThumbnailLocalPath) && !ItemsController.checknet))
                {

                    cell2.Image = null;
                    if (this.Menu[(int)indexPath.Row].Colors != null)
                    {
                        cell2.Colors = (long)this.Menu[(int)indexPath.Row].Colors;
                        cell2.ShortName = this.Menu[(int)indexPath.Row].ShortName;
                    }
                    else
                    {
                        cell2.Colors = 0;
                        cell2.ShortName = null;
                    }
                }
                else
                {


                    cell2.ShortName = null;
                    cell2.Image = this.Menu[(int)indexPath.Row];

                }
                cell = cell2;
            }
            else // add button
            {

                var cell3 = collectionView.DequeueReusableCell("ItemPOSCollectionViewCellListAdd", indexPath) as ItemPOSCollectionViewCellListAdd;
                cell3.Image = "AddItem";
                cell3.Cost = null;
                cell3.Name = null;
                //cell3.Colors = -1;
                cell3.ShortName = null;
                cell = cell3;
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