using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class BillListViewCell : UICollectionViewCell
    {
        UILabel lblBillNo,lblTotal,lblTime,lblCustomer;
        UIImageView IcomImg;
        UIView line;
        UIView upperView;
        private UILabel lblVoid;

        public BillListViewCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.White;
            initAttribute();
            setupView();
            
        }
        void initAttribute()
        {
            upperView = new UIView();
            upperView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            upperView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(upperView);

            IcomImg = new UIImageView();
            IcomImg.TranslatesAutoresizingMaskIntoConstraints = false;
            upperView.AddSubview(IcomImg);

            lblBillNo = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblBillNo.TextColor = UIColor.FromRGB(64,64,64);
            lblBillNo.Font = lblBillNo.Font.WithSize(15);
            lblBillNo.TextAlignment = UITextAlignment.Left;
            upperView.AddSubview(lblBillNo);

            lblTotal = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblTotal.TextColor = UIColor.FromRGB(0, 149, 218);
            lblTotal.Font = lblTotal.Font.WithSize(15);
            lblTotal.TextAlignment = UITextAlignment.Right;
            upperView.AddSubview(lblTotal);

            lblVoid = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            
            lblVoid.Font = lblTotal.Font.WithSize(15);
            lblVoid.TextAlignment = UITextAlignment.Right;
            upperView.AddSubview(lblVoid);

            lblTime = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblTime.TextColor = UIColor.FromRGB(172,172,172);
            lblTime.Font = lblTime.Font.WithSize(15);
            lblTime.TextAlignment = UITextAlignment.Left;
            upperView.AddSubview(lblTime);

            lblCustomer = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCustomer.TextColor = UIColor.FromRGB(172, 172, 172);
            lblCustomer.Font = lblCustomer.Font.WithSize(15);
            lblCustomer.TextAlignment = UITextAlignment.Left;
            upperView.AddSubview(lblCustomer);

            line = new UIView();
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            upperView.AddSubview(line);
        }
        void setupView()
        {
            upperView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            upperView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            upperView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            upperView.HeightAnchor.ConstraintEqualTo(70).Active = true;
            upperView.WidthAnchor.ConstraintEqualTo(30).Active = true;
            upperView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            IcomImg.TopAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.TopAnchor,5).Active = true;
            IcomImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            IcomImg.LeftAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            IcomImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBillNo.TopAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.TopAnchor,15).Active = true;
            lblBillNo.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblBillNo.LeftAnchor.ConstraintEqualTo(IcomImg.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblBillNo.WidthAnchor.ConstraintEqualTo(200).Active = true;

            lblTotal.TopAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.TopAnchor, 15).Active = true;
            lblTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblTotal.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblTotal.WidthAnchor.ConstraintEqualTo(100).Active = true;



            lblTime.TopAnchor.ConstraintEqualTo(IcomImg.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblTime.LeftAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblTime.WidthAnchor.ConstraintGreaterThanOrEqualTo(80).Active = true;

            lblCustomer.TopAnchor.ConstraintEqualTo(IcomImg.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblCustomer.LeftAnchor.ConstraintEqualTo(lblTime.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblCustomer.WidthAnchor.ConstraintEqualTo(200).Active = true;

            lblVoid.TopAnchor.ConstraintEqualTo(IcomImg.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblVoid.RightAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblVoid.WidthAnchor.ConstraintEqualTo(80).Active = true;

            line.BottomAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        public string Image
        {
            get { return IcomImg.Image.ToString(); }
            set
            {
                IcomImg.Image = UIImage.FromBundle(value);
            }
        }
        public string BillNo
        {
            get { return lblBillNo.Text; }
            set { lblBillNo.Text = value; }
        }
        public int sizeWidth
        {
            get { return 0; }
            set
            {
                Utils.SetConstant(upperView.Constraints, NSLayoutAttribute.Width, value);
                //BillDataList = new SubBillDateDataSource(TranHis); // ส่ง list ไป
                //BillHisByDateCollection.DataSource = BillDataList;
            }
        }
        public string Total
        {
            get { return lblTotal.Text; }
            set { lblTotal.Text = value; }
        }
        public string Time
        {
            get { return lblTime.Text; }
            set { lblTime.Text = value; }
        }
        public string CustomerName
        {
            get { return lblCustomer.Text; }
            set { lblCustomer.Text = value; }
        }

        public decimal Voidbill
        {
            get { return 0; }
            set
            {
                if (value == 1 )
                {
                    lblVoid.Text = "Void";
                    lblVoid.TextColor = UIColor.Red;
                    var secondAttributes = new UIStringAttributes
                    {
                        StrikethroughStyle = NSUnderlineStyle.Single
                    };

                    lblTotal.AttributedText = new NSAttributedString(lblTotal.Text, secondAttributes);
                }
                else if (value == 2)
                {
                    lblVoid.Text = "Pending";
                    lblVoid.TextColor = UIColor.FromRGB(64, 64, 64);
                    var secondAttributes = new UIStringAttributes
                    {
                        StrikethroughStyle = NSUnderlineStyle.None
                    };

                    lblTotal.AttributedText = new NSAttributedString(lblTotal.Text, secondAttributes);
                }
                else
                {
                    lblVoid.Text = "";
                    lblVoid.TextColor = UIColor.Red;
                    var secondAttributes = new UIStringAttributes
                    {
                        StrikethroughStyle = NSUnderlineStyle.None
                    };

                    lblTotal.AttributedText = new NSAttributedString(lblTotal.Text, secondAttributes);

                };
            }
        }

    }
}