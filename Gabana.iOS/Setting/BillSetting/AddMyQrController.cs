using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
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
    public partial class AddMyQrController : UIViewController
    {
        UIView QrNameView, QrBranchView, QrImageView, CommentView,_contentView,ShowImageView, BottomViewEdit;
        UIScrollView _scrollView;
        UIButton btnAdd, btnDelete, btnSave;
        UILabel lbl_QrName, lbl_QrBranch, lbl_QrImage, lbl_comment;
        UITextField txt_QrName, txt_QrBranch, txt_comment, txt_QrImageName;
        UIImageView btnAddImg, showImg,btnBranch;
        UIAlertController selectPhotoMenuSheet;
        UIImagePickerController imagePicker;
        private static byte[] picture;
        UIImage editedImage;
        BranchManage branchManage1 = new BranchManage();
        string UserLogin ="";
        int MyQrCodeRunning = 0;
        MyQrCodeManage MyQrCodeManager = new MyQrCodeManage();
        public static string selectbranch = string.Empty;
        char setBranch;
        MyQrCode QRDetail = new MyQrCode();
        public static List<BranchPolicy> listChooseBranch = new List<BranchPolicy>();
        public static char typebranch;
        public static int? sysbranch;
        public static bool edit;
        private bool Editchange = false;

        public AddMyQrController()
        {
        }

        public AddMyQrController(MyQrCode Detail)
        {
            this.QRDetail = Detail;
        }
        public async  override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            if (QRDetail==null)
            {
                Utils.SetTitle(this.NavigationController, "Setting myQR");
            }
            else
            {
                Utils.SetTitle(this.NavigationController, "Edit myQR");
            }
            
            this.NavigationController.SetNavigationBarHidden(false, false);
            try
            {
                if (edit)
                {
                    if (typebranch == 'A')
                    {
                        txt_QrBranch.Text = Utils.TextBundle("allbranch", "All Branch");
                    }
                    else
                    {

                        var branch = await branchManage1.GetAllBranch((int)MainController.merchantlocal.MerchantID);
                        var bselect = branch.Where(x => x.SysBranchID == sysbranch).FirstOrDefault();
                        txt_QrBranch.Text = bselect.BranchName;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);

            }
        }
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                Editchange = true;
                btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSave.Layer.CornerRadius = 5f;
                btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);

                btnAdd.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAdd.Layer.CornerRadius = 5f;
                btnAdd.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                Editchange = false;
                btnSave.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSave.Layer.CornerRadius = 5f;
                btnSave.BackgroundColor = UIColor.White;
                btnSave.ClipsToBounds = true;
                btnSave.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSave.Layer.BorderWidth = 0.5f;
            }
            btnSave.Enabled = enable;
            btnAdd.Enabled = enable;
        }
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", "มีการเปลี่ยนแปลงต้องการบันทึกหรือไม่", UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    async alert => await BtnSave_Click3()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => this.NavigationController.PopViewController(true)));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                this.NavigationController.PopViewController(true);
            }

        }
        private async Task BtnSave_Click3()
        {
            if (QRDetail == null )
            {
                BtnAdd_Click();
            }
            else
            {
                UpdateQR();
            }
            
            
        }

        public async override void ViewDidLoad()
        {
            try
            {
                var view = new UIView();
                var button = new UIButton(UIButtonType.Custom);
                button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                button.SetTitle("  Back", UIControlState.Normal);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);

                UserLogin = Preferences.Get("User", "");

                base.ViewDidLoad();
                typebranch = 'A';
                sysbranch = null;
                edit = false;
                initAttribute();
                setupAutoLayout();
                Textboxfocus(View);
                var lstcount = await MyQrCodeManager.GetAllMyQrCode(DataCashingAll.MerchantId);
                MyQrCodeRunning = lstcount.Count;

                if (QRDetail == null || QRDetail.MyQrCodeName==null) // notnull
                {
                    ShowImageView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    btnAdd.Hidden = false;
                    BottomViewEdit.Hidden = true;
                }
                else
                {
                    btnAdd.Hidden = true;
                    BottomViewEdit.Hidden = false;
                    ShowImageView.HeightAnchor.ConstraintEqualTo(410).Active = true;
                    setupData();
                    
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
           
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        async void SetBranchData()
        {

            

            if (QRDetail.FMyQrAllBranch == 'A')
            {

                typebranch = 'A';
                sysbranch = null;
                txt_QrBranch.Text = Utils.TextBundle("allbranch", "All Branch");
            }
            else
            {
                typebranch = 'B';
                sysbranch = (int)QRDetail.SysBranchID;
                BranchManage branchManage = new BranchManage();
                var lstBranch = await branchManage.GetAllBranch((int)MainController.merchantlocal.MerchantID);
                if (lstBranch != null)
                {
                    var result = lstBranch.Where(x => x.SysBranchID == Convert.ToInt64(sysbranch)).FirstOrDefault();
                    txt_QrBranch.Text = result.BranchName;
                }
            }
        }
        async void setupData()
        {
            SetBranchData();
            txt_QrName.Text = QRDetail.MyQrCodeName;
            txt_comment.Text = QRDetail.Comments;
            if(await GabanaAPI.CheckNetWork())
            {
                 if (QRDetail.PicturePath != null)
                {
                    Utils.SetImageURL(showImg, QRDetail.PicturePath);
                }
                else
                {
                    Utils.SetImageURL(showImg, QRDetail.PictureLocalPath);
                }
            }
            else
            {
                Utils.SetImageURL(showImg, QRDetail.PictureLocalPath);
            }
            
            
        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region QrNameView
            QrNameView = new UIView();
            QrNameView.TranslatesAutoresizingMaskIntoConstraints = false;
            QrNameView.BackgroundColor = UIColor.FromRGB(255,255,255);

            lbl_QrName = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_QrName.Font = lbl_QrName.Font.WithSize(15);
            lbl_QrName.Text = Utils.TextBundle("myqrname", "my QR Name");
            QrNameView.AddSubview(lbl_QrName);

            txt_QrName = new UITextField
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txt_QrName.EditingDidEnd += Txt_QrName_EditingChanged;
            txt_QrName.Font = txt_QrName.Font.WithSize(15);
            txt_QrName.ReturnKeyType = UIReturnKeyType.Next;
            txt_QrName.ShouldReturn = (tf) =>
            {
                txt_comment.BecomeFirstResponder();
                return true;
            };
            txt_QrName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("myqrname", "my QR Name"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            QrNameView.AddSubview(txt_QrName);
            #endregion

            #region QrBranchView
            QrBranchView = new UIView();
            QrBranchView.TranslatesAutoresizingMaskIntoConstraints = false;
            QrBranchView.BackgroundColor = UIColor.FromRGB(255, 255, 255);

            lbl_QrBranch = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_QrBranch.Font = lbl_QrBranch.Font.WithSize(15);
            lbl_QrBranch.Text = Utils.TextBundle("branch", "Branch");
            QrBranchView.AddSubview(lbl_QrBranch);

            txt_QrBranch = new UITextField
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txt_QrBranch.Enabled = false;
            txt_QrBranch.Font = txt_QrBranch.Font.WithSize(15);
            txt_QrBranch.ReturnKeyType = UIReturnKeyType.Done;
            txt_QrBranch.EditingDidEnd += Txt_QrName_EditingChanged;
            txt_QrBranch.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txt_QrBranch.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("allbranch", "All Branch"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            QrBranchView.AddSubview(txt_QrBranch);

            btnBranch = new UIImageView();
            btnBranch.Image = UIImage.FromBundle("Next");
            btnBranch.TranslatesAutoresizingMaskIntoConstraints = false;

            btnBranch.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("SelectBranch:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnBranch.AddGestureRecognizer(tapGesture);
            QrBranchView.AddSubview(btnBranch);

            #endregion

            #region QrImageView
            QrImageView = new UIView();
            QrImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            QrImageView.BackgroundColor = UIColor.FromRGB(255, 255, 255);

            lbl_QrImage = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_QrImage.Font = lbl_QrImage.Font.WithSize(15);
            lbl_QrImage.Text = Utils.TextBundle("qrimage", "QR Image");
            QrImageView.AddSubview(lbl_QrImage);

            txt_QrImageName = new UITextField
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txt_QrImageName.Enabled = false;
            txt_QrImageName.Font = txt_QrImageName.Font.WithSize(15);
            txt_QrImageName.ReturnKeyType = UIReturnKeyType.Done;
            txt_QrImageName.EditingDidEnd += Txt_QrName_EditingChanged;
            txt_QrImageName.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txt_QrImageName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("addqrimage", "Add QR Image"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            QrImageView.AddSubview(txt_QrImageName);

            btnAddImg = new UIImageView();
            btnAddImg.Image = UIImage.FromBundle("Album");
            btnAddImg.TranslatesAutoresizingMaskIntoConstraints = false;
            QrImageView.AddSubview(btnAddImg);

            btnAddImg.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("AddQR:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAddImg.AddGestureRecognizer(tapGesture1);

            #endregion

            #region ShowImageView
            ShowImageView = new UIView();
            ShowImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            ShowImageView.BackgroundColor = UIColor.FromRGB(255, 255, 255);
            //ShowImageView.BackgroundColor = UIColor.Green;

            showImg = new UIImageView();
            showImg.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            showImg.ContentMode = UIViewContentMode.ScaleAspectFit;
            showImg.TranslatesAutoresizingMaskIntoConstraints = false;
            //showImg.BackgroundColor = UIColor.Red;
            ShowImageView.AddSubview(showImg);

            #endregion

            #region CommentView
            CommentView = new UIView();
            CommentView.TranslatesAutoresizingMaskIntoConstraints = false;
            CommentView.BackgroundColor = UIColor.FromRGB(255, 255, 255);

            lbl_comment = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_comment.Font = lbl_comment.Font.WithSize(15);
            lbl_comment.Text = Utils.TextBundle("comment", "Comment");
            CommentView.AddSubview(lbl_comment);

            txt_comment = new UITextField
            {
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txt_comment.EditingDidEnd += Txt_QrName_EditingChanged;
            txt_comment.Font = txt_comment.Font.WithSize(15);
            txt_comment.ReturnKeyType = UIReturnKeyType.Done;
            txt_comment.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txt_comment.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("comment", "Comment"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            CommentView.AddSubview(txt_comment);
            #endregion

            _contentView.AddSubview(QrNameView);
            _contentView.AddSubview(QrBranchView);
            _contentView.AddSubview(QrImageView);
            _contentView.AddSubview(ShowImageView);
            _contentView.AddSubview(CommentView);

             _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);

            View.BackgroundColor = UIColor.White;

            btnAdd = new UIButton();
            btnAdd.SetTitle(Utils.TextBundle("addmyqr", "Add myQR"), UIControlState.Normal);
            btnAdd.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnAdd.Layer.CornerRadius = 5f;
            btnAdd.BackgroundColor = UIColor.White;
            btnAdd.ClipsToBounds = true;
            btnAdd.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnAdd.Layer.BorderWidth = 0.5f;
            btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAdd.TouchUpInside += async (sender, e) => {
                btnAdd.Enabled = false; 
                BtnAdd_Click();
            };
            View.AddSubview(btnAdd);

            #region BottomViewEdit
            BottomViewEdit = new UIView();
            BottomViewEdit.Hidden = true;
            BottomViewEdit.BackgroundColor = UIColor.Clear;
            BottomViewEdit.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomViewEdit);

            btnDelete = new UIButton();
            btnDelete.Layer.CornerRadius = 5f;
            btnDelete.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnDelete.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDelete.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDelete.TouchUpInside += (sender, e) => {
                Delete();
            };
            BottomViewEdit.AddSubview(btnDelete);

            btnSave = new UIButton();
            btnSave.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.BackgroundColor = UIColor.White;
            btnSave.ClipsToBounds = true;
            btnSave.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSave.Layer.BorderWidth = 0.5f;
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += (sender, e) => {
                // sum items
                UpdateQR();
            };
            BottomViewEdit.AddSubview(btnSave);
            #endregion
        }

        private void Txt_QrName_EditingChanged(object sender, EventArgs e)
        {
            Editchange = true;
            if (string.IsNullOrEmpty(txt_comment.Text) && string.IsNullOrEmpty(txt_QrBranch.Text) && string.IsNullOrEmpty(txt_QrName.Text)&& string.IsNullOrEmpty(txt_QrImageName.Text))
            {
                SetButtonAdd(true);
            }
            else
            {
                SetButtonAdd(true);
            }
           
        }

        void CheckNull()
        {
            
        }
        private async void BtnAdd_Click()
        {
            try
            {
                if (await GabanaAPI.CheckNetWork())
                {
                    if (string.IsNullOrEmpty(txt_QrName.Text) == true )
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกข้อมูลให้ครบถ้วน");
                        btnAdd.Enabled = true;
                        return;
                    }

                    byte[] imageByteArray = null;
                    if (editedImage != null)
                    {
                        imageByteArray = Utils.ReadFully(editedImage.AsJPEG(0.7f).AsStream());
                    }
                    else
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณาเลือก Qrcode");
                        btnAdd.Enabled = true;
                        return;
                    }
                    ORM.Master.MyQrCode myQrCode = new ORM.Master.MyQrCode()
                    {
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        MerchantID = (int)DataCashingAll.MerchantId,
                        Ordinary = null,
                        UserNameModified = UserLogin,
                        Comments = txt_comment.Text,
                        FMyQrAllBranch = typebranch,
                        MyQrCodeName = txt_QrName.Text,
                        MyQrCodeNo = MyQrCodeRunning++,
                        //PicturePath = txt_QrImageName.Text,
                        SysBranchID = sysbranch == null ? (int?)null : Convert.ToInt32(sysbranch) // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : SysBranchId ที่เลือก
                    };
                    var result = await GabanaAPI.PostDataMyQrCode(myQrCode, imageByteArray);
                    if (result.Status)
                    {
                        ORM.MerchantDB.MyQrCode localmyQrCode = new ORM.MerchantDB.MyQrCode()
                        {
                            DateCreated = DateTime.UtcNow,
                            DateModified = DateTime.UtcNow,
                            MerchantID = (int)DataCashingAll.MerchantId,
                            Ordinary = null,
                            UserNameModified = UserLogin,
                            Comments = txt_comment.Text,
                            FMyQrAllBranch = typebranch,
                            MyQrCodeName = txt_QrName.Text,
                            MyQrCodeNo = MyQrCodeRunning++,
                            //PicturePath = PicturePath,
                            SysBranchID = sysbranch  == null ? (int?)null : Convert.ToInt32(sysbranch)
                        };
                        var insert = await MyQrCodeManager.InsertMyQrCode(localmyQrCode);

                        Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                        MyQrSettingController.isModifyQR = true;
                        btnAdd.Enabled = true;
                        Editchange = false;
                        this.NavigationController.PopViewController(false);
                    }
                    else
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกข้อมูลให้ครบถ้วน");
                        btnAdd.Enabled = true;
                        return;
                    }
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกข้อมูลให้ครบถ้วน");
                    btnAdd.Enabled = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdd_Click at add Qr");
                btnAdd.Enabled = true;
                return;
            }
        }
        private async void Delete()
        {
            //Create Alert
            var okCancelAlertController = UIAlertController.Create("",Utils.TextBundle("checkdeleteqrcode", "Are you sure you want to delete QR Code ")  + QRDetail.MyQrCodeName + " ?", UIAlertControllerStyle.Alert);

            //Add Actions
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("delete", "Delete"), UIAlertActionStyle.Default, Action => delete_click()));
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel Delete")));

            //Present Alert
            PresentViewController(okCancelAlertController, true, null);
        }
        private async void delete_click()
        {
            if (await GabanaAPI.CheckNetWork())
            {
                var QrNo = (int)QRDetail.MyQrCodeNo;
                var result = await GabanaAPI.DeleteDataMyQrCode(QrNo);
                if (result.Status)
                {
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
                    MyQrCodeManage MyQrCodeManage = new MyQrCodeManage();
                    var delete = await MyQrCodeManage.DeleteMyQrCode((int)MainController.merchantlocal.MerchantID, QrNo);
                    MyQrSettingController.isModifyQR = true;
                    this.NavigationController.PopViewController(true);
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("failedtodelete", "Failed to delete"));
                }
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("nointernet", " Internet,Please Connect"));
                return;
            }
        }
        public async void UpdateQR()
        {
            try
            {
                if (await GabanaAPI.CheckNetWork())
                {
                    string localpath = string.Empty;
                    string Cloudpath = string.Empty;
                    CheckNull();
                    int? myInt = QRDetail.Ordinary == null ? (int?)null : Convert.ToInt32(QRDetail.Ordinary);
                    int? branch = string.IsNullOrEmpty(selectbranch) ? (int?)null : Convert.ToInt32(selectbranch);

                    byte[] stream = null;
                    var PicturePath = "";
                    if (editedImage != null)
                    {
                        stream = Utils.ReadFully(editedImage.AsJPEG().AsStream());
                    }
                    else
                    {
                        Cloudpath = QRDetail.PicturePath;
                        localpath = QRDetail.PictureLocalPath;
                    }

                    ORM.Master.MyQrCode myQrCode = new ORM.Master.MyQrCode()
                    {
                        DateCreated = QRDetail.DateCreated,
                        DateModified = DateTime.UtcNow,
                        MerchantID = (int)DataCashingAll.MerchantId,
                        Ordinary = myInt,
                        UserNameModified = UserLogin,
                        Comments = txt_comment.Text,
                        FMyQrAllBranch = typebranch,
                        MyQrCodeName = txt_QrName.Text,
                        MyQrCodeNo = (int)QRDetail.MyQrCodeNo,
                        PicturePath = Cloudpath,
                        SysBranchID = sysbranch == null ? (int?)null : Convert.ToInt32(sysbranch)
                    };

                    var result = await GabanaAPI.PutDataMyQrCode(myQrCode, stream);
                    if (result.Status)
                    {

                        ORM.MerchantDB.MyQrCode localmyQrCode = new ORM.MerchantDB.MyQrCode()
                        {
                            DateCreated = QRDetail.DateCreated,
                            DateModified = DateTime.UtcNow,
                            MerchantID = (int)DataCashingAll.MerchantId,
                            Ordinary = myInt,
                            UserNameModified = UserLogin,
                            Comments = txt_comment.Text,
                            FMyQrAllBranch = typebranch,
                            MyQrCodeName = txt_QrName.Text,
                            MyQrCodeNo = (int)QRDetail.MyQrCodeNo,
                            SysBranchID = sysbranch == null ? (int?)null : Convert.ToInt32(sysbranch)
                        };
                        var update = await MyQrCodeManager.UpdateMyQrCode(localmyQrCode);


                        Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "Edit data successfully"));
                        MyQrSettingController.isModifyQR = true;
                        Editchange = false;
                        this.NavigationController.PopViewController(false);
                    }
                    else
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกข้อมูลให้ครบถ้วน");
                        return;
                    }
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกข้อมูลให้ครบถ้วน");
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnEdit_Click at add Qr");
                return;
            }
        }
        [Export("AddQR:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            #region PhotoEditActionSheet
            selectPhotoMenuSheet = UIAlertController.Create("Add Logo", null, UIAlertControllerStyle.ActionSheet);
            //selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Take a picture", UIAlertActionStyle.Default,
            //                                alert => Pic("Take")));
            selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Choose your picture", UIAlertActionStyle.Default,
                                            alert => Pic("Choose")));
            selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

            // Show the alert
            this.NavigationController.PresentModalViewController(selectPhotoMenuSheet, true);
            #endregion
        }

        [Export("SelectBranch:")]
        public void SelectBranch(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("choosebranch", "Choose Branch"));
            var branchChoosePage = new QrcodeChooseBranchController();
            this.NavigationController.PushViewController(branchChoosePage, false);
        }

        #region Picture Set
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
                    showImg.Image = originalImage; // display
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    txt_QrImageName.Text = originalImage.ToString();
                    //picture = originalImage.AsJPEG(xx).AsStream();
                }

                 editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                if (editedImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the edited image");
                    nfloat quality = (nfloat)0.7;
                    showImg.Image = editedImage;
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    txt_QrImageName.Text = editedImage.ToString();

                    //picture = imageprofile.Image.AsJPEG(quality).AsStream();
                    ShowImageView.HeightAnchor.ConstraintEqualTo(410).Active = true;
                    Utils.SetConstant(ShowImageView.Constraints, NSLayoutAttribute.Height, 410);
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
#endregion

        void setupAutoLayout()
        {
            btnAdd.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnAdd.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAdd.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnAdd.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;

            //UIScrollView can be any size 
            BottomViewEdit.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomViewEdit.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomViewEdit.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            BottomViewEdit.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(45).Active = true;

            btnSave.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSave.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;


            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(BottomViewEdit.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region QrNameView
            QrNameView.TopAnchor.ConstraintEqualTo(QrNameView.Superview.TopAnchor, 0).Active = true;
            QrNameView.LeftAnchor.ConstraintEqualTo(QrNameView.Superview.LeftAnchor, 0).Active = true;
            QrNameView.RightAnchor.ConstraintEqualTo(QrNameView.Superview.RightAnchor, 0).Active = true;
            QrNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;


            lbl_QrName.BottomAnchor.ConstraintEqualTo(lbl_QrName.Superview.CenterYAnchor, -1).Active = true;
            lbl_QrName.LeftAnchor.ConstraintEqualTo(lbl_QrName.Superview.LeftAnchor, 20).Active = true;
            lbl_QrName.RightAnchor.ConstraintEqualTo(lbl_QrName.Superview.RightAnchor,-20).Active = true;
            lbl_QrName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txt_QrName.TopAnchor.ConstraintEqualTo(txt_QrName.Superview.CenterYAnchor, 1).Active = true;
            txt_QrName.LeftAnchor.ConstraintEqualTo(txt_QrName.Superview.LeftAnchor, 20).Active = true;
            txt_QrName.RightAnchor.ConstraintEqualTo(txt_QrName.Superview.RightAnchor, -20).Active = true;
            txt_QrName.HeightAnchor.ConstraintEqualTo(17).Active = true;
            #endregion

            #region QrBranchView
            QrBranchView.TopAnchor.ConstraintEqualTo(QrNameView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            QrBranchView.LeftAnchor.ConstraintEqualTo(QrBranchView.Superview.LeftAnchor, 0).Active = true;
            QrBranchView.RightAnchor.ConstraintEqualTo(QrBranchView.Superview.RightAnchor, 0).Active = true;
            QrBranchView.HeightAnchor.ConstraintEqualTo(60).Active = true;


            lbl_QrBranch.BottomAnchor.ConstraintEqualTo(lbl_QrBranch.Superview.CenterYAnchor, -1).Active = true;
            lbl_QrBranch.LeftAnchor.ConstraintEqualTo(lbl_QrBranch.Superview.LeftAnchor, 20).Active = true;
            lbl_QrBranch.RightAnchor.ConstraintEqualTo(lbl_QrBranch.Superview.RightAnchor, -20).Active = true;
            lbl_QrBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txt_QrBranch.TopAnchor.ConstraintEqualTo(txt_QrBranch.Superview.CenterYAnchor, 1).Active = true;
            txt_QrBranch.LeftAnchor.ConstraintEqualTo(txt_QrBranch.Superview.LeftAnchor, 20).Active = true;
            txt_QrBranch.RightAnchor.ConstraintEqualTo(txt_QrBranch.Superview.RightAnchor, -70).Active = true;
            txt_QrBranch.HeightAnchor.ConstraintEqualTo(17).Active = true;

            btnBranch.CenterYAnchor.ConstraintEqualTo(btnBranch.Superview.CenterYAnchor, 0).Active = true;
            btnBranch.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnBranch.RightAnchor.ConstraintEqualTo(btnBranch.Superview.RightAnchor, -30).Active = true;
            btnBranch.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #endregion

            #region QrImageView
            QrImageView.TopAnchor.ConstraintEqualTo(QrBranchView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            QrImageView.LeftAnchor.ConstraintEqualTo(QrImageView.Superview.LeftAnchor, 0).Active = true;
            QrImageView.RightAnchor.ConstraintEqualTo(QrImageView.Superview.RightAnchor, 0).Active = true;
            QrImageView.HeightAnchor.ConstraintEqualTo(60).Active = true;


            lbl_QrImage.BottomAnchor.ConstraintEqualTo(lbl_QrImage.Superview.CenterYAnchor, -1).Active = true;
            lbl_QrImage.LeftAnchor.ConstraintEqualTo(lbl_QrImage.Superview.LeftAnchor, 20).Active = true;
            lbl_QrImage.RightAnchor.ConstraintEqualTo(lbl_QrImage.Superview.RightAnchor, -20).Active = true;
            lbl_QrImage.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txt_QrImageName.TopAnchor.ConstraintEqualTo(txt_QrImageName.Superview.CenterYAnchor, 1).Active = true;
            txt_QrImageName.LeftAnchor.ConstraintEqualTo(txt_QrImageName.Superview.LeftAnchor, 20).Active = true;
            txt_QrImageName.RightAnchor.ConstraintEqualTo(txt_QrImageName.Superview.RightAnchor, -70).Active = true;
            txt_QrImageName.HeightAnchor.ConstraintEqualTo(17).Active = true;

            btnAddImg.CenterYAnchor.ConstraintEqualTo(btnAddImg.Superview.CenterYAnchor, 0).Active = true;
            btnAddImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnAddImg.RightAnchor.ConstraintEqualTo(btnAddImg.Superview.RightAnchor, -20).Active = true;
            btnAddImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #endregion

            #region CommentView
            CommentView.TopAnchor.ConstraintEqualTo(QrImageView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CommentView.LeftAnchor.ConstraintEqualTo(CommentView.Superview.LeftAnchor, 0).Active = true;
            CommentView.RightAnchor.ConstraintEqualTo(CommentView.Superview.RightAnchor, 0).Active = true;
            CommentView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            

            lbl_comment.TopAnchor.ConstraintEqualTo(lbl_comment.Superview.CenterYAnchor, -18).Active = true;
            lbl_comment.LeftAnchor.ConstraintEqualTo(lbl_comment.Superview.LeftAnchor, 20).Active = true;
            lbl_comment.RightAnchor.ConstraintEqualTo(lbl_comment.Superview.RightAnchor, -20).Active = true;
            lbl_comment.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txt_comment.TopAnchor.ConstraintEqualTo(lbl_comment.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txt_comment.LeftAnchor.ConstraintEqualTo(txt_comment.Superview.LeftAnchor, 20).Active = true;
            txt_comment.RightAnchor.ConstraintEqualTo(txt_comment.Superview.RightAnchor, -20).Active = true;
            txt_comment.HeightAnchor.ConstraintEqualTo(17).Active = true;
            #endregion

            #region ShowImageView
            ShowImageView.TopAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            ShowImageView.LeftAnchor.ConstraintEqualTo(ShowImageView.Superview.LeftAnchor, 0).Active = true;
            ShowImageView.RightAnchor.ConstraintEqualTo(ShowImageView.Superview.RightAnchor, 0).Active = true;
            ShowImageView.HeightAnchor.ConstraintEqualTo((nfloat)(View.Frame.Width * 1.1)).Active = true;
            ShowImageView.BottomAnchor.ConstraintEqualTo(ShowImageView.Superview.BottomAnchor).Active = true;

            showImg.TopAnchor.ConstraintEqualTo(showImg.Superview.TopAnchor, 0).Active = true;
            showImg.LeftAnchor.ConstraintEqualTo(showImg.Superview.LeftAnchor, 20).Active = true;
            showImg.RightAnchor.ConstraintEqualTo(showImg.Superview.RightAnchor, -20).Active = true;
            showImg.BottomAnchor.ConstraintEqualTo(showImg.Superview.BottomAnchor, 0).Active = true;
            #endregion

            

        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            
        }

    }
}