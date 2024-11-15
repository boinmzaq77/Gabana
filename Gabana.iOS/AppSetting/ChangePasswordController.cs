using Foundation;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class ChangePasswordController : UIViewController
    {

        UIView oldPassView, newPassView, confirmNewView,bottomView, PinCode;
        UIButton btnChangePass;
        UILabel lblOldPass, lblNewPass, lblConfirm, lblPin;
        UserAccountInfoManage UserAccountInfoManager = new UserAccountInfoManage();
        BranchPolicyManage policyManage = new BranchPolicyManage();
        UISwitch statusPINCODE;
        List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();

        public static ORM.MerchantDB.UserAccountInfo LocalUseraccount = new ORM.MerchantDB.UserAccountInfo();

        UITextField txtOld, txtNew, txtContirm;
        PinCodeController pinCodePage = null;
        Model.UserAccountInfo useraccountData;
        public static bool back;
        string LoginType = string.Empty;
        string emplogin = string.Empty;
        public static string NewPincode ;
        public static int UsePincode = 0;
        private UIView changepinView;
        private UILabel lblchangepin;
        private UITextField txtchangepin;
        private UIButton btnchangepin;
        public static bool ChangeSuccess;

        public ChangePasswordController() { }
       
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
            if (back)
            {
                if (UsePincode == 1 )
                {
                    statusPINCODE.On = true;
                }
                else
                {
                    UsePincode = 0;
                    statusPINCODE.On = false;
                    changepinView.Hidden = true;

                }
                Checkbtn();
            }
            if (!ChangeSuccess)
            {
                if (statusPINCODE.On)
                {
                    statusPINCODE.On = false;
                }
                else
                {
                    statusPINCODE.On = true;
                }
            }

        }

        private void Checkbtn()
        {
            
        }

        public async override void ViewDidLoad()
        {
            try
            {
                ChangeSuccess = true; 
                back = false;
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                this.NavigationController.SetNavigationBarHidden(false, false);
                View.BackgroundColor = UIColor.White;
                if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
                {

                    var navBarAppearance = new UINavigationBarAppearance();
                    navBarAppearance.ConfigureWithOpaqueBackground();
                    navBarAppearance.TitleTextAttributes = new UIStringAttributes()
                    {
                        ForegroundColor = UIColor.White
                    };
                    navBarAppearance.LargeTitleTextAttributes = new UIStringAttributes()
                    {
                        ForegroundColor = UIColor.White
                    };
                    navBarAppearance.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                    this.NavigationController.NavigationBar.StandardAppearance = navBarAppearance;
                    this.NavigationController.NavigationBar.ScrollEdgeAppearance = navBarAppearance;
                    this.NavigationController.NavigationBar.TintColor = UIColor.White;
                }
                else
                {
                    this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
                    this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
                    {
                        ForegroundColor = UIColor.White
                    };
                    this.NavigationController.NavigationBar.TintColor = UIColor.White;
                }


                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                

                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();
                GetDetail();
                if (LoginType.ToLower() == "owner")
                {
                    oldPassView.Hidden = true;
                    newPassView.Hidden = true;
                    confirmNewView.Hidden = true;
                    PinCode.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
                }

                
            }
            catch (Exception ex)
            {
                await TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);
            }
           
        }
        async void GetDetail()
        {
            try
            {
                if (DataCashingAll.UserAccountInfo == null)
                {
                    DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                }
                useraccountData = DataCashingAll.UserAccountInfo.Where(x => x.UserName == emplogin).FirstOrDefault();

                LocalUseraccount = await UserAccountInfoManager.GetUserAccount((int)MainController.merchantlocal.MerchantID, useraccountData?.UserName.ToLower());

                UsePincode = (int)LocalUseraccount.FUsePincode;

                if (UsePincode == 1 && LocalUseraccount.PinCode != null)
                {
                    statusPINCODE.On = true;
                    changepinView.Hidden= false;
                }
                else
                {
                    UsePincode = 0;
                    statusPINCODE.On = false;
                    changepinView.Hidden = true;
                }
            }
            catch (Exception ex)
            {
                await TinyInsightsLib.TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        void initAttribute()
        {
            #region bottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(bottomView);

            btnChangePass = new UIButton();
            btnChangePass.SetTitle("Change Password",UIControlState.Normal);
            btnChangePass.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
            btnChangePass.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnChangePass.BackgroundColor = UIColor.White;
            btnChangePass.Layer.CornerRadius = 5f;
            btnChangePass.Layer.BorderWidth = 0.5f;
            btnChangePass.Enabled = true;
            btnChangePass.TranslatesAutoresizingMaskIntoConstraints = false;
            btnChangePass.TouchUpInside += async (sender, e) => {
                btnChangePass.Enabled = false;
                ChangePass();
            };
            bottomView.AddSubview(btnChangePass);
            #endregion

            #region oldPassView
            oldPassView = new UIView();
            oldPassView.TranslatesAutoresizingMaskIntoConstraints = false;
            oldPassView.BackgroundColor = UIColor.White;
            View.AddSubview(oldPassView);

            lblOldPass= new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblOldPass.Font = lblOldPass.Font.WithSize(15);
            lblOldPass.TranslatesAutoresizingMaskIntoConstraints = false;
            lblOldPass.Text = "Old Password";
            oldPassView.AddSubview(lblOldPass);

            txtOld = new UITextField
            {
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtOld.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtOld.KeyboardType = UIKeyboardType.Default;
            txtOld.SecureTextEntry = true;
            txtOld.ReturnKeyType = UIReturnKeyType.Next;
            txtOld.ShouldReturn = (tf) =>
            {
                txtNew.BecomeFirstResponder();
                return true;
            };
            txtOld.Font = txtOld.Font.WithSize(15);
            oldPassView.AddSubview(txtOld);
            #endregion

            #region newPassView
            newPassView = new UIView();
            newPassView.TranslatesAutoresizingMaskIntoConstraints = false;
            newPassView.BackgroundColor = UIColor.White;
            View.AddSubview(newPassView);

            lblNewPass = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblNewPass.Font = lblNewPass.Font.WithSize(15);
            lblNewPass.TranslatesAutoresizingMaskIntoConstraints = false;
            lblNewPass.Text = "New Password";
            newPassView.AddSubview(lblNewPass);

            txtNew = new UITextField
            {
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtNew.ReturnKeyType = UIReturnKeyType.Next;
            txtNew.ShouldReturn = (tf) =>
            {
                txtContirm.BecomeFirstResponder();
                return true;
            };
            txtNew.SecureTextEntry = true;
            txtNew.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtNew.KeyboardType = UIKeyboardType.Default;
            txtNew.Font = txtNew.Font.WithSize(15);
            newPassView.AddSubview(txtNew);
            #endregion

            #region confirmNewView
            confirmNewView = new UIView();
            confirmNewView.TranslatesAutoresizingMaskIntoConstraints = false;
            confirmNewView.BackgroundColor = UIColor.White;
            View.AddSubview(confirmNewView);

            lblConfirm = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblConfirm.Font = lblConfirm.Font.WithSize(15);
            lblConfirm.TranslatesAutoresizingMaskIntoConstraints = false;
            lblConfirm.Text = "Confirm New Password";
            confirmNewView.AddSubview(lblConfirm);

            txtContirm = new UITextField
            {
                TextColor =  UIColor.FromRGB(51,170,225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtContirm.SecureTextEntry = true;
            txtContirm.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtContirm.ReturnKeyType = UIReturnKeyType.Done;
            txtContirm.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtContirm.KeyboardType = UIKeyboardType.Default;
            txtContirm.Font = txtContirm.Font.WithSize(15);
            confirmNewView.AddSubview(txtContirm);
            #endregion

            #region PinCode
            PinCode = new UIView();
            PinCode.TranslatesAutoresizingMaskIntoConstraints = false;
            PinCode.BackgroundColor = UIColor.White;
            View.AddSubview(PinCode);

            lblPin = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblPin.Font = lblPin.Font.WithSize(15);
            lblPin.TranslatesAutoresizingMaskIntoConstraints = false;
            lblPin.Text = "PIN Code";
            PinCode.AddSubview(lblPin);


            statusPINCODE = new UISwitch();
            statusPINCODE.TranslatesAutoresizingMaskIntoConstraints = false;
            statusPINCODE.OnTintColor = UIColor.FromRGB(0, 149, 218);
            statusPINCODE.ValueChanged += async (sender, e) =>
            {
                SwitchShow_CheckedChange();
            };
            PinCode.AddSubview(statusPINCODE);
            #endregion

            #region oldPassView
            changepinView = new UIView();
            changepinView.TranslatesAutoresizingMaskIntoConstraints = false;
            changepinView.BackgroundColor = UIColor.White;
            View.AddSubview(changepinView);

            lblchangepin = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblchangepin.Font = lblOldPass.Font.WithSize(15);
            lblchangepin.TranslatesAutoresizingMaskIntoConstraints = false;
            lblchangepin.Text = "Old Password";
            changepinView.AddSubview(lblchangepin);

            txtchangepin = new UITextField
            {
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtchangepin.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtchangepin.KeyboardType = UIKeyboardType.Default;
            txtchangepin.SecureTextEntry = true;
            txtchangepin.ReturnKeyType = UIReturnKeyType.Next;
            txtchangepin.ShouldReturn = (tf) =>
            {
                txtNew.BecomeFirstResponder();
                return true;
            };
            txtchangepin.Font = txtchangepin.Font.WithSize(15);
            changepinView.AddSubview(txtchangepin);

            btnchangepin = new UIButton();
            btnchangepin.SetTitle("Change", UIControlState.Normal);
            btnchangepin.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            
            btnchangepin.BackgroundColor = UIColor.FromRGB(225, 225, 225);
            btnchangepin.Layer.CornerRadius = 15f;
            
            btnchangepin.Enabled = true;
            btnchangepin.TranslatesAutoresizingMaskIntoConstraints = false;
            btnchangepin.TouchUpInside += async (sender, e) => {
                //
                Utils.SetTitle(this.NavigationController, "Pin Code");
                pinCodePage = new PinCodeController("ChangePincode");
                this.NavigationController.PushViewController(pinCodePage, false);
            };
            changepinView.AddSubview(btnchangepin);

            #endregion
        }
        private async void ChangePass()
        {
            try
            {
                string NewPass = txtNew.Text;
                string ConfirmPass = txtContirm.Text;
                string OldPass = txtOld.Text;

                if (!string.IsNullOrEmpty(NewPass) | !string.IsNullOrEmpty(ConfirmPass) | !string.IsNullOrEmpty(OldPass))
                {
                    // User ที่เข้าใช้งานเป็น Admin, Manager, Invoice Officer, Cashier, Editor และ Officer จะสามารถเข้ามาแก้ไขรหัสผ่าน และตั้งค่า Pin code ได้

                    //รหัสผ่าน

                    if (string.IsNullOrEmpty(NewPass) || string.IsNullOrEmpty(ConfirmPass) || string.IsNullOrEmpty(OldPass))
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกรหัสผ่าน");
                        btnChangePass.Enabled = true;
                        return;
                    }

                    if (NewPass != ConfirmPass)
                    {

                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกรหัสผ่านให้ตรงกัน");
                        btnChangePass.Enabled = true;
                        return;
                    }

                    Model.ChangePassword changePassword = new ChangePassword();
                    changePassword.Username = emplogin;
                    changePassword.OldPassword = OldPass;
                    changePassword.Password = NewPass;
                    changePassword.ConfirmPassword = ConfirmPass;
                    //Employee all 
                    var checkresult = await GabanaAPI.PutSeAuthDataChangePassword(changePassword);
                    if (checkresult.Status)
                    {
                        Utils.ShowAlert(this, "สำเร็จ !", "แก้ไขรหัสสำเร็จ");
                    }

                    if (!checkresult.Status)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "แก้ไขรหัสผ่านไม่สำเร็จ");
                    }
                    //tils.ShowMessage("แก้ไขรหัสผ่านสำเร็จ");
                }

                if (LocalUseraccount.FUsePincode != UsePincode || LocalUseraccount.PinCode != NewPincode)
                {

                    if (UsePincode == 1 && NewPincode == null)
                    {
                        return;
                    }
                    if (UsePincode == 0)
                    {
                        NewPincode = null;
                    }
                    //useraccount
                    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                    {
                        MerchantID = (int)LocalUseraccount?.MerchantID,
                        UserName = LocalUseraccount.UserName,
                        FUsePincode = UsePincode,
                        PinCode = NewPincode,
                        Comments = LocalUseraccount?.Comments
                    };
                    //Branch policy
                    var BranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, useraccountData?.UserName);
                    if (BranchPolicy != null)
                    {
                        foreach (var item in BranchPolicy)
                        {
                            ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                            {
                                MerchantID = (int)item.MerchantID,
                                UserName = item.UserName,
                                SysBranchID = (int)item.SysBranchID,
                            };
                            lstbranchPolicies.Add(branchPolicy);
                        }
                    }

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
                            MerchantID = (int)LocalUseraccount?.MerchantID,
                            UserName = useraccountData?.UserName,
                            FUsePincode = UsePincode,
                            PinCode = NewPincode,
                            Comments = LocalUseraccount?.Comments,
                        };

                        var Updatelocal = await UserAccountInfoManager.UpdateUserAccount(localUser);
                        if (Updatelocal)
                        {
                            foreach (var itembranch in lstbranchPolicies)
                            {
                                ORM.MerchantDB.BranchPolicy branchPolicy = new ORM.MerchantDB.BranchPolicy()
                                {
                                    MerchantID = itembranch.MerchantID,
                                    SysBranchID = (int)itembranch.SysBranchID,
                                    UserName = itembranch.UserName,
                                };
                                var updatelocalbranchPolicy = await policyManage.UpdateBranchPolicy(branchPolicy);
                            }

                            Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                            {
                                UserName = useraccountData?.UserName,
                                MainRoles = useraccountData?.MainRoles,
                                UserAccessProduct = useraccountData.UserAccessProduct,
                                FullName = useraccountData?.FullName,
                                MerchantID = useraccountData.MerchantID,
                                CreatedTime = useraccountData.CreatedTime,
                                FStatus = useraccountData.FStatus,
                                LastAccessTime = useraccountData.LastAccessTime,
                                LastModifyTime = useraccountData.LastModifyTime,
                                ListSeniorStaff = useraccountData.ListSeniorStaff,
                                ListUserAccessProduct = useraccountData.ListUserAccessProduct,
                                Mobile = useraccountData.Mobile,
                                PasswordHash = useraccountData.PasswordHash,
                                PasswordSalt = useraccountData.PasswordSalt,
                                ShopID = useraccountData.ShopID,
                                TryNextTime = useraccountData.TryNextTime,
                                TryPassCount = useraccountData.TryPassCount,
                            };

                            var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName == useraccountData?.UserName);
                            DataCashingAll.UserAccountInfo.Remove(data);
                            DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                            if (localUser.FUsePincode == 1)
                            {
                                DataCashingAll.UsePinCode = true;
                            }
                            else
                            {
                                DataCashingAll.UsePinCode = false;
                            }
                            
                            Utils.ShowMessage("แก้ไขรหัสผ่านสำเร็จ");
                            DataCashingAll.UserActive = DateTime.Now;
                            this.NavigationController.PopViewController(false);
                        }

                        if (!Updatelocal)
                        {
                            Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถแก้ไข Pin Code ได้");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at changePass");
            }
        }
        private void SwitchShow_CheckedChange()
        {
            try
            {
                ChangeSuccess = false;
                if (LocalUseraccount.FUsePincode == 0)
                {
                    if (statusPINCODE.On)
                    {
                        Utils.SetTitle(this.NavigationController, "Pin Code");
                        pinCodePage = new PinCodeController("NewPincode");
                       
                        
                        this.NavigationController.PushViewController(pinCodePage, false);
                    }
                }
                else
                {
                    if (!statusPINCODE.On)
                    {
                        Utils.SetTitle(this.NavigationController, "Pin Code");
                        pinCodePage = new PinCodeController("OffPincode");
                        
                        this.NavigationController.PushViewController(pinCodePage, false);
                    }
                }
                
                
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SwitchShow_CheckedChange at changePass");
            }
        }
        void SetupAutoLayout()
        {
            #region bottomView
            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnChangePass.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnChangePass.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnChangePass.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnChangePass.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            #endregion

            #region oldPassView
            oldPassView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            oldPassView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            oldPassView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            oldPassView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblOldPass.RightAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblOldPass.LeftAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width*4)/100).Active = true;
            lblOldPass.TopAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height * 16) / 1000).Active = true;

            txtOld.RightAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            txtOld.LeftAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            txtOld.TopAnchor.ConstraintEqualTo(lblOldPass.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            #endregion

            #region newPassView
            newPassView.TopAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            newPassView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            newPassView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            newPassView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblNewPass.RightAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblNewPass.LeftAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblNewPass.TopAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height * 16) / 1000).Active = true;

            txtNew.RightAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            txtNew.LeftAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            txtNew.TopAnchor.ConstraintEqualTo(lblNewPass.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            #endregion

            #region confirmNewView
            confirmNewView.TopAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            confirmNewView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            confirmNewView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            confirmNewView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblConfirm.RightAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblConfirm.LeftAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblConfirm.TopAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height * 16) / 1000).Active = true;

            txtContirm.RightAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            txtContirm.LeftAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            txtContirm.TopAnchor.ConstraintEqualTo(lblConfirm.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            #endregion

            #region PINCODE
            PinCode.TopAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            PinCode.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            PinCode.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            PinCode.HeightAnchor.ConstraintEqualTo(60).Active = true;


            lblPin.CenterYAnchor.ConstraintEqualTo(PinCode.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblPin.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblPin.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;

            statusPINCODE.CenterYAnchor.ConstraintEqualTo(PinCode.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            statusPINCODE.RightAnchor.ConstraintEqualTo(PinCode.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            statusPINCODE.WidthAnchor.ConstraintEqualTo(51).Active = true;
            statusPINCODE.HeightAnchor.ConstraintEqualTo(31).Active = true;
            #endregion

            #region newPassView
            changepinView.TopAnchor.ConstraintEqualTo(PinCode.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            changepinView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            changepinView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            changepinView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblchangepin.RightAnchor.ConstraintEqualTo(changepinView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblchangepin.LeftAnchor.ConstraintEqualTo(changepinView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblchangepin.TopAnchor.ConstraintEqualTo(changepinView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblchangepin.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtchangepin.RightAnchor.ConstraintEqualTo(changepinView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            txtchangepin.LeftAnchor.ConstraintEqualTo(changepinView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            txtchangepin.TopAnchor.ConstraintEqualTo(lblchangepin.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtchangepin.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnchangepin.RightAnchor.ConstraintEqualTo(changepinView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnchangepin.WidthAnchor.ConstraintEqualTo(85).Active = true;
            btnchangepin.HeightAnchor.ConstraintEqualTo(35).Active = true;
            btnchangepin.CenterYAnchor.ConstraintEqualTo(changepinView.CenterYAnchor).Active = true;


            
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