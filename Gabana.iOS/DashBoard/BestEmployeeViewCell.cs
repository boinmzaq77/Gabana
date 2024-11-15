using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class BestEmployeeViewCell : UICollectionViewCell
    {
        UILabel lblname;
        UIImageView settingImg,IconImage;
        private UILabel lblprice;
        private UIProgressView loading;

        public BestEmployeeViewCell(IntPtr handle) : base(handle)
        {
            

            lblname = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblname.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
            lblname.Font = lblname.Font.WithSize(15);
            ContentView.AddSubview(lblname);


            lblprice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblprice.TextColor = new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1f);
            lblprice.Font = lblprice.Font.WithSize(15);
            lblprice.TextAlignment = UITextAlignment.Right;
            ContentView.AddSubview(lblprice);

            loading = new UIProgressView();
            loading.TranslatesAutoresizingMaskIntoConstraints = false;
            loading.ProgressTintColor = UIColor.FromRGB(0, 149, 218);

            ContentView.AddSubview(loading);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {
            loading.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 15).Active = true;
            loading.HeightAnchor.ConstraintEqualTo(10).Active = true;
            loading.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            loading.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;


            lblname.TopAnchor.ConstraintEqualTo(loading.SafeAreaLayoutGuide.BottomAnchor , 5).Active = true;
            lblname.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblname.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblname.RightAnchor.ConstraintEqualTo(lblprice.SafeAreaLayoutGuide.LeftAnchor, -20).Active = true;

            lblprice.TopAnchor.ConstraintEqualTo(loading.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            lblprice.HeightAnchor.ConstraintEqualTo(20).Active = true;
            lblprice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            lblprice.WidthAnchor.ConstraintEqualTo(100).Active = true;
            //lblname.WidthAnchor.ConstraintEqualTo(150).Active = true;

        }
        public string Name
        {
            get { return lblname.Text; }
            set { lblname.Text = value; }
        }

        public string price
        {
            get { return lblprice.Text; }
            set { lblprice.Text = value; }
        }
        public float persen
        {
            get { return loading.Progress; }
            set { loading.Progress = value; }
        }

        public int color
        {
            get { return 1; }
            set
            {
                switch (value)
                {
                    case 0:
                        loading.ProgressTintColor = UIColor.FromRGB(0, 149, 218);
                        break;
                    case 1:
                        loading.ProgressTintColor = UIColor.FromRGB(51, 171, 224);
                        break;
                    case 2:
                        loading.ProgressTintColor = UIColor.FromRGB(102, 191, 233);
                        break;
                    case 3:
                        loading.ProgressTintColor = UIColor.FromRGB(153, 213, 241);
                        break;
                    case 4:
                        loading.ProgressTintColor = UIColor.FromRGB(205, 234, 248);
                        break;
                    default:
                        loading.ProgressTintColor = UIColor.FromRGB(0, 149, 218);
                        break;

                }; }
        }

    }
}