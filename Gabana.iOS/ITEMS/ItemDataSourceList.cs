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
    public class ItemDataSourceList : UICollectionViewDataSource
    {
        public List<Item> Menu;
        public List<Category> category;
        public UICollectionView collectionView;
        public CellItemList choosecell;
        public List<ItemOnBranch> ItemOnBranch; 
        public ItemDataSourceList(List<Item> item, List<ItemOnBranch> itemOnbranch,List<Category> cat)
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
            var cell = collectionView.DequeueReusableCell("itemPosCellList", indexPath) as CellItemList;
            if (choosecell != null)
            {
                if (choosecell == cell)
                {
                    choosecell = null;
                }
            }
            cell.Cost = Utils.DisplayDecimal(this.Menu[(int)indexPath.Row].Price); // this.Menu[(int)indexPath.Row].Price.ToString("N2");
            cell.Name = this.Menu[(int)indexPath.Row].ItemName;
            cell.Fav = (int)this.Menu[(int)indexPath.Row].FavoriteNo;
            cell.OnItemSwipe -= Cell_OnItemSwipe;
            cell.OnItemSwipe += Cell_OnItemSwipe;
            cell.OnFavItem -= Cell_OnFavItem;
            cell.OnFavItem += Cell_OnFavItem;
            cell.OnItem -= Cell_OnItem;
            cell.OnItem += Cell_OnItem;
            cell.OnDeleteItem -= Cell_OnDeleteItem;
            cell.OnDeleteItem += Cell_OnDeleteItem;
            if (this.category != null)
            {
                var cat = this.category.Where(x => x.SysCategoryID == this.Menu[(int)indexPath.Row].SysCategoryID).FirstOrDefault();
                if (cat == null)
                {
                    cell.Category = "";
                }
                else
                {
                    cell.Category = cat.Name ?? "";
                }
            }
            
            cell.SysID = this.Menu[(int)indexPath.Row].SysItemID.ToString();
            cell.ShortName = "";
            cell.Colors = 0;
            //cell.Image = null;
            //cell.Image = this.Menu[(int)indexPath.Row];
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
            //if ((string.IsNullOrEmpty( this.Menu[(int)indexPath.Row].PicturePath) && ItemsController.checknet) || (string.IsNullOrEmpty(this.Menu[(int)indexPath.Row].ThumbnailLocalPath) && !ItemsController.checknet)   )
            //{
                
            //    cell.Image = null;
            //    if (this.Menu[(int)indexPath.Row].Colors != null)
            //    {
            //        cell.Colors = (long)this.Menu[(int)indexPath.Row].Colors;
            //        cell.ShortName = this.Menu[(int)indexPath.Row].ShortName;
            //    }
            //    else
            //    {
            //        cell.Colors = 0;
            //        cell.ShortName = null;
            //    }
            //}
            //else
            //{
            //        cell.ShortName = null;
            //        cell.Image = this.Menu[(int)indexPath.Row];
            //}
               
            if (this.Menu[(int)indexPath.Row].FTrackStock == 1)
            {
               var stock = this.ItemOnBranch.Where(x=>x.SysItemID == this.Menu[(int)indexPath.Row].SysItemID).FirstOrDefault();

                if (stock == null)
                {
                    cell.Stock = 0;
                }
                else
                {
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
            OnScroll?.Invoke();
            return cell;
        }

        private void Cell_OnDeleteItem(CellItemList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }

        private void Cell_OnFavItem(CellItemList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellFav?.Invoke(indexPathQRcode);
        }

        private void Cell_OnItem(CellItemList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCell?.Invoke(indexPathQRcode);
        }

        private void Cell_OnItemSwipe(CellItemList indexPath)
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

        public delegate void CardCell(NSIndexPath indexPath);
        public event CardCell OnCardCell;

        public delegate void ScrollCell();
        public event ScrollCell OnScroll;

    }
}