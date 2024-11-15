using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class BillHistoryDetailCollectionViewCell : UICollectionViewCell
    {
        UILabel lblText, lblPrice, lblamount, lblX;
        public BillHistoryDetailCollectionViewCell(IntPtr handle) : base(handle)
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

            ContentView.BackgroundColor = UIColor.White;
            setupListView();

        }
        private void setupListView()
        {
            lblamount.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblamount.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
           // lblamount.WidthAnchor.ConstraintEqualTo(5).Active = true;

            lblX.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblX.LeftAnchor.ConstraintEqualTo(lblamount.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            lblX.WidthAnchor.ConstraintEqualTo(7).Active = true;

            lblText.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblText.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 65).Active = true;
            lblText.RightAnchor.ConstraintEqualTo(lblPrice.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;

            lblPrice.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblPrice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblPrice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor,-0).Active = true;
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


    }
}