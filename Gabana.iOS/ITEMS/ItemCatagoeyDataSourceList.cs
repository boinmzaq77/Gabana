using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.ORM.MerchantDB;
using Gabana.Model;
using System.Threading.Tasks;

namespace Gabana.iOS
{
    public class ItemCatagoeyDataSourceList : UICollectionViewDataSource
    {
        public List<Gabana.ORM.MerchantDB.Category> Categories;
        public itemCatagoryCollectionViewCellList choosecell;
        public UICollectionView collectionView;
        List<Item> itemcategory;

        public ItemCatagoeyDataSourceList(List<Category> item)
        {
            this.Categories = item;
        }
        public void ReloadData(List<Category> item)
        {
            this.Categories = item;
        }
        

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.collectionView = collectionView;
            var cell1 = collectionView.DequeueReusableCell("itemCatagoryCellList", indexPath) as itemCatagoryCollectionViewCellList;
            if (choosecell != null)
            {
                if (choosecell == cell1)
                {
                    choosecell = null;
                }
            }
            cell1.OnItemSwipe -= Cell_OnItemSwipe;
            cell1.OnItemSwipe += Cell_OnItemSwipe;
            cell1.OnDeleteItem -= Cell_OnDeleteItem;
            cell1.OnDeleteItem += Cell_OnDeleteItem;
            cell1.Name = this.Categories[(int)indexPath.Row].Name;

            itemcategory = ItemsController.Items.Where(x => x.SysCategoryID.ToString() == this.Categories[(int)indexPath.Row].SysCategoryID.ToString()).ToList();
            var itemcategory2 = ItemsController.Topping.Where(x => x.SysCategoryID.ToString() == this.Categories[(int)indexPath.Row].SysCategoryID.ToString()).ToList();
            var Totalitem = (itemcategory.Count + itemcategory2.Count).ToString();
            if(itemcategory.Count + itemcategory2.Count > 0)
            {
                cell1.Sum = Totalitem + " items";
            }
            else
            {
                cell1.Sum = "0 item";
            }
            OnScroll?.Invoke();
            return cell1;
 
           
        }
        private void Cell_OnDeleteItem(itemCatagoryCollectionViewCellList indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }

        private void Cell_OnItemSwipe(itemCatagoryCollectionViewCellList indexPath)
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
            if (this.Categories != null)
                return this.Categories.Count;
            else
                return 0;
        }
        
        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;
        public delegate void ScrollCell();
        public event ScrollCell OnScroll;
    }
}