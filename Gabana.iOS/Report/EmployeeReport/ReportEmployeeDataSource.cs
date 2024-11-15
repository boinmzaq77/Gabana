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
    public class ReportEmployeeDataSource : UICollectionViewDataSource
    {
        List<Gabana3.JAM.Report.SalesBySellerResponse> employeeResponses = new List<Gabana3.JAM.Report.SalesBySellerResponse>();
        List<ORM.MerchantDB.UserAccountInfo> employee;

        public ReportEmployeeDataSource(List<Gabana3.JAM.Report.SalesBySellerResponse> cus,List<ORM.MerchantDB.UserAccountInfo> listCustomer)
        {
            this.employeeResponses = cus;
            employee = listCustomer;
            //lstCustomer
        }

        public void ReloadData(List<Gabana3.JAM.Report.SalesBySellerResponse> cus)
        {
            this.employeeResponses = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("EmployeeReportDataViewCell", indexPath) as EmployeeReportDataViewCell;
            cell.Name = this.employeeResponses[(int)indexPath.Row].sellerName;
            cell.Total = this.employeeResponses[(int)indexPath.Row].sumTotalAmount.ToString("#,##0.00");

            var customer = employee.Where(x => x.UserName == employeeResponses[(int)indexPath.Row].sellerName).FirstOrDefault();
            cell.role = "";
            if (customer != null)
            {
                if (DataCashingAll.UserAccountInfo.Count != 0)
                {
                    var dateEmpPosition = DataCashingAll.UserAccountInfo?.Where(x => x.UserName == customer.UserName).FirstOrDefault();
                    cell.Name = this.employeeResponses[(int)indexPath.Row].sellerName?.ToString();
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
                    cell.role = dateEmpPosition.MainRoles;
                }
                else
                {
                    cell.Image = "EmpManagerB";
                    
                }
                
            }
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.employeeResponses.Count;
        }
    }
}