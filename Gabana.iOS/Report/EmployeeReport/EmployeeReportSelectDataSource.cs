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
    public class EmployeeReportSelectDataSource : UICollectionViewDataSource
    {
        ListEmployee listEmployee = null;

        public EmployeeReportSelectDataSource(ListEmployee listCategory)
        {
            this.listEmployee = listCategory;
        }

        public void ReloadData(ListEmployee listCategory)
        {
            this.listEmployee = listCategory;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("ReportChooseEmployeeViewCell", indexPath) as ReportChooseEmployeeViewCell;
            cell.Name = this.listEmployee[(int)indexPath.Row].UserName?.ToString();

            var dateEmpPosition = DataCashingAll.UserAccountInfo.Where(x => x.UserName == this.listEmployee[(int)indexPath.Row].UserName).FirstOrDefault();
            cell.role = dateEmpPosition.MainRoles;

            if (dateEmpPosition.MainRoles == "Admin")
                {
                    cell.Image = "EmpRoleAdminB";
                }
                else if (dateEmpPosition.MainRoles == "Cashier")
                {
                    cell.Image = "EmpCashierB";
                }
                else if (dateEmpPosition.MainRoles == "Editor")
                {
                    cell.Image = "EmpEditorB";
                }
                else if (dateEmpPosition.MainRoles == "Invoice")
                {
                    cell.Image = "EmpInvoiceOfficerB";
                }
                else if (dateEmpPosition.MainRoles == "Manager")
                {
                    cell.Image = "EmpRoleManagerB";
                }
                else if (dateEmpPosition.MainRoles == "Officer")
                {
                    cell.Image = "EmpOfficerB";
                }
                else
                {
                    cell.Image = "EmpManagerB";
                }
           
            var index = ReportSelectEmployeeController.listChooseEmployee.FindIndex(x => x.UserName == listEmployee[(int)indexPath.Row].UserName);
            if (index == -1)
            {
                cell.status = false;
            }
            else
            {
                cell.status = true;
            }


            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.listEmployee.Count;
        }
    }
}