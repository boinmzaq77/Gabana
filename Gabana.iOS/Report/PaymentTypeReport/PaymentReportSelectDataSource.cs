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
    public class PaymentReportSelectDataSource : UICollectionViewDataSource
    {
        ListPaymentType listPayment = null;

        public PaymentReportSelectDataSource(ListPaymentType listCategory)
        {
            this.listPayment = listCategory;
        }

        public void ReloadData(ListPaymentType listCategory)
        {
            this.listPayment = listCategory;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            // 1 = PaymentCash.png,2 = PaymentCredit.png,3 = PaymentGiftVoucher.png,4 = PaymentQr.png
            var cell = collectionView.DequeueReusableCell("ReportChooseEmployeeViewCell", indexPath) as ReportChooseEmployeeViewCell;

            cell.Name = listPayment[(int)indexPath.Row].Detail;
            var index = ReportSelectPaymentController.listChoosePayment.FindIndex(x => x.Type == listPayment[(int)indexPath.Row].Type);
            if (index == -1)
            {
                cell.status = false;
            }
            else
            {
                cell.status = true;
            }
            switch (listPayment[(int)indexPath.Row].Logo)
            {
                case 1:
                    cell.Image = "PaymentCash.png";
                    break;
                case 2:
                    cell.Image = "PaymentCredit.png";
                    break;
                case 3:
                    cell.Image = "PaymentGiftVoucher.png";
                    break;
                case 4:
                    cell.Image = "PaymentQr.png";
                    break;
                default:
                    break;
            }


            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.listPayment.Count;
        }
    }
}