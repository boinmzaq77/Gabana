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
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class AddGiftVoucherController : UIViewController
    {
        public  GiftVoucher GiftVoucher;
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();

        UIView GiftCodeView, GiftNameView,GiftAmountView;
        UILabel lblGiftCode, lblGiftName, lblGiftAmount;
        UITextField txtGiftCode, txtGiftName, txtGiftAmount;
        string UserLogin;

        UIView BottomViewEdit;
        UIButton btnDelete, btnEdit;

        UIView bottmView;
        UIButton btnSave;
        string currencySelec = "";
        public static bool editnum;
        public static string num;
        private UIImageView IconImage;
        private bool Editchange = false;

        public AddGiftVoucherController() {

        }
        public AddGiftVoucherController(GiftVoucher gift)
        {
            GiftVoucher = gift;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            if (GiftVoucher==null)
            {
                Utils.SetTitle(this.NavigationController, "Add Gift Voucher");
            }
            else
            {
                Utils.SetTitle(this.NavigationController, "Edit Gift Voucher");
            }
            
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (editnum)
            {
                Editchange = true;
                editnum = false;
                txtGiftAmount.Text = num; 
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
        private async Task BtnSave_Click3()
        {
            if (GiftVoucher != null)
            {
                BtnEdit_Click();
            }
            else
            {
                BtnSave_Click();
            }
            

            
        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);

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

            base.ViewDidLoad();
            View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            // this.Title = "Decimal";

            try
            {
                UserLogin = Preferences.Get("User", "");
                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();
                if (GiftVoucher!= null)
                {
                    bottmView.Hidden = true;
                    BottomViewEdit.Hidden = false;

                    txtGiftCode.Text = GiftVoucher.GiftVoucherCode;
                    txtGiftName.Text = GiftVoucher.GiftVoucherName;
                    txtGiftAmount.Text = GiftVoucher.FmlAmount;


                    txtGiftCode.Enabled = false;
                    txtGiftAmount.Enabled = false;
                }
                else
                {
                    bottmView.Hidden = false;
                    BottomViewEdit.Hidden = true;

                    txtGiftCode.Text = "";
                    txtGiftName.Text = "";
                    txtGiftAmount.Text = "";
                }
                SetBtnSave();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        void SetBtnSave()
        {
            if (GiftVoucher != null)
            {
                if (txtGiftAmount.Text.Length > 0 | txtGiftCode.Text.Length > 0 | txtGiftName.Text.Length > 0)
                {
                    btnEdit.Enabled = true;
                    btnEdit.SetTitleColor(UIColor.White, UIControlState.Normal);
                    btnEdit.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    btnEdit.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                }
                else
                {
                    btnEdit.Enabled = false;
                    btnEdit.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    btnEdit.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                    btnEdit.BackgroundColor = UIColor.White;


                }
            }
            else
            {
                if (txtGiftAmount.Text.Length > 0 | txtGiftCode.Text.Length > 0 | txtGiftName.Text.Length > 0)
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
           
        }

        void initAttribute()
        {
            #region GiftCodeView
            GiftCodeView = new UIView();
            GiftCodeView.BackgroundColor = UIColor.White;
            GiftCodeView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(GiftCodeView);

            lblGiftCode = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblGiftCode.Font = lblGiftCode.Font.WithSize(15);
            lblGiftCode.Text = Utils.TextBundle("giftvouchercode", "Gift Voucher Code");
            GiftCodeView.AddSubview(lblGiftCode);

            txtGiftCode = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtGiftCode.ReturnKeyType = UIReturnKeyType.Next;
            txtGiftCode.ShouldReturn = (tf) =>
            {
                txtGiftName.BecomeFirstResponder();
                return true;
            };
            txtGiftCode.EditingChanged += (object sender, EventArgs e) =>
            {
                Editchange = true;
                SetBtnSave();
            };
            txtGiftCode.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("giftvouchercode", "Gift Voucher Code"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138,211,245) });
            txtGiftCode.Font = txtGiftCode.Font.WithSize(15);
            GiftCodeView.AddSubview(txtGiftCode);
            #endregion

            #region GiftNameView
            GiftNameView = new UIView();
            GiftNameView.BackgroundColor = UIColor.White;
            GiftNameView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(GiftNameView);

            lblGiftName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblGiftName.Font = lblGiftName.Font.WithSize(15);
            lblGiftName.Text = Utils.TextBundle("giftvouchername", "Gift Voucher Name");
            GiftNameView.AddSubview(lblGiftName);

            txtGiftName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtGiftName.ReturnKeyType = UIReturnKeyType.Next;
            txtGiftName.ShouldReturn = (tf) =>
            {
                txtGiftAmount.BecomeFirstResponder();
                return true;
            };
            txtGiftName.EditingChanged += (object sender, EventArgs e) =>
            {
                Editchange = true;
                SetBtnSave();
            };
            txtGiftName.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("giftvouchername", "Gift Voucher Name"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtGiftName.Font = txtGiftName.Font.WithSize(15);
            GiftNameView.AddSubview(txtGiftName);

            #endregion

            #region GiftAmountView
            GiftAmountView = new UIView();
            GiftAmountView.BackgroundColor = UIColor.White;
            GiftAmountView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(GiftAmountView);

            lblGiftAmount = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblGiftAmount.Font = lblGiftAmount.Font.WithSize(15);
            lblGiftAmount.Text = Utils.TextBundle("amount", "Amount");
            GiftAmountView.AddSubview(lblGiftAmount);

           

            GiftAmountView.UserInteractionEnabled = true;
            var tapGestureGift = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("GiftAmountView:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            GiftAmountView.AddGestureRecognizer(tapGestureGift);

            var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

            if (CURRENCYSYMBOLS != null)
            {
                currencySelec = CURRENCYSYMBOLS;
            }

            txtGiftAmount = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
                Enabled = false
            };
            txtGiftAmount.EditingChanged += (object sender, EventArgs e) =>
            {
                Editchange = true;
                SetBtnSave();
            };
            txtGiftAmount.AttributedPlaceholder = new NSAttributedString("0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtGiftAmount.Font = txtGiftAmount.Font.WithSize(15);
            GiftAmountView.AddSubview(txtGiftAmount);
            IconImage = new UIImageView();
            IconImage.TranslatesAutoresizingMaskIntoConstraints = false;
            IconImage.BackgroundColor = UIColor.White;
            IconImage.Image = UIImage.FromBundle("Next");
            GiftAmountView.AddSubview(IconImage);
            #endregion

            #region bottomView Save
            bottmView = new UIView();
            bottmView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottmView.BackgroundColor = UIColor.FromRGB(248,248,248);
            View.AddSubview(bottmView);

            btnSave = new UIButton();
            btnSave.SetTitle(Utils.TextBundle("addgiftvoucher", "Add Gift Voucher"), UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.Layer.BorderWidth = 0.5f;
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += async (sender, e) => {
                btnSave.Enabled = false;
                BtnSave_Click();
            };
            bottmView.AddSubview(btnSave);
            #endregion

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
                DeleteVoucher();
            };
            BottomViewEdit.AddSubview(btnDelete);

            btnEdit = new UIButton();
            btnEdit.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnEdit.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnEdit.Enabled = false;
            btnEdit.Layer.CornerRadius = 5f;
            btnEdit.BackgroundColor = UIColor.FromRGB(51, 172, 225);
            btnEdit.TranslatesAutoresizingMaskIntoConstraints = false;
            btnEdit.TouchUpInside += (sender, e) => {

                BtnEdit_Click();
            };
            BottomViewEdit.AddSubview(btnEdit);
            #endregion
        }
        [Export("GiftAmountView:")]
        public void GiftSetting(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("giftvoucher", "Add Gift Voucher"));

            var GiftPage = new AddGiftVoucherNumberController();
            this.NavigationController.PushViewController(GiftPage, false);
        }
        void CheckNull()
        {
            if (string.IsNullOrEmpty(txtGiftCode.Text) == true | string.IsNullOrEmpty(txtGiftName.Text) == true | string.IsNullOrEmpty(txtGiftName.Text) == true)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกข้อมูลให้ครบถ้วน");
                return;
            }
        }
        private async void DeleteVoucher()
        {
            //Create Alert
            var okCancelAlertController = UIAlertController.Create(Utils.TextBundle("wanttodelete", "Are you sure you want to delete?"), "คุณแน่ใจหรือไม่ที่จะลบ GiftVoucher Code : "+GiftVoucher.GiftVoucherCode+" ?", UIAlertControllerStyle.Alert );

            //Add Actions
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("delete", "Delete"), UIAlertActionStyle.Default, Action => delete_click()));
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("cancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel Delete")));

            //Present Alert
            PresentViewController(okCancelAlertController, true, null);
        }
        private async void delete_click()
        {
            if (await GabanaAPI.CheckNetWork())
            {
                var vouchercode = GiftVoucher.GiftVoucherCode;
                var result = await GabanaAPI.DeleteDataGiftVoucher(vouchercode);
                if (result.Status)
                {
                    GiftVoucherSettingController.isModifyGift = true;
                    GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
                    var deletelocal = await giftVoucherManage.DeleteGiftVoucher(DataCashingAll.MerchantId, vouchercode);
                    GiftVoucher = null;
                    Utils.ShowMessage(Utils.TextBundle("successfullydeleted", "Successfully deleted"));
                    this.NavigationController.PopViewController(false);
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("failedtodelete", "Failed to delete"));
                }
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "No Internet ไม่สามารถแก้ไขข้อมูลได้");
                return;
            }
        }
        private async void BtnSave_Click()
        {
            if (await GabanaAPI.CheckNetWork())
            {
                CheckNull();
                ORM.Master.GiftVoucher voucher = new ORM.Master.GiftVoucher()
                {
                    DateCreated = Utils.GetTranDate(DateTime.UtcNow),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = txtGiftAmount.Text,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)DataCashingAll.MerchantId,
                    Ordinary = null,
                    UserNameModified = UserLogin
                };

                var data = GiftVoucherSettingController.lstvouchers.Where(x => x.GiftVoucherCode == txtGiftCode.Text).FirstOrDefault();
                if (data != null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "Gift Voucher Code ซ้ำกรุณากรอกข้อมูลใหม่");
                    return;
                }

                if (data == null)
                {
                    var result = await GabanaAPI.PostDataGiftVoucher(voucher);
                    if (result.Status)
                    {
                        ORM.MerchantDB.GiftVoucher localvoucher = new ORM.MerchantDB.GiftVoucher()
                        {
                            DateCreated = Utils.GetTranDate(DateTime.UtcNow),
                            DateModified = Utils.GetTranDate(DateTime.UtcNow),
                            FmlAmount = txtGiftAmount.Text,
                            GiftVoucherCode = txtGiftCode.Text,
                            GiftVoucherName = txtGiftName.Text,
                            MerchantID = (int)DataCashingAll.MerchantId,
                            Ordinary = null,
                            UserNameModified = UserLogin
                        };
                        var insert = await giftVoucherManage.InsertGiftVoucher(localvoucher);
                        GiftVoucherSettingController.isModifyGift = true;
                        Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                        Editchange = false; 
                        this.NavigationController.PopViewController(false);
                    }

                    if (!result.Status)
                    {
                        Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถแก้ไขข้อมูลได้");
                        return;
                    }
                }
                btnSave.Enabled = true;
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "No Internet ไม่สามารถแก้ไขข้อมูลได้");
                btnSave.Enabled = true;
                return;
            }
        }
        private async void BtnEdit_Click()
        {
            if (await GabanaAPI.CheckNetWork())
            {
                CheckNull();
                int? myInt = GiftVoucher.Ordinary == null ? (int?)null : Convert.ToInt32(GiftVoucher.Ordinary);

                ORM.Master.GiftVoucher voucher = new ORM.Master.GiftVoucher()
                {
                    DateCreated = GiftVoucher.DateCreated,
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = txtGiftAmount.Text,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)GiftVoucher.MerchantID,
                    Ordinary = myInt,
                    UserNameModified = UserLogin
                };
                var result = await GabanaAPI.PutDataGiftVoucher(voucher);
                if (result.Status)
                {
                    ORM.MerchantDB.GiftVoucher localvoucher = new ORM.MerchantDB.GiftVoucher()
                    {
                        DateCreated = GiftVoucher.DateCreated,
                        DateModified = Utils.GetTranDate(DateTime.UtcNow),
                        FmlAmount = txtGiftAmount.Text,
                        GiftVoucherCode = txtGiftCode.Text,
                        GiftVoucherName = txtGiftName.Text,
                        MerchantID = (int)GiftVoucher.MerchantID,
                        Ordinary = myInt,
                        UserNameModified = UserLogin
                    };
                    var insert = await giftVoucherManage.UpdateGiftVoucher(localvoucher);
                    GiftVoucherSettingController.isModifyGift = true;
                    Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "Edit data successfully"));
                    Editchange = false;
                    this.NavigationController.PopViewController(false);
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถแก้ไขข้อมูลได้");
                    return;
                }
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "No Internet ไม่สามารถแก้ไขข้อมูลได้");
                return;
            }
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

            BottomViewEdit.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomViewEdit.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomViewEdit.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomViewEdit.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(45).Active = true;

            btnEdit.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnEdit.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnEdit.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnEdit.RightAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #region GiftCodeView
            GiftCodeView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            GiftCodeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            GiftCodeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            GiftCodeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblGiftCode.CenterYAnchor.ConstraintEqualTo(GiftCodeView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblGiftCode.WidthAnchor.ConstraintEqualTo(View.Frame.Height-50).Active = true;
            lblGiftCode.LeftAnchor.ConstraintEqualTo(lblGiftCode.Superview.LeftAnchor, 15).Active = true;
            lblGiftCode.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtGiftCode.TopAnchor.ConstraintEqualTo(lblGiftCode.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            txtGiftCode.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtGiftCode.LeftAnchor.ConstraintEqualTo(txtGiftCode.Superview.LeftAnchor, 15).Active = true;
            txtGiftCode.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region GiftNameView
            GiftNameView.TopAnchor.ConstraintEqualTo(GiftCodeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            GiftNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            GiftNameView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            GiftNameView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblGiftName.CenterYAnchor.ConstraintEqualTo(GiftNameView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblGiftName.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblGiftName.LeftAnchor.ConstraintEqualTo(lblGiftName.Superview.LeftAnchor, 15).Active = true;
            lblGiftName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtGiftName.TopAnchor.ConstraintEqualTo(lblGiftName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtGiftName.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtGiftName.LeftAnchor.ConstraintEqualTo(txtGiftName.Superview.LeftAnchor, 15).Active = true;
            txtGiftName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region GiftAmountView
            GiftAmountView.TopAnchor.ConstraintEqualTo(GiftNameView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            GiftAmountView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            GiftAmountView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            GiftAmountView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblGiftAmount.CenterYAnchor.ConstraintEqualTo(GiftAmountView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblGiftAmount.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblGiftAmount.LeftAnchor.ConstraintEqualTo(lblGiftAmount.Superview.LeftAnchor, 15).Active = true;
            lblGiftAmount.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtGiftAmount.TopAnchor.ConstraintEqualTo(lblGiftAmount.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtGiftAmount.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtGiftAmount.LeftAnchor.ConstraintEqualTo(txtGiftAmount.Superview.LeftAnchor, 15).Active = true;
            txtGiftAmount.HeightAnchor.ConstraintEqualTo(18).Active = true;

            IconImage.CenterYAnchor.ConstraintEqualTo(GiftAmountView.CenterYAnchor).Active = true;
            IconImage.WidthAnchor.ConstraintEqualTo(24).Active = true;
            IconImage.RightAnchor.ConstraintEqualTo(GiftAmountView.RightAnchor, -20).Active = true;
            IconImage.HeightAnchor.ConstraintEqualTo(24).Active = true;
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