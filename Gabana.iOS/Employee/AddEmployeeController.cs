using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class AddEmployeeController : UIViewController
    {
        UILabel lblEmployee,lblNoteMark;
        UITextField txtEmployee;
        
        UIView EmployeeView, BottomView;
        UIButton btnAddEmployee;
        UpdateEmployeeController empUpdatePage = null;
        Model.UserAccount resultAccount; 
        UIView dialogBackgroundView, dialogView;
        UILabel lblCheckUserDialog;
        UIButton btnOKUserData, btnCancelUserData;
        UserAccountInfoManage UserAccountInfoManager = new UserAccountInfoManage();

        public AddEmployeeController() 
        {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            try
            {

                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                
                

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
                lblEmployee.Text = Utils.TextBundle("employeeusername", "Employee Username");
                EmployeeView.AddSubview(lblEmployee);

                txtEmployee = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB( 51, 170,225),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    AutocapitalizationType = UITextAutocapitalizationType.None
                };
                txtEmployee.ReturnKeyType = UIReturnKeyType.Done;
                txtEmployee.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    return true;
                };
                txtEmployee.EditingChanged += (object sender, EventArgs e) =>
                {
                    if (txtEmployee.Text.Length > 0)
                    {
                        btnAddEmployee.Enabled = true;
                        btnAddEmployee.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                        btnAddEmployee.SetTitleColor(UIColor.White, UIControlState.Normal);
                    }
                    else
                    {
                        btnAddEmployee.Enabled = false;
                        btnAddEmployee.BackgroundColor = UIColor.White;
                        btnAddEmployee.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    }
                };
                txtEmployee.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("username", "Username"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtEmployee.Font = txtEmployee.Font.WithSize(15);
                EmployeeView.AddSubview(txtEmployee);
                #endregion

                lblNoteMark = new UILabel
                {
                    TextAlignment = UITextAlignment.Left,
                    TextColor =  UIColor.FromRGB(200,200,200),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblNoteMark.Font = lblNoteMark.Font.WithSize(15);
                lblNoteMark.Text = Utils.TextBundle("maxcharpassword", "*Max 32 characters with a Lowercase letters,\n numbers , @ and Underscore.");
                lblNoteMark.Lines = 2;
                View.AddSubview(lblNoteMark);

                #region BottomView for check
                BottomView = new UIView();
                BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                BottomView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(BottomView);

                btnAddEmployee = new UIButton();
                btnAddEmployee.Enabled = false;
                btnAddEmployee.SetTitle(Utils.TextBundle("next", "Next"), UIControlState.Normal);
                btnAddEmployee.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
                btnAddEmployee.Layer.CornerRadius = 5f;
                btnAddEmployee.Layer.BorderWidth = 0.5f;
                btnAddEmployee.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnAddEmployee.BackgroundColor = UIColor.White;
                btnAddEmployee.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAddEmployee.TouchUpInside += (sender, e) => {
                     checkUsername();
                };
                View.AddSubview(btnAddEmployee);
                #endregion

                #region add note CategoryView
                dialogBackgroundView = new UIView();
                dialogBackgroundView.BackgroundColor = UIColor.FromRGB(25,25,25);
                dialogBackgroundView.Hidden = true;
                dialogBackgroundView.TranslatesAutoresizingMaskIntoConstraints = false;
                dialogBackgroundView.Layer.Opacity = 0.7f;
                View.AddSubview(dialogBackgroundView);

                dialogView = new UIView();
                dialogView.Hidden = true;
                dialogView.BackgroundColor = UIColor.FromRGB(255, 255, 255);
                dialogView.Layer.CornerRadius = 13;
                dialogView.ClipsToBounds = true;
                dialogView.TranslatesAutoresizingMaskIntoConstraints = false;
                // dialogBackgroundView.AddSubview(dialogView);
                View.AddSubview(dialogView);


                lblCheckUserDialog = new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
              //  lblCheckUserDialog.Font = UIFont.BoldSystemFontOfSize(15);
                lblCheckUserDialog.Font = lblCheckUserDialog.Font.WithSize(13);
                lblCheckUserDialog.Lines = 2;
                lblCheckUserDialog.LineBreakMode = UILineBreakMode.WordWrap;
                dialogView.AddSubview(lblCheckUserDialog);

                btnOKUserData = new UIButton();
                btnOKUserData.TranslatesAutoresizingMaskIntoConstraints = false;
                btnOKUserData.SetTitle(Utils.TextBundle("textok", "OK"), UIControlState.Normal);
                btnOKUserData.Layer.BorderColor = UIColor.FromRGB(226,226,226).CGColor;
                btnOKUserData.Layer.BorderWidth = 1;
                btnOKUserData.Layer.CornerRadius = 5;
                btnOKUserData.ClipsToBounds = true;
                btnOKUserData.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnOKUserData.TouchUpInside += async (sender, e) =>
                {
                    //add
                    //pop up ถามว่าใช้คนๆนี้ไหม ถ้าใช่ จบ ไม่ใช่ไป AddEmployeeActivity
                    //var getUser = await UserAccountInfoManager.GetUserAccount((int)MainController.merchantlocal.MerchantID,txtEmployee.Text);
                    if (empUpdatePage == null)
                    {
                        empUpdatePage = new UpdateEmployeeController(resultAccount);
                    }
                    this.NavigationController.PushViewController(empUpdatePage, false);
                };
                dialogView.AddSubview(btnOKUserData);

                btnCancelUserData = new UIButton();
                btnCancelUserData.TranslatesAutoresizingMaskIntoConstraints = false;
                btnCancelUserData.SetTitle(Utils.TextBundle("textcancel", "Cancel"), UIControlState.Normal);
                btnCancelUserData.Layer.BorderColor = UIColor.FromRGB(226,226,226).CGColor;
                btnCancelUserData.Layer.BorderWidth = 1;
                btnCancelUserData.Layer.CornerRadius = 5;
                btnCancelUserData.ClipsToBounds = true;
                btnCancelUserData.SetTitleColor(UIColor.FromRGB(64,64,64), UIControlState.Normal);
                btnCancelUserData.TouchUpInside += (sender, e) =>
                {
                    dialogView.Hidden = true;
                    dialogBackgroundView.Hidden = true;
                };
                dialogView.AddSubview(btnCancelUserData);

                dialogBackgroundView.UserInteractionEnabled = true;
                var tapGesture0 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Back:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                dialogBackgroundView.AddGestureRecognizer(tapGesture0);

                #endregion

                Textboxfocus(View);
                SetupAutoLayout();
            }
            catch (Exception ex)
            {

                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        [Export("Back:")]
        public void Search(UIGestureRecognizer sender)
        {
            dialogBackgroundView.Hidden = true;
            dialogView.Hidden = true;
        }
        async void checkUsername()
        {
            try
            {

            
                // check username and lead to updateEmployeeController
                if (await GabanaAPI.CheckNetWork())
                {
                    var getUserfromGabana = await GabanaAPI.GetDataUserAccount(txtEmployee.Text);
                    if (getUserfromGabana == null)
                    {
                        //Seauth
                        resultAccount = await GabanaAPI.GetSeAuthDataUserAccount(txtEmployee.Text);
                        if (resultAccount == null)
                        {
                            empUpdatePage = new UpdateEmployeeController(txtEmployee.Text);
                            this.NavigationController.PushViewController(empUpdatePage, false);
                        }
                        else
                        {
                            var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("sameusername", "มีusername อยู่แล้วต้องการดึงข้อมูลไหม"), UIAlertControllerStyle.Alert);

                            //Add Actions
                            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                                alert => DeleteCus(resultAccount)));
                            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                            //Present Alert
                            PresentViewController(okCancelAlertController, true, null);

                            //dialogBackgroundView.Hidden = false;
                            //dialogView.Hidden = false;
                        
                        }
                    }
                    else
                    {
                        // pop up แสดงว่ามีข้อมูลแล้วว
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("havedata", "มีข้อมูลอยู่แล้ว"));
                    }
                }
                else
                {
                    Utils.ShowMessage("No Internet,Please Connect");
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
        }

        private void DeleteCus(UserAccount resultAccount)
        {
            empUpdatePage = new UpdateEmployeeController(resultAccount);
            this.NavigationController.PushViewController(empUpdatePage, false);
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

            lblNoteMark.TopAnchor.ConstraintEqualTo(EmployeeView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            lblNoteMark.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            lblNoteMark.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -30).Active = true;

            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddEmployee.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddEmployee.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAddEmployee.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddEmployee.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #region Dialog

            dialogBackgroundView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
            dialogBackgroundView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            dialogBackgroundView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
            dialogBackgroundView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            View.BringSubviewToFront(dialogBackgroundView);

            dialogView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, (int)View.Frame.Height / 4).Active = true;
            dialogView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 62).Active = true;
            dialogView.HeightAnchor.ConstraintEqualTo(180).Active = true;
            dialogView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -62).Active = true;
            View.BringSubviewToFront(dialogView);

            lblCheckUserDialog.CenterXAnchor.ConstraintEqualTo(dialogView.CenterXAnchor).Active = true;
            lblCheckUserDialog.CenterYAnchor.ConstraintEqualTo(dialogView.CenterYAnchor,-30).Active = true;
            lblCheckUserDialog.HeightAnchor.ConstraintEqualTo(80).Active = true;
            lblCheckUserDialog.LeftAnchor.ConstraintEqualTo(dialogView.LeftAnchor,10).Active = true;
            lblCheckUserDialog.LeftAnchor.ConstraintEqualTo(dialogView.RightAnchor, -10).Active = true;

            btnCancelUserData.LeftAnchor.ConstraintEqualTo(dialogView.LeftAnchor).Active = true;
            btnCancelUserData.RightAnchor.ConstraintEqualTo(dialogView.CenterXAnchor).Active = true;
            btnCancelUserData.BottomAnchor.ConstraintEqualTo(dialogView.BottomAnchor).Active = true;
            btnCancelUserData.HeightAnchor.ConstraintEqualTo(50).Active = true;

            btnOKUserData.LeftAnchor.ConstraintEqualTo(dialogView.CenterXAnchor).Active = true;
            btnOKUserData.RightAnchor.ConstraintEqualTo(dialogView.RightAnchor).Active = true;
            btnOKUserData.BottomAnchor.ConstraintEqualTo(dialogView.BottomAnchor).Active = true;
            btnOKUserData.HeightAnchor.ConstraintEqualTo(50).Active = true;

            #endregion

        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }
   
}