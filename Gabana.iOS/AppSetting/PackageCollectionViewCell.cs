using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class PackageCollectionViewCell : UICollectionViewCell
    {
        UILabel lblmenu,lblCost,lblShortName;
        UIImageView iconImg,addImg;
        UIView detailView;
        private UILabel lblnamepack;
        private UIButton btnColor1;
        private UILabel lblbranch;
        private UIButton btnColor2;
        private UILabel lbluser;
        private UILabel lblprice;
        private UILabel lblmonth;
        private UIButton btnchoose;
        private UIImageView logoImg;

        public PackageCollectionViewCell(IntPtr handle) : base(handle)
        {
            ContentView.Layer.CornerRadius = 5;


            lblnamepack = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblnamepack.TextColor = UIColor.Black;
            lblnamepack.TextAlignment = UITextAlignment.Left;
            lblnamepack.Font = lblnamepack.Font.WithSize(15);
            ContentView.AddSubview(lblnamepack);

            btnColor1 = new UIButton();
            btnColor1.BackgroundColor = UIColor.FromRGB(0, 121, 107);
            btnColor1.Layer.CornerRadius = 3;
            btnColor1.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(btnColor1);

            lblbranch = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblbranch.TextColor = UIColor.Black;
            lblbranch.TextAlignment = UITextAlignment.Left;
            lblbranch.Font = lblbranch.Font.WithSize(12);
            ContentView.AddSubview(lblbranch);

            btnColor2 = new UIButton();
            btnColor2.BackgroundColor = UIColor.FromRGB(0, 121, 107);
            btnColor2.Layer.CornerRadius = 3;
            btnColor2.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(btnColor2);

            lbluser = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lbluser.TextColor = UIColor.Black;
            lbluser.TextAlignment = UITextAlignment.Left;
            lbluser.Font = lbluser.Font.WithSize(12);
            ContentView.AddSubview(lbluser);

            



            detailView = new UIView();
            
            detailView.BackgroundColor = UIColor.FromRGB(232, 232, 232);
            detailView.Layer.Opacity = 1f;
            detailView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(detailView);

            lblprice = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblprice.TextColor = UIColor.FromRGB(0, 149, 218);
            lblprice.Text = "/mounth";
            lblprice.Font = lblprice.Font.WithSize(17);
            detailView.AddSubview(lblprice);

            lblmonth = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblmonth.TextColor = UIColor.Black;
            lblmonth.Text = "/mounth";
            lblmonth.Font = lblmonth.Font.WithSize(12);
            detailView.AddSubview(lblmonth);

            btnchoose = new UIButton();
            btnchoose.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnchoose.Layer.CornerRadius =  10f;
            btnchoose.SetTitle("เลือก", UIControlState.Normal);
            btnchoose.TranslatesAutoresizingMaskIntoConstraints = false;
            detailView.AddSubview(btnchoose);

            logoImg = new UIImageView();
            // logoImg.Image = UIImage.FromFile("GabanaLogIn.png");
            logoImg.Image = UIImage.FromBundle("PackagePopular");
            logoImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(logoImg);

            ContentView.BackgroundColor = UIColor.White;
            SetupGridView();
         //   ContentView.BringSubviewToFront(detailView);

        }
        void SetupGridView()
        {
            logoImg.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor).Active = true;
            logoImg.HeightAnchor.ConstraintEqualTo(60).Active = true;
            logoImg.WidthAnchor.ConstraintEqualTo(60).Active = true;
            logoImg.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor).Active = true;

            lblnamepack.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 5).Active = true;
            lblnamepack.HeightAnchor.ConstraintEqualTo(23).Active = true;
            lblnamepack.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            lblnamepack.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            btnColor1.TopAnchor.ConstraintEqualTo(lblnamepack.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor1.HeightAnchor.ConstraintEqualTo(10).Active = true;
            btnColor1.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnColor1.WidthAnchor.ConstraintEqualTo(10).Active = true;

            lblbranch.CenterYAnchor.ConstraintEqualTo(btnColor1.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblbranch.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblbranch.LeftAnchor.ConstraintEqualTo(btnColor1.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lblbranch.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            btnColor2.TopAnchor.ConstraintEqualTo(btnColor1.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor2.HeightAnchor.ConstraintEqualTo(10).Active = true;
            btnColor2.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnColor2.WidthAnchor.ConstraintEqualTo(10).Active = true;

            lbluser.CenterYAnchor.ConstraintEqualTo(btnColor2.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lbluser.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lbluser.LeftAnchor.ConstraintEqualTo(btnColor2.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lbluser.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            detailView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            detailView.HeightAnchor.ConstraintEqualTo(50).Active = true;
            detailView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            detailView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor).Active = true;

            btnchoose.CenterYAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnchoose.HeightAnchor.ConstraintEqualTo(30).Active = true;
            btnchoose.WidthAnchor.ConstraintEqualTo(50).Active = true;
            btnchoose.RightAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            lblprice.TopAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.TopAnchor, 2).Active = true;
            lblprice.HeightAnchor.ConstraintEqualTo(23).Active = true;
            lblprice.LeftAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            lblprice.RightAnchor.ConstraintEqualTo(btnchoose.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;

            lblmonth.TopAnchor.ConstraintEqualTo(lblprice.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblmonth.HeightAnchor.ConstraintEqualTo(23).Active = true;
            lblmonth.LeftAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            lblmonth.RightAnchor.ConstraintEqualTo(btnchoose.SafeAreaLayoutGuide.LeftAnchor, -5).Active = true;

        }
        public string Name
        {
            get { return lblnamepack.Text; }
            set { lblnamepack.Text = value; }
        }
        public string bracnh
        {
            get { return lblbranch.Text; }
            set { lblbranch.Text = value; }
        }
        public string user
        {
            get { return lbluser.Text; }
            set { lbluser.Text = value; }
        }

        public string price
        {
            get { return lblprice.Text; }
            set { lblprice.Text = value; }
        }

        public bool use
        {
            
            set {

                if (value)
                {
                    logoImg.Hidden = false;
                }
                else
                {
                    logoImg.Hidden = true;
                }
                
                }
        }
        public UIImage getimage ()
        {
            var r = new UIGraphicsImageRenderer(this.Bounds.Size);
            return  r.CreateImage((UIGraphicsImageRendererContext ctxt) =>
            {
                this.Layer.RenderInContext(ctxt.CGContext);
                //View.Capture(true);
            });
            
        }
        public string Image
        {
            get { return iconImg.Image.ToString(); }
            set
            {
                if (value == "AddItem")
                {
                    addImg.Hidden = false;
                    iconImg.Hidden = true;
                    Utils.SetConstant(detailView.Constraints, NSLayoutAttribute.Height, 0);
                    //detailView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                    //detailView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                    Utils.SetImage(addImg, value);
                }
                else if (value != null)
                {
                    addImg.Hidden = true;
                    iconImg.Hidden = false;
                    Utils.SetConstant(detailView.Constraints, NSLayoutAttribute.Height, 40);
                    //Utils.SetConstant(iconImg.Constraints,NSLayoutAttribute.Bottom, 0);
                    //detailView.TopAnchor.ConstraintEqualTo(iconImg.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                    //detailView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
                    Utils.SetImage(iconImg, value);
                    
                }
                else
                {
                    addImg.Hidden = true;
                    iconImg.Hidden = false;
                    Utils.SetConstant(detailView.Constraints, NSLayoutAttribute.Height, 40);
                    Utils.SetImage(iconImg, value);
                }
            }
        }
        public long Colors
        {
            get { return Colors; }
            set { Utils.SetColor(ContentView, value); }
        }
    }
}