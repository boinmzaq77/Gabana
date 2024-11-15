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
    public class ReportCategoryDataSource : UICollectionViewDataSource
    {
        List<Gabana3.JAM.Report.SalesByCategoryResponse> categoryResponses = new List<Gabana3.JAM.Report.SalesByCategoryResponse>();


        public ReportCategoryDataSource(List<Gabana3.JAM.Report.SalesByCategoryResponse> cus)
        {
            this.categoryResponses = cus;
            //lstCustomer
        }

        public void ReloadData(List<Gabana3.JAM.Report.SalesByCategoryResponse> cus)
        {
            this.categoryResponses = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("HourSaleViewCell", indexPath) as HourSaleViewCell;
            cell.Time = this.categoryResponses[(int)indexPath.Row].categoryName;
            cell.Total = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(this.categoryResponses[(int)indexPath.Row].sumTotalAmount);
            if (this.categoryResponses[(int)indexPath.Row].sumTotalAmount == 0)
            {
                cell.Total = "00.00";
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.categoryResponses.Count;
        }
    }
}