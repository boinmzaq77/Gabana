using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using CoreGraphics;

namespace Gabana.iOS
{
    public class CartCollectionViewCell3 : UICollectionViewCell
    {
        UILabel lblText, lblPrice, lblPriceperamount, lblamount;
        
        UIImageView imgbtn1, imgbtn2, imgbtn3, imgbtn4, imgbtn5;
        UIView btn1, btn2, btn3, btn4, btn5;
        UILabel lblbtn1, lblbtn2, lblbtn3, lblbtn4, lblbtn5;
        UIStackView line;
        public CartCollectionViewCell3(IntPtr handle) : base(handle)
        {
            //ContentView.HeightAnchor.ConstraintEqualTo(500).Active = true ;
            ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.BackgroundColor = UIColor.White;
            lblText = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblText.TextColor = UIColor.FromRGB(64,64,64);
            lblText.TextAlignment = UITextAlignment.Left;
            lblText.Font = lblText.Font.WithSize(16);
            lblText.BackgroundColor = UIColor.White;
            ContentView.AddSubview(lblText);
             
            lblPrice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPrice.TextColor = UIColor.FromRGB(0, 149, 218);
            lblPrice.TextAlignment = UITextAlignment.Right;
            lblPrice.BackgroundColor = UIColor.White;
            lblPrice.Font = lblText.Font.WithSize(16);
            ContentView.AddSubview(lblPrice);

            lblamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblamount.TextColor = UIColor.Black;
            lblamount.TextAlignment = UITextAlignment.Right;
            lblamount.Font = lblText.Font.WithSize(16);
            lblamount.BackgroundColor = UIColor.White;
            ContentView.AddSubview(lblamount);

            lblPriceperamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPriceperamount.TextColor = UIColor.FromRGB(138, 221, 245);
            lblPriceperamount.TextAlignment = UITextAlignment.Left;
            lblPriceperamount.Font = lblText.Font.WithSize(16);
            lblPriceperamount.BackgroundColor = UIColor.White;
            ContentView.AddSubview(lblPriceperamount);

            line = new UIStackView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            line.Alignment = UIStackViewAlignment.Fill;
            line.Distribution = UIStackViewDistribution.FillEqually;
            line.Spacing = 8.0f;
            ContentView.AddSubview(line);

            //var icon = new UIImageView(new UIImage("icon.png"));
            //icon.ContentMode = UIViewContentMode.ScaleAspectFit;
            //line.AddArrangedSubview(icon);

            btn1 = new UIView() 
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions

            };
            btn1.ContentMode = UIViewContentMode.ScaleAspectFit;
            btn1.TranslatesAutoresizingMaskIntoConstraints = false;
            btn1.BackgroundColor = UIColor.Clear;
            line.AddArrangedSubview(btn1);

            imgbtn1 = new UIImageView();
            imgbtn1.TranslatesAutoresizingMaskIntoConstraints = false;
            imgbtn1.Image = UIImage.FromBundle("Quantity");
            btn1.AddSubview(imgbtn1);

            lblbtn1 = new UILabel();
            lblbtn1.Text = "Quantity";
            lblbtn1.TextColor = UIColor.FromRGB(0, 149, 218);
            lblbtn1.Font = lblText.Font.WithSize(11);
            lblbtn1.TextAlignment = UITextAlignment.Center;
            lblbtn1.TranslatesAutoresizingMaskIntoConstraints = false;
            btn1.AddSubview(lblbtn1);

            btn2 = new UIView()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions

            };
            btn2.ContentMode = UIViewContentMode.ScaleAspectFit;
            btn2.TranslatesAutoresizingMaskIntoConstraints = false;
            btn2.BackgroundColor = UIColor.Clear;
            line.AddArrangedSubview(btn2);

            imgbtn2 = new UIImageView();
            imgbtn2.TranslatesAutoresizingMaskIntoConstraints = false;
            imgbtn2.Image = UIImage.FromBundle("NoteTopping");
            btn2.AddSubview(imgbtn2);

            lblbtn2 = new UILabel();
            lblbtn2.Text = "Option";
            lblbtn2.TextColor = UIColor.FromRGB(0, 149, 218);
            lblbtn2.Font = lblText.Font.WithSize(11);
            lblbtn2.TextAlignment = UITextAlignment.Center;
            lblbtn2.TranslatesAutoresizingMaskIntoConstraints = false;
            btn2.AddSubview(lblbtn2);

