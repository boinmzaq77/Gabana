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
    [Register ("AddNewItemPOSController")]
    partial class AddNewItemPOSController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgItemPOS { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblItemName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblprice { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl segSelectNewCreate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewPhoto { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewProduct { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewStock { get; set; }

        [Action ("SegmenteControl_ValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SegmenteControl_ValueChanged (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
            if (imgItemPOS != null) {
                imgItemPOS.Dispose ();
                imgItemPOS = null;
            }

            if (lblItemName != null) {
                lblItemName.Dispose ();
                lblItemName = null;
            }

            if (lblprice != null) {
                lblprice.Dispose ();
                lblprice = null;
            }

            if (segSelectNewCreate != null) {
                segSelectNewCreate.Dispose ();
                segSelectNewCreate = null;
            }

            if (viewName != null) {
                viewName.Dispose ();
                viewName = null;
            }

            if (viewPhoto != null) {
                viewPhoto.Dispose ();
                viewPhoto = null;
            }

            if (viewProduct != null) {
                viewProduct.Dispose ();
                viewProduct = null;
            }

            if (viewStock != null) {
                viewStock.Dispose ();
                viewStock = null;
            }
        }
    }
}