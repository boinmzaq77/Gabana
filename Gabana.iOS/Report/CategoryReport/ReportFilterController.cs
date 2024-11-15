using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{

    public partial class ReportFilterController : UIViewController
    {

    

        UIView bottomView;
        UIButton btnSelect;

        int filterReport = 1;

        UIView DescendingView, AscendingView, AZView, ZAView;
        UILabel lblDescending, lblAscending, lblAZ, lblZA;
        UIImageView DescendingImg, AscendingImg, AZImg, ZAImg;
        UIImageView DescendingSelectImg, AscendSelectImg, AZSelectImg, ZASelectImg;

        int filterByPage = 0;
        private int oldfilterReport;

        public ReportFilterController(int fill)
        {
            // 1 = category , 2 = customer , 3 = employee , 4= paymenttype , 5 = topsale
            this.filterByPage = fill;
        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //filterReport = CategoryReportDetailController.filterReport;
        }
        public async override void ViewDidLoad()
        {
            try
            {
                Utils.SetTitle(this.NavigationController, "Filter");
                View.BackgroundColor = UIColor.FromRGB(248,248,248);

                base.ViewDidLoad();

                intitAttribute();
                SetupAutoLayout();
                setSelect();
            }
            catch (Exception ex )
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        void setSelect()
        {
            if (filterByPage == 1)
            {
                filterReport = CategoryReportDetailController.filterReport;
            }
            else if (filterByPage == 2)
            {
                filterReport = CustomerReportDetailController.filterReport;

            }
            else if (filterByPage == 3)
            {
                filterReport = EmployeeReportDetailController.filterReport;
            }
            else if (filterByPage == 4)
            {
                filterReport = PaymentReportDetailController.filterReport;
            }
            else if (filterByPage == 5)
            {
                filterReport = TopSaleReportDetailController.filterReport;
            }
            else if (filterByPage == 6)
            {
                filterReport = ItemStockReportDetailController.filterReport;
            }
            oldfilterReport = filterReport;

            if (filterReport == 1)
            {
                DescendingSelectImg.Hidden = false;
                AscendSelectImg.Hidden = true;
                AZSelectImg.Hidden = true;
                ZASelectImg.Hidden = true;
            }
            else if (filterReport == 2)
            {
                DescendingSelectImg.Hidden = true;
                AscendSelectImg.Hidden = false;
                AZSelectImg.Hidden = true;
                ZASelectImg.Hidden = true;
            }
            else if (filterReport == 4)
            {
                DescendingSelectImg.Hidden = true;
                AscendSelectImg.Hidden = true;
                AZSelectImg.Hidden = false;
                ZASelectImg.Hidden = true;
            }
            else if (filterReport == 3)
            {
                DescendingSelectImg.Hidden = true;
                AscendSelectImg.Hidden = true;
                AZSelectImg.Hidden = true;
                ZASelectImg.Hidden = false;
            }

        }
        void intitAttribute()
        {
            #region DescendingView
            DescendingView = new UIView();
            DescendingView.TranslatesAutoresizingMaskIntoConstraints = false;
            DescendingView.BackgroundColor = UIColor.White;
            View.AddSubview(DescendingView);

            DescendingImg = new UIImageView();
            DescendingImg.Image = UIImage.FromFile("SortDESC.png");
            DescendingImg.TranslatesAutoresizingMaskIntoConstraints = false;
            DescendingView.AddSubview(DescendingImg);

            lblDescending = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDescending.Font = lblDescending.Font.WithSize(14);
            lblDescending.Text = Utils.TextBundle("descending", "Descending");
            DescendingView.AddSubview(lblDescending);

            DescendingSelectImg = new UIImageView();
            DescendingSelectImg.Image = UIImage.FromBundle("Check");
            DescendingSelectImg.Hidden = true;
            DescendingSelectImg.TranslatesAutoresizingMaskIntoConstraints = false;
            DescendingView.AddSubview(DescendingSelectImg);

            DescendingView.UserInteractionEnabled = true;
            var tapGestureDES = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("DES:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DescendingView.AddGestureRecognizer(tapGestureDES);
            #endregion

            #region AscendingView
            AscendingView = new UIView();
            AscendingView.TranslatesAutoresizingMaskIntoConstraints = false;
            AscendingView.BackgroundColor = UIColor.White;
            View.AddSubview(AscendingView);

            AscendingImg = new UIImageView();
            AscendingImg.Image = UIImage.FromFile("SortASC.png");
            AscendingImg.TranslatesAutoresizingMaskIntoConstraints = false;
            AscendingView.AddSubview(AscendingImg);

            lblAscending = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblAscending.Font = lblAscending.Font.WithSize(14);
            lblAscending.Text = Utils.TextBundle("ascending", "Ascending");
            AscendingView.AddSubview(lblAscending);

            AscendSelectImg = new UIImageView();
            AscendSelectImg.Image = UIImage.FromBundle("Check");
            AscendSelectImg.Hidden = true;
            AscendSelectImg.TranslatesAutoresizingMaskIntoConstraints = false;
            AscendingView.AddSubview(AscendSelectImg);

            AscendingView.UserInteractionEnabled = true;
            var tapGestureASC = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("ASC:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            AscendingView.AddGestureRecognizer(tapGestureASC);
            #endregion

            #region AZView
            AZView = new UIView();
            AZView.TranslatesAutoresizingMaskIntoConstraints = false;
            AZView.BackgroundColor = UIColor.White;
            View.AddSubview(AZView);

            AZImg = new UIImageView();
            AZImg.Image = UIImage.FromFile("SortAz.png");
            AZImg.TranslatesAutoresizingMaskIntoConstraints = false;
            AZView.AddSubview(AZImg);

            lblAZ = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblAZ.Font = lblAZ.Font.WithSize(14);
            lblAZ.Text = "A - Z";
            AZView.AddSubview(lblAZ);

            AZSelectImg = new UIImageView();
            AZSelectImg.Image = UIImage.FromBundle("Check");
            AZSelectImg.Hidden = true;
            AZSelectImg.TranslatesAutoresizingMaskIntoConstraints = false;
            AZView.AddSubview(AZSelectImg);

            AZView.UserInteractionEnabled = true;
            var tapGestureAZ = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("AZ:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            AZView.AddGestureRecognizer(tapGestureAZ);
            #endregion

            #region ZAView
            ZAView = new UIView();
            ZAView.TranslatesAutoresizingMaskIntoConstraints = false;
            ZAView.BackgroundColor = UIColor.White;
            View.AddSubview(ZAView);

            ZAImg = new UIImageView();
            ZAImg.Image = UIImage.FromFile("SortZa.png");
            ZAImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ZAView.AddSubview(ZAImg);

            lblZA = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblZA.Font = lblZA.Font.WithSize(14);
            lblZA.Text = "Z - A";
            ZAView.AddSubview(lblZA);

            ZASelectImg = new UIImageView();
            ZASelectImg.Image = UIImage.FromBundle("Check");
            ZASelectImg.Hidden = true;
            ZASelectImg.TranslatesAutoresizingMaskIntoConstraints = false;
            ZAView.AddSubview(ZASelectImg);

            ZAView.UserInteractionEnabled = true;
            var tapGestureZA = new UITapGestureRecognizer(this,
               new ObjCRuntime.Selector("ZA:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            ZAView.AddGestureRecognizer(tapGestureZA);
            #endregion


            #region BottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomView.BackgroundColor = UIColor.Clear;
            View.AddSubview(bottomView);

            btnSelect = new UIButton();
            btnSelect.SetTitleColor(UIColor.FromRGB(0,149,218), UIControlState.Normal);
            btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSelect.BackgroundColor = UIColor.White;
            btnSelect.Layer.CornerRadius = 5f;
            btnSelect.Layer.BorderWidth = 0.5f;
            btnSelect.Enabled = false;
            btnSelect.SetTitle(Utils.TextBundle("filter", "Filter"), UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
               // 1 = category , 2 = customer , 3 = employee , 4 = paymenttype , 5 = topsale
               if(filterByPage == 1)
                {
                    CategoryReportDetailController.filterReport = filterReport;
                    CategoryReportDetailController.isModifyFilter = true;
                }
               else if (filterByPage == 2)
                {
                    CustomerReportDetailController.filterReport = filterReport;
                    CustomerReportDetailController.isModifyFilter = true;
                }
                else if (filterByPage == 3)
                {
                    EmployeeReportDetailController.filterReport = filterReport;
                    EmployeeReportDetailController.isModifyFilter = true;
                }
                else if(filterByPage == 4)
                {
                    PaymentReportDetailController.filterReport = filterReport;
                    PaymentReportDetailController.isModifyFilter = true;
                }
                else if (filterByPage == 5)
                {
                    TopSaleReportDetailController.filterReport = filterReport;
                    TopSaleReportDetailController.isModifyFilter = true;
                }
                else if (filterByPage == 6)
                {
                    ItemStockReportDetailController.filterReport = filterReport;
                    ItemStockReportDetailController.isModifyFilter = true;
                }

                this.NavigationController.PopViewController(false);
            };
            View.AddSubview(btnSelect);
            #endregion
        }
        void ChangeButton()
        {
            if (oldfilterReport != filterReport)
            {
                btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnSelect.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                btnSelect.Enabled = true;
            }
            else
            {
                btnSelect.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnSelect.BackgroundColor = UIColor.White;
                btnSelect.Enabled = false;
            }
        }
        #region Go To Report
        [Export("DES:")]
        public void DES(UIGestureRecognizer sender)
        {
            DescendingSelectImg.Hidden = false;
            AscendSelectImg.Hidden = true;
            AZSelectImg.Hidden = true;
            ZASelectImg.Hidden = true;

            filterReport = 1;
            ChangeButton();
        }
        [Export("ASC:")]
        public void ASC(UIGestureRecognizer sender)
        {
            DescendingSelectImg.Hidden = true;
            AscendSelectImg.Hidden = false;
            AZSelectImg.Hidden = true;
            ZASelectImg.Hidden = true;

            filterReport = 2;
            ChangeButton();
        }
        [Export("AZ:")]
        public void AZ(UIGestureRecognizer sender)
        {
            DescendingSelectImg.Hidden = true;
            AscendSelectImg.Hidden = true;
            AZSelectImg.Hidden = false;
            ZASelectImg.Hidden = true;

            filterReport = 4;
            ChangeButton();
        }
        [Export("ZA:")]
        public void ZA(UIGestureRecognizer sender)
        {
            DescendingSelectImg.Hidden = true;
            AscendSelectImg.Hidden = true;
            AZSelectImg.Hidden = true;
            ZASelectImg.Hidden = false;

            filterReport = 3;
            ChangeButton();
        }
        #endregion
        void SetupAutoLayout()
        {
            #region DescendingView
            DescendingView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            DescendingView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            DescendingView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DescendingView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            DescendingImg.CenterYAnchor.ConstraintEqualTo(DescendingView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            DescendingImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            DescendingImg.LeftAnchor.ConstraintEqualTo(DescendingView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            DescendingImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblDescending.CenterYAnchor.ConstraintEqualTo(DescendingView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblDescending.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblDescending.LeftAnchor.ConstraintEqualTo(DescendingImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lblDescending.HeightAnchor.ConstraintEqualTo(16).Active = true;

            DescendingSelectImg.CenterYAnchor.ConstraintEqualTo(DescendingView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            DescendingSelectImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            DescendingSelectImg.RightAnchor.ConstraintEqualTo(DescendingView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            DescendingSelectImg.HeightAnchor.ConstraintEqualTo(20).Active = true;

            #endregion

            #region Ascending
            AscendingView.TopAnchor.ConstraintEqualTo(DescendingView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            AscendingView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            AscendingView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AscendingView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            AscendingImg.CenterYAnchor.ConstraintEqualTo(AscendingView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            AscendingImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            AscendingImg.LeftAnchor.ConstraintEqualTo(AscendingView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            AscendingImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblAscending.CenterYAnchor.ConstraintEqualTo(AscendingView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblAscending.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblAscending.LeftAnchor.ConstraintEqualTo(AscendingImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lblAscending.HeightAnchor.ConstraintEqualTo(16).Active = true;

            AscendSelectImg.CenterYAnchor.ConstraintEqualTo(AscendingView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            AscendSelectImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            AscendSelectImg.RightAnchor.ConstraintEqualTo(AscendingView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            AscendSelectImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region A - Z
            AZView.TopAnchor.ConstraintEqualTo(AscendingView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            AZView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            AZView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AZView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            AZImg.CenterYAnchor.ConstraintEqualTo(AZView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            AZImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            AZImg.LeftAnchor.ConstraintEqualTo(AZView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            AZImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblAZ.CenterYAnchor.ConstraintEqualTo(AZView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblAZ.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblAZ.LeftAnchor.ConstraintEqualTo(AZImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lblAZ.HeightAnchor.ConstraintEqualTo(16).Active = true;

            AZSelectImg.CenterYAnchor.ConstraintEqualTo(AZView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            AZSelectImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            AZSelectImg.RightAnchor.ConstraintEqualTo(AZView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            AZSelectImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region Z - A
            ZAView.TopAnchor.ConstraintEqualTo(AZView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            ZAView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ZAView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ZAView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            ZAImg.CenterYAnchor.ConstraintEqualTo(ZAView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            ZAImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            ZAImg.LeftAnchor.ConstraintEqualTo(ZAView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            ZAImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblZA.CenterYAnchor.ConstraintEqualTo(ZAView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            lblZA.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblZA.LeftAnchor.ConstraintEqualTo(ZAImg.SafeAreaLayoutGuide.RightAnchor, 20).Active = true;
            lblZA.HeightAnchor.ConstraintEqualTo(16).Active = true;

            ZASelectImg.CenterYAnchor.ConstraintEqualTo(ZAView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            ZASelectImg.WidthAnchor.ConstraintEqualTo(20).Active = true;
            ZASelectImg.RightAnchor.ConstraintEqualTo(ZAView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;
            ZASelectImg.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            #region bottomView
            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnSelect.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSelect.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnSelect.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSelect.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            #endregion
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}