using Foundation;
using UIKit;

namespace Gabana.iOS
{
    public partial class ChangePasswordController : UIViewController
    {

        UIView oldPassView, newPassView, confirmNewView,bottomView;
        UIButton btnChangePass;
        UILabel lblOldPass, lblNewPass, lblConfirm;
        UITextField txtOld, txtNew, txtContirm;
        public ChangePasswordController() { }
       
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = new UIColor(51 / 255f, 172 / 255f, 225 / 255f, 1);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        }
        public async override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.TopItem.Title = "Change Password";
            this.NavigationController.NavigationBar.BarTintColor = new UIColor(51 / 255f, 172 / 255f, 225 / 255f, 1);
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
            initAttribute();
            Textboxfocus(View);
            SetupAutoLayout();
        
        }
        void initAttribute()
        {
            #region bottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomView.BackgroundColor = UIColor.White;
            View.AddSubview(bottomView);

            btnChangePass = new UIButton();
            btnChangePass.SetTitle("Change Password",UIControlState.Normal);
            btnChangePass.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
            btnChangePass.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
            btnChangePass.BackgroundColor = UIColor.White;
            btnChangePass.Layer.CornerRadius = 5f;
            btnChangePass.Layer.BorderWidth = 0.5f;
            btnChangePass.Enabled = true;
            btnChangePass.TranslatesAutoresizingMaskIntoConstraints = false;
            btnChangePass.TouchUpInside += async (sender, e) => {

            };
            bottomView.AddSubview(btnChangePass);
            #endregion

            #region oldPassView
            oldPassView = new UIView();
            oldPassView.TranslatesAutoresizingMaskIntoConstraints = false;
            oldPassView.BackgroundColor = UIColor.White;
            View.AddSubview(oldPassView);

            lblOldPass= new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblOldPass.Font = lblOldPass.Font.WithSize(15);
            lblOldPass.TranslatesAutoresizingMaskIntoConstraints = false;
            lblOldPass.Text = "Old Password";
            oldPassView.AddSubview(lblOldPass);

            txtOld = new UITextField
            {
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtOld.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtOld.KeyboardType = UIKeyboardType.Default;
            txtOld.Font = txtOld.Font.WithSize(15);
            oldPassView.AddSubview(txtOld);
            #endregion

            #region newPassView
            newPassView = new UIView();
            newPassView.TranslatesAutoresizingMaskIntoConstraints = false;
            newPassView.BackgroundColor = UIColor.White;
            View.AddSubview(newPassView);

            lblNewPass = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblNewPass.Font = lblNewPass.Font.WithSize(15);
            lblNewPass.TranslatesAutoresizingMaskIntoConstraints = false;
            lblNewPass.Text = "New Password";
            newPassView.AddSubview(lblNewPass);

            txtNew = new UITextField
            {
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtNew.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtNew.KeyboardType = UIKeyboardType.Default;
            txtNew.Font = txtNew.Font.WithSize(15);
            newPassView.AddSubview(txtNew);
            #endregion

            #region confirmNewView
            confirmNewView = new UIView();
            confirmNewView.TranslatesAutoresizingMaskIntoConstraints = false;
            confirmNewView.BackgroundColor = UIColor.White;
            View.AddSubview(confirmNewView);

            lblConfirm = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblConfirm.Font = lblConfirm.Font.WithSize(15);
            lblConfirm.TranslatesAutoresizingMaskIntoConstraints = false;
            lblConfirm.Text = "Old Password";
            confirmNewView.AddSubview(lblConfirm);

            txtContirm = new UITextField
            {
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtContirm.AttributedPlaceholder = new NSAttributedString("********", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtContirm.KeyboardType = UIKeyboardType.Default;
            txtContirm.Font = txtContirm.Font.WithSize(15);
            confirmNewView.AddSubview(txtContirm);
            #endregion
        }
        void SetupAutoLayout()
        {
            #region bottomView
            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*97)/1000).Active = true;

            btnChangePass.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnChangePass.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnChangePass.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnChangePass.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            #endregion

            #region oldPassView
            oldPassView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            oldPassView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            oldPassView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            oldPassView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 89) / 1000).Active = true;

            lblOldPass.RightAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblOldPass.LeftAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width*4)/100).Active = true;
            lblOldPass.TopAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height * 16) / 1000).Active = true;

            txtOld.RightAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            txtOld.LeftAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            txtOld.TopAnchor.ConstraintEqualTo(lblOldPass.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            #endregion

            #region newPassView
            newPassView.TopAnchor.ConstraintEqualTo(oldPassView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            newPassView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            newPassView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            newPassView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 89) / 1000).Active = true;

            lblNewPass.RightAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblNewPass.LeftAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblNewPass.TopAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height * 16) / 1000).Active = true;

            txtNew.RightAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            txtNew.LeftAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            txtNew.TopAnchor.ConstraintEqualTo(lblNewPass.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            #endregion

            #region confirmNewView
            confirmNewView.TopAnchor.ConstraintEqualTo(newPassView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            confirmNewView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            confirmNewView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            confirmNewView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 89) / 1000).Active = true;

            lblConfirm.RightAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            lblConfirm.LeftAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            lblConfirm.TopAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height * 16) / 1000).Active = true;

            txtContirm.RightAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Width * 4) / 100).Active = true;
            txtContirm.LeftAnchor.ConstraintEqualTo(confirmNewView.SafeAreaLayoutGuide.LeftAnchor, ((int)View.Frame.Width * 4) / 100).Active = true;
            txtContirm.TopAnchor.ConstraintEqualTo(lblConfirm.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
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