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
using Gabana3.JAM.Dashboard;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class BestEmployeeDataSource : UICollectionViewDataSource
    {
        List<BestEmployee> bestEmployees = new List<BestEmployee>();
        decimal maxvalue;

        public BestEmployeeDataSource(List<BestEmployee> bestEmployees , decimal maxvalue)
        {
            this.bestEmployees = bestEmployees;
            this.maxvalue = maxvalue;
        }
        public void ReloadData(List<BestEmployee> bestEmployees, decimal maxvalue)
        {
            this.bestEmployees = bestEmployees;
            this.maxvalue = maxvalue;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("BestEmployeeViewCell", indexPath) as BestEmployeeViewCell;
            cell.Name = this.bestEmployees[(int)indexPath.Row].employeeName .ToString();
            cell.price = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " " + Utils.DisplayDecimal(this.bestEmployees[(int)indexPath.Row].totalAmount);   
            var per =  this.bestEmployees[(int)indexPath.Row].totalAmount/ maxvalue ;
            cell.color = (int)indexPath.Row;
            cell.persen = (float)per;
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.bestEmployees.Count;
        }
    }

}