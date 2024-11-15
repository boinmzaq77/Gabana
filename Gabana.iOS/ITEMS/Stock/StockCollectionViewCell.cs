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

            Imageitem = new UIImageView();
            Imageitem.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(Imageitem);

            lbltype = new UILabel();
            lbltype.Font = lbltype.Font.WithSize(14);
            lbltype.TextColor = UIColor.FromRGB(64,64,64);
            lbltype.TextAlignment = UITextAlignment.Left;
            lbltype.Text = "Day , DD/MM/YYYY";
            lbltype.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(lbltype);

            lblby = new UILabel();
            lblby.Font = lblby.Font.WithSize(14);
            lblby.TextColor = UIColor.FromRGB(162, 162, 162);
            lblby.TextAlignment = UITextAlignment.Left;
            lblby.Text = "Day , DD/MM/YYYY";
            
            lblby.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(lblby);

            lbldate = new UILabel();
            lbldate.Font = lbldate.Font.WithSize(14);
            lbldate.TextColor = UIColor.FromRGB(162, 162, 162);
            lbldate.TextAlignment = UITextAlignment.Left;
            lbldate.Text = "Day , DD/MM/YYYY";
            lbldate.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(lbldate);

            lblbalance = new UILabel();
            lblbalance.Font = lblbalance.Font.WithSize(14);
            lblbalance.TextColor = UIColor.FromRGB(64, 64, 64);
            lblbalance.TextAlignment = UITextAlignment.Right;
            lblbalance.Text = "Day , DD/MM/YYYY";
            lblbalance.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(lblbalance);
            #endregion

        }
        void setupView()
        {
            #region upperView

            Imageitem.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            Imageitem.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            Imageitem.WidthAnchor.ConstraintEqualTo(28).Active = true;
            Imageitem.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lbltype.BottomAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.CenterYAnchor,-2).Active = true;
            lbltype.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lbltype.LeftAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;

            

            lblbalance.BottomAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.CenterYAnchor, -2).Active = true;
            lblbalance.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lblbalance.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lbldate.TopAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.CenterYAnchor, 2).Active = true;
            lbldate.HeightAnchor.ConstraintEqualTo(16).Active = true;
            lbldate.LeftAnchor.ConstraintEqualTo(Imageitem.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lbldate.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;


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
                    Imageitem.Image = UIImage.FromFile("StockDecrease.png");
                }
                else
                {
                    Imageitem.Image = UIImage.FromFile("StockIncrease.png");
                    lblbalance.TextColor = UIColor.FromRGB(0, 149, 218);
                }
                Imageitem.Image = UIImage.FromBundle(value);
            }
        }

    }
}