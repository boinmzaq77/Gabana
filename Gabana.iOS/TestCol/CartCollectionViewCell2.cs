using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using CoreGraphics;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class CartCollectionViewCell2 : UICollectionViewCell
    {
        UILabel lblText, lblPrice, lblPriceperamount, lblamount , lblcomment , lblnewprice , lblsizename;
        //UIView line;
        UIImageView imgchangeprice ,imgdiscount; 
        UICollectionViewFlowLayout itemflowLayoutList;
        UICollectionView ToppingCollection;
        List<string> x;
        ToppingCartDataSource BranchDataList;
        UIStackView line;
        UIView btn1, btn2, btn3, btn4, btn5;
        UILabel lblbtn1, lblbtn2, lblbtn3, lblbtn4, lblbtn5;
        UIImageView imgbtn1, imgbtn2, imgbtn3, imgbtn4, imgbtn5;
        private UILabel lbldiscount;
        private UIView btndelete;
        private UILabel lblbtndelete;

        public CartCollectionViewCell2(IntPtr handle) : base(handle)
        {
            //ContentView.HeightAnchor.ConstraintEqualTo(500).Active = true ;
            ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.BackgroundColor = UIColor.White;
            lblText = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblText.TextColor = UIColor.FromRGB(64, 64, 64);
            
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

            lblnewprice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblnewprice.TextColor = UIColor.FromRGB(138, 221, 245); 
            lblnewprice.TextAlignment = UITextAlignment.Left;
            lblnewprice.BackgroundColor = UIColor.White;
            lblnewprice.Font = lblText.Font.WithSize(16);
            //lblnewprice.Text = "999999999";
            ContentView.AddSubview(lblnewprice);
            
            //lblsizename = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            //lblsizename.TextColor = UIColor.FromRGB(0, 149, 218);
            //lblsizename.TextAlignment = UITextAlignment.Left;
            //lblsizename.BackgroundColor = UIColor.White;
            //lblsizename.Font = lblText.Font.WithSize(16);
            //lblsizename.Text = "999999999";
            //ContentView.AddSubview(lblsizename);

            lblamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblamount.TextColor = UIColor.Black;
            lblamount.TextAlignment = UITextAlignment.Right;
            lblamount.Font = lblText.Font.WithSize(16);
            lblamount.BackgroundColor = UIColor.White;
            ContentView.AddSubview(lblamount);

            imgdiscount = new UIImageView();
            imgdiscount.TranslatesAutoresizingMaskIntoConstraints = false;
            imgdiscount.Image = UIImage.FromBundle("DiscountItem");
            ContentView.AddSubview(imgdiscount);

            lblPriceperamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPriceperamount.TextColor = UIColor.FromRGB(138, 221, 245);
            lblPriceperamount.TextAlignment = UITextAlignment.Left;
            lblPriceperamount.Font = lblText.Font.WithSize(16);
            lblPriceperamount.BackgroundColor = UIColor.White;
            ContentView.AddSubview(lblPriceperamount);

            //line = new UIView();
            //line.TranslatesAutoresizingMaskIntoConstraints = false;
            //line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            //ContentView.AddSubview(line);
            lbldiscount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbldiscount.TextColor = UIColor.FromRGB(200, 200, 200);
            lbldiscount.TextAlignment = UITextAlignment.Left;
            lbldiscount.BackgroundColor = UIColor.White;
            lbldiscount.Font = lblText.Font.WithSize(16);
            lbldiscount.Lines = 5;
            //lblcomment.Text = "44445555555555555555555555555555555555555555555555555555555555555555555555555555231321546465 \n 5555";
            ContentView.AddSubview(lbldiscount);

            ContentView.BackgroundColor = UIColor.White;
            
            itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (ContentView.Frame.Width), height: 20);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            // itemflowLayoutList.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
            ToppingCollection = new UICollectionView(frame: ContentView.Frame, layout: itemflowLayoutList);
            ToppingCollection.BackgroundColor = UIColor.White;
            ToppingCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            ToppingCollection.RegisterClassForCell(cellType: typeof(ToppingCartCollectionViewCell), reuseIdentifier: "ToppingCartCollectionViewCell");
            //ToppingCollection.BackgroundColor = UIColor.Red;
            ToppingCollection.ScrollEnabled = false;
            //x = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g" };
            //BranchDataList = new ToppingCartDataSource(x); // ส่ง list ไป
            //ToppingCollection.DataSource = BranchDataList;
            CustomerCollectionDelegate CustomerCollectionDelegate = new CustomerCollectionDelegate();
            ContentView.AddSubview(ToppingCollection);

            lblcomment = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblcomment.TextColor = UIColor.FromRGB(200, 200, 200);
            lblcomment.TextAlignment = UITextAlignment.Left;
            lblcomment.BackgroundColor = UIColor.White;
            lblcomment.Font = lblText.Font.WithSize(16);
            lblcomment.Lines = 5;
            //lblcomment.Text = "44445555555555555555555555555555555555555555555555555555555555555555555555555555231321546465 \n 5555";
            ContentView.AddSubview(lblcomment);

            btndelete = new UIView();
            //btndelete.ContentMode = UIViewContentMode.ScaleAspectFit;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.BackgroundColor = UIColor.FromRGB(232, 232, 232);
            btndelete.Layer.CornerRadius = 10;
            ContentView.AddSubview(btndelete);

            btndelete.UserInteractionEnabled = true;
            var tapGesture6 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("btn4:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btndelete.AddGestureRecognizer(tapGesture6);

            lblbtndelete = new UILabel();
            lblbtndelete.Text = "";
            lblbtndelete.TextColor = UIColor.FromRGB(0, 149, 218);
            lblbtndelete.Font = lblText.Font.WithSize(14);
            lblbtndelete.TextAlignment = UITextAlignment.Center;
            lblbtndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.AddSubview(lblbtndelete);

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

            imgbtn3 = new UIImageView();
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
            var tapGesture5 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("btn4:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btn5.AddGestureRecognizer(tapGesture5);


            ContentView.BackgroundColor = UIColor.White;
            //setupListView();
            //var swipeRight = new UISwipeGestureRecognizer((s) => { swiped(s); });
            //swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            //this.AddGestureRecognizer(swipeRight);

            setupListView();


            //itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (ToppingCollection.Frame.Width), height: 20);
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            
        }
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
        public delegate void CardCellDelegate0(CartCollectionViewCell2 merchantCollectionViewCell);
        public event CardCellDelegate0 OnCardCellQRCodeBtn0;

        public delegate void CardCellDelegate1(CartCollectionViewCell2 merchantCollectionViewCell);
        public event CardCellDelegate1 OnCardCellQRCodeBtn1;

        public delegate void CardCellDelegate2(CartCollectionViewCell2 merchantCollectionViewCell);
        public event CardCellDelegate2 OnCardCellQRCodeBtn2;

        public delegate void CardCellDelegate3(CartCollectionViewCell2 merchantCollectionViewCell);
        public event CardCellDelegate3 OnCardCellQRCodeBtn3;

        public delegate void CardCellDelegate4(CartCollectionViewCell2 merchantCollectionViewCell);
        public event CardCellDelegate4 OnCardCellQRCodeBtn4;
        private void setupListView()
        {
            



            lblamount.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            lblamount.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor,0).Active = true;
            lblamount.WidthAnchor.ConstraintEqualTo(50).Active = true;
            lblamount.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblamount.SetContentHuggingPriority(1000, UILayoutConstraintAxis.Horizontal);

            lblText.TopAnchor.ConstraintEqualTo(lblamount.TopAnchor).Active = true;
            lblText.LeftAnchor.ConstraintEqualTo(lblamount.RightAnchor, 5).Active = true;
            lblText.RightAnchor.ConstraintEqualTo(lblPrice.LeftAnchor, -5).Active = true;
            lblText.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblText.SetContentHuggingPriority(999, UILayoutConstraintAxis.Horizontal);
            //lblText.BackgroundColor = UIColor.Blue;

            lblPrice.TopAnchor.ConstraintEqualTo(lblamount.TopAnchor).Active = true;
            lblPrice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblPrice.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor, -10).Active = true;
            lblPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblPrice.BackgroundColor = UIColor.Blue;
            //lblPrice.SetContentHuggingPriority(1000, UILayoutConstraintAxis.Horizontal);

            //lblsizename.TopAnchor.ConstraintEqualTo(lblamount.TopAnchor).Active = true;
            //lblsizename.LeftAnchor.ConstraintEqualTo(lblText.RightAnchor, 5).Active = true;
            //lblsizename.RightAnchor.ConstraintEqualTo(lblPrice.LeftAnchor, -5).Active = true;
            //lblsizename.HeightAnchor.ConstraintEqualTo(18).Active = true;
            ////lblsizename.WidthAnchor.ConstraintEqualTo(50).Active = true;
            //lblsizename.SetContentHuggingPriority(499, UILayoutConstraintAxis.Horizontal);
            //lblsizename.BackgroundColor = UIColor.Red;

            

            

            lblPriceperamount.TopAnchor.ConstraintEqualTo(lblamount.BottomAnchor, 5).Active = true;
            lblPriceperamount.LeftAnchor.ConstraintEqualTo(lblText.LeftAnchor).Active = true; 
            //lblPriceperamount.WidthAnchor.ConstraintEqualTo(lblText.WidthAnchor).Active = true;
            lblPriceperamount.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblPriceperamount.SetContentHuggingPriority(1000, UILayoutConstraintAxis.Horizontal);
           // lblPriceperamount.BackgroundColor = UIColor.Yellow;

            lblnewprice.TopAnchor.ConstraintEqualTo(lblamount.BottomAnchor, 5).Active = true;
            lblnewprice.LeftAnchor.ConstraintEqualTo(lblPriceperamount.RightAnchor, 5).Active = true;
            lblnewprice.RightAnchor.ConstraintEqualTo(imgdiscount.LeftAnchor,-5).Active = true;
            lblnewprice.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblnewprice.SetContentHuggingPriority(500, UILayoutConstraintAxis.Horizontal);

            lbldiscount.TopAnchor.ConstraintEqualTo(lblnewprice.BottomAnchor, 0).Active = true;
            lbldiscount.LeftAnchor.ConstraintEqualTo(lblPriceperamount.LeftAnchor).Active = true;
            lbldiscount.RightAnchor.ConstraintEqualTo(imgdiscount.LeftAnchor , - 5).Active = true;

            imgdiscount.TopAnchor.ConstraintEqualTo(lbldiscount.TopAnchor).Active = true;
            imgdiscount.WidthAnchor.ConstraintEqualTo(20).Active = true;
            imgdiscount.RightAnchor.ConstraintEqualTo(lblPrice.RightAnchor).Active = true;
            imgdiscount.HeightAnchor.ConstraintEqualTo(20).Active = true;
            //lblnewprice.BackgroundColor = UIColor.Red;
            //lblPriceperamount.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

            //line.TopAnchor.ConstraintEqualTo(lblPriceperamount.BottomAnchor, 5).Active = true;
            //line.LeftAnchor.ConstraintEqualTo(lblText.LeftAnchor).Active = true;
            //line.WidthAnchor.ConstraintEqualTo(lblText.WidthAnchor).Active = true;
            ////line.HeightAnchor.ConstraintEqualTo(0).Active = true;
            //line.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

            ToppingCollection.TopAnchor.ConstraintEqualTo(lbldiscount.BottomAnchor, 5).Active = true;
            ToppingCollection.LeftAnchor.ConstraintEqualTo(lblPriceperamount.LeftAnchor).Active = true;
            ToppingCollection.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
            ToppingCollection.HeightAnchor.ConstraintEqualTo(50).Active = true;
            //ToppingCollection.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -5).Active = true;

            lblcomment.TopAnchor.ConstraintEqualTo(ToppingCollection.BottomAnchor, 5).Active = true;
            lblcomment.LeftAnchor.ConstraintEqualTo(lblPriceperamount.LeftAnchor).Active = true;
            lblcomment.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;

            btndelete.BottomAnchor.ConstraintEqualTo(lblcomment.BottomAnchor).Active = true;
            btndelete.WidthAnchor.ConstraintEqualTo(70).Active = true;
            btndelete.HeightAnchor.ConstraintEqualTo(0).Active = true;
            btndelete.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor,-10).Active = true;

            lblbtndelete.CenterXAnchor.ConstraintEqualTo(btndelete.CenterXAnchor).Active = true;
            lblbtndelete.CenterYAnchor.ConstraintEqualTo(btndelete.CenterYAnchor).Active = true;
            //lblbtn1.LeftAnchor.ConstraintEqualTo(btn1.LeftAnchor).Active = true;
            //lblbtn1.RightAnchor.ConstraintEqualTo(btn1.RightAnchor).Active = true;

            //lblcomment.HeightAnchor.ConstraintEqualTo(0).Active = true;
            //lblcomment.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, -5).Active = true;

            line.TopAnchor.ConstraintEqualTo(lblcomment.BottomAnchor, 5).Active = true;
            line.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(50).Active = true;
            line.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor, 0).Active = true;

            imgbtn1.TopAnchor.ConstraintEqualTo(btn1.TopAnchor, 5).Active = true;
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
        public List<TranDetailItemTopping> toppinglist
        {
            
            set
            {
                //x = value;
                BranchDataList = new ToppingCartDataSource(value);
                ToppingCollection.DataSource = BranchDataList;


            }
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            return base.SizeThatFits(size);
        }
        public string Name
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }
        public bool Dummy
        {
           
            set {
                if (value)
                {
                    btn2.Alpha = 0.5f;
                    lblbtn2.Alpha = 0.5f;
                }
                else
                {
                    btn2.Alpha = 1;
                    lblbtn2.Alpha = 1;
                }
                }
        }

        public nfloat Height
        {
            get { return 252; }
            set { Utils.SetConstant(ToppingCollection.Constraints, NSLayoutAttribute.Height, (int)value); } 
        }
        //public bool imgchangepriceset
        //{

        //    set
        //    {
        //        if (value)
        //        {
        //            Utils.SetConstant(imgchangeprice.Constraints, NSLayoutAttribute.Width, 20);
        //        }
        //        else
        //        { 
        //            Utils.SetConstant(imgchangeprice.Constraints, NSLayoutAttribute.Width, 20);
        //        }
                
        //    }
        //}
        public bool imgdiscountset
        {

            set
            {
                if (value)
                {
                    Utils.SetConstant(imgdiscount.Constraints, NSLayoutAttribute.Width, 20);
                    //Utils.SetConstant(lbldiscount.Constraints, NSLayoutAttribute.Height, 20);
                    lbldiscount.TopAnchor.ConstraintEqualTo(lblnewprice.BottomAnchor, 5).Active = true;
                }
                else
                {

                    Utils.SetConstant(imgdiscount.Constraints, NSLayoutAttribute.Width, 0);
                    //Utils.SetConstant(lbldiscount.Constraints, NSLayoutAttribute.Height, 0);
                    //lbldiscount.TopAnchor.ConstraintEqualTo(lblnewprice.BottomAnchor, 0).Active = true;
                }
            }
        }
        public nfloat size
        {
            set { lblText.WidthAnchor.ConstraintEqualTo(value-165).Active=true;
                UICollectionViewFlowLayout itemViewLayout = ToppingCollection.CollectionViewLayout as UICollectionViewFlowLayout;
                itemViewLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemViewLayout.ItemSize = new CoreGraphics.CGSize(value-50,20);
                itemViewLayout.MinimumLineSpacing = 5;
                itemViewLayout.MinimumInteritemSpacing = 3;
            }
        }
        public string price
        {
            get { return lblPrice.Text; }
            set { lblPrice.Text = value; }
        }
        public string comment
        {
            get { return lblcomment.Text; }
            set { lblcomment.Text = value;
                if (!string.IsNullOrEmpty(value) && value=="delete")
                {
                    lblbtndelete.Text = "ลบสินค้า";
                    Utils.SetConstant(btndelete.Constraints, NSLayoutAttribute.Height, 30);
                }
                else
                {
                    lblbtndelete.Text = "";
                    Utils.SetConstant(btndelete.Constraints, NSLayoutAttribute.Height, 0);
                }
            }
        }

        public string discount
        {
            get { return lbldiscount.Text; }
            set { lbldiscount.Text = value; }
        }
        public string priceperamount
        {
            get { return lblPriceperamount.Text; }
            set { lblPriceperamount.Text = value; }
        }
        public string pricenew
        {
            get { return lblnewprice.Text; }
            set {
                if (value != "")
                {
                    var attrString = new NSAttributedString(lblPriceperamount.Text,
                    new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.Single });
                    lblPriceperamount.AttributedText = attrString;
                    lblPriceperamount.TextColor = UIColor.FromRGB(200, 200, 200);
                    lblnewprice.Text = value;
                }
                else
                {
                    var attrString = new NSAttributedString(lblPriceperamount.Text,
                    new UIStringAttributes { StrikethroughStyle = NSUnderlineStyle.None });
                    lblPriceperamount.AttributedText = attrString;
                    lblPriceperamount.TextColor = UIColor.FromRGB(138, 221, 245);
                    lblnewprice.Text = "";
                }
                
            
            }
        }
        public string amount
        {
            get { return lblamount.Text; }
            set { lblamount.Text = value; }
        }

        public bool showhead
        {
            set
            {
                if (value)
                {
                    UIView.Animate(0.7, () =>
                    {
                        Utils.SetConstant(line.Constraints, NSLayoutAttribute.Height, 50);
                        line.Hidden = false;
                    });
                }
                else
                {
                    UIView.Animate(0.7, () =>
                    {
                        Utils.SetConstant(line.Constraints, NSLayoutAttribute.Height, 0);
                        line.Hidden = true;
                    });
                }
                
                
                //line.HeightAnchor.ConstraintEqualTo(value).Active = true;
            }
        }
        //public string size
        //{
        //    get { return lblText.Text; }
        //    set { lblText.WidthAnchor.ConstraintEqualTo(nfloat.Parse(value).Active = true; }
        //}


    }
}