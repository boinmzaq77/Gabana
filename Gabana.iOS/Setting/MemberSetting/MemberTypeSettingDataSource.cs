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
    public class MemberTypeSettingDataSource : UICollectionViewDataSource
    {
        BranchManage setbranch = new BranchManage();
        public int merchantID;
        public UICollectionView collectionView;
        public MemberTypeSettingViewCell choosecell;
        private List<Gabana.ORM.MerchantDB.MemberType> branches;

        public MemberTypeSettingDataSource(List<ORM.MerchantDB.MemberType> cus)
        {
            this.branches = cus;
            
            //lstCustomer
        }

        public void ReloadData(List<ORM.MerchantDB.MemberType> cus)
        {
            this.branches = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.collectionView = collectionView;
            var cell = collectionView.DequeueReusableCell("MemberTypeSettingViewCell", indexPath) as MemberTypeSettingViewCell;
            cell.Name = this.branches[(int)indexPath.Row].MemberTypeName;

            if (choosecell != null)
            {
                if (choosecell == cell)
                {
                    choosecell = null;
                }
            }

            cell.OnItemSwipe -= Cell_OnItemSwipe;
            cell.OnItemSwipe += Cell_OnItemSwipe; 
            cell.OnDeleteItem -= Cell_OnDeleteItem;
            cell.OnDeleteItem += Cell_OnDeleteItem;
            return cell;
        }
        private void Cell_OnDeleteItem(MemberTypeSettingViewCell indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.branches.Count;
        }
        private void Cell_OnItemSwipe(MemberTypeSettingViewCell indexPath)
        {

            if (choosecell != null)
            {
                UIView.Animate(0.7, () =>
                {
                    var frame2 = choosecell.Frame;
                    frame2.X = 0;
                    choosecell.showbtndelete = false;
                    choosecell.Frame = frame2;
                });
            };
            choosecell = indexPath;
            UIView.Animate(0.7, () =>
            {
                var frame = indexPath.Frame;
                frame.X = -60;
                choosecell.showbtndelete = true;
                indexPath.Frame = frame;
            });
        }
        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;
    }
}