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
using Gabana3.JAM.Report;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class ReportCustomerDataSource : UICollectionViewDataSource
    {
        List<Gabana3.JAM.Report.SalesByCustomerResponse> customerResponses = new List<Gabana3.JAM.Report.SalesByCustomerResponse>();
        List<ORM.MerchantDB.Customer> customers;

        public ReportCustomerDataSource(List<Gabana3.JAM.Report.SalesByCustomerResponse> cus,List<Customer> listCustomer)
        {
            this.customerResponses = cus;
            customers = listCustomer;
            //lstCustomer
        }

        public void ReloadData(List<Gabana3.JAM.Report.SalesByCustomerResponse> cus)
        {
            this.customerResponses = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("CustomerReportDataViewCell", indexPath) as CustomerReportDataViewCell;
            cell.Name = this.customerResponses[(int)indexPath.Row].customerName;
            cell.Total = this.customerResponses[(int)indexPath.Row].sumTotalAmount.ToString("#,##0.00");

            var customer = customers.Where(x => x.CustomerName == customerResponses[(int)indexPath.Row].customerName).FirstOrDefault();
            
            if (customer != null)
            {
                var path = customer.PicturePath;
                if (!string.IsNullOrEmpty(path))
                {
                    cell.Image = customer.PictureLocalPath;
                    if (DataCashingAll.CheckConnectInternet)
                    {
                        cell.Image = path;
                    }
                }
                else
                {
                    cell.Image = "defaultcust";
                }
            }
            else
            {
                cell.Image = "defaultcust";
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.customerResponses.Count;
        }
    }
}