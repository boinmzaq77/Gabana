﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class CartFooterCollectionViewCell : UICollectionViewCell
    {
        UILabel lblmenu, lblCost, lblShortName;
        UIView line;
        UIImageView imageView;
        public CartFooterCollectionViewCell(IntPtr handle) : base(handle)
        {
            imageView = new UIImageView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(imageView);

            lblShortName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblShortName.TextColor = UIColor.White;
            lblShortName.TextAlignment = UITextAlignment.Center;
            lblShortName.Font = lblShortName.Font.WithSize(12);
            ContentView.AddSubview(lblShortName);

            lblmenu = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblmenu.TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1f);
            //lblmenu.TextColor = UIColor.FromRGB(227, 45, 73);
            lblmenu.Font = lblmenu.Font.WithSize(12);
            ContentView.AddSubview(lblmenu);

            lblCost = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCost.TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1f);
            lblCost.Font = lblCost.Font.WithSize(12);
            ContentView.AddSubview(lblCost);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
        private void setupListView()
        {
            imageView.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(45).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(80).Active = true;

            lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblmenu.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblmenu.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblmenu.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblCost.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblCost.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblCost.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblCost.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lblShortName.HeightAnchor.ConstraintEqualTo(14).Active = true;
            lblShortName.CenterYAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortName.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblShortName.CenterXAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -1).Active = true;
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
            set { lblCost.Text = value; }
        }
        public string Image
        {
            get { return imageView.Image.ToString(); }
            set
            {
                SetImage(imageView, value);
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