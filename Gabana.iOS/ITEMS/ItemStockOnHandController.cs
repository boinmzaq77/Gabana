using Foundation;
using Gabana.iOS.ITEMS;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class ItemStockOnHandController : UIViewController
    {
        UIView dummyNumberView,bottomView,numpadView;
        string txtDummyStr="0";
        UILabel lblDummy , lblOnHandText ;
        UIButton btnAddDummy, btn5, btn10, btn50, btn100;
        int dotCount = 0;
        UIButton btnone, btntwo, btnthree, btnfour, btnfive, btnsix, btnseven, btneight, btnnine, btnzero, btndelete, btnClear;
        string Page="";
        string strValue = "0";
        int count;
        public ItemStockOnHandController(string page,string onhand)
        {
            this.Page = page;
            txtDummyStr = onhand;
        }
        public ItemStockOnHandController()
        {

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.SetNavigationBarHidden(false, false);
            txtDummyStr = "0";
            lblDummy.Text = "0";

        }
        public override void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region dummyNumberView
            dummyNumberView = new UIView();
            dummyNumberView.BackgroundColor = UIColor.FromRGB(248,248,248);
            dummyNumberView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(dummyNumberView);

            lblDummy = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(0,149,218),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            
            lblDummy.Font = lblDummy.Font.WithSize(60);
            dummyNumberView.AddSubview(lblDummy);

            lblOnHandText = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblOnHandText.Text = Utils.TextBundle("onhand", "onhand");
            lblOnHandText.Font = lblOnHandText.Font.WithSize(15);
            dummyNumberView.AddSubview(lblOnHandText);

            //btn5 = new UIButton();
            //btn5.Layer.CornerRadius = 5;
            //btn5.SetTitle("5", UIControlState.Normal);
            //btn5.BackgroundColor = UIColor.White;
            //btn5.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            //btn5.Layer.BorderWidth = 1;
            //btn5.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            //btn5.TranslatesAutoresizingMaskIntoConstraints = false;
            //btn5.TouchUpInside += (sender, e) => {

            //    var x = Convert.ToInt32(txtDummyStr) + 5;
            //    txtDummyStr = x.ToString();
            //    lblDummy.Text = txtDummyStr;

            //};
            //dummyNumberView.AddSubview(btn5);

            //btn10 = new UIButton();
            //btn10.Layer.CornerRadius = 5;
            //btn10.SetTitle("10", UIControlState.Normal);
            //btn10.BackgroundColor = UIColor.White;
            //btn10.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            //btn10.Layer.BorderWidth = 1;
            //btn10.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            //btn10.TranslatesAutoresizingMaskIntoConstraints = false;
            //btn10.TouchUpInside += (sender, e) => {
            //    var x = Convert.ToInt32(txtDummyStr) + 10;
            //    txtDummyStr = x.ToString();
            //    lblDummy.Text = txtDummyStr;


            //};
            //dummyNumberView.AddSubview(btn10);

            //btn50 = new UIButton();
            //btn50.Layer.CornerRadius = 5;
            //btn50.SetTitle("50", UIControlState.Normal);
            //btn50.BackgroundColor = UIColor.White;
            //btn50.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            //btn50.Layer.BorderWidth = 1;
            //btn50.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            //btn50.TranslatesAutoresizingMaskIntoConstraints = false;
            //btn50.TouchUpInside += (sender, e) => {
            //    var x = Convert.ToInt32(txtDummyStr) + 50;
            //    txtDummyStr = x.ToString();
            //    lblDummy.Text = txtDummyStr;

            //};
            //dummyNumberView.AddSubview(btn50);

            //btn100 = new UIButton();
            //btn100.Layer.CornerRadius = 5;
            //btn100.SetTitle("100", UIControlState.Normal);
            //btn100.BackgroundColor = UIColor.White;
            //btn100.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            //btn100.Layer.BorderWidth = 1;
            //btn100.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
            //btn100.TranslatesAutoresizingMaskIntoConstraints = false;
            //btn100.TouchUpInside += (sender, e) => {
            //    var x = Convert.ToInt32(txtDummyStr) + 100;
            //    txtDummyStr = x.ToString();
            //    lblDummy.Text = txtDummyStr;

            //};
            //dummyNumberView.AddSubview(btn100);

            #endregion

            #region numpadView
            numpadView = new UIView();
            numpadView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            numpadView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(numpadView);
            #endregion

            #region bottomView
            bottomView = new UIView();
            bottomView.BackgroundColor = UIColor.White;
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(bottomView);

            btnAddDummy = new UIButton();
            btnAddDummy.Layer.CornerRadius = 5;
            btnAddDummy.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnAddDummy.BackgroundColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1);
            btnAddDummy.SetTitleColor(UIColor.White,UIControlState.Normal);
            btnAddDummy.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddDummy.TouchUpInside += (sender, e) => {
                if(this.Page == "Extra")
                {
                    ItemsAddToppingController.isModifyOnhand = true;
                    ItemsAddToppingController.onHand = Convert.ToInt64(lblDummy.Text?.Replace(",",""));
                    this.NavigationController.PopViewController(false);
                }
                else
                {
                    AddItemControllerScroll.isModifyOnhand = true;
                    AddItemControllerScroll.onHand = Convert.ToInt64(lblDummy.Text?.Replace(",", ""));
                    this.NavigationController.PopViewController(false);
                }
               
            };
            View.AddSubview(btnAddDummy);
            #endregion
            Textboxfocus(View);
            SetupAutoLayout();
        }
        void SetupAutoLayout()
        {
            dummyNumberView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            dummyNumberView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            dummyNumberView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            dummyNumberView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*31)/100).Active = true;

            lblDummy.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblDummy.CenterYAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDummy.HeightAnchor.ConstraintEqualTo(84).Active = true;

            lblOnHandText.BottomAnchor.ConstraintEqualTo(lblDummy.SafeAreaLayoutGuide.TopAnchor , -17).Active = true;
            lblOnHandText.CenterXAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblOnHandText.HeightAnchor.ConstraintEqualTo(18).Active = true;

            //btn10.BottomAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //btn10.WidthAnchor.ConstraintEqualTo(82).Active = true;
            //btn10.RightAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, -5).Active = true;
            //btn10.HeightAnchor.ConstraintEqualTo(40).Active = true;

            //btn5.BottomAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //btn5.WidthAnchor.ConstraintEqualTo(82).Active = true;
            //btn5.RightAnchor.ConstraintEqualTo(btn10.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
            //btn5.HeightAnchor.ConstraintEqualTo(40).Active = true;

            //btn50.BottomAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //btn50.WidthAnchor.ConstraintEqualTo(82).Active = true;
            //btn50.LeftAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.CenterXAnchor, 5).Active = true;
            //btn50.HeightAnchor.ConstraintEqualTo(40).Active = true;

            //btn100.BottomAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //btn100.WidthAnchor.ConstraintEqualTo(82).Active = true;
            //btn100.LeftAnchor.ConstraintEqualTo(btn50.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            //btn100.HeightAnchor.ConstraintEqualTo(40).Active = true;



            numpadView.TopAnchor.ConstraintEqualTo(dummyNumberView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            numpadView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            numpadView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            numpadView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnAddDummy.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddDummy.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddDummy.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor,-10).Active = true;
            btnAddDummy.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor,-10).Active = true;

            NumberpadSetup();
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        void NumberpadSetup()
        {
            btnone = new UIButton();
            btnone.BackgroundColor = UIColor.White;
            btnone.TitleLabel.Font = btnone.TitleLabel.Font.WithSize(30);
            btnone.SetTitle("1", UIControlState.Normal);
            btnone.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnone.TranslatesAutoresizingMaskIntoConstraints = false;
            btnone.TouchUpInside += (sender, e) => {
                //1 press

                strValue = lblDummy.Text;
                SetValue("1");
            };
            numpadView.AddSubview(btnone);

            btntwo = new UIButton();
            btntwo.BackgroundColor = UIColor.White;
            btntwo.TitleLabel.Font = btntwo.TitleLabel.Font.WithSize(30);
            btntwo.SetTitle("2", UIControlState.Normal);
            btntwo.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btntwo.TranslatesAutoresizingMaskIntoConstraints = false;
            btntwo.TouchUpInside += (sender, e) => {
                //2 press

                strValue = lblDummy.Text;
                SetValue("2");

            };
            numpadView.AddSubview(btntwo);

            btnthree = new UIButton();
            btnthree.BackgroundColor = UIColor.White;
            btnthree.SetTitle("3", UIControlState.Normal);
            btnthree.TitleLabel.Font = btnthree.TitleLabel.Font.WithSize(30);
            btnthree.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnthree.TranslatesAutoresizingMaskIntoConstraints = false;
            btnthree.TouchUpInside += (sender, e) => {
                //3 press

                strValue = lblDummy.Text;
                SetValue("3");

            };
            numpadView.AddSubview(btnthree);

            btnfour = new UIButton();
            btnfour.BackgroundColor = UIColor.White;
            btnfour.SetTitle("4", UIControlState.Normal);
            btnfour.TitleLabel.Font = btnfour.TitleLabel.Font.WithSize(30);
            btnfour.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnfour.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfour.TouchUpInside += (sender, e) => {
                //4 press

                strValue = lblDummy.Text;
                SetValue("4");
            };
            numpadView.AddSubview(btnfour);

            btnfive = new UIButton();
            btnfive.BackgroundColor = UIColor.White;
            btnfive.SetTitle("5", UIControlState.Normal);
            btnfive.TitleLabel.Font = btnfive.TitleLabel.Font.WithSize(30);
            btnfive.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnfive.TranslatesAutoresizingMaskIntoConstraints = false;
            btnfive.TouchUpInside += (sender, e) => {
                //5 press

                strValue = lblDummy.Text;
                SetValue("5");
            };
            numpadView.AddSubview(btnfive);

            btnsix = new UIButton();
            btnsix.BackgroundColor = UIColor.White;
            btnsix.TitleLabel.Font = btnsix.TitleLabel.Font.WithSize(30);
            btnsix.SetTitle("6", UIControlState.Normal);
            btnsix.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnsix.TranslatesAutoresizingMaskIntoConstraints = false;
            btnsix.TouchUpInside += (sender, e) => {

                strValue = lblDummy.Text;
                SetValue("6");
            };
            numpadView.AddSubview(btnsix);

            btnseven = new UIButton();
            btnseven.BackgroundColor = UIColor.White;
            btnseven.SetTitle("7", UIControlState.Normal);
            btnseven.TitleLabel.Font = btnseven.TitleLabel.Font.WithSize(30);
            btnseven.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnseven.TranslatesAutoresizingMaskIntoConstraints = false;
            btnseven.TouchUpInside += (sender, e) => {
                //7 press

                strValue = lblDummy.Text;
                SetValue("7");
            };
            numpadView.AddSubview(btnseven);

            btneight = new UIButton();
            btneight.BackgroundColor = UIColor.White;
            btneight.TitleLabel.Font = btneight.TitleLabel.Font.WithSize(30);
            btneight.SetTitle("8", UIControlState.Normal);
            btneight.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btneight.TranslatesAutoresizingMaskIntoConstraints = false;
            btneight.TouchUpInside += (sender, e) => {
                //8 press

                strValue = lblDummy.Text;
                SetValue("8");
            };
            numpadView.AddSubview(btneight);

            btnnine = new UIButton();
            btnnine.BackgroundColor = UIColor.White;
            btnnine.TitleLabel.Font = btnnine.TitleLabel.Font.WithSize(30);
            btnnine.SetTitle("9", UIControlState.Normal);
            btnnine.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnnine.TranslatesAutoresizingMaskIntoConstraints = false;
            btnnine.TouchUpInside += (sender, e) => {
                //9 press

                strValue = lblDummy.Text;
                SetValue("9");
            };
            numpadView.AddSubview(btnnine);

            btnzero = new UIButton();
            btnzero.BackgroundColor = UIColor.White;
            btnzero.TitleLabel.Font = btnzero.TitleLabel.Font.WithSize(30);
            btnzero.SetTitle("0", UIControlState.Normal);
            btnzero.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnzero.TranslatesAutoresizingMaskIntoConstraints = false;
            btnzero.TouchUpInside += (sender, e) => {
                //0 press

                strValue = lblDummy.Text;
                SetValue("0");
            };
            numpadView.AddSubview(btnzero);

            btndelete = new UIButton();
            btndelete.SetImage(UIImage.FromBundle("Del"), UIControlState.Normal);
            btndelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            //btndelete.ImageEdgeInsets = new UIEdgeInsets(20, 20, 20, 20);
            btndelete.BackgroundColor = UIColor.White;
            
            btndelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btndelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btndelete.TouchUpInside += (sender, e) => {
                //0 press
                strValue = lblDummy.Text;
                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = Utils.CheckLenghtValue(strValue);
                    strValue = strValue.Remove(strValue.Length - 1);
                    lblDummy.Text = Convert.ToInt64(Utils.CheckLenghtValue(strValue)).ToString("#,##0");
                    return;
                }

                //กรณีกดลบจนเหลือตัวสุดท้าย ถ้าลบอีกครั้งให้เป็น 1 //ui ให้เปลี่ยนเป็นเหลือ 0
                if (strValue != string.Empty && strValue.Length == 1)
                {
                    strValue = "0";
                    lblDummy.Text = strValue;
                    return;
                }
            };
            numpadView.AddSubview(btndelete);

            btnClear = new UIButton();
            btnClear.BackgroundColor = UIColor.White;
            btnClear.SetTitle("C", UIControlState.Normal);
            btnClear.TitleLabel.Font = btnClear.TitleLabel.Font.WithSize(30);
            btnClear.SetTitleColor(new UIColor(red: 64 / 255f, green: 64 / 255f, blue: 64 / 255f, alpha: 1), UIControlState.Normal);
            btnClear.TranslatesAutoresizingMaskIntoConstraints = false;
            btnClear.TouchUpInside += (sender, e) => {
                //. press
                txtDummyStr = "0";
                lblDummy.Text = "0";
            };
            numpadView.AddSubview(btnClear);


            btnone.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnone.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnone.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*12)/100).Active = true;
            btnone.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btntwo.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btntwo.LeftAnchor.ConstraintEqualTo(btnone.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btntwo.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btntwo.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnthree.TopAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnthree.LeftAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnthree.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnthree.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnfour.TopAnchor.ConstraintEqualTo(btnone.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnfour.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnfour.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnfour.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnfive.TopAnchor.ConstraintEqualTo(btntwo.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnfive.LeftAnchor.ConstraintEqualTo(btnfour.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnfive.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnfive.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnsix.TopAnchor.ConstraintEqualTo(btnthree.BottomAnchor, 1).Active = true;
            btnsix.LeftAnchor.ConstraintEqualTo(btnfive.RightAnchor, 1).Active = true;
            btnsix.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnsix.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnseven.TopAnchor.ConstraintEqualTo(btnfour.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnseven.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnseven.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnseven.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btneight.TopAnchor.ConstraintEqualTo(btnfive.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btneight.LeftAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btneight.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btneight.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnnine.TopAnchor.ConstraintEqualTo(btnsix.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnnine.LeftAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnnine.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 12) / 100).Active = true;
            btnnine.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnClear.TopAnchor.ConstraintEqualTo(btnseven.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnClear.LeftAnchor.ConstraintEqualTo(numpadView.SafeAreaLayoutGuide.LeftAnchor, 1).Active = true;
            btnClear.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor,1).Active = true;
            btnClear.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btnzero.TopAnchor.ConstraintEqualTo(btneight.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btnzero.LeftAnchor.ConstraintEqualTo(btnClear.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btnzero.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btnzero.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;

            btndelete.TopAnchor.ConstraintEqualTo(btnnine.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            btndelete.LeftAnchor.ConstraintEqualTo(btnzero.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            btndelete.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 1).Active = true;
            btndelete.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 3).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
        public async void SetValue(string btn)
        {
            try
            {
                string amount = "0";
                
                if (!string.IsNullOrEmpty(lblDummy.Text))
                {
                    var datas = Utils.CheckLenghtValue(lblDummy.Text);
                    amount = (Convert.ToInt64(datas)).ToString("#,##0");
                }
                else
                {
                    amount = "0";
                }
                var num = btn.ToString();


                if (count == 0)
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            count++;
                            break;
                    }
                }
                else
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            break;
                    }
                }
                var data = Utils.CheckLenghtValue(amount);
                if (data.Length > 6)
                {
                    Utils.ShowMessage(Utils.TextBundle("maxstock", "จำนวนสินค้าสูงสุด 999999"));
                    //Toast.MakeText(this, GetString(Resource.String.maxitem) + " 999,999", ToastLength.Short).Show();
                    lblDummy.Text = "999,999";
                    strValue = lblDummy.Text;
                    return;
                }
                lblDummy.Text = (Convert.ToInt64(data)).ToString("#,##0");
                strValue = lblDummy.Text;

                //SetBtnCharge();
                //SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        //private void SetBtnSave()
        //{
        //    if (decimal.Parse(strValue) > 0)
        //    {
        //        btnAddDummy.SetTitle("Charge " + CURRENCYSYMBOLS + Utils.DisplayDecimal(decimal.Parse(strValue)), UIControlState.Normal);
        //        lblDummy.TextColor = UIColor.FromRGB(0, 149, 218);
        //    }
        //    else
        //    {
        //        btnAddDummy.SetTitle("Charge " + CURRENCYSYMBOLS + Utils.DisplayDecimal(pay), UIControlState.Normal);
        //        lblDummy.TextColor = UIColor.FromRGB(134, 206, 239);
        //    }

        //}
    }
}