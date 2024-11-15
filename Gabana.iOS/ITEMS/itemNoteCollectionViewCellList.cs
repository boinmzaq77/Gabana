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
        UILabel lblNote,lblCount;
        UIImageView btnNext;
        UIView line;
        public itemNoteCollectionViewCellList(IntPtr handle) : base(handle)
        {
            lblNote = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblNote.TextColor = UIColor.FromRGB(64, 64, 64);
            lblNote.Font = lblNote.Font.WithSize(14);
            ContentView.AddSubview(lblNote);

            lblCount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCount.TextColor = UIColor.FromRGB(172,172,172);
            lblCount.Font = lblCount.Font.WithSize(14);
            ContentView.AddSubview(lblCount);

            btnNext = new UIImageView();
            btnNext.Image = UIImage.FromBundle("Next");
            btnNext.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(btnNext);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            lblNote.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 12).Active = true;
            lblNote.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNote.RightAnchor.ConstraintEqualTo(btnNext.SafeAreaLayoutGuide.LeftAnchor, -20).Active = true;
            lblNote.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;

            lblCount.TopAnchor.ConstraintEqualTo(lblNote.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblCount.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblCount.RightAnchor.ConstraintEqualTo(btnNext.SafeAreaLayoutGuide.LeftAnchor, -20).Active = true;
            lblCount.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;

            btnNext.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnNext.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnNext.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -24).Active = true;
            btnNext.WidthAnchor.ConstraintEqualTo(28).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -1).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public string NoteItem
        {
            get { return lblNote.Text; }
            set { lblNote.Text = value; }
        }
        public string CountItem
        {
            get { return lblCount.Text; }
            set { lblCount.Text = value; }
        }
    }
}