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
    [Register ("POSController")]
    partial class POSController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBasket { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnItemSum { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnQr { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSearch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnUser { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView itemPOSCollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView itemPOSVertical { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView POSMenuCollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtSearch { get; set; }

        [Action ("BtnBasket_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnBasket_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnItemSum_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnItemSum_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnList_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnList_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnQr_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnQr_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnSearch_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnSearch_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnUser_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnUser_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnBasket != null) {
                btnBasket.Dispose ();
                btnBasket = null;
            }

            if (btnItemSum != null) {
                btnItemSum.Dispose ();
                btnItemSum = null;
            }

            if (btnList != null) {
                btnList.Dispose ();
                btnList = null;
            }

            if (btnQr != null) {
                btnQr.Dispose ();
                btnQr = null;
            }

            if (btnSearch != null) {
                btnSearch.Dispose ();
                btnSearch = null;
            }

            if (btnUser != null) {
                btnUser.Dispose ();
                btnUser = null;
            }

            if (itemPOSCollection != null) {
                itemPOSCollection.Dispose ();
                itemPOSCollection = null;
            }

            if (itemPOSVertical != null) {
                itemPOSVertical.Dispose ();
                itemPOSVertical = null;
            }

            if (POSMenuCollection != null) {
                POSMenuCollection.Dispose ();
                POSMenuCollection = null;
            }

            if (txtSearch != null) {
                txtSearch.Dispose ();
                txtSearch = null;
            }
        }
    }
}