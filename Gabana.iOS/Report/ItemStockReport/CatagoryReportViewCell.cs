using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class CatagoryReportViewCell : UICollectionViewCell
    {
        UIView line;
        UILabel lblCatagory,lblCatagoryItemSum;
        UIImageView SelectCategory;

        public CatagoryReportViewCell(IntPtr handle) : base(handle)
        {


            lblCatagory = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCatagory.TextColor = UIColor.FromRGB(64, 64, 64);
            lblCatagory.Font = lblCatagory.Font.WithSize(14);
            ContentView.AddSubview(lblCatagory);


            lblCatagoryItemSum = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCatagoryItemSum.TextColor = UIColor.FromRGB(162, 162, 162);
            lblCatagoryItemSum.Font = lblCatagoryItemSum.Font.WithSize(14);
            ContentView.AddSubview(lblCatagoryItemSum);


            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

            SelectCategory = new UIImageView();
            SelectCategory.Hidden = true;
            SelectCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            SelectCategory.Image = UIImage.FromBundle("Check");
            ContentView.AddSubview(SelectCategory);



            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
       
        private void setupListView()
        {

            lblCatagory.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblCatagory.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor,-8).Active = true;
            lblCatagory.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblCatagory.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lblCatagoryItemSum.TopAnchor.ConstraintEqualTo(lblCatagory.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lblCatagoryItemSum.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblCatagoryItemSum.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblCatagoryItemSum.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;


            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            SelectCategory.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            SelectCategory.WidthAnchor.ConstraintEqualTo(20).Active = true;
            SelectCategory.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -30).Active = true;
            SelectCategory.HeightAnchor.ConstraintEqualTo(20).Active = true;
        }
        public bool Select
        {
            get { return SelectCategory.Hidden; }
            set
            {
                SelectCategory.Hidden = true;
                if (value)
                {
                    SelectCategory.Hidden = false;
                }
            }
        }
        public string Name
        {
            get { return lblCatagory.Text; }
            set { lblCatagory.Text = value;
            }
        }
        public string Sum
        {
            get { return lblCatagoryItemSum.Text; }
            set { lblCatagoryItemSum.Text = value; }
        }


    }
}