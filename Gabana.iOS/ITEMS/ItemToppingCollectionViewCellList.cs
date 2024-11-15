using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;

namespace Gabana.iOS
{
    public class ItemToppingCollectionViewCellList : UICollectionViewCell
    {
        UILabel lblmenu,lblCost, lblShortName, lblCategory, lblSysyID;
        UIView line;
        UIImageView imageView;
        public ItemToppingCollectionViewCellList(IntPtr handle) : base(handle)
        {
            imageView = new UIImageView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(imageView);

            lblShortName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblShortName.TextColor = UIColor.White;
            lblShortName.TextAlignment = UITextAlignment.Center;
            lblShortName.Font = lblShortName.Font.WithSize(12);
            imageView.AddSubview(lblShortName);

            lblmenu = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblmenu.TextColor = UIColor.FromRGB(64,64,64);
            //lblmenu.TextColor = UIColor.FromRGB(227, 45, 73);
            lblmenu.Font = lblmenu.Font.WithSize(14);
            ContentView.AddSubview(lblmenu);

            lblSysyID = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblSysyID.TextColor = UIColor.White;
            lblSysyID.Hidden = true;
            ContentView.AddSubview(lblSysyID);

            lblCategory = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCategory.TextColor = UIColor.FromRGB(162,162,162);
            //lblmenu.TextColor = UIColor.FromRGB(227, 45, 73);
            lblCategory.Font = lblCategory.Font.WithSize(14);
            ContentView.AddSubview(lblCategory);

            lblCost = new UILabel() {
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false };
            lblCost.TextColor = UIColor.FromRGB(0, 149, 218);
            lblCost.Font = lblCost.Font.WithSize(14);
            ContentView.AddSubview(lblCost);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226,226,226);
            ContentView.AddSubview(line);

            ContentView.BackgroundColor = UIColor.White;
            setupListView();
            
        }
        private void setupListView()
        {
            imageView.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor,-5).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(((int)ContentView.Frame.Height * 7) /10).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, ((int)ContentView.Frame.Width*4)/100).Active = true;
            //  imageView.WidthAnchor.ConstraintEqualTo(((int)ContentView.Frame.Width*149) / 1000).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo((((int)ContentView.Frame.Height * 7) / 10)*4/3).Active = true;

            lblmenu.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, ((int)ContentView.Frame.Width * 4) / 100).Active = true;
           
           
            lblCategory.TopAnchor.ConstraintEqualTo(lblmenu.SafeAreaLayoutGuide.BottomAnchor,4).Active = true;
            lblCategory.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblCategory.WidthAnchor.ConstraintEqualTo(200).Active = true;

            lblCost.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblCost.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblCost.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -((int)ContentView.Frame.Width * 4) / 100).Active = true;

            lblSysyID.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblSysyID.WidthAnchor.ConstraintEqualTo(0).Active = true;
            lblSysyID.HeightAnchor.ConstraintEqualTo(0).Active = true;
            lblSysyID.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblShortName.CenterYAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortName.WidthAnchor.ConstraintEqualTo(60).Active = true;
            lblShortName.CenterXAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(1).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public string Name
        {
            get { return lblmenu.Text; }
            set { lblmenu.Text = value; }
        }
        public string Cost
        {
            get { return lblCost.Text; }
            set { lblCost.Text = "฿ "+ value; }
        }
        public string SysID
        {
            get { return lblSysyID.Text; }
            set { lblSysyID.Text = value; }
        }
        public string Category
        {
            get { return lblCategory.Text; }
            set {
                lblCategory.Text = value;
                if(value != null) {
                    if (lblCategory.Text != "" && lblCategory.Text != " " && lblCategory.Text != "\n")
                    {
                        lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, -((int)ContentView.Frame.Height) / 6).Active = true;
                    }
                    else
                    {
                        lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor,-5).Active = true;
                    }
                }
                else 
                {
                    lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                }
            }
        }
        public string Image
        {
            get { return imageView.Image.ToString(); }
            set
            {
                Utils.SetImage(imageView, value);
            }
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            if (value != null && value != "")
            {
                ImageView.Image = UIImage.FromBundle(value);
            }
        }
        public long Colors
        {
            get { return Colors; }
            set { Utils.SetColor(imageView, value); }
        }
        public string ShortName
        {
            get { return lblShortName.Text; }
            set { lblShortName.Text = value; }
        }
        

    }
}