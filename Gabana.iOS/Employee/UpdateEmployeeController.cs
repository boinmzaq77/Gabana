using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class UpdateEmployeeController : UIViewController
    {
        UILabel lblEmployee, lblBranch;
        UITextField txtEmployee, txtBranch;
        public static List<BranchPolicy> listChooseBranch = new List<BranchPolicy>();
        UIView EmployeeView, BottomView, PinCodeView, BranchView;
        UIButton btnAddEmployee, btnChangePass;
        UILabel lblPincode;
        UISwitch PincodeWitch;
        long BranchId;
        bool changePass;
        public static string positionEmp, oldpositionEmp;

        UIView BottomViewEdit;
        UIButton btnEditEmployee, btnDelete;

        UIView EmployeeFullnameView;
        UILabel lblEmployeeFullName;
        UITextField txtEmployeeFullName;
        

        UIView ResetPassView, NewPasswordView, ConfirmResetView;
        UILabel lblNewPass, lblConfirmResetPass;
        UITextField txtNewPass, txtConfirmResetPass;
        UIImageView btnClose;
        int UsePincode;
        List<ORM.MerchantDB.Branch> BranchList = new List<ORM.MerchantDB.Branch>();
        Model.UserAccountInfo Detail;
        UILabel lblpassWord, lblConfirmPass, lblEmployeeRole, lblComment;
        UITextField txtpassWord, txtConfirmPass, txtEmployeeRole, txtComment;
        UIView PasswordView, ConfirmPasswordView, CommentView, RoleView;
        UIImageView selectRoleView, selectBranchView;
        BranchManage branchManage = new BranchManage();
        BranchPolicyManage policyManage = new BranchPolicyManage();
        UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
        EmployeeRoleController employeeRolePage = null;
        EmployeeChooseBranchController branchChoosePage = null;
        public static string RoleSelected = null;
        public static string BranchSelect = null;

        public static bool isModifyRole = false;
        public static bool isModifyBranch = false;
        string LoginType;
        private readonly List<string> RoleList = new List<string>
        {
            // a m c e i o
            "Admin",
            "Manager",
            "Cashier",
            "Editor",
            "Invoice",
            "Officer"
            ,"Owner"
        };
        public static ORM.MerchantDB.UserAccountInfo EmployeeDetail = new ORM.MerchantDB.UserAccountInfo();
        private bool edit;
        string username = null;
        private UserAccount resultAccount;
        private bool Editchange = false;

        public UpdateEmployeeController() { }
        public UpdateEmployeeController(string Username) {
            EmployeeDetail = null;
            this.resultAccount = null;
            username = Username;
        }
        public UpdateEmployeeController(ORM.MerchantDB.UserAccountInfo Employee)
        {
            this.resultAccount = null;
            username = null;
            EmployeeDetail = Employee;
            edit = true;

        }
        public UpdateEmployeeController(UserAccount resultAccount)
        {
            this.resultAccount = resultAccount;
            EmployeeDetail = null;
            username = resultAccount.UserName;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated); 
            try
            {

            
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (isModifyRole)
            {
                txtEmployeeRole.Text = RoleSelected;
                checkbtnadd();
            }
                if (isModifyBranch)
                {
                
                    string branch = "";
                    if (BranchSelect == "All branch")
                    {
                        branch = "All branch";
                    }
                    else
                    {

                        foreach (var item in listChooseBranch)
                        {
                            if (branch == "")
                            {
                                branch += BranchList.Where(x => x.SysBranchID == item.SysBranchID).FirstOrDefault().TaxBranchName;
                            }
                            else
                            {
                                branch +=" , "+ BranchList.Where(x => x.SysBranchID == item.SysBranchID).FirstOrDefault().TaxBranchName;
                            }
                        }
                    }

                    txtBranch.Text = branch;
                    checkbtnadd();
                }
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }


        }
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", "มีการเปลี่ยนแปลต้องการบันทึกหรือไม่", UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    async alert => await BtnSave_Click3()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => this.NavigationController.PopViewController(true)));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                this.NavigationController.PopViewController(true);
            }

        }
        private async Task BtnSave_Click3()
        {
            if (await GabanaAPI.CheckNetWork())
            {
                if (EmployeeDetail==null)
                {
                    InsertEmployee();
                    isModifyRole = false;
                    isModifyBranch = false;
                }
                else
                {
                    UpdateEmployee();
                }
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "No Internet ไม่สามารถเพิ่มข้อมูลได้");
            }
            

        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            try
            {
                var view = new UIView();
                var button = new UIButton(UIButtonType.Custom);
                button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                button.SetTitle("  Back", UIControlState.Normal);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);

                LoginType = Preferences.Get("LoginType", "");
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                //employeeRolePage
                //UIBarButtonItem showRoleBtn = new UIBarButtonItem();
                //showRoleBtn.Image = UIImage.FromBundle("EmployeeRoles");
                //showRoleBtn.Clicked += (sender, e) => {
                //    // open select customer page
                //    if (employeeRolePage == null)
                //    {
                //        employeeRolePage = new EmployeeRoleController();
                //    }
                //    this.NavigationController.PushViewController(employeeRolePage, false);
                //};
                //this.NavigationItem.RightBarButtonItem = showRoleBtn;





                #region EmployeeView
                EmployeeView = new UIView();
                EmployeeView.BackgroundColor = UIColor.White;
                EmployeeView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(EmployeeView);

                lblEmployee = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblEmployee.Font = lblEmployee.Font.WithSize(15);
                lblEmployee.Text = "Employee Username";
                EmployeeView.AddSubview(lblEmployee);

                txtEmployee = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(51, 170, 225),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtEmployee.ReturnKeyType = UIReturnKeyType.Done;
                txtEmployee.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    return true;
                };
                txtEmployee.EditingChanged += (object sender, EventArgs e) =>
                {
                    checkbtnadd();
                    checkbtnedit();
                };
                txtEmployee.AttributedPlaceholder = new NSAttributedString("Username", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtEmployee.Font = txtEmployee.Font.WithSize(15);
                EmployeeView.AddSubview(txtEmployee);
                if (username != null)
                {
                    txtEmployee.Text = username;
                    txtEmployee.Enabled = false;
                    txtEmployee.TextColor = UIColor.FromRGB(162, 162, 162);
                }
                #endregion

                #region EmployeeFullnameView
                EmployeeFullnameView = new UIView();
                EmployeeFullnameView.BackgroundColor = UIColor.White;
                EmployeeFullnameView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(EmployeeFullnameView);

                lblEmployeeFullName = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblEmployeeFullName.Font = lblEmployeeFullName.Font.WithSize(15);
                lblEmployeeFullName.Text = "Full Name";
                EmployeeFullnameView.AddSubview(lblEmployeeFullName);

                txtEmployeeFullName = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(51, 170, 225),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtEmployeeFullName.ReturnKeyType = UIReturnKeyType.Next;
                txtEmployeeFullName.ShouldReturn = (tf) =>
                {
                    txtpassWord.BecomeFirstResponder();
                    return true;
                };
                txtEmployeeFullName.EditingChanged += (object sender, EventArgs e) =>
                {
                    checkbtnadd();
                    checkbtnedit();
                };
                txtEmployeeFullName.AttributedPlaceholder = new NSAttributedString("Full Name", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtEmployeeFullName.Font = txtEmployeeFullName.Font.WithSize(15);
                EmployeeFullnameView.AddSubview(txtEmployeeFullName);
                #endregion

                #region PasswordView
                PasswordView = new UIView();
                PasswordView.BackgroundColor = UIColor.White;
                PasswordView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(PasswordView);

                lblpassWord = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblpassWord.Font = lblpassWord.Font.WithSize(15);
                lblpassWord.Text = "Password";
                PasswordView.AddSubview(lblpassWord);

                txtpassWord = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtpassWord.ReturnKeyType = UIReturnKeyType.Next;
                txtpassWord.ShouldReturn = (tf) =>
                {
                    txtConfirmPass.BecomeFirstResponder();
                    return true;
                };
                txtpassWord.EditingChanged += (object sender, EventArgs e) =>
                {
                    checkbtnadd();
                    checkbtnedit();
                };
                txtpassWord.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtpassWord.Font = txtpassWord.Font.WithSize(15);
                txtpassWord.SecureTextEntry = true;
                PasswordView.AddSubview(txtpassWord);

                btnChangePass = new UIButton();
                btnChangePass.SetTitle("Reset", UIControlState.Normal);
                btnChangePass.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnChangePass.Layer.CornerRadius = 20f;
                btnChangePass.Layer.BorderWidth = 0.5f;
                btnChangePass.Layer.BorderColor = UIColor.FromRGB(225, 225, 225).CGColor;
                btnChangePass.BackgroundColor = UIColor.FromRGB(225, 225, 225);
                btnChangePass.TranslatesAutoresizingMaskIntoConstraints = false;
                btnChangePass.TouchUpInside += (sender, e) => {
                    ChangePassClick();
                };
                PasswordView.AddSubview(btnChangePass);
                #endregion

                #region ResetPassView
                ResetPassView = new UIView();
                ResetPassView.TranslatesAutoresizingMaskIntoConstraints = false;
                ResetPassView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                View.AddSubview(ResetPassView);

                #region New
                NewPasswordView = new UIView();
                NewPasswordView.TranslatesAutoresizingMaskIntoConstraints = false;
                NewPasswordView.BackgroundColor = UIColor.White;
                ResetPassView.AddSubview(NewPasswordView);

                lblNewPass = new UILabel
                {

                    TextAlignment = UITextAlignment.Left,
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblNewPass.Font = lblNewPass.Font.WithSize(15);
                lblNewPass.Text = "Old Password";
                NewPasswordView.AddSubview(lblNewPass);

                txtNewPass = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtNewPass.ReturnKeyType = UIReturnKeyType.Next;
                txtNewPass.ShouldReturn = (tf) =>
                {
                    txtConfirmResetPass.BecomeFirstResponder();
                    return true;
                };
                txtNewPass.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(226, 226, 226) });
                txtNewPass.Font = txtNewPass.Font.WithSize(15);
                txtNewPass.SecureTextEntry = true;
                NewPasswordView.AddSubview(txtNewPass);
                #endregion

                #region ConfirmResetView
                ConfirmResetView = new UIView();
                ConfirmResetView.TranslatesAutoresizingMaskIntoConstraints = false;
                ConfirmResetView.BackgroundColor = UIColor.White;
                ResetPassView.AddSubview(ConfirmResetView);

                lblConfirmResetPass = new UILabel
                {

                    TextAlignment = UITextAlignment.Left,
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblConfirmResetPass.Font = lblConfirmResetPass.Font.WithSize(15);
                lblConfirmResetPass.Text = "Confirm Password";
                ConfirmResetView.AddSubview(lblConfirmResetPass);

                txtConfirmResetPass = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtConfirmResetPass.ReturnKeyType = UIReturnKeyType.Done;
                txtConfirmResetPass.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    return true;
                };
                txtConfirmResetPass.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtConfirmResetPass.Font = txtNewPass.Font.WithSize(15);
                txtConfirmResetPass.SecureTextEntry = true;
                ConfirmResetView.AddSubview(txtConfirmResetPass);
                #endregion

                btnClose = new UIImageView();
                btnClose.Image = UIImage.FromBundle("Del2");
                btnClose.TranslatesAutoresizingMaskIntoConstraints = false;
                ResetPassView.AddSubview(btnClose);

                btnClose.UserInteractionEnabled = true;
                var tapGesture0 = new UITapGestureRecognizer(this,
                  new ObjCRuntime.Selector("close:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnClose.AddGestureRecognizer(tapGesture0);
                #endregion

                #region ConfirmPasswordView
                ConfirmPasswordView = new UIView();
                ConfirmPasswordView.BackgroundColor = UIColor.White;
                ConfirmPasswordView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(ConfirmPasswordView);

                lblConfirmPass = new UILabel
                {
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblConfirmPass.Font = lblConfirmPass.Font.WithSize(15);
                lblConfirmPass.Text = "Confirm Password";
                ConfirmPasswordView.AddSubview(lblConfirmPass);

                txtConfirmPass = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(0, 149, 218),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtConfirmPass.ReturnKeyType = UIReturnKeyType.Next;
                txtConfirmPass.SecureTextEntry = true;
                txtConfirmPass.ShouldReturn = (tf) =>
                {
                    txtEmployeeRole.BecomeFirstResponder();
                    return true;
                };
                txtConfirmPass.EditingChanged += (object sender, EventArgs e) =>
                {
                    checkbtnadd();
                    checkbtnedit();
                };
                txtConfirmPass.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtConfirmPass.Font = txtConfirmPass.Font.WithSize(15);
                ConfirmPasswordView.AddSubview(txtConfirmPass);
                #endregion

                #region RoleView
                RoleView = new UIView();
                RoleView.BackgroundColor = UIColor.White;
                RoleView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(RoleView);

                lblEmployeeRole = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblEmployeeRole.Font = lblEmployeeRole.Font.WithSize(15);
                lblEmployeeRole.Text = "Employee Roles";
                RoleView.AddSubview(lblEmployeeRole);

                txtEmployeeRole = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(162, 162, 162),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtEmployeeRole.Enabled = false;
                txtEmployeeRole.ReturnKeyType = UIReturnKeyType.Next;
                txtEmployeeRole.ShouldReturn = (tf) =>
                {
                    txtEmployeeRole.BecomeFirstResponder();
                    return true;
                };
                txtEmployeeRole.EditingChanged += (object sender, EventArgs e) =>
                {
                    //owner กับ admin เข้าได้ทุก Branch ไม่ต้องเลือกค่ะ ส่วน Manager เลือก Branch ได้มากกว่า 1 และที่เหลือ
                    //"Admin",
                    //""
                    if (txtEmployeeRole.Text == "Invoice" || txtEmployeeRole.Text == "officer" || txtEmployeeRole.Text == "Cashier" || txtEmployeeRole.Text == "Editor")
                    {
                        BranchView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    }
                    else
                    {
                        BranchView.HeightAnchor.ConstraintEqualTo(60).Active = true;
                    }
                };
                txtEmployeeRole.AttributedPlaceholder = new NSAttributedString("Please Select", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
                txtEmployeeRole.Font = txtEmployeeRole.Font.WithSize(15);
                RoleView.AddSubview(txtEmployeeRole);

                selectRoleView = new UIImageView();
                selectRoleView.Image = UIImage.FromBundle("Next");
                selectRoleView.TranslatesAutoresizingMaskIntoConstraints = false;
                RoleView.AddSubview(selectRoleView);

                selectRoleView.UserInteractionEnabled = true;
                var tapGestureSelectRole = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Roles:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                
                
                #endregion

                #region BranchView
                BranchView = new UIView();
                BranchView.BackgroundColor = UIColor.White;
                BranchView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(BranchView);

                lblBranch = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblBranch.Font = lblBranch.Font.WithSize(15);
                lblBranch.Text = "Branch";
                BranchView.AddSubview(lblBranch);

                txtBranch = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(162, 162, 162),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtBranch.Enabled = false;
                txtBranch.ReturnKeyType = UIReturnKeyType.Next;
                txtBranch.ShouldReturn = (tf) =>
                {
                    txtComment.BecomeFirstResponder();
                    return true;
                };
                txtBranch.AttributedPlaceholder = new NSAttributedString("Please Select", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
                txtBranch.Font = txtBranch.Font.WithSize(15);
                BranchView.AddSubview(txtBranch);

                selectBranchView = new UIImageView();
                selectBranchView.Image = UIImage.FromBundle("Next");
                selectBranchView.TranslatesAutoresizingMaskIntoConstraints = false;
                BranchView.AddSubview(selectBranchView);

                BranchView.UserInteractionEnabled = true;
                var tapGestureSelectBranch = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Branch:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                var emplogin = Preferences.Get("User", "");
                if (EmployeeDetail != null)
                {
                    if (EmployeeDetail?.UserName != emplogin)
                    {
                        selectRoleView.AddGestureRecognizer(tapGestureSelectRole);
                        BranchView.AddGestureRecognizer(tapGestureSelectBranch);
                    }
                }
                else
                {
                    selectRoleView.AddGestureRecognizer(tapGestureSelectRole);
                    BranchView.AddGestureRecognizer(tapGestureSelectBranch);
                }
                
                
                #endregion

                #region CommentView
                CommentView = new UIView();
                CommentView.BackgroundColor = UIColor.White;
                CommentView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(CommentView);

                lblComment = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblComment.Font = lblComment.Font.WithSize(15);
                lblComment.Text = "Comment";
                CommentView.AddSubview(lblComment);

                txtComment = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(51, 170, 225),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtComment.ReturnKeyType = UIReturnKeyType.Done;
                txtComment.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    return true;
                };
                txtComment.AttributedPlaceholder = new NSAttributedString("Comment", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtComment.Font = txtComment.Font.WithSize(15);
                CommentView.AddSubview(txtComment);
                #endregion

                #region PinCodeView
                PinCodeView = new UIView();
                PinCodeView.BackgroundColor = UIColor.White;
                PinCodeView.Hidden = true;
                PinCodeView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(PinCodeView);

                lblPincode = new UILabel
                {
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblPincode.Hidden = true;
                lblPincode.Font = lblPincode.Font.WithSize(15);
                lblPincode.Text = "Pin Code";
                PinCodeView.AddSubview(lblPincode);

                PincodeWitch = new UISwitch();
                PincodeWitch.Hidden = true;
                PincodeWitch.OnTintColor = UIColor.FromRGB(0, 149, 218);
                PincodeWitch.TranslatesAutoresizingMaskIntoConstraints = false;
                PincodeWitch.SetState(PincodeWitch.On, true);
                PincodeWitch.ValueChanged += (sender, e) =>
                {
                    if (PincodeWitch.On)
                    {
                        //enable pincode
                        EmployeeDetail.FUsePincode = 1;
                    }
                    else if (!PincodeWitch.On)
                    {
                        // disable pincode
                        EmployeeDetail.FUsePincode = 0;
                    }
                };
                PinCodeView.AddSubview(PincodeWitch);
                #endregion

                #region BottomView for Add
                BottomView = new UIView();
                BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                BottomView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(BottomView);

                btnAddEmployee = new UIButton();
                btnAddEmployee.Enabled = false;
                btnAddEmployee.SetTitle("Add Employee", UIControlState.Normal);
                btnAddEmployee.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAddEmployee.Layer.CornerRadius = 5f;
                btnAddEmployee.Layer.BorderWidth = 0.5f;
                btnAddEmployee.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnAddEmployee.BackgroundColor = UIColor.White;
                btnAddEmployee.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAddEmployee.TouchUpInside += async (sender, e) => {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        InsertEmployee();
                        isModifyRole = false;
                        isModifyBranch = false;
                    }
                    else
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "No Internet ไม่สามารถเพิ่มข้อมูลได้");
                    }
                };
                BottomView.AddSubview(btnAddEmployee);
                #endregion

                #region BottomView for Edit
                BottomViewEdit = new UIView();
                BottomViewEdit.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                BottomViewEdit.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(BottomViewEdit);

                btnDelete = new UIButton();
                btnDelete.Layer.CornerRadius = 5f;
                btnDelete.BackgroundColor = UIColor.FromRGB(226, 226, 226);
                btnDelete.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
                btnDelete.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
                btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
                btnDelete.TouchUpInside += (sender, e) => {
                    Delete();
                };
                BottomViewEdit.AddSubview(btnDelete);

                btnEditEmployee = new UIButton();
                btnEditEmployee.SetTitle("Edit Employee", UIControlState.Normal);
                btnEditEmployee.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnEditEmployee.Layer.CornerRadius = 5f;
                btnEditEmployee.Layer.BorderWidth = 0.5f;
                btnEditEmployee.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnEditEmployee.BackgroundColor = UIColor.White;
                btnEditEmployee.TranslatesAutoresizingMaskIntoConstraints = false;
                btnEditEmployee.TouchUpInside +=  (sender, e) => {
                    if (DataCashingAll.CheckConnectInternet)
                    {
                        UpdateEmployee();
                        
                    }
                    else
                    {
                        Utils.ShowMessage("No Internet ไม่สามารถแก้ไขข้อมูลได้");
                        //Toast.MakeText(this, "No Internet ไม่สามารถแก้ไขข้อมูลได้", ToastLength.Short).Show();
                    }
                    
                };
                BottomViewEdit.AddSubview(btnEditEmployee);


                #endregion

                SelectBranch();
                Textboxfocus(View);
                SetupAutoLayout();
                //SetupPicker();

                if (EmployeeDetail != null)
                {
                    BottomViewEdit.Hidden = false;
                    BottomView.Hidden = true;
                    
                    ShowUserAccountDetail();
                    PinCodeView.Hidden = false;
                    btnChangePass.Hidden = false;
                    btnChangePass.Enabled = true;
                    txtpassWord.Enabled = false;
                    txtEmployee.Enabled = false; 
                }
                else
                {
                    btnChangePass.Hidden = true;
                    BottomViewEdit.Hidden = true;
                    BottomView.Hidden = false;
                    ResetPassView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    View.LayoutIfNeeded();
                    if (this.resultAccount != null)
                    {
                        txtEmployee.Text = this.resultAccount.UserName;
                        txtEmployeeFullName.Text = this.resultAccount.FullName;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                await TinyInsights.TrackErrorAsync(ex);
            }

        }

        private void checkbtnadd()
        {
            if (txtEmployee.Text.Length > 0 
                && txtEmployeeRole.Text.Length >0 
                && txtEmployee.Text.Length > 0
                && txtpassWord.Text.Length > 0
                && txtConfirmPass.Text.Length > 0
                && txtBranch.Text.Length > 0)
            {
                btnAddEmployee.Enabled = true;
                btnAddEmployee.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                btnAddEmployee.SetTitleColor(UIColor.White, UIControlState.Normal);
                Editchange = true;
            }
            else
            {
                Editchange = false;
                btnAddEmployee.Enabled = false;
                btnAddEmployee.BackgroundColor = UIColor.White;
                btnAddEmployee.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            }
        }
        private void checkbtnedit()
        {
            btnEditEmployee.Enabled = true;
            Editchange = true;
            btnEditEmployee.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnEditEmployee.SetTitleColor(UIColor.White, UIControlState.Normal);
        }

        public async void Delete()
        {
            try
            {
                var okCancelAlertController = UIAlertController.Create("", "Are you sure you want to delete employee?", UIAlertControllerStyle.Alert);

                //Add Actions
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => DeleteEmp()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                //Present Alert
                PresentViewController(okCancelAlertController, true, null);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูล Customer ได้");
                return;
            }
            
        }

        private async void DeleteEmp()
        {
            try
            {
                var mainrole = DataCashingAll.UserAccountInfo.Where(x => x.UserName == EmployeeDetail.UserName & x.MainRoles.ToLower() == "owner").FirstOrDefault();
                if (mainrole != null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบได้ เนื่องจาก สถานะเป็นเจ้าของ");
                    return;
                }

                if (EmployeeDetail != null)
                {
                    var result = await GabanaAPI.DeleteSeAuthDataUserAccount(EmployeeDetail.UserName);
                    if (!result.Status)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), result.Message);
                        return;
                    }

                    var resultGabana = await GabanaAPI.DeleteDataUserAccount(EmployeeDetail.UserName);
                    if (!resultGabana.Status)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), resultGabana.Message);
                        return;
                    }

                    //Delete BranchPolicy ของ employee 
                    var lstbranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, EmployeeDetail.UserName);
                    foreach (var item in lstbranchPolicy)
                    {
                        var delete = await policyManage.DeleteBranch(DataCashingAll.MerchantId, EmployeeDetail.UserName);
                    }

                    //Delete useraccount
                    var resultLocal = await accountInfoManage.DeleteUserAccount(DataCashingAll.MerchantId, EmployeeDetail.UserName);
                    if (!resultLocal)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูลพนักงานได้");
                        return;
                    }

                    if (resultLocal)
                    {
                        var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName == txtEmployee.Text);
                        DataCashingAll.UserAccountInfo.Remove(data);
                        Utils.ShowMessage("ลบพนักงานสำเร็จ");
                        Editchange = false;
                        this.NavigationController.PopViewController(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        public async void SelectBranch()
        {
            try
            {
                BranchList = await branchManage.GetAllBranch(DataCashingAll.MerchantId);

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        private void ChangePassClick()
        {
            changePass = true;
            txtpassWord.Enabled = false;
            btnChangePass.Enabled = false;
            txtConfirmResetPass.Text = "";
            txtNewPass.Text = "";
            Utils.SetConstant(ResetPassView.Constraints, NSLayoutAttribute.Height, 150);
            //  ResetPassView.HeightAnchor.ConstraintEqualTo(160).Active = true;
            View.LayoutIfNeeded();
        }
        public async void ShowUserAccountDetail()
        {
            try
            {
                ConfirmPasswordView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                if (DataCashingAll.UserAccountInfo != null)
                {
                    Detail = DataCashingAll.UserAccountInfo.Where(x => x.UserName == EmployeeDetail.UserName).FirstOrDefault();
                    if (Detail != null)
                    {
                        txtEmployee.Text = EmployeeDetail.UserName;
                        txtpassWord.Text = string.Empty;
                        txtComment.Text = EmployeeDetail.Comments;
                        txtEmployeeFullName.Text = Detail.FullName;

                        positionEmp = Detail.MainRoles;
                        oldpositionEmp = Detail.MainRoles;
                        RoleSelected = Detail.MainRoles;
                        // a m c e i o
                        var LoginType = Preferences.Get("LoginType", "");
                        if (Detail.MainRoles.ToLower() == "admin")
                        {
                            txtEmployeeRole.Text = RoleList[0];
                            if (LoginType.ToLower() == "admin")
                            {
                                btnDelete.WidthAnchor.ConstraintEqualTo(0).Active = true;
                                btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                                btnDelete.Hidden = true;
                            }
                        }
                        else if (Detail.MainRoles.ToLower() == "manager")
                        {
                            txtEmployeeRole.Text = RoleList[1];
                            if (LoginType.ToLower() == "manager")
                            {
                                btnDelete.WidthAnchor.ConstraintEqualTo(0).Active = true;
                                btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                                btnDelete.Hidden = true;
                            }
                        }
                        else if (Detail.MainRoles.ToLower() == "cashier")
                        {
                            txtEmployeeRole.Text = RoleList[2];
                            if (LoginType.ToLower() == "cashier")
                            {
                                btnDelete.WidthAnchor.ConstraintEqualTo(0).Active = true;
                                btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                                btnDelete.Hidden = true;
                            }
                        }
                        else if (Detail.MainRoles.ToLower() == "editor")
                        {
                            txtEmployeeRole.Text = RoleList[3];
                            if (LoginType.ToLower() == "editor")
                            {
                                btnDelete.WidthAnchor.ConstraintEqualTo(0).Active = true;
                                btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                                btnDelete.Hidden = true;
                            }
                        }
                        else if (Detail.MainRoles.ToLower() == "invoice")
                        {
                            txtEmployeeRole.Text = RoleList[4];
                            if (LoginType.ToLower() == "invoice")
                            {
                                btnDelete.WidthAnchor.ConstraintEqualTo(0).Active = true;
                                btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                                btnDelete.Hidden = true;
                            }
                        }

                        else if (Detail.MainRoles.ToLower() == "officer")
                        {
                            txtEmployeeRole.Text = RoleList[5];
                            if (LoginType.ToLower() == "officer")
                            {
                                btnDelete.WidthAnchor.ConstraintEqualTo(0).Active = true;
                                btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                                btnDelete.Hidden = true;
                            }
                        }
                        else
                        {
                            //Owner
                            txtEmployeeRole.Text = RoleList[6];
                            PasswordView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                            ResetPassView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                            RoleView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                            BranchView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                            selectBranchView.Hidden = true;
                            selectRoleView.Hidden = true;
                            btnDelete.WidthAnchor.ConstraintEqualTo(0).Active = true;
                            btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                            btnDelete.Hidden = true;
                            
                            Utils.SetConstant(ResetPassView.Constraints, NSLayoutAttribute.Height, 0);
                            ResetPassView.Hidden = true;
                            btnChangePass.Hidden = true;
                            btnChangePass.HeightAnchor.ConstraintEqualTo(0).Active = true;
                            btnChangePass.SetTitle("",UIControlState.Normal);
                            
                            if (LoginType.ToLower() == "admin" )
                            {
                                BottomViewEdit.Hidden = true; 
                            }
                            //ConfirmPasswordView.HeightAnchor.ConstraintEqualTo(0).Active = true;

                        }

                        //Switch
                        if (EmployeeDetail.FUsePincode == 1)
                        {
                            PincodeWitch.SetState(PincodeWitch.On, true);
                        }
                        else
                        {
                            PincodeWitch.SetState(PincodeWitch.On, false);
                        }
                        BranchPolicyManage policyManage = new BranchPolicyManage();
                        listChooseBranch = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, Detail.UserName);
                        
                        if (positionEmp.ToLower() == "owner" | positionEmp.ToLower() == "admin")
                        {
                            txtBranch.Text = "ทุกสาขา";
                            //   textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        }
                        else
                        {
                            string branch = "";
                            foreach (var item in listChooseBranch)
                            {
                                if (branch == "")
                                {
                                    branch += BranchList.Where(x => x.SysBranchID == item.SysBranchID).FirstOrDefault().TaxBranchName;
                                }
                                else
                                {
                                    branch += " , " + BranchList.Where(x => x.SysBranchID == item.SysBranchID).FirstOrDefault().TaxBranchName;
                                }
                            }
                            txtBranch.Text = branch;
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        [Export("close:")]
        public void close(UIGestureRecognizer sender)
        {
            changePass = false;
            txtpassWord.Enabled = true;
            btnChangePass.Enabled = true;
            txtConfirmResetPass.Text = "";
            txtNewPass.Text = "";
            Utils.SetConstant(ResetPassView.Constraints, NSLayoutAttribute.Height, 0);
            View.LayoutIfNeeded();
        }
        public async void InsertEmployee()
        {
            try
            {
                string password = string.Empty;
                string confirm = string.Empty;
                string comment = string.Empty;

                comment = txtComment.Text;
                password = txtpassWord.Text.Trim();
                confirm = txtConfirmPass.Text.Trim();

                if (password != confirm)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกรหัสผ่านให้ตรงกัน");
                    return;
                }
                var empRole = "";
                if (txtEmployeeRole.Text.ToLower() == "invoice officer")
                {
                    empRole = "invoice";
                    txtEmployeeRole.Text = empRole;
                }
                

                Gabana.Model.UserAccount APIUser = new Model.UserAccount()
                {
                    UserName = txtEmployee.Text,
                    FullName = txtEmployeeFullName.Text, //Not null
                    MainRoles = txtEmployeeRole.Text,
                    PasswordHash = password.ToString(),
                };
                if (this.resultAccount != null)
                {
                    List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();
                    //GabanaAPI

                    if (txtEmployeeRole.Text.ToLower() == "admin")
                    {
                        listChooseBranch = new List<BranchPolicy>();
                    }
                    else
                    {
                        if (listChooseBranch.Count == 0)
                        {
                            Utils.ShowMessage("กรุณาเลือกสาขาให้พนักงาน");
                            return;
                        }
                    }

                    foreach (var item in listChooseBranch)
                    {
                        ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            UserName = txtEmployeeFullName.Text,
                            SysBranchID = (int)item.SysBranchID
                        };
                        lstbranchPolicies.Add(branchPolicy);
                    }

                    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployee.Text,
                        FUsePincode = 0,
                        PinCode = null,
                        Comments = comment,
                    };

                    Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                    {
                        branchPolicy = lstbranchPolicies,
                        userAccountInfo = gbnAPIUser
                    };
                    var postgbnAPIUser = await GabanaAPI.PostDataUserAccount(userAccountResult);
                    if (postgbnAPIUser.Status)
                    {
                        //Local
                        ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            UserName = txtEmployee.Text,
                            FUsePincode = 0,
                            PinCode = null,
                            Comments = comment
                        };

                        //insert Local useraccount
                        var Updatetlocal = await accountInfoManage.UpdateUserAccount(localUser);
                        if (Updatetlocal)
                        {
                            //insert Local BranchPolicy
                            foreach (var itembranch in lstbranchPolicies)
                            {
                                ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                {
                                    MerchantID = itembranch.MerchantID,
                                    SysBranchID = (int)itembranch.SysBranchID,
                                    UserName = itembranch.UserName,
                                };
                                var insertlocalbranchPolicy = await policyManage.InsertBranchPolicy(branchPolicy);
                            }

                            Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                            {
                                UserName = txtEmployee.Text,
                                MainRoles = positionEmp,
                                UserAccessProduct = true,
                                FullName = txtEmployeeFullName.Text,
                            };

                            DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                            ClearText();
                            EmployeeController.Ismodify = true;
                            Editchange = false;
                            this.NavigationController.PopToViewController(DataCaching.employeeController, false);
                            Utils.ShowAlert(this, "สำเร็จ !", "เพิ่มข้อมูลสำเร็จ");
                        }

                        if (!Updatetlocal)
                        {
                            return;
                        }
                    }

                    //insert ที่ gabanaAPI ไม่เสร็จ ลบที่ Seauth ด้วย
                    if (!postgbnAPIUser.Status)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), postgbnAPIUser.Message);
                        var result = await GabanaAPI.DeleteSeAuthDataUserAccount(txtEmployee.Text);
                        return;
                    }

                }
                else
                {
                    var resultInsert = await GabanaAPI.PostSeAuthDataUserAccount(APIUser);
                    if (!resultInsert.Status)
                    {
                        //Pop up Error
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), resultInsert.Message);
                        return;
                    }

                    if (resultInsert.Status)
                    {
                        //สิทธิ์ในการ Login Branch
                        // Owner , Admin จะได้ All Branch ไม่ต้อง Add ข้อมูลที่ Table นี้
                        // Manager: กำหนดได้มากกว่า 1 row
                        // สิทธิ์ที่ต่ำกว่า Manager กำหนดได้แค่ 1 row เท่านั้น

                        List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();
                        //GabanaAPI

                        if (RoleSelected.ToLower() == "admin")
                        {
                            listChooseBranch = new List<BranchPolicy>();
                        }
                        else
                        {
                            if (listChooseBranch.Count == 0)
                            {
                                Utils.ShowMessage("กรุณาเลือกสาขาให้พนักงาน");
                                return;
                            }
                        }


                        foreach (var item in listChooseBranch)
                        {
                            ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                UserName = txtEmployeeFullName.Text,
                                SysBranchID = (int)item.SysBranchID
                            };
                            lstbranchPolicies.Add(branchPolicy);
                        }

                        ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            UserName = txtEmployee.Text,
                            FUsePincode = 0,
                            PinCode = null,
                            Comments = comment,
                        };

                        Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                        {
                            branchPolicy = lstbranchPolicies,
                            userAccountInfo = gbnAPIUser
                        };

                        //GabanaAPI
                        var postgbnAPIUser = await GabanaAPI.PostDataUserAccount(userAccountResult);
                        if (postgbnAPIUser.Status)
                        {
                            //Local
                            ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                UserName = txtEmployee.Text,
                                FUsePincode = 0,
                                PinCode = null,
                                Comments = comment
                            };

                            //insert Local useraccount
                            var insertlocal = await accountInfoManage.InsertUserAccount(localUser);
                            if (insertlocal)
                            {
                                //insert Local BranchPolicy
                                foreach (var itembranch in lstbranchPolicies)
                                {
                                    ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                    {
                                        MerchantID = itembranch.MerchantID,
                                        SysBranchID = (int)itembranch.SysBranchID,
                                        UserName = itembranch.UserName,
                                    };
                                    var insertlocalbranchPolicy = await policyManage.InsertBranchPolicy(branchPolicy);
                                }

                                Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                                {
                                    UserName = txtEmployee.Text,
                                    MainRoles = positionEmp,
                                    UserAccessProduct = true,
                                    FullName = txtEmployeeFullName.Text,
                                    
                                };

                                DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                                ClearText();
                                EmployeeController.Ismodify = true;
                                Editchange = false; 
                                this.NavigationController.PopToViewController(DataCaching.employeeController, false);
                                Utils.ShowAlert(this, "สำเร็จ !", "เพิ่มข้อมูลสำเร็จ");
                            }

                            if (!insertlocal)
                            {
                                return;
                            }
                        }

                        //insert ที่ gabanaAPI ไม่เสร็จ ลบที่ Seauth ด้วย
                        if (!postgbnAPIUser.Status)
                        {
                            Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), postgbnAPIUser.Message);
                            var result = await GabanaAPI.DeleteSeAuthDataUserAccount(txtEmployee.Text);
                            
                            return;
                        }
                    }
                }

                    
                
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), ex.Message);
                return;
            }



        }
        public async void UpdateEmployee()
        {

            try
            {
                //Send to Ownerchange
                string password = string.Empty;
                string confirm = string.Empty;
                string comment = string.Empty;
                string Newpass = string.Empty;

                comment = txtComment.Text;
                password = txtpassWord.Text;
                Newpass = txtNewPass.Text;
                confirm = txtConfirmResetPass.Text;
                if (changePass)
                {
                    if (Newpass != confirm)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกรหัสผ่านให้ตรงกัน");
                        return;
                    }
                    Model.ChangePassword changePassword = new Model.ChangePassword();
                    changePassword.Username = txtEmployee.Text;
                    changePassword.OldPassword = string.Empty;
                    changePassword.Password = Newpass;
                    changePassword.ConfirmPassword = confirm;

                    var result = await GabanaAPI.PutSeAuthDataOnwerChangePassword(changePassword);
                    if (!result.Status)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), result.Message);
                        return;
                    }
                }

                if (txtEmployeeRole.Text.ToLower() == "invoice")
                {
                    var newstr = "Invoice";
                    txtEmployeeRole.Text = newstr;
                }
                // Send to Seauth Put UserAccount
                Gabana.Model.UserAccount APIUser = new Model.UserAccount()
                {
                    UserName = txtEmployee.Text,
                    FullName = txtEmployeeFullName.Text, //Not null
                    MainRoles = txtEmployeeRole.Text,
                    PasswordHash = password.ToString(),
                };
                Model.ResultAPI resultUpdate;
                if (LoginType.ToLower() == "owner" | LoginType.ToLower() == "admin")
                {
                    //APIUser.MainRoles = "Admin";
                    resultUpdate = await GabanaAPI.PutSeAuthDataUserAccount(APIUser);
                }
                else
                {
                    resultUpdate = await GabanaAPI.PutSeAuthEmployeeDataUserAccount(APIUser);
                }
                //var resultUpdate = await GabanaAPI.PutSeAuthDataUserAccount(APIUser);
                if (!resultUpdate.Status)
                {
                    //Pop up Error
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), resultUpdate.Message);
                    return;
                }

                if (resultUpdate.Status)
                {
                    //GabanaAPI
                    List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();

                    if (txtEmployeeRole.Text.ToLower() == "admin" )
                    {
                        listChooseBranch = new List<BranchPolicy>();
                    }
                    else
                    {
                        if (listChooseBranch.Count == 0)
                        {
                            Utils.ShowMessage("กรุณาเลือกสาขาให้พนักงาน");
                            return;
                        }
                    }
                    //การเปลี่ยนสิทธิ์ลดระดับลงจะทำการ Clear BranchPolicy ของ User นั้นทั้งหมด 
                    //จาก admin , manager -> อื่นๆ

                    if (isModifyBranch || isModifyRole)
                    {
                        var result = await GabanaAPI.DeleteDataBranchPolicy(txtEmployee.Text);
                        if (result.Status)
                        {
                            var lstbranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, txtEmployee.Text);
                            foreach (var item in lstbranchPolicy)
                            {
                                var delete = await policyManage.DeleteBranch(DataCashingAll.MerchantId, txtEmployee.Text);
                            }
                        }
                        else
                        {
                            Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), resultUpdate.Message);
                            return;
                        }
                        foreach (var item in listChooseBranch)
                        {
                            ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                UserName = txtEmployee.Text,
                                SysBranchID = (int)item.SysBranchID
                            };
                            lstbranchPolicies.Add(branchPolicy);
                        }
                    }

                    

                    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployee.Text,
                        FUsePincode = (int)EmployeeDetail.FUsePincode,
                        PinCode = null,
                        Comments = comment
                    };

                    

                    Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                    {
                        branchPolicy = lstbranchPolicies,
                        userAccountInfo = gbnAPIUser
                    };

                    var postgbnAPIUser = await GabanaAPI.PutDataUserAccount(userAccountResult);
                    if (postgbnAPIUser.Status)
                    {
                        // Updatelocal
                        ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            UserName = txtEmployee.Text,
                            FUsePincode = (int)EmployeeDetail.FUsePincode,
                            PinCode = null,
                            Comments = comment
                        };

                        var Updatelocal = await accountInfoManage.UpdateUserAccount(localUser);
                        if (Updatelocal)
                        {
                            foreach (var itembranch in lstbranchPolicies)
                            {
                                ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                {
                                    MerchantID = itembranch.MerchantID,
                                    SysBranchID = (int)itembranch.SysBranchID,
                                    UserName = itembranch.UserName,
                                };
                                var updatelocalbranchPolicy = await policyManage.UpdateBranchPolicy(branchPolicy);
                            }

                            Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                            {
                                UserName = txtEmployee.Text,
                                MainRoles = txtEmployeeRole.Text,
                                UserAccessProduct = Detail.UserAccessProduct,
                                FullName = txtEmployeeFullName.Text,
                                
                            };

                            EmployeeDetail = null;
                            var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName == txtEmployee.Text);
                            DataCashingAll.UserAccountInfo.Remove(data);
                            DataCashingAll.UserAccountInfo.Add(UserAccountInfo);

                            var data1 = DataCashingAll.Merchant.UserAccountInfo.Find(x => x.UserName == txtEmployee.Text);
                            DataCashingAll.Merchant.UserAccountInfo.Remove(data1);
                            DataCashingAll.Merchant.UserAccountInfo.Add(gbnAPIUser);
                            ClearText();

                            EmployeeController.Ismodify = true;
                            Editchange = false;
                            this.NavigationController.PopToViewController(DataCaching.employeeController, false);
                            Utils.ShowAlert(this, "สำเร็จ !", "แก้ไขข้อมูลสำเร็จ");
                            isModifyRole = false;
                            isModifyBranch = false;
                        }

                        if (!Updatelocal)
                        {
                            Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถแก้ไขพนักงานได้");
                            return;
                        }
                    }

                    if (!postgbnAPIUser.Status)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), postgbnAPIUser.Message);
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), ex.Message);
                return;
            }

        }
        void ClearText()
        {
            listChooseBranch = new List<BranchPolicy>();
            txtEmployeeRole.Text = string.Empty;
        }
        [Export("Roles:")]
        public void Roles(UIGestureRecognizer sender)
        {

            RoleSelected = txtEmployeeRole.Text;
            employeeRolePage = new EmployeeRoleController();
            
            this.NavigationController.PushViewController(employeeRolePage, false);
        }
        [Export("Branch:")]
        public void BranchFocus(UIGestureRecognizer sender)
        {
            if (string.IsNullOrEmpty(RoleSelected))
            {
                Utils.ShowMessage("กรุณาเลือกตำแหน่งก่อน");
                return;
            }
            if (RoleSelected == "Admin")
            {
                return;
            }
            BranchSelect = txtBranch.Text;
            branchChoosePage = new EmployeeChooseBranchController();
            this.NavigationController.PushViewController(branchChoosePage, false);
        }
        void SetupAutoLayout()
        {
            #region EmployeeView
            EmployeeView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            EmployeeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            EmployeeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            EmployeeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblEmployee.TopAnchor.ConstraintEqualTo(EmployeeView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblEmployee.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblEmployee.LeftAnchor.ConstraintEqualTo(EmployeeView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblEmployee.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtEmployee.TopAnchor.ConstraintEqualTo(lblEmployee.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtEmployee.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtEmployee.LeftAnchor.ConstraintEqualTo(EmployeeView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtEmployee.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region EmployeeFullnameView
            EmployeeFullnameView.TopAnchor.ConstraintEqualTo(EmployeeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            EmployeeFullnameView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            EmployeeFullnameView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            EmployeeFullnameView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblEmployeeFullName.TopAnchor.ConstraintEqualTo(EmployeeFullnameView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblEmployeeFullName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblEmployeeFullName.LeftAnchor.ConstraintEqualTo(EmployeeFullnameView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblEmployeeFullName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtEmployeeFullName.TopAnchor.ConstraintEqualTo(lblEmployeeFullName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtEmployeeFullName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtEmployeeFullName.LeftAnchor.ConstraintEqualTo(EmployeeFullnameView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtEmployeeFullName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region PasswordView
            PasswordView.TopAnchor.ConstraintEqualTo(EmployeeFullnameView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            PasswordView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            PasswordView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PasswordView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblpassWord.TopAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblpassWord.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblpassWord.LeftAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblpassWord.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtpassWord.TopAnchor.ConstraintEqualTo(lblpassWord.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtpassWord.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtpassWord.LeftAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtpassWord.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnChangePass.CenterYAnchor.ConstraintEqualTo(btnChangePass.Superview.CenterYAnchor).Active = true;
            btnChangePass.WidthAnchor.ConstraintEqualTo(85).Active = true;
            btnChangePass.RightAnchor.ConstraintEqualTo(btnChangePass.Superview.RightAnchor, -15).Active = true;
            btnChangePass.HeightAnchor.ConstraintEqualTo(35).Active = true;

            #endregion

            #region ResetPassView
            ResetPassView.TopAnchor.ConstraintEqualTo(PasswordView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            //     ResetPassView.HeightAnchor.ConstraintEqualTo(150).Active = true;
            ResetPassView.HeightAnchor.ConstraintEqualTo(0).Active = true;
            ResetPassView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ResetPassView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region NewPass
            NewPasswordView.TopAnchor.ConstraintEqualTo(ResetPassView.SafeAreaLayoutGuide.TopAnchor, 15).Active = true;
            NewPasswordView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            NewPasswordView.LeftAnchor.ConstraintEqualTo(ResetPassView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            NewPasswordView.RightAnchor.ConstraintEqualTo(ResetPassView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            lblNewPass.TopAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblNewPass.RightAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblNewPass.LeftAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtNewPass.TopAnchor.ConstraintEqualTo(lblNewPass.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtNewPass.RightAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            txtNewPass.LeftAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            #endregion

            #region ConfirmResetView
            ConfirmResetView.TopAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            ConfirmResetView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            ConfirmResetView.LeftAnchor.ConstraintEqualTo(ResetPassView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            ConfirmResetView.RightAnchor.ConstraintEqualTo(ResetPassView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            lblConfirmResetPass.TopAnchor.ConstraintEqualTo(ConfirmResetView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblConfirmResetPass.RightAnchor.ConstraintEqualTo(ConfirmResetView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblConfirmResetPass.LeftAnchor.ConstraintEqualTo(ConfirmResetView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtConfirmResetPass.TopAnchor.ConstraintEqualTo(lblConfirmResetPass.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtConfirmResetPass.RightAnchor.ConstraintEqualTo(ConfirmResetView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            txtConfirmResetPass.LeftAnchor.ConstraintEqualTo(ConfirmResetView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            #endregion

            btnClose.TopAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.TopAnchor, -7).Active = true;
            btnClose.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnClose.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnClose.RightAnchor.ConstraintEqualTo(NewPasswordView.SafeAreaLayoutGuide.RightAnchor, -25).Active = true;
            #endregion

            #region ConfirmPasswordView
            ConfirmPasswordView.TopAnchor.ConstraintEqualTo(ResetPassView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            ConfirmPasswordView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            ConfirmPasswordView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ConfirmPasswordView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblConfirmPass.TopAnchor.ConstraintEqualTo(ConfirmPasswordView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblConfirmPass.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblConfirmPass.LeftAnchor.ConstraintEqualTo(ConfirmPasswordView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblConfirmPass.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtConfirmPass.TopAnchor.ConstraintEqualTo(lblConfirmPass.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtConfirmPass.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtConfirmPass.LeftAnchor.ConstraintEqualTo(ConfirmPasswordView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtConfirmPass.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region RoleView
            RoleView.TopAnchor.ConstraintEqualTo(ConfirmPasswordView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            RoleView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            RoleView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            RoleView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblEmployeeRole.TopAnchor.ConstraintEqualTo(RoleView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblEmployeeRole.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblEmployeeRole.LeftAnchor.ConstraintEqualTo(RoleView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblEmployeeRole.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtEmployeeRole.TopAnchor.ConstraintEqualTo(lblEmployeeRole.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtEmployeeRole.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtEmployeeRole.LeftAnchor.ConstraintEqualTo(RoleView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtEmployeeRole.HeightAnchor.ConstraintEqualTo(18).Active = true;

            selectRoleView.CenterYAnchor.ConstraintEqualTo(RoleView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            selectRoleView.WidthAnchor.ConstraintEqualTo(28).Active = true;
            selectRoleView.RightAnchor.ConstraintEqualTo(RoleView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            selectRoleView.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #endregion

            #region BranchView
            BranchView.TopAnchor.ConstraintEqualTo(RoleView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            BranchView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BranchView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            BranchView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblBranch.TopAnchor.ConstraintEqualTo(BranchView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblBranch.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblBranch.LeftAnchor.ConstraintEqualTo(BranchView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtBranch.TopAnchor.ConstraintEqualTo(lblBranch.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtBranch.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtBranch.LeftAnchor.ConstraintEqualTo(BranchView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;

            selectBranchView.CenterYAnchor.ConstraintEqualTo(BranchView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            selectBranchView.WidthAnchor.ConstraintEqualTo(28).Active = true;
            selectBranchView.RightAnchor.ConstraintEqualTo(BranchView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            selectBranchView.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #endregion

            #region CommentView
            CommentView.TopAnchor.ConstraintEqualTo(BranchView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            CommentView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            CommentView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CommentView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblComment.TopAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblComment.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblComment.LeftAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblComment.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtComment.TopAnchor.ConstraintEqualTo(lblComment.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtComment.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtComment.LeftAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtComment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region PinCodeView
            PinCodeView.TopAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
       //     PinCodeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            PinCodeView.HeightAnchor.ConstraintEqualTo(0).Active = true;
            PinCodeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PinCodeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblPincode.CenterYAnchor.ConstraintEqualTo(PinCodeView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblPincode.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblPincode.LeftAnchor.ConstraintEqualTo(PinCodeView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            //  lblPincode.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblPincode.HeightAnchor.ConstraintEqualTo(0).Active = true;

            PincodeWitch.CenterYAnchor.ConstraintEqualTo(PinCodeView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            PincodeWitch.WidthAnchor.ConstraintEqualTo(50).Active = true;
            PincodeWitch.RightAnchor.ConstraintEqualTo(PinCodeView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            PincodeWitch.HeightAnchor.ConstraintEqualTo(0).Active = true;
            // PincodeWitch.HeightAnchor.ConstraintEqualTo(30).Active = true;
            #endregion

            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddEmployee.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddEmployee.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAddEmployee.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddEmployee.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            BottomViewEdit.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomViewEdit.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomViewEdit.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomViewEdit.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(45).Active = true;

            btnEditEmployee.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnEditEmployee.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnEditEmployee.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnEditEmployee.RightAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        public class PickerModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;


            private readonly IList<string> values;
            public PickerModel(IList<string> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)];
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs { SelectedValue = values[Convert.ToInt32(row)] });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        public class PickerModelBranch : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;


            private readonly List<Gabana.ORM.MerchantDB.Branch> values;
            public PickerModelBranch(List<Gabana.ORM.MerchantDB.Branch> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)].BranchName;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].BranchName,
                        ID = (int)values[Convert.ToInt32(row)].SysBranchID
                    });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        
    }
   
}