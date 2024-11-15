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
    [Register ("ScanItemsQrController")]
    partial class ScanItemsQrController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSumCart { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView itemPOScollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewScanQr { get; set; }

        [Action ("BtnSumCart_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSumCart_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnSumCart != null) {
                btnSumCart.Dispose ();
                btnSumCart = null;
            }

            if (itemPOScollection != null) {
                itemPOScollection.Dispose ();
                itemPOScollection = null;
            }

            if (viewScanQr != null) {
                viewScanQr.Dispose ();
                viewScanQr = null;
            }
        }
    }
}