using CoreFoundation;
using Foundation;
//using Gabana.ORM.Master;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Merchant;
using Gabana3.JAM.PubSub;
using LinqToDB.SqlQuery;
using MetalPerformanceShaders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;


namespace Gabana.iOS
{
    public partial class MerchantSettingController : UIViewController
    {
        Uri keepCropedUri = null;
        private static byte[] picture;
        bool flagDetail = false;
        UIScrollView _scrollView;
        UIView _contentView;
        IJobQueue jobQueue;


        public string Phone = "";

        UIPickerView ProvincePicker, AmpurePicker, DistrictPicker;
        UIImagePickerController imagePicker;
        UIAlertController selectPhotoMenuSheet;
        UIButton btnChangeImage, btnSave, btndetailtoggle;
        UIImageView profileImg;
        UITextField txtMerchantName;
        UILabel lblMerchantName, lbltxtDetail;
        UIView logoView, line, MerchantView, bottomView, DetailToggleView, line3, RegistrationNoView;
        UIView TaxIDView, PhoneView, AddressView, ProvinceView, DistrictView, SubdistrictView, PostalCodeView, CommentView, LinkProView;
        UILabel lblTaxID, lblPhone, lblAddress, lblProvince, lblDistrict, lblSubDistrict, lblPostal, lblComment, lblLinkProMax, lblRegisNo;
        UITextField txtTaxID, txtPhone, txtAddress, txtProvince, txtDistrict, txtSubDistrict, txtPostal, txtComment, txtLinkProMax, txtRegisNo;
        UIImageView btnProvince, btnDistrict, btnSubDistrict, btnPostal;
        PoolManage poolManager = new PoolManage();
        public int idprovice;
        public int idamphure;
        public int iddistist;

        UIToolbar toolbar, toolbar3, toolbarDis;
        UIBarButtonItem flexible;
        UIBarButtonItem doneButtonAmpure, doneButton3, doneButton5;

        List<Amphure> amphure;
        List<District> distist;
        UIImage editedImage;
        bool reset = false;
        Branch BranchDetail;
        ORM.Master.Branch UpdatetBranch;
        Gabana3.JAM.Merchant.Merchants MerchantDetail;

        public List<Gabana.ORM.PoolDB.Province> province;
        private string NAME;
        private string TAXID;
        private string REG;
        private UIView Viewblock;
        private bool Editchange = false; 

        public MerchantSettingController()
        {
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            

        }
        public override async void WillMoveToParentViewController(UIViewController parent)
        {
           
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
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
            





                            var LoginType = Preferences.Get("LoginType", "");
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "merchant");
                this.NavigationController.SetNavigationBarHidden(false, false);

                Textboxfocus(View);
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                initAttribute();
                setupAutoLayout();
                province = new List<Province>();
                var provincedef = new Gabana.ORM.PoolDB.Province()
                {
                    ProvincesId = 0,
                    ProvincesNameEN = "Province",
                    ProvincesNameTH = "จังหวัด"
                };
                province.Add(provincedef);
                var provinceget = await poolManager.GetProvinces();
                province.AddRange(provinceget);
                DataCashingAll.Provinces = province;

                //setDetailMerchant();
                SetupPicker();
                setDetailMerchant();
            


                NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
                NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
                void OnKeyboardNotification(NSNotification notification)
                {
                    if (!IsViewLoaded) return;

                    //Check if the keyboard is becoming visible
                    var visible = notification.Name == UIKeyboard.WillShowNotification;

                    //Start an animation, using values from the keyboard
                    //UIView.BeginAnimations("AnimateForKeyboard");
                    //UIView.SetAnimationBeginsFromCurrentState(true);
                    UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
                    UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

                    //Pass the notification, calculating keyboard height, etc.
                    bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
                    var keyboardFrame = visible
                                            ? UIKeyboard.FrameEndFromNotification(notification)
                                            : UIKeyboard.FrameBeginFromNotification(notification);

                    OnKeyboardChanged(visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);

                    //Commit the animation
                    //UIView.CommitAnimations();
                }

                #region Spinner
                btnProvince.UserInteractionEnabled = true;
                var tapGesture2 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Province:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnProvince.AddGestureRecognizer(tapGesture2);

