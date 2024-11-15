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
using System.IO;

namespace Gabana.iOS
{
    public class ItemToppingDataSourceList : UICollectionViewDataSource
    {
        public List<Gabana.ORM.MerchantDB.Item> topping;
        public List<Category> category;
        public List<ItemOnBranch> ItemOnBranch;
        public UICollectionView collectionView;
        CategoryManage catManage = new CategoryManage();
        ItemOnBranchManage itemOnBranchManage = new ItemOnBranchManage();
        public CellItemToppingList choosecell;
        public ItemToppingDataSourceList(List<Item> Top, List<ItemOnBranch> itemOnbranch, List<Category> cat)
       {
            this.topping = Top;
            this.ItemOnBranch = itemOnbranch;
            this.category = cat;
        }
        public void ReloadData(List<Item> item,List<ItemOnBranch> itemOnbranch, List<Category> cat)
        {
            this.topping = item;
            this.ItemOnBranch = itemOnbranch;
            this.category = cat;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.collectionView = collectionView;
            var cell1 = collectionView.DequeueReusableCell("cellItemToppingList", indexPath) as CellItemToppingList;
            cell1.Cost = Utils.DisplayDecimal(this.topping[(int)indexPath.Row].Price);
            cell1.Name = this.topping[(int)indexPath.Row].ItemName;
            cell1.Fav = (int)this.topping[(int)indexPath.Row].FavoriteNo;
            if(this.category != null)
            {
                var cat = this.category.Where(x => x.SysCategoryID == this.topping[(int)indexPath.Row].SysCategoryID).FirstOrDefault();
                if (cat == null)
                {
                    cell1.Category = "";
                }
                else
                {
                    cell1.Category = cat.Name ?? "";
                }
            }
            cell1.SysID = this.topping[(int)indexPath.Row].SysItemID.ToString();
            cell1.ShortName = "";
            cell1.Colors = 0;
            cell1.Image = null;
            if (this.topping[(int)indexPath.Row].ThumbnailLocalPath == null || this.topping[(int)indexPath.Row].ThumbnailLocalPath == "")
            {

                cell1.Image = null;
                if (this.topping[(int)indexPath.Row].Colors != null)
                {
                    cell1.Colors = (long)this.topping[(int)indexPath.Row].Colors;
                    cell1.ShortName = this.topping[(int)indexPath.Row].ShortName;
                }
                else
                {
                    cell1.Colors = 0;
                    cell1.ShortName = null;
                }
            }
            else
            {
                cell1.ShortName = null;
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                cell1.Image = Path.Combine(docFolder, this.topping[(int)indexPath.Row].ThumbnailLocalPath);
            }
            #region Aleart Stock
            //กำหนด MinimumStock ระบบจะแสดงเป็น Indicater(วงกลม Notification) สีเหลืองเมื่อต่ำกว่า MinimumStock
            //สีแดง สินค้า =0 หรือติดลบ
            if (this.topping[(int)indexPath.Row].FTrackStock == 1)
            {
                var stock = this.ItemOnBranch.Where(x => x.SysItemID == this.topping[(int)indexPath.Row].SysItemID).FirstOrDefault();
                if (stock != null)
                {
                    if (stock.BalanceStock > stock.MinimumStock)
                    {
                        cell1.setStock(0);
                    }
                    else if (stock.BalanceStock <= stock.MinimumStock & stock.BalanceStock > 0)
                    {
                        cell1.setStock(1);
                    }
                    else
                    {
                        cell1.setStock(2);
                    }
                }
                else
                {
                    cell1.setStock(0);
                }
            }
            else
            {
                cell1.setStock(0);
            }
            #endregion
            cell1.OnItemSwipe -= Cell_OnItemSwipe;
            cell1.OnItemSwipe += Cell_OnItemSwipe;
            cell1.OnDeleteItem -= Cell_OnDeleteItem;
            cell1.OnDeleteItem += Cell_OnDeleteItem;
            cell1.OnFavItem -= Cell_OnFavItem;
            cell1.OnFavItem += Cell_OnFavItem;
            OnScroll2?.Invoke();
            return cell1;
        }

        private void Cell_OnDeleteItem(CellItemToppingList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }
        private void Cell_OnFavItem(CellItemToppingList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellFav?.Invoke(indexPathQRcode);
        }
        private void Cell_OnItemSwipe(CellItemToppingList indexPath)
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
            if (this.topping != null)
                return this.topping.Count;
            else
                return 0;
        }
        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;

        public delegate void CardCellFav(NSIndexPath indexPath);
        public event CardCellFav OnCardCellFav;

        public delegate void ScrollCell2();
        public event ScrollCell2 OnScroll2;
    }
}