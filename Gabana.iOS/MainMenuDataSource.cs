using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;


namespace Gabana.iOS
{
    public class MyDataSource : UICollectionViewDataSource
    {
        public List<Menuitem> Menu;

        public MyDataSource()
        {
            Menu = new List<Menuitem>();
            Menu.Add(new Menuitem(Utils.TextBundle("menupos", "POS"), "POS.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menuitem", "ITEMS"), "Item.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menucustomer", "CUSTOMER"), "Customer.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menuemployee", "EMPLOYEE"), "Employee.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menudashboard", "DASHBOARD"), "Dashboard.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menureport", "REPORT"), "Report.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menusetting", "SETTING"), "Setting.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menubillhis", "BILL HISTORY"), "History.png"));
            Menu.Add(new Menuitem(Utils.TextBundle("menumyqr", "MY QR"), "QRSetting.png"));
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("mainMenuCell", indexPath) as MainMenuCollectionViewCell;
            cell.Name = Menu[(int)indexPath.Row].MenuName;
            cell.Image = Menu[(int)indexPath.Row].MenuIcon;
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Menu.Count;
        }
    }
}