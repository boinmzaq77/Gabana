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
using static Gabana.iOS.CustomerController;

namespace Gabana.iOS
{
    public class CategoryDataSource : UICollectionViewDataSource
    {
        public int merchantID;
        private List<Item> listItem = new List<Item>();
        private List<Category> listChooseCategory = new List<Category>();
        ListCategory listCategory;

        public CategoryDataSource(ListCategory listCategory, List<ORM.MerchantDB.Item> AllCategory, List<Category> ChooseCategory)
        {
            this.listCategory = listCategory;
            this.listItem = AllCategory;
            this.listChooseCategory = ChooseCategory;
        }

        public void ReloadData(ListCategory listCategory, List<ORM.MerchantDB.Item> AllCategory, List<Category> ChooseCategory)
        {
            this.listCategory = listCategory;
            this.listItem = AllCategory;
            this.listChooseCategory = ChooseCategory;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ReportChooseCategoryViewCell", indexPath) as ReportChooseCategoryViewCell;
            cell.Name = this.listCategory[(int)indexPath.Row].Name?.ToString();
            var categoryid = listCategory[(int)indexPath.Row].SysCategoryID.ToString();
            List<Item> itemcategory;

            cell.status = false;
            if(listItem != null)
            {
                itemcategory = listItem.Where(x => x.SysCategoryID.ToString() == categoryid).ToList();
                var Totalitem = itemcategory.Count.ToString();
                if (itemcategory.Count <= 1)
                {
                    cell.Item = Totalitem +" "+Utils.TextBundle("item", "Items");

                }
                else
                {
                    cell.Item = Totalitem +" "+Utils.TextBundle("item", "Items");
                }
            }
            else
            {
                itemcategory = null;
                cell.Item  = "0 "+Utils.TextBundle("item", "Items");
            }
            var index = listChooseCategory.FindIndex(x => x.SysCategoryID == listCategory[(int)indexPath.Row].SysCategoryID);
            if (index == -1)
            {
                cell.status = false;
            }
            else
            {
                cell.status = true;
            }


            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.listCategory.Count;
        }
    }
}