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
    public class ReportPaymentDataSource : UICollectionViewDataSource
    {
        List<SalesByPaymentResponse> paymentResponses = new List<SalesByPaymentResponse>();
        List<PaymentType> payment;

        public ReportPaymentDataSource(List<SalesByPaymentResponse> cus,List<PaymentType> listCustomer)
        {
            this.paymentResponses = cus;
            payment = listCustomer;
        }

        public void ReloadData(List<SalesByPaymentResponse> cus)
        {
            this.paymentResponses = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("PaymentReportDataViewCell", indexPath) as PaymentReportDataViewCell;
            cell.Name = Utils.SetPaymentName(this.paymentResponses[(int)indexPath.Row].paymentType);
            cell.persent = this.paymentResponses[(int)indexPath.Row].percentTotal.ToString("#,##0.00")+"%";
            cell.Total = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " "+ Utils.DisplayDecimal( this.paymentResponses[(int)indexPath.Row].sumTotalAmount);
            switch (this.paymentResponses[(int)indexPath.Row].paymentType)
            {
                case "CH":
                    cell.Image = "Color1";
                    break;
                case "MYQR":
                    cell.Image = "Color5";
                    break;
                case "GV":
                    cell.Image = "Color4";
                    break;
                case "Cr":
                    cell.Image = "Color2";
                    break;
                case "Dr":
                    cell.Image = "Color3";
                    break;
                default:
                    cell.Image = "Color3";
                    break;
            }
            //var customer = payment.Where(x => x.Type == paymentResponses[(int)indexPath.Row].paymentType).FirstOrDefault();
            
            //if (customer != null)
            //{
            //    switch ((int)indexPath.Row)
            //    {
            //        case 0:
            //            cell.Image = "Color1";
            //            break;
            //        case 1:
            //            cell.Image = "Color3";
            //            break;
            //        case 2:
            //            cell.Image = "Color4";
            //            break;
            //        case 3:
            //            cell.Image = "Color2";
            //            break;

            //        case 4:
            //            cell.Image = "Color5";
            //            break;
            //        default:
            //            break;
            //    }
            //}
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.paymentResponses.Count;
        }
    }
}