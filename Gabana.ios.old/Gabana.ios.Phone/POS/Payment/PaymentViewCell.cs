using FFImageLoading;
using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class PaymentViewCell : UICollectionViewCell
    {
        public PaymentViewCell (IntPtr handle) : base (handle)
        {
        }
        public PaymentViewCell()
        {

        }
        public string Name
        {
            get { return lblpaymentTypeName.Text; }
            set { lblpaymentTypeName.Text = value; }
        }
        public string Image
        {
            get { return imgPaymentMenu.Image.ToString(); }
            set
            {
                SetImage(imgPaymentMenu, value);
            }
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            string noimg;
            noimg = "logo.jpg";
            if (value != null)
            {
                ImageService.Instance.LoadUrl(value)
                    // todo : LoadingPlaceholder
                    .LoadingPlaceholder(value, FFImageLoading.Work.ImageSource.CompiledResource)
                    .WithCache(FFImageLoading.Cache.CacheType.Disk)
                    //.dura
                    //.CacheDuration(TimeSpan.FromDays(30))
                    .Into(ImageView);
            }
        }
    }
}