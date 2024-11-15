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
    public class MYQRDataSourceList : UICollectionViewDataSource
    {
        ListMyQRCodeIOS Menu;
        List<ORM.MerchantDB.Branch> lstBranch;
        CategoryManage catManage = new CategoryManage();
        public MYQRDataSourceList(ListMyQRCodeIOS l)
        {
            this.Menu = l;
            Branch();
        }
        public void ReloadData(ListMyQRCodeIOS l)
        {
            this.Menu = l;
        }
        async void Branch()
        {
            BranchManage branchManage = new BranchManage();
            lstBranch = await branchManage.GetAllBranch((int)MainController.merchantlocal.MerchantID);
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("MYQRCollectionViewCellList", indexPath) as MYQRCollectionViewCellList;
            var position = (int)indexPath.Row;
            cell.Code = this.Menu[position].MyQrCodeName;
            if (this.Menu[position].FMyQrAllBranch == 'A')
            {
                cell.BranchName = Utils.TextBundle("allbranch", "All Branch");
            }
            else
            {
                var detail = lstBranch.Where(x => x.SysBranchID == this.Menu[position].SysBranchID).ToList();
                if (detail != null)
                {
                    foreach (var item in detail)
                    {
                        if (detail.Count == 1)
                        {
                            cell.BranchName  = item.TaxBranchName;
                        }
                        else
                        {
                            cell.BranchName  += item.TaxBranchName + ", ";
                        }
                    }
                }
            }
            var paths = this.Menu[position].PicturePath;
            if (!string.IsNullOrEmpty(paths))
            {
                if (DataCashingAll.CheckConnectInternet)
                {
                    cell.Image = paths;
                }
                else
                {
                    cell.Image = this.Menu[position].PictureLocalPath;

                }
            }
            else
            {
                cell.Image = this.Menu[position].PictureLocalPath;
            }

            return cell;
        }

     

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (this.Menu != null)
                return this.Menu.Count;
            else
                return 0;
        }

    }
}