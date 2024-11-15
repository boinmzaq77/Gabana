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
    [Register ("DummyItemController")]
    partial class DummyItemController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAddDrescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtAddDummy { get; set; }

        [Action ("BtnAddDrescription_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnAddDrescription_TouchUpInside (UIKit.UIButton sender);

        [Action ("UIButton107770_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton107770_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnAddDrescription != null) {
                btnAddDrescription.Dispose ();
                btnAddDrescription = null;
            }

            if (txtAddDummy != null) {
                txtAddDummy.Dispose ();
                txtAddDummy = null;
            }
        }
    }
}