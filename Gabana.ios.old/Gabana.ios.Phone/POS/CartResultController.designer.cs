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
    [Register ("CartResultController")]
    partial class CartResultController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnGoSumResult { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnOptions { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView itemPOScollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblsumPrice { get; set; }

        [Action ("BtnGoSumResult_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnGoSumResult_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnOptions_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnOptions_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnGoSumResult != null) {
                btnGoSumResult.Dispose ();
                btnGoSumResult = null;
            }

            if (btnOptions != null) {
                btnOptions.Dispose ();
                btnOptions = null;
            }

            if (itemPOScollection != null) {
                itemPOScollection.Dispose ();
                itemPOScollection = null;
            }

            if (lblsumPrice != null) {
                lblsumPrice.Dispose ();
                lblsumPrice = null;
            }
        }
    }
}