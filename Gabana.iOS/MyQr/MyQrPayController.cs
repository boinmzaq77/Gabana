using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class MyQrPayController : UIViewController
    {
        UIImage editedImage;
        private static byte[] picture;
        UIImagePickerController imagePicker;
        UIView orderView, commentView, CommentView, BottomView , pictureView;
        UILabel lblorder, lblcomment, lblComment , lblpicture;
        UITextField txtorder, txtcomment, txtComment, txtpicture;
        UIAlertController selectPhotoMenuSheet;
        UIButton btnSave , btnimage;
        TransManage transManage = new TransManage();
        Model.TranWithDetailsLocal tranWithDetails;
        private string type;
        private UIImageView showImg;

        public MyQrPayController(string v)
        {
            this.tranWithDetails = POSController.tranWithDetails;
            this.type = v; 
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            POSController.tranWithDetails = this.tranWithDetails;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            try
            {
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        void initAttribute()
        {
            #region DeviceNoView
            orderView = new UIView();
            orderView.BackgroundColor = UIColor.White;
            orderView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblorder = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblorder.Font = lblorder.Font.WithSize(15);
            lblorder.Text = Utils.TextBundle("receiptname", "All Branch");
            orderView.AddSubview(lblorder);

            txtorder = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtorder.ReturnKeyType = UIReturnKeyType.Next;
            txtorder.ShouldReturn = (tf) =>
            {
                txtcomment.BecomeFirstResponder();
                return true;
            };
            txtorder.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtorder.AttributedPlaceholder = new NSAttributedString("", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtorder.Text = "Receipt-" + DateTime.Now.ToString("HH:mm");
            txtorder.Font = txtorder.Font.WithSize(15);
            orderView.AddSubview(txtorder);
            #endregion

            #region UDIDViewView
            commentView = new UIView();
            commentView.BackgroundColor = UIColor.White;
            commentView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblcomment = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblcomment.Font = lblcomment.Font.WithSize(15);
            lblcomment.TextAlignment = UITextAlignment.Left;
            lblcomment.Text = Utils.TextBundle("comment", "Comment");

            commentView.AddSubview(lblcomment);

            txtcomment = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtcomment.ReturnKeyType = UIReturnKeyType.Next;
            txtcomment.ShouldReturn = (tf) =>
            {
                //txtComment.BecomeFirstResponder();
                return true;
            };
            txtcomment.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtcomment.AttributedPlaceholder = new NSAttributedString("จ่ายแล้ว", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtcomment.Font = txtcomment.Font.WithSize(15);
            txtcomment.Text = Utils.TextBundle("payla", "All Branch"); 
            commentView.AddSubview(txtcomment);
            #endregion

            #region pictureview
            pictureView = new UIView();
            pictureView.BackgroundColor = UIColor.White;
            pictureView.TranslatesAutoresizingMaskIntoConstraints = false;
            pictureView.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Choosepic:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            pictureView.AddGestureRecognizer(tapGesture);
            lblpicture = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblpicture.Font = lblpicture.Font.WithSize(15);
            lblpicture.TextAlignment = UITextAlignment.Left;
            lblpicture.Text = Utils.TextBundle("imgreceipt", "All Branch");

            pictureView.AddSubview(lblpicture);

            txtpicture = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtpicture.ReturnKeyType = UIReturnKeyType.Next;
            txtpicture.ShouldReturn = (tf) =>
            {
                //txtComment.BecomeFirstResponder();
                return true;
            };
            txtpicture.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtpicture.AttributedPlaceholder = new NSAttributedString("Image Name", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtpicture.Font = txtpicture.Font.WithSize(15);
            txtpicture.Enabled = false;
            pictureView.AddSubview(txtpicture);

            

            btnimage = new UIButton();
            btnimage.SetImage(UIImage.FromBundle("Album"), UIControlState.Normal);
            btnimage.TranslatesAutoresizingMaskIntoConstraints = false;
            btnimage.TouchUpInside += (sender, e) =>
            {
                #region PhotoEditActionSheet

                selectPhotoMenuSheet = UIAlertController.Create(Utils.TextBundle("addlogo", "All Branch"), null, UIAlertControllerStyle.ActionSheet);
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("takepic", "All Branch"), UIAlertActionStyle.Default,
                                                alert => Pic("Take")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("choosepic", "All Branch"), UIAlertActionStyle.Default,
                                                alert => Pic("Choose")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

                // Show the alert
                this.PresentViewController(selectPhotoMenuSheet, true, null);
                #endregion
            };
            pictureView.AddSubview(btnimage);

            #endregion

            showImg = new UIImageView();
            showImg.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            showImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            showImg.TranslatesAutoresizingMaskIntoConstraints = false;
            //showImg.BackgroundColor = UIColor.Red;
            View.AddSubview(showImg);

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;


            btnSave = new UIButton();
            btnSave.SetTitle(Utils.TextBundle("savereceipt", "Save Receipt"), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += async (sender, e) => {
                try
                {
                    btnSave.Enabled = false;

                    decimal Cash = 0;
                    decimal paymentAmount = 0;
                    foreach (var item in tranWithDetails.tranPayments)
                    {
                        paymentAmount += item.PaymentAmount;
                    }

                    decimal amount = tranWithDetails.tran.GrandTotal - paymentAmount;

                    

                    var tranPayment = new TranPayment()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        SysBranchID = DataCashingAll.SysBranchId,
                        TranNo = tranWithDetails.tran.TranNo,
                        PaymentNo = tranWithDetails.tranPayments.Count + 1,
                        PaymentType = "MYQR",
                        PaymentAmount = (decimal)0, //เงินที่ต้องจ่าย
                        CreditCardType = null,
                        CardNo = null,
                        ExprieDateYYYYMM = null,
                        ApproveCode = null,
                        TotalRedeemPoint = null,
                        RequestNum = null,
                        RequestDateTime = null,
                        FEPaymentCancel = 0,
                        ReferenceNo1 = null,
                        ReferenceNo2 = null,
                        ReferenceNo3 = null,
                        ReferenceNo4 = null,
                        Comments = null,
                    };


                    if (editedImage != null)
                    {
                        var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                        var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        var libFolder = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture");
                        var namepic = DateTime.UtcNow.Ticks.ToString();
                        var filePath = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", namepic + ".png");
                        var FullfilePath = Path.Combine(docFolder, libFolder, namepic + ".png");
                        if (!Directory.Exists(libFolder))
                        {
                            Directory.CreateDirectory(libFolder);
                        }
                        NSData data = thumbnail.AsPNG();
                        var _picture = new byte[data.Length];
                        System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                        File.WriteAllBytes(FullfilePath, _picture);
                        tranPayment.PicturePath = filePath;
                        
                    }


                    tranPayment.PaymentAmount = amount; //เงินที่จ่าย
                    tranPayment.Comments = txtcomment.Text;
                    tranWithDetails.tranPayments.Add(tranPayment);
                    ChangeController ChangePage = new ChangeController();
                    ChangePage.Setitem(0, (double)Cash);
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("change", "All Branch"));
                    this.NavigationController.PushViewController(ChangePage, false);
                }
                catch (Exception ex )
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                }
            };
            BottomView.AddSubview(btnSave);
            #endregion

            // UIView DeviceNoView, UDIDViewView, CommentView, BottomView;
            View.AddSubview(orderView);
            View.AddSubview(commentView);
            View.AddSubview(pictureView);
            View.AddSubview(BottomView);
            BottomView.BringSubviewToFront(btnSave);


        }
        [Export("Choosepic:")]
        private void Choosepic(UIGestureRecognizer sender)
        {

            selectPhotoMenuSheet = UIAlertController.Create(Utils.TextBundle("addlogo", "All Branch"), null, UIAlertControllerStyle.ActionSheet);
            selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("takepic", "All Branch"), UIAlertActionStyle.Default,
                                            alert => Pic("Take")));
            selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("choosepic", "All Branch"), UIAlertActionStyle.Default,
                                            alert => Pic("Choose")));
            selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

            // Show the alert
            this.PresentViewController(selectPhotoMenuSheet, true, null);
        }

        private void Pic(string v)
        {
            imagePicker = new UIImagePickerController();
            if (v == "Take")
            {
                if (Utils.Checkpermisstion())
                {
                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    imagePicker.AllowsEditing = false;
                    imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
                    imagePicker.Canceled += Handle_Canceled;
                    imagePicker.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                    this.NavigationController.PresentModalViewController(imagePicker, true);
                }


            }
            else
            {

                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                imagePicker.AllowsEditing = false;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
                imagePicker.Canceled += Handle_Canceled;
                imagePicker.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                this.NavigationController.PresentModalViewController(imagePicker, true);

            }


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
                    //profileImg.Image = originalImage; // display
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = originalImage.AsJPEG(xx).AsStream();
                    showImg.Image = originalImage;
                }

                editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                editedImage = originalImage; 
                //if (editedImage != null)
                //{
                //    // do something with the image
                //    Console.WriteLine("got the edited image");
                //    nfloat quality = (nfloat)0.7;
                //    //profileImg.Image = editedImage;
                //    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                //    //picture = imageprofile.Image.AsJPEG(quality).AsStream();
                //    showImg.Image = editedImage;
                //}

            }
            imagePicker.DismissModalViewController(true);
        }
        void NewSale()
        {
            #region NewSale
            //StartActivity(new Intent(Application.Context, typeof(PosActivity)));
            //PosActivity.totlaItems = 0;
            //DataCashing.setQuantityToCart = 1;
            //POSController.tranWithDetails = null;
            //DataCashing.SysCustomerID = null;

            //if (CartActivity.cart != null)
            //{
            //    CartActivity.addRemark = false;
            //    CartActivity.cart.lnRemark.Visibility = Android.Views.ViewStates.Gone;
            //    this.Finish();
            //} 

            //if (CartScanActivity.scan != null)
            //{
            //    CartScanActivity.addRemark = false;
            //    CartScanActivity.lnRemark.Visibility = Android.Views.ViewStates.Gone;
            //    this.Finish();
            //}
            #endregion
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
        void SetupAutoLayout()
        {
            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSave.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSave.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #region DeviceNoView
            orderView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            orderView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            orderView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            orderView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblorder.CenterYAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblorder.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblorder.LeftAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblorder.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtorder.TopAnchor.ConstraintEqualTo(lblorder.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtorder.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtorder.LeftAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtorder.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region UDIDViewView
            commentView.TopAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            commentView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            commentView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            commentView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblcomment.CenterYAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblcomment.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblcomment.LeftAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblcomment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblcomment.BackgroundColor = UIColor.Red;

            txtcomment.TopAnchor.ConstraintEqualTo(lblcomment.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtcomment.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtcomment.LeftAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtcomment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region pictureView
            pictureView.TopAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            pictureView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            pictureView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            pictureView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblpicture.CenterYAnchor.ConstraintEqualTo(pictureView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblpicture.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblpicture.LeftAnchor.ConstraintEqualTo(pictureView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblpicture.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblcomment.BackgroundColor = UIColor.Red;

            txtpicture.TopAnchor.ConstraintEqualTo(lblpicture.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtpicture.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtpicture.LeftAnchor.ConstraintEqualTo(pictureView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtpicture.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnimage.CenterYAnchor.ConstraintEqualTo(pictureView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnimage.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnimage.RightAnchor.ConstraintEqualTo(pictureView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnimage.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            showImg.TopAnchor.ConstraintEqualTo(pictureView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            showImg.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
            showImg.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
            showImg.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor).Active = true;


        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }

}