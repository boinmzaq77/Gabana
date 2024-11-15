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
    [Register ("itemPOSViewCell")]
    partial class itemPOSViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView itemPOSIMG { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblitemName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblItemprice { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (itemPOSIMG != null) {
                itemPOSIMG.Dispose ();
                itemPOSIMG = null;
            }

            if (lblitemName != null) {
                lblitemName.Dispose ();
                lblitemName = null;
            }

            if (lblItemprice != null) {
                lblItemprice.Dispose ();
                lblItemprice = null;
            }
        }
    }
}