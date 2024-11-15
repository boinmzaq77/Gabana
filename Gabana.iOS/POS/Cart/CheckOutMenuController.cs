using Foundation;
using Gabana.iOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.POS.Cart
{
    public partial class CheckOutMenuController : UIViewController
    {
        UIView OptionView, ClearCartView, AddDiscountView, AddRemarkView, CheckOutView;
        UILabel lblOption;
        UIButton btnCancel,  btnMore, btnCheckOut,btnDeleteRemark;
        UILabel lblAddRemark, lblAddDiscount, lblClearCart, lblRemark;
        UIImageView RemarkImg, DiscountImg, ClearImage,moreImg;
        public CheckOutMenuController()
        {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.Clear;
            View.Layer.Opacity = 0.6f;

            #region OptionView
            OptionView = new UIView();
            OptionView.TranslatesAutoresizingMaskIntoConstraints = false;
            OptionView.BackgroundColor = UIColor.White;
            View.AddSubview(OptionView);

            lblOption = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.Black,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblOption.Font = lblOption.Font.WithSize(15);
            lblOption.TranslatesAutoresizingMaskIntoConstraints = false;
            lblOption.Text = "Option";
            OptionView.AddSubview(lblOption);

            btnCancel = new UIButton();
            btnCancel.VerticalAlignment  = UIControlContentVerticalAlignment.Center;
            btnCancel.SetTitleColor(UIColor.FromRGB(226, 226, 226),UIControlState.Normal);
            btnCancel.SetTitle(Utils.TextBundle("textcancel", "Cancel"),UIControlState.Normal);
            btnCancel.TranslatesAutoresizingMaskIntoConstraints = false;
            btnCancel.TouchUpInside += (sender, e) => {
                // select type
                DataCaching.CheckOutNavigation.DismissViewController(true, null);

            };
            OptionView.AddSubview(btnCancel);
            #endregion

            #region AddRemark
            AddRemarkView = new UIView();
            AddRemarkView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddRemarkView.BackgroundColor = UIColor.White;
            View.AddSubview(AddRemarkView);

            lblAddRemark = new UILabel();
            lblAddRemark.TranslatesAutoresizingMaskIntoConstraints = false;
            lblAddRemark.TextAlignment = UITextAlignment.Left;
            lblAddRemark.Font = lblAddRemark.Font.WithSize(15);
            lblAddRemark.TextColor = UIColor.FromRGB(64, 64, 64);
            AddRemarkView.AddSubview(lblAddRemark);

            lblRemark = new UILabel();
            lblRemark.Text = POSController.remark;
            lblRemark.TranslatesAutoresizingMaskIntoConstraints = false;
            lblRemark.TextAlignment = UITextAlignment.Left;
            lblRemark.Font = lblAddRemark.Font.WithSize(15);
            lblRemark.TextColor = UIColor.FromRGB(0, 149, 218);
            AddRemarkView.AddSubview(lblRemark);

            btnDeleteRemark = new UIButton();
            btnDeleteRemark.SetBackgroundImage(UIImage.FromBundle("Trash"),UIControlState.Normal);
            btnDeleteRemark.BackgroundColor = UIColor.White;
            btnDeleteRemark.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDeleteRemark.TouchUpInside += (sender, e) => {
                POSController.remark = null;
                lblRemark.Text = "";
                lblAddRemark.Text = "Add Remark";
                lblRemark.HeightAnchor.ConstraintEqualTo(0).Active = true;
                lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                //     lblRemark.HeightAnchor.ConstraintEqualTo(0).Active = true;
                lblRemark.Hidden = true;
                btnDeleteRemark.Hidden = true;

            };
            AddRemarkView.AddSubview(btnDeleteRemark);

            if (POSController.remark == null || POSController.remark == "")
            {
                lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                lblRemark.HeightAnchor.ConstraintEqualTo(0).Active = true;
                btnDeleteRemark.Hidden = true;
                lblAddRemark.Text = "Add Remark";
            }
            else
            {
                lblAddRemark.Text = "Remark";
                lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor,-9).Active = true;
                lblRemark.HeightAnchor.ConstraintEqualTo(18).Active = true;
                btnDeleteRemark.Hidden = false;
            }

            RemarkImg = new UIImageView();
            RemarkImg.TranslatesAutoresizingMaskIntoConstraints = false;
            RemarkImg.Image = UIImage.FromBundle("Remark");
            AddRemarkView.AddSubview(RemarkImg);

            AddRemarkView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Remark:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            AddRemarkView.AddGestureRecognizer(tapGesture0);
            #endregion

            #region AddDiscount

            AddDiscountView = new UIView();
            AddDiscountView.TranslatesAutoresizingMaskIntoConstraints = false;
            AddDiscountView.BackgroundColor = UIColor.White;
            View.AddSubview(AddDiscountView);

            lblAddDiscount = new UILabel();
            lblAddDiscount.Text = "Add Discount";
            lblAddDiscount.TranslatesAutoresizingMaskIntoConstraints = false;
            lblAddDiscount.TextAlignment = UITextAlignment.Left;
            lblAddDiscount.Font = lblAddDiscount.Font.WithSize(15);
            lblAddDiscount.TextColor = UIColor.FromRGB(64, 64, 64);
            AddDiscountView.AddSubview(lblAddDiscount);

            DiscountImg = new UIImageView();
            DiscountImg.TranslatesAutoresizingMaskIntoConstraints = false;
            DiscountImg.Image = UIImage.FromBundle("DiscountCart");
            AddDiscountView.AddSubview(DiscountImg);

            AddDiscountView.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Discount:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            AddDiscountView.AddGestureRecognizer(tapGesture1);
            #endregion

            #region ClearCart

            ClearCartView = new UIView();
            ClearCartView.TranslatesAutoresizingMaskIntoConstraints = false;
            ClearCartView.BackgroundColor = UIColor.White;
            View.AddSubview(ClearCartView);

            lblClearCart = new UILabel();
            lblClearCart.Text = "Clear cart";
            lblClearCart.TranslatesAutoresizingMaskIntoConstraints = false;
            lblClearCart.TextAlignment = UITextAlignment.Left;
            lblClearCart.Font = lblAddDiscount.Font.WithSize(15);
            lblClearCart.TextColor = UIColor.FromRGB(227, 45, 73);
            ClearCartView.AddSubview(lblClearCart);

            ClearImage = new UIImageView();
            ClearImage.TranslatesAutoresizingMaskIntoConstraints = false;
            ClearImage.Image = UIImage.FromBundle("ClearCart");
            ClearCartView.AddSubview(ClearImage);

            ClearCartView.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("Clear:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ClearCartView.AddGestureRecognizer(tapGesture2);
            #endregion

            #region CheckOut
            CheckOutView = new UIView();
            CheckOutView.TranslatesAutoresizingMaskIntoConstraints = false;
            CheckOutView.BackgroundColor = UIColor.White;
            View.AddSubview(CheckOutView);

            btnMore = new UIButton();
            btnMore.BackgroundColor = UIColor.FromRGB(226,226,226);
            btnMore.Layer.CornerRadius = 5;
            btnMore.TranslatesAutoresizingMaskIntoConstraints = false;
            btnMore.TouchUpInside += (sender, e) => {
                DataCaching.CheckOutNavigation.DismissViewController(true, null);
            };
            CheckOutView.AddSubview(btnMore);

            moreImg = new UIImageView();
            moreImg.Image = UIImage.FromBundle("Option");
            moreImg.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            moreImg.TranslatesAutoresizingMaskIntoConstraints = false;
            btnMore.AddSubview(moreImg);

            btnCheckOut = new UIButton();
            btnCheckOut.Layer.CornerRadius = 5;
            btnCheckOut.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            btnCheckOut.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnCheckOut.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            btnCheckOut.SetTitle("Check Out", UIControlState.Normal);
            btnCheckOut.TranslatesAutoresizingMaskIntoConstraints = false;
            btnCheckOut.TouchUpInside += (sender, e) => {
                // select type
                // this.NavigationController.DismissViewController(true,null);
                Utils.SetTitle(this.NavigationController, "Payment");
                PaymentController payment = new PaymentController();
                this.NavigationController.PushViewController(payment, false);
            };
            CheckOutView.AddSubview(btnCheckOut);
            #endregion

            setupAutoLayout();
           
        }
        void setupAutoLayout()
        {
            #region OptionView
            OptionView.BottomAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            OptionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            OptionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            OptionView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            lblOption.CenterYAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblOption.LeftAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblOption.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblOption.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnCancel.CenterYAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnCancel.RightAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            btnCancel.WidthAnchor.ConstraintEqualTo(80).Active = true;
            btnCancel.HeightAnchor.ConstraintEqualTo(30).Active = true;
            #endregion

            #region AddRemarkView
            AddRemarkView.BottomAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.TopAnchor,1).Active = true;
            AddRemarkView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AddRemarkView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            AddRemarkView.HeightAnchor.ConstraintEqualTo(70).Active = true;

            RemarkImg.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            RemarkImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            RemarkImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            RemarkImg.LeftAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            btnDeleteRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnDeleteRemark.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnDeleteRemark.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnDeleteRemark.RightAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

            lblAddRemark.RightAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
           // lblAddRemark.CenterYAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblAddRemark.LeftAnchor.ConstraintEqualTo(RemarkImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;

            lblRemark.RightAnchor.ConstraintEqualTo(AddRemarkView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            lblRemark.TopAnchor.ConstraintEqualTo(lblAddRemark.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            lblRemark.LeftAnchor.ConstraintEqualTo(RemarkImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            #endregion

            #region AddDiscount
            AddDiscountView.BottomAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            AddDiscountView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AddDiscountView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            AddDiscountView.HeightAnchor.ConstraintEqualTo(70).Active = true;

            DiscountImg.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            DiscountImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            DiscountImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            DiscountImg.LeftAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            lblAddDiscount.RightAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            lblAddDiscount.CenterYAnchor.ConstraintEqualTo(AddDiscountView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblAddDiscount.LeftAnchor.ConstraintEqualTo(DiscountImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            #endregion

            #region ClearCart
            ClearCartView.BottomAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            ClearCartView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ClearCartView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ClearCartView.HeightAnchor.ConstraintEqualTo(70).Active = true;

            ClearImage.CenterYAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            ClearImage.WidthAnchor.ConstraintEqualTo(28).Active = true;
            ClearImage.HeightAnchor.ConstraintEqualTo(28).Active = true;
            ClearImage.LeftAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            lblClearCart.RightAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            lblClearCart.CenterYAnchor.ConstraintEqualTo(ClearCartView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblClearCart.LeftAnchor.ConstraintEqualTo(ClearImage.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            #endregion

            #region CheckOut
            CheckOutView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 97) / 1000).Active = true;
            CheckOutView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CheckOutView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CheckOutView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            btnMore.TopAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnMore.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnMore.BottomAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnMore.LeftAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;

            moreImg.TopAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            moreImg.RightAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;
            moreImg.BottomAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            moreImg.LeftAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;

            btnCheckOut.TopAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnCheckOut.RightAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.RightAnchor,-10).Active = true;
            btnCheckOut.BottomAnchor.ConstraintEqualTo(CheckOutView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnCheckOut.LeftAnchor.ConstraintEqualTo(btnMore.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            #endregion
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        [Export("Remark:")]
        public void Remark(UIGestureRecognizer sender)
        {
            RemarkController remark = new RemarkController();
            DataCaching.CheckOutNavigation.DismissViewController(false,null);
           DataCaching.CheckOutNavigation.PushViewController(remark, false);
        }
        [Export("Discount:")]
        public void Discount(UIGestureRecognizer sender)
        {
            DiscountController Discount = new DiscountController(POSController.tranWithDetails);
            DataCaching.CheckOutNavigation.DismissViewController(false, null);
            DataCaching.CheckOutNavigation.PushViewController(Discount, false);
        }
        [Export("Clear:")]
        public void Clear(UIGestureRecognizer sender)
        {
            // CartController.ClearCart();
            Utils.SetTitle(this.NavigationController, "Cart");
            DataCaching.CheckOutNavigation.DismissViewController(false, null);
            CartController Cart = new CartController(false);
            DataCaching.CheckOutNavigation.PushViewController(Cart, false);
            //CartController Cart = new CartController();
            //this.NavigationController.PushViewController(Cart, false);
        }
    }
}