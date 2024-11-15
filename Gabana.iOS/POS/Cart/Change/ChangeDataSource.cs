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
using Gabana3.JAM.Trans;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class ChangeDataSource : UICollectionViewDataSource
    {
        //public List<Gabana.ORM.MerchantDB.Branch> branches;
        
        private List<TranPayment> tranPayments;
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;


        public ChangeDataSource(List<TranPayment> tranPayments)
        {
            this.tranPayments = tranPayments;
        }

        public void ReloadData(List<TranPayment> tranPayments)
        {
            this.tranPayments = tranPayments;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ChangeCollectionViewCell", indexPath) as ChangeCollectionViewCell;
            
            cell.price = /*Currency + " " + */  Utils.DisplayDecimal( this.tranPayments[(int)indexPath.Row].PaymentAmount);
            switch (tranPayments[(int)indexPath.Row].PaymentType.ToUpper())
            {
                case "CH":
                    cell.Image = "Cash.png";
                    cell.Name = "Cash";
                    break;
                case "MYQR":
                    cell.Image = "HistoryQR.png";
                    cell.Name = "myQR";
                    break;
                case "GV":
                    cell.Image = "Giftvoucher.png";
                    cell.Name = "Giftvoucher";
                    break;
                case "CR":
                    cell.Image = "Credit.png";
                    cell.Name = "Credit Card";
                    break;
                case "DR":
                    cell.Image = "Credit.png";
                    cell.Name = "Debit Card";
                    break;

                default:
                    cell.Image = "Cash.png";
                    cell.Name = "Cash";
                    break;
            }
            
           
            


            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.tranPayments.Count;
        }
    }
}