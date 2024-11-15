﻿using AutoMapper;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class PaymentTypeReportController : UIViewController
    {
        UIScrollView _scrollView;
        UIView _contentView;

        string End, Start;

        UILabel lbl_filterHead, lbl_DateHead;
        UIView filterView, DateView;

        //-----------------------------------
        UIView TodayView, MonthView, YearView;
        UILabel lbl_today, lbl_todayDate;
        UILabel lbl_Month, lbl_thisMonth;
        UILabel lbl_Year, lbl_thisYear;

        UIImageView TodaySelect_img, MonthSelect_img, YearSelect_img;
        //-----------------------------------

        UIView StartDateView, EndDateView;
        UILabel lbl_StartDate, lbl_EndDate;
        UIImageView btnSelectStartDate, btnSelectEndDate;
        UITextField txtStartDate, txtEndDate;

        UIButton btnViewReport;
        UIView line1, line2, line3,line0;

        BranchManage setBranch = new BranchManage();
        ReportBranchController ReportBranchPage;
        ReportSelectPaymentController PaymentPAge = null;
        PaymentReportDetailController PaymentDetailReport = null;

        UIDatePicker datePickerStartView, datePickerEndView;
        //------------------------------------------

        UILabel lbl_PaymentHead;
        UIView PaymentView;
        UILabel lbl_Payment,lbl_choosePayment;
        UIImageView btnSelectPayment;

        UIBarButtonItem selectBranch;
        public static int filterType = 0;
        bool today = false;
        bool month = false;
        bool year = false;
        public static string BranchSelect = null;
        public static bool isModifyPayment = false;

        TranPaymentManage Payment = new TranPaymentManage();

        internal static List<PaymentType> listChoosePayment= new List<PaymentType>();

        List<PaymentType> lsPayment;

        private UILabel lbl_filterBranch;
        private UIView BranchView;
        private UILabel lbl_Branch;
        private UILabel lbl_BranchSelect;
        private UIImageView BranchSelect_img;
        private bool dateend;
        private bool datestart;
        NSDateFormatter dateFormatter;
        private string filename;

        public PaymentTypeReportController() 
        {

        }
        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Utils.SetTitle(this.NavigationController, Utils.TextBundle("salebypaymentmethod_report", "Items"));
            this.NavigationController.SetNavigationBarHidden(false, false);
            
            if (isModifyPayment)
            {
                isModifyPayment = false;
                lbl_choosePayment.Text = "";
                SetDataPayment();
            }
            string namebranch = "";
            if (ReportController.listChooseBranch.Count == ReportController.listAllBranch.Count)
            {
                namebranch = Utils.TextBundle("allbranch", "Items");
            }
            else
            {
                foreach (var item in ReportController.listChooseBranch)
                {
                    if (namebranch == "")
                    {
                        namebranch += item.BranchName;
                    }
                    else
                    {
                        namebranch += " , " + item.BranchName;
                    }

                }
            }
            lbl_BranchSelect.Text = namebranch;
        }

        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            ReportController.listChooseBranch = new List<ORM.MerchantDB.Branch>(ReportController.listAllBranch);
            base.ViewDidLoad();
            try
            {
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                listChoosePayment = null;
                initAttribute();
                SetupAutoLayout();
                setDate();
                SetupPicker();
                setData();
                SetDataPayment();
                SetDefault();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        void SetDefault()
        {
            filterType = 1;
            today = true;
            month = false;
            year = false;
            lbl_today.TextColor = UIColor.FromRGB(0, 149, 218);
            lbl_Month.TextColor = UIColor.FromRGB(64, 64, 64);
            lbl_Year.TextColor = UIColor.FromRGB(64, 64, 64);
            TodaySelect_img.Hidden = false;
            MonthSelect_img.Hidden = true;
            YearSelect_img.Hidden = true;

            txtEndDate.Enabled = false;
            txtStartDate.Enabled = false;
        }

        void setData()
        {
            List<Gabana.Model.PaymentType> listPayments = new List<Gabana.Model.PaymentType>
            {
                new Model.PaymentType(){ Type ="CH" ,Detail = "Cash" , Logo = 1},
                new Model.PaymentType(){ Type ="Dr" ,Detail = "Debit Card", Logo = 2},
                new Model.PaymentType(){ Type ="Cr" ,Detail = "Credit Card", Logo = 2},
                new Model.PaymentType(){ Type ="GV" ,Detail = "Gift Voucher" , Logo = 3},
                new Model.PaymentType(){ Type ="MYQR",Detail = "myQR", Logo = 4},
            };
            // lsPayment = await Payment.GetTranPayment();
            Model.GabanaModel.gabanaMain.payments = listPayments;
            if (listChoosePayment == null || listChoosePayment.Count == 0)
            {
                listChoosePayment = listPayments;
            }
        }
        private async void SetDataPayment()
        {
            if (5 == listChoosePayment.Count)
            {
                lbl_choosePayment.Text = Utils.TextBundle("allpayment", "Items");
                return;
            }
            else if (listChoosePayment.Count == 0)
            {
                lbl_choosePayment.Text = Utils.TextBundle("none", "Items");
                return;
            }
            foreach (var item in listChoosePayment)
            {
                if (lbl_choosePayment.Text != "")
                {
                    lbl_choosePayment.Text += "," + item.Detail;

                }
                else
                {
                    lbl_choosePayment.Text = item.Detail;
                }
            }
        }
        void setDate()
        {
            lbl_todayDate.Text = DateTime.Now.ToString("d MMM", CultureInfo.CreateSpecificCulture("en-US"));
            lbl_thisMonth.Text = DateTime.Now.ToString("MMM", CultureInfo.CreateSpecificCulture("en-US"));
            lbl_thisYear.Text = DateTime.Now.ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

            var dateFormatter = new NSDateFormatter();
            dateFormatter.Locale = new NSLocale("en_US");
            dateFormatter.DateFormat = "dd MMM yyyy";


            txtEndDate.AttributedPlaceholder = new NSAttributedString(dateFormatter.ToString((NSDate)DateTime.UtcNow), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(226, 226, 226) });
            txtStartDate.AttributedPlaceholder = new NSAttributedString(dateFormatter.ToString((NSDate)DateTime.UtcNow), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(226, 226, 226) });

        }
        void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.BackgroundColor = UIColor.FromRGB(248,248,248);

            _contentView = new UIView();
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _contentView.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            line0 = new UIView();
            line0.BackgroundColor = UIColor.White;
            line0.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_filterBranch = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_filterBranch.Font = lbl_filterBranch.Font.WithSize(15);
            lbl_filterBranch.Text = Utils.TextBundle("branch", "Items");

            #region BranchView
            BranchView = new UIView();
            BranchView.BackgroundColor = UIColor.White;
            BranchView.TranslatesAutoresizingMaskIntoConstraints = false;
            //filterView.AddSubview(BranchView);

            lbl_Branch = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Branch.Font = lbl_Branch.Font.WithSize(15);
            lbl_Branch.Text = Utils.TextBundle("branch", "Items");
            BranchView.AddSubview(lbl_Branch);

            lbl_BranchSelect = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_BranchSelect.Font = lbl_BranchSelect.Font.WithSize(15);
            lbl_BranchSelect.Text = Utils.TextBundle("today", "Items");
            BranchView.AddSubview(lbl_BranchSelect);

            BranchSelect_img = new UIImageView();
            BranchSelect_img.Hidden = true;
            BranchSelect_img.Image = UIImage.FromFile("Next.png");
            BranchSelect_img.TranslatesAutoresizingMaskIntoConstraints = false;
            BranchView.AddSubview(BranchSelect_img);

            BranchView.UserInteractionEnabled = true;
            var tapGestureBracnh = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Branch_select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            BranchView.AddGestureRecognizer(tapGestureBracnh);

            #endregion

            lbl_filterHead = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247,86,0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_filterHead.Font = lbl_filterHead.Font.WithSize(15);
            lbl_filterHead.Text = Utils.TextBundle("filterbydate", "Items");

            #region filterView

            filterView = new UIView();
            filterView.BackgroundColor = UIColor.White;
            filterView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region TodayView
            TodayView = new UIView();
            TodayView.BackgroundColor = UIColor.White;
            TodayView.TranslatesAutoresizingMaskIntoConstraints = false;
            filterView.AddSubview(TodayView);

            lbl_today = new UILabel
            {
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_today.Font = lbl_today.Font.WithSize(15);
            lbl_today.Text = Utils.TextBundle("today", "Items");
            TodayView.AddSubview(lbl_today);

            lbl_todayDate = new UILabel
            {
                TextColor = UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_todayDate.Font = lbl_todayDate.Font.WithSize(15);
            lbl_todayDate.Text = Utils.TextBundle("today", "Items");
            TodayView.AddSubview(lbl_todayDate);

            TodaySelect_img = new UIImageView();
            TodaySelect_img.Hidden = true;
            TodaySelect_img.Image = UIImage.FromBundle("Check");
            TodaySelect_img.TranslatesAutoresizingMaskIntoConstraints = false;
            TodayView.AddSubview(TodaySelect_img);

            TodayView.UserInteractionEnabled = true;
            var tapGestureToday = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Today_select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            TodayView.AddGestureRecognizer(tapGestureToday);

            #endregion

            line1 = new UIView();
            line1.BackgroundColor = UIColor.FromRGB(226,226,226);
            line1.TranslatesAutoresizingMaskIntoConstraints = false;
            filterView.AddSubview(line1);

            #region MonthView
            MonthView = new UIView();
            MonthView.BackgroundColor = UIColor.White;
            MonthView.TranslatesAutoresizingMaskIntoConstraints = false;
            filterView.AddSubview(MonthView);

            lbl_Month = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Month.Font = lbl_Month.Font.WithSize(15);
            lbl_Month.Text = Utils.TextBundle("month", "Items");
            MonthView.AddSubview(lbl_Month);

            lbl_thisMonth = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_thisMonth.Font = lbl_thisMonth.Font.WithSize(15);
            lbl_thisMonth.Text = Utils.TextBundle("month", "Items");
            MonthView.AddSubview(lbl_thisMonth);

            MonthSelect_img = new UIImageView();
            MonthSelect_img.Hidden = true;
            MonthSelect_img.Image = UIImage.FromBundle("Check");
            MonthSelect_img.TranslatesAutoresizingMaskIntoConstraints = false;
            MonthView.AddSubview(MonthSelect_img);

            MonthView.UserInteractionEnabled = true;
            var tapGestureMonth = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Month_select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            MonthView.AddGestureRecognizer(tapGestureMonth);

            #endregion

            line2 = new UIView();
            line2.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            line2.TranslatesAutoresizingMaskIntoConstraints = false;
            filterView.AddSubview(line2);

            #region YearView
            YearView = new UIView();
            YearView.BackgroundColor = UIColor.White;
            YearView.TranslatesAutoresizingMaskIntoConstraints = false;
            filterView.AddSubview(YearView);

            lbl_Year = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Year.Font = lbl_Year.Font.WithSize(15);
            lbl_Year.Text = Utils.TextBundle("year", "Items");
            YearView.AddSubview(lbl_Year);

            lbl_thisYear = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_thisYear.Font = lbl_thisYear.Font.WithSize(15);
            lbl_thisYear.Text = Utils.TextBundle("year", "Items");
            YearView.AddSubview(lbl_thisYear);

            YearSelect_img = new UIImageView();
            YearSelect_img.Hidden = true;
            YearSelect_img.Image = UIImage.FromBundle("Check");
            YearSelect_img.TranslatesAutoresizingMaskIntoConstraints = false;
            YearView.AddSubview(YearSelect_img);

            YearView.UserInteractionEnabled = true;
            var tapGestureYear = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Year_select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            YearView.AddGestureRecognizer(tapGestureYear);

            #endregion

            #endregion

            lbl_DateHead = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_DateHead.Font = lbl_DateHead.Font.WithSize(15);
            lbl_DateHead.Text =  Utils.TextBundle("customizedate", "Items");

            #region DateView
            DateView = new UIView();
            DateView.BackgroundColor = UIColor.White;
            DateView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region StartDateView
            StartDateView = new UIView();
            StartDateView.BackgroundColor = UIColor.White;
            StartDateView.TranslatesAutoresizingMaskIntoConstraints = false;
            DateView.AddSubview(StartDateView);

            lbl_StartDate = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_StartDate.Font = lbl_StartDate.Font.WithSize(15);
            lbl_StartDate.Text = Utils.TextBundle("startdate", "Items");
            StartDateView.AddSubview(lbl_StartDate);

            txtStartDate = new UITextField
            {
                TextAlignment = UITextAlignment.Right,
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtStartDate.AttributedPlaceholder = new NSAttributedString("DD/MM/YYYY", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(226, 226, 226) });
            txtStartDate.Font = txtStartDate.Font.WithSize(15);
            txtStartDate.Enabled = false;
            StartDateView.AddSubview(txtStartDate);

            btnSelectStartDate = new UIImageView();
            btnSelectStartDate.Image = UIImage.FromFile("DbCalendar.png");
            btnSelectStartDate.TranslatesAutoresizingMaskIntoConstraints = false;
            StartDateView.AddSubview(btnSelectStartDate);

            StartDateView.UserInteractionEnabled = true;
            var tapGestureStart = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("StartDate:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            StartDateView.AddGestureRecognizer(tapGestureStart);

            #endregion

            line3 = new UIView();
            line3.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            line3.TranslatesAutoresizingMaskIntoConstraints = false;
            DateView.AddSubview(line3);

            #region EndDateView
            EndDateView = new UIView();
            EndDateView.BackgroundColor = UIColor.White;
            EndDateView.TranslatesAutoresizingMaskIntoConstraints = false;
            DateView.AddSubview(EndDateView);

            lbl_EndDate = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_EndDate.Font = lbl_EndDate.Font.WithSize(15);
            lbl_EndDate.Text = Utils.TextBundle("enddate", "Items");
            EndDateView.AddSubview(lbl_EndDate);

            txtEndDate = new UITextField
            {
                TextAlignment = UITextAlignment.Right,
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,  // this enables autolayout fo rour usernameField
            };
            txtEndDate.AttributedPlaceholder = new NSAttributedString("DD/MM/YYYY", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(226, 226, 226) });
            txtEndDate.Font = txtEndDate.Font.WithSize(15);
            txtEndDate.Enabled = false;
            EndDateView.AddSubview(txtEndDate);

            btnSelectEndDate = new UIImageView();
            btnSelectEndDate.Image = UIImage.FromFile("DbCalendar.png");
            btnSelectEndDate.TranslatesAutoresizingMaskIntoConstraints = false;
            EndDateView.AddSubview(btnSelectEndDate);

            EndDateView.UserInteractionEnabled = true;
            var tapGestureEnd = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("EndDate:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            EndDateView.AddGestureRecognizer(tapGestureEnd);

            #endregion
            #endregion

            lbl_PaymentHead = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_PaymentHead.Font = lbl_PaymentHead.Font.WithSize(15);
            lbl_PaymentHead.Text = Utils.TextBundle("filterbypayment", "Items");

            #region PaymentView
            PaymentView = new UIView();
            PaymentView.BackgroundColor = UIColor.White;
            PaymentView.TranslatesAutoresizingMaskIntoConstraints = false;

            lbl_Payment = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_Payment.Font = lbl_Payment.Font.WithSize(15);
            lbl_Payment.Text = Utils.TextBundle("payment", "Items");
            PaymentView.AddSubview(lbl_Payment);

            lbl_choosePayment = new UILabel
            {
                TextColor = UIColor.FromRGB(162, 162, 162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_choosePayment.Font = lbl_choosePayment.Font.WithSize(15);
            lbl_choosePayment.Text = Utils.TextBundle("allpayment", "Items");
            PaymentView.AddSubview(lbl_choosePayment);

            btnSelectPayment = new UIImageView();
            btnSelectPayment.Image = UIImage.FromBundle("Next");
            btnSelectPayment.TranslatesAutoresizingMaskIntoConstraints = false;
            PaymentView.AddSubview(btnSelectPayment);

            btnSelectPayment.UserInteractionEnabled = true;
            var tapGestureCustomer = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Payment_select:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnSelectPayment.AddGestureRecognizer(tapGestureCustomer);
            #endregion

            _contentView.AddSubview(line0);
            _contentView.AddSubview(lbl_filterBranch);
            _contentView.AddSubview(BranchView);
            _contentView.AddSubview(lbl_filterHead);
            _contentView.AddSubview(lbl_DateHead);
            _contentView.AddSubview(lbl_PaymentHead);
            _contentView.AddSubview(filterView);
            _contentView.AddSubview(DateView);
            _contentView.AddSubview(PaymentView);
            

            _scrollView.AddSubview(_contentView);

            #region BottomView
            btnViewReport = new UIButton();
            btnViewReport.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnViewReport.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnViewReport.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnViewReport.Layer.CornerRadius = 5f;
            btnViewReport.Layer.BorderWidth = 0.5f;
            btnViewReport.Enabled = true;
            btnViewReport.SetTitle(Utils.TextBundle("viewreport", "Items"), UIControlState.Normal);
            btnViewReport.TranslatesAutoresizingMaskIntoConstraints = false;
            btnViewReport.TouchUpInside += (sender, e) => {
                //filterType
                if (ReportController.listChooseBranch.Count == 0)
                {
                    Utils.ShowMessage(Utils.TextBundle("plschoosebranch", "Items"));
                    return;
                }
                var now = DateTime.UtcNow;
                if(listChoosePayment.Count > 0)
                {
                    if (filterType == 0) // date
                    {
                        if (string.IsNullOrEmpty(txtEndDate.Text))
                        {
                            var dateString = datePickerStartView.Date.Description;
                            dateFormatter.DateFormat = "dd MMM yyyy";
                            var currentDate = NSDate.Now;
                            txtEndDate.Text = dateFormatter.ToString(currentDate);
                            dateFormatter.DateFormat = "yyyyMMdd";
                            End = dateFormatter.ToString(currentDate);
                        }
                        dateFormatter.DateFormat = "dd-MM-yyyy";
                        var startdate = dateFormatter.ToString(datePickerStartView.Date);
                        var enddate = dateFormatter.ToString(datePickerEndView.Date);
                        filename = startdate + "-" + enddate;
                    }
                    else if (filterType == 1) //today
                    {
                        End = Utils.ChangeDateTimeReport(now);
                        Start = Utils.ChangeDateTimeReport(now);

                    }
                    else if (filterType == 2) //month
                    {
                        var startOfMonth = new DateTime(now.Year, now.Month, 1);
                        var DaysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                        var lastDayOfMonth = new DateTime(now.Year, now.Month, DaysInMonth);
                        Start = Utils.ChangeDateTimeReport(startOfMonth);
                        End = Utils.ChangeDateTimeReport(lastDayOfMonth);

                    }
                    else //year
                    {
                        var year = DateTime.UtcNow.Year;
                        var startOfYear = new DateTime(year, 1, 1);
                        var lastDayOfYear = DateTime.UtcNow;
                        Start = Utils.ChangeDateTimeReport(startOfYear);
                        End = Utils.ChangeDateTimeReport(lastDayOfYear);
                    }

                    PaymentDetailReport = new PaymentReportDetailController(listChoosePayment, txtStartDate.Text +"-"+ txtEndDate.Text, filterType, Start, End, filename);
                    this.NavigationController.PushViewController(PaymentDetailReport, false);
                }
                else
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("choosepay", "Items"));
                }


               
            };


            #endregion

            View.AddSubview(btnViewReport);
            View.AddSubview(_scrollView);
        }
        #region Select Mode
        //today, month, year
        [Export("Today_select:")]
        public void Today_select(UIGestureRecognizer sender)
        {
            if (!today)
            {
                filterType = 1;
                today = true;
                month = false;
                year = false;
                lbl_today.TextColor = UIColor.FromRGB(0, 149, 218);
                lbl_Month.TextColor = UIColor.FromRGB(64, 64, 64);
                lbl_Year.TextColor = UIColor.FromRGB(64, 64, 64);
                TodaySelect_img.Hidden = false;
                MonthSelect_img.Hidden = true;
                YearSelect_img.Hidden = true;

                txtEndDate.Enabled = false;
                txtStartDate.Enabled = false;
                txtStartDate.Text = "";
                txtEndDate.Text = "";
            }
            //else
            //{
            //    ClearFlag();
            //}

            // setflag
        }
        [Export("Month_select:")]
        public void Month_select(UIGestureRecognizer sender)
        {
            if (!month)
            {
                month = true;
                today = false;
                year = false;
                filterType = 2;
                lbl_today.TextColor = UIColor.FromRGB(64, 64, 64);
                lbl_Month.TextColor = UIColor.FromRGB(0, 149, 218);
                lbl_Year.TextColor = UIColor.FromRGB(64, 64, 64);
                TodaySelect_img.Hidden = true;
                MonthSelect_img.Hidden = false;
                YearSelect_img.Hidden = true;

                txtEndDate.Enabled = false;
                txtStartDate.Enabled = false;
                txtStartDate.Text = "";
                txtEndDate.Text = "";
            }
            //else
            //{
            //    ClearFlag();
            //}
            // setflag
        }
        [Export("Year_select:")]
        public void Year_select(UIGestureRecognizer sender)
        {
            if (!year)
            {
                month = false;
                today = false;
                year = true;
                filterType = 3;
                lbl_today.TextColor = UIColor.FromRGB(64, 64, 64);
                lbl_Month.TextColor = UIColor.FromRGB(64, 64, 64);
                lbl_Year.TextColor = UIColor.FromRGB(0, 149, 218);
                TodaySelect_img.Hidden = true;
                MonthSelect_img.Hidden = true;
                YearSelect_img.Hidden = false;

                txtEndDate.Enabled = false;
                txtStartDate.Enabled = false;
                txtStartDate.Text = "";
                txtEndDate.Text = "";
            }
            //else
            //{
            //    ClearFlag();
            //}
            // setflag
        }
        [Export("StartDate:")]
        public void StartDate(UIGestureRecognizer sender)
        {
            ClearFlag();

            txtStartDate.Enabled = true;
            txtStartDate.BecomeFirstResponder();
        }
        [Export("EndDate:")]
        public void EndDate(UIGestureRecognizer sender)
        {
            ClearFlag();
            //txtEndDate.Enabled = true;
            txtEndDate.BecomeFirstResponder();
        }
        #endregion

        private void ClearFlag()
        {
            today = false;
            month = false;
            year = false;
            filterType = 0;
            lbl_today.TextColor = UIColor.FromRGB(64, 64, 64);
            lbl_Month.TextColor = UIColor.FromRGB(64, 64, 64);
            lbl_Year.TextColor = UIColor.FromRGB(64, 64, 64);
            TodaySelect_img.Hidden = true;
            MonthSelect_img.Hidden = true;
            YearSelect_img.Hidden = true;
        }

        [Export("Payment_select:")]
        public void Payment_select(UIGestureRecognizer sender)
        {
            //open selectCategory page
            PaymentPAge = new ReportSelectPaymentController();
            this.NavigationController.PushViewController(PaymentPAge, false);
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
        private void SetupPicker()
        {
            UIToolbar toolbar = new UIToolbar();
            toolbar.Translucent = true;
            toolbar.SizeToFit();
            var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                if (datestart)
                {
                    var dateString = datePickerStartView.Date.Description;
                    dateFormatter.DateFormat = "dd MMM yyyy";
                    txtStartDate.Text = dateFormatter.ToString(datePickerStartView.Date);
                    dateFormatter.DateFormat = "yyyyMMdd";
                    Start = dateFormatter.ToString(datePickerStartView.Date);
                    datePickerEndView.MinimumDate = datePickerStartView.Date;
                    datestart = false;
                    txtEndDate.Enabled = true;
                    txtEndDate.Text = "";
                }
                if (dateend)
                {
                    var dateString = datePickerEndView.Date.Description;
                    dateFormatter.DateFormat = "dd MMM yyyy";
                    txtEndDate.Text = dateFormatter.ToString(datePickerEndView.Date);
                    dateFormatter.DateFormat = "yyyyMMdd";
                    End = dateFormatter.ToString(datePickerEndView.Date);
                    dateend = false;
                }

                View.EndEditing(true);
            });
            toolbar.SetItems(new UIBarButtonItem[] { flexible, doneButton }, true);
            //-------------------------------------------
            datePickerStartView = new UIDatePicker();
            datePickerEndView = new UIDatePicker();

            dateFormatter = new NSDateFormatter();
            dateFormatter.Locale = new NSLocale("en_US");


            var calendar = new NSCalendar(NSCalendarType.Gregorian);
            var currentDate = NSDate.Now;
            var components = new NSDateComponents();

            #region startDate Picker
            datePickerStartView.ValueChanged += (sender, s) => {

                var dateString = datePickerStartView.Date.Description;
                dateFormatter.DateFormat = "dd MMM yyyy";
                txtStartDate.Text = dateFormatter.ToString(datePickerStartView.Date);
                dateFormatter.DateFormat = "yyyyMMdd";
                Start = dateFormatter.ToString(datePickerStartView.Date);
                datePickerEndView.MinimumDate = datePickerStartView.Date;
                txtEndDate.Enabled = true;
                txtEndDate.Text = "";
            };
            components.Year = -100;
            NSDate minDate = calendar.DateByAddingComponents(components, NSDate.Now, NSCalendarOptions.None);
            datePickerStartView.MinimumDate = minDate;

            datePickerStartView.Locale = new NSLocale("th_USR");
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
            {
                datePickerStartView.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
            }

            datePickerStartView.MaximumDate = currentDate;
            datePickerStartView.Date = currentDate;
            datePickerStartView.Mode = UIDatePickerMode.Date;
            datePickerStartView.Calendar = new NSCalendar(NSCalendarType.Gregorian);

            txtStartDate.InputView = datePickerStartView;
            txtStartDate.InputAccessoryView = toolbar;
            #endregion

            #region EndDate Picker
            datePickerEndView.ValueChanged += (sender, s) => {

                var dateString = datePickerEndView.Date.Description;
                dateFormatter.DateFormat = "dd MMM yyyy";
                txtEndDate.Text = dateFormatter.ToString(datePickerEndView.Date);
                dateFormatter.DateFormat = "yyyyMMdd";
                End = dateFormatter.ToString(datePickerEndView.Date);
            };
            components.Year = -100;
            datePickerEndView.MinimumDate = minDate;

            datePickerEndView.Locale = new NSLocale("th_USR");
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
            {
                datePickerEndView.PreferredDatePickerStyle = UIDatePickerStyle.Wheels;
            }

            datePickerEndView.MaximumDate = currentDate;
            datePickerEndView.Date = currentDate;
            datePickerEndView.Mode = UIDatePickerMode.Date;
            datePickerEndView.Calendar = new NSCalendar(NSCalendarType.Gregorian);

            txtEndDate.InputView = datePickerEndView;
            txtEndDate.InputAccessoryView = toolbar;
            #endregion
            txtStartDate.EditingDidBegin += DatePickerStartView_TouchDown;
            txtEndDate.EditingDidBegin += DatePickerEndView_TouchDown;
        }
        private void DatePickerEndView_TouchDown(object sender, EventArgs e)
        {
            dateend = true;
        }

        private void DatePickerStartView_TouchDown(object sender, EventArgs e)
        {
            datestart = true;
        }
        void SetupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(btnViewReport.TopAnchor, -10).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            line0.TopAnchor.ConstraintEqualTo(line0.Superview.TopAnchor, 0).Active = true;
            line0.LeftAnchor.ConstraintEqualTo(line0.Superview.LeftAnchor, 0).Active = true;
            line0.RightAnchor.ConstraintEqualTo(line0.Superview.CenterXAnchor, 0).Active = true;
            line0.HeightAnchor.ConstraintEqualTo(1).Active = true;

            lbl_filterBranch.TopAnchor.ConstraintEqualTo(line0.BottomAnchor, 12).Active = true;
            lbl_filterBranch.LeftAnchor.ConstraintEqualTo(lbl_filterBranch.Superview.LeftAnchor, 15).Active = true;
            lbl_filterBranch.RightAnchor.ConstraintEqualTo(lbl_filterBranch.Superview.CenterXAnchor, 0).Active = true;
            lbl_filterBranch.HeightAnchor.ConstraintEqualTo(18).Active = true;

            BranchView.TopAnchor.ConstraintEqualTo(lbl_filterBranch.BottomAnchor, 10).Active = true;
            BranchView.LeftAnchor.ConstraintEqualTo(BranchView.Superview.LeftAnchor, 10).Active = true;
            BranchView.RightAnchor.ConstraintEqualTo(BranchView.Superview.RightAnchor, -10).Active = true;
            BranchView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbl_Branch.CenterYAnchor.ConstraintEqualTo(lbl_Branch.Superview.CenterYAnchor, -12).Active = true;
            lbl_Branch.LeftAnchor.ConstraintEqualTo(lbl_Branch.Superview.LeftAnchor, 20).Active = true;
            lbl_Branch.RightAnchor.ConstraintEqualTo(lbl_Branch.Superview.CenterXAnchor, 0).Active = true;
            lbl_Branch.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lbl_BranchSelect.TopAnchor.ConstraintEqualTo(lbl_Branch.BottomAnchor, 2).Active = true;
            lbl_BranchSelect.LeftAnchor.ConstraintEqualTo(lbl_BranchSelect.Superview.LeftAnchor, 20).Active = true;
            lbl_BranchSelect.RightAnchor.ConstraintEqualTo(lbl_BranchSelect.Superview.CenterXAnchor, 0).Active = true;
            lbl_BranchSelect.HeightAnchor.ConstraintEqualTo(18).Active = true;

            BranchSelect_img.CenterYAnchor.ConstraintEqualTo(BranchSelect_img.Superview.CenterYAnchor).Active = true;
            BranchSelect_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            BranchSelect_img.RightAnchor.ConstraintEqualTo(BranchSelect_img.Superview.RightAnchor, -30).Active = true;
            BranchSelect_img.HeightAnchor.ConstraintEqualTo(20).Active = true;


            lbl_PaymentHead.TopAnchor.ConstraintEqualTo(BranchView.BottomAnchor, 12).Active = true;
            lbl_PaymentHead.LeftAnchor.ConstraintEqualTo(lbl_PaymentHead.Superview.LeftAnchor, 15).Active = true;
            lbl_PaymentHead.RightAnchor.ConstraintEqualTo(lbl_PaymentHead.Superview.CenterXAnchor, 0).Active = true;
            lbl_PaymentHead.HeightAnchor.ConstraintEqualTo(18).Active = true;

            #region EmployeeView
            PaymentView.TopAnchor.ConstraintEqualTo(lbl_PaymentHead.BottomAnchor, 10).Active = true;
            PaymentView.LeftAnchor.ConstraintEqualTo(PaymentView.Superview.LeftAnchor, 10).Active = true;
            PaymentView.RightAnchor.ConstraintEqualTo(PaymentView.Superview.RightAnchor, -10).Active = true;
            PaymentView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            

            lbl_Payment.CenterYAnchor.ConstraintEqualTo(lbl_Payment.Superview.CenterYAnchor, -12).Active = true;
            lbl_Payment.LeftAnchor.ConstraintEqualTo(lbl_Payment.Superview.LeftAnchor, 20).Active = true;
            lbl_Payment.RightAnchor.ConstraintEqualTo(lbl_Payment.Superview.CenterXAnchor, 0).Active = true;
            lbl_Payment.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lbl_choosePayment.TopAnchor.ConstraintEqualTo(lbl_Payment.BottomAnchor, 2).Active = true;
            lbl_choosePayment.LeftAnchor.ConstraintEqualTo(lbl_choosePayment.Superview.LeftAnchor, 20).Active = true;
            lbl_choosePayment.RightAnchor.ConstraintEqualTo(lbl_choosePayment.Superview.CenterXAnchor, 0).Active = true;
            lbl_choosePayment.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSelectPayment.CenterYAnchor.ConstraintEqualTo(btnSelectPayment.Superview.CenterYAnchor).Active = true;
            btnSelectPayment.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSelectPayment.RightAnchor.ConstraintEqualTo(btnSelectPayment.Superview.RightAnchor, -25).Active = true;
            btnSelectPayment.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #endregion

            lbl_filterHead.TopAnchor.ConstraintEqualTo(PaymentView.BottomAnchor, 12).Active = true;
            lbl_filterHead.LeftAnchor.ConstraintEqualTo(lbl_filterHead.Superview.LeftAnchor, 15).Active = true;
            lbl_filterHead.RightAnchor.ConstraintEqualTo(lbl_filterHead.Superview.CenterXAnchor, 0).Active = true;
            lbl_filterHead.HeightAnchor.ConstraintEqualTo(18).Active = true;

            filterView.TopAnchor.ConstraintEqualTo(lbl_filterHead.BottomAnchor, 10).Active = true;
            filterView.LeftAnchor.ConstraintEqualTo(filterView.Superview.LeftAnchor, 10).Active = true;
            filterView.RightAnchor.ConstraintEqualTo(filterView.Superview.RightAnchor, -10).Active = true;
            filterView.HeightAnchor.ConstraintEqualTo(180).Active = true;

            #region filter
            #region today
            TodayView.TopAnchor.ConstraintEqualTo(TodayView.Superview.TopAnchor, 0).Active = true;
            TodayView.LeftAnchor.ConstraintEqualTo(TodayView.Superview.LeftAnchor, 0).Active = true;
            TodayView.RightAnchor.ConstraintEqualTo(TodayView.Superview.RightAnchor, 0).Active = true;
            TodayView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbl_today.CenterYAnchor.ConstraintEqualTo(lbl_today.Superview.CenterYAnchor, -12).Active = true;
            lbl_today.LeftAnchor.ConstraintEqualTo(lbl_today.Superview.LeftAnchor, 20).Active = true;
            lbl_today.RightAnchor.ConstraintEqualTo(lbl_today.Superview.CenterXAnchor, 0).Active = true;
            lbl_today.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lbl_todayDate.TopAnchor.ConstraintEqualTo(lbl_today.BottomAnchor, 2).Active = true;
            lbl_todayDate.LeftAnchor.ConstraintEqualTo(lbl_todayDate.Superview.LeftAnchor, 20).Active = true;
            lbl_todayDate.RightAnchor.ConstraintEqualTo(lbl_todayDate.Superview.CenterXAnchor, 0).Active = true;
            lbl_todayDate.HeightAnchor.ConstraintEqualTo(18).Active = true;

            TodaySelect_img.CenterYAnchor.ConstraintEqualTo(TodaySelect_img.Superview.CenterYAnchor).Active = true;
            TodaySelect_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            TodaySelect_img.RightAnchor.ConstraintEqualTo(TodaySelect_img.Superview.RightAnchor, -30).Active = true;
            TodaySelect_img.HeightAnchor.ConstraintEqualTo(20).Active = true;

            #endregion

            line1.TopAnchor.ConstraintEqualTo(TodayView.BottomAnchor, 0).Active = true;
            line1.LeftAnchor.ConstraintEqualTo(line1.Superview.LeftAnchor, 10).Active = true;
            line1.RightAnchor.ConstraintEqualTo(line1.Superview.RightAnchor, -10).Active = true;
            line1.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region Month
            MonthView.TopAnchor.ConstraintEqualTo(line1.BottomAnchor, 0).Active = true;
            MonthView.LeftAnchor.ConstraintEqualTo(MonthView.Superview.LeftAnchor, 0).Active = true;
            MonthView.RightAnchor.ConstraintEqualTo(MonthView.Superview.RightAnchor, 0).Active = true;
            MonthView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbl_Month.CenterYAnchor.ConstraintEqualTo(lbl_Month.Superview.CenterYAnchor, -12).Active = true;
            lbl_Month.LeftAnchor.ConstraintEqualTo(lbl_Month.Superview.LeftAnchor, 20).Active = true;
            lbl_Month.RightAnchor.ConstraintEqualTo(lbl_Month.Superview.CenterXAnchor, 0).Active = true;
            lbl_Month.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lbl_thisMonth.TopAnchor.ConstraintEqualTo(lbl_Month.BottomAnchor, 2).Active = true;
            lbl_thisMonth.LeftAnchor.ConstraintEqualTo(lbl_thisMonth.Superview.LeftAnchor, 20).Active = true;
            lbl_thisMonth.RightAnchor.ConstraintEqualTo(lbl_thisMonth.Superview.CenterXAnchor, 0).Active = true;
            lbl_thisMonth.HeightAnchor.ConstraintEqualTo(18).Active = true;

            MonthSelect_img.CenterYAnchor.ConstraintEqualTo(MonthSelect_img.Superview.CenterYAnchor).Active = true;
            MonthSelect_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            MonthSelect_img.RightAnchor.ConstraintEqualTo(MonthSelect_img.Superview.RightAnchor, -30).Active = true;
            MonthSelect_img.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion

            line2.TopAnchor.ConstraintEqualTo(MonthView.BottomAnchor, 0).Active = true;
            line2.LeftAnchor.ConstraintEqualTo(line2.Superview.LeftAnchor, 10).Active = true;
            line2.RightAnchor.ConstraintEqualTo(line2.Superview.RightAnchor, -10).Active = true;
            line2.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region Year
            YearView.TopAnchor.ConstraintEqualTo(line2.BottomAnchor, 0).Active = true;
            YearView.LeftAnchor.ConstraintEqualTo(YearView.Superview.LeftAnchor, 0).Active = true;
            YearView.RightAnchor.ConstraintEqualTo(YearView.Superview.RightAnchor, 0).Active = true;
            YearView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbl_Year.CenterYAnchor.ConstraintEqualTo(lbl_Year.Superview.CenterYAnchor, -12).Active = true;
            lbl_Year.LeftAnchor.ConstraintEqualTo(lbl_Year.Superview.LeftAnchor, 20).Active = true;
            lbl_Year.RightAnchor.ConstraintEqualTo(lbl_Year.Superview.CenterXAnchor, 0).Active = true;
            lbl_Year.HeightAnchor.ConstraintEqualTo(18).Active = true;

            lbl_thisYear.TopAnchor.ConstraintEqualTo(lbl_Year.BottomAnchor, 2).Active = true;
            lbl_thisYear.LeftAnchor.ConstraintEqualTo(lbl_thisYear.Superview.LeftAnchor, 20).Active = true;
            lbl_thisYear.RightAnchor.ConstraintEqualTo(lbl_thisYear.Superview.CenterXAnchor, 0).Active = true;
            lbl_thisYear.HeightAnchor.ConstraintEqualTo(18).Active = true;

            YearSelect_img.CenterYAnchor.ConstraintEqualTo(YearSelect_img.Superview.CenterYAnchor).Active = true;
            YearSelect_img.WidthAnchor.ConstraintEqualTo(20).Active = true;
            YearSelect_img.RightAnchor.ConstraintEqualTo(YearSelect_img.Superview.RightAnchor, -30).Active = true;
            YearSelect_img.HeightAnchor.ConstraintEqualTo(20).Active = true;
            #endregion
            #endregion

            lbl_DateHead.TopAnchor.ConstraintEqualTo(filterView.BottomAnchor, 12).Active = true;
            lbl_DateHead.LeftAnchor.ConstraintEqualTo(lbl_DateHead.Superview.LeftAnchor, 15).Active = true;
            lbl_DateHead.RightAnchor.ConstraintEqualTo(lbl_DateHead.Superview.CenterXAnchor, 0).Active = true;
            lbl_DateHead.HeightAnchor.ConstraintEqualTo(18).Active = true;

            DateView.TopAnchor.ConstraintEqualTo(lbl_DateHead.BottomAnchor, 10).Active = true;
            DateView.LeftAnchor.ConstraintEqualTo(DateView.Superview.LeftAnchor, 10).Active = true;
            DateView.RightAnchor.ConstraintEqualTo(DateView.Superview.RightAnchor, -10).Active = true;
            DateView.HeightAnchor.ConstraintEqualTo(120).Active = true;
            DateView.BottomAnchor.ConstraintEqualTo(DateView.Superview.BottomAnchor).Active = true;
            #region Date
            #region StartDate

            StartDateView.TopAnchor.ConstraintEqualTo(StartDateView.Superview.TopAnchor, 0).Active = true;
            StartDateView.LeftAnchor.ConstraintEqualTo(StartDateView.Superview.LeftAnchor, 0).Active = true;
            StartDateView.RightAnchor.ConstraintEqualTo(StartDateView.Superview.RightAnchor, 0).Active = true;
            StartDateView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lbl_StartDate.CenterYAnchor.ConstraintEqualTo(lbl_StartDate.Superview.CenterYAnchor, 0).Active = true;
            lbl_StartDate.LeftAnchor.ConstraintEqualTo(lbl_StartDate.Superview.LeftAnchor, 20).Active = true;
            lbl_StartDate.RightAnchor.ConstraintEqualTo(lbl_StartDate.Superview.CenterXAnchor, 0).Active = true;
            lbl_StartDate.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtStartDate.CenterYAnchor.ConstraintEqualTo(txtStartDate.Superview.CenterYAnchor, 0).Active = true;
            txtStartDate.LeftAnchor.ConstraintEqualTo(txtStartDate.Superview.CenterXAnchor).Active = true;
            txtStartDate.RightAnchor.ConstraintEqualTo(btnSelectStartDate.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            txtStartDate.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSelectStartDate.CenterYAnchor.ConstraintEqualTo(btnSelectStartDate.Superview.CenterYAnchor).Active = true;
            btnSelectStartDate.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSelectStartDate.RightAnchor.ConstraintEqualTo(btnSelectStartDate.Superview.RightAnchor, -20).Active = true;
            btnSelectStartDate.HeightAnchor.ConstraintEqualTo(28).Active = true;


            #endregion

            line3.TopAnchor.ConstraintEqualTo(StartDateView.BottomAnchor, 0).Active = true;
            line3.LeftAnchor.ConstraintEqualTo(line2.Superview.LeftAnchor, 10).Active = true;
            line3.RightAnchor.ConstraintEqualTo(line2.Superview.RightAnchor, -10).Active = true;
            line3.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;

            #region EndDate

            EndDateView.TopAnchor.ConstraintEqualTo(line3.BottomAnchor, 0).Active = true;
            EndDateView.LeftAnchor.ConstraintEqualTo(EndDateView.Superview.LeftAnchor, 0).Active = true;
            EndDateView.RightAnchor.ConstraintEqualTo(EndDateView.Superview.RightAnchor, 0).Active = true;
            EndDateView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            

            lbl_EndDate.CenterYAnchor.ConstraintEqualTo(lbl_EndDate.Superview.CenterYAnchor, 0).Active = true;
            lbl_EndDate.LeftAnchor.ConstraintEqualTo(lbl_EndDate.Superview.LeftAnchor, 20).Active = true;
            lbl_EndDate.RightAnchor.ConstraintEqualTo(lbl_EndDate.Superview.CenterXAnchor, 0).Active = true;
            lbl_EndDate.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtEndDate.CenterYAnchor.ConstraintEqualTo(txtEndDate.Superview.CenterYAnchor, 0).Active = true;
            txtEndDate.LeftAnchor.ConstraintEqualTo(txtEndDate.Superview.CenterXAnchor).Active = true;
            txtEndDate.RightAnchor.ConstraintEqualTo(btnSelectEndDate.SafeAreaLayoutGuide.LeftAnchor, -15).Active = true;
            txtEndDate.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnSelectEndDate.CenterYAnchor.ConstraintEqualTo(btnSelectEndDate.Superview.CenterYAnchor).Active = true;
            btnSelectEndDate.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnSelectEndDate.RightAnchor.ConstraintEqualTo(btnSelectEndDate.Superview.RightAnchor, -20).Active = true;
            btnSelectEndDate.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #endregion

            #endregion

            

            btnViewReport.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnViewReport.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnViewReport.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnViewReport.HeightAnchor.ConstraintEqualTo(45).Active = true;
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        [Export("Branch_select:")]
        public void Branch_select(UIGestureRecognizer sender)
        {
            var branchChoosePage = new ReportBranchController();
            this.NavigationController.PushViewController(branchChoosePage, false);
        }
    }
   
}