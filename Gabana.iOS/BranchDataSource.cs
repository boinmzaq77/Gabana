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

namespace Gabana.iOS
{
    public class BranchDataSource : UICollectionViewDataSource
    {
        //public List<Gabana.ORM.MerchantDB.Branch> branches;
        BranchManage setBranch = new BranchManage();
        public int merchantID;
        //private List<ORM.MerchantDB.Branch> branches;
        private List<ChooseBranch> branches;

        public BranchDataSource(List<ChooseBranch> branch)
        {
            this.branches = branch;
        }
        public void ReloadData(List<ChooseBranch> branch)
        {
            this.branches = branch;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("BranchViewCell", indexPath) as BranchCollectionViewCell;
            cell.Name = this.branches[(int)indexPath.Row].BranchName;
            cell.show = branches[(int)indexPath.Row].Choose;
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.branches.Count;
        }
    }
}