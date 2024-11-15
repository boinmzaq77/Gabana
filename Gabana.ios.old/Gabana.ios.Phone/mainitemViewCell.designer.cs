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
    [Register ("mainitemViewCell")]
    partial class mainitemViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView iconImg { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel IconName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (iconImg != null) {
                iconImg.Dispose ();
                iconImg = null;
            }

            if (IconName != null) {
                IconName.Dispose ();
                IconName = null;
            }
        }
    }
}