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
    public class ItemPOSCollectionViewCell : UICollectionViewCell
    {
        UILabel lblmenu,lblCost,lblShortName;
        UIImageView iconImg,addImg;
        UIView detailView;
        public ItemPOSCollectionViewCell(IntPtr handle) : base(handle)
        {
            ContentView.Layer.CornerRadius = 5;
            

            lblShortName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblShortName.TextColor = UIColor.White;
            lblShortName.TextAlignment = UITextAlignment.Center;
            lblShortName.Font = lblShortName.Font.WithSize(25);
            ContentView.AddSubview(lblShortName);

            

            detailView = new UIView();
            detailView.Layer.CornerRadius = 5;
            detailView.BackgroundColor = UIColor.Black;
            detailView.Layer.Opacity = 0.2f;
            detailView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(detailView);

            lblmenu = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblmenu.TextColor = UIColor.White;
            lblmenu.Font = lblmenu.Font.WithSize(12);
            ContentView.AddSubview(lblmenu);

            lblCost = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
            lblCost.TextColor = UIColor.White;
            lblCost.Font = lblCost.Font.WithSize(12);
            ContentView.AddSubview(lblCost);

            iconImg = new UIImageView() { TranslatesAutoresizingMaskIntoConstraints = false };
            iconImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            
            ContentView.AddSubview(iconImg);

            addImg = new UIImageView() { TranslatesAutoresizingMaskIntoConstraints = false };
            addImg.Hidden = true;
            ContentView.AddSubview(addImg);

            ContentView.BackgroundColor = UIColor.White;
            SetupGridView();
         //   ContentView.BringSubviewToFront(detailView);

        }
        void SetupGridView()
        {
            

            addImg.CenterXAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterXAnchor, 0).Active = true;
            addImg.HeightAnchor.ConstraintEqualTo(48).Active = true;
            addImg.WidthAnchor.ConstraintEqualTo(48).Active = true;
            addImg.CenterYAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;

            detailView.TopAnchor.ConstraintEqualTo(iconImg.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            detailView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            detailView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            var x = ContentView.Frame.Height * (nfloat)0.33;
            detailView.HeightAnchor.ConstraintEqualTo(ContentView.Frame.Height*(nfloat)0.33).Active = true; 
            detailView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblShortName.HeightAnchor.ConstraintEqualTo(30).Active = true;
            lblShortName.CenterYAnchor.ConstraintEqualTo(iconImg.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortName.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblShortName.CenterXAnchor.ConstraintEqualTo(iconImg.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lblmenu.TopAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.TopAnchor, 4).Active = true;
            lblmenu.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblmenu.LeftAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            lblmenu.RightAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;

            lblCost.TopAnchor.ConstraintEqualTo(lblmenu.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            lblCost.LeftAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            lblCost.HeightAnchor.ConstraintEqualTo(15).Active = true;
            lblCost.RightAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.RightAnchor, -5).Active = true;
            lblCost.BottomAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.BottomAnchor, -5).Active = true;

            iconImg.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            iconImg.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            iconImg.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -0).Active = true;
            iconImg.CenterXAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            iconImg.BottomAnchor.ConstraintEqualTo(detailView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
        }
        public string Name
        {
            get { return lblmenu.Text; }
            set { lblmenu.Text = value; }
        }
        public string Cost
        {
            get { return lblCost.Text; }
            set { lblCost.Text = value; }
        }
        public string ShortName
        {
            get { return lblShortName.Text; }
            set { lblShortName.Text = value; }
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
        public Item Imageghavenet
        {
            //get { return imageView.Image.ToString(); }
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrEmpty(value.ThumbnailLocalPath))
                    {
                        Utils.SetImageItem(iconImg, value);

                    }
                    else
                    {
                        Utils.SetImageItemlocal(iconImg, value);
                    }
                }
                else
                {
                    iconImg.Image = null;
                }
            }
        }
        public Item Imagegnothavenet
        {
            //get { return imageView.Image.ToString(); }
            set
            {
                if (value != null)
                {
                    Utils.SetImageItemlocal(iconImg, value);
                    //if (string.IsNullOrEmpty(value.ThumbnailLocalPath))
                    //{
                    //    Utils.SetImageItem(imageView, value);

                    //}
                    //else
                    //{
                    //    Utils.SetImageItemlocal(imageView, value);
                    //}
                }
                else
                {
                    iconImg.Image = null;
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