            btn3 = new UIView()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions

            };
            btn3.ContentMode = UIViewContentMode.ScaleAspectFit;
            btn3.TranslatesAutoresizingMaskIntoConstraints = false;
            btn3.BackgroundColor = UIColor.Clear;
            line.AddArrangedSubview(btn3);

            imgbtn3   = new UIImageView();
            imgbtn3.TranslatesAutoresizingMaskIntoConstraints = false;
            imgbtn3.Image = UIImage.FromBundle("Price");
            btn3.AddSubview(imgbtn3);

            lblbtn3 = new UILabel();
            lblbtn3.Text = "Price";
            lblbtn3.TextColor = UIColor.FromRGB(0, 149, 218);
            lblbtn3.Font = lblText.Font.WithSize(11);
            lblbtn3.TextAlignment = UITextAlignment.Center;
            lblbtn3.TranslatesAutoresizingMaskIntoConstraints = false;
            btn3.AddSubview(lblbtn3);

            btn4 = new UIView()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions
            };
            btn4.ContentMode = UIViewContentMode.ScaleAspectFit;
            btn4.TranslatesAutoresizingMaskIntoConstraints = false;
            btn4.BackgroundColor = UIColor.Clear;
            line.AddArrangedSubview(btn4);

            imgbtn4 = new UIImageView();
            imgbtn4.TranslatesAutoresizingMaskIntoConstraints = false;
            imgbtn4.Image = UIImage.FromBundle("DiscountItem");
            btn4.AddSubview(imgbtn4);

            lblbtn4 = new UILabel();
            lblbtn4.Text = "Discount";
            lblbtn4.TextColor = UIColor.FromRGB(0, 149, 218);
            lblbtn4.Font = lblText.Font.WithSize(11);
            lblbtn4.TextAlignment = UITextAlignment.Center;
            lblbtn4.TranslatesAutoresizingMaskIntoConstraints = false;
            btn4.AddSubview(lblbtn4);

            btn5 = new UIView()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions

            };
            btn5.ContentMode = UIViewContentMode.ScaleAspectFit;
            btn5.TranslatesAutoresizingMaskIntoConstraints = false;
            btn5.BackgroundColor = UIColor.Clear;
            line.AddArrangedSubview(btn5);

            imgbtn5 = new UIImageView();
            imgbtn5.TranslatesAutoresizingMaskIntoConstraints = false;
            imgbtn5.Image = UIImage.FromBundle("Trash");
            btn5.AddSubview(imgbtn5);

            lblbtn5 = new UILabel();
            lblbtn5.Text = "Delete";
            lblbtn5.TextColor = UIColor.FromRGB(0, 149, 218);
            lblbtn5.Font = lblText.Font.WithSize(11);
            lblbtn5.TextAlignment = UITextAlignment.Center;
            lblbtn5.TranslatesAutoresizingMaskIntoConstraints = false;
            btn5.AddSubview(lblbtn5);


            btn1.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("btn0:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btn1.AddGestureRecognizer(tapGesture1);

            btn2.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("btn1:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btn2.AddGestureRecognizer(tapGesture2);

            btn3.UserInteractionEnabled = true;
            var tapGesture3 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("btn2:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btn3.AddGestureRecognizer(tapGesture3);

            btn4.UserInteractionEnabled = true;
            var tapGesture4 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("btn3:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btn4.AddGestureRecognizer(tapGesture4);

            btn5.UserInteractionEnabled = true;
            var tapGesture5= new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("btn4:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btn5.AddGestureRecognizer(tapGesture5);


            ContentView.BackgroundColor = UIColor.White;
            setupListView();
            var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            this.AddGestureRecognizer(swipeRight);
        }
        private void swiped(UISwipeGestureRecognizer s)
        {

            if (s.Direction == UISwipeGestureRecognizerDirection.Left)
            {
                
            }
        }
        private void setupListView()
        {
            
            lblamount.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblamount.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            lblamount.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblamount.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblText.TopAnchor.ConstraintEqualTo(lblamount.TopAnchor).Active = true;
            lblText.LeftAnchor.ConstraintEqualTo(lblamount.RightAnchor, 5).Active = true;
            //lblText.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblText.RightAnchor.ConstraintEqualTo(lblPrice.LeftAnchor, -5).Active = true;
            lblText.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblPrice.TopAnchor.ConstraintEqualTo(lblamount.TopAnchor).Active = true;
            lblPrice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblPrice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;
            lblPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblPriceperamount.TopAnchor.ConstraintEqualTo(lblamount.BottomAnchor, 5).Active = true;
            lblPriceperamount.LeftAnchor.ConstraintEqualTo(lblText.LeftAnchor).Active = true; 
            lblPriceperamount.WidthAnchor.ConstraintEqualTo(lblText.WidthAnchor).Active = true;
            lblPriceperamount.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblPriceperamount.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

            line.TopAnchor.ConstraintEqualTo(lblPriceperamount.BottomAnchor, 5).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor,0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0).Active = true;
            line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;

            imgbtn1.TopAnchor.ConstraintEqualTo(btn1.TopAnchor ,5).Active = true;
            imgbtn1.HeightAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn1.WidthAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn1.CenterXAnchor.ConstraintEqualTo(btn1.CenterXAnchor).Active = true;

            lblbtn1.TopAnchor.ConstraintEqualTo(imgbtn1.BottomAnchor).Active = true;
            lblbtn1.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblbtn1.LeftAnchor.ConstraintEqualTo(btn1.LeftAnchor).Active = true;
            lblbtn1.RightAnchor.ConstraintEqualTo(btn1.RightAnchor).Active = true;

            imgbtn2.TopAnchor.ConstraintEqualTo(btn2.TopAnchor, 5).Active = true;
            imgbtn2.HeightAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn2.WidthAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn2.CenterXAnchor.ConstraintEqualTo(btn2.CenterXAnchor).Active = true;

            lblbtn2.TopAnchor.ConstraintEqualTo(imgbtn2.BottomAnchor).Active = true;
            lblbtn2.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblbtn2.LeftAnchor.ConstraintEqualTo(btn2.LeftAnchor).Active = true;
            lblbtn2.RightAnchor.ConstraintEqualTo(btn2.RightAnchor).Active = true;

            imgbtn3.TopAnchor.ConstraintEqualTo(btn3.TopAnchor, 5).Active = true;
            imgbtn3.HeightAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn3.WidthAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn3.CenterXAnchor.ConstraintEqualTo(btn3.CenterXAnchor).Active = true;

            lblbtn3.TopAnchor.ConstraintEqualTo(imgbtn3.BottomAnchor).Active = true;
            lblbtn3.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblbtn3.LeftAnchor.ConstraintEqualTo(btn3.LeftAnchor).Active = true;
            lblbtn3.RightAnchor.ConstraintEqualTo(btn3.RightAnchor).Active = true;

            imgbtn4.TopAnchor.ConstraintEqualTo(btn4.TopAnchor, 5).Active = true;
            imgbtn4.HeightAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn4.WidthAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn4.CenterXAnchor.ConstraintEqualTo(btn4.CenterXAnchor).Active = true;

            lblbtn4.TopAnchor.ConstraintEqualTo(imgbtn4.BottomAnchor).Active = true;
            lblbtn4.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblbtn4.LeftAnchor.ConstraintEqualTo(btn4.LeftAnchor).Active = true;
            lblbtn4.RightAnchor.ConstraintEqualTo(btn4.RightAnchor).Active = true;

            imgbtn5.TopAnchor.ConstraintEqualTo(btn5.TopAnchor, 5).Active = true;
            imgbtn5.HeightAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn5.WidthAnchor.ConstraintEqualTo(30).Active = true;
            imgbtn5.CenterXAnchor.ConstraintEqualTo(btn5.CenterXAnchor).Active = true;

            lblbtn5.TopAnchor.ConstraintEqualTo(imgbtn5.BottomAnchor).Active = true;
            lblbtn5.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblbtn5.LeftAnchor.ConstraintEqualTo(btn5.LeftAnchor).Active = true;
            lblbtn5.RightAnchor.ConstraintEqualTo(btn5.RightAnchor).Active = true;

        }


        public string Name
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }
        public nfloat size
        {
            set { lblText.WidthAnchor.ConstraintEqualTo(value-170).Active=true; }
        }
        public string price
        {
            get { return lblPrice.Text; }
            set { lblPrice.Text = value; }
        }
        public string priceperamount
        {
            get { return lblPriceperamount.Text; }
            set { lblPriceperamount.Text = value; }
        }
        public string amount
        {
            get { return lblamount.Text; }
            set { lblamount.Text = value; }
        }

        //public bool showhead
        //{
        //    set
        //    {
        //        nfloat x = 0;

        //        if (value)
        //        {
        //            x = 200;
        //        }
        //        line.HeightAnchor.ConstraintEqualTo(x).Active = true;

        //    }
        //}
        //public string size
        //{
        //    get { return lblText.Text; }
        //    set { lblText.WidthAnchor.ConstraintEqualTo(nfloat.Parse(value).Active = true; }
        //}
        #region Events
        public delegate void CardCellDelegate0(CartCollectionViewCell3  merchantCollectionViewCell);
        public event CardCellDelegate0 OnCardCellQRCodeBtn0;

        public delegate void CardCellDelegate1(CartCollectionViewCell3 merchantCollectionViewCell);
        public event CardCellDelegate1 OnCardCellQRCodeBtn1;

        public delegate void CardCellDelegate2(CartCollectionViewCell3 merchantCollectionViewCell);
        public event CardCellDelegate2 OnCardCellQRCodeBtn2;

        public delegate void CardCellDelegate3(CartCollectionViewCell3 merchantCollectionViewCell);
        public event CardCellDelegate3 OnCardCellQRCodeBtn3;

        public delegate void CardCellDelegate4(CartCollectionViewCell3 merchantCollectionViewCell);
        public event CardCellDelegate4 OnCardCellQRCodeBtn4;

        #endregion

        [Export("btn0:")]
        public void btn1Click(UIGestureRecognizer sender)
        {
            OnCardCellQRCodeBtn0(this);
        }
        [Export("btn1:")]
        public void btn2Click(UIGestureRecognizer sender)
        {
            OnCardCellQRCodeBtn1(this);
        }

        [Export("btn2:")]
        public void btn3Click(UIGestureRecognizer sender)
        {
            OnCardCellQRCodeBtn2(this);
        }

        [Export("btn3:")]
        public void btn4Click(UIGestureRecognizer sender)
        {
            OnCardCellQRCodeBtn3(this);
        }

        [Export("btn4:")]
        public void btn5Click(UIGestureRecognizer sender)
        {
            OnCardCellQRCodeBtn4(this);
        }

    }
}