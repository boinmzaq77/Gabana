using FFImageLoading;
using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class mainitemViewCell : UICollectionViewCell
    {
        public mainitemViewCell (IntPtr handle) : base (handle)
        {
        }
        public mainitemViewCell()
        {

        }
        public string Image
        {
            get { return iconImg.Image.ToString(); }
            set
            {
                SetImage(iconImg, value);
            }
        }
        public string Name
        {
            get { return IconName.Text; }
            set { IconName.Text = value; }
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            if(value!= null && value!="")
            {
               ImageView.Image = UIImage.FromBundle(value);
            }
        }

    }
    
}