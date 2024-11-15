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
using Xamarin.Essentials;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class EmployeeManageDataSource : UICollectionViewDataSource
    {
        private ListEmployee listemployee;

        public EmployeeManageDataSource(ListEmployee cus)
        {
            this.listemployee = cus;
        }

        public void ReloadData(ListEmployee cus)
        {
            this.listemployee = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("employeeManageViewCell", indexPath) as EmployeeManageViewCell;
            var emplogin = Preferences.Get("User", "");
            var LoginType = Preferences.Get("LoginType", "");

            var position = (int)indexPath.Row;

            var data = DataCashingAll.UserAccountInfo.Where(x => x.UserName == listemployee[position].UserName).FirstOrDefault();
            if (data != null)
            {
                 cell.Name = listemployee[position].UserName?.ToString();
                 cell.Permit = data.MainRoles?.ToString();

                //ไม่แสดง Owner
                if (data.MainRoles?.ToString().ToLower() == "owner")
                {
                    cell.Status = 99;
                }
                else
                {
                    if (data.UserAccessProduct) // check
                    {
                        cell.Status = 1;
                    }
                    else
                    {
                        cell.Status = 0;
                    }
                }
                //Owner
                if (LoginType.ToLower() == "owner")
                {
                    cell.switchEnable = true;
                }
                else
                {
                    //Employee
                    var user = DataCashingAll.UserAccountInfo.Where(x => x.UserName == emplogin).FirstOrDefault();
                    if (user != null)
                    {
                        //Check LoginType
                        if (user.MainRoles == "Admin")
                        {
                            cell.switchEnable = true;
                        }
                        else
                        {
                            cell.switchEnable = false;
                        }
                    }
                }
            }

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.listemployee.Count;
        }
    }
}