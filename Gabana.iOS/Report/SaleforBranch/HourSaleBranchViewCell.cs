using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;

namespace Gabana.iOS
{
    public class HourSaleBranchViewCell : UICollectionViewCell
    {
        UILabel lblname,lblSaleTotal;

        public HourSaleBranchViewCell(IntPtr handle) : base(handle)
        {
           

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false , TextAlignment = UITextAlignment.Left };
            lblname.TextColor = UIColor.FromRGB(64, 64, 64);
            lblname.Font = lblname.Font.WithSize(15);
            ContentView.AddSubview(lblname);

            lblSaleTotal= new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false,TextAlignment = UITextAlignment.Right };
            lblSaleTotal.TextColor = UIColor.FromRGB(0,149,218);
            lblSaleTotal.Font = lblSaleTotal.Font.WithSize(15);
            ContentView.AddSubview(lblSaleTotal);


            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
      
        private void setupListView()
        {

            lblname.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblname.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(16).Active = true;

            lblSaleTotal.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblSaleTotal.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblSaleTotal.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblSaleTotal.HeightAnchor.ConstraintEqualTo(18).Active = true;

        }
        public string Total
        {
            get { return lblSaleTotal.Text; }
            set
            {
                lblSaleTotal.Text = value;
            }
        }

        public string Time
        {
            get { return lblname.Text; }
            set
            {
                lblname.Text = value;
            }
        }
    }

    }