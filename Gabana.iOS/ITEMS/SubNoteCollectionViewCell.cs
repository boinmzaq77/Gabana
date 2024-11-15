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
    public class SubNoteCollectionViewCell : UICollectionViewCell
    {
        UIView SubNoteView;
        UILabel lbltxtSubnote;
        public static UITextField txtSubnote;
        UIImageView btnClose;
      
        public SubNoteCollectionViewCell(IntPtr handle) : base(handle)
        {
            InitAttribute();
            setupView();
            Textboxfocus(ContentView);
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

            #region SubNoteView
            SubNoteView = new UIView();
            SubNoteView.TranslatesAutoresizingMaskIntoConstraints = false;
            SubNoteView.BackgroundColor = UIColor.White;
            SubNoteView.Layer.CornerRadius = 5;
            SubNoteView.ClipsToBounds = true;
            ContentView.AddSubview(SubNoteView);

            lbltxtSubnote = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbltxtSubnote.Font = lbltxtSubnote.Font.WithSize(15);
            lbltxtSubnote.Text = "Sub Note";
            SubNoteView.AddSubview(lbltxtSubnote);

            txtSubnote = new UITextField
            {
                AttributedPlaceholder = new NSAttributedString("Sub Note", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) }),
                TextAlignment = UITextAlignment.Left,
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtSubnote.ReturnKeyType = UIReturnKeyType.Done;
            txtSubnote.ShouldReturn = (tf) =>
            {
                ContentView.EndEditing(true);
                return true;
            };
            txtSubnote.Font = txtSubnote.Font.WithSize(15);
            SubNoteView.AddSubview(txtSubnote);
            #endregion

            btnClose = new UIImageView();
            btnClose.Image = UIImage.FromBundle("Del2");
            btnClose.TranslatesAutoresizingMaskIntoConstraints = false;
            SubNoteView.AddSubview(btnClose);

            btnClose.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("Delete:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnClose.AddGestureRecognizer(tapGesture0);

        }
        [Export("Delete:")]
        public void Delete(UIGestureRecognizer sender)
        {
            OnCardCellDeleteCodeBtn(this);
        }
        #region Events
        public delegate void CardCellDelegate(SubNoteCollectionViewCell subNoteCollectionViewCell);
        public event CardCellDelegate OnCardCellDeleteCodeBtn;
        #endregion
        private void setupView()
        {
            #region SizeNameView
            SubNoteView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            SubNoteView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-10).Active = true;
            SubNoteView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor,10).Active = true;
            SubNoteView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            btnClose.CenterYAnchor.ConstraintEqualTo(SubNoteView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnClose.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnClose.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnClose.RightAnchor.ConstraintEqualTo(SubNoteView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lbltxtSubnote.TopAnchor.ConstraintEqualTo(SubNoteView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lbltxtSubnote.RightAnchor.ConstraintEqualTo(btnClose.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            lbltxtSubnote.LeftAnchor.ConstraintEqualTo(SubNoteView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtSubnote.TopAnchor.ConstraintEqualTo(lbltxtSubnote.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtSubnote.RightAnchor.ConstraintEqualTo(btnClose.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            txtSubnote.LeftAnchor.ConstraintEqualTo(SubNoteView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            #endregion
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        public string SubnoteName
        {
            get { return txtSubnote.Text; }
            set { txtSubnote.Text = value; }
        }
       
    }
}