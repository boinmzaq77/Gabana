using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using System.Threading.Tasks;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class MenuPosDataSource : UICollectionViewDataSource
    {
        public List<Category> Menu;
        Gabana.ShareSource.Manage.CategoryManage catagory;
        public MenuPosDataSource(List<Category> menuPos)
        {
            Menu = menuPos;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            POSMenuCollectionViewCell cell = collectionView.DequeueReusableCell("menuPosCell", indexPath) as POSMenuCollectionViewCell;
            
            cell.Name = this.Menu[(int)indexPath.Row].Name;
            
            if(POSController.Select == (int)indexPath.Row)
            {
                cell.ShowSelected(true);
            }
            else
            {
                cell.ShowSelected(false);
            }

            var index = NSIndexPath.FromRowSection(0,0);
            POSMenuCollectionViewCell cell2 = collectionView.DequeueReusableCell("menuPosCell", index) as POSMenuCollectionViewCell;

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.Menu.Count;
        }
        
        public string GetItem(int row)
        {
            return this.Menu[row].Name;
        }
        public string Getcell (int row)
        {
            return this.Menu[row].Name;
        }
    }
}