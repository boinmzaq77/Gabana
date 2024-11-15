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
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class CustomerReportSelectDataSource : UICollectionViewDataSource
    {
        ListCustomer listCustomer = null;

        public CustomerReportSelectDataSource(ListCustomer listCategory)
        {
            this.listCustomer = listCategory;
        }

        public void ReloadData(ListCustomer listCategory)
        {
            this.listCustomer = listCategory;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ReportChooseCustomerViewCell", indexPath) as ReportChooseCustomerViewCell;
            cell.Name = this.listCustomer[(int)indexPath.Row].CustomerName?.ToString();
            cell.status = false;
            var path = this.listCustomer[(int)indexPath.Row].PicturePath;
            if (!string.IsNullOrEmpty(path))
            {
                cell.Image = this.listCustomer[(int)indexPath.Row].PictureLocalPath;
                if (DataCashingAll.CheckConnectInternet)
                {
                    cell.Image = path;
                }
            }
            else
            {
                cell.Image = "defaultcust";
            }
            var index = ReportSelectCustomerController.listChooseCustomer.FindIndex(x => x.SysCustomerID == listCustomer[(int)indexPath.Row].SysCustomerID);
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
            return this.listCustomer.Count;
        }
    }
}