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
using Gabana.ShareSource;
using Xamarin.Essentials;
using TinyInsightsLib;

namespace Gabana.iOS
{
    public class EmployeeDataSource : UICollectionViewDataSource
    {
        private List<Gabana.ORM.MerchantDB.UserAccountInfo> Employee;
        public UICollectionView collectionView;
        public EmployeeViewCell choosecell;
        string UserLogin;
        string TypeLogin;

        public EmployeeDataSource(List<ORM.MerchantDB.UserAccountInfo> cus)
        {
            this.Employee = cus;
        }

        public void ReloadData(List<ORM.MerchantDB.UserAccountInfo> cus)
        {
            this.Employee = cus;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            try
            {
                this.collectionView = collectionView;
                var cell = collectionView.DequeueReusableCell("employeeViewCell", indexPath) as EmployeeViewCell;
                UserLogin = Preferences.Get("User", "");
                TypeLogin = Preferences.Get("LoginType", "");
                string Language = Preferences.Get("Language", "");
                cell.Name = this.Employee[(int)indexPath.Row].UserName?.ToString();
                var dateEmpPosition = DataCashingAll.UserAccountInfo.Where(x => x.UserName == this.Employee[(int)indexPath.Row].UserName).FirstOrDefault();
                if (dateEmpPosition != null)
                {
                    cell.Name = this.Employee[(int)indexPath.Row].UserName?.ToString();
                    if (dateEmpPosition.UserAccessProduct)
                    {
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
                        else if (dateEmpPosition.MainRoles == "Owner")
                        {
                            cell.Image = "EmpRoleOwnerB";
                        }
                        else
                        {
                            cell.Image = "EmpManagerB";
                        }
                    }
                    else
                    {
                        if (dateEmpPosition.MainRoles == "Admin")
                        {
                            cell.Image = "EmpRoleAdminG";
                        }
                        else if (dateEmpPosition.MainRoles == "Cashier")
                        {
                            cell.Image = "EmpCashierG";
                        }
                        else if (dateEmpPosition.MainRoles == "Editor")
                        {
                            cell.Image = "EmpEditorG";
                        }
                        else if (dateEmpPosition.MainRoles == "Invoice")
                        {
                            cell.Image = "EmpInvoiceOfficerG";
                        }
                        else if (dateEmpPosition.MainRoles == "Manager")
                        {
                            cell.Image = "EmpRoleManagerG";
                        }
                        else if (dateEmpPosition.MainRoles == "Officer")
                        {
                            cell.Image = "EmpOfficerG";
                        }
                        else if (dateEmpPosition.MainRoles == "Owner")
                        {
                            cell.Image = "EmpRoleOwnerG";
                        }
                        else
                        {
                            cell.Image = "EmpManagerG";
                        }
                    }
                    var LoginType = Preferences.Get("LoginType", "");
                    var role = dateEmpPosition.MainRoles.ToLower();
                    switch (LoginType.ToLower())
                    {
                        case "owner":
                            if (role == "owner")
                            {
                                cell.swipe = false;
                            }
                            else
                            {
                                cell.swipe = true;
                            }
                            break;
                        case "admin":
                            if (role == "owner" || role == "admin")
                            {
                                cell.swipe = false;
                            }
                            else
                            {
                                cell.swipe = true;
                            }
                            break;
                        default:
                            cell.swipe = false;
                            break;
                    }

                    
                    cell.Status = dateEmpPosition.UserAccessProduct;
                    cell.Permit = dateEmpPosition.MainRoles?.ToString();
                }

               
               
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return null;
            }
            
            
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.Employee.Count;
        }
        private void Cell_OnDeleteItem(EmployeeViewCell indexPath)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(indexPath);
            OnCardCellDelete?.Invoke(indexPathQRcode);
        }

        private void Cell_OnItemSwipe(EmployeeViewCell indexPath)
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
                frame.X = -80;
                choosecell.showbtndelete = true;
                indexPath.Frame = frame;
            });
        }
        public delegate void CardCellDelete(NSIndexPath indexPath);
        public event CardCellDelete OnCardCellDelete;
    }
}