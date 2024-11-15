using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class ChangeCollectionViewCell : UICollectionViewCell
    {
        UIImageView paymentimage;
        UILabel lblprice , lbltypepayment , lbldate  ;
        UIView line;

        public ChangeCollectionViewCell(IntPtr handle) : base(handle)
        {
            paymentimage = new UIImageView();
            paymentimage.Image = UIImage.FromBundle("PaymentCash");
            paymentimage.ContentMode = UIViewContentMode.ScaleAspectFill;
            
            paymentimage.BackgroundColor = UIColor.White;
            paymentimage.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(paymentimage);

            lblprice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblprice.TextColor = UIColor.FromRGB(0, 149, 218);
            lblprice.TextAlignment = UITextAlignment.Right;
            lblprice.Font = lblprice.Font.WithSize(15);
            ContentView.AddSubview(lblprice);

            lbltypepayment = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbltypepayment.TextColor = UIColor.FromRGB(64, 64, 64);
            lbltypepayment.TextAlignment = UITextAlignment.Left;
            lbltypepayment.Font = lbltypepayment.Font.WithSize(15);
            ContentView.AddSubview(lbltypepayment);

            ContentView.BackgroundColor = UIColor.White;
            setupView();
            
        }
        private void setupView()
        {

            paymentimage.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            paymentimage.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            paymentimage.HeightAnchor.ConstraintEqualTo(28).Active = true;
            paymentimage.WidthAnchor.ConstraintEqualTo(28).Active = true;
            //paymentimage.BackgroundColor = UIColor.Red;

            lblprice.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblprice.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblprice.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblprice.WidthAnchor.ConstraintEqualTo(100).Active = true;


            lbltypepayment.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbltypepayment.RightAnchor.ConstraintEqualTo(lblprice.LeftAnchor, -15).Active = true;
            lbltypepayment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbltypepayment.LeftAnchor.ConstraintEqualTo(paymentimage.RightAnchor ,15 ).Active = true;



        }
        public string Name
        {
            get { return lbltypepayment.Text; }
            set { lbltypepayment.Text = value; }
        }
        public string price
        {
            get { return lblprice.Text; }
            set { lblprice.Text = value; }
        }
        
        public string Image
        {
            get { return paymentimage.Image.ToString(); }
            set
            {
                paymentimage.Image = UIImage.FromFile(value);
                //Utils.SetImageURL(ProfileImg, value);
            }
        }
      

    }
}