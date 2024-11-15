using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;


namespace Gabana.iOS
{
    public class MenuItemDataSource : UICollectionViewDataSource
    {
        public List<MenuitemHeaderIOS> Menu;

        public MenuItemDataSource(List<MenuitemHeaderIOS> m )
        {
            this.Menu = m;

        }

        public void ReloadData(List<MenuitemHeaderIOS> all)
        {
            this.Menu = all;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            MenuCollectionViewCell cell = collectionView.DequeueReusableCell("MenuCollectionViewCell", indexPath) as MenuCollectionViewCell;
            cell.Name = Menu[(int)indexPath.Row].MenuName;
            cell.ShowSelected(Menu[(int)indexPath.Row].select);

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