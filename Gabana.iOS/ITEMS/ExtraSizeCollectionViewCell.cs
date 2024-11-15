using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using System.Drawing;

namespace Gabana.iOS
{
    public class ExtraSizeCollectionViewCell : UICollectionViewCell
    {
        UIView SizeNameView, PriceView, EstimateView;
        UILabel  lbltxtSizeName, lbltxtEstimateCost, lbltxtPrice;
        UITextField txtSizeName, txtEstimateCost, txtPrice;  
        UIImageView btnClose;
        private int nub;
      
        public ExtraSizeCollectionViewCell(IntPtr handle) : base(handle)
        {
            InitAttribute();
            setupView();
            Textboxfocus(ContentView);
         //   ContentView.BackgroundColor = UIColor.Red;
        }
        private void InitAttribute()
        {
            UIToolbar NumpadToolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, (float)ContentView.Frame.Width, 44.0f));
            NumpadToolbar.Translucent = true;
            NumpadToolbar.Items = new UIBarButtonItem[]{
            new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                 ContentView.EndEditing(true);
            }) };
            NumpadToolbar.SizeToFit();

            #region SizeNameView
            SizeNameView = new UIView();
            SizeNameView.TranslatesAutoresizingMaskIntoConstraints = false;
            SizeNameView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(SizeNameView);

            lbltxtSizeName = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtSizeName.Font = lbltxtSizeName.Font.WithSize(15);
            lbltxtSizeName.Text = Utils.TextBundle("sizename", "Size Name");
            SizeNameView.AddSubview(lbltxtSizeName);

            txtSizeName = new UITextField
            {
                AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("sizename", "sizename"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) }),
                TextAlignment = UITextAlignment.Left,
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
                
            };
            txtSizeName.EditingChanged += TxtSizeName_EditingChanged;
            txtSizeName.EditingDidEnd += (object sender, EventArgs e) => 
            {
                txtSizeName.Text = ((UITextField)sender).Text;
            };
            txtSizeName.ReturnKeyType = UIReturnKeyType.Next;
            txtSizeName.ShouldReturn = (tf) =>
            {
                txtPrice.BecomeFirstResponder();
                return true;
            };
            txtSizeName.Font = txtSizeName.Font.WithSize(15);
            SizeNameView.AddSubview(txtSizeName);
            #endregion

            #region PriceView
            PriceView = new UIView();
            PriceView.TranslatesAutoresizingMaskIntoConstraints = false;
            PriceView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(PriceView);

            lbltxtPrice = new UILabel
            {

                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtPrice.Font = lbltxtPrice.Font.WithSize(15);
            lbltxtPrice.Text = Utils.TextBundle("price", "Price");
            PriceView.AddSubview(lbltxtPrice);

            txtPrice = new UITextField
            {
                AttributedPlaceholder = new NSAttributedString("0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) }),
                TextAlignment = UITextAlignment.Left,
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtPrice.EditingChanged += TxtPrice_EditingChanged;
            txtPrice.InputAccessoryView = NumpadToolbar;
            txtPrice.Font = txtPrice.Font.WithSize(15);
            txtPrice.KeyboardType = UIKeyboardType.DecimalPad;
            PriceView.AddSubview(txtPrice);
            #endregion

            #region EstimateView
            EstimateView = new UIView();
            EstimateView.TranslatesAutoresizingMaskIntoConstraints = false;
            EstimateView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(EstimateView);

            lbltxtEstimateCost = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtEstimateCost.Font = lbltxtEstimateCost.Font.WithSize(15);
            lbltxtEstimateCost.Text = Utils.TextBundle("estimatecost", "Estimate Cost");
            EstimateView.AddSubview(lbltxtEstimateCost);

            txtEstimateCost = new UITextField
            {
                AttributedPlaceholder = new NSAttributedString("0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) }),
                TextAlignment = UITextAlignment.Left,
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtEstimateCost.EditingChanged += TxtEstimateCost_EditingChanged;
            txtEstimateCost.InputAccessoryView = NumpadToolbar;
            txtEstimateCost.KeyboardType = UIKeyboardType.DecimalPad;
            txtEstimateCost.Font = txtEstimateCost.Font.WithSize(15);
            EstimateView.AddSubview(txtEstimateCost);
            #endregion

            btnClose = new UIImageView();
            btnClose.Image = UIImage.FromFile("DeleteSize.png");
            btnClose.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(btnClose);

            btnClose.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("Delete:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnClose.AddGestureRecognizer(tapGesture0);

        }

        private void TxtEstimateCost_EditingChanged(object sender, EventArgs e)
        {
            txtEstimateCost.Text = ((UITextField)sender).Text;
        }

        private void TxtPrice_EditingChanged(object sender, EventArgs e)
        {
            txtPrice.Text = ((UITextField)sender).Text;
        }

        private void TxtSizeName_EditingChanged(object sender, EventArgs e)
        {
            txtSizeName.Text = ((UITextField)sender).Text;
        }

        [Export("Delete:")]
        public void Delete(UIGestureRecognizer sender)
        {
            OnCardCellDeleteCodeBtn(this);
        }
        #region Events
        public delegate void CardCellDelegate(ExtraSizeCollectionViewCell extraSizeCollectionViewCell);
        public event CardCellDelegate OnCardCellDeleteCodeBtn;
        #endregion
        private void setupView()
        {
            btnClose.TopAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.TopAnchor, -10).Active = true;
            btnClose.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnClose.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnClose.RightAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            #region SizeNameView
            SizeNameView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 15).Active = true;
            SizeNameView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-15).Active = true;
            SizeNameView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor,15).Active = true;
            SizeNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbltxtSizeName.TopAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lbltxtSizeName.RightAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lbltxtSizeName.LeftAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtSizeName.TopAnchor.ConstraintEqualTo(lbltxtSizeName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtSizeName.RightAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            txtSizeName.LeftAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            #endregion

            #region PriceView
            PriceView.TopAnchor.ConstraintEqualTo(SizeNameView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            PriceView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            PriceView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            PriceView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbltxtPrice.TopAnchor.ConstraintEqualTo(PriceView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lbltxtPrice.RightAnchor.ConstraintEqualTo(PriceView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lbltxtPrice.LeftAnchor.ConstraintEqualTo(PriceView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtPrice.TopAnchor.ConstraintEqualTo(lbltxtPrice.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtPrice.RightAnchor.ConstraintEqualTo(PriceView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            txtPrice.LeftAnchor.ConstraintEqualTo(PriceView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            #endregion

            #region EstimateView
            EstimateView.TopAnchor.ConstraintEqualTo(PriceView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            EstimateView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            EstimateView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            EstimateView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbltxtEstimateCost.TopAnchor.ConstraintEqualTo(EstimateView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lbltxtEstimateCost.RightAnchor.ConstraintEqualTo(EstimateView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lbltxtEstimateCost.LeftAnchor.ConstraintEqualTo(EstimateView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtEstimateCost.TopAnchor.ConstraintEqualTo(lbltxtEstimateCost.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtEstimateCost.RightAnchor.ConstraintEqualTo(EstimateView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            txtEstimateCost.LeftAnchor.ConstraintEqualTo(EstimateView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            #endregion
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        public string SizeName
        {
            get { return txtSizeName.Text; }
            set { txtSizeName.Text = value; }
        }
        public string EstimateCost
        {
            get { return txtEstimateCost.Text; }
            set { txtEstimateCost.Text = value; }
        }
       
        public string Price
        {
            get { return txtPrice.Text; }
            set { txtPrice.Text = value; }
        }

        public int Nub
        {
            get { return nub; }
            set { nub = value; }
        }
    }
}