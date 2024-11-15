using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class ItemPOSCollectionViewCellList : UICollectionViewCell
    {
        UILabel lblmenu, lblCost, lblShortName, lblCategory, lblSysyID, lbldelete, lblamount;
        UIView line, viewdelete, viewall;
        UIImageView imageView, imageDelete;
        UIButton btndeleteitem;
        NSLayoutConstraint leftimage; 
        public ItemPOSCollectionViewCellList(IntPtr handle) : base(handle)
        {
            var frame = this.Frame;
            frame.X = 0;
            this.Frame = frame; 

            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);

            imageView = new UIImageView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            viewall.AddSubview(imageView);

            lblShortName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblShortName.TextColor = UIColor.White;
            lblShortName.TextAlignment = UITextAlignment.Center;
            lblShortName.Font = lblShortName.Font.WithSize(12);
            imageView.AddSubview(lblShortName);

            lblmenu = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblmenu.TextColor = UIColor.FromRGB(64, 64, 64);
            lblmenu.TextAlignment = UITextAlignment.Left;
            //lblmenu.TextColor = UIColor.FromRGB(227, 45, 73);
            lblmenu.Font = lblmenu.Font.WithSize(15);
            viewall.AddSubview(lblmenu);

            lblamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblamount.TextColor = UIColor.FromRGB(64, 64, 64);
            //lblmenu.TextColor = UIColor.FromRGB(227, 45, 73);
            lblamount.Font = lblmenu.Font.WithSize(14);
            lblamount.Hidden = true; 
            viewall.AddSubview(lblamount);

            lblSysyID = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblSysyID.TextColor = UIColor.White;
            lblSysyID.Hidden = true;
            viewall.AddSubview(lblSysyID);

            lblCategory = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCategory.TextColor = UIColor.FromRGB(162, 162, 162);
            //lblmenu.TextColor = UIColor.FromRGB(227, 45, 73);
            lblCategory.Font = lblCategory.Font.WithSize(14);
            viewall.AddSubview(lblCategory);

            lblCost = new UILabel()
            {
                TextAlignment = UITextAlignment.Right,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCost.TextColor = UIColor.FromRGB(0, 149, 218);
            lblCost.Font = lblCost.Font.WithSize(14);
            viewall.AddSubview(lblCost);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);


            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }

        private void setupListView()
        {
            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -5).Active = true;
            viewall.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            
            imageView.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(45).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            //  imageView.WidthAnchor.ConstraintEqualTo(((int)ContentView.Frame.Width*149) / 1000).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(65).Active = true;

            //imageView.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            //imageView.HeightAnchor.ConstraintEqualTo(((int)ContentView.Frame.Height * 7) / 10).Active = true;
            //imageView.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ////  imageView.WidthAnchor.ConstraintEqualTo(((int)ContentView.Frame.Width*149) / 1000).Active = true;
            //imageView.WidthAnchor.ConstraintEqualTo((((int)ContentView.Frame.Height * 7) / 10) * 4 / 3).Active = true;
            imageView.BackgroundColor = UIColor.White;

            lblamount.BackgroundColor = UIColor.Gray;
            lblamount.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblamount.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            lblamount.HeightAnchor.ConstraintEqualTo(20).Active = true;
            //lblmenu.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, ((int)ContentView.Frame.Width * 4) / 100).Active = true;
            lblmenu.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            lblmenu.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblmenu.RightAnchor.ConstraintEqualTo(lblCost.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            //lblmenu.BackgroundColor = UIColor.Red;

            lblCategory.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblCategory.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 10).Active = true;
            lblCategory.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblCategory.RightAnchor.ConstraintEqualTo(lblCost.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;

            lblCost.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblCost.WidthAnchor.ConstraintEqualTo(150).Active = true;
            //lblCost.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -((int)ContentView.Frame.Width * 4) / 100).Active = true;
            lblCost.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor,-5).Active = true;

            lblSysyID.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblSysyID.WidthAnchor.ConstraintEqualTo(0).Active = true;
            lblSysyID.HeightAnchor.ConstraintEqualTo(0).Active = true;
            lblSysyID.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblShortName.CenterYAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortName.WidthAnchor.ConstraintEqualTo(60).Active = true;
            lblShortName.CenterXAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
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
            set { lblCost.Text =  value; }
        }
        public string SysID
        {
            get { return lblSysyID.Text; }
            set { lblSysyID.Text = value; }
        }
        
        public string Category
        {
            get { return lblCategory.Text; }
            set
            {
                lblCategory.Text = value;
                if (value != null)
                {
                    if (lblCategory.Text != "" && lblCategory.Text != " " && lblCategory.Text != "\n")
                    {
                        lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, -((int)ContentView.Frame.Height) / 6).Active = true;
                    }
                    else
                    {
                        lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, -5).Active = true;
                    }
                }
                else
                {
                    lblmenu.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
                }
            }
        }
        public Item Image
        {
          
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrEmpty(value.ThumbnailLocalPath))
                    {
                        Utils.SetImageItem(imageView, value);

                    }
                    else
                    {
                        Utils.SetImageItemlocal(imageView, value);
                    }
                }
                else
                {
                    imageView.Image = null;
                }
                
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
        public delegate void itemPOSSelectedDelegate(ItemPOSCollectionViewCellList indexPath);
        public event itemPOSSelectedDelegate OnItemSwipe;

        public delegate void itemPOSdeleteDelegate(ItemPOSCollectionViewCellList indexPath);
        public event itemPOSdeleteDelegate OnDeleteItem;
    }
}