using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class DecimalController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;
        UIView DecimalTypeView, DecimalCutView,DecimalShowView;
        UILabel lblDecimalType, lblDecimalDigiCut,lblDecimalShow;
        UIImageView btnDecimalType, btndecimalDigiCut,btnDecimalShow;
        UITextField txtDecimalType, txtDecimalDigiCut,txtDecimalShow;

        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string DECIMALPOINTDISPLAY;
        public static string OPTIONROUNDING_string, OPTIONROUNDING_int;
        string DECIMALPOINTCALC;


        public static bool isModifyDigi = false;
        public static int DigiCutTxtSelect = 0;

        UIView bottmView;
        UIButton btnSave;
        internal static string multiply;

        public DecimalController() {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            if(isModifyDigi)
            {
                txtDecimalDigiCut.Enabled = true;
                txtDecimalDigiCut.Text = DigiCutTxt[DigiCutTxtSelect];
                txtDecimalDigiCut.Enabled = false;
                OPTIONROUNDING_string = DigiCutTxtSelect.ToString();
                if (OPTIONROUNDING_string=="4")
                {
                    OPTIONROUNDING_int = multiply;
                }
                isModifyDigi = false;
            }
        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
           
            try
            {
                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();

                DECIMALPOINTCALC = DataCashingAll.setmerchantConfig.DECIMAL_POINT_CALC;//รูปแบบการคำนวณทศนิยม
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; // รูปแบบการแสดงทศนิยม
                OPTIONROUNDING_string = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING; //การปัดเศษทศนิยม
                OPTIONROUNDING_int = DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT; //การปัดเศษทศนิยม
                DigiCutTxtSelect = Int32.Parse( DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING);


                SetupPicker();
                SetBtnSave();




            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        private void SetBtnSave()
        {

            if (DECIMALPOINTDISPLAY != DECIMALPOINTDISPLAY || OPTIONROUNDING_int != DECIMALPOINTCALC)
            {
                btnSave.Enabled = true;
                btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSave.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                btnSave.Enabled = false;
                btnSave.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSave.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnSave.BackgroundColor = UIColor.White;


            }
        }
        private void BtnSave_Click()
        {
            if (DataCashingAll.CheckConnectInternet)
            {
                UpdateMerchantConfig();
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("nointernet", "No Internet,Please Connect"));
            }
        }
        private async void UpdateMerchantConfig()
        {
            try
            {
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();

                //DecimalCal 2,4
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "DECIMAL_POINT_DISPLAY",
                    CfgInteger = Convert.ToInt32(DECIMALPOINTDISPLAY),
                };
                lstmerchantConfig.Add(merchantConfig);

                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "DECIMAL_POINT_CALC",
                    CfgInteger = Convert.ToInt32(DECIMALPOINTCALC),
                };
                lstmerchantConfig.Add(merchantConfig);


                //DecimalType 1,2,3,4,5 การปัดเศษ
                if (OPTIONROUNDING_string == "4")
                {
                    merchantConfig = new ORM.Master.MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "OPTION_ROUNDING",
                        CfgInteger = Convert.ToInt32(OPTIONROUNDING_int),
                        CfgString = OPTIONROUNDING_string
                    };
                    lstmerchantConfig.Add(merchantConfig);
                }
                else
                {
                    merchantConfig = new ORM.Master.MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "OPTION_ROUNDING",
                        CfgString = OPTIONROUNDING_string
                    };
                    lstmerchantConfig.Add(merchantConfig);
                }

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (update.Status)
                {
                    //Insert to Local DB
                    foreach (var item in lstmerchantConfig)
                    {
                        MerchantConfig localConfig = new MerchantConfig()
                        {
                            MerchantID = item.MerchantID,
                            CfgKey = item.CfgKey,
                            CfgInteger = item.CfgInteger,
                            CfgString = item.CfgString
                        };
                        var local = await configManage.InsertorReplacrMerchantConfig(localConfig);
                    }

                    DataCashingAll.setmerchantConfig.DECIMAL_POINT_CALC = DECIMALPOINTCALC;
                    DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY = DECIMALPOINTDISPLAY;
                    DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING = OPTIONROUNDING_string;
                    DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT = OPTIONROUNDING_int;
                    Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                    this.NavigationController.PopViewController(false);
                }
                ItemsController.Ismodify = true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248,248,248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region DecimalTypeView
            DecimalTypeView = new UIView();
            DecimalTypeView.BackgroundColor = UIColor.White;
            DecimalTypeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblDecimalType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDecimalType.Font = lblDecimalType.Font.WithSize(15);
            lblDecimalType.Text = Utils.TextBundle("decimalcalformat", "Decimal Calculation format");
            DecimalTypeView.AddSubview(lblDecimalType);

            txtDecimalType = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtDecimalType.ReturnKeyType = UIReturnKeyType.Next;
            txtDecimalType.ShouldReturn = (tf) =>
            {
                txtDecimalDigiCut.BecomeFirstResponder();
                return true;
            };
            txtDecimalType.EditingChanged += (object sender, EventArgs e) =>
            {
                SetBtnSave();
            };
            txtDecimalType.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("type", "Type"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtDecimalType.Font = txtDecimalType.Font.WithSize(15);
            DecimalTypeView.AddSubview(txtDecimalType);

            btnDecimalType = new UIImageView();
            btnDecimalType.Image = UIImage.FromBundle("Next");
            btnDecimalType.TranslatesAutoresizingMaskIntoConstraints = false;
            DecimalTypeView.AddSubview(btnDecimalType);

            btnDecimalType.UserInteractionEnabled = true;
            var tapGestureLinkType = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("SelectType:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnDecimalType.AddGestureRecognizer(tapGestureLinkType);
            #endregion

            #region DecimalShowView
            DecimalShowView = new UIView();
            DecimalShowView.BackgroundColor = UIColor.White;
            DecimalShowView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblDecimalShow = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDecimalShow.Font = lblDecimalShow.Font.WithSize(15);
            lblDecimalShow.Text = Utils.TextBundle("decimaldisformat", "Decimal Display format");
            DecimalShowView.AddSubview(lblDecimalShow);

            txtDecimalShow = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtDecimalShow.ReturnKeyType = UIReturnKeyType.Next;
            txtDecimalShow.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtDecimalShow.EditingChanged += (object sender, EventArgs e) =>
            {
                SetBtnSave();
            };
            txtDecimalShow.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("type", "Type"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtDecimalShow.Font = txtDecimalShow.Font.WithSize(15);
            DecimalShowView.AddSubview(txtDecimalShow);

            btnDecimalShow = new UIImageView();
            btnDecimalShow.Image = UIImage.FromBundle("Next");
            btnDecimalShow.TranslatesAutoresizingMaskIntoConstraints = false;
            DecimalShowView.AddSubview(btnDecimalShow);

            btnDecimalShow.UserInteractionEnabled = true;
            var tapGestureShow = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("SelectShow:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnDecimalShow.AddGestureRecognizer(tapGestureShow);
            #endregion

            #region PaperTypeView
            DecimalCutView = new UIView();
            DecimalCutView.BackgroundColor = UIColor.White;
            DecimalCutView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblDecimalDigiCut = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDecimalDigiCut.Font = lblDecimalDigiCut.Font.WithSize(15);
            lblDecimalDigiCut.Text = Utils.TextBundle("decimalrounding", "Rounding a decimal number");
            DecimalCutView.AddSubview(lblDecimalDigiCut);

            txtDecimalDigiCut = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtDecimalDigiCut.Enabled = false;
            txtDecimalDigiCut.EditingChanged += (object sender, EventArgs e) =>
            {
                SetBtnSave();
            };
            txtDecimalDigiCut.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("notrounded", "Not Rounded"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtDecimalDigiCut.Font = txtDecimalDigiCut.Font.WithSize(15);
            DecimalCutView.AddSubview(txtDecimalDigiCut);

            btndecimalDigiCut = new UIImageView();
            btndecimalDigiCut.Image = UIImage.FromBundle("Next");
            btndecimalDigiCut.TranslatesAutoresizingMaskIntoConstraints = false;
            DecimalCutView.AddSubview(btndecimalDigiCut);

            btndecimalDigiCut.UserInteractionEnabled = true;
            var tapGesturePaperType = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("SelectDigi:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DecimalCutView.AddGestureRecognizer(tapGesturePaperType);
            #endregion

            _contentView.AddSubview(DecimalTypeView);
            _contentView.AddSubview(DecimalShowView);
            _contentView.AddSubview(DecimalCutView);
            _scrollView.AddSubview(_contentView);

            #region bottomView
            bottmView = new UIView();
            bottmView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottmView.BackgroundColor = UIColor.FromRGB(248,248,248);
            View.AddSubview(bottmView);

            btnSave = new UIButton();
            btnSave.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.Layer.BorderWidth = 0.5f;
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += async (sender, e) => {
                BtnSave_Click();
            };
            bottmView.AddSubview(btnSave);
            #endregion

            View.AddSubview(_scrollView);
        }
        #region SelectType
        [Export("SelectDigi:")]
        public void SelectDigi(UIGestureRecognizer sender)
        {
          //  txtDecimalDigiCut.BecomeFirstResponder();
            DecimalDetailController Detail = new DecimalDetailController(DigiCutTxtSelect);
            this.NavigationController.PushViewController(Detail, false);

        }
        [Export("SelectShow:")]
        public void SelectShow(UIGestureRecognizer sender)
        {
            txtDecimalShow.BecomeFirstResponder();
        }
        [Export("SelectType:")]
        public void SelectType(UIGestureRecognizer sender)
        {
             txtDecimalType.BecomeFirstResponder();
         //   DigiCutTxt
            
        }
        #endregion

        private readonly List<string> DecimalTypeList = new List<string>
        {
        Utils.TextBundle("2digits", "2 Decimal digits"),
        Utils.TextBundle("4digits", "4 Decimal digits")
        };
        private readonly List<string> DecimaShowList = new List<string>
        {
        Utils.TextBundle("2digits", "2 Decimal digits"),
        Utils.TextBundle("4digits", "4 Decimal digits")
        };
        private readonly List<string> DigiCutTxt = new List<string>
        {
        Utils.TextBundle("notrounded", "Not Rounded"),
        Utils.TextBundle("option1", "Option 1"),
        Utils.TextBundle("option2", "Option 2"),
        Utils.TextBundle("option3", "Option 3"),
        Utils.TextBundle("option4", "Option 4"),
        Utils.TextBundle("option5", "Option 5")
        };
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
        private void SetupPicker()
        {
            #region DecinalType
            PickerModel modelDecinalType = new PickerModel(DecimalTypeList);
            int selectedDecimal = Convert.ToInt32(DECIMALPOINTCALC);
           
            modelDecinalType.PickerChanged += (sender, e) => {
                txtDecimalType.Text = e.SelectedValue;
                if (e.SelectedValue == Utils.TextBundle("2digits", "2 Decimal digits"))
                {
                    DECIMALPOINTCALC = "2";
                }
                else
                {
                    DECIMALPOINTCALC = "4";
                }
            };

            UIToolbar toolbar = new UIToolbar();
            toolbar.Translucent = true;
            toolbar.SizeToFit();
            var flexible1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
            var doneButton1 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            });
            toolbar.SetItems(new UIBarButtonItem[] { flexible1, doneButton1 }, true);
            var x =  new UIPickerView() { Model = modelDecinalType, ShowSelectionIndicator = true };
            txtDecimalType.InputView = x;
            if (DecimalTypeList != null)
            {
                if (selectedDecimal == 2)
                {
                    txtDecimalType.Text = DecimalTypeList[0];
                    x.Select(0, 0, false);
                }
                else
                {
                    txtDecimalType.Text = DecimalTypeList[1];
                    x.Select(1, 0, false);
                }
            }
            txtDecimalType.InputAccessoryView = toolbar;

            #endregion

            #region DecinalShow // cal
            PickerModel modelDecinaShow = new PickerModel(DecimaShowList);
            int selectedShow = Convert.ToInt32(DECIMALPOINTDISPLAY);

            modelDecinaShow.PickerChanged += (sender, e) => {
                txtDecimalShow.Text = e.SelectedValue;
                if (e.SelectedValue == Utils.TextBundle("2digits", "2 Decimal digits"))
                {
                    DECIMALPOINTDISPLAY = "2";
                }
                else
                {
                    DECIMALPOINTDISPLAY = "4";
                }
            };
            UIToolbar toolbar2 = new UIToolbar();
            toolbar2.Translucent = true;
            toolbar2.SizeToFit();
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                View.EndEditing(true);
            });
            var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
            toolbar2.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
            var y = new UIPickerView() { Model = modelDecinaShow, ShowSelectionIndicator = true };
            txtDecimalShow.InputView = y;
            if (DecimaShowList != null)
            {
                if (selectedShow == 2)
                {
                    txtDecimalShow.Text = DecimaShowList[0];
                    y.Select(0, 0, false);
                }
                else
                {
                    txtDecimalShow.Text = DecimaShowList[1];
                    y.Select(1, 0, false);
                }
            }
            txtDecimalShow.InputAccessoryView = toolbar2;

            #endregion

            SetDecimalDisplay();
        }
        private void SetDecimalDisplay()
        {
            txtDecimalDigiCut.Enabled = true;
            switch (OPTIONROUNDING_string)
            {
                case "0":
                    txtDecimalDigiCut.Text = Utils.TextBundle("notrounded", "Not Rounded");
                    break;
                case "1":
                    txtDecimalDigiCut.Text = Utils.TextBundle("option1", "Option 1");
                    break;
                case "2":
                    txtDecimalDigiCut.Text = Utils.TextBundle("option2", "Option 2");
                    break;
                case "3":
                    txtDecimalDigiCut.Text = Utils.TextBundle("option3", "Option 3");
                    break;
                case "4":
                    txtDecimalDigiCut.Text = Utils.TextBundle("option4", "Option 4");
                    break;
                case "5":
                    txtDecimalDigiCut.Text = Utils.TextBundle("option5", "Option 5");
                    break;
            }
            txtDecimalDigiCut.Enabled = false;
        }
        void SetupAutoLayout()
        {
            #region bottomView
            bottmView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottmView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottmView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottmView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnSave.BottomAnchor.ConstraintEqualTo(bottmView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(bottmView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(bottmView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSave.TopAnchor.ConstraintEqualTo(bottmView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            #endregion

            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(bottmView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region LinkTypeView
            DecimalTypeView.TopAnchor.ConstraintEqualTo(DecimalTypeView.Superview.TopAnchor, 1).Active = true;
            DecimalTypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            DecimalTypeView.LeftAnchor.ConstraintEqualTo(DecimalTypeView.Superview.LeftAnchor, 0).Active = true;
            DecimalTypeView.RightAnchor.ConstraintEqualTo(DecimalTypeView.Superview.RightAnchor, 0).Active = true;

            lblDecimalType.CenterYAnchor.ConstraintEqualTo(lblDecimalType.Superview.CenterYAnchor, -12).Active = true;
            lblDecimalType.WidthAnchor.ConstraintEqualTo(View.Frame.Height-50).Active = true;
            lblDecimalType.LeftAnchor.ConstraintEqualTo(lblDecimalType.Superview.LeftAnchor, 15).Active = true;
            lblDecimalType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtDecimalType.TopAnchor.ConstraintEqualTo(lblDecimalType.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            txtDecimalType.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtDecimalType.LeftAnchor.ConstraintEqualTo(txtDecimalType.Superview.LeftAnchor, 15).Active = true;
            txtDecimalType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnDecimalType.CenterYAnchor.ConstraintEqualTo(btnDecimalType.Superview.CenterYAnchor, 0).Active = true;
            btnDecimalType.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnDecimalType.RightAnchor.ConstraintEqualTo(btnDecimalType.Superview.RightAnchor, -15).Active = true;
            btnDecimalType.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region DecimalShowView
            DecimalShowView.TopAnchor.ConstraintEqualTo(DecimalTypeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            DecimalShowView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            DecimalShowView.LeftAnchor.ConstraintEqualTo(DecimalShowView.Superview.LeftAnchor, 0).Active = true;
            DecimalShowView.RightAnchor.ConstraintEqualTo(DecimalShowView.Superview.RightAnchor, 0).Active = true;

            lblDecimalShow.CenterYAnchor.ConstraintEqualTo(lblDecimalShow.Superview.CenterYAnchor, -12).Active = true;
            lblDecimalShow.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblDecimalShow.LeftAnchor.ConstraintEqualTo(lblDecimalShow.Superview.LeftAnchor, 15).Active = true;
            lblDecimalShow.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtDecimalShow.TopAnchor.ConstraintEqualTo(lblDecimalShow.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtDecimalShow.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtDecimalShow.LeftAnchor.ConstraintEqualTo(txtDecimalShow.Superview.LeftAnchor, 15).Active = true;
            txtDecimalShow.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnDecimalShow.CenterYAnchor.ConstraintEqualTo(btnDecimalShow.Superview.CenterYAnchor, 0).Active = true;
            btnDecimalShow.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnDecimalShow.RightAnchor.ConstraintEqualTo(btnDecimalShow.Superview.RightAnchor, -15).Active = true;
            btnDecimalShow.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region DecimalCutView
            DecimalCutView.TopAnchor.ConstraintEqualTo(DecimalShowView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            DecimalCutView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            DecimalCutView.BottomAnchor.ConstraintEqualTo(DecimalCutView.Superview.BottomAnchor,0).Active=true;
            DecimalCutView.LeftAnchor.ConstraintEqualTo(DecimalCutView.Superview.LeftAnchor, 0).Active = true;
            DecimalCutView.RightAnchor.ConstraintEqualTo(DecimalCutView.Superview.RightAnchor, 0).Active = true;

            lblDecimalDigiCut.CenterYAnchor.ConstraintEqualTo(lblDecimalDigiCut.Superview.CenterYAnchor, -12).Active = true;
            lblDecimalDigiCut.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblDecimalDigiCut.LeftAnchor.ConstraintEqualTo(lblDecimalDigiCut.Superview.LeftAnchor, 15).Active = true;
            lblDecimalDigiCut.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtDecimalDigiCut.TopAnchor.ConstraintEqualTo(lblDecimalDigiCut.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtDecimalDigiCut.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtDecimalDigiCut.LeftAnchor.ConstraintEqualTo(txtDecimalDigiCut.Superview.LeftAnchor, 15).Active = true;
            txtDecimalDigiCut.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btndecimalDigiCut.CenterYAnchor.ConstraintEqualTo(btndecimalDigiCut.Superview.CenterYAnchor, 0).Active = true;
            btndecimalDigiCut.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btndecimalDigiCut.RightAnchor.ConstraintEqualTo(btndecimalDigiCut.Superview.RightAnchor, -15).Active = true;
            btndecimalDigiCut.HeightAnchor.ConstraintEqualTo(28).Active = true;
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