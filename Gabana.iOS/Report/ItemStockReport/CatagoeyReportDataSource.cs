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
    public class CatagoeyReportDataSource : UICollectionViewDataSource
    {
        public List<Gabana.ORM.MerchantDB.Category> Categories;
        public ListCategory listCategory;
        bool flag = false;
        public List<Item> listItem;
        private List<Category> listChooseCategory;

        public CatagoeyReportDataSource(ListCategory l, List<Item> item, List<Category> c)
        {
            listCategory = l;
            listItem = item;
            listChooseCategory = c;
        }
        public void ReloadData(List<Category> item)
        {
            this.listChooseCategory = item;
        }
        

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell1 = collectionView.DequeueReusableCell("CatagoryReportViewCell", indexPath) as CatagoryReportViewCell;
          
            cell1.Name = this.listCategory[(int)indexPath.Row].Name.ToString();
            var categoryid = listCategory[(int)indexPath.Row].SysCategoryID.ToString();

            List<Item> itemcategory;

            if (listItem != null)
            {
                itemcategory = listItem.Where(x => x.SysCategoryID.ToString() == categoryid).ToList();
                var Totalitem = itemcategory.Count.ToString();
                if (itemcategory.Count <= 1)
                {
                    cell1.Sum = Totalitem + " "+Utils.TextBundle("item", "Items");

                }
                else
                {
                    cell1.Sum = Totalitem + " " + Utils.TextBundle("item", "Items");
                }
            }
            else
            {
                itemcategory = null;
                cell1.Sum = 0 + " "+ Utils.TextBundle("item", "Items");
            }

            var index = listChooseCategory.FindIndex(x => x.SysCategoryID == listCategory[(int)indexPath.Row].SysCategoryID);
            if (index == -1)
            {
                cell1.Select = false;
            }
            else
            {
                cell1.Select = true;
            }

            return cell1;
 
           
        }
      
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.listCategory != null)
                return this.listCategory.Count;
            else
                return 0;
        }
        
    }
}