using FFImageLoading;
using Foundation;
using System;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class itemPOSViewCell : UICollectionViewCell
    {
        public itemPOSViewCell (IntPtr handle) : base (handle)
        {
        }
        public itemPOSViewCell()
        {

        }
        public string Name
        {
            get { return lblitemName.Text; }
            set { lblitemName.Text = value; }
        }
        public string Price
        {
            get { return lblItemprice.Text; }
            set { lblItemprice.Text = value; }
        }
        public string Image
        {
            get { return itemPOSIMG.Image.ToString(); }
            set
            {
                SetImage(itemPOSIMG, value);
            }
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            if(value!=null && value != "")
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