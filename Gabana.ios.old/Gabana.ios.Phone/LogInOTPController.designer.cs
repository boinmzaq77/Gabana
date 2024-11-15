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
    [Register ("LogInOTPController")]
    partial class LogInOTPController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblOTPref { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblOTPtel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField otp1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField otp2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField otp3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField otp4 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField otp5 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField otp6 { get; set; }

        [Action ("otpchange1:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void otpchange1 (UIKit.UITextField sender);

        [Action ("otpchange2:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void otpchange2 (UIKit.UITextField sender);

        [Action ("otpchange3:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void otpchange3 (UIKit.UITextField sender);

        [Action ("otpchange4:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void otpchange4 (UIKit.UITextField sender);

        [Action ("otpchange5:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void otpchange5 (UIKit.UITextField sender);

        [Action ("otpchange6:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void otpchange6 (UIKit.UITextField sender);

        void ReleaseDesignerOutlets ()
        {
            if (lblOTPref != null) {
                lblOTPref.Dispose ();
                lblOTPref = null;
            }

            if (lblOTPtel != null) {
                lblOTPtel.Dispose ();
                lblOTPtel = null;
            }

            if (otp1 != null) {
                otp1.Dispose ();
                otp1 = null;
            }

            if (otp2 != null) {
                otp2.Dispose ();
                otp2 = null;
            }

            if (otp3 != null) {
                otp3.Dispose ();
                otp3 = null;
            }

            if (otp4 != null) {
                otp4.Dispose ();
                otp4 = null;
            }

            if (otp5 != null) {
                otp5.Dispose ();
                otp5 = null;
            }

            if (otp6 != null) {
                otp6.Dispose ();
                otp6 = null;
            }
        }
    }
}