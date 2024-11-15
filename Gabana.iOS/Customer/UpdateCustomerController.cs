using AssetsLibrary;
using Foundation;
//using Gabana.ORM.Master;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource.Sync;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class UpdateCustomerController : UIViewController
    {
        Uri keepCropedUri = null;
        UIView BottomViewEdit;
        public string Phone = "";
        UIButton btnDelete, btnSaveCategory;
        private static byte[] picture;
        bool flagDetail=false;
        UIScrollView _scrollView;
        UIView DeleteView;
        UIButton btnAdd;
        UIImageView deleteBtn;
        UIView _contentView;
        UIView ShortNameView, CustomerTypeView;
        UILabel lblShortName,lblShortNameLogo;
        UIImageView btnCustype;
        UITextField txtShortName;
        IJobQueue jobQueue;
        UIImagePickerController imagePicker;
        UIAlertController selectPhotoMenuSheet;
        UIButton btnChangeImage, btnSave,btndetailtoggle;
        UIImageView profileImg;
        UIImageView btnSelectGender, btnSelectBirth;
        UITextField txtCustomerName, txtGender,txtBirth;
        UILabel lblCustomerName,lblGender,lblBirth,lbltxtDetail;
        UIView logoView, line, line2, customerView, bottomView,GenderView,BirthView, DetailToggleView, line0, line1, line3;
        UIView lineD1, lineD2, lineD3, lineD4, lineD5, lineD6, lineD7, lineD8, lineD9, lineD10, lineD11, lineD12, lineD13;
        Gabana.ORM.MerchantDB.Customer Customer = new Gabana.ORM.MerchantDB.Customer();
        UIView CusIDView, PhoneView, EmailView, IDCardView, AddressView, ProvinceView, DistrictView, SubdistrictView, PostalCodeView,CommentView,LinkProView;
        UILabel lblCusID, lblPhone, lblEmail, lblIdCard, lblAddress, lblProvince, lblDistrict, lblSubDistrict, lblPostal, lblComment, lblLinkProMax;
        UITextField txtCusID, txtPhone, txtEmail, txtIdCard, txtAddress, txtProvince, txtDistrict, txtSubDistrict, txtPostal, txtComment, txtLinkProMax;
        UIImageView btnProvince, btnDistrict, btnSubDistrict, btnPostal;
        PoolManage poolManager = new PoolManage();
        CustomerManage CustomerManage = new CustomerManage();
        DeviceSystemSeqNo deviceSystemSeq = new DeviceSystemSeqNo();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        string filePath;
        UIImage editedImage;
        List<string> CustomerTypenamelist = new List<string>();
        List<string> CustomerTypeNolist = new List<string>(); 
        public int idprovice;
        public int idamphure;
        public char gender = 'N';
        public int iddistist;
        public static string status;
        public UIDatePicker datePickerView;
        UILabel lblCusType;
        UITextField txtCusType;
        string path;
        UIView LineView;
        UILabel lblLine;
        int CustomerTypeNo = 0;
        UITextField txtLine;
        List<MemberType> memberTypes; 
        CustomerController cusPage=null;
        bool reset = false;
        private readonly List<string> sex = new List<string>
        {
        Utils.TextBundle("none", "None"),
        Utils.TextBundle("male", "Male"),
        Utils.TextBundle("female", "Female")
        };
        public List<Gabana.ORM.PoolDB.Province> province;
        private UIPickerView AmpurePicker;
        private UIPickerView DistrictPicker;
        private UIPickerView ProvincePicker;
        private bool Editchange;
        private UIView Viewblock;

        public UpdateCustomerController()
        {
        }
        public UpdateCustomerController(Gabana.ORM.MerchantDB.Customer listCustomer)
        {
            this.Customer = listCustomer;
        }
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("savechanges", "savechanges"), UIAlertControllerStyle.Alert);
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
            if (Customer == null)
            {
                ClickAdd();
            }
            else
            {
                UpdateClick();
            }


        }
        async Task GetMemberTypeData()
        {
            try
            {
                List<MemberType> lstmembertypr = new List<MemberType>();
                var membertypeManage = new MemberTypeManage();
                //MemberType
                var listmembertype = await GabanaAPI.GetDataMemberType();
                if (listmembertype != null && listmembertype.Count > 0)
                {
                    var Allmember = await membertypeManage.DeleteAllMemberType(DataCashingAll.MerchantId);
                    foreach (var item in listmembertype)
                    {
                        MemberType memberType = new MemberType()
                        {
                            DateModified = item.DateModified,
                            LinkProMaxxID = item.LinkProMaxxID,
                            MemberTypeName = item.MemberTypeName,
                            MemberTypeNo = item.MemberTypeNo,
                            MerchantID = item.MerchantID,
                            PercentDiscount = item.PercentDiscount
                        };
                        var InsertorReplace = await membertypeManage.InsertorReplacrMemberType(memberType);
                    }
                }
            }
            catch (Exception ex)
            {
                string text = "GetMemberTypeData";
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync(text);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public override void ViewWillAppear(bool animated)
        {
            Utils.SetTitle(this.NavigationController,Utils.TextBundle("billhis", "BillHistory"));
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
                button.SetTitle("  "+Utils.TextBundle("back", "Back"), UIControlState.Normal);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);

                CultureInfo cultureInfo = new CultureInfo("en-US");
                this.NavigationController.SetNavigationBarHidden(false, false);

                Textboxfocus(View);
                View.BackgroundColor = UIColor.FromRGB(248,248,248);

                initAttribute();
                setupAutoLayout();
                await GetMemberTypeData();
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
                btnSelectGender.UserInteractionEnabled = true;
                var tapGesture0 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("SelectGender:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnSelectGender.AddGestureRecognizer(tapGesture0);

                btnSelectBirth.UserInteractionEnabled = true;
                var tapGesture1 = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("Birth:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnSelectBirth.AddGestureRecognizer(tapGesture1);

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

                

                var tapGesture6 = new UITapGestureRecognizer(this,
                   new ObjCRuntime.Selector("SelectCustype:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                btnCustype.AddGestureRecognizer(tapGesture6);

                #endregion

                _contentView.BackgroundColor = UIColor.FromRGB(248,248,248);
                _scrollView.BackgroundColor = UIColor.FromRGB(248,248,248);
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
                SelectCutomerType();
                await SetupPicker();

                if (Customer !=null && Customer.CustomerName!=null && Customer.CustomerName!="")
                {
                    await SetupCustomerData();
                    bottomView.Hidden = true;
                    BottomViewEdit.Hidden = false;
                    setDetailShow(true);
                    btnSaveCategory.Enabled = true;
                    if (Customer.ProvincesId > 0)
                    {
                        var pro = province.Where(x => x.ProvincesId == Customer.ProvincesId).FirstOrDefault();
                        var index = province.FindIndex(x => x.ProvincesId == Customer.ProvincesId);
                        txtProvince.Text = pro.ProvincesNameTH;
                        idprovice = (int)pro.ProvincesId;
                        setamphure((int)pro.ProvincesId, (int)Customer.AmphuresId);
                        setdistist((int)Customer.AmphuresId, (int)Customer.DistrictsId);
                        ProvincePicker.Select(index, 0, false);

                    }
                    


                }
                else
                {
                    bottomView.Hidden = false;
                    BottomViewEdit.Hidden = true;
                }
                Checkper();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        private void Checkper()
        {
            var LoginType = Preferences.Get("LoginType", "");
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "customer");
            if (check)
            {

            }
            else
            {
                Viewblock = new UIView();
                Viewblock.TranslatesAutoresizingMaskIntoConstraints = false;
                Viewblock.BackgroundColor = UIColor.Clear;
                View.AddSubview(Viewblock);
                View.BringSubviewToFront(Viewblock);

                Viewblock.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
                Viewblock.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
                Viewblock.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
                Viewblock.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
                View.Alpha = 0.9f;
                btnSaveCategory.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

    string addTextTel(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                if (value.Length == 9 & value.StartsWith("02"))
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 2 | i == 5)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }

                if (value.Length == 9 & value.StartsWith("03") | value.StartsWith("04") | value.StartsWith("05") | value.StartsWith("07"))
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

                if (value.Length == 10)
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
                _ = TinyInsights.TrackPageViewAsync("addTextTel at add Customer");
                return value;
            }
        }
        public async void SelectCutomerType()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                CustomerTypeNolist = new List<string>();
                CustomerTypenamelist = new List<string>();
                MemberType memberType = new MemberType();

                var getallmembertype = new List<MemberType>();
                MemberTypeManage memberTypeManage = new MemberTypeManage();

                memberTypes = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                memberTypes.Insert(0, new MemberType() {MemberTypeNo = 0 , MemberTypeName = Utils.TextBundle("membertype", "Member Type") });
                //for (int i = 0; i < getallmembertype.Count; i++)
                //{
                //    temp = getallmembertype[i].MemberTypeNo.ToString();
                //    temp2 = getallmembertype[i].MemberTypeName.ToString();
                //    CustomerTypeNolist.Add(temp);
                //    CustomerTypenamelist.Add(temp2);
                //}

                //DataCaching.Membertype = CustomerTypeNolist;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        private async void DeleteCustomer()
        {
            try
            {
                CustomerManage CustomerManage = new CustomerManage();
                var cusdelete = Customer;
                cusdelete.DataStatus = 'D';
                cusdelete.FWaitSending = 1;
                cusdelete.LastDateModified = DateTime.UtcNow;
                var result = await CustomerManage.UpdateCustomer(cusdelete);
                if (result)
                {
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendCustomer((int)MainController.merchantlocal.MerchantID, (int)cusdelete.SysCustomerID);
                    }
                    //await CustomerSync.SentCustomer((int)MainController.merchantlocal.MerchantID, (int)cusdelete.SysCustomerID,null);

                    this.NavigationController.PopViewController(false);
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "deletesuccessfully"));
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("failedtodelete", "failedtodelete"));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(Utils.TextBundle("cantdeletecustomer", "cantdeletecustomer"));
                return;
            }
        }
        void setDetailShow(bool set)
        {
            if(set)
            {
                LinkProView.Hidden = true;
                CommentView.Hidden = true;
                PostalCodeView.Hidden = true;
                SubdistrictView.Hidden = true;
                DistrictView.Hidden = true;
                ProvinceView.Hidden = true;
                AddressView.Hidden = true;
                IDCardView.Hidden = true;
                BirthView.Hidden = true;
                GenderView.Hidden = true;
                ShortNameView.Hidden = true;
                EmailView.Hidden = true;
                LineView.Hidden = true;
                CusIDView.Hidden = true;
            }
            else
            {
                LinkProView.Hidden = false;
                CommentView.Hidden = false;
                PostalCodeView.Hidden = false;
                SubdistrictView.Hidden = false;
                DistrictView.Hidden = false;
                ProvinceView.Hidden = false;
                BirthView.Hidden = false;
                ShortNameView.Hidden = false;
                GenderView.Hidden = false;
                AddressView.Hidden = false;
                IDCardView.Hidden = false;
                EmailView.Hidden = false;
                LineView.Hidden = false;
                CusIDView.Hidden = false;
            }
        }

        #region Picker
        
        //SelectCustype
        [Export("SelectCustype:")]
        public void SelectCustype(UIGestureRecognizer sender)
        {
            txtCusType.BecomeFirstResponder();
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
        [Export("Birth:")]
        public void Birth(UIGestureRecognizer sender)
        {
            txtBirth.BecomeFirstResponder();
        }
        [Export("SelectGender:")]
        public void SelectGender(UIGestureRecognizer sender)
        {
            txtGender.BecomeFirstResponder();
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
        public class PickerModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;


            private readonly IList<string> values;
            public PickerModel(IList<string> values)
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
                return values[Convert.ToInt32(row)];
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs { SelectedValue = values[Convert.ToInt32(row)] });
                }
            }



            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        public class PickerCustomerTypeModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;


            private readonly IList<MemberType> values;
            public PickerCustomerTypeModel(List<MemberType> values)
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
                return values[Convert.ToInt32(row)].MemberTypeName;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs { SelectedValue = values[Convert.ToInt32(row)].MemberTypeName ,ID = Convert.ToInt32(values[Convert.ToInt32(row)].MemberTypeNo) });
                }
            }



            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        private async Task SetupPicker()
        {
            try
            {
                var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);

                #region sex
                PickerModel modelsex = new PickerModel(sex);

                UIToolbar toolbar2 = new UIToolbar();
                toolbar2.Translucent = true;
                toolbar2.SizeToFit();
                var doneButton2 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    View.EndEditing(true);
                });
                toolbar2.SetItems(new UIBarButtonItem[] { flexible, doneButton2 }, true);
                modelsex.PickerChanged += (sender, e) => {
                    Editchange = true;
                    txtGender.Text = e.SelectedValue;
                    if (e.SelectedValue == Utils.TextBundle("male", "Male"))
                    {
                        gender = 'M';
                    }
                    else if (e.SelectedValue == Utils.TextBundle("female", "Female"))
                    {
                        gender = 'F';
                    }
                    else
                    {
                        gender = 'N';
                    }
                };
                var Gender = new UIPickerView() { Model = modelsex, ShowSelectionIndicator = true };
                txtGender.InputView = Gender;
                txtGender.InputAccessoryView = toolbar2;

                if (sex != null)
                {
                    if(Customer.Gender == 'M')
                    {
                        txtGender.Text = sex[1];
                        Gender.Select(1, 0, false);
                    }
                    else if (Customer.Gender == 'F')
                    {
                        txtGender.Text = sex[2];
                        Gender.Select(2, 0, false);
                    }
                    else
                    {
                        txtGender.Text = sex[0];
                        Gender.Select(0, 0, false);
                    }
                }
                #endregion

                #region MemberType
                PickerCustomerTypeModel modelCusType = new PickerCustomerTypeModel(memberTypes);
                modelCusType.PickerChanged += (sender, e) => {
                    if (memberTypes != null)
                    {
                        txtCusType.Text = e.SelectedValue;
                        CustomerTypeNo = e.ID;
                    }
                    Editchange = true;
                };
                UIToolbar toolbar21 = new UIToolbar();
                toolbar21.BarStyle = UIBarStyle.Default;
                toolbar21.Translucent = true;
                toolbar21.SizeToFit();

                var doneButton21 = new UIBarButtonItem(Utils.TextBundle("done", "DONE"), UIBarButtonItemStyle.Done, this, new ObjCRuntime.Selector("CategoryAction"));
                toolbar21.SetItems(new UIBarButtonItem[] { flexible, doneButton21 }, true);
                var xy = new UIPickerView() { Model = modelCusType, ShowSelectionIndicator = true };
                txtCusType.InputView = xy;
                if (memberTypes != null)
                {
                    if (Customer == null && Customer.MemberTypeNo == null)
                    {
                        txtCusType.Text = memberTypes[0].MemberTypeName;
                        CustomerTypeNo = Convert.ToInt32(memberTypes[0].MemberTypeNo);
                        xy.Select(0, 0, false);
                        return;
                    }

                    if (memberTypes.Count > 0 && Customer.MemberTypeNo != null)
                    {
                        txtCusType.Text = memberTypes.Where(x => x.MemberTypeNo == Customer.MemberTypeNo).FirstOrDefault().MemberTypeName;
                        CustomerTypeNo = (int)memberTypes.FindIndex(x => x.MemberTypeNo == Customer.MemberTypeNo);
                        xy.Select((int)CustomerTypeNo, 0, false);
                    }
                    else if(memberTypes.Count > 0 && Customer.MemberTypeNo == null)
                    {
                        txtCusType.Text = Utils.TextBundle("membertype", "OK");
                    }
                    else 
                    {
                        txtCusType.Text = Utils.TextBundle("membertype", "OK");
                        txtCusType.Enabled = false;
                    }

                }
                txtCusType.InputAccessoryView = toolbar21;
                #endregion

                #region DATE
                datePickerView = new UIDatePicker();

                var dateFormatter = new NSDateFormatter();
                dateFormatter.Locale = new NSLocale("en_US");
                dateFormatter.DateFormat = "yyyy-MM-dd";


                var calendar = new NSCalendar(NSCalendarType.Gregorian);
                var currentDate = NSDate.Now;
                var components = new NSDateComponents();

                datePickerView.ValueChanged += (sender, s) => {
                    Editchange = true;
                    var dateString = datePickerView.Date.Description;
                    dateFormatter.DateFormat = "dd/MM/yyyy";
                    txtBirth.Text = dateFormatter.ToString(datePickerView.Date);
                };
                components.Year = -100;
                NSDate minDate = calendar.DateByAddingComponents(components, NSDate.Now, NSCalendarOptions.None);
                datePickerView.MinimumDate = minDate;

                datePickerView.Locale = new NSLocale("th_USR");
                if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
                {
                    datePickerView.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
                }

                datePickerView.MaximumDate = currentDate;
                datePickerView.Date = currentDate;
                datePickerView.Mode = UIDatePickerMode.Date;
                datePickerView.Calendar = new NSCalendar(NSCalendarType.Gregorian);
                UIToolbar toolbar = new UIToolbar();
                toolbar.Translucent = true;
                toolbar.SizeToFit();
                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                    View.EndEditing(true);
                });
                toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
                txtBirth.InputView = datePickerView;
                txtBirth.InputAccessoryView = toolbar;
                
                #endregion

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
                    Editchange = true;
                    txtProvince.Text = e.SelectedValue;
                        reset = true;
                        setamphure(e.ID,0);
                        idprovice = e.ID;
                        txtDistrict.Enabled = true;
                    
                    
                };
                ProvincePicker = new UIPickerView() { Model = provinceM, ShowSelectionIndicator = true };
                txtProvince.InputView = ProvincePicker;
                txtProvince.InputAccessoryView = toolbar3;
                txtProvince.Text = Utils.TextBundle("province", "OK");
                //provinceM.Selected(ProvincePicker, 0, 0);


                var amphure = new List<Amphure>() { new Amphure() {AmphuresId = 0 , AmphuresNameEN ="อำเภอ", AmphuresNameTH = "อำเภอ" } };

                PickerAmphureModel model2 = new PickerAmphureModel(amphure);
                txtSubDistrict.Enabled = true;
                idamphure = (int)amphure[0].AmphuresId;
                model2.PickerChanged += async (sender, e) =>
                {
                    if (e.ID !=0)
                    {
                        reset = true;
                        txtDistrict.Text = e.SelectedValue;
                        setdistist(e.ID,0);
                        txtSubDistrict.Enabled = true;
                        idamphure = e.ID;
                    }
                    else
                    {
                        setdistist(e.ID,0);
                        txtSubDistrict.Enabled = false;
                        DistrictPicker.Select(0, 0, false);
                        AmpurePicker.Select(0, 0, false);
                    }
                    
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
                txtDistrict.Text = Utils.TextBundle("district", "OK");


                var distist = new List<District>() { new District() { DistrictsId = 0, DistrictsNameEN = "ตำบล", DistrictsNameTH = "ตำบล" } };


                PickerDistrictModel model3 = new PickerDistrictModel(distist);
                model3.PickerChanged += async (sender, e) =>
                {
                    if (e.ID!=0)
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
                txtSubDistrict.Text = Utils.TextBundle("subdistrict", "OK");

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
        private async void setamphure(int id , int idamp)
        {
            try
            {
                var amphure = new List<Amphure>() { new Amphure() { AmphuresId = 0, AmphuresNameEN = "อำเภอ", AmphuresNameTH = "อำเภอ" } };
                if (id != 0&& id !=null )
                {
                     amphure.AddRange( await poolManager.GetAmphures(Convert.ToInt32(id)));
                }
                
                PickerAmphureModel model2 = new PickerAmphureModel(amphure);
                txtSubDistrict.Enabled = true;
                idamphure = (int)amphure[0].AmphuresId;
                model2.PickerChanged += async (sender, e) =>
                {
                    Editchange = true;
                    reset = true;
                    txtDistrict.Text = e.SelectedValue;
                    setdistist(e.ID,0);
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

                if (idamp>0)
                {
                    var index = amphure.FindIndex(x => x.AmphuresId == idamp);
                    txtDistrict.Text = amphure[index].AmphuresNameTH;
                    idamphure = (int)amphure[index].AmphuresId;
                    AmpurePicker.Select(index,0,false);
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
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
            }
        }
        private async void setdistist(int id,int iddis)
        {
            try
            {
                var distist = new List<District>() { new District() { DistrictsId = 0, DistrictsNameEN = "ตำบล", DistrictsNameTH = "ตำบล" } };
                if (id != 0 && id != null)
                {
                    distist.AddRange(await poolManager.GetDistricts(Convert.ToInt32(id)));
                }
                
                PickerDistrictModel model3 = new PickerDistrictModel(distist);
                model3.PickerChanged += async (sender, e) =>
                {
                    Editchange = true;
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
        #endregion
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region LayoutAtrribute

            #region LogoView
            logoView = new UIView();
            logoView.BackgroundColor = UIColor.White;
            logoView.TranslatesAutoresizingMaskIntoConstraints = false;


            profileImg = new UIImageView();
            //  profileImg.BackgroundColor = UIColor.FromRGB(226,226,226);
            profileImg.Image = UIImage.FromFile("defaultcust.png");
            profileImg.TranslatesAutoresizingMaskIntoConstraints = false;
            profileImg.Layer.CornerRadius = 50;
            profileImg.ClipsToBounds = true;
            logoView.AddSubview(profileImg);

            lblShortNameLogo = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblShortNameLogo.Font = lblShortNameLogo.Font.WithSize(15);
            lblShortNameLogo.Text = "Name";
            lblShortNameLogo.TextAlignment = UITextAlignment.Center;
            profileImg.AddSubview(lblShortNameLogo);

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

            #region customerView
            customerView = new UIView();
            customerView.BackgroundColor = UIColor.White;
            customerView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblCustomerName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCustomerName.Font = lblCustomerName.Font.WithSize(15);
            lblCustomerName.Text = Utils.TextBundle("customername", "Customer Name");
            customerView.AddSubview(lblCustomerName);

            txtCustomerName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtCustomerName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("customername", "Customer Name"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtCustomerName.ReturnKeyType = UIReturnKeyType.Next;
            txtCustomerName.ShouldReturn = (tf) =>
            {
                txtPhone.BecomeFirstResponder();
                return true;
            };
            txtCustomerName.Font = txtCustomerName.Font.WithSize(15);
            txtCustomerName.EditingChanged += (object sender, EventArgs e) =>
            {
                Editchange = true;
                var CustomerName = txtCustomerName.Text.Trim();


                if (CustomerName.Length >= 8)
                {
                    txtShortName.Text = CustomerName.Substring(0, 7);
                    lblShortNameLogo.Text = CustomerName.Substring(0, 7);
                }
                else
                {
                    txtShortName.Text = CustomerName;
                    lblShortNameLogo.Text = CustomerName;
                }
                if (editedImage != null || !string.IsNullOrEmpty(this.Customer.ThumbnailLocalPath))
                {
                    profileImg.BackgroundColor = UIColor.FromRGB(226, 226, 226);
                }

                Changebtn();
            };
            customerView.AddSubview(txtCustomerName);
            #endregion

            line0 = new UIView();
            line0.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            line0.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(line0);

            #region GenderView
            GenderView = new UIView();
            GenderView.Hidden = true;
            GenderView.BackgroundColor = UIColor.White;
            GenderView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblGender = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblGender.Font = lblGender.Font.WithSize(15);
            lblGender.Text = Utils.TextBundle("sex", "Gender");
            GenderView.AddSubview(lblGender);

            txtGender = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtGender.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("sex", "Gender"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtGender.Font = txtGender.Font.WithSize(15);
            GenderView.AddSubview(txtGender);

            btnSelectGender = new UIImageView();
            btnSelectGender.Image = UIImage.FromBundle("Next");
            btnSelectGender.TranslatesAutoresizingMaskIntoConstraints = false;
            GenderView.AddSubview(btnSelectGender);
            #endregion

            line1 = new UIView();
            line1.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            line1.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(line1);

            #region CustomerTypeView
            CustomerTypeView = new UIView();
            CustomerTypeView.BackgroundColor = UIColor.White;
            CustomerTypeView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblCusType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblCusType.Font = lblCusType.Font.WithSize(15);
            lblCusType.Text = "Member Type";
            CustomerTypeView.AddSubview(lblCusType);

            txtCusType = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtCusType.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("membertype", "Member Type"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtCusType.Font = txtGender.Font.WithSize(15);
            CustomerTypeView.AddSubview(txtCusType);

            btnCustype = new UIImageView();
            btnCustype.Image = UIImage.FromBundle("Next");
            btnCustype.TranslatesAutoresizingMaskIntoConstraints = false;
            CustomerTypeView.AddSubview(btnCustype);

            btnCustype.UserInteractionEnabled = true;

            #endregion

            line2 = new UIView();
            line2.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            line2.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.AddSubview(line2);

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

            #region CusIDView
            CusIDView = new UIView();
            CusIDView.Hidden = true;
            CusIDView.BackgroundColor = UIColor.White;
            CusIDView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblCusID = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblCusID.Font = lblCusID.Font.WithSize(15);
            lblCusID.Text = Utils.TextBundle("customerid", "Customer ID");
            CusIDView.AddSubview(lblCusID);

            txtCusID = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtCusID.AttributedPlaceholder = new NSAttributedString("Customer ID", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtCusID.Font = txtCusID.Font.WithSize(15);
            txtCusID.ReturnKeyType = UIReturnKeyType.Next;
            txtCusID.ShouldReturn = (tf) =>
            {
                txtShortName.BecomeFirstResponder();
                return true;
            };
            CusIDView.AddSubview(txtCusID);
            #endregion

            lineD1 = new UIView();
            lineD1.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD1.TranslatesAutoresizingMaskIntoConstraints = false;
            CusIDView.AddSubview(lineD1);

            #region ShortNameView
            ShortNameView = new UIView();
            ShortNameView.Hidden = true;
            ShortNameView.BackgroundColor = UIColor.White;
            ShortNameView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblShortName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblShortName.Font = lblShortName.Font.WithSize(15);
            lblShortName.Text = Utils.TextBundle("shortname", "Short Name");
            ShortNameView.AddSubview(lblShortName);

            txtShortName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtShortName.AttributedPlaceholder = new NSAttributedString("Short Name", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtShortName.Font = txtShortName.Font.WithSize(15);
            txtShortName.ReturnKeyType = UIReturnKeyType.Next;
            txtShortName.ShouldReturn = (tf) =>
            {
                txtIdCard.BecomeFirstResponder();
                return true;
            };
            txtShortName.EditingChanged += (object sender, EventArgs e) =>
            {
                if(txtShortName.Text.Length >= 8)
                {
                    txtShortName.Text = txtShortName.Text.Substring(0, 7);
                    
                }
                else
                {
                    lblShortNameLogo.Text = txtShortName.Text;
                }
            };
            ShortNameView.AddSubview(txtShortName);
            #endregion

            lineD2 = new UIView();
            lineD2.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD2.TranslatesAutoresizingMaskIntoConstraints = false;
            ShortNameView.AddSubview(lineD2);

            #region IDCardView
            IDCardView = new UIView();
            IDCardView.Hidden = true;
            IDCardView.BackgroundColor = UIColor.White;
            IDCardView.TranslatesAutoresizingMaskIntoConstraints = false;
           

            lblIdCard = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  
            };
            lblIdCard.Font = lblIdCard.Font.WithSize(15);
            lblIdCard.Text = Utils.TextBundle("nationalidnumber", "National ID Number");
            IDCardView.AddSubview(lblIdCard);

            txtIdCard = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  
            };
            txtIdCard.EditingChanged += TxtnationalID_TextChanged1;
            txtIdCard.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 17;
            };
            txtIdCard.AttributedPlaceholder = new NSAttributedString("x xxxx xxxxx xx x", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtIdCard.Font = txtIdCard.Font.WithSize(15);
            txtIdCard.InputAccessoryView = NumpadToolbar;
            txtIdCard.KeyboardType = UIKeyboardType.NumberPad;
            IDCardView.AddSubview(txtIdCard);
            #endregion

            lineD3 = new UIView();
            lineD3.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD3.TranslatesAutoresizingMaskIntoConstraints = false;
            IDCardView.AddSubview(lineD3);

            #region BirthView
            BirthView = new UIView();
            BirthView.Hidden = true;
            BirthView.BackgroundColor = UIColor.White;
            BirthView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblBirth = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblBirth.Font = lblBirth.Font.WithSize(15);
            lblBirth.Text = Utils.TextBundle("birthdate", "Birthdate");
            BirthView.AddSubview(lblBirth);

            txtBirth = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtBirth.AttributedPlaceholder = new NSAttributedString("dd/mm/yyyy", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtBirth.Font = txtBirth.Font.WithSize(15);
            BirthView.AddSubview(txtBirth);

            btnSelectBirth = new UIImageView();
            btnSelectBirth.Image = UIImage.FromBundle("Next");
            btnSelectBirth.TranslatesAutoresizingMaskIntoConstraints = false;
            BirthView.AddSubview(btnSelectBirth);
            #endregion

            lineD4 = new UIView();
            lineD4.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD4.TranslatesAutoresizingMaskIntoConstraints = false;
            BirthView.AddSubview(lineD4);

            #region PhoneView
            PhoneView = new UIView();
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
            txtPhone.EditingChanged += TxtPhone_EditingChanged;
            txtPhone.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 12;
            };
            txtPhone.AttributedPlaceholder = new NSAttributedString("021-234-5678", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtPhone.Font = txtPhone.Font.WithSize(15);
            txtPhone.InputAccessoryView = NumpadToolbar;
            txtPhone.KeyboardType = UIKeyboardType.NumberPad;
            PhoneView.AddSubview(txtPhone);
            #endregion

            lineD5 = new UIView();
            lineD5.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD5.TranslatesAutoresizingMaskIntoConstraints = false;
            PhoneView.AddSubview(lineD5);

            #region EmailView
            EmailView = new UIView();
            EmailView.Hidden = true;
            EmailView.BackgroundColor = UIColor.White;
            EmailView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblEmail = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblEmail.Font = lblEmail.Font.WithSize(15);
            lblEmail.Text = Utils.TextBundle("email", "Email");
            EmailView.AddSubview(lblEmail);

            txtEmail = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtEmail.AttributedPlaceholder = new NSAttributedString("email@xxx.com", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtEmail.Font = txtEmail.Font.WithSize(15);
            txtEmail.ReturnKeyType = UIReturnKeyType.Next;
            txtEmail.ShouldReturn = (tf) =>
            {
                txtLine.BecomeFirstResponder();
                return true;
            };
            EmailView.AddSubview(txtEmail);
            #endregion

            lineD6 = new UIView();
            lineD6.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD6.TranslatesAutoresizingMaskIntoConstraints = false;
            EmailView.AddSubview(lineD6);

            #region LineView
            LineView = new UIView();
            LineView.Hidden = true;
            LineView.BackgroundColor = UIColor.White;
            LineView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblLine = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblLine.Font = lblLine.Font.WithSize(15);
            lblLine.Text = Utils.TextBundle("lineid", "Line ID");
            LineView.AddSubview(lblLine);

            txtLine = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtLine.AttributedPlaceholder = new NSAttributedString("Line ID", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtLine.Font = txtLine.Font.WithSize(15);
            txtLine.ReturnKeyType = UIReturnKeyType.Next;
            txtLine.ShouldReturn = (tf) =>
            {
                txtAddress.BecomeFirstResponder();
                return true;
            };
            LineView.AddSubview(txtLine);
            #endregion

            lineD7 = new UIView();
            lineD7.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD7.TranslatesAutoresizingMaskIntoConstraints = false;
            LineView.AddSubview(lineD7);

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
            txtAddress.AttributedPlaceholder = new NSAttributedString("Address", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
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

            lineD8 = new UIView();
            lineD8.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD8.TranslatesAutoresizingMaskIntoConstraints = false;
            AddressView.AddSubview(lineD8);

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
            txtProvince.AttributedPlaceholder = new NSAttributedString("Select Province", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtProvince.Font = txtProvince.Font.WithSize(15);
            ProvinceView.AddSubview(txtProvince);
            
            btnProvince = new UIImageView();
            btnProvince.Image = UIImage.FromBundle("Next");
            btnProvince.TranslatesAutoresizingMaskIntoConstraints = false;
            ProvinceView.AddSubview(btnProvince);
            #endregion

            lineD9 = new UIView();
            lineD9.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD9.TranslatesAutoresizingMaskIntoConstraints = false;
            ProvinceView.AddSubview(lineD9);

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
            txtDistrict.AttributedPlaceholder = new NSAttributedString("Select District", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            DistrictView.AddSubview(txtDistrict);

            btnDistrict = new UIImageView();
            btnDistrict.Image = UIImage.FromBundle("Next");
            btnDistrict.TranslatesAutoresizingMaskIntoConstraints = false;
            DistrictView.AddSubview(btnDistrict);
            #endregion

            lineD10 = new UIView();
            lineD10.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD10.TranslatesAutoresizingMaskIntoConstraints = false;
            DistrictView.AddSubview(lineD10);

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
            txtSubDistrict.AttributedPlaceholder = new NSAttributedString("Select Sub District", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            SubdistrictView.AddSubview(txtSubDistrict);

            btnSubDistrict = new UIImageView();
            btnSubDistrict.Image = UIImage.FromBundle("Next");
            btnSubDistrict.TranslatesAutoresizingMaskIntoConstraints = false;
            SubdistrictView.AddSubview(btnSubDistrict);
            #endregion

            lineD11 = new UIView();
            lineD11.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD11.TranslatesAutoresizingMaskIntoConstraints = false;
            SubdistrictView.AddSubview(lineD11);

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
            txtPostal.Enabled = false;
            txtPostal.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("selectpostalcode", "Postal Code"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
            txtPostal.Font = txtPostal.Font.WithSize(15);
            PostalCodeView.AddSubview(txtPostal);

            
            #endregion

            lineD12 = new UIView();
            lineD12.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD12.TranslatesAutoresizingMaskIntoConstraints = false;
            PostalCodeView.AddSubview(lineD12);

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
            txtComment.AttributedPlaceholder = new NSAttributedString("Comment", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtComment.Font = txtComment.Font.WithSize(15);
            txtComment.ReturnKeyType = UIReturnKeyType.Done;
            CommentView.AddSubview(txtComment);
            #endregion

            lineD13 = new UIView();
            lineD13.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            lineD13.TranslatesAutoresizingMaskIntoConstraints = false;
            CommentView.AddSubview(lineD13);

            #region LinkProView
            LinkProView = new UIView();
            LinkProView.Hidden = true;
            LinkProView.Layer.Opacity = 0.7f;
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
            txtLinkProMax.Enabled = false;
            txtLinkProMax.AttributedPlaceholder = new NSAttributedString("Link ProMaxx ID", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtLinkProMax.ReturnKeyType = UIReturnKeyType.Done;
            txtLinkProMax.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            LinkProView.AddSubview(txtLinkProMax);
            #endregion

            #endregion

            #region bottomView
            bottomView = new UIView();
            bottomView.BackgroundColor = UIColor.Clear;
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
        //    View.AddSubview(bottomView);

            btnAdd = new UIButton();
            btnAdd.SetTitle(Utils.TextBundle("addcustomer", "Add Customer"), UIControlState.Normal);
            btnAdd.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
            btnAdd.Layer.CornerRadius = 5f;
            btnAdd.Layer.BorderWidth = 0.5f;
            btnAdd.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
            btnAdd.BackgroundColor = UIColor.White;
            btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAdd.TouchUpInside += (sender, e) => {
                // sum items
                ClickAdd();
                
            };
            bottomView.AddSubview(btnAdd);

            #endregion

            #region BottomViewEdit
            BottomViewEdit = new UIView();
            BottomViewEdit.Hidden = true;
            BottomViewEdit.BackgroundColor = UIColor.Clear;
            BottomViewEdit.TranslatesAutoresizingMaskIntoConstraints = false;
        //    View.AddSubview(BottomViewEdit);

            btnDelete = new UIButton();
            btnDelete.Layer.CornerRadius = 5f;
            btnDelete.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnDelete.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDelete.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDelete.TouchUpInside += (sender, e) => {
                try
                {
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "OK"), UIAlertControllerStyle.Alert);

                    //Add Actions
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => DeleteCustomer()));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                    //Present Alert
                    PresentViewController(okCancelAlertController, true, null);
                }
                catch (Exception ex)
                {
                    _ = TinyInsights.TrackErrorAsync(ex);
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotdelete", "cannotdelete"));
                    return;
                }
                
            };
            BottomViewEdit.AddSubview(btnDelete);

            btnSaveCategory = new UIButton();
            btnSaveCategory.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSaveCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSaveCategory.Enabled = false;
            btnSaveCategory.Layer.CornerRadius = 5f;
            btnSaveCategory.BackgroundColor = UIColor.FromRGB(51, 172, 225);
            btnSaveCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSaveCategory.TouchUpInside += (sender, e) => {
                // sum items
                UpdateClick();
            };
            BottomViewEdit.AddSubview(btnSaveCategory);
            #endregion

            #endregion


            _contentView.AddSubview(logoView);
            _contentView.AddSubview(customerView);
            _contentView.AddSubview(GenderView);
            _contentView.AddSubview(CustomerTypeView);
            _contentView.AddSubview(BirthView);
            _contentView.AddSubview(DetailToggleView);
            _contentView.AddSubview(LinkProView);
            _contentView.AddSubview(CommentView);
            _contentView.AddSubview(PostalCodeView);
            _contentView.AddSubview(SubdistrictView);
            _contentView.AddSubview(DistrictView);
            _contentView.AddSubview(ProvinceView);
            _contentView.AddSubview(AddressView);
            _contentView.AddSubview(ShortNameView);
            _contentView.AddSubview(IDCardView);
            _contentView.AddSubview(EmailView);
            _contentView.AddSubview(LineView);
            _contentView.AddSubview(CusIDView);
            _contentView.AddSubview(PhoneView);

            _contentView.BringSubviewToFront(line1);
            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
            View.AddSubview(bottomView);
            View.AddSubview(BottomViewEdit);
        }
        private void TxtnationalID_TextChanged1(object sender, EventArgs e)
        {
            try
            {
                var idcard = txtIdCard.Text;
                int textlength = txtIdCard.Text.Length;

                if (Phone.EndsWith(" "))
                    return;

                if (textlength == 2)
                {
                    var index = txtIdCard.Text.LastIndexOf(" ");
                    if (textlength == 2 & index == 1)
                    {
                        idcard.Remove(1, 1);
                    }
                    else
                    {
                        txtIdCard.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                    }
                }
                else if (textlength == 7)
                {
                    var index = txtIdCard.Text.LastIndexOf(" ");
                    if (textlength == 7 & index == 6)
                    {
                        idcard.Remove(6, 1);
                    }
                    else
                    {
                        txtIdCard.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                    }
                }
                else if (textlength == 13)
                {
                    var index = txtIdCard.Text.LastIndexOf(" ");
                    if (textlength == 13 & index == 12)
                    {
                        idcard.Remove(12, 1);
                    }
                    else
                    {
                        txtIdCard.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                    }
                }
                else if (textlength == 16)
                {
                    var index = txtIdCard.Text.LastIndexOf(" ");
                    if (textlength == 16 & index == 15)
                    {
                        idcard.Remove(15, 1);
                    }
                    else
                    {
                        txtIdCard.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                    }
                }
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        private void TxtPhone_EditingChanged(object sender, EventArgs e)
        {
            try
            {
                Editchange = true;
                Phone = txtPhone.Text;
                int textlength = txtPhone.Text.Length;

                if (Phone.EndsWith(" "))
                    return;

                if (Phone.StartsWith("02"))
                {
                    if (Phone.Length == 12)
                    {
                        Phone = Phone.Remove(Phone.Length - 1);
                        txtPhone.Text = Phone;
                        //txtPhone.SetSelection(txtPhone.Text.Length);
                    }
                    if (textlength == 3)
                    {
                        var index = txtPhone.Text.LastIndexOf("-");
                        if (textlength == 3 & index == 2)
                        {
                            Phone.Remove(2, 1);
                        }
                        else
                        {
                            txtPhone.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            //txtPhone.SetSelection(txtPhone.Text.Length);
                        }
                    }
                    else if (textlength == 7)
                    {
                        var index = txtPhone.Text.LastIndexOf("-");
                        if (textlength == 7 & index == 6)
                        {
                            Phone.Remove(6, 1);
                        }
                        else
                        {
                            txtPhone.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            //txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                        }
                    }
                }
                else if (Phone.StartsWith("03") | Phone.StartsWith("04") | Phone.StartsWith("05") | Phone.StartsWith("07"))
                {
                    if (Phone.Length == 12)
                    {
                        Phone = Phone.Remove(Phone.Length - 1);
                        txtPhone.Text = Phone;
                        //txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                    }
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
                            //txtphonenumber.SetSelection(txtphonenumber.Text.Length);
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
                            //txtPhone.SetSelection(txtPhone.Text.Length);
                        }
                    }
                }
                else
                {
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
                            //txtPhone.SetSelection(txtPhone.Text.Length);
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
                            //txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                        }
                    }
                }

                //Phone = txtPhone.Text;
                //int textlength = txtPhone.Text.Length;

                //if (Phone.EndsWith(" "))
                //    return;

                //if (textlength == 4)
                //{
                //    var index = txtPhone.Text.LastIndexOf("-");
                //    if (textlength == 4 & index == 3)
                //    {
                //        Phone.Remove(3, 1);
                //    }
                //    else
                //    {
                //        txtPhone.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                //    }
                //}
                //else if (textlength == 8)
                //{
                //    var index = txtPhone.Text.LastIndexOf("-");
                //    if (textlength == 8 & index == 7)
                //    {
                //        Phone.Remove(7, 1);
                //    }
                //    else
                //    {
                //        txtPhone.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                //    }
                //}
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        private void Changebtn()
        {
            btnAdd.Enabled = true;
            btnAdd.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnAdd.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSaveCategory.Enabled = true;
            btnSaveCategory.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSaveCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
        }

        private async void UpdateClick()
        {
            try
            {
                //var birth = txtBirth.Text;

                if (string.IsNullOrEmpty(txtCustomerName.Text))
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("enternamecustomer", "กรุณากรอกชื่อผู้ใช้"));
                    return;
                }
                if (!Utils.IsEmail(txtEmail.Text.Trim()) && !string.IsNullOrEmpty(txtEmail.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("enteremail", "กรุณากรอก Email ให้ถูกต้อง"));
                    return;
                }
                if ((txtPhone.Text.Replace("-", "").Length != 10 && txtPhone.Text.Replace("-", "").Length != 9) && !string.IsNullOrEmpty(txtPhone.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("entermoblie", "Report"));
                    return;
                }


                CultureInfo culture = new CultureInfo("en-US");

                DateTime dateTime = new DateTime();
                dateTime = DateTime.UtcNow;
                string datenow = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));


                DateTime birthdate;

                //BirthDate  type DATE
                

                Gabana.ORM.MerchantDB.Customer customer = new Customer();

                if (editedImage != null)
                {
                    var namepic = DateTime.UtcNow.Ticks.ToString();
                    var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var libFolder = Path.Combine(docFolder,"..", "Library", DataCashingAll.MerchantId.ToString(), "picture");
                    var filePath = Path.Combine(libFolder, txtCustomerName.Text + ".png");
                    var FullfilePath = Path.Combine(docFolder,libFolder, namepic + ".png");
                    if (!Directory.Exists(libFolder))
                    {
                        Directory.CreateDirectory(libFolder);
                    }
                    NSData data = editedImage.AsPNG();
                    var _picture = new byte[data.Length];
                    System.Runtime.InteropServices.Marshal.Copy(data.Bytes, _picture, 0, Convert.ToInt32(_picture.Length));
                    File.WriteAllBytes(filePath, _picture);

                    //var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                    var libFolderthumbnail = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "picture", "thumbnail");
                    var filePaththumbnail = Path.Combine(libFolderthumbnail, namepic + ".png");
                    if (!Directory.Exists(libFolderthumbnail))
                    {
                        Directory.CreateDirectory(libFolderthumbnail);
                    }
                    NSData datathumbnail = editedImage.AsPNG();
                    var _picturethumbnail = new byte[datathumbnail.Length];
                    System.Runtime.InteropServices.Marshal.Copy(datathumbnail.Bytes, _picturethumbnail, 0, Convert.ToInt32(_picturethumbnail.Length));
                    File.WriteAllBytes(filePaththumbnail, _picturethumbnail);

                    customer.ThumbnailLocalPath = filePaththumbnail;
                    customer.PictureLocalPath = filePath;
                    //customer.ThumbnailPath = filePaththumbnail;

                    
                }
                else
                {

                    customer.ThumbnailLocalPath = this.Customer.ThumbnailLocalPath;
                    customer.PictureLocalPath = this.Customer.PictureLocalPath;
                    customer.ThumbnailPath = this.Customer.ThumbnailPath;
                    customer.PicturePath = this.Customer.PicturePath;
                    
                }

                customer.ShortName = txtShortName.Text;
                //customer.PicturePath = this.Customer.PicturePath;
                customer.MerchantID = this.Customer.MerchantID;
                customer.SysCustomerID = this.Customer.SysCustomerID;
                customer.CustomerName = txtCustomerName.Text;
                customer.Ordinary = this.Customer.Ordinary;
                customer.CustomerID = txtCusID.Text;
                if (txtCustomerName.Text.Length > 8)
                {
                    customer.ShortName = txtCustomerName.Text.Substring(0, 7);
                }
                else
                {
                    customer.ShortName = txtCustomerName.Text;
                }
                
                customer.EMail = txtEmail.Text;
                customer.LineID = txtLine.Text;
                if (txtPhone.Text.Replace("-", "").Length == 0)
                {
                    customer.Mobile = null;
                }
                else
                {
                    customer.Mobile = txtPhone.Text.Replace("-", "");
                }
                customer.Gender = gender;
                if (txtBirth.Text != "")
                {
                    var birth = txtBirth.Text;
                    birth = txtBirth.Text.Replace("/","-");
                    birthdate = Utils.f_String_To_Date_AD(birth);
                    if (birth == datenow)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("enterbrithdate", "Report"));
                        return;
                    }

                    if (birthdate > dateTime)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("enterbrithdate", "Report"));
                        return;
                    }
                    customer.BirthDate = birthdate;
                }
                else
                {
                    customer.BirthDate = null;
                }
                //customer.BirthDate = birthdate;
                customer.Address = txtAddress.Text;
                customer.ProvincesId = idprovice;
                customer.AmphuresId = idamphure;
                customer.DistrictsId = iddistist;

                if (idprovice == 0) customer.ProvincesId = null;
                if (idamphure == 0) customer.AmphuresId = null;
                if (iddistist == 0) customer.DistrictsId = null;
                
                if (txtIdCard.Text != "")
                {
                    var id = txtIdCard.Text.Replace(" ", "");
                    if (id.Length == 13)
                    {
                        customer.IDCard = id;
                    }
                    else
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("enteridcart", "Report"));
                        return;
                    }
                }
                else
                {
                    customer.IDCard = "";
                }
                customer.Comments = txtComment.Text;
                customer.LastDateModified = DateTime.UtcNow;
                customer.UserLastModified = DataCashingAll.MerchantId.ToString();
                customer.LinkProMaxxID = txtLinkProMax.Text;
                customer.DataStatus = 'M';
                customer.FWaitSending = 1;
                customer.WaitSendingTime = DateTime.UtcNow;
                if (CustomerTypeNo != 0)
                {
                    customer.MemberTypeNo = CustomerTypeNo;
                }
                else
                {
                    customer.MemberTypeNo = null; 
                }
                //customer.MemberTypeNo = CustomerTypeNo;

                var UpdateCustomer = await CustomerManage.UpdateCustomer(customer);
                if (UpdateCustomer)
                {
                    Editchange = false;
                    this.NavigationController.PopViewController(false);
                    Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "Report"));
                    CustomerController.Ismodify = true;
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                    }
                }

                if (!UpdateCustomer)
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "Report"));
                }
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                }
                else
                {
                    customer.FWaitSending = 2;
                    await CustomerManage.UpdateCustomer(customer);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(Utils.TextBundle("cannotedit", "Report"));
            }
        }
        private async void ClickAdd()
        {
            try
            {
                

                if (string.IsNullOrEmpty(txtCustomerName.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("enternamecustomer", "Report"));
                    return;
                }

                if ((txtPhone.Text.Replace("-", "").Length != 10 && txtPhone.Text.Replace("-", "").Length != 9) && !string.IsNullOrEmpty(txtPhone.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("entermoblie", "Report"));
                    return;
                }

                var x = Utils.IsEmail(txtEmail.Text.Trim());
                if (!Utils.IsEmail(txtEmail.Text.Trim()) && !string.IsNullOrEmpty(txtEmail.Text) )
                {
                    Utils.ShowMessage(Utils.TextBundle("enteremail", "Report"));
                    return;
                }
                CultureInfo culture = new CultureInfo("en-US");

                DateTime dateTime = new DateTime();
                dateTime = DateTime.UtcNow;
                string datenow = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));


                DateTime birthdate ;
                
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 50);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                Gabana.ORM.MerchantDB.Customer customer = new Customer();
                customer.ShortName = txtShortName.Text;

                if (editedImage != null)
                {
                    var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                    var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var libFolder = Path.Combine(docFolder,"..", "Library", DataCashingAll.MerchantId.ToString(), "Picture");
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

                    //var thumbnail = editedImage.Scale(new CoreGraphics.CGSize(200, 200));
                    var libFolderthumbnail = Path.Combine(docFolder, "..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail");
                    var filePaththumbnail = Path.Combine("..", "Library", DataCashingAll.MerchantId.ToString(), "Picture", "thumbnail", namepic + ".png");
                    var FullfilePaththumbnail = Path.Combine(docFolder, libFolderthumbnail, namepic + ".png");
                    if (!Directory.Exists(libFolderthumbnail))
                    {
                        Directory.CreateDirectory(libFolderthumbnail);
                    }
                    NSData datathumbnail = editedImage.AsPNG();
                    var _picturethumbnail = new byte[datathumbnail.Length];
                    System.Runtime.InteropServices.Marshal.Copy(datathumbnail.Bytes, _picturethumbnail, 0, Convert.ToInt32(_picturethumbnail.Length));
                    File.WriteAllBytes(FullfilePaththumbnail, _picturethumbnail);


                    customer.ThumbnailLocalPath = filePaththumbnail;
                    customer.PictureLocalPath = filePath;

                    
                }
                else
                {
                    customer.ThumbnailLocalPath = null;
                    customer.PictureLocalPath = null;
                    customer.PicturePath = null;
                    customer.ThumbnailPath = null;
                }
                


                
                customer.MerchantID = MainController.merchantlocal.MerchantID;
                customer.SysCustomerID = long.Parse(sys);
                customer.CustomerName = txtCustomerName.Text;
                customer.Ordinary = 1;
                customer.CustomerID = txtCusID.Text;
                if (txtCustomerName.Text.Length > 8)
                {
                    customer.ShortName = txtCustomerName.Text.Substring(0, 7);
                }
                else
                {
                    customer.ShortName = txtCustomerName.Text;
                }
                customer.EMail = txtEmail.Text;
                customer.LineID = txtLine.Text;
                if (txtPhone.Text.Replace("-", "").Length == 0)
                {
                    customer.Mobile = null;
                }
                else
                {
                    customer.Mobile = txtPhone.Text.Replace("-", "");
                }
               
                customer.Gender = gender;

                if (txtBirth.Text != "")
                {
                    var birth = txtBirth.Text;
                    birth=birth.Replace("/", "-");
                    birthdate = Utils.f_String_To_Date_AD(birth);
                    if (birth == datenow)
                    {
                        Utils.ShowMessage(Utils.TextBundle("enterbrithdate", "Report"));
                    }

                    if (birthdate > dateTime)
                    {
                        Utils.ShowMessage(Utils.TextBundle("enterbrithdate", "Report"));
                    }
                    customer.BirthDate = birthdate;
                }
                else
                {
                    customer.BirthDate = null;
                }
                
                customer.Address = txtAddress.Text;

                customer.ProvincesId = idprovice;
                customer.AmphuresId = idamphure;
                customer.DistrictsId = iddistist;

                if (idprovice == 0) customer.ProvincesId = null;
                if (idamphure == 0) customer.AmphuresId = null;
                if (iddistist == 0) customer.DistrictsId = null;


               
                if (txtIdCard.Text != "")
                {
                    var id = txtIdCard.Text.Replace(" ", "");

                    if (id.Length == 13)
                    {
                        customer.IDCard = id;
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("enteridcart", "Report"));
                        return;
                    }
                }
                else
                {
                    customer.IDCard = "";
                }
                
                customer.Comments = txtComment.Text;
                customer.LastDateModified = DateTime.UtcNow;
                customer.UserLastModified = DataCashingAll.MerchantId.ToString();
                customer.LinkProMaxxID = txtLinkProMax.Text.Trim();
                customer.DataStatus = 'I';
                customer.FWaitSending = 1;
                customer.WaitSendingTime = DateTime.UtcNow;
                if (CustomerTypeNo != 0)
                {
                    customer.MemberTypeNo = CustomerTypeNo;
                }
                else
                {
                    customer.MemberTypeNo = null;
                }
               

                var insertCustomer = await CustomerManage.InsertCustomer(customer);
                if (insertCustomer)
                {
                    Editchange = false;
                    this.NavigationController.PopViewController(false);
                    Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Report"));
                    CustomerController.Ismodify = true;
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                    }
                    else
                    {
                        customer.FWaitSending = 2;
                        await CustomerManage.UpdateCustomer(customer);
                    }
                }

                if (!insertCustomer)
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotsave", "Report"));
                }

                

            }
            catch (Exception ex)
            {
                await Utils.ReloadInitialData();
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(Utils.TextBundle("cannotsave", "Report"));
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
        public async System.Threading.Tasks.Task SetupCustomerData()
        {

            lblShortNameLogo.Text = this.Customer.ShortName;
            if (!string.IsNullOrEmpty( this.Customer.ThumbnailLocalPath))
            {
                var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                profileImg.Image = UIImage.FromFile(Path.Combine(docFolder, this.Customer.ThumbnailLocalPath));
                lblShortNameLogo.Hidden = true;
            }
            else
            {
                profileImg.Image = UIImage.FromFile("defaultcust.png");
            }
            txtCustomerName.Text = this.Customer.CustomerName;
            txtCusID.Text = this.Customer.CustomerID?.ToString();
            txtEmail.Text = this.Customer.EMail;
            txtShortName.Text = this.Customer.ShortName;
            if (!string.IsNullOrEmpty( this.Customer.Mobile))
            {
                //var phone = this.Customer.Mobile.Insert(3, "-");
                //phone = phone.Insert(7, "-");
                txtPhone.Text = addTextTel(this.Customer.Mobile);
            }
            
            if (this.Customer.MemberTypeNo!=null)
            {
                txtCusType.Text = memberTypes.Where(x => x.MemberTypeNo == this.Customer.MemberTypeNo).FirstOrDefault().MemberTypeName;
                CustomerTypeNo = (int)this.Customer.MemberTypeNo;
            }
            
            
            
            gender = this.Customer.Gender;
            if (Customer.BirthDate != null)
            {
                txtBirth.Text = Utils.f_Date_To_String_AD(Utils.GetTranDate((DateTime)Customer.BirthDate)).Replace("-", "/");
                var currentDate = (DateTime)Customer.BirthDate;
                var x = (NSDate)DateTime.SpecifyKind((DateTime)Customer.BirthDate, DateTimeKind.Utc);
                //var x1 = NSDate.CreateFromSRAbsoluteTime(Customer.BirthDate.Value.Ticks);
                datePickerView.Date = x;
                //datePickerView.Select(x);
            }
            txtAddress.Text = this.Customer.Address;
            txtLine.Text = this.Customer.LineID;
            txtIdCard.Text = this.Customer.IDCard;
            txtComment.Text = this.Customer.Comments;
            txtLinkProMax.Text = this.Customer.LinkProMaxxID;
        }

        #region Picture
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
            
            
            // if it was an image, get the other image info
            if (isImage)
            {
                var x = e.Info[UIImagePickerController.OriginalImage];

                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {
                   

                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
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
                lblShortNameLogo.Hidden = true;

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
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(bottomView.TopAnchor, 0).Active = true;

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
               logoView.HeightAnchor.ConstraintEqualTo(150).Active = true;
           // logoView.HeightAnchor.ConstraintEqualTo(0).Active = true;

            profileImg.CenterXAnchor.ConstraintEqualTo(profileImg.Superview.CenterXAnchor).Active = true;
            profileImg.CenterYAnchor.ConstraintEqualTo(profileImg.Superview.CenterYAnchor).Active = true;
            profileImg.HeightAnchor.ConstraintEqualTo(100).Active = true;
            profileImg.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblShortNameLogo.CenterXAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblShortNameLogo.CenterYAnchor.ConstraintEqualTo(profileImg.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblShortNameLogo.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblShortNameLogo.WidthAnchor.ConstraintEqualTo(70).Active = true;


            btnChangeImage.RightAnchor.ConstraintEqualTo(profileImg.RightAnchor, 0).Active = true;
            btnChangeImage.BottomAnchor.ConstraintEqualTo(profileImg.BottomAnchor, 0).Active = true;
            btnChangeImage.HeightAnchor.ConstraintEqualTo(36).Active = true;
            btnChangeImage.WidthAnchor.ConstraintEqualTo(36).Active = true;
            #endregion

            line.TopAnchor.ConstraintEqualTo(logoView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            line.LeftAnchor.ConstraintEqualTo(line.Superview.LeftAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(5).Active = true;
            line.RightAnchor.ConstraintEqualTo(line.Superview.RightAnchor,0).Active = true;

            #region customerView
            customerView.TopAnchor.ConstraintEqualTo(line.BottomAnchor, 0).Active = true;
            customerView.LeftAnchor.ConstraintEqualTo(customerView.Superview.LeftAnchor, 0).Active = true;
            customerView.RightAnchor.ConstraintEqualTo(customerView.Superview.RightAnchor, 0).Active = true;
            customerView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblCustomerName.TopAnchor.ConstraintEqualTo(lblCustomerName.Superview.TopAnchor, 11).Active = true;
            lblCustomerName.LeftAnchor.ConstraintEqualTo(lblCustomerName.Superview.LeftAnchor, 15).Active = true;
            lblCustomerName.RightAnchor.ConstraintEqualTo(lblCustomerName.Superview.RightAnchor, -20).Active = true;
            lblCustomerName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtCustomerName.TopAnchor.ConstraintEqualTo(lblCustomerName.BottomAnchor, 0).Active = true;
            txtCustomerName.LeftAnchor.ConstraintEqualTo(txtCustomerName.Superview.LeftAnchor, 15).Active = true;
            txtCustomerName.RightAnchor.ConstraintEqualTo(txtCustomerName.Superview.RightAnchor, -20).Active = true;
            txtCustomerName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            line0.BottomAnchor.ConstraintEqualTo(customerView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line0.LeftAnchor.ConstraintEqualTo(line0.Superview.LeftAnchor, 0).Active = true;
            line0.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            line0.RightAnchor.ConstraintEqualTo(line0.Superview.RightAnchor, 0).Active = true;

            #region PhoneView
            PhoneView.TopAnchor.ConstraintEqualTo(customerView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
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

            line1.BottomAnchor.ConstraintEqualTo(PhoneView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line1.LeftAnchor.ConstraintEqualTo(line1.Superview.LeftAnchor, 0).Active = true;
            line1.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            line1.RightAnchor.ConstraintEqualTo(line1.Superview.RightAnchor, 0).Active = true;

            #region CustomerTypeView
            CustomerTypeView.TopAnchor.ConstraintEqualTo(line1.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CustomerTypeView.LeftAnchor.ConstraintEqualTo(CustomerTypeView.Superview.LeftAnchor, 0).Active = true;
            CustomerTypeView.RightAnchor.ConstraintEqualTo(CustomerTypeView.Superview.RightAnchor, 0).Active = true;
            CustomerTypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblCusType.TopAnchor.ConstraintEqualTo(lblCusType.Superview.TopAnchor, 11).Active = true;
            lblCusType.LeftAnchor.ConstraintEqualTo(lblCusType.Superview.LeftAnchor, 15).Active = true;
            lblCusType.RightAnchor.ConstraintEqualTo(lblCusType.Superview.RightAnchor, -20).Active = true;
            lblCusType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtCusType.TopAnchor.ConstraintEqualTo(lblCusType.BottomAnchor, 0).Active = true;
            txtCusType.LeftAnchor.ConstraintEqualTo(txtCusType.Superview.LeftAnchor, 15).Active = true;
            txtCusType.RightAnchor.ConstraintEqualTo(txtCusType.Superview.RightAnchor, -20).Active = true;
            txtCusType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnCustype.CenterYAnchor.ConstraintEqualTo(btnCustype.Superview.CenterYAnchor).Active = true;
            btnCustype.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnCustype.RightAnchor.ConstraintEqualTo(btnCustype.Superview.RightAnchor, -15).Active = true;
            btnCustype.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            line2.TopAnchor.ConstraintEqualTo(CustomerTypeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line2.LeftAnchor.ConstraintEqualTo(line2.Superview.LeftAnchor, 0).Active = true;
            line2.HeightAnchor.ConstraintEqualTo(5).Active = true;
            line2.RightAnchor.ConstraintEqualTo(line2.Superview.RightAnchor, 0).Active = true;

            #region DetailToggleView
            DetailToggleView.TopAnchor.ConstraintEqualTo(line2.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
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

            line3.BottomAnchor.ConstraintEqualTo(DetailToggleView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line3.LeftAnchor.ConstraintEqualTo(line3.Superview.LeftAnchor, 0).Active = true;
            line3.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            line3.RightAnchor.ConstraintEqualTo(line3.Superview.RightAnchor, 0).Active = true;

            #region CusIDView
            CusIDView.TopAnchor.ConstraintEqualTo(DetailToggleView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CusIDView.LeftAnchor.ConstraintEqualTo(CusIDView.Superview.LeftAnchor, 0).Active = true;
            CusIDView.RightAnchor.ConstraintEqualTo(CusIDView.Superview.RightAnchor, 0).Active = true;
            CusIDView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblCusID.TopAnchor.ConstraintEqualTo(lblCusID.Superview.TopAnchor, 11).Active = true;
            lblCusID.LeftAnchor.ConstraintEqualTo(lblCusID.Superview.LeftAnchor, 15).Active = true;
            lblCusID.RightAnchor.ConstraintEqualTo(lblCusID.Superview.RightAnchor, -20).Active = true;
            lblCusID.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtCusID.TopAnchor.ConstraintEqualTo(lblCusID.BottomAnchor, 0).Active = true;
            txtCusID.LeftAnchor.ConstraintEqualTo(txtCusID.Superview.LeftAnchor, 15).Active = true;
            txtCusID.RightAnchor.ConstraintEqualTo(txtCusID.Superview.RightAnchor, -20).Active = true;
            txtCusID.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            lineD1.BottomAnchor.ConstraintEqualTo(CusIDView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD1.LeftAnchor.ConstraintEqualTo(lineD1.Superview.LeftAnchor, 0).Active = true;
            lineD1.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD1.RightAnchor.ConstraintEqualTo(lineD1.Superview.RightAnchor, 0).Active = true;

            #region ShortNameView
            ShortNameView.TopAnchor.ConstraintEqualTo(CusIDView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            ShortNameView.LeftAnchor.ConstraintEqualTo(ShortNameView.Superview.LeftAnchor, 0).Active = true;
            ShortNameView.RightAnchor.ConstraintEqualTo(ShortNameView.Superview.RightAnchor, 0).Active = true;
            ShortNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblShortName.TopAnchor.ConstraintEqualTo(lblShortName.Superview.TopAnchor, 11).Active = true;
            lblShortName.LeftAnchor.ConstraintEqualTo(lblShortName.Superview.LeftAnchor, 15).Active = true;
            lblShortName.RightAnchor.ConstraintEqualTo(lblShortName.Superview.RightAnchor, -20).Active = true;
            lblShortName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtShortName.TopAnchor.ConstraintEqualTo(lblShortName.BottomAnchor, 0).Active = true;
            txtShortName.LeftAnchor.ConstraintEqualTo(txtShortName.Superview.LeftAnchor, 15).Active = true;
            txtShortName.RightAnchor.ConstraintEqualTo(txtShortName.Superview.RightAnchor, -20).Active = true;
            txtShortName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            lineD2.BottomAnchor.ConstraintEqualTo(ShortNameView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD2.LeftAnchor.ConstraintEqualTo(lineD2.Superview.LeftAnchor, 0).Active = true;
            lineD2.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD2.RightAnchor.ConstraintEqualTo(lineD2.Superview.RightAnchor, 0).Active = true;

            #region IDCardView
            IDCardView.TopAnchor.ConstraintEqualTo(ShortNameView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            IDCardView.LeftAnchor.ConstraintEqualTo(IDCardView.Superview.LeftAnchor, 0).Active = true;
            IDCardView.RightAnchor.ConstraintEqualTo(IDCardView.Superview.RightAnchor, 0).Active = true;
            IDCardView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblIdCard.TopAnchor.ConstraintEqualTo(lblIdCard.Superview.TopAnchor, 11).Active = true;
            lblIdCard.LeftAnchor.ConstraintEqualTo(lblIdCard.Superview.LeftAnchor, 15).Active = true;
            lblIdCard.RightAnchor.ConstraintEqualTo(lblIdCard.Superview.RightAnchor, -20).Active = true;
            lblIdCard.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtIdCard.TopAnchor.ConstraintEqualTo(lblIdCard.BottomAnchor, 0).Active = true;
            txtIdCard.LeftAnchor.ConstraintEqualTo(txtIdCard.Superview.LeftAnchor, 15).Active = true;
            txtIdCard.RightAnchor.ConstraintEqualTo(txtIdCard.Superview.RightAnchor, -20).Active = true;
            txtIdCard.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            lineD3.BottomAnchor.ConstraintEqualTo(IDCardView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD3.LeftAnchor.ConstraintEqualTo(lineD3.Superview.LeftAnchor, 0).Active = true;
            lineD3.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD3.RightAnchor.ConstraintEqualTo(lineD3.Superview.RightAnchor, 0).Active = true;

            #region BirthView
            BirthView.TopAnchor.ConstraintEqualTo(IDCardView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            BirthView.LeftAnchor.ConstraintEqualTo(BirthView.Superview.LeftAnchor, 0).Active = true;
            BirthView.RightAnchor.ConstraintEqualTo(BirthView.Superview.RightAnchor, 0).Active = true;
            BirthView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblBirth.TopAnchor.ConstraintEqualTo(lblBirth.Superview.TopAnchor, 11).Active = true;
            lblBirth.LeftAnchor.ConstraintEqualTo(lblBirth.Superview.LeftAnchor, 15).Active = true;
            lblBirth.RightAnchor.ConstraintEqualTo(lblBirth.Superview.RightAnchor, -20).Active = true;
            lblBirth.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtBirth.TopAnchor.ConstraintEqualTo(lblBirth.BottomAnchor, 0).Active = true;
            txtBirth.LeftAnchor.ConstraintEqualTo(txtBirth.Superview.LeftAnchor, 15).Active = true;
            txtBirth.RightAnchor.ConstraintEqualTo(txtBirth.Superview.RightAnchor, -20).Active = true;
            txtBirth.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSelectBirth.CenterYAnchor.ConstraintEqualTo(btnSelectBirth.Superview.CenterYAnchor).Active = true;
            btnSelectBirth.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSelectBirth.RightAnchor.ConstraintEqualTo(btnSelectBirth.Superview.RightAnchor, -15).Active = true;
            btnSelectBirth.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            lineD4.BottomAnchor.ConstraintEqualTo(BirthView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD4.LeftAnchor.ConstraintEqualTo(lineD4.Superview.LeftAnchor, 0).Active = true;
            lineD4.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD4.RightAnchor.ConstraintEqualTo(lineD4.Superview.RightAnchor, 0).Active = true;

            #region GenderView
            GenderView.TopAnchor.ConstraintEqualTo(BirthView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            GenderView.LeftAnchor.ConstraintEqualTo(GenderView.Superview.LeftAnchor, 0).Active = true;
            GenderView.RightAnchor.ConstraintEqualTo(GenderView.Superview.RightAnchor, 0).Active = true;
            GenderView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblGender.TopAnchor.ConstraintEqualTo(lblGender.Superview.TopAnchor, 11).Active = true;
            lblGender.LeftAnchor.ConstraintEqualTo(lblGender.Superview.LeftAnchor, 15).Active = true;
            lblGender.RightAnchor.ConstraintEqualTo(lblGender.Superview.RightAnchor, -20).Active = true;
            lblGender.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtGender.TopAnchor.ConstraintEqualTo(lblGender.BottomAnchor, 0).Active = true;
            txtGender.LeftAnchor.ConstraintEqualTo(txtGender.Superview.LeftAnchor, 15).Active = true;
            txtGender.RightAnchor.ConstraintEqualTo(txtGender.Superview.RightAnchor, -20).Active = true;
            txtGender.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSelectGender.CenterYAnchor.ConstraintEqualTo(btnSelectGender.Superview.CenterYAnchor).Active = true;
            btnSelectGender.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSelectGender.RightAnchor.ConstraintEqualTo(btnSelectGender.Superview.RightAnchor, -15).Active = true;
            btnSelectGender.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            lineD5.BottomAnchor.ConstraintEqualTo(GenderView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD5.LeftAnchor.ConstraintEqualTo(lineD5.Superview.LeftAnchor, 0).Active = true;
            lineD5.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD5.RightAnchor.ConstraintEqualTo(lineD5.Superview.RightAnchor, 0).Active = true;

            #region EmailView
            EmailView.TopAnchor.ConstraintEqualTo(GenderView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            EmailView.LeftAnchor.ConstraintEqualTo(EmailView.Superview.LeftAnchor, 0).Active = true;
            EmailView.RightAnchor.ConstraintEqualTo(EmailView.Superview.RightAnchor, 0).Active = true;
            EmailView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblEmail.TopAnchor.ConstraintEqualTo(lblEmail.Superview.TopAnchor, 11).Active = true;
            lblEmail.LeftAnchor.ConstraintEqualTo(lblEmail.Superview.LeftAnchor, 15).Active = true;
            lblEmail.RightAnchor.ConstraintEqualTo(lblEmail.Superview.RightAnchor, -20).Active = true;
            lblEmail.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtEmail.TopAnchor.ConstraintEqualTo(lblEmail.BottomAnchor, 0).Active = true;
            txtEmail.LeftAnchor.ConstraintEqualTo(txtEmail.Superview.LeftAnchor, 15).Active = true;
            txtEmail.RightAnchor.ConstraintEqualTo(txtEmail.Superview.RightAnchor, -20).Active = true;
            txtEmail.HeightAnchor.ConstraintEqualTo(18).Active = true;

            #endregion

            lineD6.BottomAnchor.ConstraintEqualTo(EmailView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD6.LeftAnchor.ConstraintEqualTo(lineD6.Superview.LeftAnchor, 0).Active = true;
            lineD6.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD6.RightAnchor.ConstraintEqualTo(lineD6.Superview.RightAnchor, 0).Active = true;

            #region LineView
            LineView.TopAnchor.ConstraintEqualTo(EmailView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            LineView.LeftAnchor.ConstraintEqualTo(LineView.Superview.LeftAnchor, 0).Active = true;
            LineView.RightAnchor.ConstraintEqualTo(LineView.Superview.RightAnchor, 0).Active = true;
            LineView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblLine.TopAnchor.ConstraintEqualTo(lblLine.Superview.TopAnchor, 11).Active = true;
            lblLine.LeftAnchor.ConstraintEqualTo(lblLine.Superview.LeftAnchor, 15).Active = true;
            lblLine.RightAnchor.ConstraintEqualTo(lblLine.Superview.RightAnchor, -20).Active = true;
            lblLine.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtLine.TopAnchor.ConstraintEqualTo(lblLine.BottomAnchor, 0).Active = true;
            txtLine.LeftAnchor.ConstraintEqualTo(txtLine.Superview.LeftAnchor, 15).Active = true;
            txtLine.RightAnchor.ConstraintEqualTo(txtLine.Superview.RightAnchor, -20).Active = true;
            txtLine.HeightAnchor.ConstraintEqualTo(18).Active = true;

            #endregion

            lineD7.BottomAnchor.ConstraintEqualTo(LineView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD7.LeftAnchor.ConstraintEqualTo(lineD7.Superview.LeftAnchor, 0).Active = true;
            lineD7.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD7.RightAnchor.ConstraintEqualTo(lineD7.Superview.RightAnchor, 0).Active = true;

            #region AddressView
            AddressView.TopAnchor.ConstraintEqualTo(LineView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            AddressView.LeftAnchor.ConstraintEqualTo(AddressView.Superview.LeftAnchor, 0).Active = true;
            AddressView.RightAnchor.ConstraintEqualTo(AddressView.Superview.RightAnchor, 0).Active = true;
            AddressView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblAddress.TopAnchor.ConstraintEqualTo(lblAddress.Superview.TopAnchor, 11).Active = true;
            lblAddress.LeftAnchor.ConstraintEqualTo(lblAddress.Superview.LeftAnchor, 15).Active = true;
            lblAddress.RightAnchor.ConstraintEqualTo(lblAddress.Superview.RightAnchor, -20).Active = true;
            lblAddress.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtAddress.TopAnchor.ConstraintEqualTo(lblAddress.BottomAnchor, 0).Active = true;
            txtAddress.LeftAnchor.ConstraintEqualTo(txtAddress.Superview.LeftAnchor, 15).Active = true;
            txtAddress.RightAnchor.ConstraintEqualTo(txtAddress.Superview.RightAnchor, -20).Active = true;
            txtAddress.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            lineD8.BottomAnchor.ConstraintEqualTo(AddressView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD8.LeftAnchor.ConstraintEqualTo(lineD8.Superview.LeftAnchor, 0).Active = true;
            lineD8.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD8.RightAnchor.ConstraintEqualTo(lineD8.Superview.RightAnchor, 0).Active = true;

            #region ProvinceView
            ProvinceView.TopAnchor.ConstraintEqualTo(AddressView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            ProvinceView.LeftAnchor.ConstraintEqualTo(ProvinceView.Superview.LeftAnchor, 0).Active = true;
            ProvinceView.RightAnchor.ConstraintEqualTo(ProvinceView.Superview.RightAnchor, 0).Active = true;
            ProvinceView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblProvince.TopAnchor.ConstraintEqualTo(lblProvince.Superview.TopAnchor, 11).Active = true;
            lblProvince.LeftAnchor.ConstraintEqualTo(lblProvince.Superview.LeftAnchor, 15).Active = true;
            lblProvince.RightAnchor.ConstraintEqualTo(lblProvince.Superview.RightAnchor, -60).Active = true;
            lblProvince.HeightAnchor.ConstraintEqualTo(18).Active = true;

           // txtProvince.BackgroundColor = UIColor.Blue;
            txtProvince.TopAnchor.ConstraintEqualTo(lblProvince.BottomAnchor, 0).Active = true;
            txtProvince.LeftAnchor.ConstraintEqualTo(txtProvince.Superview.LeftAnchor, 15).Active = true;
            txtProvince.RightAnchor.ConstraintEqualTo(txtProvince.Superview.RightAnchor, -60).Active = true;
            txtProvince.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnProvince.CenterYAnchor.ConstraintEqualTo(btnProvince.Superview.CenterYAnchor).Active = true;
            btnProvince.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnProvince.RightAnchor.ConstraintEqualTo(btnProvince.Superview.RightAnchor, -15).Active = true;
            btnProvince.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            lineD9.BottomAnchor.ConstraintEqualTo(ProvinceView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD9.LeftAnchor.ConstraintEqualTo(lineD9.Superview.LeftAnchor, 0).Active = true;
            lineD9.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD9.RightAnchor.ConstraintEqualTo(lineD9.Superview.RightAnchor, 0).Active = true;

            #region DistrictView
            DistrictView.TopAnchor.ConstraintEqualTo(ProvinceView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            DistrictView.LeftAnchor.ConstraintEqualTo(DistrictView.Superview.LeftAnchor, 0).Active = true;
            DistrictView.RightAnchor.ConstraintEqualTo(DistrictView.Superview.RightAnchor, 0).Active = true;
            DistrictView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblDistrict.TopAnchor.ConstraintEqualTo(lblDistrict.Superview.TopAnchor, 11).Active = true;
            lblDistrict.LeftAnchor.ConstraintEqualTo(lblDistrict.Superview.LeftAnchor, 15).Active = true;
            lblDistrict.RightAnchor.ConstraintEqualTo(lblDistrict.Superview.RightAnchor, -60).Active = true;
            lblDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtDistrict.TopAnchor.ConstraintEqualTo(lblDistrict.BottomAnchor, 0).Active = true;
            txtDistrict.LeftAnchor.ConstraintEqualTo(txtDistrict.Superview.LeftAnchor, 15).Active = true;
            txtDistrict.RightAnchor.ConstraintEqualTo(txtDistrict.Superview.RightAnchor, -60).Active = true;
            txtDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnDistrict.CenterYAnchor.ConstraintEqualTo(btnDistrict.Superview.CenterYAnchor).Active = true;
            btnDistrict.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnDistrict.RightAnchor.ConstraintEqualTo(btnDistrict.Superview.RightAnchor, -15).Active = true;
            btnDistrict.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            lineD10.BottomAnchor.ConstraintEqualTo(DistrictView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD10.LeftAnchor.ConstraintEqualTo(lineD10.Superview.LeftAnchor, 0).Active = true;
            lineD10.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD10.RightAnchor.ConstraintEqualTo(lineD10.Superview.RightAnchor, 0).Active = true;

            #region SubdistrictView
            SubdistrictView.TopAnchor.ConstraintEqualTo(DistrictView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            SubdistrictView.LeftAnchor.ConstraintEqualTo(SubdistrictView.Superview.LeftAnchor, 0).Active = true;
            SubdistrictView.RightAnchor.ConstraintEqualTo(SubdistrictView.Superview.RightAnchor, 0).Active = true;
            SubdistrictView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblSubDistrict.TopAnchor.ConstraintEqualTo(lblSubDistrict.Superview.TopAnchor, 11).Active = true;
            lblSubDistrict.LeftAnchor.ConstraintEqualTo(lblSubDistrict.Superview.LeftAnchor, 15).Active = true;
            lblSubDistrict.RightAnchor.ConstraintEqualTo(lblSubDistrict.Superview.RightAnchor, -60).Active = true;
            lblSubDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtSubDistrict.TopAnchor.ConstraintEqualTo(lblSubDistrict.BottomAnchor, 0).Active = true;
            txtSubDistrict.LeftAnchor.ConstraintEqualTo(txtSubDistrict.Superview.LeftAnchor, 15).Active = true;
            txtSubDistrict.RightAnchor.ConstraintEqualTo(txtSubDistrict.Superview.RightAnchor, -60).Active = true;
            txtSubDistrict.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSubDistrict.CenterYAnchor.ConstraintEqualTo(btnSubDistrict.Superview.CenterYAnchor).Active = true;
            btnSubDistrict.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSubDistrict.RightAnchor.ConstraintEqualTo(btnSubDistrict.Superview.RightAnchor, -15).Active = true;
            btnSubDistrict.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            lineD11.BottomAnchor.ConstraintEqualTo(SubdistrictView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD11.LeftAnchor.ConstraintEqualTo(lineD11.Superview.LeftAnchor, 0).Active = true;
            lineD11.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD11.RightAnchor.ConstraintEqualTo(lineD11.Superview.RightAnchor, 0).Active = true;

            #region PostalCodeView
            PostalCodeView.TopAnchor.ConstraintEqualTo(SubdistrictView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            PostalCodeView.LeftAnchor.ConstraintEqualTo(PostalCodeView.Superview.LeftAnchor, 0).Active = true;
            PostalCodeView.RightAnchor.ConstraintEqualTo(PostalCodeView.Superview.RightAnchor, 0).Active = true;
            PostalCodeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblPostal.TopAnchor.ConstraintEqualTo(lblPostal.Superview.TopAnchor, 11).Active = true;
            lblPostal.LeftAnchor.ConstraintEqualTo(lblPostal.Superview.LeftAnchor, 15).Active = true;
            lblPostal.RightAnchor.ConstraintEqualTo(lblPostal.Superview.RightAnchor, -60).Active = true;
            lblPostal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtPostal.TopAnchor.ConstraintEqualTo(lblPostal.BottomAnchor, 0).Active = true;
            txtPostal.LeftAnchor.ConstraintEqualTo(txtPostal.Superview.LeftAnchor, 15).Active = true;
            txtPostal.RightAnchor.ConstraintEqualTo(txtPostal.Superview.RightAnchor, -60).Active = true;
            txtPostal.HeightAnchor.ConstraintEqualTo(18).Active = true;

            
            #endregion

            lineD12.BottomAnchor.ConstraintEqualTo(PostalCodeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD12.LeftAnchor.ConstraintEqualTo(lineD12.Superview.LeftAnchor, 0).Active = true;
            lineD12.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD12.RightAnchor.ConstraintEqualTo(lineD12.Superview.RightAnchor, 0).Active = true;

            #region CommentView
            CommentView.TopAnchor.ConstraintEqualTo(PostalCodeView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            CommentView.LeftAnchor.ConstraintEqualTo(CommentView.Superview.LeftAnchor, 0).Active = true;
            CommentView.RightAnchor.ConstraintEqualTo(CommentView.Superview.RightAnchor, 0).Active = true;
            CommentView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblComment.TopAnchor.ConstraintEqualTo(lblComment.Superview.TopAnchor, 11).Active = true;
            lblComment.LeftAnchor.ConstraintEqualTo(lblComment.Superview.LeftAnchor, 15).Active = true;
            lblComment.RightAnchor.ConstraintEqualTo(lblComment.Superview.RightAnchor, -60).Active = true;
            lblComment.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtComment.TopAnchor.ConstraintEqualTo(lblComment.BottomAnchor, 0).Active = true;
            txtComment.LeftAnchor.ConstraintEqualTo(txtComment.Superview.LeftAnchor, 15).Active = true;
            txtComment.RightAnchor.ConstraintEqualTo(txtComment.Superview.RightAnchor, -60).Active = true;
            txtComment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            lineD13.BottomAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            lineD13.LeftAnchor.ConstraintEqualTo(lineD13.Superview.LeftAnchor, 0).Active = true;
            lineD13.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            lineD13.RightAnchor.ConstraintEqualTo(lineD13.Superview.RightAnchor, 0).Active = true;

            #region LinkProView
            LinkProView.TopAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            LinkProView.LeftAnchor.ConstraintEqualTo(LinkProView.Superview.LeftAnchor, 0).Active = true;
            LinkProView.RightAnchor.ConstraintEqualTo(LinkProView.Superview.RightAnchor, 0).Active = true;
            LinkProView.BottomAnchor.ConstraintEqualTo(LinkProView.Superview.BottomAnchor, 0).Active = true;
            LinkProView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblLinkProMax.TopAnchor.ConstraintEqualTo(lblLinkProMax.Superview.TopAnchor, 11).Active = true;
            lblLinkProMax.LeftAnchor.ConstraintEqualTo(lblLinkProMax.Superview.LeftAnchor, 15).Active = true;
            lblLinkProMax.RightAnchor.ConstraintEqualTo(lblLinkProMax.Superview.RightAnchor, -60).Active = true;
            lblLinkProMax.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtLinkProMax.TopAnchor.ConstraintEqualTo(lblLinkProMax.BottomAnchor, 0).Active = true;
            txtLinkProMax.LeftAnchor.ConstraintEqualTo(txtLinkProMax.Superview.LeftAnchor, 15).Active = true;
            txtLinkProMax.RightAnchor.ConstraintEqualTo(txtLinkProMax.Superview.RightAnchor, -60).Active = true;
            txtLinkProMax.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region bottomView
            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAdd.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAdd.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAdd.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAdd.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            BottomViewEdit.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomViewEdit.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomViewEdit.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomViewEdit.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(45).Active = true;

            btnSaveCategory.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSaveCategory.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSaveCategory.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnSaveCategory.RightAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

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
        [Export("CategoryAction")]
        private void DoneAction4()
        {
            txtCusType.ResignFirstResponder();
        }

    }
}