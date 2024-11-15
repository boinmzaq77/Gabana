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
    public class ChangeBranchDatasource : UICollectionViewDataSource
    {
        BranchManage setbranch = new BranchManage();
        public int merchantID;
        private List<ChooseBranch> branches;

        public ChangeBranchDatasource(List<ChooseBranch> cus)
        {
            this.branches = cus;
        }

        public void ReloadData(List<ChooseBranch> cus)
        {
            this.branches = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ReportChooseBranchViewCell", indexPath) as ReportChooseBranchViewCell;
            cell.Name = this.branches[(int)indexPath.Row].BranchName;
            cell.status = false;
            if (ChangeBranchController.branchSelect == this.branches[(int)indexPath.Row].SysBranchID)
            {
                cell.status = true;
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.branches.Count;
        }
    }
}