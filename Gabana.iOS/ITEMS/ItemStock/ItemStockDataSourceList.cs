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
    public class ItemStockDataSourceList : UICollectionViewDataSource
    {
        public List<Item> Menu;
        public List<Category> category;
        public UICollectionView collectionView;
        public CellItemStockList choosecell;
        public List<ItemOnBranch> ItemOnBranch = new List<ItemOnBranch>(); 
        public ItemStockDataSourceList(List<Item> item, List<ItemOnBranch> itemOnbranch,List<Category> cat)
        {
            this.Menu = item;
            this.ItemOnBranch = itemOnbranch;
            this.category = cat;
        }
        public void ReloadData(List<Item> item, List<ItemOnBranch> itemOnbranch, List<Category> cat)
        {
            this.Menu = item;
            this.ItemOnBranch = itemOnbranch;
            this.category = cat;
        }
        public async Task<ItemOnBranch> getStock(int id)
        {
            var stock = await GabanaAPI.GetDataStock((int)DataCashingAll.SysBranchId, id);
            return stock;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            
            this.collectionView = collectionView;
            var cell = collectionView.DequeueReusableCell("itemStockPosCellList", indexPath) as CellItemStockList;

            var item = this.Menu[(int)indexPath.Row];
            if (choosecell != null)
            {
                if (choosecell == cell)
                {
                    choosecell = null;
                }
            }
            cell.Name = item.ItemName;
            //cell.OnItemSwipe -= Cell_OnItemSwipe;
            //cell.OnItemSwipe += Cell_OnItemSwipe;
            cell.OnFavItem -= Cell_OnFavItem;
            cell.OnFavItem += Cell_OnFavItem;
            cell.OnDeleteItem -= Cell_OnDeleteItem;
            cell.OnDeleteItem += Cell_OnDeleteItem;
            if (this.category != null)
            {
                var cat = this.category.Where(x => x.SysCategoryID == item.SysCategoryID).FirstOrDefault();
                if (cat == null)
                {
                    cell.Category = "";
                }
                else
                {
                    cell.Category = cat.Name ?? "";
                }
            }
            
            cell.SysID = item.SysItemID.ToString();
            cell.ShortName = "";
            cell.Colors = 0;

            if (ItemsController.checknet)
            {
                if (string.IsNullOrEmpty(this.Menu[(int)indexPath.Row].ThumbnailLocalPath))
                {
                    if (string.IsNullOrEmpty(this.Menu[(int)indexPath.Row].PicturePath))
                    {
                        cell.Imageghavenet = null;
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
                    }
                    else
                    {
                        cell.ShortName = null;
                        cell.Imageghavenet = this.Menu[(int)indexPath.Row];
                    }
                }
                else
                {
                    cell.ShortName = null;
                    cell.Imageghavenet = this.Menu[(int)indexPath.Row];
                }
            }
            else
            {
                if (string.IsNullOrEmpty(this.Menu[(int)indexPath.Row].ThumbnailLocalPath))
                {
                    cell.Imagegnothavenet = null;
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
                }
                else
                {
                    cell.ShortName = null;
                    cell.Imagegnothavenet = this.Menu[(int)indexPath.Row];
                }
            }
            if (item.FTrackStock == 1)
            {
                var stock = ItemOnBranch.Where(x => x.SysItemID == item.SysItemID).FirstOrDefault(); // (int)indexPath.Row];
                
                if (stock == null)
                {
                    cell.Stock = 0;
                    cell.Cost = "0";
                }
                else
                {
                    cell.Cost = stock.BalanceStock.ToString();
                    if (stock.BalanceStock > stock.MinimumStock)
                    {
                        cell.Stock = 0;
                    }
                    else if (stock.BalanceStock <= stock.MinimumStock & stock.BalanceStock > 0)
                    {
                        cell.Stock = 1;
                    }
                    else
                    {
                        cell.Stock = 2;
                    }
                }
            }
            else
            {
                cell.Stock = 0;
            }
            OnScroll2?.Invoke();
            return cell;
        }

        private void Cell_OnDeleteItem(CellItemStockList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }

        private void Cell_OnFavItem(CellItemStockList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellFav?.Invoke(indexPathQRcode);
        }

        private void Cell_OnItemSwipe(CellItemStockList indexPath)
        {
             
            if (choosecell != null)
            {
                UIView.Animate(0.7, () =>
                {
                    var frame2 = choosecell.Frame;
                    frame2.X = 0;
                    choosecell.showbtndelete = false;
                    choosecell.Frame = frame2;
                });
            };
            choosecell = indexPath;
            UIView.Animate(0.7, () =>
            {
                var frame = indexPath.Frame;
                frame.X = -80;
                choosecell.showbtndelete = true;
                indexPath.Frame = frame;
            });

             
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.Menu != null)
                return this.Menu.Count;
            else
                return 0;
        }

        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;

        public delegate void CardCellFav(NSIndexPath indexPath);
        public event CardCellFav OnCardCellFav;

        public delegate void Scroll2Cell();
        public event Scroll2Cell OnScroll2;
    }
}