using Foundation;
using Gabana.ORM.Local;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UIKit;

namespace Gabana.ios.Phone
{
    public partial class editProfileController : UIViewController
    {
        UIImagePickerController imagePicker;
        private static byte[] picture;
        public LocalDBTransaction conn;
        public editProfileController(IntPtr handle) : base(handle)
        {
        }
        public async override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
           
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationItem.Title = "";
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.NavigationBar.TintColor = new UIColor(red: 0f, green: 149f / 255f, blue: 218f / 255f, alpha: 1f);

        }

        partial void BtnEditPIC_TouchUpInside(UIButton sender)
        {
            // upload pic
            string pic = "MainLogout.png";
        }

        partial void UIButton373718_TouchUpInside(UIButton sender)
        {
            //update profile
            try
            {
                conn = new LocalDBTransaction();
                //conn.ConnectLocalBase();
                var b = new Merchant()
                {
                    MerchantID = 1,
                    DateModified = DateTime.Now,
                    Name = "Weeraya Zhang",
                    Logo = "MainLogout.png"
                };
                conn.UpdateDataTable(b);
                MainPageController edit = this.Storyboard?.InstantiateViewController("MainPageController") as MainPageController;
                this.NavigationController.PushViewController(edit, true);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            //  MainPageController.a = b;
        }
        [Export("choosepic:")]
        public void QrCode(UIGestureRecognizer sender)
        {
            var okCancelAlertController = UIAlertController.Create("", "Get Picture From ? ", UIAlertControllerStyle.Alert);

            //Add Actions
            okCancelAlertController.AddAction(UIAlertAction.Create("Choose Picture", UIAlertActionStyle.Default,
                alert => Pic("Choose")));
            okCancelAlertController.AddAction(UIAlertAction.Create("Take Picture", UIAlertActionStyle.Cancel, alert => Pic("Take")));

            //Present Alert
            PresentViewController(okCancelAlertController, true, null);



        }

        private void Pic(string v)
        {
            imagePicker = new UIImagePickerController();
            if (v == "Take")
            {
                imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
            }
            else
            {
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            }

            imagePicker.AllowsEditing = true;
            //imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;

            imagePicker.Canceled += Handle_Canceled;
            imagePicker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            NavigationController.PresentModalViewController(imagePicker, true);
        }
        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }

        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            // get common info (shared between images and video)
            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.ToString());

            // if it was an image, get the other image info
            if (isImage)
            {
                var x = e.Info[UIImagePickerController.OriginalImage];

                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {

                    originalImage.Scale(new CoreGraphics.CGSize(200, 200));
                    nfloat quality = (nfloat)0.7;
                    // do something with the image
                    imgProfile.Image = originalImage; // display
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = originalImage.AsJPEG(xx).AsStream();
                }

                UIImage editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                if (editedImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the edited image");
                    nfloat quality = (nfloat)0.7;
                    imgProfile.Image = editedImage;
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = imageprofile.Image.AsJPEG(quality).AsStream();

                }

            }
            imagePicker.DismissModalViewController(true);
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}