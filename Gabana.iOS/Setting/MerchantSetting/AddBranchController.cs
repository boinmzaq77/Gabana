using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class AddBranchController : UIViewController
    {
        UIScrollView _scrollView;
        public string Phone = "";
        public int idprovice;
        public int idamphure;
        public int iddistist;
        bool flagDetail = false;
        UIButton btndetailtoggle;
        UILabel lbltxtDetail;
        UIView DetailToggleView;
        UIView PhoneView, AddressView, ProvinceView, DistrictView, SubdistrictView, PostalCodeView, CommentView, LinkProView;
        UILabel lblPhone, lblAddress, lblProvince, lblDistrict, lblSubDistrict, lblPostal, lblComment, lblLinkProMax;
        UITextField txtPhone, txtAddress, txtProvince, txtDistrict, txtSubDistrict, txtPostal, txtComment, txtLinkProMax;
        UIImageView btnProvince, btnDistrict, btnSubDistrict, btnPostal;
        UIView _contentView;
        UILabel lblbranchName, lblbranchID;
        UITextField txtbranchName, txtbranchID;
        UIView BranchNameView, BottomView, BranchIDView;
        UIButton btnAddBranch, btnDeleteBranch;
        Branch BranchDetail ;
        PoolManage poolManager = new PoolManage();
        BranchManage BranchManager = new BranchManage();
        public List<Gabana.ORM.PoolDB.Province> province;

        UIToolbar toolbar, toolbar2, toolbar3, toolbarAm, toolbarDis;
        UIBarButtonItem flexible;
        UIBarButtonItem doneButton, doneButton2, doneButton3, doneButton4, doneButton5;
        bool reset = false;
        private UIPickerView AmpurePicker;
        private UIPickerView DistrictPicker;
        private UIPickerView ProvincePicker;
        private UIView Viewblock;
        private bool Editchange = false;

        public AddBranchController() {
        }
        public AddBranchController(Branch detail)
        {
            this.BranchDetail = detail;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", "มีการเปลี่ยนแปลต้องการบันทึกหรือไม่", UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => BtnSave_Click()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => this.NavigationController.PopViewController(true)));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                this.NavigationController.PopViewController(true);
            }
        }

        private void BtnSave_Click()
        {
            if (this.BranchDetail == null)
            {
                //insert
                InsertBranch();
            }
            else
            {
                //update
                UpdatetBranch();
            }
        }

        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            try
            {
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                var view = new UIView();
                var button = new UIButton(UIButtonType.Plain);
                button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                button.SetTitle("   Back", UIControlState.Normal);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -10, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);
                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();
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

                await SetupPicker();

                int sysBranchID = await BranchManager.GetLastBranch();
                int BranchId = sysBranchID + 1;
                txtbranchID.Text = BranchId.ToString();
                
                if (this.BranchDetail !=null)
                {
                    setDataDetail();
                    Utils.SetConstant(btnDeleteBranch.Constraints, NSLayoutAttribute.Width, 50);
                    btnAddBranch.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);


                }
                else
                {
                    Utils.SetConstant(btnDeleteBranch.Constraints, NSLayoutAttribute.Width, 0);
                    
                    btnDeleteBranch.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                    btnDeleteBranch.Hidden = true; 
                    btnAddBranch.SetTitle(Utils.TextBundle("addbranch", "Add Branch"), UIControlState.Normal);
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
                Checkper();
                #endregion
            }
            catch (Exception ex)
            {

                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        private void Checkper()
        {
            var LoginType = Preferences.Get("LoginType", "");
            var check = UtilsAll.CheckPermissionRoleUser(LoginType, "update", "branch");
            if (check)
            {

            }
            else
            {

                View.Alpha = 0.9f;
                Viewblock = new UIView();
                Viewblock.TranslatesAutoresizingMaskIntoConstraints = false;
                Viewblock.BackgroundColor = UIColor.Clear;
                View.AddSubview(Viewblock);
                View.BringSubviewToFront(Viewblock);

                Viewblock.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
                Viewblock.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
                Viewblock.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
                Viewblock.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
                btnDeleteBranch.UserInteractionEnabled = false;
                btnAddBranch.Enabled = false;

            }
        }
        void setDataDetail()
        {
            var provincename = province.Where(x => x.ProvincesId == this.BranchDetail.ProvincesId).FirstOrDefault();

            txtbranchName.Text = this.BranchDetail.BranchName;
            txtbranchID.Text = this.BranchDetail.BranchID;
            if (!string.IsNullOrEmpty(this.BranchDetail.Tel))
            {
                //var phone = this.Customer.Mobile.Insert(3, "-");
                //phone = phone.Insert(7, "-");
                txtPhone.Text = addTextTel(this.BranchDetail.Tel);
            }
            txtAddress.Text = this.BranchDetail.Address;
            txtLinkProMax.Text = this.BranchDetail.LinkProMaxxID;
            txtComment.Text = this.BranchDetail.Comments;

            if (this.BranchDetail.ProvincesId > 0)
            {
                var pro = province.Where(x => x.ProvincesId == this.BranchDetail.ProvincesId).FirstOrDefault();
                var index = province.FindIndex(x => x.ProvincesId == this.BranchDetail.ProvincesId);
                txtProvince.Text = pro.ProvincesNameTH;
                idprovice = (int)pro.ProvincesId;
                setamphure((int?)pro.ProvincesId, (int?)this.BranchDetail.AmphuresId);
                setdistist((int?)this.BranchDetail.AmphuresId, (int?)this.BranchDetail.DistrictsId);
                ProvincePicker.Select(index, 0, false);

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
        private async Task SetupPicker()
        {
            try
            {
                var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);

                

                

                
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


                var amphure = new List<Amphure>() { new Amphure() { AmphuresId = 0, AmphuresNameEN = "District", AmphuresNameTH = "อำเภอ" } };

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
                txtDistrict.Text = Utils.TextBundle("district", "District") ;


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
        private async void setamphure(int? id, int? idamp)
        {
            try
            {
                var amphure = new List<Amphure>() { new Amphure() { AmphuresId = 0, AmphuresNameEN = "District", AmphuresNameTH = "อำเภอ" } };
                if (id != 0 && id != null)
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

                if (idamp > 0 && id != null)
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
            }
        }
        private async void setdistist(int? id, int? iddis)
        {
            try
            {
                var distist = new List<District>() { new District() { DistrictsId = 0, DistrictsNameEN = "Sub District", DistrictsNameTH = "ตำบล" } };
                if (id != 0 && id != null)
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

                if (iddis > 0 && id != null)
                {
                    var index = distist.FindIndex(x => x.DistrictsId == iddis);
                    txtPostal.Text = distist[index].ZipCode;
                    txtSubDistrict.Text = distist[index].DistrictsNameTH;
                    iddistist = (int)distist[index].DistrictsId;
                    DistrictPicker.Select(index, 0, false);
                }
                
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        //private void SetupPicker()
        //{
        //    try
        //    {

        //        PickerprovinceModel provinceM = new PickerprovinceModel(province);
        //        txtDistrict.Enabled = true;
        //        provinceM.PickerChanged += async (sender, e) => {
        //            txtProvince.Text = e.SelectedValue;
        //            reset = true;
        //            setamphure(e.ID);
        //            idprovice = e.ID;
        //            txtDistrict.Enabled = true;
        //        };

        //        var xy = new UIPickerView() { Model = provinceM, ShowSelectionIndicator = true };
        //        txtProvince.InputView = xy;
        //        if (province != null)
        //        {
        //            if (BranchDetail == null || BranchDetail.DistrictsId == null)
        //            {
        //                txtProvince.Text = province[0].ProvincesNameTH;
        //                xy.Select(0, 0, false);
        //            }
        //            else
        //            {
        //                txtProvince.Text = province.Where(c => c.ProvincesId == BranchDetail.ProvincesId).FirstOrDefault().ProvincesNameTH;
        //                var index = province.FindIndex(c => c.ProvincesId == BranchDetail.ProvincesId);
        //                idprovice = (int)province.Where(c => c.ProvincesId == BranchDetail.ProvincesId).FirstOrDefault().ProvincesId;
        //                setamphure(idprovice);
        //                xy.Select((int)index, 0, false);
        //            }
        //        }
        //        txtProvince.InputAccessoryView = toolbar3;
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //    }
        //}
        private async void setamphure(int id)
        {
            try
            {
                var amphure = await poolManager.GetAmphures(Convert.ToInt32(id));
                txtSubDistrict.Enabled = true;

                PickerAmphureModel model2 = new PickerAmphureModel(amphure);
                model2.PickerChanged += async (sender, e) =>
                {
                    reset = true;
                    txtDistrict.Text = e.SelectedValue;
                    setdistist(e.ID);
                    txtSubDistrict.Enabled = true;
                    idamphure = e.ID;
                };
               
                var xy = new UIPickerView() { Model = model2, ShowSelectionIndicator = true };
                txtDistrict.InputView = xy;
                if (amphure != null)
                {
                    if (reset == true)
                    {
                        txtDistrict.Text = amphure[0].AmphuresNameTH;
                        xy.Select(0, 0, false);
                        setdistist((int)amphure[0].AmphuresId);
                        return;
                    }
                    if (BranchDetail == null || BranchDetail.AmphuresId == null)
                    {
                        txtDistrict.Text = amphure[0].AmphuresNameTH;
                        xy.Select(0, 0, false);
                    }
                    else
                    {
                        //id
                        txtDistrict.Text = amphure.Where(c => c.AmphuresId == BranchDetail.AmphuresId).FirstOrDefault().AmphuresNameTH;
                        var index = amphure.FindIndex(c => c.AmphuresId == BranchDetail.AmphuresId);
                        idamphure = (int)amphure.Where(c => c.AmphuresId == BranchDetail.AmphuresId).FirstOrDefault().AmphuresId;
                        setdistist(idamphure);
                        xy.Select((int)index, 0, false);
                    }
                }
                txtDistrict.InputAccessoryView = toolbarAm;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private async void setdistist(int id)
        {
            try
            {
                var distist = await poolManager.GetDistricts(Convert.ToInt32(id));
                txtSubDistrict.Enabled = true;

                PickerDistrictModel model3 = new PickerDistrictModel(distist);
                model3.PickerChanged += async (sender, e) =>
                {
                    txtSubDistrict.Text = e.SelectedValue;
                    iddistist = e.ID;
                    txtPostal.Text = e.ZipCode;
                };
               
                var xy = new UIPickerView() { Model = model3, ShowSelectionIndicator = true };
                txtSubDistrict.InputView = xy;
                if (distist != null)
                {
                    if (reset == true)
                    {
                        txtPostal.Text = distist[0].ZipCode;
                        txtSubDistrict.Text = distist[0].DistrictsNameTH;
                        xy.Select(0, 0, false);
                        reset = false;
                        return;
                    }
                    if (BranchDetail == null || BranchDetail.AmphuresId == null)
                    {
                        txtSubDistrict.Text = distist[0].DistrictsNameTH;
                        txtPostal.Text = distist[0].ZipCode;
                        xy.Select(0, 0, false);
                    }
                    else
                    {
                        //id
                        txtSubDistrict.Text = distist.Where(c => c.DistrictsId == BranchDetail.DistrictsId).FirstOrDefault().DistrictsNameTH;
                        var index = distist.FindIndex(c => c.DistrictsId == BranchDetail.DistrictsId);
                        iddistist = (int)distist.Where(c => c.DistrictsId == BranchDetail.DistrictsId).FirstOrDefault().DistrictsId;
                        txtPostal.Text = distist[index].ZipCode;
                        xy.Select((int)index, 0, false);
                    }
                }
                txtSubDistrict.InputAccessoryView = toolbarDis;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
            //sub district
           

        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region BranchIDView
            BranchIDView = new UIView();
            BranchIDView.BackgroundColor = UIColor.White;
            BranchIDView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblbranchID = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblbranchID.Font = lblbranchID.Font.WithSize(15);
            lblbranchID.Text = Utils.TextBundle("branchid", "Branch ID");
            BranchIDView.AddSubview(lblbranchID);

            txtbranchID = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Enabled = false
            };
            txtbranchID.ReturnKeyType = UIReturnKeyType.Next;
            txtbranchID.ShouldReturn = (tf) =>
            {
                txtbranchName.BecomeFirstResponder();
                return true;
            };
            txtbranchID.EditingChanged += (object sender, EventArgs e) =>
            {
                
            };
            txtbranchID.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("branchid", "Branch ID"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtbranchID.Font = txtbranchID.Font.WithSize(15);
            BranchIDView.AddSubview(txtbranchID);
            #endregion

            #region BranchNameView
            BranchNameView = new UIView();
            BranchNameView.BackgroundColor = UIColor.White;
            BranchNameView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblbranchName = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblbranchName.Font = lblbranchName.Font.WithSize(15);
            lblbranchName.Text = Utils.TextBundle("branchname", "Branch Name"); 
            BranchNameView.AddSubview(lblbranchName);

            txtbranchName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtbranchName.ReturnKeyType = UIReturnKeyType.Done;
            txtbranchName.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtbranchName.EditingChanged += (object sender, EventArgs e) =>
            {
                Editchange = true; 
                ////if (txtbranchName.Text.Length > 0)
                ////{
                ////    btnAddBranch.Enabled = true;
                ////    btnAddBranch.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                ////    btnAddBranch.SetTitleColor(UIColor.White, UIControlState.Normal);
                ////}
                ////else
                ////{
                ////    btnAddBranch.Enabled = false;
                ////    btnAddBranch.BackgroundColor = UIColor.White;
                ////    btnAddBranch.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                ////}
            };
            txtbranchName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("branchname", "Branch Name"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtbranchName.Font = txtbranchName.Font.WithSize(15);
            BranchNameView.AddSubview(txtbranchName);
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
            txtPhone.AttributedPlaceholder = new NSAttributedString("000-000-0000", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtPhone.Font = txtPhone.Font.WithSize(15);
            txtPhone.InputAccessoryView = NumpadToolbar;
            txtPhone.KeyboardType = UIKeyboardType.NumberPad;
            txtPhone.EditingChanged += TxtPhone_EditingChanged;
            txtPhone.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 12;
            };
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
            txtAddress.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("address", "Address"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtAddress.Font = txtAddress.Font.WithSize(15);
            txtAddress.EditingChanged += TxtAddress_EditingChanged;
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
            //txtProvince.AttributedPlaceholder = new NSAttributedString("Select Province", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
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
            lblDistrict.Text = Utils.TextBundle("city", "City");
            DistrictView.AddSubview(lblDistrict);

            txtDistrict = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = txtPhone.Font.WithSize(15)
            };
            txtDistrict.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("selectdistrict", "Select District"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
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
            txtSubDistrict.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("selectsubdistrict", "Select Sub District"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
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
            lblPostal.Text = Utils.TextBundle("postalcode", "Postal Code") ;
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
            txtComment.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("comment", "Comment"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtComment.Font = txtComment.Font.WithSize(15);
            CommentView.AddSubview(txtComment);
            #endregion

            #region LinkProView
            LinkProView = new UIView();
            LinkProView.Hidden = true;
            LinkProView.BackgroundColor = UIColor.White;
            LinkProView.Alpha = 0.5f;
            LinkProView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblLinkProMax = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblLinkProMax.Font = lblLinkProMax.Font.WithSize(15);
            lblLinkProMax.Text = Utils.TextBundle("linkpromaxx", "Link ProMaxx");
            LinkProView.AddSubview(lblLinkProMax);

            txtLinkProMax = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Enabled = false
            };
            txtLinkProMax.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("linkpromaxx", "Link ProMaxx"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(128, 211, 245) });
            txtLinkProMax.Font = txtLinkProMax.Font.WithSize(15);
            txtLinkProMax.ReturnKeyType = UIReturnKeyType.Done;
            txtLinkProMax.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            LinkProView.AddSubview(txtLinkProMax);
            #endregion

            #endregion

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;


            btnAddBranch = new UIButton();
            btnAddBranch.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddBranch.Layer.CornerRadius = 5f;
            btnAddBranch.Layer.BorderWidth = 0.5f;
            btnAddBranch.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            //btnAddBranch.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
            btnAddBranch.BackgroundColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1);
            btnAddBranch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddBranch.TouchUpInside += (sender, e) => {
                btnAddBranch.Enabled = false;
                Editchange = false;
                if (this.BranchDetail == null)
                {
                    //insert
                    InsertBranch();
                }
                else
                {
                    //update
                    UpdatetBranch();
                }
            };
            BottomView.AddSubview(btnAddBranch);

            btnDeleteBranch = new UIButton();
            //btnDeleteBranch.SetTitle("Save", UIControlState.Normal);
            //btnDeleteBranch.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnDeleteBranch.Layer.CornerRadius = 5f;
            btnDeleteBranch.BackgroundColor = UIColor.FromRGB(232, 232, 232);
            btnDeleteBranch.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDeleteBranch.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDeleteBranch.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDeleteBranch.Layer.CornerRadius = 5f;
            //btnDeleteBranch.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnDeleteBranch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDeleteBranch.TouchUpInside += (sender, e) => {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("confirmdeletebranch", "Please enter branch name to confirm deletion.") , UIAlertControllerStyle.Alert);
                //okCancelAlertController = UIAlertController.Create("", "Are you sure you want to delete branch?", UIAlertControllerStyle.);
                okCancelAlertController.AddTextField((obj) =>
                {
                    obj.Font = UIFont.SystemFontOfSize(14);
                    obj.TextAlignment = UITextAlignment.Center;
                    obj.Placeholder = Utils.TextBundle("branchname", "Branch Name") ;

                });
                okCancelAlertController.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Default,
                    alert => DeleteBranch(this.BranchDetail, okCancelAlertController.TextFields[0].Text )));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                //Present Alert
                PresentViewController(okCancelAlertController, true, null);
            };
            
            BottomView.AddSubview(btnDeleteBranch);
            #endregion

            _contentView.AddSubview(BranchIDView);
            _contentView.AddSubview(BranchNameView);

            _contentView.AddSubview(DetailToggleView);
            _contentView.AddSubview(LinkProView);
            _contentView.AddSubview(CommentView);
            _contentView.AddSubview(PostalCodeView);
            _contentView.AddSubview(SubdistrictView);
            _contentView.AddSubview(DistrictView);
            _contentView.AddSubview(ProvinceView);
            _contentView.AddSubview(AddressView);
            _contentView.AddSubview(PhoneView);

            _scrollView.AddSubview(_contentView);
            View.AddSubview(_scrollView);
            View.AddSubview(BottomView);
            BottomView.BringSubviewToFront(btnAddBranch);


            #region toolbar
            flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);

            toolbar = new UIToolbar();
            toolbar.Translucent = true;
            toolbar.SizeToFit();
            doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            });
            toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);

            toolbar2 = new UIToolbar();
            toolbar2.Translucent = true;
            toolbar2.SizeToFit();
            doneButton2 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            }); toolbar2.SetItems(new UIBarButtonItem[] { flexible, doneButton2 }, true);

            toolbar3 = new UIToolbar();
            toolbar3.Translucent = true;
            toolbar3.SizeToFit();
            doneButton3 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
                txtDistrict.BecomeFirstResponder();
            });
            toolbar3.SetItems(new UIBarButtonItem[] { flexible, doneButton3 }, true);

            toolbarAm = new UIToolbar();
            toolbarAm.Translucent = true;
            toolbarAm.SizeToFit();
            doneButton4 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
                txtSubDistrict.BecomeFirstResponder();
            });
            toolbarAm.SetItems(new UIBarButtonItem[] { flexible, doneButton4 }, true);

            toolbarDis = new UIToolbar();
            toolbarDis.Translucent = true;
            toolbarDis.SizeToFit();
            doneButton5 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            });
            toolbarDis.SetItems(new UIBarButtonItem[] { flexible, doneButton5 }, true);
            #endregion
        }

        private void TxtAddress_EditingChanged(object sender, EventArgs e)
        {
            Editchange = true; 
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        private async void DeleteBranch(Branch branch, string text)
        {
            try
            {
                if (text != branch.BranchName)
                {
                    Utils.ShowMessage(Utils.TextBundle("wrongbranchname", "Wrong branch name"));
                    return;
                }
                var SysbranchID = branch.SysBranchID;
                if (SysbranchID == 1)
                {
                    Utils.ShowMessage(Utils.TextBundle("confirmdeletebranch", "Can't delete Head Office")); }
                else
                {
                    var DeleteonCloud = await GabanaAPI.DeleteDataBranch((int)SysbranchID);
                    if (DeleteonCloud.Status)
                    {
                        BranchManage branchManage = new BranchManage();
                        var branchdata = await branchManage.GetBranch(DataCashingAll.MerchantId, (int)SysbranchID);
                        branchdata.Status = 'D';
                        var DeleteonLocal = await branchManage.UpdateBranch(branchdata);
                        if (DeleteonLocal)
                        {
                            Utils.ShowMessage(Utils.TextBundle("successfullydeleted", "Successfully deleted"));
                            MyQrCodeManage myQrCodeManage = new MyQrCodeManage();
                            var deleteqr = await myQrCodeManage.DeleteMyQrCodefromBranch(DataCashingAll.MerchantId, (int)SysbranchID);
                            this.NavigationController.PopViewController(false);
                        }
                        else
                        {
                            Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                        }
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cantdeletebranch", "Can't delete branch") );
                return;
            }
        }
        async void UpdatetBranch()
        {
            try
            {
                string branchName = txtbranchName.Text;
                if (string.IsNullOrEmpty(branchName))
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("notcompletedata", "Please fill in all information.") );
                    btnAddBranch.Enabled = true;
                    return;
                }

                if ((txtPhone.Text.Replace("-", "").Length != 10 && txtPhone.Text.Replace("-", "").Length != 9) && !string.IsNullOrEmpty(txtPhone.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("telnotcomplete", "Please enter 10 digit phone number."));
                    btnAddBranch.Enabled = false;
                    return;
                }

                string TAXbranchName = txtbranchName.Text;
                string TAXbranchID = txtbranchID.Text;
                string phone = txtPhone.Text.Replace("-","");
                string address = txtAddress.Text;
                string comment = txtComment.Text;
                string linkPromaxx = txtLinkProMax.Text;

                Branch UpdatetBranch = new Branch();
                UpdatetBranch.MerchantID = DataCashingAll.MerchantId;
                UpdatetBranch.SysBranchID = this.BranchDetail.SysBranchID;
                UpdatetBranch.Ordinary = 0;
                UpdatetBranch.BranchName = branchName;
                UpdatetBranch.BranchID = this.BranchDetail.SysBranchID.ToString();
                UpdatetBranch.Address = address;

                UpdatetBranch.ProvincesId = idprovice;
                UpdatetBranch.AmphuresId = idamphure;
                UpdatetBranch.DistrictsId = iddistist;

                if (idprovice == 0) UpdatetBranch.ProvincesId = null;
                if (idamphure == 0) UpdatetBranch.AmphuresId = null;
                if (iddistist == 0) UpdatetBranch.DistrictsId = null;

                
                UpdatetBranch.DisplayLanguage = 'L';
                UpdatetBranch.Lat = null;
                UpdatetBranch.Lng = null;
                UpdatetBranch.Email = null;
                UpdatetBranch.Tel = phone;
                UpdatetBranch.Line = null;
                UpdatetBranch.Facebook = null;
                UpdatetBranch.Instagram = null;
                UpdatetBranch.TaxBranchName = TAXbranchName;
                UpdatetBranch.TaxBranchID = TAXbranchID;
                UpdatetBranch.LinkProMaxxID = linkPromaxx;
                UpdatetBranch.Comments = comment;
                UpdatetBranch.Status = 'A';
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>();
                });

                var Imapper = config.CreateMapper();
                var MasterBranch = Imapper.Map<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>(UpdatetBranch);

                //Branch
                var result = await GabanaAPI.PutDataBranch(MasterBranch);
                if (!result.Status)
                {
                    Utils.ShowMessage(result.Message);
                    btnAddBranch.Enabled = true;
                    return;
                }
                var branch = JsonConvert.DeserializeObject<Gabana.ORM.Master.Branch>(result.Message);

                //Mapping 
                var configlocal = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>();
                });

                var Imapperlocal = configlocal.CreateMapper();
                var Branchlocal = Imapperlocal.Map<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>(branch);

                var update = await BranchManager.UpdateBranch(Branchlocal);
                if (!update)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotsave", "Failed to save"));
                    btnAddBranch.Enabled = true;
                    return;
                }
                Utils.ShowMessage("บันทึกสาขาสำเร็จ");
                this.NavigationController.PopViewController(false);

                //Mapping 

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                btnAddBranch.Enabled = true;
                return;
            }
        }
        async void InsertBranch()
        {
            try
            {
                string branchName = txtbranchName.Text.Trim();
                if (string.IsNullOrEmpty(branchName))
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("notcompletedata", "Please fill in all information."));
                    btnAddBranch.Enabled = true;
                    return;
                }
                if ((txtPhone.Text.Replace("-", "").Length != 10 && txtPhone.Text.Replace("-", "").Length != 9) && !string.IsNullOrEmpty(txtPhone.Text))
                {
                    Utils.ShowMessage(Utils.TextBundle("telnotcomplete", "Please enter 10 digit phone number."));
                    btnAddBranch.Enabled = false;
                    return;
                }

                int sysBranchID = await BranchManager.GetLastBranch();
                int BranchId = sysBranchID + 1;

                string TAXbranchName = txtbranchName.Text;
                string TAXbranchID = txtbranchID.Text;
                string phone = txtPhone.Text.Replace("-", "");




                string address = txtAddress.Text;
                string comment = txtComment.Text;
                string linkPromaxx = txtLinkProMax.Text;

                Branch insertBranch = new Branch();
                insertBranch.MerchantID = DataCashingAll.MerchantId;
                insertBranch.SysBranchID = BranchId;
                insertBranch.Ordinary = 0;
                insertBranch.Status = 'A';
                insertBranch.BranchName = branchName;
                insertBranch.BranchID = BranchId.ToString();
                insertBranch.Address = address;

                insertBranch.ProvincesId = idprovice;
                insertBranch.AmphuresId = idamphure;
                insertBranch.DistrictsId = iddistist;

                if (idprovice == 0 ) insertBranch.ProvincesId = null;
                if (idamphure == 0) insertBranch.AmphuresId = null;
                if (iddistist == 0) insertBranch.DistrictsId = null;

                
                insertBranch.DisplayLanguage = 'L';
                insertBranch.Lat = null;
                insertBranch.Lng = null;
                insertBranch.Email = null;
                insertBranch.Tel = phone;
                insertBranch.Line = null;
                insertBranch.Facebook = null;
                insertBranch.Instagram = null;
                insertBranch.TaxBranchName = TAXbranchName;
                insertBranch.TaxBranchID = TAXbranchID;
                insertBranch.LinkProMaxxID = linkPromaxx;
                insertBranch.Comments = comment;

               

                //Mapping 
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>();
                });

                var Imapper = config.CreateMapper();
                var MasterBranch = Imapper.Map<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>(insertBranch);

                //Branch
                var result = await GabanaAPI.PostDataBranch(MasterBranch);
                if (!result.Status)
                {
                    Utils.ShowMessage(result.Message);
                    btnAddBranch.Enabled = true;
                    return;
                }
                var branch = JsonConvert.DeserializeObject<Gabana.ORM.Master.Branch>(result.Message);

                //Mapping 
                var configlocal = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>();
                });

                var Imapperlocal = configlocal.CreateMapper();
                var Branchlocal = Imapperlocal.Map<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>(branch);

                var insert = await BranchManager.InsertBranch(Branchlocal);
                if (!insert)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotsave", "Failed to save"));
                    btnAddBranch.Enabled = true;
                    return;
                }
                Utils.ShowMessage(Utils.TextBundle("savebranchsuccessfully", "Save branch successfully"));
                this.NavigationController.PopViewController(false);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                btnAddBranch.Enabled = true;
                return;
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

                //if (value.Length == 9 )
                //{
                //    var Phone = string.Empty;
                //    for (int i = 0; i < value.Length; i++)
                //    {
                //        if (i == 2 | i == 5)
                //        {
                //            Phone += "-";
                //        }
                //        Phone += value[i];
                //    }
                //    return Phone;
                //}

                if (value.Length == 9)
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
        void setDetailShow(bool set)
        {
            if (set)
            {
                LinkProView.Hidden = true;
                CommentView.Hidden = true;
                PostalCodeView.Hidden = true;
                SubdistrictView.Hidden = true;
                DistrictView.Hidden = true;
                ProvinceView.Hidden = true;
                //----------------------------
                AddressView.Hidden = true;
                PhoneView.Hidden = true;
            }
            else
            {
                LinkProView.Hidden = false;
                CommentView.Hidden = false;
                PostalCodeView.Hidden = false;
                SubdistrictView.Hidden = false;
                DistrictView.Hidden = false;
                ProvinceView.Hidden = false;
                //---------------------------------------
                PhoneView.Hidden = false;
                AddressView.Hidden = false;
            }
        }
        void SetupAutoLayout()
        {
            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddBranch.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddBranch.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //btnAddBranch.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddBranch.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            btnDeleteBranch.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDeleteBranch.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDeleteBranch.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDeleteBranch.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnDeleteBranch.RightAnchor.ConstraintEqualTo(btnAddBranch.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
            #endregion

            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region BranchIDView
            BranchIDView.TopAnchor.ConstraintEqualTo(BranchIDView.Superview.TopAnchor, 0).Active = true;
            BranchIDView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            BranchIDView.LeftAnchor.ConstraintEqualTo(BranchIDView.Superview.LeftAnchor, 0).Active = true;
            BranchIDView.RightAnchor.ConstraintEqualTo(BranchIDView.Superview.RightAnchor, 0).Active = true;

            lblbranchID.TopAnchor.ConstraintEqualTo(lblbranchID.Superview.TopAnchor, 11).Active = true;
            lblbranchID.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblbranchID.LeftAnchor.ConstraintEqualTo(lblbranchID.Superview.LeftAnchor, 20).Active = true;
            lblbranchID.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtbranchID.TopAnchor.ConstraintEqualTo(lblbranchID.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtbranchID.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtbranchID.LeftAnchor.ConstraintEqualTo(txtbranchID.Superview.LeftAnchor, 20).Active = true;
            txtbranchID.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region BranchNameView
            BranchNameView.TopAnchor.ConstraintEqualTo(BranchIDView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            BranchNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            BranchNameView.LeftAnchor.ConstraintEqualTo(BranchNameView.Superview.LeftAnchor, 0).Active = true;
            BranchNameView.RightAnchor.ConstraintEqualTo(BranchNameView.Superview.RightAnchor, 0).Active = true;

            lblbranchName.TopAnchor.ConstraintEqualTo(lblbranchName.Superview.TopAnchor, 11).Active = true;
            lblbranchName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblbranchName.LeftAnchor.ConstraintEqualTo(lblbranchName.Superview.LeftAnchor, 20).Active = true;
            lblbranchName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtbranchName.TopAnchor.ConstraintEqualTo(lblbranchName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtbranchName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtbranchName.LeftAnchor.ConstraintEqualTo(txtbranchName.Superview.LeftAnchor, 20).Active = true;
            txtbranchName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region DetailToggleView
            DetailToggleView.TopAnchor.ConstraintEqualTo(BranchNameView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
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
            
            #region PhoneView
            PhoneView.TopAnchor.ConstraintEqualTo(DetailToggleView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
            AddressView.TopAnchor.ConstraintEqualTo(PhoneView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
            ProvinceView.TopAnchor.ConstraintEqualTo(AddressView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
            DistrictView.TopAnchor.ConstraintEqualTo(ProvinceView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
            SubdistrictView.TopAnchor.ConstraintEqualTo(DistrictView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
            PostalCodeView.TopAnchor.ConstraintEqualTo(SubdistrictView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
            CommentView.TopAnchor.ConstraintEqualTo(PostalCodeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
            LinkProView.TopAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
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
    }
   
}