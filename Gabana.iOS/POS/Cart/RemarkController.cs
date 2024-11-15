using Foundation;
using Gabana.iOS;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.POS.Cart
{
    public partial class RemarkController : UIViewController
    {
        UIView RemarkVieW,buttonView;
        UILabel lblTxtRemark;
        UITextField txtRemark;
        UIButton btnAddRemark;
        public RemarkController()
        {
           
        }
       
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (!string.IsNullOrEmpty( POSController.tranWithDetails.tran.Comments))
            {
                txtRemark.Text = POSController.tranWithDetails.tran.Comments; 
            }
            //POSController.tranWithDetails.tran.Comments = txtRemark.Text;

        }
        public override void ViewDidLoad()
        {
            UIBarButtonItem clearVat = new UIBarButtonItem();
            clearVat.Title = Utils.TextBundle("clear", "Items");
            clearVat.Clicked += (sender, e) => {
                txtRemark.Text = "";
                btnAddRemark.Enabled = false;
                btnAddRemark.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAddRemark.BackgroundColor = UIColor.White;
            };
            this.NavigationItem.RightBarButtonItem = clearVat;
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            //this.NavigationController.NavigationBar.TopItem.Title = "Add Remark";
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("addremark", "Items"));
            base.ViewDidLoad();
            //  View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            View.BackgroundColor = UIColor.White;

            #region TopAmountView
            RemarkVieW = new UIView();
            RemarkVieW.BackgroundColor = UIColor.White;
            RemarkVieW.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(RemarkVieW);

            //lblTxtAmount, lblAmount;
            lblTxtRemark = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64,64,64),
              
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTxtRemark.Font = lblTxtRemark.Font.WithSize(15);
            lblTxtRemark.Text = Utils.TextBundle("remark", "Items");
            RemarkVieW.AddSubview(lblTxtRemark);

            txtRemark = new UITextField
            {
                Placeholder = Utils.TextBundle("remark", "Items"),
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
                
            };
            txtRemark.Font = txtRemark.Font.WithSize(15);
            txtRemark.EditingChanged += (sender, e) => {
                if (txtRemark.Text != "")
                {
                    btnAddRemark.Enabled = true;
                    btnAddRemark.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnAddRemark.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                    return;
                }
                else
                {
                    btnAddRemark.Enabled = false;
                    btnAddRemark.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnAddRemark.BackgroundColor = UIColor.White;
                }
            };
            txtRemark.ReturnKeyType = UIReturnKeyType.Done;
            txtRemark.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            RemarkVieW.AddSubview(txtRemark);
            #endregion


            #region buttonView
            buttonView = new UIView();
            buttonView.BackgroundColor = UIColor.White;
            buttonView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(buttonView);

            btnAddRemark = new UIButton();
            btnAddRemark.Layer.CornerRadius = 5;
            btnAddRemark.ClipsToBounds = true;
            btnAddRemark.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnAddRemark.Layer.BorderWidth = 0.5f;
            btnAddRemark.SetTitle(Utils.TextBundle("done", "Items"), UIControlState.Normal);
            btnAddRemark.BackgroundColor = UIColor.White;
            btnAddRemark.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnAddRemark.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddRemark.TouchUpInside += async (sender, e) => {
                //Done
                POSController.tranWithDetails.tran.Comments = txtRemark.Text;
                //this.NavigationController.DismissViewController(false, null);
                //CartController Cart = new CartController(false);
                this.NavigationController.PopViewController(false);
            };
            buttonView.AddSubview(btnAddRemark);
            #endregion

            setUpAutoLayout();

            if(txtRemark.Text == "" || txtRemark.Text == null)
            {
                btnAddRemark.Enabled = false;
            }
            Textboxfocus(View);
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        void setUpAutoLayout()
        {
            #region TopAmountView
            RemarkVieW.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor,0).Active=true;
            RemarkVieW.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            RemarkVieW.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            RemarkVieW.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height*90/1000).Active = true;

            lblTxtRemark.TopAnchor.ConstraintEqualTo(RemarkVieW.SafeAreaLayoutGuide.TopAnchor,((int)View.Frame.Height*16)/1000).Active = true;
            lblTxtRemark.LeftAnchor.ConstraintEqualTo(RemarkVieW.SafeAreaLayoutGuide.LeftAnchor, 30).Active = true;
            lblTxtRemark.RightAnchor.ConstraintEqualTo(RemarkVieW.SafeAreaLayoutGuide.RightAnchor, -30).Active = true;
            lblTxtRemark.HeightAnchor.ConstraintEqualTo(20).Active = true;

            txtRemark.TopAnchor.ConstraintEqualTo(lblTxtRemark.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            txtRemark.LeftAnchor.ConstraintEqualTo(RemarkVieW.SafeAreaLayoutGuide.LeftAnchor, 30).Active = true;
            txtRemark.RightAnchor.ConstraintEqualTo(RemarkVieW.SafeAreaLayoutGuide.RightAnchor, -30).Active = true;
            txtRemark.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region buttonView
            buttonView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            buttonView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            buttonView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            buttonView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnAddRemark.TopAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddRemark.LeftAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddRemark.RightAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnAddRemark.BottomAnchor.ConstraintEqualTo(buttonView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            #endregion
        }
    }
}