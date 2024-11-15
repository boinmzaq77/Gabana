using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class StockCollectionViewCell : UICollectionViewCell
    {
        UILabel  lblbalance, lblby ,lbldate,lbltype ;
        UICollectionView BillHisByDateCollection;
        ReceiptHistoryDetailController HistoryPage;
        UIView upperView;
        int tall = 0;
        UIImageView Imageitem; 
        SubBillDateDataSource BillDataList;
        TranWithDetailsLocal TranHis;

        public StockCollectionViewCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.White;
            initAttribte();
            setupView();
        }
         void initAttribte()
        {
            #region upperView
            upperView = new UIView();
            upperView.BackgroundColor = UIColor.FromRGB(248,248,248);
            upperView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(upperView);

            Imageitem = new UIImageView();
            Imageitem.Image = UIImage.FromBundle("Next");
            Imageitem.TranslatesAutoresizingMaskIntoConstraints = false;
            upperView.AddSubview(Imageitem);

            lbltype = new UILabel();
            lbltype.Font = lbltype.Font.WithSize(15);
            lbltype.TextColor = UIColor.FromRGB(64,64,64);
            lbltype.TextAlignment = UITextAlignment.Left;
            lbltype.Text = "Day , DD/MM/YYYY";
            lbltype.TranslatesAutoresizingMaskIntoConstraints = false;
            upperView.AddSubview(lbltype);

            lblby = new UILabel();
            lblby.Font = lblby.Font.WithSize(15);
            lblby.TextColor = UIColor.FromRGB(162, 162, 162);
            lblby.TextAlignment = UITextAlignment.Left;
            lblby.Text = "Day , DD/MM/YYYY";
            
            lblby.TranslatesAutoresizingMaskIntoConstraints = false;
            upperView.AddSubview(lblby);

            lbldate = new UILabel();
            lbldate.Font = lbldate.Font.WithSize(15);
            lbldate.TextColor = UIColor.FromRGB(162, 162, 162);
            lbldate.TextAlignment = UITextAlignment.Left;
            lbldate.Text = "Day , DD/MM/YYYY";
            lbldate.TranslatesAutoresizingMaskIntoConstraints = false;
            upperView.AddSubview(lbldate);

            lblbalance = new UILabel();
            lblbalance.Font = lblbalance.Font.WithSize(15);
            lblbalance.TextColor = UIColor.FromRGB(64, 64, 64);
            lblbalance.TextAlignment = UITextAlignment.Right;
            lblbalance.Text = "Day , DD/MM/YYYY";
            lblbalance.TranslatesAutoresizingMaskIntoConstraints = false;
            upperView.AddSubview(lblbalance);
            #endregion

        }
        void setupView()
        {
            #region upperView
            upperView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;
            upperView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            upperView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            upperView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            upperView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            Imageitem.CenterYAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            Imageitem.LeftAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            Imageitem.WidthAnchor.ConstraintEqualTo(28).Active = true;
            Imageitem.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lbltype.BottomAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.CenterYAnchor,-2).Active = true;
            lbltype.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lbltype.LeftAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;

            

            lblbalance.BottomAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.CenterYAnchor, -2).Active = true;
            lblbalance.HeightAnchor.ConstraintEqualTo(16).Active = true;
            //lblbalance.LeftAnchor.ConstraintEqualTo(lblby.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lblbalance.RightAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lbldate.TopAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.CenterYAnchor, 2).Active = true;
            lbldate.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lbldate.LeftAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbldate.RightAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;


            lblby.BottomAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.CenterYAnchor, -2).Active = true;
            lblby.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblby.LeftAnchor.ConstraintEqualTo(lbltype.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lblby.RightAnchor.ConstraintEqualTo(lblbalance.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
            #endregion

        }
        public string Type
        {
            get { return lbltype.Text; }
            set { lbltype.Text = value; }
        }

        public string By
        {
            get { return lblby.Text; }
            set { lblby.Text = value; }
        }

        public string Balance
        {
            get { return lblbalance.Text; }
            set { lblbalance.Text = value; }
        }

        public string Date
        {
            get { return lbldate.Text; }
            set { lbldate.Text = value; }
        }

        public string Image
        {
            get { return Imageitem.Image.ToString(); }
            set
            {
                if (value == "StockDecrease")
                {
                    lblbalance.TextColor = UIColor.FromRGB(162, 162, 162);
                }
                else
                {
                    lblbalance.TextColor = UIColor.FromRGB(0, 149, 218);
                }
                Imageitem.Image = UIImage.FromBundle(value);
            }
        }

    }
}