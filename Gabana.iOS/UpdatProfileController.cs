using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class UpdatProfileController : UIViewController
    {
        private static byte[] picture;
        UIImagePickerController imagePicker;
        UIAlertController selectPhotoMenuSheet;
        UIButton btnChangeImage, btnSave;
        UIImageView profileImg;
        UITextField txtMerchantName, txtUsername;
        UILabel lblMerchantName, lblUsername;
        UIView logoView, line, merchantView, usernameView, bottomView;
        Merchant mer ;
        UIImage editedImage;
        bool flagRegis = true;
        public string UserName, MerchantName;
        private UILabel lblLogo;
        private UIView ReferralView;
        private UILabel lblReferral;
        private UITextField txtReferral;

        public UpdatProfileController()
        {
        }
        public UpdatProfileController(Merchant merchant)
        {
            mer = merchant; 
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //Utils.SetTitle(this.NavigationController, "Update Merchant");
            //this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51,172,225);
            //this.NavigationController.NavigationBar.TopItem.Title = "Update Merchant";
            //this.NavigationController.NavigationBar.TintColor = UIColor.White;

            this.NavigationController.NavigationBar.Translucent = true;

            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 170, 225);
            this.NavigationController.NavigationBar.BackgroundColor = UIColor.FromRGB(51, 170, 225);
            this.NavigationController.NavigationBar.TopItem.Title = "Update Merchant";
            this.NavigationController.NavigationBar.TintColor = UIColor.White;
            //Utils.SetTitle(this.NavigationController, "Choose Branch");

            View.BackgroundColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
                ForegroundColor = UIColor.White
                //BackgroundColor = UIColor.FromRGB(51, 170, 225)
            };

            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 172, 225);
            this.NavigationController.SetNavigationBarHidden(false, false);

            Textboxfocus(View);
            View.BackgroundColor = UIColor.White;

            
            #region LayoutAtrribute

            #region LogoView
            logoView = new UIView();
            logoView.BackgroundColor = UIColor.White;
            logoView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(logoView);

            profileImg = new UIImageView();
            profileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            profileImg.Layer.CornerRadius = 75;
            profileImg.ClipsToBounds = true;
            profileImg.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            logoView.AddSubview(profileImg);

            lblLogo = new UILabel
            {
                TextColor = UIColor.FromRGB(172, 172, 172),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblLogo.Font = lblLogo.Font.WithSize(24);
            lblLogo.TextAlignment = UITextAlignment.Center;
            lblLogo.Text = "";
            logoView.AddSubview(lblLogo);

            

            btnChangeImage = new UIButton();
            btnChangeImage.Layer.CornerRadius = 75;
            btnChangeImage.SetImage(UIImage.FromFile("AddImg.png"), UIControlState.Normal);
            btnChangeImage.ImageView.ContentMode = UIViewContentMode.ScaleToFill;
            btnChangeImage.TranslatesAutoresizingMaskIntoConstraints = false;
            btnChangeImage.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnChangeImage.TouchUpInside += (sender, e) => {
                // change image
                //selectPhotoMenuSheet.ShowInView(View);
                #region PhotoEditActionSheet

                selectPhotoMenuSheet = UIAlertController.Create("Add Logo", null, UIAlertControllerStyle.ActionSheet);
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Take a picture", UIAlertActionStyle.Default,
                                                alert => Pic("Take")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Choose your picture", UIAlertActionStyle.Default,
                                                alert => Pic("Choose")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

                // Show the alert
                this.PresentViewController(selectPhotoMenuSheet, true, null);
                #endregion
            };
            logoView.AddSubview(btnChangeImage);

            #endregion

            #region MerchantView
            merchantView = new UIView();
            merchantView.BackgroundColor = UIColor.White;
            merchantView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(merchantView);

            lblMerchantName = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerchantName.Font = lblMerchantName.Font.WithSize(15);
            lblMerchantName.Text = "Merchant Name";
            merchantView.AddSubview(lblMerchantName);

            txtMerchantName = new UITextField
            {
                TextColor = UIColor.FromRGB(51,170,225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtMerchantName.AttributedPlaceholder = new NSAttributedString("Merchant Name", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtMerchantName.Font = txtMerchantName.Font.WithSize(15);
            txtMerchantName.ReturnKeyType = UIReturnKeyType.Next;
            txtMerchantName.ShouldReturn = (tf) =>
            {
                txtUsername.BecomeFirstResponder();
                return true;
            };
            txtMerchantName.EditingChanged += (object sender, EventArgs e) =>
            {
                if (txtUsername.Text.Length > 0 && txtMerchantName.Text.Length > 0)
                {
                    btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnSave.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                }
                else
                {
                    btnSave.SetTitleColor(UIColor.FromRGB(51, 170, 225), UIControlState.Normal);
                    btnSave.BackgroundColor = UIColor.White;
                }

            };
            merchantView.AddSubview(txtMerchantName);
            #endregion

            #region usernameView
            usernameView = new UIView();
            usernameView.BackgroundColor = UIColor.White;
            usernameView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(usernameView);

            lblUsername = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblUsername.Font = lblMerchantName.Font.WithSize(15);
            lblUsername.Text = "User Name";
            usernameView.AddSubview(lblUsername);

            txtUsername = new UITextField
            {
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtUsername.AttributedPlaceholder = new NSAttributedString("User Name", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtUsername.Font = txtUsername.Font.WithSize(15);
            txtUsername.ReturnKeyType = UIReturnKeyType.Done;
            txtUsername.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtUsername.EditingChanged += (object sender, EventArgs e) =>
            {
                if (txtUsername.Text.Length > 0 && txtMerchantName.Text.Length > 0)
                {
                    btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnSave.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                }
                else
                {
                    btnSave.SetTitleColor(UIColor.FromRGB(51, 170, 225), UIControlState.Normal);
                    btnSave.BackgroundColor = UIColor.White;
                }

            };
            usernameView.AddSubview(txtUsername);

            #endregion

            #region ReferralView
            ReferralView = new UIView();
            ReferralView.BackgroundColor = UIColor.White;
            ReferralView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(ReferralView);

            lblReferral = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblReferral.Font = lblMerchantName.Font.WithSize(15);
            lblReferral.Text = "Referral Code (If any)";
            ReferralView.AddSubview(lblReferral);

            txtReferral = new UITextField
            {
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtReferral.AttributedPlaceholder = new NSAttributedString("Referral Code", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtReferral.Font = txtUsername.Font.WithSize(15);
            txtReferral.ReturnKeyType = UIReturnKeyType.Done;
            txtReferral.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };

            ReferralView.AddSubview(txtReferral);

            #endregion

            #region BottomView
            bottomView = new UIImageView();
            bottomView.BackgroundColor = UIColor.White;
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(bottomView);

            btnSave = new UIButton();
            btnSave.Layer.BorderColor = UIColor.FromRGB(51,170,225).CGColor;
            btnSave.Layer.BorderWidth = 1;
            btnSave.SetTitleColor(UIColor.FromRGB(51,170,225),UIControlState.Normal);
            //if (MainController.merchant == null)
            //{
                flagRegis = true;
                btnSave.SetTitle("Register", UIControlState.Normal);

                var view = new UIView();
                var button = new UIButton(UIButtonType.Custom);
                button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                //button.SetTitle("  " + Utils.TextBundle("back", "Back"), UIControlState.Normal);
                button.SetTitleColor(UIColor.White, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);
            //}
            //else
            //{
            //    flagRegis = false;
            //    btnSave.SetTitle("Save", UIControlState.Normal);
               
            //    SetupMerchantDataAsync();
            //}
            btnSave.Layer.CornerRadius = 5f;
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += async (sender, e) => {
                if ((txtMerchantName.Text != null && txtMerchantName.Text != "") || (txtUsername.Text != null && txtUsername.Text != ""))
                {
                    await InsertProfile();
                }
                else
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                }
                //if (flagRegis)
                //{
                    
                //}
                //else
                //{
                //    if ((txtMerchantName.Text != null && txtMerchantName.Text != "") || (txtUsername.Text != null && txtUsername.Text != ""))
                //    {
                //        UpdateProfile();
                //    }
                //    else
                //    {
                //        Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                //    }
                //}
                

            };
            View.AddSubview(btnSave);
            #endregion

                line = new UIView();
                line.BackgroundColor = new UIColor(red: 248 / 255f, green: 248 / 255f, blue: 248 / 255f, alpha: 1);
                line.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(line);
            #endregion
            setupAutoLayout();
        }

        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            Preferences.Set("AppState", "logout");
            Preferences.Set("Branch", "");
            this.NavigationController.DismissViewController(false, null);
        }

        public async void UpdateProfile()
        {
            try
            {
                this.NavigationController.DismissViewController(false, null);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        public async Task InsertProfile()
        {
            try
            {
                var PathLogo = "";
                byte[] imageByteArray = null; 
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
                    NSData data = editedImage.AsPNG();
                    var _picture = new byte[data.Length];
                    System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                    File.WriteAllBytes(FullfilePath, _picture);

                    imageByteArray = Utils.ReadFully(editedImage.AsJPEG().AsStream());
                    PathLogo = filePath;
                }
               

                string Id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
                var merchants = new Gabana3.JAM.Merchant.Merchants()
                {
                    Merchant = new ORM.Master.Merchant()
                    {
                        LogoPath = PathLogo,
                        Name = txtMerchantName.Text
                    },
                    Device = new ORM.Master.Device()
                    {
                        UDID = Id,
                        Platform = "APNS"
                    },
                    UserAccountInfo = new List<ORM.Master.UserAccountInfo>()
                    {
                        new ORM.Master.UserAccountInfo()
                        {
                            UserName = txtUsername.Text,
                        }
                    }
                    ,
                    BonusCode = txtReferral.Text?.Trim(),
                };
                
                var InsertMerchant = await GabanaAPI.PostMerchant(merchants, imageByteArray);
                this.NavigationController.DismissViewController(false, null);
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        public async Task SetupMerchantDataAsync()
        {
            
            profileImg.Image = UIImage.FromFile("LogoDefault.png");
            txtMerchantName.Text = mer.Name;
            txtUsername.Text = mer.Name;
        }
        private void Pic(string v)
        {
            imagePicker = new UIImagePickerController();
            if (v == "Take")
            {
                if (Utils.Checkpermisstion())
                {
                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    imagePicker.AllowsEditing = true;
                    imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
                    imagePicker.Canceled += Handle_Canceled;
                    imagePicker.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
                    this.NavigationController.PresentModalViewController(imagePicker, true);
                }


            }
            else
            {

                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                imagePicker.AllowsEditing = true;
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
                    profileImg.Image = originalImage; // display
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = originalImage.AsJPEG(xx).AsStream();
                }

                editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                if (editedImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the edited image");
                    nfloat quality = (nfloat)0.7;
                    lblLogo.Hidden = true;
                    profileImg.Image = editedImage;
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
        void setupAutoLayout()
        {
            logoView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            logoView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            logoView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            logoView.HeightAnchor.ConstraintEqualTo(224).Active = true;

            profileImg.CenterXAnchor.ConstraintEqualTo(logoView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            profileImg.CenterYAnchor.ConstraintEqualTo(logoView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            profileImg.HeightAnchor.ConstraintEqualTo(150).Active = true;
            profileImg.WidthAnchor.ConstraintEqualTo(150).Active = true;

            lblLogo.CenterXAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblLogo.CenterYAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblLogo.HeightAnchor.ConstraintEqualTo(30).Active = true;
            lblLogo.WidthAnchor.ConstraintEqualTo(60).Active = true;

            btnChangeImage.BottomAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            btnChangeImage.RightAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.RightAnchor).Active = true;
            btnChangeImage.HeightAnchor.ConstraintEqualTo(50).Active = true;
            btnChangeImage.WidthAnchor.ConstraintEqualTo(50).Active = true;

            line.TopAnchor.ConstraintEqualTo(logoView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(5).Active = true;
            line.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor,0).Active = true;

            merchantView.TopAnchor.ConstraintEqualTo(line.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            merchantView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            merchantView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            merchantView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblMerchantName.TopAnchor.ConstraintEqualTo(merchantView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblMerchantName.LeftAnchor.ConstraintEqualTo(merchantView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblMerchantName.RightAnchor.ConstraintEqualTo(merchantView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblMerchantName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtMerchantName.TopAnchor.ConstraintEqualTo(lblMerchantName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtMerchantName.LeftAnchor.ConstraintEqualTo(merchantView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtMerchantName.RightAnchor.ConstraintEqualTo(merchantView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtMerchantName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            usernameView.TopAnchor.ConstraintEqualTo(merchantView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            usernameView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            usernameView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            usernameView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblUsername.TopAnchor.ConstraintEqualTo(usernameView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblUsername.LeftAnchor.ConstraintEqualTo(usernameView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblUsername.RightAnchor.ConstraintEqualTo(usernameView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblUsername.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtUsername.TopAnchor.ConstraintEqualTo(lblUsername.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtUsername.LeftAnchor.ConstraintEqualTo(usernameView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtUsername.RightAnchor.ConstraintEqualTo(usernameView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtUsername.HeightAnchor.ConstraintEqualTo(18).Active = true;

            ReferralView.TopAnchor.ConstraintEqualTo(usernameView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ReferralView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ReferralView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ReferralView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblReferral.TopAnchor.ConstraintEqualTo(ReferralView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblReferral.LeftAnchor.ConstraintEqualTo(ReferralView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblReferral.RightAnchor.ConstraintEqualTo(ReferralView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            lblReferral.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtReferral.TopAnchor.ConstraintEqualTo(lblReferral.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtReferral.LeftAnchor.ConstraintEqualTo(ReferralView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtReferral.RightAnchor.ConstraintEqualTo(ReferralView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtReferral.HeightAnchor.ConstraintEqualTo(18).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSave.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSave.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}