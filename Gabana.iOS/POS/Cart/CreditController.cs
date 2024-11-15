using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.POS.Cart;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class CreditController : UIViewController
    {
        UIView orderView, commentView, CommentView, BottomView;
        UILabel lblorder, lblcomment, lblComment;
        UITextField txtorder, txtcomment, txtComment;
        UIButton btnSave;
        public static TranPayment tranPayment = new TranPayment();
        TransManage transManage = new TransManage();
        Model.TranWithDetailsLocal tranWithDetails;
        private UIView TypeView, CredittypeView, CreditNoViewView;
        private UILabel lblType, lblCredittype, lblCreditNo;
        private UITextField txtType, txtCredittype, txtCreditNo;
        private UIButton btnSelectType2;
        private UIButton btnSelectType;
        public string Phone = "";
        public CreditController() {
            this.tranWithDetails = POSController.tranWithDetails;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (POSController.SelectedCustomer != null)
            {
                UIImageView uIImageView = new UIImageView(new CGRect(0, 0, 50, 50));
                uIImageView.Image = UIImage.FromBundle("CustB");
                UIButton btn = new UIButton();
                //btn.SetImage(UIImage.FromBundle("Cust"), default);
                btn.ImageView.BackgroundColor = UIColor.Black;
                btn.Frame = new CGRect(0, 0, 200, 50);
                btn.Layer.CornerRadius = 5f;
                btn.Layer.BorderWidth = 0.5f;
                btn.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                //btn.BackgroundColor = UIColor.Red;
                UILabel lab = new UILabel();
                lab.TextColor = UIColor.FromRGB(0, 149, 218);
                lab.Text = POSController.SelectedCustomer.CustomerName;
                lab.TextAlignment = UITextAlignment.Right;
                lab.TranslatesAutoresizingMaskIntoConstraints = false;
                uIImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                btn.AddSubview(uIImageView);
                btn.AddSubview(lab);

                lab.RightAnchor.ConstraintEqualTo(btn.RightAnchor, -5).Active = true;
                lab.HeightAnchor.ConstraintEqualTo(50).Active = true;
                lab.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;
                uIImageView.RightAnchor.ConstraintEqualTo(lab.LeftAnchor).Active = true;
                uIImageView.LeftAnchor.ConstraintEqualTo(btn.LeftAnchor).Active = true;
                uIImageView.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;

                UIBarButtonItem selectCustomer = new UIBarButtonItem(btn);
                btn.TouchUpInside += (sender, e) => {
                    var selectCustomerPage = new POSCustomerController();
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
            else
            {
                UIBarButtonItem selectCustomer = new UIBarButtonItem();
                selectCustomer.Image = UIImage.FromBundle("Cust");
                selectCustomer.Clicked += (sender, e) => {
                    var selectCustomerPage = new POSCustomerController();
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
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
            var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
            #region TypeView
            TypeView = new UIView();
            TypeView.BackgroundColor = UIColor.White;
            TypeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblType = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblType.Font = lblType.Font.WithSize(15);
            lblType.Text = Utils.TextBundle("cardtype", "Items");
            TypeView.AddSubview(lblType);

            
            txtType = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtType.ReturnKeyType = UIReturnKeyType.Next;
            txtType.ShouldReturn = (tf) =>
            {
                txtcomment.BecomeFirstResponder();
                return true;
            };
            txtType.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtType.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("creditcard", "Items"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtType.Text = Utils.TextBundle("creditcard", "Items");
            txtType.Font = txtType.Font.WithSize(15);
            TypeView.AddSubview(txtType);

            btnSelectType = new UIButton();
            btnSelectType.SetBackgroundImage(UIImage.FromBundle("Next"), UIControlState.Normal);
            btnSelectType.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectType.TouchUpInside += (sender, e) => {
                txtType.BecomeFirstResponder();
            };
            TypeView.AddSubview(btnSelectType);

            PickerModel modelVat = new PickerModel(Type);
            txtType.Text = Type[0];
            modelVat.PickerChanged += async (sender, e) =>
            {
                txtType.Text = e.SelectedValue;
            };

            UIToolbar toolbar2 = new UIToolbar();
            toolbar2.BarStyle = UIBarStyle.Default;
            toolbar2.Translucent = true;
            toolbar2.SizeToFit();
            //VatAction
            var doneButton2 = new UIBarButtonItem(Utils.TextBundle("done", "Items"), UIBarButtonItemStyle.Done, this, new ObjCRuntime.Selector("VatAction"));
            toolbar2.SetItems(new UIBarButtonItem[] { flexible, doneButton2 }, true);
            txtType.InputView = new UIPickerView() { Model = modelVat, ShowSelectionIndicator = true };
            txtType.InputAccessoryView = toolbar2;


            #endregion

            #region CredittypeView
            CredittypeView = new UIView();
            CredittypeView.BackgroundColor = UIColor.White;
            CredittypeView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblCredittype = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCredittype.Font = lblCredittype.Font.WithSize(15);
            lblCredittype.TextAlignment = UITextAlignment.Left;
            lblCredittype.Text = Utils.TextBundle("creditcardtype", "Items");

            CredittypeView.AddSubview(lblCredittype);

           

            txtCredittype = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtCredittype.ReturnKeyType = UIReturnKeyType.Next;
            txtCredittype.ShouldReturn = (tf) =>
            {
                //txtComment.BecomeFirstResponder();
                return true;
            };
            
            txtCredittype.AttributedPlaceholder = new NSAttributedString("Visa", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtCredittype.Font = txtCredittype.Font.WithSize(15);

            CredittypeView.AddSubview(txtCredittype);

            btnSelectType2 = new UIButton();
            btnSelectType2.SetBackgroundImage(UIImage.FromBundle("Next"), UIControlState.Normal);
            btnSelectType2.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectType2.TouchUpInside += (sender, e) => {
                txtCredittype.BecomeFirstResponder();
            };
            CredittypeView.AddSubview(btnSelectType2);

            PickerModel modelVat2 = new PickerModel(Type2);
            txtCredittype.Text = Type2[0];
            modelVat2.PickerChanged += async (sender, e) =>
            {
                txtCredittype.Text = e.SelectedValue;
            };

            UIToolbar toolbar3 = new UIToolbar();
            toolbar3.BarStyle = UIBarStyle.Default;
            toolbar3.Translucent = true;
            toolbar3.SizeToFit();
            //VatAction
            var doneButton3 = new UIBarButtonItem(Utils.TextBundle("done", "Items"), UIBarButtonItemStyle.Done, this, new ObjCRuntime.Selector("VatAction2"));
            toolbar3.SetItems(new UIBarButtonItem[] { flexible, doneButton3 }, true);
            txtCredittype.InputView = new UIPickerView() { Model = modelVat2, ShowSelectionIndicator = true };
            txtCredittype.InputAccessoryView = toolbar2;

            #endregion

            #region CreditNoViewView
            CreditNoViewView = new UIView();
            CreditNoViewView.BackgroundColor = UIColor.White;
            CreditNoViewView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblCreditNo = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCreditNo.Font = lblCreditNo.Font.WithSize(15);
            lblCreditNo.TextAlignment = UITextAlignment.Left;
            lblCreditNo.Text = Utils.TextBundle("creditcardno", "Items");

            CreditNoViewView.AddSubview(lblCreditNo);

            txtCreditNo = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
                KeyboardType = UIKeyboardType.NumberPad
            };
            txtCreditNo.ReturnKeyType = UIReturnKeyType.Next;

            txtCreditNo.ShouldReturn = (tf) =>
            {
                //txtComment.BecomeFirstResponder();
                return true;
            };
            txtCreditNo.EditingChanged += TxtCredittype_EditingChanged;
            txtCreditNo.ShouldChangeCharacters = (textField, range, replacementString) => {
                var newLength = textField.Text.Length + replacementString.Length - range.Length;
                return newLength <= 19;
            };
            txtCreditNo.AttributedPlaceholder = new NSAttributedString("1234-4567-8910-1112", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtCreditNo.Font = txtCreditNo.Font.WithSize(15);

            CreditNoViewView.AddSubview(txtCreditNo);
            #endregion



            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;


            btnSave = new UIButton();
            btnSave.SetTitle(Utils.TextBundle("saveorder", "Items"), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += async (sender, e) => {
                try
                {
                    initialData();

                    string cardno = "";
                    if (!string.IsNullOrEmpty(txtCreditNo.Text))
                    {
                        cardno = txtCreditNo.Text.Replace("-", "");
                        if (cardno.Length < 16)
                        {
                            Utils.ShowMessage(Utils.TextBundle("entercredit", "Items"));
                            //Toast.MakeText(this, GetString(Resource.String.cardnonotcomplete), ToastLength.Short).Show();
                            return;
                        }
                    }
                   

                    decimal paymentAmount = 0;
                    foreach (var item in tranWithDetails.tranPayments)
                    {
                        paymentAmount += item.PaymentAmount;
                    }
                    decimal Cash = 0;
                    decimal amount = tranWithDetails.tran.GrandTotal - paymentAmount;

                   
                    tranPayment.PaymentAmount = amount; //เงินที่จ่าย
                    tranPayment.CardNo = cardno;
                    tranWithDetails.tranPayments.Add(tranPayment);

                    ChangeController ChangePage = new ChangeController();
                    Utils.SetTitle(this.NavigationController, Utils.TextBundle("change", "Items"));
                    ChangePage.Setitem(0, (double)Cash);
                    //this.NavigationController.ViewControllers)
                    this.NavigationController.PushViewController(ChangePage, false);


                }
                catch (Exception ex)
                {
                    Utils.ShowMessage(ex.Message);
                    _ = TinyInsights.TrackErrorAsync(ex);
                    //Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                    return;
                }
            };
            BottomView.AddSubview(btnSave);
            #endregion

            // UIView DeviceNoView, UDIDViewView, CommentView, BottomView;
            View.AddSubview(TypeView);
            View.AddSubview(CredittypeView);
            View.AddSubview(CreditNoViewView);
            //View.AddSubview(CommentView);
            View.AddSubview(BottomView);
            BottomView.BringSubviewToFront(btnSave);


        }

       
        private void TxtCredittype_EditingChanged(object sender, EventArgs e)
        {
            try
            {
                Phone = txtCreditNo.Text;
                int textlength = txtCreditNo.Text.Length;

                if (Phone.EndsWith(" "))
                    return;

                if (textlength == 5)
                {
                    var index = txtCreditNo.Text.LastIndexOf("-");
                    if (textlength == 5 & index == 4)
                    {
                        Phone.Remove(4, 1);
                    }
                    else
                    {
                        txtCreditNo.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                    }
                }
                else if (textlength == 10)
                {
                    var index = txtCreditNo.Text.LastIndexOf("-");
                    if (textlength == 10 & index == 9)
                    {
                        Phone.Remove(9, 1);
                    }
                    else
                    {
                        txtCreditNo.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                    }
                }
                else if (textlength == 15)
                {
                    var index = txtCreditNo.Text.LastIndexOf("-");
                    if (textlength == 15 & index == 14)
                    {
                        Phone.Remove(14, 1);
                    }
                    else
                    {
                        txtCreditNo.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        [Export("VatAction")]
        private void DoneAction5()
        {
            txtType.ResignFirstResponder();
        }
        [Export("VatAction2")]
        private void DoneAction6()
        {
            txtCredittype.ResignFirstResponder();
        }
        private void initialData()
        {
            if (tranWithDetails == null)
            {
                return;
            }
            var type1 ="Cr";
            if (txtType.Text == "Credit Card")
            {
                type1 = "Cr";
            }
            else
            {
                type1 = "Dr";
            }
            
            var no = tranWithDetails.tranPayments.Count + 1;
            tranPayment = new TranPayment()
            {
                MerchantID = DataCashingAll.MerchantId,
                SysBranchID = DataCashingAll.SysBranchId,
                TranNo = tranWithDetails.tran.TranNo,
                PaymentNo = no,
                PaymentType = type1,
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
            TypeView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            TypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            TypeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            TypeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblType.CenterYAnchor.ConstraintEqualTo(TypeView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblType.WidthAnchor.ConstraintEqualTo(View.Frame.Height-50).Active = true;
            lblType.LeftAnchor.ConstraintEqualTo(TypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtType.TopAnchor.ConstraintEqualTo(lblType.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            txtType.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtType.LeftAnchor.ConstraintEqualTo(TypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtType.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSelectType.CenterYAnchor.ConstraintEqualTo(TypeView.CenterYAnchor).Active = true;
            btnSelectType.RightAnchor.ConstraintEqualTo(TypeView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectType.HeightAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectType.WidthAnchor.ConstraintEqualTo(24).Active = true;
            #endregion

            #region UDIDViewView
            CredittypeView.TopAnchor.ConstraintEqualTo(TypeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CredittypeView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            CredittypeView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CredittypeView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblCredittype.CenterYAnchor.ConstraintEqualTo(CredittypeView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblCredittype.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblCredittype.LeftAnchor.ConstraintEqualTo(CredittypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblCredittype.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblcomment.BackgroundColor = UIColor.Red;

            txtCredittype.TopAnchor.ConstraintEqualTo(lblCredittype.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtCredittype.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtCredittype.LeftAnchor.ConstraintEqualTo(CredittypeView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtCredittype.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion
            btnSelectType2.CenterYAnchor.ConstraintEqualTo(CredittypeView.CenterYAnchor).Active = true;
            btnSelectType2.RightAnchor.ConstraintEqualTo(CredittypeView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectType2.HeightAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectType2.WidthAnchor.ConstraintEqualTo(24).Active = true;
            #region UDIDViewView
            CreditNoViewView.TopAnchor.ConstraintEqualTo(CredittypeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CreditNoViewView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            CreditNoViewView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CreditNoViewView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblCreditNo.CenterYAnchor.ConstraintEqualTo(CreditNoViewView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblCreditNo.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblCreditNo.LeftAnchor.ConstraintEqualTo(CreditNoViewView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblCreditNo.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblcomment.BackgroundColor = UIColor.Red;

            txtCreditNo.TopAnchor.ConstraintEqualTo(lblCreditNo.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtCreditNo.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            txtCreditNo.LeftAnchor.ConstraintEqualTo(CreditNoViewView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtCreditNo.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion


        }
        private readonly List<string> Type = new List<string>
        {
            "Credit Card",
            "Debit Card"
        };
        private readonly List<string> Type2 = new List<string>
        {
            "Visa",
            "Master Card"
        };
        public class PickerModel : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public string Vat { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private readonly List<string> values;
            public PickerModel(List<string> values)
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
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].ToString(),
                    });
                }
            }

            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }
   
}