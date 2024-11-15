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
using System.IO;

namespace Gabana.iOS
{
    public class POSCustomerDataSource : UICollectionViewDataSource
    {
        //public List<Gabana.ORM.MerchantDB.Branch> branches;
        CustomerManage setCustomer = new CustomerManage();
        public int merchantID;
        private List<Gabana.ORM.MerchantDB.Customer> customers;

        public POSCustomerDataSource(List<ORM.MerchantDB.Customer> cus)
        {
            this.customers = cus;
           
        }
        public void ReloadData(List<Customer> cus)
        {
            this.customers = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("posCustomerCollectionViewCell", indexPath) as POSCustomerCollectionViewCell;
            cell.Name = this.customers[(int)indexPath.Row].CustomerName;
            if (this.customers[(int)indexPath.Row].ThumbnailLocalPath == null || this.customers[(int)indexPath.Row].ThumbnailLocalPath == "")
            {
                cell.Image = "defaultcust";
            }
            else
            {
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                cell.Image = Path.Combine(docFolder, this.customers[(int)indexPath.Row].ThumbnailLocalPath);
            }
            if (POSCustomerController.SelectedCustomer!= null)
            {
                if(POSCustomerController.SelectedCustomer.SysCustomerID == this.customers[(int)indexPath.Row].SysCustomerID)
                {
                    cell.selectCheck = true;
                }
                else
                {
                    cell.selectCheck = false;
                }
            }
            else
            {
                cell.selectCheck = false;
            }
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.customers.Count;
        }
    }
}