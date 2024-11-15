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
    [Register ("PaymentController")]
    partial class PaymentController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnNext { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPayLater { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSaveOrder { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblsumAmount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView menuPaymentCollect { get; set; }

        [Action ("UIButton113250_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton113250_TouchUpInside (UIKit.UIButton sender);

        [Action ("UIButton113872_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton113872_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnNext != null) {
                btnNext.Dispose ();
                btnNext = null;
            }

            if (btnPayLater != null) {
                btnPayLater.Dispose ();
                btnPayLater = null;
            }

            if (btnSaveOrder != null) {
                btnSaveOrder.Dispose ();
                btnSaveOrder = null;
            }

            if (lblsumAmount != null) {
                lblsumAmount.Dispose ();
                lblsumAmount = null;
            }

            if (menuPaymentCollect != null) {
                menuPaymentCollect.Dispose ();
                menuPaymentCollect = null;
            }
        }
    }
}