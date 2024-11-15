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
    [Register ("POSheaderViewCell")]
    partial class POSheaderViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel menuPOSName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (menuPOSName != null) {
                menuPOSName.Dispose ();
                menuPOSName = null;
            }
        }
    }
}