using Foundation;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS
{
    public partial class ItemsAddDiscountController : UIViewController
    {
        UIAlertController DiscountTypeMenuSheet;
        UILabel lblDiscountName,lblDiscountType,lblAmount, lblDiscountTypeSelected;
        UITextField txtDiscountName, txtAmount;
        UIView DiscountNameView, DiscountTypeView, AmountView, BottomView1, BottomView2;
        UIButton btnAdd, btnSelectType,btnUpdate,btnDelete;

        DiscountTemplateManage discountTemplateManage = new DiscountTemplateManage();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        string sys;
        public Gabana.ORM.MerchantDB.DiscountTemplate discount;
        UIView line1, line2, line3;
        public ItemsAddDiscountController() { }
        public ItemsAddDiscountController(Gabana.ORM.MerchantDB.DiscountTemplate  Discount) {
            this.discount = Discount;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = new UIColor(51 / 255f, 172 / 255f, 225 / 255f, 1);
            this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        }
        public override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(51, 172, 225);
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.FromRGB(248,248,248);
            #region DiscountNameView
                DiscountNameView = new UIView();
                DiscountNameView.BackgroundColor = UIColor.White;
                DiscountNameView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(DiscountNameView);

                lblDiscountName = new UILabel
                {
                    TextColor = UIColor.FromRGB(64, 64, 64),
                    TranslatesAutoresizingMaskIntoConstraints = false  
                };
                lblDiscountName.Font = lblDiscountName.Font.WithSize(15);
                lblDiscountName.Text = "Discount Name";
                View.AddSubview(lblDiscountName);

                txtDiscountName = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(51, 172, 225),
                    TranslatesAutoresizingMaskIntoConstraints = false,  
                };
                txtDiscountName.ReturnKeyType = UIReturnKeyType.Next;
                txtDiscountName.ShouldReturn = (tf) =>
                {
                    lblDiscountTypeSelected.BecomeFirstResponder();
                    return true;
                };
                txtDiscountName.AttributedPlaceholder = new NSAttributedString("Discount", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
                txtDiscountName.Font = txtDiscountName.Font.WithSize(15);
                View.AddSubview(txtDiscountName);
            #endregion

            line1 = new UIView();
            line1.BackgroundColor = UIColor.FromRGB(226,226,226);
            line1.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(line1);

            #region DiscountTypeView
            DiscountTypeView = new UIView();
            DiscountTypeView.BackgroundColor = UIColor.White;
            DiscountTypeView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(DiscountTypeView);

            lblDiscountType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDiscountType.Font = lblDiscountType.Font.WithSize(15);
            lblDiscountType.Text = "Discount Type";
            View.AddSubview(lblDiscountType);

            lblDiscountTypeSelected = new UILabel
            {
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDiscountTypeSelected.Font = lblDiscountTypeSelected.Font.WithSize(15);
            lblDiscountTypeSelected.Text = "Discount Type";
            View.AddSubview(lblDiscountTypeSelected);

            btnSelectType = new UIButton();
            btnSelectType.SetBackgroundImage(UIImage.FromBundle("Next"),UIControlState.Normal);
            btnSelectType.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectType.TouchUpInside += (sender, e) => {
                // select type
                #region DiscountTypeActionSheet

                DiscountTypeMenuSheet = UIAlertController.Create("Discount Type", null, UIAlertControllerStyle.ActionSheet);
                DiscountTypeMenuSheet.AddAction(UIAlertAction.Create("Percent", UIAlertActionStyle.Default,
                                                Action => lblDiscountTypeSelected.Text = "Percent")) ;
                DiscountTypeMenuSheet.AddAction(UIAlertAction.Create("Value", UIAlertActionStyle.Default,
                                                Action => lblDiscountTypeSelected.Text ="Value"));
                DiscountTypeMenuSheet.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, Action => Console.WriteLine("Cancel clicked")));

                // Show the alert
                this.PresentViewController(DiscountTypeMenuSheet, true, null);
                #endregion
            };
            View.AddSubview(btnSelectType);
            #endregion

            line2 = new UIView();
            line2.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            line2.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(line2);

            #region AmountView
            AmountView = new UIView();
            AmountView.BackgroundColor = UIColor.White;
            AmountView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(AmountView);

            lblAmount = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblAmount.Font = lblAmount.Font.WithSize(15);
            lblAmount.Text = "Amount";
            View.AddSubview(lblAmount);

            txtAmount = new UITextField
            {
                Placeholder = "Amount",
                BackgroundColor = UIColor.White,
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtAmount.ReturnKeyType = UIReturnKeyType.Done;
            txtAmount.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtAmount.AttributedPlaceholder = new NSAttributedString("0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtAmount.Font = txtAmount.Font.WithSize(15);
            txtAmount.KeyboardType = UIKeyboardType.DecimalPad;
            View.AddSubview(txtAmount);
            #endregion

            line3 = new UIView();
            line3.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            line3.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(line3);

            #region BottomView1
            BottomView1 = new UIView();
            BottomView1.BackgroundColor = UIColor.White;
            BottomView1.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(BottomView1);

            btnAdd = new UIButton();
            btnAdd.SetTitle("Add Discount", UIControlState.Normal);
            btnAdd.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
            btnAdd.Layer.CornerRadius = 5f;
            btnAdd.Layer.BorderWidth = 0.5f;
            btnAdd.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnAdd.BackgroundColor = UIColor.White;
            btnAdd.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAdd.TouchUpInside += (sender, e) => {
                if (string.IsNullOrEmpty(txtDiscountName.Text))
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกชื่อต้นแบบส่วนลด");
                    txtDiscountName.BecomeFirstResponder();
                }

                if (string.IsNullOrEmpty(txtAmount.Text))
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกส่วนลด");
                    txtAmount.BecomeFirstResponder();
                }
                if (string.IsNullOrEmpty(txtDiscountName.Text) != true && string.IsNullOrEmpty(txtAmount.Text) != true
                        && (lblDiscountTypeSelected.Text =="Value" || lblDiscountTypeSelected.Text =="Percent"))
                {
                    CreateDiscount();
                    this.NavigationController.PopViewController(false);
                }
                else
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                }
            };
            BottomView1.AddSubview(btnAdd);
            #endregion

            #region BottomView2
            BottomView2 = new UIView();
            BottomView2.Hidden = true;
            BottomView2.BackgroundColor = UIColor.White;
            BottomView2.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomView2);

            btnDelete = new UIButton();
            btnDelete.Layer.CornerRadius = 5f;
            btnDelete.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnDelete.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDelete.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDelete.TouchUpInside += (sender, e) => {
                //delete
                DeleteDiscount();
            };
            BottomView2.AddSubview(btnDelete);

            btnUpdate = new UIButton();
            btnUpdate.SetTitle("Save", UIControlState.Normal);
            btnUpdate.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnUpdate.Layer.CornerRadius = 5f;
            btnUpdate.BackgroundColor = UIColor.FromRGB(51,170,225);
            btnUpdate.TranslatesAutoresizingMaskIntoConstraints = false;
            btnUpdate.TouchUpInside += (sender, e) => {
                if (string.IsNullOrEmpty(txtDiscountName.Text))
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกชื่อต้นแบบส่วนลด");
                    txtDiscountName.BecomeFirstResponder();
                }

                if (string.IsNullOrEmpty(txtAmount.Text))
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกส่วนลด");
                    txtAmount.BecomeFirstResponder();
                }
                if (string.IsNullOrEmpty(txtDiscountName.Text) != true && string.IsNullOrEmpty(txtAmount.Text) != true
                        && (lblDiscountTypeSelected.Text == "Value" || lblDiscountTypeSelected.Text == "Percent"))
                {
                    UpdateDiscount();
                    this.NavigationController.PopViewController(false);
                    Utils.ShowAlert(this, "สำเร็จ !", "แก้ไขข้อมูลสำเร็จ");
                }
                else
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                }
            };
            BottomView2.AddSubview(btnUpdate);
            #endregion

            Textboxfocus(View);
            //this.NavigationController.NavigationBar.BackItem.Title = " ";
            SetupAutoLayout();
            if (this.discount != null)
            {
                getDiscountDataAsync();
                BottomView1.Hidden = true;
                BottomView2.Hidden = false;
            }
        }
        private async void DeleteDiscount()
        {
            Gabana.ORM.MerchantDB.DiscountTemplate dis = new ORM.MerchantDB.DiscountTemplate();
            try
            {
                dis = await discountTemplateManage.GetDiscountTemplate((int)MainController.merchantlocal.MerchantID, (int)discount.SysDiscountTemplate);
                if(dis != null)
                {
                    var resultDelete = await discountTemplateManage.DeleteDiscountTemplate((int)MainController.merchantlocal.MerchantID, (int)discount.SysDiscountTemplate);
                    if (!resultDelete)
                    {
                        Utils.ShowAlert(this, "ไม่สามารถบันทึกข้อมูลได้!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                    }
                    else
                    {
                      //  Utils.ShowAlert(this, "สำเร็จ!", "ลบ Discount เรียบร้อย");
                        this.NavigationController.PopViewController(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private async void CreateDiscount()
        {
            Gabana.ORM.MerchantDB.DiscountTemplate dis = new ORM.MerchantDB.DiscountTemplate();
            try
            {
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo((int)MainController.merchantlocal.MerchantID, DataCashingAll.DeviceNo, 40);
                sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");
                CultureInfo.CurrentCulture = new CultureInfo("en-US");

                dis.MerchantID = MainController.merchantlocal.MerchantID;
                dis.SysDiscountTemplate = long.Parse(sys);
                dis.TemplateName = txtDiscountName.Text;
                if (lblDiscountTypeSelected.Text == "Value")
                {
                    dis.TemplateType = "FV";
                }
                else
                {
                    dis.TemplateType = "FP";
                }
                dis.FmlDiscount = txtAmount.Text;
                dis.DateCreated = DateTime.UtcNow;
                dis.DateModified = DateTime.UtcNow;
                dis.DataStatus = 'I';
                dis.FWaitSending = 1;
                dis.WaitSendingTime = DateTime.UtcNow;

                var resultInsert = await discountTemplateManage.InsertDiscountTemplate(dis);
                if (!resultInsert)
                {
                    Utils.ShowAlert(this, "ไม่สามารถบันทึกข้อมูลได้!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                }
             //   var result = discountTemplateManage.GetAllDiscountTemplate();

            }
            catch (Exception ex)
            {
                await Utils.ReloadInitialData();
                return;
            }
        }
        private async void UpdateDiscount()
        {
            Gabana.ORM.MerchantDB.DiscountTemplate dis = new ORM.MerchantDB.DiscountTemplate();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                dis = await discountTemplateManage.GetDiscountTemplate((int)MainController.merchantlocal.MerchantID,(int)discount.SysDiscountTemplate);

                dis.MerchantID = MainController.merchantlocal.MerchantID;
                dis.SysDiscountTemplate = dis.SysDiscountTemplate;
                dis.TemplateName = txtDiscountName.Text;
                if (lblDiscountTypeSelected.Text == "Value")
                {
                    dis.TemplateType = "FV";
                }
                else
                {
                    dis.TemplateType = "FP";
                }
                dis.FmlDiscount = txtAmount.Text;
                dis.DateCreated = dis.DateCreated;
                dis.DateModified = DateTime.UtcNow;
                dis.DataStatus = 'M';
                dis.FWaitSending = 1;
                dis.WaitSendingTime = DateTime.UtcNow;

                var resultUpdate = await discountTemplateManage.UpdateDiscountTemplate(dis);
                if (!resultUpdate)
                {
                    Utils.ShowAlert(this, "ไม่สามารถบันทึกข้อมูลได้!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                }
             //   var result = discountTemplateManage.GetAllDiscountTemplate();

            }
            catch (Exception ex)
            {
                return;
            }
        }
        async System.Threading.Tasks.Task getDiscountDataAsync()
        {
            txtDiscountName.Text = this.discount.TemplateName??"";
            txtAmount.Text = this.discount.FmlDiscount.ToString()??"";
            if(this.discount.TemplateType == "FV")
            {
                lblDiscountTypeSelected.Text = "Value";
            }
            else
            {
                lblDiscountTypeSelected.Text = "Percent";
            }
        }
        void SetupAutoLayout()
        {
            #region DiscountNameView
            DiscountNameView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            DiscountNameView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 9) / 100).Active = true;
            DiscountNameView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DiscountNameView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblDiscountName.CenterYAnchor.ConstraintEqualTo(DiscountNameView.SafeAreaLayoutGuide.CenterYAnchor, -11).Active = true;
            lblDiscountName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblDiscountName.LeftAnchor.ConstraintEqualTo(DiscountNameView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblDiscountName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtDiscountName.TopAnchor.ConstraintEqualTo(lblDiscountName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtDiscountName.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtDiscountName.LeftAnchor.ConstraintEqualTo(DiscountNameView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtDiscountName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            line1.TopAnchor.ConstraintEqualTo(DiscountNameView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line1.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line1.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line1.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region DiscountTypeView
            DiscountTypeView.TopAnchor.ConstraintEqualTo(line1.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            DiscountTypeView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 9) / 100).Active = true;
            DiscountTypeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DiscountTypeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblDiscountType.CenterYAnchor.ConstraintEqualTo(DiscountTypeView.SafeAreaLayoutGuide.CenterYAnchor, -11).Active = true;
            lblDiscountType.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblDiscountType.LeftAnchor.ConstraintEqualTo(DiscountTypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblDiscountType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lblDiscountTypeSelected.TopAnchor.ConstraintEqualTo(lblDiscountType.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblDiscountTypeSelected.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblDiscountTypeSelected.LeftAnchor.ConstraintEqualTo(DiscountTypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblDiscountTypeSelected.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSelectType.CenterYAnchor.ConstraintEqualTo(DiscountTypeView.SafeAreaLayoutGuide.CenterYAnchor, 11).Active = true;
            btnSelectType.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSelectType.RightAnchor.ConstraintEqualTo(DiscountTypeView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectType.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            line2.TopAnchor.ConstraintEqualTo(DiscountTypeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line2.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line2.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line2.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region AmountView
            AmountView.TopAnchor.ConstraintEqualTo(line2.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            AmountView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 9) / 100).Active = true;
            AmountView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AmountView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblAmount.CenterYAnchor.ConstraintEqualTo(AmountView.SafeAreaLayoutGuide.CenterYAnchor, -11).Active = true;
            lblAmount.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblAmount.LeftAnchor.ConstraintEqualTo(AmountView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblAmount.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtAmount.TopAnchor.ConstraintEqualTo(lblAmount.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtAmount.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtAmount.LeftAnchor.ConstraintEqualTo(AmountView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtAmount.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            line3.TopAnchor.ConstraintEqualTo(AmountView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line3.HeightAnchor.ConstraintEqualTo(0.2f).Active = true;
            line3.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line3.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region BottomViewLayout1
            BottomView1.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView1.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView1.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView1.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAdd.TopAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAdd.BottomAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAdd.LeftAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAdd.RightAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #region BottomLayout2
            BottomView2.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView2.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView2.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView2.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(45).Active = true;

            btnUpdate.TopAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnUpdate.BottomAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnUpdate.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnUpdate.RightAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
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