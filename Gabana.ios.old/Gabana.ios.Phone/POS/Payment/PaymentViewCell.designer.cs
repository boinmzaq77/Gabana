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
    [Register ("PaymentViewCell")]
    partial class PaymentViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgPaymentMenu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblpaymentTypeName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (imgPaymentMenu != null) {
                imgPaymentMenu.Dispose ();
                imgPaymentMenu = null;
            }

            if (lblpaymentTypeName != null) {
                lblpaymentTypeName.Dispose ();
                lblpaymentTypeName = null;
            }
        }
    }
}