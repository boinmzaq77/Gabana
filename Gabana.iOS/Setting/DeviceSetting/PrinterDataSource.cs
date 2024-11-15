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
    public class PrinterDataSource : UICollectionViewDataSource
    {
        private List<Bluetooth2> Printer = new List<Bluetooth2>();

        public PrinterDataSource(List<Bluetooth2> cus)
        {
            this.Printer = cus;
        }

        public void ReloadData(List<Bluetooth2> cus)
        {
            this.Printer = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("BluetoothViewCell", indexPath) as BluetoothViewCell;
            cell.Name = this.Printer[(int)indexPath.Row].BluetoothName;
            if(this.Printer[(int)indexPath.Row].BluetoothStatus == "on")
            {
                cell.Image = "BluetoothB.png";
                cell.Status = true;
            }
            else
            {
                cell.Image = "BluetoothG.png";
                cell.Status = false;
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.Printer.Count;
        }
    }
}