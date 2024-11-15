using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;


namespace Gabana.iOS
{
    public class MenuAddItemDataSource : UICollectionViewDataSource
    {
        public List<MenuitemHeader> Menu;

        public MenuAddItemDataSource()
        {
            Menu = new List<MenuitemHeader>();
            Menu.Add(new MenuitemHeader(0,"Item"));
            Menu.Add(new MenuitemHeader(1,"Stock"));
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            MenuCollectionViewCell cell = collectionView.DequeueReusableCell("menuItemCell", indexPath) as MenuCollectionViewCell;
            cell.Name = Menu[(int)indexPath.Row].MenuName;
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Menu.Count;
        }

        public string GetItem(int row)
        {
            return Menu[row].MenuName;
        }
    }
}