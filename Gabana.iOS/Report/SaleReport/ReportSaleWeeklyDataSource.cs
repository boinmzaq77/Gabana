﻿using Foundation;
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
    public class ReportSaleWeeklyDataSource : UICollectionViewDataSource
    {
        public List<ReportWeekly> reportSale;

        public ReportSaleWeeklyDataSource(List<ReportWeekly> cus)
        {
            this.reportSale = cus;
            //lstCustomer
        }

        public void ReloadData(List<ReportWeekly> cus)
        {
            this.reportSale = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("HourSaleViewCell", indexPath) as HourSaleViewCell;
            cell.Color = false;
            cell.Time = this.reportSale[(int)indexPath.Row].Weeklyname;
            cell.Total = "00.00";
            if (this.reportSale[(int)indexPath.Row].value != 0 && this.reportSale[(int)indexPath.Row].value != null)
            {
                cell.Total = this.reportSale[(int)indexPath.Row].value.ToString("#,##0.00");
            }
            cell.Color = false;
            if (indexPath.Row % 2 == 1)
            {
                cell.Color = true;
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.reportSale.Count;
        }
    }
}