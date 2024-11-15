﻿using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class CellItemList : UICollectionViewCell
    {
        UILabel lblname, lblCost, lblShortName, lblCategory, lblSysyID;
        UIView line, viewdelete, viewall;
        UIImageView StockImg;
        UIImageView imageView, imageDelete, imagefav;

        public CellItemList(IntPtr handle) : base(handle)
        {
            viewall = new UIView();
            viewall.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.BackgroundColor = UIColor.White;
            viewall.Alpha = 1;
            ContentView.AddSubview(viewall);

            imageView = new UIImageView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.ContentMode = UIViewContentMode.ScaleToFill;
            viewall.AddSubview(imageView);

            imageView.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("image:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            imageView.AddGestureRecognizer(tapGesture4);




            StockImg = new UIImageView();
            StockImg.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.AddSubview(StockImg);

            lblShortName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblShortName.TextColor = UIColor.White;
            lblShortName.TextAlignment = UITextAlignment.Center;
            lblShortName.Font = lblShortName.Font.WithSize(12);
            imageView.AddSubview(lblShortName);

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblname.TextColor = UIColor.FromRGB(64, 64, 64);
            lblname.Font = lblname.Font.WithSize(14);
            viewall.AddSubview(lblname);

            lblSysyID = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblSysyID.TextColor = UIColor.White;
            lblSysyID.Hidden = true;
            viewall.AddSubview(lblSysyID);

            lblCategory = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCategory.TextColor = UIColor.FromRGB(162, 162, 162);
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

            imagefav = new UIImageView();
            imagefav.TranslatesAutoresizingMaskIntoConstraints = false;
            viewall.AddSubview(imagefav);

            imagefav.UserInteractionEnabled = true;
            var tapGesture3 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("imageFav:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            imagefav.AddGestureRecognizer(tapGesture3);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            ContentView.AddSubview(line);

            viewdelete = new UIView();
            viewdelete.TranslatesAutoresizingMaskIntoConstraints = false;
            viewdelete.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            viewdelete.Alpha = 1;
            ContentView.AddSubview(viewdelete);

            #region imagedelete
            imageDelete = new UIImageView();
            imageDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            imageDelete.Image = UIImage.FromFile("DeleteBt2.png");
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
            setupListView();

            var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Left;
            this.AddGestureRecognizer(swipeRight);

        }
        //
        [Export("imageFav:")]
        public void imageFav(UIGestureRecognizer sender)
        {
            OnFavItem?.Invoke(this);
        }

        [Export("imageDelete:")]
        public void DeleteItem(UIGestureRecognizer sender)
        {
            OnDeleteItem?.Invoke(this);
        }

        private void swiped(UISwipeGestureRecognizer s)
        {
            OnItemSwipe?.Invoke(this);
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

        }
        [Export("image:")]
        public void image(UIGestureRecognizer sender)
        {
            OnItem?.Invoke(this);
        }
        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();

        }
        private void setupListView()
        {

            imageView.CenterYAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(85).Active = true;
            imageView.BackgroundColor = UIColor.White;

            StockImg.CenterXAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            StockImg.CenterYAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            StockImg.HeightAnchor.ConstraintEqualTo(18).Active = true;
            StockImg.WidthAnchor.ConstraintEqualTo(18).Active = true;

            lblname.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblname.BottomAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, -5).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(lblCost.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;

            lblCategory.TopAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.CenterYAnchor, 5).Active = true;
            lblCategory.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblCategory.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblCategory.RightAnchor.ConstraintEqualTo(lblCost.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            lblCost.CenterYAnchor.ConstraintEqualTo(lblname.SafeAreaLayoutGuide.CenterYAnchor,0).Active = true;
            lblCost.WidthAnchor.ConstraintEqualTo(120).Active = true;
            lblCost.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblCost.HeightAnchor.ConstraintEqualTo(16).Active = true;

            imagefav.CenterYAnchor.ConstraintEqualTo(lblCategory.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            imagefav.WidthAnchor.ConstraintEqualTo(30).Active = true;
            imagefav.HeightAnchor.ConstraintEqualTo(30).Active = true;
            imagefav.RightAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

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

            viewall.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            viewall.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewall.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            viewdelete.TopAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            viewdelete.LeftAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.WidthAnchor.ConstraintEqualTo(80).Active = true;
            viewdelete.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            viewdelete.BottomAnchor.ConstraintEqualTo(viewall.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;


            imageDelete.TopAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            imageDelete.BottomAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            imageDelete.LeftAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            imageDelete.RightAnchor.ConstraintEqualTo(viewdelete.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

        }
        public int Stock
        {
            get 
            { 
                if (StockImg.Image == null)
                {
                    return 0;
                }
                else if (StockImg.Image.ToString() == "IndicatorYellow")
                {
                    return 1;
                }
                else
                {
                    return 2;
                }

            }
            set
            {
                if (value == 0) // normal not show
                {
                    StockImg.Image = null;
                    StockImg.Hidden = true;
                }
                else if (value == 1) // = stock min
                {
                    StockImg.Hidden = false;
                    StockImg.Image = UIImage.FromBundle("IndicatorYellow");
                }
                else // under stock min
                {
                    StockImg.Hidden = false;
                    StockImg.Image = UIImage.FromBundle("IndicatorRed");
                }
            }

        }
        public string Name
        {
            get { return lblname.Text; }
            set
            {
                var frame = this.Frame;
                frame.X = 0;
                this.Frame = frame;
                lblname.Text = value;
            }
        }
        public string Cost
        {
            get { return lblCost.Text;  }
            set { lblCost.Text = value + DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS; }
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
                
                if (!string.IsNullOrEmpty(value))
                {
                    //Utils.SetConstant(lblname.Constraints, NSLayoutAttribute.CenterY, -30);
                    lblCategory.Hidden = false;         
                    lblCategory.Text = value;
                }
                else
                {
                  //  Utils.SetConstant(lblname.Constraints, NSLayoutAttribute.CenterY, 0);
                    lblCategory.Hidden = true;
                }
            }
        }
        public int Fav
        {
            get
            {
                if (imagefav.Image != null)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value==0)
                {
                    imagefav.Image = UIImage.FromBundle("Unfav");
                }
                else
                {
                    imagefav.Image = UIImage.FromBundle("Fav");
                }
            }
        }
        public Item Imageghavenet
        {
            //get { return imageView.Image.ToString(); }
            set
            {
                if (value!=null)
                {
                    if (string.IsNullOrEmpty( value.ThumbnailLocalPath))
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
        public Item Imagegnothavenet
        {
            //get { return imageView.Image.ToString(); }
            set
            {
                if (value != null)
                {
                    Utils.SetImageItemlocal(imageView, value);
                    //if (string.IsNullOrEmpty(value.ThumbnailLocalPath))
                    //{
                    //    Utils.SetImageItem(imageView, value);

                    //}
                    //else
                    //{
                    //    Utils.SetImageItemlocal(imageView, value);
                    //}
                }
                else
                {
                    imageView.Image = null;
                }
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
       
        public delegate void itemSelectedDelegate(CellItemList indexPath);
        public event itemSelectedDelegate OnItemSwipe;

        public delegate void itemdeleteDelegate(CellItemList indexPath);
        public event itemdeleteDelegate OnDeleteItem;

        public delegate void itemFavDelegate(CellItemList indexPath);
        public event itemFavDelegate OnFavItem;

        public delegate void itemDelegate(CellItemList indexPath);
        public event itemDelegate OnItem;
    }

    }