using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
//using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;
using System.Threading.Tasks;
using static Gabana.iOS.CustomerController;

namespace Gabana.iOS
{
    public class ReportChooseBranchDataSource : UICollectionViewDataSource
    {
        BranchManage setbranch = new BranchManage();
        public int merchantID;
        private List<Branch> branches;

        public ReportChooseBranchDataSource(List<Branch> cus)
        {
            this.branches = cus;
            //lstCustomer
        }

        public void ReloadData(List<Branch> cus)
        {
            this.branches = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ReportChooseBranchViewCell", indexPath) as ReportChooseBranchViewCell;
            cell.Name = this.branches[(int)indexPath.Row].BranchName;
            cell.status = false;
            if (ReportController.listChooseBranch != null)
            {
                if (ReportController.listChooseBranch.Any(x => x.SysBranchID == this.branches[(int)indexPath.Row].SysBranchID))
                {
                    cell.status = true;
                }
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.branches.Count;
        }
    }
}