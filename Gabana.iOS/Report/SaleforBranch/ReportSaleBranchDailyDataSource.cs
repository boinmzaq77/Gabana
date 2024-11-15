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
    public class ReportSaleBranchDailyDataSource : UICollectionViewDataSource
    {
       
        private List<SaleReportBranch> reportSale1;

        public ReportSaleBranchDailyDataSource(List<SaleReportBranch> reportSale1)
        {
            this.reportSale1 = reportSale1;
        }

        public void ReloadData(List<SaleReportBranch> reportSale1)
        {
            this.reportSale1 = reportSale1;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("HourSaleBranchViewCell", indexPath) as HourSaleBranchViewCell;
            cell.Time = this.reportSale1[(int)indexPath.Row].BranchName;
            cell.Total = "00.00";
            if (this.reportSale1[(int)indexPath.Row].sumGrandTotal != 0 && this.reportSale1[(int)indexPath.Row].sumGrandTotal != null)
            {
                cell.Total = this.reportSale1[(int)indexPath.Row].sumGrandTotal.ToString("#,##0.00");
            }
            
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.reportSale1.Count;
        }
    }
}