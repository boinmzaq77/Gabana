using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class ReceiptCollectionViewCell : UICollectionViewCell
    {
        UILabel lblText, lblPrice, lblamount, lblX, lblcomment;
        UICollectionViewFlowLayout itemflowLayoutList;
        ToppingReceiptDataSource BranchDataList;
        UICollectionView ToppingCollection;
        private UILabel lbldiscount;

        public ReceiptCollectionViewCell(IntPtr handle) : base(handle)
        {
            
            lblText = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblText.TextColor = UIColor.FromRGB(64,64,64);
            lblText.TextAlignment = UITextAlignment.Left;
            lblText.Font = lblText.Font.WithSize(16);
            ContentView.AddSubview(lblText);
            
            lblPrice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblPrice.TextColor = UIColor.FromRGB(64, 64, 64);
            lblPrice.TextAlignment = UITextAlignment.Right;
            lblPrice.Font = lblPrice.Font.WithSize(16);
            ContentView.AddSubview(lblPrice);

            lblamount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblamount.TextColor = UIColor.FromRGB(64, 64, 64);
            lblamount.TextAlignment = UITextAlignment.Left;
         //   lblamount.BackgroundColor = UIColor.Yellow;
            lblamount.Font = lblamount.Font.WithSize(16);
            ContentView.AddSubview(lblamount);

            lblX = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblX.TextColor = UIColor.FromRGB(162,162,162);
            lblX.TextAlignment = UITextAlignment.Left;
            lblX.Font = lblX.Font.WithSize(16);
            lblX.Text = "x";
            ContentView.AddSubview(lblX);

            itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (ContentView.Frame.Width), height: 20);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayoutList.MinimumInteritemSpacing = 0;
            itemflowLayoutList.MinimumLineSpacing = 0;
            // itemflowLayoutList.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
            ToppingCollection = new UICollectionView(frame: ContentView.Frame, layout: itemflowLayoutList);
            ToppingCollection.BackgroundColor = UIColor.White;
            ToppingCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            ToppingCollection.RegisterClassForCell(cellType: typeof(ToppingReceiptCollectionViewCell), reuseIdentifier: "ToppingReceiptCollectionViewCell");
            //ToppingCollection.BackgroundColor = UIColor.Red;
            ToppingCollection.ScrollEnabled = false;
            //x = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g" };
            //BranchDataList = new ToppingCartDataSource(x); // ส่ง list ไป
            //ToppingCollection.DataSource = BranchDataList;
            CustomerCollectionDelegate CustomerCollectionDelegate = new CustomerCollectionDelegate();
            ContentView.AddSubview(ToppingCollection);

            lbldiscount = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbldiscount.TextColor = UIColor.FromRGB(200, 200, 200);
            lbldiscount.TextAlignment = UITextAlignment.Left;
            lbldiscount.BackgroundColor = UIColor.White;
            lbldiscount.Font = lbldiscount.Font.WithSize(16);
            lbldiscount.Lines = 5;
            //lblcomment.Text = "44445555555555555555555555555555555555555555555555555555555555555555555555555555231321546465 \n 5555";
            ContentView.AddSubview(lbldiscount);

            lblcomment = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblcomment.TextColor = UIColor.FromRGB(200, 200, 200);
            lblcomment.TextAlignment = UITextAlignment.Left;
            lblcomment.BackgroundColor = UIColor.White;
            lblcomment.Font = lblText.Font.WithSize(16);
            lblcomment.Lines = 5;
            //lblcomment.Text = "44445555555555555555555555555555555555555555555555555555555555555555555555555555231321546465 \n 5555";
            ContentView.AddSubview(lblcomment);

            //ContentView.BackgroundColor = UIColor.Green;
            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
        private void setupListView()
        {
            //ContentView.BackgroundColor = UIColor.Red;
            lblamount.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            //lblamount.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblamount.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
           // lblamount.WidthAnchor.ConstraintEqualTo(5).Active = true;

            lblX.CenterYAnchor.ConstraintEqualTo(lblamount.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblX.LeftAnchor.ConstraintEqualTo(lblamount.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            lblX.WidthAnchor.ConstraintEqualTo(7).Active = true;

            lblText.CenterYAnchor.ConstraintEqualTo(lblamount.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblText.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 45).Active = true;
            lblText.RightAnchor.ConstraintEqualTo(lblPrice.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            //lblText.WidthAnchor.ConstraintEqualTo(lblText.Superview.WidthAnchor).Active = true;

            lblPrice.CenterYAnchor.ConstraintEqualTo(lblamount.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblPrice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblPrice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-0).Active = true;
            //lblPrice.BackgroundColor = UIColor.Green;

            ToppingCollection.TopAnchor.ConstraintEqualTo(lblamount.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            ToppingCollection.LeftAnchor.ConstraintEqualTo(lblText.LeftAnchor).Active = true;
            ToppingCollection.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
            ToppingCollection.HeightAnchor.ConstraintEqualTo(50).Active = true;

            lbldiscount.TopAnchor.ConstraintEqualTo(ToppingCollection.BottomAnchor, 0).Active = true;
            lbldiscount.LeftAnchor.ConstraintEqualTo(lblText.LeftAnchor).Active = true;
            lbldiscount.RightAnchor.ConstraintEqualTo(lblPrice.RightAnchor).Active = true;
            lbldiscount.HeightAnchor.ConstraintEqualTo(20).Active = true;
            

            lblcomment.TopAnchor.ConstraintEqualTo(lbldiscount.BottomAnchor, 0).Active = true;
            lblcomment.LeftAnchor.ConstraintEqualTo(lblText.LeftAnchor).Active = true;
            lblcomment.RightAnchor.ConstraintEqualTo(lblPrice.RightAnchor).Active = true;
            lblcomment.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblcomment.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor,-5).Active = true;

            //lblcomment.BackgroundColor = UIColor.Black;

        }
        public List<TranDetailItemTopping> toppinglist
        {
            
            set
            {
                //x = value;
                BranchDataList = new ToppingReceiptDataSource(value);
                ToppingCollection.DataSource = BranchDataList;
                if (value.Count == 0)
                {
                    ToppingCollection.TopAnchor.ConstraintEqualTo(lblamount.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                }
            }
        }
        public bool hiddentopping
        {

            set
            {
                //x = value;
                if (value)
                {
                    
                }


            }
        }
        public nfloat size
        {
            set
            {
                lblText.WidthAnchor.ConstraintEqualTo(value - 225).Active = true;
                UICollectionViewFlowLayout itemViewLayout = ToppingCollection.CollectionViewLayout as UICollectionViewFlowLayout;
                itemViewLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
                itemViewLayout.ItemSize = new CoreGraphics.CGSize(value - 100, 20);
                itemViewLayout.MinimumLineSpacing = 5;
                itemViewLayout.MinimumInteritemSpacing = 3;
            }
        }
        public string Name
        {
            get { return lblText.Text; }
            set { lblText.Text = value; }
        }
        public string price
        {
            get { return lblPrice.Text; }
            set { lblPrice.Text = value; }
        }
        public string amount
        {
            get { return lblamount.Text; }
            set { lblamount.Text = value;
                lblamount.WidthAnchor.ConstraintEqualTo(10*value.Length).Active = true;
            }
        }
        public nfloat Height
        {
            get { return 252; }
            set { Utils.SetConstant(ToppingCollection.Constraints, NSLayoutAttribute.Height, (int)value); }
        }
        public string comment
        {
            get { return lblcomment.Text; }
            set { lblcomment.Text = value;
                if (string.IsNullOrEmpty(value))
                {
                    lblcomment.TopAnchor.ConstraintEqualTo(lbldiscount.BottomAnchor, 0).Active = true;
                    Utils.SetConstant(lblcomment.Constraints, NSLayoutAttribute.Height, 0);
                }
                else
                {
                    lblcomment.TopAnchor.ConstraintEqualTo(lbldiscount.BottomAnchor, 5).Active = true;
                    Utils.SetConstant(lblcomment.Constraints, NSLayoutAttribute.Height, 20);
                }
               
            }
        }

        public string discount
        {
            get { return lbldiscount.Text; }
            set
            {
                lbldiscount.Text = value;
                if (string.IsNullOrEmpty(value))
                {
                    lbldiscount.TopAnchor.ConstraintEqualTo(ToppingCollection.BottomAnchor, 0).Active = true;
                    Utils.SetConstant(lbldiscount.Constraints, NSLayoutAttribute.Height, 0);
                }
                else
                {
                    lbldiscount.TopAnchor.ConstraintEqualTo(ToppingCollection.BottomAnchor, 5).Active = true;
                    Utils.SetConstant(lbldiscount.Constraints, NSLayoutAttribute.Height, 20);
                }

            }
        }

    }
}