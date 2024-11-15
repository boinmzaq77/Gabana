using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class EmployeeRoleController : UIViewController
    {
        UIView  AdminView, ManagerView, InvoiceOfficerView, CashierView, EditorView,_contentView, OfficerView;
        UIScrollView _scrollView;
        UIImageView  AdminImg, ManagerImg, InvoiceImg, CashierImg, EditorImg, OfficeImg;
        UILabel  lbl_Head_Admin, lbl_Head_Manager, lbl_Head_Invoice, lbl_Head_Cashier, lbl_Head_Editor, lbl_Head_Officer;
        UILabel  lbl_Admin, lbl_Manager, lbl__Invoice, lbl_Cashier, lbl_Editor, lbl_Officer;
        
        BranchManage setBranch = new BranchManage();
        UIButton btnApplyEmpRole;
        string Select=null;

        UIImageView SelectOwnerImg, SelectAdminImg, SelectManagerImg, SelectInvoiceOfficerImg, SelectCashierImg, SelectEditorImg, SelectOfficerImg;
        public EmployeeRoleController()
        {
            Select = UpdateEmployeeController.RoleSelected;
        }
        public async  override void ViewWillAppear(bool animated)
        {
            try
            {
                base.ViewWillAppear(animated);
                this.NavigationController.SetNavigationBarHidden(false, false);
                Select = UpdateEmployeeController.RoleSelected;
                checkSelectRole();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
        }
        public async override void ViewDidLoad()
        {
            try
            {
                

                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248,248,248);
                initAttribute();
                setupAutoLayout();

                checkSelectRole();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
           
        }
        void checkSelectRole()
        {
            
            if (UpdateEmployeeController.RoleSelected == "Admin")
            {
                SelectAdminImg.Hidden = false;
                SelectManagerImg.Hidden = true;
                SelectCashierImg.Hidden = true;
                SelectEditorImg.Hidden = true;
                SelectOfficerImg.Hidden = true;
                SelectInvoiceOfficerImg.Hidden = true;
                //SelectOwnerImg.Hidden = true;
            }
            else if (UpdateEmployeeController.RoleSelected == "Manager")
            {
                SelectAdminImg.Hidden = true;
                SelectManagerImg.Hidden = false;
                SelectCashierImg.Hidden = true;
                SelectEditorImg.Hidden = true;
                SelectOfficerImg.Hidden = true;
                SelectInvoiceOfficerImg.Hidden = true;
               // SelectOwnerImg.Hidden = true;
            }
            else if (UpdateEmployeeController.RoleSelected == "Cashier")
            {
                SelectAdminImg.Hidden = true;
                SelectManagerImg.Hidden = true;
                SelectCashierImg.Hidden = false;
                SelectEditorImg.Hidden = true;
                SelectOfficerImg.Hidden = true;
                SelectInvoiceOfficerImg.Hidden = true;
                //SelectOwnerImg.Hidden = true;
            }
            else if (UpdateEmployeeController.RoleSelected == "Editor")
            {
                SelectAdminImg.Hidden = true;
                SelectManagerImg.Hidden = true;
                SelectCashierImg.Hidden = true;
                SelectEditorImg.Hidden = false;
                SelectOfficerImg.Hidden = true;
                SelectInvoiceOfficerImg.Hidden = true;
                //SelectOwnerImg.Hidden = true;
            }
            else if (UpdateEmployeeController.RoleSelected == "Invoice")
            {
                SelectAdminImg.Hidden = true;
                SelectManagerImg.Hidden = true;
                SelectCashierImg.Hidden = true;
                SelectEditorImg.Hidden = true;
                SelectOfficerImg.Hidden = true;
                SelectInvoiceOfficerImg.Hidden = false;
                //SelectOwnerImg.Hidden = true;
            }
            else if (UpdateEmployeeController.RoleSelected == "Officer")
            {
                SelectAdminImg.Hidden = true;
                SelectManagerImg.Hidden = true;
                SelectCashierImg.Hidden = true;
                SelectEditorImg.Hidden = true;
                SelectOfficerImg.Hidden = true;
                SelectInvoiceOfficerImg.Hidden = false;
                //SelectOwnerImg.Hidden = true;
            }
            //else if (UpdateEmployeeController.RoleSelected == "Owner")
            //{
            //    SelectAdminImg.Hidden = true;
            //    SelectManagerImg.Hidden = true;
            //    SelectCashierImg.Hidden = true;
            //    SelectEditorImg.Hidden = true;
            //    SelectOfficerImg.Hidden = true;
            //    SelectInvoiceOfficerImg.Hidden = true;
            //    SelectOwnerImg.Hidden = false;
            //}
        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248,248,248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region View Layout

            #region OwnerView
            //OwnerView = new UIView();
            //OwnerView.TranslatesAutoresizingMaskIntoConstraints = false;
            //OwnerView.BackgroundColor = UIColor.White;
            //OwnerView.Layer.ShadowColor = UIColor.FromRGB(248,248,248).CGColor;
            //OwnerView.Layer.ShadowOpacity = 1;
            //OwnerView.Layer.ShadowOffset = new CoreGraphics.CGSize(-15, 20);
            //OwnerView.Layer.MasksToBounds = false;

            //lbl_Head_owner = new UILabel
            //{
            //    TextColor = UIColor.FromRGB(0,149,218),
            //    TranslatesAutoresizingMaskIntoConstraints = false
            //};
            //lbl_Head_owner.Font = lbl_Head_owner.Font.WithSize(16);
            //lbl_Head_owner.Text = "Owner";
            //OwnerView.AddSubview(lbl_Head_owner);

            //lbl_owner = new UILabel
            //{
            //    TextColor = UIColor.FromRGB(64, 64, 64),
            //    TranslatesAutoresizingMaskIntoConstraints = false
            //};
            //lbl_owner.Font = lbl_owner.Font.WithSize(12);
            //lbl_owner.Text = "เจ้าของร้าน จัดการได้ทุกอย่าง";
            //OwnerView.AddSubview(lbl_owner);

            //OwnerImg = new UIImageView();
            //OwnerImg.Image = UIImage.FromBundle("EmpOwnerB");
            //OwnerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            //OwnerView.AddSubview(OwnerImg);

            //SelectOwnerImg = new UIImageView();
            //SelectOwnerImg.Image = UIImage.FromBundle("Check");
            //SelectOwnerImg.Hidden = true;
            //SelectOwnerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            //OwnerView.AddSubview(SelectOwnerImg);

            //OwnerView.UserInteractionEnabled = true;
            //var tapGestureSelectRole1 = new UITapGestureRecognizer(this,
            //    new ObjCRuntime.Selector("RolesOwner:"))
            //{
            //    NumberOfTapsRequired = 1 // change number as you want 
            //};
            //OwnerView.AddGestureRecognizer(tapGestureSelectRole1);
            #endregion

            #region AdminView
            AdminView = new UIView();
            AdminView.TranslatesAutoresizingMaskIntoConstraints = false;
            AdminView.BackgroundColor = UIColor.White;
            AdminView.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            AdminView.Layer.ShadowOpacity = 1;
            AdminView.ClipsToBounds = true;
            AdminView.Layer.CornerRadius = 7;
            AdminView.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            AdminView.Layer.MasksToBounds = false;

            lbl_Head_Admin = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Head_Admin.Font = lbl_Head_Admin.Font.WithSize(16);
            lbl_Head_Admin.Text = Utils.TextBundle("admin", "Admin");
            AdminView.AddSubview(lbl_Head_Admin);

            lbl_Admin = new UILabel
            {
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Admin.Font = lbl_Admin.Font.WithSize(12);
            lbl_Admin.Text = Utils.TextBundle("roleadmin", "ผู้ดูแลระบบ จัดการระบบพนักงาน และการตั้งค่าได้");
            AdminView.AddSubview(lbl_Admin);

            AdminImg = new UIImageView();
            AdminImg.Image = UIImage.FromBundle("EmpAdminB");
            AdminImg.TranslatesAutoresizingMaskIntoConstraints = false;
            AdminView.AddSubview(AdminImg);

            SelectAdminImg = new UIImageView();
            SelectAdminImg.Hidden = true;
            SelectAdminImg.Image = UIImage.FromBundle("Check");
            SelectAdminImg.TranslatesAutoresizingMaskIntoConstraints = false;
            AdminView.AddSubview(SelectAdminImg);

            AdminView.UserInteractionEnabled = true;
            var tapGestureSelectRole2 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("RolesAdmin:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            AdminView.AddGestureRecognizer(tapGestureSelectRole2);
            #endregion

            #region ManagerView
            ManagerView = new UIView();
            ManagerView.TranslatesAutoresizingMaskIntoConstraints = false;
            ManagerView.BackgroundColor = UIColor.White;
            ManagerView.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            ManagerView.Layer.ShadowOpacity = 1;
            ManagerView.ClipsToBounds = true;
            ManagerView.Layer.CornerRadius = 7;
            ManagerView.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            ManagerView.Layer.MasksToBounds = false;

            lbl_Head_Manager = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Head_Manager.Font = lbl_Head_Manager.Font.WithSize(16);
            lbl_Head_Manager.Text = Utils.TextBundle("manager", "Manager");
            ManagerView.AddSubview(lbl_Head_Manager);

            lbl_Manager = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Manager.Font = lbl_Manager.Font.WithSize(12);
            lbl_Manager.Text = Utils.TextBundle("rolemanager", "ผู้จัดการร้าน จัดการข้อมูลบิลได้");
            lbl_Manager.Lines = 2;
            ManagerView.AddSubview(lbl_Manager);

            ManagerImg = new UIImageView();
            ManagerImg.Image = UIImage.FromBundle("EmpManagerB");
            ManagerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ManagerView.AddSubview(ManagerImg);

            SelectManagerImg = new UIImageView();
            SelectManagerImg.Hidden = true;
            SelectManagerImg.Image = UIImage.FromBundle("Check");
            SelectManagerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ManagerView.AddSubview(SelectManagerImg);

            ManagerView.UserInteractionEnabled = true;
            var tapGestureSelectRole3 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("RolesManager:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ManagerView.AddGestureRecognizer(tapGestureSelectRole3);
            #endregion

            #region InvoiceView
            InvoiceOfficerView = new UIView();
            InvoiceOfficerView.TranslatesAutoresizingMaskIntoConstraints = false;
            InvoiceOfficerView.BackgroundColor = UIColor.White;
            InvoiceOfficerView.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            InvoiceOfficerView.Layer.ShadowOpacity = 1;
            InvoiceOfficerView.ClipsToBounds = true;
            InvoiceOfficerView.Layer.CornerRadius = 7;
            InvoiceOfficerView.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            InvoiceOfficerView.Layer.MasksToBounds = false;

            lbl_Head_Invoice = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Head_Invoice.Font = lbl_Head_Invoice.Font.WithSize(16);
            lbl_Head_Invoice.Text = Utils.TextBundle("invoiceofficer", "Invoice Officer");
            InvoiceOfficerView.AddSubview(lbl_Head_Invoice);

            lbl__Invoice = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl__Invoice.Font = lbl__Invoice.Font.WithSize(12);
            lbl__Invoice.Text = Utils.TextBundle("roleinvoiceofficer", "พนักงาน ทำงานได้ทั้งบิลรับและบิลจ่าย");
            lbl__Invoice.Lines = 2;
            InvoiceOfficerView.AddSubview(lbl__Invoice);

            InvoiceImg = new UIImageView();
            InvoiceImg.Image = UIImage.FromBundle("EmpInvoiceOfficerB");
            InvoiceImg.TranslatesAutoresizingMaskIntoConstraints = false;
            InvoiceOfficerView.AddSubview(InvoiceImg);

            SelectInvoiceOfficerImg = new UIImageView();
            SelectInvoiceOfficerImg.Hidden = true;
            SelectInvoiceOfficerImg.Image = UIImage.FromBundle("Check");
            SelectInvoiceOfficerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            InvoiceOfficerView.AddSubview(SelectInvoiceOfficerImg);

            InvoiceOfficerView.UserInteractionEnabled = true;
            var tapGestureSelectRole4 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("RolesInvoice:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            InvoiceOfficerView.AddGestureRecognizer(tapGestureSelectRole4);
            #endregion

            #region CashierView
            CashierView = new UIView();
            CashierView.TranslatesAutoresizingMaskIntoConstraints = false;
            CashierView.BackgroundColor = UIColor.White;
            CashierView.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            CashierView.Layer.ShadowOpacity = 1;
            CashierView.Layer.CornerRadius = 7;
            CashierView.ClipsToBounds = true;
            CashierView.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            CashierView.Layer.MasksToBounds = false;

            lbl_Head_Cashier = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Head_Cashier.Font = lbl_Head_Cashier.Font.WithSize(16);
            lbl_Head_Cashier.Text = Utils.TextBundle("cashier", "Cashier");
            CashierView.AddSubview(lbl_Head_Cashier);

            lbl_Cashier = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Cashier.Font = lbl_Cashier.Font.WithSize(12);
            lbl_Cashier.Text = Utils.TextBundle("rolecashier", "พนักงาน ทำงานเกี่ยวข้องเฉพาะเอกสารจ่าย");
            lbl_Cashier.Lines = 2;
            CashierView.AddSubview(lbl_Cashier);

            CashierImg = new UIImageView();
            CashierImg.Image = UIImage.FromBundle("EmpCashierB");
            CashierImg.TranslatesAutoresizingMaskIntoConstraints = false;
            CashierView.AddSubview(CashierImg);

            SelectCashierImg = new UIImageView();
            SelectCashierImg.Hidden = true;
            SelectCashierImg.Image = UIImage.FromBundle("Check");
            SelectCashierImg.TranslatesAutoresizingMaskIntoConstraints = false;
            CashierView.AddSubview(SelectCashierImg);

            CashierView.UserInteractionEnabled = true;
            var tapGestureSelectRole5 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("RolesCashier:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            CashierView.AddGestureRecognizer(tapGestureSelectRole5);
            #endregion

            #region EditorView
            EditorView = new UIView();
            EditorView.TranslatesAutoresizingMaskIntoConstraints = false;
            EditorView.BackgroundColor = UIColor.White;
            EditorView.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            EditorView.Layer.ShadowOpacity = 1;
            EditorView.Layer.CornerRadius = 7;
            EditorView.ClipsToBounds = true;
            EditorView.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            EditorView.Layer.MasksToBounds = false;

            lbl_Head_Editor = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Head_Editor.Font = lbl_Head_Editor.Font.WithSize(16);
            lbl_Head_Editor.Text = Utils.TextBundle("editor", "Editor");
            EditorView.AddSubview(lbl_Head_Editor);

            lbl_Editor = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Editor.Font = lbl_Editor.Font.WithSize(12);
            lbl_Editor.Text = Utils.TextBundle("roleeditor", "พนักงาน แก้ไขบิลได้ แต่ไม่สามารถลบบิลได้");
            lbl_Editor.Lines = 2;
            EditorView.AddSubview(lbl_Editor);

            EditorImg = new UIImageView();
            EditorImg.Image = UIImage.FromBundle("EmpEditorB");
            EditorImg.TranslatesAutoresizingMaskIntoConstraints = false;
            EditorView.AddSubview(EditorImg);

            SelectEditorImg = new UIImageView();
            SelectEditorImg.Hidden = true;
            SelectEditorImg.Image = UIImage.FromBundle("Check");
            SelectEditorImg.TranslatesAutoresizingMaskIntoConstraints = false;
            EditorView.AddSubview(SelectEditorImg);

            EditorView.UserInteractionEnabled = true;
            var tapGestureSelectRole6 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("RolesEditor:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            EditorView.AddGestureRecognizer(tapGestureSelectRole6);
            #endregion

            #region OfficerView
            OfficerView = new UIView();
            OfficerView.TranslatesAutoresizingMaskIntoConstraints = false;
            OfficerView.BackgroundColor = UIColor.White;
            OfficerView.Layer.ShadowColor = UIColor.FromRGB(248, 248, 248).CGColor;
            OfficerView.Layer.ShadowOpacity = 1;
            OfficerView.Layer.CornerRadius = 7;
            OfficerView.ClipsToBounds = true;
            OfficerView.Layer.ShadowOffset = new CoreGraphics.CGSize(0f, 7f);
            OfficerView.Layer.MasksToBounds = false;

            lbl_Head_Officer = new UILabel
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Head_Officer.Font = lbl_Head_Officer.Font.WithSize(16);
            lbl_Head_Officer.Text = Utils.TextBundle("officer", "Officer");
            OfficerView.AddSubview(lbl_Head_Officer);

            lbl_Officer = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Officer.Font = lbl_Officer.Font.WithSize(12);
            lbl_Officer.Text = Utils.TextBundle("roleofficer", "พนักงาน ป้อนข้อมูลเข้าระบบ");
            OfficerView.AddSubview(lbl_Officer);

            OfficeImg = new UIImageView();
            OfficeImg.Image = UIImage.FromBundle("EmpOfficerB");
            OfficeImg.TranslatesAutoresizingMaskIntoConstraints = false;
            OfficerView.AddSubview(OfficeImg);

            SelectOfficerImg = new UIImageView();
            SelectOfficerImg.Hidden = true;
            SelectOfficerImg.Image = UIImage.FromBundle("Check");
            SelectOfficerImg.TranslatesAutoresizingMaskIntoConstraints = false;
            OfficerView.AddSubview(SelectOfficerImg);

            OfficerView.UserInteractionEnabled = true;
            var tapGestureSelectRole7 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("RolesOfficer:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            OfficerView.AddGestureRecognizer(tapGestureSelectRole7);
            #endregion

            #endregion

        //    _contentView.AddSubview(OwnerView);
            _contentView.AddSubview(AdminView);
            _contentView.AddSubview(ManagerView);
            _contentView.AddSubview(InvoiceOfficerView);
            _contentView.AddSubview(CashierView);
            _contentView.AddSubview(EditorView);
            _contentView.AddSubview(OfficerView);

            _scrollView.AddSubview(_contentView);


            btnApplyEmpRole = new UIButton();
            btnApplyEmpRole.SetTitle(Utils.TextBundle("applyemployeerole", "Apply Employee Role"), UIControlState.Normal);
            btnApplyEmpRole.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnApplyEmpRole.Layer.CornerRadius = 5f;
            btnApplyEmpRole.Layer.BorderWidth = 0.5f;
            btnApplyEmpRole.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnApplyEmpRole.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnApplyEmpRole.TranslatesAutoresizingMaskIntoConstraints = false;
            btnApplyEmpRole.TouchUpInside += async  (sender, e)  => {
                UpdateEmployeeController.RoleSelected = Select;
                UpdateEmployeeController.isModifyRole = true;
                if (Select == "Admin")
                {
                    UpdateEmployeeController.isModifyBranch = true;
                    UpdateEmployeeController.BranchSelect = "All branch";
                    UpdateEmployeeController.listChooseBranch = new List<BranchPolicy>();
                }
                else
                {
                    var branch = await setBranch.GetAllBranch((int)MainController.merchantlocal.MerchantID);
                    UpdateEmployeeController.isModifyBranch = true;
                    UpdateEmployeeController.BranchSelect = branch[0].BranchName;
                    UpdateEmployeeController.listChooseBranch = new List<BranchPolicy>();
                }
                this.NavigationController.PopViewController(false);
            };
            View.AddSubview(btnApplyEmpRole);

            View.AddSubview(_scrollView);
        }
        #region Select
        [Export("RolesOfficer:")]
        public void RolesOfficer(UIGestureRecognizer sender)
        {

            SelectAdminImg.Hidden = true;
            SelectManagerImg.Hidden = true;
            SelectCashierImg.Hidden = true;
            SelectEditorImg.Hidden = true;
            SelectOfficerImg.Hidden = false;
            SelectInvoiceOfficerImg.Hidden = true;
            //SelectOwnerImg.Hidden = true;
            Select = "Officer";
        }
        [Export("RolesEditor:")]
        public void RolesEditor(UIGestureRecognizer sender)
        {
            SelectAdminImg.Hidden = true;
            SelectManagerImg.Hidden = true;
            SelectCashierImg.Hidden = true;
            SelectEditorImg.Hidden = false;
            SelectOfficerImg.Hidden = true;
            SelectInvoiceOfficerImg.Hidden = true;
            //SelectOwnerImg.Hidden = true;
            Select = "Editor";
        }
        [Export("RolesCashier:")]
        public void RolesCashier(UIGestureRecognizer sender)
        {
            SelectAdminImg.Hidden = true;
            SelectManagerImg.Hidden = true;
            SelectCashierImg.Hidden = false;
            SelectEditorImg.Hidden = true;
            SelectOfficerImg.Hidden = true;
            SelectInvoiceOfficerImg.Hidden = true;
            //SelectOwnerImg.Hidden = true;
            Select = "Cashier";
        }
        [Export("RolesInvoice:")]
        public void RolesInvoice(UIGestureRecognizer sender)
        {
            SelectAdminImg.Hidden = true;
            SelectManagerImg.Hidden = true;
            SelectCashierImg.Hidden = true;
            SelectEditorImg.Hidden = true;
            SelectOfficerImg.Hidden = true;
            SelectInvoiceOfficerImg.Hidden = false;
            //SelectOwnerImg.Hidden = true;
            Select = "Invoice";
        }
        [Export("RolesManager:")]
        public void RolesManager(UIGestureRecognizer sender)
        {
            SelectAdminImg.Hidden = true;
            SelectManagerImg.Hidden = false;
            SelectCashierImg.Hidden = true;
            SelectEditorImg.Hidden = true;
            SelectOfficerImg.Hidden = true;
            SelectInvoiceOfficerImg.Hidden = true;
            //SelectOwnerImg.Hidden = true;
            Select = "Manager";
        }
        [Export("RolesAdmin:")]
        public void RolesAdmin(UIGestureRecognizer sender)
        {
            SelectAdminImg.Hidden = false;
            SelectManagerImg.Hidden = true;
            SelectCashierImg.Hidden = true;
            SelectEditorImg.Hidden = true;
            SelectOfficerImg.Hidden = true;
            SelectInvoiceOfficerImg.Hidden = true;
            //SelectOwnerImg.Hidden = true;
            Select = "Admin";
        }
        //[Export("RolesOwner:")]
        //public void RolesOwner(UIGestureRecognizer sender)
        //{
        //    SelectAdminImg.Hidden = true;
        //    SelectManagerImg.Hidden = true;
        //    SelectCashierImg.Hidden = true;
        //    SelectEditorImg.Hidden = true;
        //    SelectOfficerImg.Hidden = true;
        //    SelectInvoiceOfficerImg.Hidden = true;
        //    SelectOwnerImg.Hidden = false;
        //    Select = "Owner";
        //}
        #endregion

        void setupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(btnApplyEmpRole.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region Owner
            //OwnerView.TopAnchor.ConstraintEqualTo(OwnerView.Superview.TopAnchor, 10).Active = true;
            //OwnerView.LeftAnchor.ConstraintEqualTo(OwnerView.Superview.LeftAnchor, 10).Active = true;
            //OwnerView.RightAnchor.ConstraintEqualTo(OwnerView.Superview.RightAnchor, -10).Active = true;
            //OwnerView.HeightAnchor.ConstraintEqualTo(87).Active = true;

            //OwnerImg.CenterYAnchor.ConstraintEqualTo(OwnerImg.Superview.CenterYAnchor,0).Active = true;
            //OwnerImg.LeftAnchor.ConstraintEqualTo(OwnerImg.Superview.LeftAnchor, 20).Active = true;
            //OwnerImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            //OwnerImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            //lbl_Head_owner.TopAnchor.ConstraintEqualTo(lbl_Head_owner.Superview.TopAnchor, 19).Active = true;
            //lbl_Head_owner.LeftAnchor.ConstraintEqualTo(OwnerImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            //lbl_Head_owner.RightAnchor.ConstraintEqualTo(lbl_Head_owner.Superview.RightAnchor,-8).Active = true;
            //lbl_Head_owner.HeightAnchor.ConstraintEqualTo(22).Active = true;

            //lbl_owner.TopAnchor.ConstraintEqualTo(lbl_Head_owner.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            //lbl_owner.LeftAnchor.ConstraintEqualTo(OwnerImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            //lbl_owner.RightAnchor.ConstraintEqualTo(lbl_owner.Superview.RightAnchor, -8).Active = true;
            //lbl_owner.BottomAnchor.ConstraintEqualTo(lbl_owner.SafeAreaLayoutGuide.BottomAnchor,-15).Active = true;

            //SelectOwnerImg.CenterYAnchor.ConstraintEqualTo(SelectOwnerImg.Superview.CenterYAnchor, 0).Active = true;
            //SelectOwnerImg.RightAnchor.ConstraintEqualTo(SelectOwnerImg.Superview.RightAnchor, -20).Active = true;
            //SelectOwnerImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            //SelectOwnerImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region AdminView
            AdminView.TopAnchor.ConstraintEqualTo(AdminView.Superview.TopAnchor, 10).Active = true;
            AdminView.LeftAnchor.ConstraintEqualTo(AdminView.Superview.LeftAnchor, 10).Active = true;
            AdminView.RightAnchor.ConstraintEqualTo(AdminView.Superview.RightAnchor, -10).Active = true;
            AdminView.HeightAnchor.ConstraintEqualTo(87).Active = true;

            AdminImg.CenterYAnchor.ConstraintEqualTo(AdminImg.Superview.CenterYAnchor, 0).Active = true;
            AdminImg.LeftAnchor.ConstraintEqualTo(AdminImg.Superview.LeftAnchor, 20).Active = true;
            AdminImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            AdminImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            lbl_Head_Admin.TopAnchor.ConstraintEqualTo(lbl_Head_Admin.Superview.TopAnchor, 19).Active = true;
            lbl_Head_Admin.LeftAnchor.ConstraintEqualTo(AdminImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lbl_Head_Admin.RightAnchor.ConstraintEqualTo(lbl_Head_Admin.Superview.RightAnchor, -8).Active = true;
            lbl_Head_Admin.HeightAnchor.ConstraintEqualTo(22).Active = true;

            lbl_Admin.TopAnchor.ConstraintEqualTo(lbl_Head_Admin.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_Admin.LeftAnchor.ConstraintEqualTo(AdminImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl_Admin.RightAnchor.ConstraintEqualTo(lbl_Admin.Superview.RightAnchor, -8).Active = true;
            lbl_Admin.BottomAnchor.ConstraintEqualTo(lbl_Admin.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;

            SelectAdminImg.CenterYAnchor.ConstraintEqualTo(SelectAdminImg.Superview.CenterYAnchor, 0).Active = true;
            SelectAdminImg.RightAnchor.ConstraintEqualTo(SelectAdminImg.Superview.RightAnchor, -20).Active = true;
            SelectAdminImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectAdminImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region ManagerView
            ManagerView.TopAnchor.ConstraintEqualTo(AdminView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            ManagerView.LeftAnchor.ConstraintEqualTo(ManagerView.Superview.LeftAnchor, 10).Active = true;
            ManagerView.RightAnchor.ConstraintEqualTo(ManagerView.Superview.RightAnchor, -10).Active = true;
            ManagerView.HeightAnchor.ConstraintEqualTo(87).Active = true;

            ManagerImg.CenterYAnchor.ConstraintEqualTo(ManagerImg.Superview.CenterYAnchor, 0).Active = true;
            ManagerImg.LeftAnchor.ConstraintEqualTo(ManagerImg.Superview.LeftAnchor, 20).Active = true;
            ManagerImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            ManagerImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            lbl_Head_Manager.TopAnchor.ConstraintEqualTo(lbl_Head_Manager.Superview.TopAnchor, 19).Active = true;
            lbl_Head_Manager.LeftAnchor.ConstraintEqualTo(ManagerImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lbl_Head_Manager.RightAnchor.ConstraintEqualTo(lbl_Head_Manager.Superview.RightAnchor, -8).Active = true;
            lbl_Head_Manager.HeightAnchor.ConstraintEqualTo(22).Active = true;

            lbl_Manager.TopAnchor.ConstraintEqualTo(lbl_Head_Manager.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_Manager.LeftAnchor.ConstraintEqualTo(ManagerImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl_Manager.RightAnchor.ConstraintEqualTo(lbl_Manager.Superview.RightAnchor, -8).Active = true;
            lbl_Manager.BottomAnchor.ConstraintEqualTo(lbl_Manager.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;

            SelectManagerImg.CenterYAnchor.ConstraintEqualTo(SelectManagerImg.Superview.CenterYAnchor, 0).Active = true;
            SelectManagerImg.RightAnchor.ConstraintEqualTo(SelectManagerImg.Superview.RightAnchor, -20).Active = true;
            SelectManagerImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectManagerImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region InvoiceOfficerView
            InvoiceOfficerView.TopAnchor.ConstraintEqualTo(ManagerView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            InvoiceOfficerView.LeftAnchor.ConstraintEqualTo(InvoiceOfficerView.Superview.LeftAnchor, 10).Active = true;
            InvoiceOfficerView.RightAnchor.ConstraintEqualTo(InvoiceOfficerView.Superview.RightAnchor, -10).Active = true;
            InvoiceOfficerView.HeightAnchor.ConstraintEqualTo(87).Active = true;

            InvoiceImg.CenterYAnchor.ConstraintEqualTo(InvoiceImg.Superview.CenterYAnchor, 0).Active = true;
            InvoiceImg.LeftAnchor.ConstraintEqualTo(InvoiceImg.Superview.LeftAnchor, 20).Active = true;
            InvoiceImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            InvoiceImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            lbl_Head_Invoice.TopAnchor.ConstraintEqualTo(lbl_Head_Invoice.Superview.TopAnchor, 19).Active = true;
            lbl_Head_Invoice.LeftAnchor.ConstraintEqualTo(InvoiceImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lbl_Head_Invoice.RightAnchor.ConstraintEqualTo(lbl_Head_Invoice.Superview.RightAnchor, -8).Active = true;
            lbl_Head_Invoice.HeightAnchor.ConstraintEqualTo(22).Active = true;

            lbl__Invoice.TopAnchor.ConstraintEqualTo(lbl_Head_Invoice.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl__Invoice.LeftAnchor.ConstraintEqualTo(InvoiceImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl__Invoice.RightAnchor.ConstraintEqualTo(lbl__Invoice.Superview.RightAnchor, -8).Active = true;
            lbl__Invoice.BottomAnchor.ConstraintEqualTo(lbl__Invoice.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;

            SelectInvoiceOfficerImg.CenterYAnchor.ConstraintEqualTo(SelectInvoiceOfficerImg.Superview.CenterYAnchor, 0).Active = true;
            SelectInvoiceOfficerImg.RightAnchor.ConstraintEqualTo(SelectInvoiceOfficerImg.Superview.RightAnchor, -20).Active = true;
            SelectInvoiceOfficerImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectInvoiceOfficerImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region CashierView
            CashierView.TopAnchor.ConstraintEqualTo(InvoiceOfficerView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            CashierView.LeftAnchor.ConstraintEqualTo(CashierView.Superview.LeftAnchor, 10).Active = true;
            CashierView.RightAnchor.ConstraintEqualTo(CashierView.Superview.RightAnchor, -10).Active = true;
            CashierView.HeightAnchor.ConstraintEqualTo(87).Active = true;

            CashierImg.CenterYAnchor.ConstraintEqualTo(CashierImg.Superview.CenterYAnchor, 0).Active = true;
            CashierImg.LeftAnchor.ConstraintEqualTo(CashierImg.Superview.LeftAnchor, 20).Active = true;
            CashierImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            CashierImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            lbl_Head_Cashier.TopAnchor.ConstraintEqualTo(lbl_Head_Cashier.Superview.TopAnchor, 19).Active = true;
            lbl_Head_Cashier.LeftAnchor.ConstraintEqualTo(CashierImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl_Head_Cashier.RightAnchor.ConstraintEqualTo(lbl_Head_Cashier.Superview.RightAnchor, -8).Active = true;
            lbl_Head_Cashier.HeightAnchor.ConstraintEqualTo(22).Active = true;

            lbl_Cashier.TopAnchor.ConstraintEqualTo(lbl_Head_Cashier.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_Cashier.LeftAnchor.ConstraintEqualTo(CashierImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lbl_Cashier.RightAnchor.ConstraintEqualTo(lbl_Cashier.Superview.RightAnchor, -8).Active = true;
            lbl_Cashier.BottomAnchor.ConstraintEqualTo(lbl_Cashier.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;

            SelectCashierImg.CenterYAnchor.ConstraintEqualTo(SelectCashierImg.Superview.CenterYAnchor, 0).Active = true;
            SelectCashierImg.RightAnchor.ConstraintEqualTo(SelectCashierImg.Superview.RightAnchor, -20).Active = true;
            SelectCashierImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectCashierImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region EditorView
            EditorView.TopAnchor.ConstraintEqualTo(CashierView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            EditorView.LeftAnchor.ConstraintEqualTo(EditorView.Superview.LeftAnchor, 10).Active = true;
            EditorView.RightAnchor.ConstraintEqualTo(EditorView.Superview.RightAnchor, -10).Active = true;
            EditorView.HeightAnchor.ConstraintEqualTo(87).Active = true;

            EditorImg.CenterYAnchor.ConstraintEqualTo(EditorImg.Superview.CenterYAnchor, 0).Active = true;
            EditorImg.LeftAnchor.ConstraintEqualTo(EditorImg.Superview.LeftAnchor, 20).Active = true;
            EditorImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            EditorImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            lbl_Head_Editor.TopAnchor.ConstraintEqualTo(lbl_Head_Editor.Superview.TopAnchor, 19).Active = true;
            lbl_Head_Editor.LeftAnchor.ConstraintEqualTo(EditorImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl_Head_Editor.RightAnchor.ConstraintEqualTo(lbl_Head_Editor.Superview.RightAnchor, -8).Active = true;
            lbl_Head_Editor.HeightAnchor.ConstraintEqualTo(22).Active = true;

            lbl_Editor.TopAnchor.ConstraintEqualTo(lbl_Head_Editor.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_Editor.LeftAnchor.ConstraintEqualTo(EditorImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lbl_Editor.RightAnchor.ConstraintEqualTo(lbl_Editor.Superview.RightAnchor, -8).Active = true;
            lbl_Editor.BottomAnchor.ConstraintEqualTo(lbl_Editor.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;

            SelectEditorImg.CenterYAnchor.ConstraintEqualTo(SelectEditorImg.Superview.CenterYAnchor, 0).Active = true;
            SelectEditorImg.RightAnchor.ConstraintEqualTo(SelectEditorImg.Superview.RightAnchor, -20).Active = true;
            SelectEditorImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectEditorImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region OfficerView
            OfficerView.TopAnchor.ConstraintEqualTo(EditorView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            OfficerView.LeftAnchor.ConstraintEqualTo(OfficerView.Superview.LeftAnchor, 10).Active = true;
            OfficerView.RightAnchor.ConstraintEqualTo(OfficerView.Superview.RightAnchor, -10).Active = true;
            OfficerView.BottomAnchor.ConstraintEqualTo(OfficerView.Superview.BottomAnchor, -10).Active = true;
            OfficerView.HeightAnchor.ConstraintEqualTo(87).Active = true;

            OfficeImg.CenterYAnchor.ConstraintEqualTo(OfficeImg.Superview.CenterYAnchor, 0).Active = true;
            OfficeImg.LeftAnchor.ConstraintEqualTo(OfficeImg.Superview.LeftAnchor, 20).Active = true;
            OfficeImg.WidthAnchor.ConstraintEqualTo(34).Active = true;
            OfficeImg.HeightAnchor.ConstraintEqualTo(34).Active = true;

            lbl_Head_Officer.TopAnchor.ConstraintEqualTo(lbl_Head_Officer.Superview.TopAnchor, 19).Active = true;
            lbl_Head_Officer.LeftAnchor.ConstraintEqualTo(OfficeImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbl_Head_Officer.RightAnchor.ConstraintEqualTo(lbl_Head_Officer.Superview.RightAnchor, -8).Active = true;
            lbl_Head_Officer.HeightAnchor.ConstraintEqualTo(22).Active = true;

            lbl_Officer.TopAnchor.ConstraintEqualTo(lbl_Head_Officer.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lbl_Officer.LeftAnchor.ConstraintEqualTo(OfficeImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lbl_Officer.RightAnchor.ConstraintEqualTo(lbl_Officer.Superview.RightAnchor, -8).Active = true;
            lbl_Officer.BottomAnchor.ConstraintEqualTo(lbl_Officer.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;

            SelectOfficerImg.CenterYAnchor.ConstraintEqualTo(SelectOfficerImg.Superview.CenterYAnchor, 0).Active = true;
            SelectOfficerImg.RightAnchor.ConstraintEqualTo(SelectOfficerImg.Superview.RightAnchor, -20).Active = true;
            SelectOfficerImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectOfficerImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            btnApplyEmpRole.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnApplyEmpRole.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnApplyEmpRole.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnApplyEmpRole.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            
        }

    }
}