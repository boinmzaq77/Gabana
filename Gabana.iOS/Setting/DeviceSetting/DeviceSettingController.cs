using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class DeviceSettingController : UIViewController
    {
        UIView DeviceNoView, UDIDViewView, CommentView, BottomView;
        UILabel lblDeviceNo, lblUDID, lblComment;
        UITextField txtDeviceNo, txtUDID, txtComment;
        UIButton btnSave;
        public DeviceSettingController() {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            try
            {
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();

                txtUDID.Text = (DataCashingAll.DeviceUDID).ToUpper();
                txtDeviceNo.Text = DataCashingAll.Merchant .Device.DeviceNo.ToString();

                if (DataCashingAll.Device != null)
                {
                    txtComment.Text = DataCashingAll.Device.Comments;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        void initAttribute()
        {
            #region DeviceNoView
            DeviceNoView = new UIView();
            DeviceNoView.BackgroundColor = UIColor.White;
            DeviceNoView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblDeviceNo = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDeviceNo.Font = lblDeviceNo.Font.WithSize(15);
            lblDeviceNo.Text = "Device No";
            DeviceNoView.AddSubview(lblDeviceNo);

            txtDeviceNo = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Enabled = false
            };
            txtDeviceNo.ReturnKeyType = UIReturnKeyType.Next;
            txtDeviceNo.ShouldReturn = (tf) =>
            {
                txtUDID.BecomeFirstResponder();
                return true;
            };
            txtDeviceNo.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtDeviceNo.AttributedPlaceholder = new NSAttributedString("001", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtDeviceNo.Font = txtDeviceNo.Font.WithSize(15);
            DeviceNoView.AddSubview(txtDeviceNo);
            #endregion

            #region UDIDViewView
            UDIDViewView = new UIView();
            UDIDViewView.BackgroundColor = UIColor.White;
            UDIDViewView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblUDID = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblUDID.Font = lblUDID.Font.WithSize(15);
            lblUDID.Text = "UDID";
            UDIDViewView.AddSubview(lblUDID);

            txtUDID = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Enabled = false
            };
            txtUDID.ReturnKeyType = UIReturnKeyType.Next;
            txtUDID.ShouldReturn = (tf) =>
            {
                txtComment.BecomeFirstResponder();
                return true;
            };
            txtUDID.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtUDID.AttributedPlaceholder = new NSAttributedString("XXXXXXX", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtUDID.Font = txtUDID.Font.WithSize(15);
            UDIDViewView.AddSubview(txtUDID);
            #endregion

            #region CommentView
            CommentView = new UIView();
            CommentView.BackgroundColor = UIColor.White;
            CommentView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblComment = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblComment.Font = lblComment.Font.WithSize(15);
            lblComment.Text = "Comment";
            CommentView.AddSubview(lblComment);

            txtComment = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtComment.ReturnKeyType = UIReturnKeyType.Next;
            txtComment.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtComment.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtComment.AttributedPlaceholder = new NSAttributedString("Comment", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtComment.Font = txtComment.Font.WithSize(15);
            CommentView.AddSubview(txtComment);
            #endregion

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;


            btnSave = new UIButton();
            btnSave.SetTitle("Save", UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += async (sender, e) => {
                try
                {
                    if (DataCashingAll.Device != null)
                    {
                        ORM.Master.Device device = new ORM.Master.Device();
                        device.MerchantID = DataCashingAll.Device.MerchantID;
                        device.Comments = txtComment.Text;
                        device.DateCreated = DataCashingAll.Device.DateCreated;
                        device.DateLastActive = DataCashingAll.Device.DateLastActive;
                        device.DeviceInfo = DataCashingAll.Device.DeviceInfo;
                        device.DeviceNo = DataCashingAll.Device.DeviceNo;
                        device.Platform = DataCashingAll.Device.Platform;
                        device.UDID = DataCashingAll.Device.UDID;

                        var updateDevice = await GabanaAPI.PutDataDevice(device);
                        if (updateDevice.Status)
                        {
                            DataCashingAll.Device.Comments = device.Comments;
                            DataCashingAll.Device.DateLastActive = device.DateLastActive;
                            Utils.ShowMessage("บันทึกสำเร็จ");
                            this.NavigationController.PopViewController(false);
                            //this.Finish();
                        }
                    }
                }
                catch (Exception ex)
                {
                    await TinyInsights.TrackErrorAsync(ex);
                    Utils.ShowMessage(ex.Message);
                    //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
            };
            BottomView.AddSubview(btnSave);
            #endregion

            // UIView DeviceNoView, UDIDViewView, CommentView, BottomView;
            View.AddSubview(DeviceNoView);
            View.AddSubview(UDIDViewView);
            View.AddSubview(CommentView);
            View.AddSubview(BottomView);
            BottomView.BringSubviewToFront(btnSave);
        }
        
        void SetupAutoLayout()
        {
            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSave.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSave.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #region DeviceNoView
            DeviceNoView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            DeviceNoView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            DeviceNoView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DeviceNoView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblDeviceNo.CenterYAnchor.ConstraintEqualTo(DeviceNoView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblDeviceNo.WidthAnchor.ConstraintEqualTo(View.Frame.Height-50).Active = true;
            lblDeviceNo.LeftAnchor.ConstraintEqualTo(DeviceNoView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblDeviceNo.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtDeviceNo.TopAnchor.ConstraintEqualTo(lblDeviceNo.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            txtDeviceNo.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtDeviceNo.LeftAnchor.ConstraintEqualTo(DeviceNoView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtDeviceNo.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region UDIDViewView
            UDIDViewView.TopAnchor.ConstraintEqualTo(DeviceNoView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            UDIDViewView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            UDIDViewView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            UDIDViewView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblUDID.CenterYAnchor.ConstraintEqualTo(UDIDViewView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblUDID.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblUDID.LeftAnchor.ConstraintEqualTo(UDIDViewView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblUDID.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtUDID.TopAnchor.ConstraintEqualTo(lblUDID.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtUDID.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtUDID.LeftAnchor.ConstraintEqualTo(UDIDViewView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtUDID.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region CommentView
            CommentView.TopAnchor.ConstraintEqualTo(UDIDViewView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CommentView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            CommentView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CommentView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblComment.CenterYAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblComment.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblComment.LeftAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblComment.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtComment.TopAnchor.ConstraintEqualTo(lblComment.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtComment.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtComment.LeftAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtComment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }
   
}