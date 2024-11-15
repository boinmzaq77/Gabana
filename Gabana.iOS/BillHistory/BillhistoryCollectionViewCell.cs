using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;

namespace Gabana.iOS
{
    public class BillhistoryCollectionViewCell : UICollectionViewCell
    {
        UILabel lblDate;
        UICollectionView BillHisByDateCollection;
        ReceiptHistoryDetailController HistoryPage;
        UIView upperView;
        int tall = 0;
        SubBillDateDataSource BillDataList;
        TranWithDetailsLocal TranHis;

        public BillhistoryCollectionViewCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.White;
            initAttribte();
            setupView();
        }
         void initAttribte()
        {
            #region upperView
            upperView = new UIView();
            upperView.BackgroundColor = UIColor.FromRGB(248,248,248);
            upperView.TranslatesAutoresizingMaskIntoConstraints = false;
            ContentView.AddSubview(upperView);

            lblDate = new UILabel();
            lblDate.Font = lblDate.Font.WithSize(15);
            lblDate.TextColor = UIColor.FromRGB(64,64,64);
            lblDate.TextAlignment = UITextAlignment.Left;
            lblDate.Text = "Day , DD/MM/YYYY";
            lblDate.TranslatesAutoresizingMaskIntoConstraints = false;
            upperView.AddSubview(lblDate);
            #endregion

            #region CollectionView
            UICollectionViewFlowLayout itemViewLayout = new UICollectionViewFlowLayout();
            itemViewLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemViewLayout.ItemSize = new CoreGraphics.CGSize(ContentView.Frame.Width, 70);

            //BillHisByDateCollection = new UICollectionView(frame: ContentView.Frame, layout: itemViewLayout);
            //BillHisByDateCollection.BackgroundColor = UIColor.Yellow;
            //BillHisByDateCollection.ShowsVerticalScrollIndicator = false;
            //BillHisByDateCollection.TranslatesAutoresizingMaskIntoConstraints = false;
            //BillHisByDateCollection.RegisterClassForCell(cellType: typeof(BillListViewCell), reuseIdentifier: "billListViewCell");


            //BillDataList = new SubBillDateDataSource(); // ส่ง list ไป
            //BillHisByDateCollection.DataSource = BillDataList;
            //SubBillHistoryCollectionDelegate SubBillCollectionDelegate = new SubBillHistoryCollectionDelegate();
            //SubBillCollectionDelegate.OnItemSelected += (indexPath) =>
            //{
            //    // do somthing
            //    var x = BillHistoryController.lsttransHistory[(int)indexPath.Row];
            //   // HistoryPage = new ReceiptHistoryDetailController(x);
            //   // DataCaching.BillHistoryNavigation.PushViewController(HistoryPage, false);
            //    // NavigationController.PushViewController(HistoryPage, false);
            //};
            //BillHisByDateCollection.Delegate = SubBillCollectionDelegate;


            //ContentView.AddSubview(BillHisByDateCollection);
            #endregion
        }
        void setupView()
        {
            #region upperView
            upperView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor,0).Active = true;
            upperView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            upperView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            upperView.HeightAnchor.ConstraintEqualTo(30).Active = true;
            upperView.WidthAnchor.ConstraintEqualTo(30).Active = true;
            upperView.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;

            lblDate.CenterYAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblDate.LeftAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblDate.RightAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            #endregion

            #region CollectionView
            //BillHisByDateCollection.TopAnchor.ConstraintEqualTo(upperView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            //BillHisByDateCollection.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            //BillHisByDateCollection.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        //    BillHisByDateCollection.HeightAnchor.ConstraintEqualTo(30).Active = true;
            #endregion
        }
        public string Date
        {
            get { return lblDate.Text; }
            set { lblDate.Text = value; }
        }
        public int sizeWidth
        {
            get { return 0; }
            set { 
                Utils.SetConstant(upperView.Constraints, NSLayoutAttribute.Width, value);
                //BillDataList = new SubBillDateDataSource(TranHis); // ส่ง list ไป
                //BillHisByDateCollection.DataSource = BillDataList;
            }
        }
        public int countRow
        {
            set {

                if(value>0)
                {
                    tall += 70;
                }
                //Utils.SetConstant(BillHisByDateCollection.Constraints , NSLayoutAttribute.Height , tall);
                //BillHisByDateCollection.HeightAnchor.ConstraintEqualTo(tall).Active = true;
            }
        }
    }
}