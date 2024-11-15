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
using static Gabana.iOS.BillHistoryController;
using System.Globalization;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class SubBillDateDataSource : UICollectionViewDataSource
    {
        //  List<tra>

        TranWithDetailsLocal bill ;
        public SubBillDateDataSource(TranWithDetailsLocal list)
        {
            this.bill = list;

        }
        public SubBillDateDataSource()
        {
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("billListViewCell", indexPath) as BillListViewCell;
            if(this.bill!=null)
            {
                //if(this.bill.tranPayments[(int)indexPath.Row].PaymentType == "CH") //cash
                //{cell.Image= "CashB"; }
                //else if (this.bill.tranPayments[(int)indexPath.Row].PaymentType == "Dr") //Debit Card
                //{
                //    cell.Image = "CreditB";
                //}
                //else if (this.bill.tranPayments[(int)indexPath.Row].PaymentType == "Cr") //Credit Card
                //{
                //    cell.Image = "CreditB";
                //}
                //else if (this.bill.tranPayments[(int)indexPath.Row].PaymentType == "GV") //Gift Voucher
                //{
                //    cell.Image = "GiftvoucherB";
                //}
                //else if (this.bill.tranPayments[(int)indexPath.Row].PaymentType == "RP") //Redeem Point
                //{
                //    cell.Image = "GiftvoucherB";
                //}
                //else if (this.bill.tranPayments[(int)indexPath.Row].PaymentType == "EP") //ePayment
                //{
                //    cell.Image = "ePaymentB";
                //}
                cell.Image = "GiftvoucherB";
                cell.CustomerName = this.bill.tran.CustomerName;
                cell.BillNo = this.bill.tran.TranNo;
                cell.Total = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " "+Utils.DisplayDecimal(this.bill.tran.Total);
                cell.Time = this.bill.tran.TranDate.ToString("HH:mm tt", new CultureInfo("en-US"));
            }
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
           // return this.bill.tranPayments.Count;
            return 1;
        }
    }
}