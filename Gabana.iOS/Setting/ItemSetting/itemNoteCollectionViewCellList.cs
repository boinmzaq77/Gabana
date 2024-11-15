using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class itemNoteCollectionViewCellList : UICollectionViewCell
    {
        UILabel lblNote,lblNoteCategory;
        
        UIView viewdelete, viewall;
        UIImageView imageDelete;
        public itemNoteCollectionViewCellList(IntPtr handle) : base(handle)
        {
            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);

            lblNote = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblNote.TextColor = UIColor.FromRGB(64, 64, 64);
            lblNote.Font = lblNote.Font.WithSize(14);
            viewall.AddSubview(lblNote);

            lblNoteCategory = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblNoteCategory.TextColor = UIColor.FromRGB(162, 162, 162);
            lblNoteCategory.Font = lblNoteCategory.Font.WithSize(14);
            viewall.AddSubview(lblNoteCategory);

           

            viewdelete = new UIView();
            viewdelete.TranslatesAutoresizingMaskIntoConstraints = false;
            viewdelete.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            viewdelete.Alpha = 0;
            ContentView.AddSubview(viewdelete);

            #region imagedelete
            imageDelete = new UIImageView();
            imageDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            imageDelete.Image = UIImage.FromFile("DeleteBt2.png");
            imageDelete.ContentMode = UIViewContentMode.ScaleAspectFit;
            viewdelete.AddSubview(imageDelete);

            viewdelete.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("imageDelete:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            viewdelete.AddGestureRecognizer(tapGesture2);
            #endregion

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
            this.AddGestureRecognizer(swipeRight);

        }
        private void setupView()
        {
            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            lblNote.TopAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblNote.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNote.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblNote.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblNoteCategory.TopAnchor.ConstraintEqualTo(lblNote.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            lblNoteCategory.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNoteCategory.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblNoteCategory.HeightAnchor.ConstraintEqualTo(18).Active = true;

            viewdelete.TopAnchor.ConstraintEqualTo(viewall.TopAnchor, 0).Active = true;
            viewdelete.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.WidthAnchor.ConstraintEqualTo(60).Active = true;
            viewdelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.BottomAnchor.ConstraintEqualTo(viewall.BottomAnchor, 4).Active = true;

            imageDelete.TopAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            imageDelete.BottomAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            imageDelete.LeftAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            imageDelete.RightAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            

            
        }
        public string NoteItem
        {
            get { return lblNote.Text; }
            set { lblNote.Text = value; 
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
            }
        }
        public bool showbtndelete
        {
            get { return viewdelete.Hidden; }
            set
            {
                if (value)
                {
                    viewdelete.Alpha = 1;
                }
                else
                {
                    viewdelete.Alpha = 0;
                }
            }
        }
        public string NoteCategory
        {
            get { return lblNoteCategory.Text; }
            set { lblNoteCategory.Text = value; }
        }
        private void swiped(UISwipeGestureRecognizer s)
        {
            OnItemSwipe?.Invoke(this);
        }
        public delegate void branchSelectedDelegate(itemNoteCollectionViewCellList indexPath);
        public event branchSelectedDelegate OnItemSwipe;
        public delegate void branchdeleteDelegate(itemNoteCollectionViewCellList indexPath);
        public event branchdeleteDelegate OnDeleteItem;
        [Export("imageDelete:")]
        public void DeleteItem(UIGestureRecognizer sender)
        {
            OnDeleteItem?.Invoke(this);
        }
    }
}