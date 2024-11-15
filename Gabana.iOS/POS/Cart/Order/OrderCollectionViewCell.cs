using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace Gabana.iOS
{
    public class OrderCollectionViewCell : UICollectionViewCell
    {
        UIImageView ProfileImg;
        UILabel  lbldevice , lbldate , lblprice ;
        UIView line;
        private UILabel lblcomment;
        UILabel lblordername;
        private UIView nameView;
        UIView upperView;
        private UIView lineView;

        public OrderCollectionViewCell(IntPtr handle) : base(handle)
        {
            upperView = new UIView();
            upperView.BackgroundColor = UIColor.Clear;
            upperView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(upperView);

            nameView = new UIStackView();
            //nameView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            nameView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(nameView);

            lblordername = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblordername.TextColor = UIColor.FromRGB(64,64,64);
            lblordername.TextAlignment = UITextAlignment.Left;
            lblordername.Font = lblordername.Font.WithSize(15);
            nameView.AddSubview(lblordername);

            lbldevice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbldevice.TextColor = UIColor.FromRGB(112, 112, 112) ;
            lbldevice.TextAlignment = UITextAlignment.Left;
            lbldevice.Font = lbldevice.Font.WithSize(15);
            ContentView.AddSubview(lbldevice);

            lbldate = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbldate.TextColor = UIColor.FromRGB(172, 172, 172);
            lbldate.TextAlignment = UITextAlignment.Right;
            lbldate.Font = lbldate.Font.WithSize(15);
            ContentView.AddSubview(lbldate);

            lblprice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblprice.TextColor = UIColor.FromRGB(0, 149, 218);
            lblprice.TextAlignment = UITextAlignment.Right;
            lblprice.Font = lblprice.Font.WithSize(15);
            ContentView.AddSubview(lblprice);

            lblcomment = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblcomment.TextColor = UIColor.FromRGB(172, 172, 172);
            lblcomment.TextAlignment = UITextAlignment.Left ;
            lblcomment.Font = lblcomment.Font.WithSize(15);
            
            nameView.AddSubview(lblcomment);

            ContentView.BackgroundColor = UIColor.White; 
            lineView = new UIView();
            lineView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(lineView);
            setupView();

            
        }
        private void setupView()
        {
            upperView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            upperView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            upperView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            upperView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            upperView.WidthAnchor.ConstraintEqualTo(30).Active = true;
            upperView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            lbldate.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 8).Active = true;
            lbldate.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -8).Active = true;
            lbldate.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lbldate.WidthAnchor.ConstraintEqualTo(85).Active = true;
            //lbldate.BackgroundColor = UIColor.Red;

            nameView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 8).Active = true;
            nameView.RightAnchor.ConstraintEqualTo(lbldate.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;
            nameView.HeightAnchor.ConstraintEqualTo(20).Active = true;
            nameView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            //nameView.BackgroundColor = UIColor.Red;

            lblordername.TopAnchor.ConstraintEqualTo(nameView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            lblordername.LeftAnchor.ConstraintEqualTo(nameView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            //lblordername.RightAnchor.ConstraintEqualTo(lblcomment.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            lblordername.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblordername.WidthAnchor.ConstraintGreaterThanOrEqualTo(10).Active = true;

            lblcomment.TopAnchor.ConstraintEqualTo(nameView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            lblcomment.LeftAnchor.ConstraintEqualTo(lblordername.SafeAreaLayoutGuide.RightAnchor, 5).Active = true;
            lblcomment.RightAnchor.ConstraintEqualTo(nameView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            lblcomment.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblcomment.WidthAnchor.ConstraintGreaterThanOrEqualTo(50).Active = true;




            //lblordername.WidthAnchor.ConstraintLessThanOrEqualTo(500).Active = true;

            lblcomment.SetContentHuggingPriority(999, UILayoutConstraintAxis.Horizontal);
            lblordername.SetContentHuggingPriority(1000, UILayoutConstraintAxis.Horizontal);

            //lblcomment.WidthAnchor.ConstraintLessThanOrEqualTo(500).Active = true;

            lblprice.TopAnchor.ConstraintEqualTo(lbldate.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblprice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -8).Active = true;
            lblprice.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblprice.WidthAnchor.ConstraintEqualTo(120).Active = true;

            lbldevice.TopAnchor.ConstraintEqualTo(lbldate.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lbldevice.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lbldevice.RightAnchor.ConstraintEqualTo(lblprice.SafeAreaLayoutGuide.LeftAnchor, -20).Active = true;
            lbldevice.HeightAnchor.ConstraintEqualTo(20).Active = true;

            lineView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            lineView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor).Active = true;
            lineView.HeightAnchor.ConstraintEqualTo(1).Active = true;
            lineView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
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
        public string Name
        {
            get { return lblordername.Text; }
            set { lblordername.Text = value; }
        }
        public string price
        {
            get { return lblprice.Text; }
            set { lblprice.Text = value; }
        }
        public string date
        {
            get { return lbldate.Text; }
            set { lbldate.Text = value; }
        }
        public string device
        {
            get { return lbldevice.Text; }
            set { lbldevice.Text = value; }
        }
        public string comment
        {
            get { return lblcomment.Text; }
            set { lblcomment.Text = value; }
        }

    }
}