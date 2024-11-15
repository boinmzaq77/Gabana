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
    [Register ("MainPageController")]
    partial class MainPageController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageProfile { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblBranch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCompName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblMemname { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView logo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView menuCollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewMemberCard { get; set; }

        [Action ("UIButton371357_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton371357_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (imageProfile != null) {
                imageProfile.Dispose ();
                imageProfile = null;
            }

            if (lblBranch != null) {
                lblBranch.Dispose ();
                lblBranch = null;
            }

            if (lblCompName != null) {
                lblCompName.Dispose ();
                lblCompName = null;
            }

            if (lblMemname != null) {
                lblMemname.Dispose ();
                lblMemname = null;
            }

            if (logo != null) {
                logo.Dispose ();
                logo = null;
            }

            if (menuCollection != null) {
                menuCollection.Dispose ();
                menuCollection = null;
            }

            if (viewMemberCard != null) {
                viewMemberCard.Dispose ();
                viewMemberCard = null;
            }
        }
    }
}