                btnDistrict.UserInteractionEnabled = true;
                var tapGesture3 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("District:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnDistrict.AddGestureRecognizer(tapGesture3);

                btnSubDistrict.UserInteractionEnabled = true;
                var tapGesture4 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("SybDis:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnSubDistrict.AddGestureRecognizer(tapGesture4);

                btnPostal.UserInteractionEnabled = true;
                var tapGesture5 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Potal:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnPostal.AddGestureRecognizer(tapGesture5);

                #endregion

                _contentView.BackgroundColor = UIColor.White;

                Checkper();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", "มีการเปลี่ยนแปลต้องการบันทึกหรือไม่", UIAlertControllerStyle.Alert);
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

        private void Checkper()
        {
            var LoginType = Preferences.Get("LoginType", "");
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "merchant");
            if (check)
            {

            }
            else
            {
                setDetailShow(false);
                View.Alpha = 0.9f;
                Viewblock = new UIView();
                Viewblock.TranslatesAutoresizingMaskIntoConstraints = false;
                Viewblock.BackgroundColor = UIColor.Clear;
                _contentView.AddSubview(Viewblock);
                _contentView.BringSubviewToFront(Viewblock);

                Viewblock.TopAnchor.ConstraintEqualTo(_contentView.TopAnchor, 0).Active = true;
                Viewblock.LeftAnchor.ConstraintEqualTo(_contentView.LeftAnchor, 0).Active = true;
                Viewblock.RightAnchor.ConstraintEqualTo(_contentView.RightAnchor, 0).Active = true;
                Viewblock.BottomAnchor.ConstraintEqualTo(_contentView.BottomAnchor, 0).Active = true;
                //btnDeleteView.UserInteractionEnabled = false;
                //btnAdd.Enabled = false;

            }
        }
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSave.Layer.CornerRadius = 5f;
                btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                btnSave.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSave.Layer.CornerRadius = 5f;
                btnSave.BackgroundColor = UIColor.White;
                btnSave.ClipsToBounds = true;
                btnSave.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSave.Layer.BorderWidth = 0.5f;
            }
            btnSave.Enabled = enable;
        }
        string addTextTel(string value)
        {
            try
            {
                if (value.Length > 0)
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 3 | i == 6)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }
                return value;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("addTextTel at Merchant");
                return value;
            }
        }
        #region Spinner

        [Export("Potal:")]
        public void Potal(UIGestureRecognizer sender)
        {
            txtPostal.BecomeFirstResponder();
        }
        [Export("SybDis:")]
        public void SybDis(UIGestureRecognizer sender)
        {
            txtSubDistrict.BecomeFirstResponder();
        }
        [Export("District:")]
        public void District(UIGestureRecognizer sender)
        {
            txtDistrict.BecomeFirstResponder();
        }
        [Export("Province:")]
        public void Province(UIGestureRecognizer sender)
        {
            txtProvince.BecomeFirstResponder();
        }
        #endregion
        async void setDetailMerchant()
        {
            try
            {
                BranchManage branchManage = new BranchManage();

                BranchDetail = await branchManage.GetBranch(DataCashingAll.MerchantId, 1); //สาขาสำนักงานใหญ่

                if (BranchDetail != null & BranchDetail.SysBranchID == 1)
                {
                    
                    if (!string.IsNullOrEmpty(BranchDetail.Tel))
                    {
                        txtPhone.Text = addTextTel(BranchDetail.Tel);
                    }
                    txtComment.Text = BranchDetail.Comments;
                    txtLinkProMax.Text = BranchDetail.LinkProMaxxID;
                    Phone = txtPhone.Text;
                    txtAddress.Text = BranchDetail.Address;
                    if (BranchDetail.ProvincesId > 0)
                    {
                        var pro = province.Where(x => x.ProvincesId == BranchDetail.ProvincesId).FirstOrDefault();
                        var index = province.FindIndex(x => x.ProvincesId == BranchDetail.ProvincesId);
                        txtProvince.Text = pro.ProvincesNameTH;
                        idprovice = (int)pro.ProvincesId;
                        setamphure((int)pro.ProvincesId, (int)BranchDetail.AmphuresId);
                        setdistist((int)BranchDetail.AmphuresId, (int)BranchDetail.DistrictsId);
                        ProvincePicker.Select(index, 0, false);

                    }
                }
                if (DataCashingAll.Merchant != null)
                {
                    NAME = DataCashingAll.Merchant.Merchant.Name;
                    TAXID = DataCashingAll.Merchant.Merchant.TaxID;
                    REG = DataCashingAll.Merchant.Merchant.RegMark;
                    
                }
                else
                {
                    NAME = DataCashingAll.MerchantLocal.Name;
                    TAXID = DataCashingAll.MerchantLocal.TaxID;
                    REG = DataCashingAll.MerchantLocal.RegMark;
                    
                }

                txtMerchantName.Text = NAME;
                txtTaxID.Text = TAXID;
                txtRegisNo.Text = REG;
                SetButtonAdd(false);
                ShareSource.Manage.MerchantManage merchantManage = new ShareSource.Manage.MerchantManage();
                var merchantlocal = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                var path = merchantlocal.LogoLocalPath;
                if (!string.IsNullOrEmpty(path))
                {
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    Utils.SetImage(profileImg, Path.Combine(docFolder, path));
                }
                else
                {
                    profileImg.Image = UIImage.FromFile("LogoDefault.png");
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        async void UpdateDetailCustomer()
        {
            try
            {
                #region local image
                MerchantManage merchantManage = new MerchantManage();
                //  var merchant = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                var merchant = MainController.merchantlocal;

                if (!string.IsNullOrEmpty(merchant?.LogoLocalPath))
                {

                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    Utils.SetImage(profileImg, Path.Combine(docFolder, merchant?.LogoLocalPath));
                }

                #endregion

                txtMerchantName.Text = MerchantDetail.Merchant.Name;
                txtTaxID.Text = MerchantDetail.Merchant.TaxID;
                txtRegisNo.Text = MerchantDetail.Merchant.RegMark;

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        void setDetailShow(bool set)
        {
            if (set)
            {
                _contentView.BackgroundColor = UIColor.White;
                LinkProView.Hidden = true;
                CommentView.Hidden = true;
                PostalCodeView.Hidden = true;
                SubdistrictView.Hidden = true;
                DistrictView.Hidden = true;
                ProvinceView.Hidden = true;
                //----------------------------
                AddressView.Hidden = true;
                TaxIDView.Hidden = true;
                RegistrationNoView.Hidden = true;
                PhoneView.Hidden = true;
            }
            else
            {
                _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                LinkProView.Hidden = false;
                CommentView.Hidden = false;
                PostalCodeView.Hidden = false;
                SubdistrictView.Hidden = false;
                DistrictView.Hidden = false;
                ProvinceView.Hidden = false;
                //---------------------------------------
                TaxIDView.Hidden = false;
                PhoneView.Hidden = false;
                AddressView.Hidden = false;
                RegistrationNoView.Hidden = false;
            }
        }
        public class PickerprovinceModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;


            private readonly List<Gabana.ORM.PoolDB.Province> values;
            public PickerprovinceModel(List<Gabana.ORM.PoolDB.Province> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)].ProvincesNameTH;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].ProvincesNameTH,
                        ID = (int)values[Convert.ToInt32(row)].ProvincesId
                    });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }

        public class PickerAmphureModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;


            private readonly List<Gabana.ORM.PoolDB.Amphure> values;
            public PickerAmphureModel(List<Gabana.ORM.PoolDB.Amphure> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)].AmphuresNameTH;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].AmphuresNameTH,
                        ID = (int)values[Convert.ToInt32(row)].AmphuresId
                    });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        public class PickerDistrictModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
                public string ZipCode { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;


            private readonly List<Gabana.ORM.PoolDB.District> values;
            public PickerDistrictModel(List<Gabana.ORM.PoolDB.District> values)
            {
                this.values = values;
            }

            public override nint GetComponentCount(UIPickerView v)
            {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return values.Count;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component)
            {
                return values[Convert.ToInt32(row)].DistrictsNameTH;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].DistrictsNameTH,
                        ID = (int)values[Convert.ToInt32(row)].DistrictsId,
                        ZipCode = values[Convert.ToInt32(row)].ZipCode
                    });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }
        }

        private async void SetupPicker()
        {
            try
            {
                #region Province
                UIToolbar toolbar3 = new UIToolbar();
                toolbar3.Translucent = true;
                toolbar3.SizeToFit();
                var doneButton3 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    View.EndEditing(true);
                    txtDistrict.BecomeFirstResponder();
                });
                toolbar3.SetItems(new UIBarButtonItem[] { flexible, doneButton3 }, true);

                PickerprovinceModel provinceM = new PickerprovinceModel(province);
                provinceM.PickerChanged += async (sender, e) => {

                    txtProvince.Text = e.SelectedValue;
                    reset = true;
                    setamphure(e.ID, 0);
                    idprovice = e.ID;
                    txtDistrict.Enabled = true;

                    Editchange = true;
                };
                ProvincePicker = new UIPickerView() { Model = provinceM, ShowSelectionIndicator = true };
                txtProvince.InputView = ProvincePicker;
                txtProvince.InputAccessoryView = toolbar3;
                txtProvince.Text = Utils.TextBundle("province", "Province");
                //provinceM.Selected(ProvincePicker, 0, 0);


                var amphure = new List<Amphure>() { new Amphure() { AmphuresId = 0, AmphuresNameEN = "province", AmphuresNameTH = "อำเภอ" } };

                PickerAmphureModel model2 = new PickerAmphureModel(amphure);
                txtSubDistrict.Enabled = true;
                idamphure = (int)amphure[0].AmphuresId;
                model2.PickerChanged += async (sender, e) =>
                {
                    if (e.ID != 0)
                    {
                        reset = true;
                        txtDistrict.Text = e.SelectedValue;
                        setdistist(e.ID, 0);
                        txtSubDistrict.Enabled = true;
                        idamphure = e.ID;
                    }
                    else
                    {
                        setdistist(e.ID, 0);
                        txtSubDistrict.Enabled = false;
                        DistrictPicker.Select(0, 0, false);
                        AmpurePicker.Select(0, 0, false);
                    }
                    Editchange = true;
                };

                UIToolbar toolbar1 = new UIToolbar();
                toolbar1.Translucent = true;
                toolbar1.SizeToFit();
                var flexible1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
                var doneButton1 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    View.EndEditing(true);
                    txtSubDistrict.BecomeFirstResponder();
                });
                toolbar1.SetItems(new UIBarButtonItem[] { flexible1, doneButton1 }, true);
                AmpurePicker = new UIPickerView() { Model = model2, ShowSelectionIndicator = true };
                txtDistrict.InputView = AmpurePicker;
                txtDistrict.InputAccessoryView = toolbar1;
                txtDistrict.Text = Utils.TextBundle("subdistrict", "Sub District");


                var distist = new List<District>() { new District() { DistrictsId = 0, DistrictsNameEN = "Sub District", DistrictsNameTH = "ตำบล" } };


                PickerDistrictModel model3 = new PickerDistrictModel(distist);
                model3.PickerChanged += async (sender, e) =>
                {
                    if (e.ID != 0)
                    {
                        txtSubDistrict.Text = e.SelectedValue;
                        iddistist = e.ID;
                        txtPostal.Text = e.ZipCode;
                    }
                    else
                    {
                        //txtSubDistrict.Enabled = false;

                        AmpurePicker.Select(0, 0, false);
                    }
                    Editchange = true;
                };
                UIToolbar toolbar5 = new UIToolbar();
                toolbar5.Translucent = true;
                toolbar5.SizeToFit();
                var flexible5 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
                var doneButton5 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    View.EndEditing(true);
                });
                toolbar5.SetItems(new UIBarButtonItem[] { flexible5, doneButton5 }, true);
                DistrictPicker = new UIPickerView() { Model = model3, ShowSelectionIndicator = true };
                txtSubDistrict.InputView = DistrictPicker;
                txtSubDistrict.InputAccessoryView = toolbar5;
                txtSubDistrict.Text = Utils.TextBundle("subdistrict", "Sub District");

                //if (province != null)
                //{
                //    if (Customer == null || Customer.ProvincesId == null || Customer.ProvincesId == 0)
                //    {
                //        txtProvince.Text = province[0].ProvincesNameTH;
                //        setamphure((int)province[0].ProvincesId);
                //        ProvincePicker.Select(0, 0, false);
                //    }
                //    else
                //    {
                //        txtProvince.Text = province.Where(c => c.ProvincesId == Customer.ProvincesId).FirstOrDefault().ProvincesNameTH;
                //        var index = province.FindIndex(c => c.ProvincesId == Customer.ProvincesId);
                //        idprovice = (int)province.Where(c => c.ProvincesId == Customer.ProvincesId).FirstOrDefault().ProvincesId;
                //        setamphure(idprovice);
                //        ProvincePicker.Select((int)index, 0, false);
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private async void setamphure(int id, int idamp)
        {
            try
            {
                var amphure = new List<Amphure>() { new Amphure() { AmphuresId = 0, AmphuresNameEN = "District", AmphuresNameTH = "อำเภอ" } };
                if (id != 0)
                {
                    amphure.AddRange(await poolManager.GetAmphures(Convert.ToInt32(id)));
                }

                PickerAmphureModel model2 = new PickerAmphureModel(amphure);
                txtSubDistrict.Enabled = true;
                idamphure = (int)amphure[0].AmphuresId;
                model2.PickerChanged += async (sender, e) =>
                {
                    reset = true;
                    txtDistrict.Text = e.SelectedValue;
                    setdistist(e.ID, 0);
                    txtSubDistrict.Enabled = true;
                    idamphure = e.ID;
                };

                UIToolbar toolbar = new UIToolbar();
                toolbar.Translucent = true;
                toolbar.SizeToFit();
                var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    View.EndEditing(true);
                    txtSubDistrict.BecomeFirstResponder();
                });
                toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
                AmpurePicker = new UIPickerView() { Model = model2, ShowSelectionIndicator = true };
                txtDistrict.InputView = AmpurePicker;
                txtDistrict.InputAccessoryView = toolbar;
                txtDistrict.Text = amphure[0].AmphuresNameTH;
                idamphure = 0;

                if (idamp > 0)
                {
                    var index = amphure.FindIndex(x => x.AmphuresId == idamp);
                    txtDistrict.Text = amphure[index].AmphuresNameTH;
                    idamphure = (int)amphure[index].AmphuresId;
                    AmpurePicker.Select(index, 0, false);
                }
                //if (amphure != null)
                //{
                //    if (reset == true)
                //    {
                //        txtDistrict.Text = amphure[0].AmphuresNameTH;
                //        AmpurePicker.Select(0, 0, false);
                //        setdistist((int)amphure[0].AmphuresId);
                //        return;
                //    }
                //    if (Customer == null || Customer.AmphuresId == null || Customer.AmphuresId ==0)
                //    {
                //        txtDistrict.Text = amphure[0].AmphuresNameTH;
                //        AmpurePicker.Select(0, 0, false);
                //        setdistist((int)amphure[0].AmphuresId);
                //    }
                //    else
                //    {
                //        var x = amphure.Where(c => c.AmphuresId == Customer.AmphuresId).FirstOrDefault();
                //        txtDistrict.Text = x.AmphuresNameTH;
                //        var index = amphure.FindIndex(c => c.AmphuresId == Customer.AmphuresId);
                //        idamphure = (int)amphure.Where(c => c.AmphuresId == Customer.AmphuresId).FirstOrDefault().AmphuresId;
                //        setdistist(idamphure);
                //        AmpurePicker.Select((int)index, 0, false);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
        private async void setdistist(int id, int iddis)
        {
            try
            {
                var distist = new List<District>() { new District() { DistrictsId = 0, DistrictsNameEN = "Sub District", DistrictsNameTH = "ตำบล" } };
                if (id != 0)
                {
                    distist.AddRange(await poolManager.GetDistricts(Convert.ToInt32(id)));
                }

                PickerDistrictModel model3 = new PickerDistrictModel(distist);
                model3.PickerChanged += async (sender, e) =>
                {
                    txtSubDistrict.Text = e.SelectedValue;
                    iddistist = e.ID;
                    txtPostal.Text = e.ZipCode;
                };
                UIToolbar toolbar = new UIToolbar();
                toolbar.Translucent = true;
                toolbar.SizeToFit();
                var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    View.EndEditing(true);
                });
                toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
                DistrictPicker = new UIPickerView() { Model = model3, ShowSelectionIndicator = true };
                txtSubDistrict.InputView = DistrictPicker;
                txtSubDistrict.InputAccessoryView = toolbar;
                txtPostal.Text = distist[0].ZipCode;
                txtSubDistrict.Text = distist[0].DistrictsNameTH;
                iddistist = 0;

                if (iddis > 0)
                {
                    var index = distist.FindIndex(x => x.DistrictsId == iddis);
                    txtPostal.Text = distist[index].ZipCode;
                    txtSubDistrict.Text = distist[index].DistrictsNameTH;
                    iddistist = (int)distist[index].DistrictsId;
                    DistrictPicker.Select(index, 0, false);
                }
                //if (distist != null)
                //{
                //    if (reset == true)
                //    {
                //        txtPostal.Text = distist[0].ZipCode;
                //        txtSubDistrict.Text = distist[0].DistrictsNameTH;
                //        DistrictPicker.Select(0, 0, false);
                //        reset = false;
                //        return;
                //    }
                //    if (Customer == null || Customer.DistrictsId == null || Customer.DistrictsId == 0)
                //    {
                //        txtPostal.Text = distist[0].ZipCode;
                //        txtSubDistrict.Text = distist[0].DistrictsNameTH;
                //        DistrictPicker.Select(0, 0, false);
                //        reset = false;
                //    }
                //    else
                //    {
                //        //id
                //        txtSubDistrict.Text = distist.Where(c => c.DistrictsId == Customer.DistrictsId).FirstOrDefault().DistrictsNameTH;
                //        var index = distist.FindIndex(c => c.DistrictsId == Customer.DistrictsId);
                //        iddistist = (int)distist.Where(c => c.DistrictsId == Customer.DistrictsId).FirstOrDefault().DistrictsId;
                //        txtPostal.Text = distist[index].ZipCode;
                //        DistrictPicker.Select((int)index, 0, false);
                //    }
                //}
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        //private async void setamphure(int id)
        //{
        //    try
        //    {
        //        amphure = await poolManager.GetAmphures(id);
        //        PickerAmphureModel model2 = new PickerAmphureModel(amphure);
        //        txtDistrict.Enabled = true;
        //        model2.PickerChanged += async (sender, e) =>
        //        {
        //            reset = true;
        //            txtDistrict.Text = e.SelectedValue;
        //            setdistist(e.ID);
        //            txtSubDistrict.Enabled = true;
        //            idamphure = e.ID;
        //        };

        //        AmpurePicker = new UIPickerView() { Model = model2, ShowSelectionIndicator = true };
        //        txtDistrict.InputView = AmpurePicker;
        //        txtDistrict.InputAccessoryView = toolbar;

        //        if (amphure != null)
        //        {
        //            if(reset == true)
        //            {
        //                txtDistrict.Text = amphure[0].AmphuresNameTH;
        //                AmpurePicker.Select(0, 0, false);
        //                setdistist((int)amphure[0].AmphuresId);
        //                return;
        //            }
        //            if ((BranchDetail == null || BranchDetail.AmphuresId == null))
        //            {
        //                txtDistrict.Text = amphure[0].AmphuresNameTH;
        //                AmpurePicker.Select(0, 0, false);
        //                setdistist((int)amphure[0].AmphuresId);
        //            }
        //            else
        //            {
        //                var x = amphure.Where(c => c.AmphuresId == BranchDetail.AmphuresId).FirstOrDefault();
        //                txtDistrict.Text = x.AmphuresNameTH;
        //                var index = amphure.FindIndex(c => c.AmphuresId == BranchDetail.AmphuresId);
        //                idamphure = (int)amphure.Where(c => c.AmphuresId == BranchDetail.AmphuresId).FirstOrDefault().AmphuresId;
        //                setdistist(idamphure);
        //                AmpurePicker.Select((int)index, 0, false);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        await TinyInsights.TrackErrorAsync(ex);
        //        return;
        //    }
        //}
        //private async void setdistist(int id)
        //{
        //    //sub district
        //    try
        //    {
        //        distist = await poolManager.GetDistricts(id);

        //        PickerDistrictModel model3 = new PickerDistrictModel(distist);;

        //        model3.PickerChanged += async (sender, e) =>
        //        {
        //            txtSubDistrict.Text = e.SelectedValue;
        //            iddistist = e.ID;
        //            txtPostal.Text = e.ZipCode;
        //        };

        //        DistrictPicker = new UIPickerView() { Model = model3, ShowSelectionIndicator = true };
        //        txtSubDistrict.InputView = DistrictPicker;
        //        txtSubDistrict.InputAccessoryView = toolbarDis;

        //        if (distist != null)
        //        {
        //            if (reset == true)
        //            {
        //                txtPostal.Text = distist[0].ZipCode;
        //                txtSubDistrict.Text = distist[0].DistrictsNameTH;
        //                DistrictPicker.Select(0, 0, false);
        //                reset = false;
        //                return;
        //            }
        //            if (BranchDetail == null || BranchDetail.DistrictsId == null )
        //            {
        //                txtPostal.Text = distist[0].ZipCode;
        //                txtSubDistrict.Text = distist[0].DistrictsNameTH;
        //                DistrictPicker.Select(0, 0, false);
        //                reset = false;
        //            }
        //            else
        //            {
        //                //id
        //                txtSubDistrict.Text = distist.Where(c => c.DistrictsId == BranchDetail.DistrictsId).FirstOrDefault().DistrictsNameTH;
        //                var index = distist.FindIndex(c => c.DistrictsId == BranchDetail.DistrictsId);
        //                iddistist = (int)distist.Where(c => c.DistrictsId == BranchDetail.DistrictsId).FirstOrDefault().DistrictsId;
        //                txtPostal.Text = distist[index].ZipCode;
        //                DistrictPicker.Select((int)index, 0, false);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await TinyInsights.TrackErrorAsync(ex);
        //        return;
        //    }

        //}
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region LayoutAtrribute

            #region LogoView
            logoView = new UIView();
            logoView.BackgroundColor = UIColor.White;
            logoView.TranslatesAutoresizingMaskIntoConstraints = false;


            profileImg = new UIImageView();
            profileImg.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            profileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            profileImg.Image = UIImage.FromFile("LogoDefault.png");
            profileImg.Layer.CornerRadius = 75;
            profileImg.ClipsToBounds = true;
            logoView.AddSubview(profileImg);


            btnChangeImage = new UIButton();
            btnChangeImage.Layer.CornerRadius = 25;
            btnChangeImage.SetImage(UIImage.FromBundle("AddImg"), UIControlState.Normal);
            btnChangeImage.ImageView.ContentMode = UIViewContentMode.ScaleToFill;
            btnChangeImage.TranslatesAutoresizingMaskIntoConstraints = false;
            btnChangeImage.TouchUpInside += (sender, e) => {
                // change image
                //selectPhotoMenuSheet.ShowInView(View);
                #region PhotoEditActionSheet

                selectPhotoMenuSheet = UIAlertController.Create(Utils.TextBundle("addlogo", "Add Logo"), null, UIAlertControllerStyle.ActionSheet);
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("takepic", "Take a picture"), UIAlertActionStyle.Default,
                                                alert => Pic("Take")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("choosepic", "Choose your picture"), UIAlertActionStyle.Default,
                                                alert => Pic("Choose")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

                // Show the alert
                this.PresentViewController(selectPhotoMenuSheet, true, null);
                #endregion
            };
            logoView.AddSubview(btnChangeImage);

            #endregion

            line = new UIView();
            line.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(line);

            #region MerchantView
            MerchantView = new UIView();
            MerchantView.BackgroundColor = UIColor.White;
            MerchantView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblMerchantName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblMerchantName.Font = lblMerchantName.Font.WithSize(15);
            lblMerchantName.Text = Utils.TextBundle("merchantname", "Merchant Name");
            MerchantView.AddSubview(lblMerchantName);

            txtMerchantName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtMerchantName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("merchantname", "Merchant Name"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtMerchantName.ReturnKeyType = UIReturnKeyType.Done;
            txtMerchantName.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtMerchantName.EditingChanged += TxtMerchantName_EditingChanged;
            txtMerchantName.Font = txtMerchantName.Font.WithSize(15);
            MerchantView.AddSubview(txtMerchantName);
            #endregion

            #region DetailToggleView
            DetailToggleView = new UIView();
            DetailToggleView.BackgroundColor = UIColor.White;
            DetailToggleView.TranslatesAutoresizingMaskIntoConstraints = false;


            lbltxtDetail = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lbltxtDetail.Font = lbltxtDetail.Font.WithSize(15);
            lbltxtDetail.Text = Utils.TextBundle("details", "Details");
            DetailToggleView.AddSubview(lbltxtDetail);

            btndetailtoggle = new UIButton();
            btndetailtoggle.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
            btndetailtoggle.TranslatesAutoresizingMaskIntoConstraints = false;
            btndetailtoggle.TouchUpInside += (sender, e) =>
            {
                //Detail Not Show
                if (flagDetail)
                {
                    btndetailtoggle.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                    setDetailShow(true);
                    flagDetail = false;
                }
                else
                {
                    btndetailtoggle.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                    setDetailShow(false);
                    flagDetail = true;
                }
            };
            DetailToggleView.AddSubview(btndetailtoggle);
            #endregion

            line3 = new UIView();
            line3.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            line3.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(line3);

            #region DetailsView
            UIToolbar NumpadToolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, (float)View.Frame.Width, 44.0f));
            NumpadToolbar.Translucent = true;
            NumpadToolbar.Items = new UIBarButtonItem[]{
            new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                 View.EndEditing(true);
            })
            };
            NumpadToolbar.SizeToFit();

            #region TaxIDView
            TaxIDView = new UIView();
            TaxIDView.Hidden = true;
            TaxIDView.BackgroundColor = UIColor.White;
            TaxIDView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblTaxID = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblTaxID.Font = lblTaxID.Font.WithSize(15);
            lblTaxID.Text = Utils.TextBundle("taxid", "Tax ID");
            TaxIDView.AddSubview(lblTaxID);

            txtTaxID = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtTaxID.EditingChanged += TxtMerchantName_EditingChanged;
            txtTaxID.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("taxid", "Tax ID"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtTaxID.Font = txtTaxID.Font.WithSize(15);
            txtTaxID.ReturnKeyType = UIReturnKeyType.Next;
            txtTaxID.ShouldReturn = (tf) =>
            {
                txtRegisNo.BecomeFirstResponder();
                return true;
            };
            txtTaxID.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 13;
            };
            txtTaxID.InputAccessoryView = NumpadToolbar;
            txtTaxID.KeyboardType = UIKeyboardType.NumberPad;
            TaxIDView.AddSubview(txtTaxID);
            #endregion

            #region RegistrationNoView
            RegistrationNoView = new UIView();
            RegistrationNoView.Hidden = true;
            RegistrationNoView.BackgroundColor = UIColor.White;
            RegistrationNoView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblRegisNo = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblRegisNo.Font = lblRegisNo.Font.WithSize(15);
            lblRegisNo.Text = Utils.TextBundle("registrationno", "Registration No");
            RegistrationNoView.AddSubview(lblRegisNo);

            txtRegisNo = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtRegisNo.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("registrationno", "Registration No"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtRegisNo.Font = txtRegisNo.Font.WithSize(15);
            txtRegisNo.ReturnKeyType = UIReturnKeyType.Next;
            txtRegisNo.EditingChanged += TxtMerchantName_EditingChanged;
            txtRegisNo.ShouldReturn = (tf) =>
            {
                txtPhone.BecomeFirstResponder();
                return true;
            };
            txtRegisNo.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 13;
            };
            txtRegisNo.InputAccessoryView = NumpadToolbar;
            txtRegisNo.KeyboardType = UIKeyboardType.NumberPad;
            RegistrationNoView.AddSubview(txtRegisNo);
            #endregion

            #region PhoneView
            PhoneView = new UIView();
            PhoneView.Hidden = true;
            PhoneView.BackgroundColor = UIColor.White;
            PhoneView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblPhone = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblPhone.Font = lblPhone.Font.WithSize(15);
            lblPhone.Text = Utils.TextBundle("phonenumber", "Phone Number");
            PhoneView.AddSubview(lblPhone);

            txtPhone = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtPhone.EditingChanged += TxtMerchantName_EditingChanged;
            txtPhone.AttributedPlaceholder = new NSAttributedString("000-000-0000", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtPhone.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 12;
            };
            txtPhone.EditingChanged += (object sender, EventArgs e) =>
            {
                try
                {
                    Phone = txtPhone.Text;
                    int textlength = txtPhone.Text.Length;

                    if (Phone.EndsWith(" "))
                        return;

                    if (textlength == 4)
                    {
                        var index = txtPhone.Text.LastIndexOf("-");
                        if (textlength == 4 & index == 3)
                        {
                            Phone.Remove(3, 1);
                        }
                        else
                        {
                            txtPhone.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                        }
                    }
                    else if (textlength == 8)
                    {
                        var index = txtPhone.Text.LastIndexOf("-");
                        if (textlength == 8 & index == 7)
                        {
                            Phone.Remove(7, 1);
                        }
                        else
                        {
                            txtPhone.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                        }
                    }
                }
                catch (Exception ex )
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                    return;
                }
            };


            txtPhone.Font = txtPhone.Font.WithSize(15);
            txtPhone.InputAccessoryView = NumpadToolbar;
            txtPhone.KeyboardType = UIKeyboardType.NumberPad;
            PhoneView.AddSubview(txtPhone);
            #endregion

            #region AddressView

            AddressView = new UIView();
            AddressView.Hidden = true;
            AddressView.BackgroundColor = UIColor.White;
            AddressView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblAddress = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblAddress.Font = lblAddress.Font.WithSize(15);
            lblAddress.Text = Utils.TextBundle("detailedaddress", "Detailed Address");
            AddressView.AddSubview(lblAddress);

            txtAddress = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtAddress.EditingChanged += TxtMerchantName_EditingChanged;
            txtAddress.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("address", "Address"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtAddress.Font = txtAddress.Font.WithSize(15);
            txtAddress.ReturnKeyType = UIReturnKeyType.Next;
            txtAddress.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                txtProvince.BecomeFirstResponder();
                return true;
            };
            AddressView.AddSubview(txtAddress);
            #endregion

            #region ProvinceView
            ProvinceView = new UIView();
            ProvinceView.Hidden = true;
            ProvinceView.BackgroundColor = UIColor.White;
            ProvinceView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblProvince = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblProvince.Font = lblProvince.Font.WithSize(15);
            lblProvince.Text = Utils.TextBundle("province", "Province");
            ProvinceView.AddSubview(lblProvince);

            txtProvince = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtProvince.TintColor = UIColor.Clear;
            txtProvince.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("province", "Province"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtProvince.Font = txtProvince.Font.WithSize(15);
            ProvinceView.AddSubview(txtProvince);

            btnProvince = new UIImageView();
            btnProvince.Image = UIImage.FromBundle("Next");
            btnProvince.TranslatesAutoresizingMaskIntoConstraints = false;
            ProvinceView.AddSubview(btnProvince);
            #endregion

            #region DistrictView
            DistrictView = new UIView();
            DistrictView.Hidden = true;
            DistrictView.BackgroundColor = UIColor.White;
            DistrictView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblDistrict = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDistrict.Font = lblDistrict.Font.WithSize(15);
            lblDistrict.Text = Utils.TextBundle("district", "District");
            DistrictView.AddSubview(lblDistrict);

            txtDistrict = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtDistrict.TintColor = UIColor.Clear;
            txtDistrict.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("district", "District"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            DistrictView.AddSubview(txtDistrict);

            btnDistrict = new UIImageView();
            btnDistrict.Image = UIImage.FromBundle("Next");
            btnDistrict.TranslatesAutoresizingMaskIntoConstraints = false;
            DistrictView.AddSubview(btnDistrict);
            #endregion

            #region SubdistrictView
            SubdistrictView = new UIView();
            SubdistrictView.Hidden = true;
            SubdistrictView.BackgroundColor = UIColor.White;
            SubdistrictView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblSubDistrict = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSubDistrict.Font = lblSubDistrict.Font.WithSize(15);
            lblSubDistrict.Text = Utils.TextBundle("subdistrict", "Sub District");
            SubdistrictView.AddSubview(lblSubDistrict);

            txtSubDistrict = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtSubDistrict.TintColor = UIColor.Clear;
            txtSubDistrict.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("selectpostalcode", "Select Sub District"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtSubDistrict.Font = txtSubDistrict.Font.WithSize(15);
            SubdistrictView.AddSubview(txtSubDistrict);

            btnSubDistrict = new UIImageView();
            btnSubDistrict.Image = UIImage.FromBundle("Next");
            btnSubDistrict.TranslatesAutoresizingMaskIntoConstraints = false;
            SubdistrictView.AddSubview(btnSubDistrict);
            #endregion

            #region PostalCodeView
            PostalCodeView = new UIView();
            PostalCodeView.Hidden = true;
            PostalCodeView.BackgroundColor = UIColor.White;
            PostalCodeView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblPostal = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblPostal.Font = lblPostal.Font.WithSize(15);
            lblPostal.Text = Utils.TextBundle("postalcode", "Postal Code");
            PostalCodeView.AddSubview(lblPostal);

            txtPostal = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtPostal.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("selectpostalcode", "Select Postal Code"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtPostal.Font = txtPostal.Font.WithSize(15);
            PostalCodeView.AddSubview(txtPostal);

            btnPostal = new UIImageView();
            btnPostal.Image = UIImage.FromBundle("Next");
            btnPostal.TranslatesAutoresizingMaskIntoConstraints = false;
            PostalCodeView.AddSubview(btnPostal);
            #endregion

            #region CommentView
            CommentView = new UIView();
            CommentView.Hidden = true;
            CommentView.BackgroundColor = UIColor.White;
            CommentView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblComment = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblComment.Font = lblComment.Font.WithSize(15);
            lblComment.Text = Utils.TextBundle("comment", "Comment");
            CommentView.AddSubview(lblComment);

            txtComment = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtComment.EditingChanged += TxtMerchantName_EditingChanged;
            txtComment.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("comment", "Comment"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtComment.Font = txtComment.Font.WithSize(15);
            txtComment.ReturnKeyType = UIReturnKeyType.Done;
            txtComment.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            CommentView.AddSubview(txtComment);
            #endregion

            #region LinkProView
            LinkProView = new UIView();
            LinkProView.Hidden = true;
            LinkProView.BackgroundColor = UIColor.White;
            LinkProView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblLinkProMax = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblLinkProMax.Font = lblLinkProMax.Font.WithSize(15);
            lblLinkProMax.Text = Utils.TextBundle("linkpromaxxid", "Link ProMaxx ID");
            LinkProView.AddSubview(lblLinkProMax);

            txtLinkProMax = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtLinkProMax.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("linkpromaxxid", "Link ProMaxx ID"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtLinkProMax.Font = txtLinkProMax.Font.WithSize(15);
            txtLinkProMax.Enabled = false;

            LinkProView.AddSubview(txtLinkProMax);
            #endregion

            #endregion

            #region BottomView
            bottomView = new UIView();
            bottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;


            btnSave = new UIButton();
            btnSave.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += (sender, e) => {
                Editchange =false;
                BtnSave_Click();
            };
            bottomView.AddSubview(btnSave);
            #endregion


            #endregion


            _contentView.AddSubview(logoView);
            _contentView.AddSubview(DetailToggleView);
            _contentView.AddSubview(LinkProView);
            _contentView.AddSubview(CommentView);
            _contentView.AddSubview(PostalCodeView);
            _contentView.AddSubview(SubdistrictView);
            _contentView.AddSubview(DistrictView);
            _contentView.AddSubview(ProvinceView);
            _contentView.AddSubview(AddressView);
            _contentView.AddSubview(RegistrationNoView);
            _contentView.AddSubview(PhoneView);
            _contentView.AddSubview(TaxIDView);
            _contentView.AddSubview(MerchantView);

            _scrollView.AddSubview(_contentView);
            View.AddSubview(bottomView);
            View.AddSubview(_scrollView);
            bottomView.BringSubviewToFront(btnSave);

            #region toolbar
            flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);

            toolbar3 = new UIToolbar();
            toolbar3.Translucent = true;
            toolbar3.SizeToFit();
            doneButton3 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
                txtDistrict.BecomeFirstResponder();
            });
            toolbar3.SetItems(new UIBarButtonItem[] { flexible, doneButton3 }, true);

            toolbar = new UIToolbar();
            toolbar.Translucent = true;
            toolbar.SizeToFit();

            doneButtonAmpure = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
                txtSubDistrict.BecomeFirstResponder();
            });
            toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButtonAmpure }, true);

            toolbarDis = new UIToolbar();
            toolbarDis.Translucent = true;
            toolbarDis.SizeToFit();
            doneButton5 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            });
            toolbarDis.SetItems(new UIBarButtonItem[] { flexible, doneButton5 }, true);

            #endregion
        }

        private void TxtMerchantName_EditingChanged(object sender, EventArgs e)
        {
            Editchange = true;
            if (txtMerchantName.Text != NAME || txtTaxID.Text != TAXID || txtRegisNo.Text != REG
                || txtPhone.Text != BranchDetail.Tel || txtAddress.Text != BranchDetail.Address )
            {
                SetButtonAdd(true);
                return;
            }
            SetButtonAdd(false);
            
        }

        private void BtnSave_Click()
        {
            if (DataCashingAll.CheckConnectInternet)
            {
                EditMerchantConfig();
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("nointernet", "No Internet,Please Connect"));
            }
        }
       
        private async Task BtnSave_Click3()
        {
            if (DataCashingAll.CheckConnectInternet)
            {
                EditMerchantConfig();
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("nointernet", "No Internet,Please Connect"));
            }
        }
        async void EditMerchantConfig()
        {
            try
            {
                GabanaLoading.SharedInstance.Show(this);
                var usernamelogin = Preferences.Get("User", "");

                Gabana3.JAM.Merchant.Merchants PutMerchant = new Gabana3.JAM.Merchant.Merchants();

                MerchantManage merchantManage = new MerchantManage();

                //Merchant
                Gabana.ORM.Master.Merchant merchants = new ORM.Master.Merchant();
                merchants = MainController.merchant.Merchant;

                byte[] imageByteArray = null;
                if (editedImage != null)
                {
                    imageByteArray = Utils.ReadFully(editedImage.AsJPEG().AsStream());
                }

                merchants.Name = txtMerchantName.Text;
                merchants.TaxID = txtTaxID.Text;
                merchants.RegMark = txtRegisNo.Text;

               
                var resultBranch = await UpdatetBranchDetail();
                if (resultBranch)
                {
                    PutMerchant.Merchant = merchants;

                    var updatemerchant = await GabanaAPI.PutMerchant(PutMerchant, imageByteArray);
                    if (updatemerchant.Status)
                    {
                        
                        DataCashingAll.Merchant.Merchant = merchants;

                        //insert to local
                        //merchant

                        Merchant merchant = new Merchant()
                        {
                            MerchantID = merchants.MerchantID,
                            Name = merchants.Name,
                            FMasterMerchant = merchants.FMasterMerchant,
                            RefMasterMerchantID = merchants.MerchantID,
                            Status = merchants.Status,
                            DateOpenMerchant = merchants.DateOpenMerchant,
                            RefPackageID = merchants.RefPackageID,
                            DayOfPeriod = merchants.DayOfPeriod,
                            DueDate = merchants.DueDate,
                            LanguageCountryCode = merchants.LanguageCountryCode,
                            TimeZoneName = merchants.TimeZoneName,
                            TimeZoneUTCOffset = merchants.TimeZoneUTCOffset,
                            LogoPath = merchants.LogoPath,
                            DateCreated = merchants.DateCreated,
                            DateModified = merchants.DateModified,
                            UserNameModified = merchants.UserNameModified,
                            DateCloseMerchant = merchants.DateCloseMerchant,
                            FPrivacyPolicy = merchants.FPrivacyPolicy,
                            FTermConditions = merchants.FTermConditions,
                            RegMark = merchants.RegMark,
                            TaxID = merchants.TaxID
                        };
                        if (editedImage == null)
                        {
                            var data = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                            if (data != null)
                            {
                                merchant.LogoLocalPath = data.LogoLocalPath; 
                            }
                        }
                        var result = await merchantManage.UpdateMerchant(merchant);
                        if (result)
                        {
                            await Utils.InsertLocalPictureMerchant(merchant);
                        }
                        Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                        GabanaLoading.SharedInstance.Hide();
                        this.NavigationController.PopViewController(true);
                    }
                    else
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotsave", "Failed to save"));
                        GabanaLoading.SharedInstance.Hide();
                        return;
                    }
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotsave", "Failed to save"));
                    GabanaLoading.SharedInstance.Hide();
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                GabanaLoading.SharedInstance.Hide();
            }
        }
        async Task<bool> UpdatetBranchDetail()
        {
            try
            {
                string phone = txtPhone.Text.Replace("-", ""); ;
                string address = txtAddress.Text;
                string comment = txtComment.Text;
                string linkPromaxx = txtLinkProMax.Text;

                if (BranchDetail != null)
                {
                    UpdatetBranch = new ORM.Master.Branch();
                    UpdatetBranch.MerchantID = (int)BranchDetail.MerchantID;
                    UpdatetBranch.SysBranchID = (int)BranchDetail.SysBranchID;
                    UpdatetBranch.Ordinary = BranchDetail.Ordinary == null ? (int?)null : Convert.ToInt32(BranchDetail.Ordinary);
                    UpdatetBranch.BranchID = BranchDetail.BranchID;
                    UpdatetBranch.BranchName = BranchDetail.BranchName;
                    UpdatetBranch.Address = address;
                    if (idprovice == 0 & idamphure == 0 & iddistist == 0)
                    {
                        UpdatetBranch.ProvincesId = null;
                        UpdatetBranch.AmphuresId = null;
                        UpdatetBranch.DistrictsId = null;
                    }
                    else
                    {
                        UpdatetBranch.ProvincesId = idprovice;
                        UpdatetBranch.AmphuresId = idamphure;
                        UpdatetBranch.DistrictsId = iddistist;
                    }
                    UpdatetBranch.Status = BranchDetail.Status;
                    UpdatetBranch.DisplayLanguage = BranchDetail.DisplayLanguage;
                    UpdatetBranch.Lat = BranchDetail.Lat;
                    UpdatetBranch.Lng = BranchDetail.Lng;
                    UpdatetBranch.Email = BranchDetail.Email;
                    UpdatetBranch.Tel = phone;
                    UpdatetBranch.Line = BranchDetail.Line;
                    UpdatetBranch.Facebook = BranchDetail.Facebook;
                    UpdatetBranch.Instagram = BranchDetail.Instagram;
                    UpdatetBranch.TaxBranchName = BranchDetail.TaxBranchName == null ? BranchDetail.BranchName : BranchDetail.TaxBranchName;
                    UpdatetBranch.TaxBranchID = BranchDetail.TaxBranchID;
                    UpdatetBranch.LinkProMaxxID = linkPromaxx;
                    UpdatetBranch.Comments = comment;
                }
                var update = await GabanaAPI.PutDataBranch(UpdatetBranch); //Cloud
                if (update.Status)
                {
                    //insert local
                    var branch = JsonConvert.DeserializeObject<Gabana.ORM.Master.Branch>(update.Message);
                    if (branch != null)
                    {
                        //Branch
                        BranchManage branchManage = new BranchManage();
                        ORM.MerchantDB.Branch insertBranch = new ORM.MerchantDB.Branch();
                        insertBranch.MerchantID = branch.MerchantID;
                        insertBranch.SysBranchID = branch.SysBranchID;
                        insertBranch.Ordinary = branch.Ordinary;
                        insertBranch.BranchName = branch.BranchName;
                        insertBranch.BranchID = branch.BranchID;
                        insertBranch.Address = branch.Address;
                        insertBranch.ProvincesId = branch.ProvincesId;
                        insertBranch.AmphuresId = branch.AmphuresId;
                        insertBranch.DistrictsId = branch.DistrictsId;
                        insertBranch.Status = branch.Status;
                        insertBranch.DisplayLanguage = branch.DisplayLanguage;
                        insertBranch.Lat = branch.Lat;
                        insertBranch.Lng = branch.Lng;
                        insertBranch.Email = branch.Email;
                        insertBranch.Tel = branch.Tel;
                        insertBranch.Line = branch.Line;
                        insertBranch.Facebook = branch.Facebook;
                        insertBranch.Instagram = branch.Instagram;
                        insertBranch.TaxBranchName = branch.TaxBranchName == null ? branch.BranchName : branch.TaxBranchName;
                        insertBranch.TaxBranchID = branch.TaxBranchID;
                        insertBranch.LinkProMaxxID = branch.LinkProMaxxID;
                        insertBranch.Comments = branch.Comments;

                        var insert = await branchManage.UpdateBranch(insertBranch); //Local
                        var index = DataCashingAll.Merchant.Branch.FindIndex(x => x.SysBranchID == 1); //สาขาสำนักงานใหญ่
                        if (index != -1)
                        {
                            DataCashingAll.Merchant.Branch.RemoveAt(index);
                            DataCashingAll.Merchant.Branch.Add(UpdatetBranch);
                        }
                        return true;
                    }
                    return false;
                }
                else
                {
                    Utils.ShowMessage(update.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return false;
            }
        }
        private void OnKeyboardChanged(bool visible, nfloat nfloat)
        {
            if (!visible)
                RestoreScrollPosition(_scrollView);
            else
                CenterViewInScroll(View, _scrollView, nfloat);
        }
        protected virtual void CenterViewInScroll(UIView viewToCenter, UIScrollView scrollView, nfloat keyboardHeight)
        {
            var contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardHeight, 0.0f);
            scrollView.ContentInset = contentInsets;
            scrollView.ScrollIndicatorInsets = contentInsets;
        }

        protected virtual void RestoreScrollPosition(UIScrollView scrollView)
        {
            scrollView.ContentInset = UIEdgeInsets.Zero;
            scrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
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
            SetButtonAdd(true);
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
            #region BottomView
            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSave.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSave.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region UpperView
            logoView.TopAnchor.ConstraintEqualTo(logoView.Superview.TopAnchor, 0).Active = true;
            logoView.LeftAnchor.ConstraintEqualTo(logoView.Superview.LeftAnchor, 0).Active = true;
            logoView.RightAnchor.ConstraintEqualTo(logoView.Superview.RightAnchor, 0).Active = true;
            logoView.HeightAnchor.ConstraintEqualTo(224).Active = true;

            profileImg.CenterXAnchor.ConstraintEqualTo(profileImg.Superview.CenterXAnchor).Active = true;
            profileImg.CenterYAnchor.ConstraintEqualTo(profileImg.Superview.CenterYAnchor).Active = true;
            profileImg.HeightAnchor.ConstraintEqualTo(150).Active = true;
            profileImg.WidthAnchor.ConstraintEqualTo(150).Active = true;


            btnChangeImage.RightAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            btnChangeImage.BottomAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            btnChangeImage.HeightAnchor.ConstraintEqualTo(44).Active = true;
            btnChangeImage.WidthAnchor.ConstraintEqualTo(44).Active = true;
            #endregion

            line.TopAnchor.ConstraintEqualTo(logoView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(line.Superview.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(5).Active = true;
            line.RightAnchor.ConstraintEqualTo(line.Superview.RightAnchor,0).Active = true;

            #region customerView
            MerchantView.TopAnchor.ConstraintEqualTo(line.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            MerchantView.LeftAnchor.ConstraintEqualTo(MerchantView.Superview.LeftAnchor, 0).Active = true;
            MerchantView.RightAnchor.ConstraintEqualTo(MerchantView.Superview.RightAnchor, 0).Active = true;
            MerchantView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblMerchantName.TopAnchor.ConstraintEqualTo(lblMerchantName.Superview.TopAnchor, 11).Active = true;
            lblMerchantName.LeftAnchor.ConstraintEqualTo(lblMerchantName.Superview.LeftAnchor, 15).Active = true;
            lblMerchantName.RightAnchor.ConstraintEqualTo(lblMerchantName.Superview.RightAnchor, -20).Active = true;
            lblMerchantName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtMerchantName.TopAnchor.ConstraintEqualTo(lblMerchantName.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtMerchantName.LeftAnchor.ConstraintEqualTo(txtMerchantName.Superview.LeftAnchor, 15).Active = true;
            txtMerchantName.RightAnchor.ConstraintEqualTo(txtMerchantName.Superview.RightAnchor, -20).Active = true;
            txtMerchantName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

           

            #region DetailToggleView
            DetailToggleView.TopAnchor.ConstraintEqualTo(MerchantView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            DetailToggleView.LeftAnchor.ConstraintEqualTo(DetailToggleView.Superview.LeftAnchor, 0).Active = true;
            DetailToggleView.RightAnchor.ConstraintEqualTo(DetailToggleView.Superview.RightAnchor, 0).Active = true;
            DetailToggleView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            lbltxtDetail.CenterYAnchor.ConstraintEqualTo(lbltxtDetail.Superview.CenterYAnchor).Active = true;
            lbltxtDetail.LeftAnchor.ConstraintEqualTo(lbltxtDetail.Superview.LeftAnchor, 15).Active = true;
            lbltxtDetail.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btndetailtoggle.CenterYAnchor.ConstraintEqualTo(btndetailtoggle.Superview.CenterYAnchor).Active = true;
            btndetailtoggle.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btndetailtoggle.RightAnchor.ConstraintEqualTo(btndetailtoggle.Superview.RightAnchor, -15).Active = true;
            btndetailtoggle.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region TaxIDView
            TaxIDView.TopAnchor.ConstraintEqualTo(DetailToggleView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            TaxIDView.LeftAnchor.ConstraintEqualTo(TaxIDView.Superview.LeftAnchor, 0).Active = true;
            TaxIDView.RightAnchor.ConstraintEqualTo(TaxIDView.Superview.RightAnchor, 0).Active = true;
            TaxIDView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblTaxID.TopAnchor.ConstraintEqualTo(lblTaxID.Superview.TopAnchor, 11).Active = true;
            lblTaxID.LeftAnchor.ConstraintEqualTo(lblTaxID.Superview.LeftAnchor, 15).Active = true;
            lblTaxID.RightAnchor.ConstraintEqualTo(lblTaxID.Superview.RightAnchor, -20).Active = true;
            lblTaxID.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtTaxID.TopAnchor.ConstraintEqualTo(lblTaxID.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtTaxID.LeftAnchor.ConstraintEqualTo(txtTaxID.Superview.LeftAnchor, 15).Active = true;
            txtTaxID.RightAnchor.ConstraintEqualTo(txtTaxID.Superview.RightAnchor, -20).Active = true;
            txtTaxID.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region RegistrationNoView
            RegistrationNoView.TopAnchor.ConstraintEqualTo(TaxIDView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            RegistrationNoView.LeftAnchor.ConstraintEqualTo(RegistrationNoView.Superview.LeftAnchor, 0).Active = true;
            RegistrationNoView.RightAnchor.ConstraintEqualTo(RegistrationNoView.Superview.RightAnchor, 0).Active = true;
            RegistrationNoView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblRegisNo.TopAnchor.ConstraintEqualTo(lblRegisNo.Superview.TopAnchor, 11).Active = true;
            lblRegisNo.LeftAnchor.ConstraintEqualTo(lblRegisNo.Superview.LeftAnchor, 15).Active = true;
            lblRegisNo.RightAnchor.ConstraintEqualTo(lblRegisNo.Superview.RightAnchor, -20).Active = true;
            lblRegisNo.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtRegisNo.TopAnchor.ConstraintEqualTo(lblRegisNo.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtRegisNo.LeftAnchor.ConstraintEqualTo(txtRegisNo.Superview.LeftAnchor, 15).Active = true;
            txtRegisNo.RightAnchor.ConstraintEqualTo(txtRegisNo.Superview.RightAnchor, -20).Active = true;
            txtRegisNo.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region PhoneView
            PhoneView.TopAnchor.ConstraintEqualTo(RegistrationNoView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            PhoneView.LeftAnchor.ConstraintEqualTo(PhoneView.Superview.LeftAnchor, 0).Active = true;
            PhoneView.RightAnchor.ConstraintEqualTo(PhoneView.Superview.RightAnchor, 0).Active = true;
            PhoneView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblPhone.TopAnchor.ConstraintEqualTo(lblPhone.Superview.TopAnchor, 11).Active = true;
            lblPhone.LeftAnchor.ConstraintEqualTo(lblPhone.Superview.LeftAnchor, 15).Active = true;
            lblPhone.RightAnchor.ConstraintEqualTo(lblPhone.Superview.RightAnchor, -20).Active = true;
            lblPhone.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtPhone.TopAnchor.ConstraintEqualTo(lblPhone.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtPhone.LeftAnchor.ConstraintEqualTo(txtPhone.Superview.LeftAnchor, 15).Active = true;
            txtPhone.RightAnchor.ConstraintEqualTo(txtPhone.Superview.RightAnchor, -20).Active = true;
            txtPhone.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region AddressView
            AddressView.TopAnchor.ConstraintEqualTo(PhoneView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            AddressView.LeftAnchor.ConstraintEqualTo(AddressView.Superview.LeftAnchor, 0).Active = true;
            AddressView.RightAnchor.ConstraintEqualTo(AddressView.Superview.RightAnchor, 0).Active = true;
            AddressView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblAddress.TopAnchor.ConstraintEqualTo(lblAddress.Superview.TopAnchor, 11).Active = true;
            lblAddress.LeftAnchor.ConstraintEqualTo(lblAddress.Superview.LeftAnchor, 15).Active = true;
            lblAddress.RightAnchor.ConstraintEqualTo(lblAddress.Superview.RightAnchor, -20).Active = true;
            lblAddress.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtAddress.TopAnchor.ConstraintEqualTo(lblAddress.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtAddress.LeftAnchor.ConstraintEqualTo(txtAddress.Superview.LeftAnchor, 15).Active = true;
            txtAddress.RightAnchor.ConstraintEqualTo(txtAddress.Superview.RightAnchor, -20).Active = true;
            txtAddress.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region ProvinceView
            ProvinceView.TopAnchor.ConstraintEqualTo(AddressView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            ProvinceView.LeftAnchor.ConstraintEqualTo(ProvinceView.Superview.LeftAnchor, 0).Active = true;
            ProvinceView.RightAnchor.ConstraintEqualTo(ProvinceView.Superview.RightAnchor, 0).Active = true;
            ProvinceView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblProvince.TopAnchor.ConstraintEqualTo(lblProvince.Superview.TopAnchor, 11).Active = true;
            lblProvince.LeftAnchor.ConstraintEqualTo(lblProvince.Superview.LeftAnchor, 15).Active = true;
            lblProvince.RightAnchor.ConstraintEqualTo(lblProvince.Superview.RightAnchor, -60).Active = true;
            lblProvince.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtProvince.TopAnchor.ConstraintEqualTo(lblProvince.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtProvince.LeftAnchor.ConstraintEqualTo(txtProvince.Superview.LeftAnchor, 15).Active = true;
            txtProvince.RightAnchor.ConstraintEqualTo(txtProvince.Superview.RightAnchor, -60).Active = true;
            txtProvince.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnProvince.CenterYAnchor.ConstraintEqualTo(btnProvince.Superview.CenterYAnchor).Active = true;
            btnProvince.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnProvince.RightAnchor.ConstraintEqualTo(btnProvince.Superview.RightAnchor, -15).Active = true;
            btnProvince.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region DistrictView
            DistrictView.TopAnchor.ConstraintEqualTo(ProvinceView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            DistrictView.LeftAnchor.ConstraintEqualTo(DistrictView.Superview.LeftAnchor, 0).Active = true;
            DistrictView.RightAnchor.ConstraintEqualTo(DistrictView.Superview.RightAnchor, 0).Active = true;
            DistrictView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblDistrict.TopAnchor.ConstraintEqualTo(lblDistrict.Superview.TopAnchor, 11).Active = true;
            lblDistrict.LeftAnchor.ConstraintEqualTo(lblDistrict.Superview.LeftAnchor, 15).Active = true;
            lblDistrict.RightAnchor.ConstraintEqualTo(lblDistrict.Superview.RightAnchor, -60).Active = true;
            lblDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtDistrict.TopAnchor.ConstraintEqualTo(lblDistrict.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtDistrict.LeftAnchor.ConstraintEqualTo(txtDistrict.Superview.LeftAnchor, 15).Active = true;
            txtDistrict.RightAnchor.ConstraintEqualTo(txtDistrict.Superview.RightAnchor, -60).Active = true;
            txtDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnDistrict.CenterYAnchor.ConstraintEqualTo(btnDistrict.Superview.CenterYAnchor).Active = true;
            btnDistrict.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnDistrict.RightAnchor.ConstraintEqualTo(btnDistrict.Superview.RightAnchor, -15).Active = true;
            btnDistrict.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region SubdistrictView
            SubdistrictView.TopAnchor.ConstraintEqualTo(DistrictView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            SubdistrictView.LeftAnchor.ConstraintEqualTo(SubdistrictView.Superview.LeftAnchor, 0).Active = true;
            SubdistrictView.RightAnchor.ConstraintEqualTo(SubdistrictView.Superview.RightAnchor, 0).Active = true;
            SubdistrictView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblSubDistrict.TopAnchor.ConstraintEqualTo(lblSubDistrict.Superview.TopAnchor, 11).Active = true;
            lblSubDistrict.LeftAnchor.ConstraintEqualTo(lblSubDistrict.Superview.LeftAnchor, 15).Active = true;
            lblSubDistrict.RightAnchor.ConstraintEqualTo(lblSubDistrict.Superview.RightAnchor, -60).Active = true;
            lblSubDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtSubDistrict.TopAnchor.ConstraintEqualTo(lblSubDistrict.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtSubDistrict.LeftAnchor.ConstraintEqualTo(txtSubDistrict.Superview.LeftAnchor, 15).Active = true;
            txtSubDistrict.RightAnchor.ConstraintEqualTo(txtSubDistrict.Superview.RightAnchor, -60).Active = true;
            txtSubDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSubDistrict.CenterYAnchor.ConstraintEqualTo(btnSubDistrict.Superview.CenterYAnchor).Active = true;
            btnSubDistrict.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSubDistrict.RightAnchor.ConstraintEqualTo(btnSubDistrict.Superview.RightAnchor, -15).Active = true;
            btnSubDistrict.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region PostalCodeView
            PostalCodeView.TopAnchor.ConstraintEqualTo(SubdistrictView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            PostalCodeView.LeftAnchor.ConstraintEqualTo(PostalCodeView.Superview.LeftAnchor, 0).Active = true;
            PostalCodeView.RightAnchor.ConstraintEqualTo(PostalCodeView.Superview.RightAnchor, 0).Active = true;
            PostalCodeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblPostal.TopAnchor.ConstraintEqualTo(lblPostal.Superview.TopAnchor, 11).Active = true;
            lblPostal.LeftAnchor.ConstraintEqualTo(lblPostal.Superview.LeftAnchor, 15).Active = true;
            lblPostal.RightAnchor.ConstraintEqualTo(lblPostal.Superview.RightAnchor, -60).Active = true;
            lblPostal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtPostal.TopAnchor.ConstraintEqualTo(lblPostal.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtPostal.LeftAnchor.ConstraintEqualTo(txtPostal.Superview.LeftAnchor, 15).Active = true;
            txtPostal.RightAnchor.ConstraintEqualTo(txtPostal.Superview.RightAnchor, -60).Active = true;
            txtPostal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnPostal.CenterYAnchor.ConstraintEqualTo(btnPostal.Superview.CenterYAnchor).Active = true;
            btnPostal.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnPostal.RightAnchor.ConstraintEqualTo(btnPostal.Superview.RightAnchor, -15).Active = true;
            btnPostal.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region CommentView
            CommentView.TopAnchor.ConstraintEqualTo(PostalCodeView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            CommentView.LeftAnchor.ConstraintEqualTo(CommentView.Superview.LeftAnchor, 0).Active = true;
            CommentView.RightAnchor.ConstraintEqualTo(CommentView.Superview.RightAnchor, 0).Active = true;
            CommentView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblComment.TopAnchor.ConstraintEqualTo(lblComment.Superview.TopAnchor, 11).Active = true;
            lblComment.LeftAnchor.ConstraintEqualTo(lblComment.Superview.LeftAnchor, 15).Active = true;
            lblComment.RightAnchor.ConstraintEqualTo(lblComment.Superview.RightAnchor, -60).Active = true;
            lblComment.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtComment.TopAnchor.ConstraintEqualTo(lblComment.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtComment.LeftAnchor.ConstraintEqualTo(txtComment.Superview.LeftAnchor, 15).Active = true;
            txtComment.RightAnchor.ConstraintEqualTo(txtComment.Superview.RightAnchor, -60).Active = true;
            txtComment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region LinkProView
            LinkProView.TopAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            LinkProView.LeftAnchor.ConstraintEqualTo(LinkProView.Superview.LeftAnchor, 0).Active = true;
            LinkProView.RightAnchor.ConstraintEqualTo(LinkProView.Superview.RightAnchor, 0).Active = true;
            LinkProView.BottomAnchor.ConstraintEqualTo(LinkProView.Superview.BottomAnchor, 0).Active = true;
            LinkProView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblLinkProMax.TopAnchor.ConstraintEqualTo(lblLinkProMax.Superview.TopAnchor, 11).Active = true;
            lblLinkProMax.LeftAnchor.ConstraintEqualTo(lblLinkProMax.Superview.LeftAnchor, 15).Active = true;
            lblLinkProMax.RightAnchor.ConstraintEqualTo(lblLinkProMax.Superview.RightAnchor, -60).Active = true;
            lblLinkProMax.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtLinkProMax.TopAnchor.ConstraintEqualTo(lblLinkProMax.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            txtLinkProMax.LeftAnchor.ConstraintEqualTo(txtLinkProMax.Superview.LeftAnchor, 15).Active = true;
            txtLinkProMax.RightAnchor.ConstraintEqualTo(txtLinkProMax.Superview.RightAnchor, -60).Active = true;
            txtLinkProMax.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion
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