// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Gabana.ios.Phone
{
    [Register ("LoginController")]
    partial class LoginController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnEmpLogIn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnGoOwnerLogIn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BtnGoRegister { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnOwnerLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnRegis { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnRegisBack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView Logo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl segLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmpMer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmpPass { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmpUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtOwnerTel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtRegisTel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewEmployee { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewMain { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewMainLogin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewOwner { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewRegister { get; set; }

        [Action ("BtnEmpLogIn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnEmpLogIn_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnGoOwnerLogIn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnGoOwnerLogIn_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnGoRegister_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnGoRegister_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnOwnerLogin_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnOwnerLogin_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnRegis_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnRegis_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnRegisBack_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnRegisBack_TouchUpInside (UIKit.UIButton sender);

        [Action ("SegmenteControl_ValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SegmenteControl_ValueChanged (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnEmpLogIn != null) {
                btnEmpLogIn.Dispose ();
                btnEmpLogIn = null;
            }

            if (BtnGoOwnerLogIn != null) {
                BtnGoOwnerLogIn.Dispose ();
                BtnGoOwnerLogIn = null;
            }

            if (BtnGoRegister != null) {
                BtnGoRegister.Dispose ();
                BtnGoRegister = null;
            }

            if (btnOwnerLogin != null) {
                btnOwnerLogin.Dispose ();
                btnOwnerLogin = null;
            }

            if (btnRegis != null) {
                btnRegis.Dispose ();
                btnRegis = null;
            }

            if (btnRegisBack != null) {
                btnRegisBack.Dispose ();
                btnRegisBack = null;
            }

            if (Logo != null) {
                Logo.Dispose ();
                Logo = null;
            }

            if (segLogin != null) {
                segLogin.Dispose ();
                segLogin = null;
            }

            if (txtEmpMer != null) {
                txtEmpMer.Dispose ();
                txtEmpMer = null;
            }

            if (txtEmpPass != null) {
                txtEmpPass.Dispose ();
                txtEmpPass = null;
            }

            if (txtEmpUser != null) {
                txtEmpUser.Dispose ();
                txtEmpUser = null;
            }

            if (txtOwnerTel != null) {
                txtOwnerTel.Dispose ();
                txtOwnerTel = null;
            }

            if (txtRegisTel != null) {
                txtRegisTel.Dispose ();
                txtRegisTel = null;
            }

            if (viewEmployee != null) {
                viewEmployee.Dispose ();
                viewEmployee = null;
            }

            if (viewMain != null) {
                viewMain.Dispose ();
                viewMain = null;
            }

            if (viewMainLogin != null) {
                viewMainLogin.Dispose ();
                viewMainLogin = null;
            }

            if (viewOwner != null) {
                viewOwner.Dispose ();
                viewOwner = null;
            }

            if (viewRegister != null) {
                viewRegister.Dispose ();
                viewRegister = null;
            }
        }
    }
}