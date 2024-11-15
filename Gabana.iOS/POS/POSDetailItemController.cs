using Foundation;
using Gabana.iOS;
using Gabana.iOS.ITEMS;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.POS
{
    public partial class POSDetailItemController : UIViewController
    {
        public static Gabana.ORM.MerchantDB.Item SelectedPOSItemDetail ;
        UILabel lblFavText, lblEditText, lblOptionText, lblDeleteText;
        UIImageView FavImg, EditImg, OptionImg, DeleteImg;
        UIView btnView, CardFooterView,CardView;
        UIView FavView, EditView, OptionView, DeleteView;
        UIImageView TopImageView;
       // UIButton btnFav, btnEdit, btnDelete;
        UILabel lblItemShortName, lblItemNameFooter, lblItemPrice;
        AddItemControllerScroll additemPage;
        public POSDetailItemController()
        {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.Clear;

            View.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Clear:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            View.AddGestureRecognizer(tapGesture);

            #region CardView
            CardView = new UIView();
            CardView.TranslatesAutoresizingMaskIntoConstraints = false;
            CardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            CardView.Layer.CornerRadius = 10f;
            View.AddSubview(CardView);

            #region TopImageCardView
            TopImageView = new UIImageView();
            TopImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            CardView.AddSubview(TopImageView);


            lblItemShortName =  new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false  // this enables autolayout fo rour usernameField
            };
            lblItemShortName.Text = "ITEMS";
            lblItemShortName.Font = lblItemShortName.Font.WithSize(35);
            CardView.AddSubview(lblItemShortName);
            #endregion

            #region CardFooterView
            CardFooterView = new UIView();
            CardFooterView.TranslatesAutoresizingMaskIntoConstraints = false;
            CardFooterView.BackgroundColor = UIColor.Black;
            CardFooterView.Layer.Opacity = 0.2f;
            CardFooterView.Layer.CornerRadius = 10f;
            CardView.AddSubview(CardFooterView);

            lblItemNameFooter = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false 
            };
            lblItemNameFooter.Text = "Item Name";
            lblItemNameFooter.Font = lblItemNameFooter.Font.WithSize(13);
            CardFooterView.AddSubview(lblItemNameFooter);

            lblItemPrice = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false  
            };
            lblItemPrice.Text = "฿xx.xx";
            lblItemPrice.Font = lblItemPrice.Font.WithSize(16);
            CardFooterView.AddSubview(lblItemPrice);
            #endregion
            #endregion

            #region ButtonView
            btnView = new UIView();
            btnView.Layer.CornerRadius = 10f;
            btnView.TranslatesAutoresizingMaskIntoConstraints = false;
            btnView.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            View.AddSubview(btnView);

            #region FavView
            FavView = new UIView();
            FavView.Layer.CornerRadius = 10f;
            FavView.TranslatesAutoresizingMaskIntoConstraints = false;
            FavView.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            btnView.AddSubview(FavView);

            FavImg = new UIImageView();
            FavImg.Image = UIImage.FromBundle("Unfav");
            FavImg.TranslatesAutoresizingMaskIntoConstraints = false;
            FavView.AddSubview(FavImg);

            lblFavText = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblFavText.Text = "Favorite";
            lblFavText.Font = lblFavText.Font.WithSize(13);
            FavView.AddSubview(lblFavText);

            FavView.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Fav:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            FavView.AddGestureRecognizer(tapGesture0);
            #endregion

            #region EditView
            EditView = new UIView();
            EditView.Layer.CornerRadius = 10f;
            EditView.TranslatesAutoresizingMaskIntoConstraints = false;
            EditView.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            btnView.AddSubview(EditView);

            EditImg = new UIImageView();
            EditImg.Image = UIImage.FromBundle("Edit");
            EditImg.TranslatesAutoresizingMaskIntoConstraints = false;
            EditView.AddSubview(EditImg);

            lblEditText = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblEditText.Text = "Edit";
            lblEditText.Font = lblEditText.Font.WithSize(13);
            EditView.AddSubview(lblEditText);

            EditView.UserInteractionEnabled = true;
            var tapGesture1 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Edit:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            EditView.AddGestureRecognizer(tapGesture1);
            #endregion

            #region OptionView
            OptionView = new UIView();
            OptionView.Layer.CornerRadius = 10f;
            OptionView.TranslatesAutoresizingMaskIntoConstraints = false;
            OptionView.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            btnView.AddSubview(OptionView);

            OptionImg = new UIImageView();
            OptionImg.Image = UIImage.FromBundle("NoteTopping");
            OptionImg.TranslatesAutoresizingMaskIntoConstraints = false;
            OptionView.AddSubview(OptionImg);

            lblOptionText = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblOptionText.Text = "Option";
            lblOptionText.Font = lblFavText.Font.WithSize(13);
            OptionView.AddSubview(lblOptionText);

            OptionView.UserInteractionEnabled = true;
            var tapGesture2 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Option:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            OptionView.AddGestureRecognizer(tapGesture2);
            #endregion

            #region DeleteView
            DeleteView = new UIView();
            DeleteView.Layer.CornerRadius = 10f;
            DeleteView.TranslatesAutoresizingMaskIntoConstraints = false;
            DeleteView.BackgroundColor = UIColor.FromRGB(241, 250, 255);
            btnView.AddSubview(DeleteView);

            DeleteImg = new UIImageView();
            DeleteImg.Image = UIImage.FromBundle("Trash");
            DeleteImg.TranslatesAutoresizingMaskIntoConstraints = false;
            DeleteView.AddSubview(DeleteImg);

            lblDeleteText = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TextAlignment = UITextAlignment.Center,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDeleteText.Text = "Delete";
            lblDeleteText.Font = lblDeleteText.Font.WithSize(13);
            DeleteView.AddSubview(lblDeleteText);

            DeleteView.UserInteractionEnabled = true;
            var tapGesture3 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("Delete:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            DeleteView.AddGestureRecognizer(tapGesture3);
            #endregion
            #endregion

            SetupData();
            SetupAutoLayout();
        }
        void SetupData()
        {
            if (SelectedPOSItemDetail.Colors != null && SelectedPOSItemDetail.Colors != 0)
            {
                Utils.SetColor(CardView, (long)SelectedPOSItemDetail.Colors);
            }
            if(SelectedPOSItemDetail.PictureLocalPath != null && SelectedPOSItemDetail.PictureLocalPath != "")
            {
                SetImage(TopImageView, SelectedPOSItemDetail.PictureLocalPath);
            }
            lblItemShortName.Text = SelectedPOSItemDetail.ShortName;
            lblItemPrice.Text = "฿" + SelectedPOSItemDetail.Price.ToString();
            lblItemNameFooter.Text = SelectedPOSItemDetail.ItemName;
        }
        void SetupAutoLayout()
        {
            #region CardViewLayout
            CardView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height/3).Active = true;
            //  CardView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, (((int)View.Frame.Height/3)-((int)View.Frame.Width*8)/10)).Active = true;
            CardView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, (int)View.Frame.Height / 5).Active = true;
            CardView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            CardView.WidthAnchor.ConstraintEqualTo(((int)View.Frame.Width/2)+(int)((View.Frame.Width*2)/10)).Active = true;

            #region TopImageCardView
            TopImageView.HeightAnchor.ConstraintEqualTo((int)3*((View.Frame.Height/3)/ 4)).Active = true;
            TopImageView.TopAnchor.ConstraintEqualTo(CardView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            TopImageView.LeftAnchor.ConstraintEqualTo(CardView.SafeAreaLayoutGuide.LeftAnchor,0).Active = true;
            TopImageView.RightAnchor.ConstraintEqualTo(CardView.SafeAreaLayoutGuide.RightAnchor,0).Active = true;

            lblItemShortName.HeightAnchor.ConstraintEqualTo(41).Active = true;
            lblItemShortName.CenterXAnchor.ConstraintEqualTo(TopImageView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblItemShortName.CenterYAnchor.ConstraintEqualTo(TopImageView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;

            #endregion

            #region CardFooterView
            CardFooterView.BottomAnchor.ConstraintEqualTo(CardView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CardFooterView.TopAnchor.ConstraintEqualTo(TopImageView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            CardFooterView.LeftAnchor.ConstraintEqualTo(CardView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CardFooterView.RightAnchor.ConstraintEqualTo(CardView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblItemNameFooter.TopAnchor.ConstraintEqualTo(CardFooterView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblItemNameFooter.LeftAnchor.ConstraintEqualTo(CardFooterView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblItemNameFooter.HeightAnchor.ConstraintEqualTo(13).Active = true;
            lblItemNameFooter.RightAnchor.ConstraintEqualTo(CardFooterView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            lblItemPrice.TopAnchor.ConstraintEqualTo(lblItemNameFooter.SafeAreaLayoutGuide.BottomAnchor, 11).Active = true;
            lblItemPrice.LeftAnchor.ConstraintEqualTo(CardFooterView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            lblItemPrice.HeightAnchor.ConstraintEqualTo(19).Active = true;
            lblItemPrice.RightAnchor.ConstraintEqualTo(CardFooterView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #endregion

            #region ButtonViewLayout
            
            btnView.HeightAnchor.ConstraintEqualTo(((int)(View.Frame.Height*9)/100)).Active = true;
            btnView.TopAnchor.ConstraintEqualTo(CardView.SafeAreaLayoutGuide.BottomAnchor, 30).Active = true;
            btnView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            btnView.WidthAnchor.ConstraintEqualTo((int)((View.Frame.Width / 2) + ((View.Frame.Width * 2) / 10))).Active = true;

            #region FavView
            FavView.BottomAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            FavView.TopAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            FavView.LeftAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.LeftAnchor,0).Active = true;
            FavView.WidthAnchor.ConstraintEqualTo((int)(((View.Frame.Width / 2) + ((View.Frame.Width * 2) / 10)) / 4)).Active = true;

            FavImg.CenterXAnchor.ConstraintEqualTo(FavView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            FavImg.CenterYAnchor.ConstraintEqualTo(FavView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            FavImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            FavImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblFavText.CenterXAnchor.ConstraintEqualTo(FavView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblFavText.TopAnchor.ConstraintEqualTo(FavImg.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            lblFavText.WidthAnchor.ConstraintEqualTo(70).Active = true;
            #endregion

            #region EditView
            EditView.BottomAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            EditView.TopAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            EditView.LeftAnchor.ConstraintEqualTo(FavView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            EditView.WidthAnchor.ConstraintEqualTo((int)(((View.Frame.Width / 2) + ((View.Frame.Width * 2) / 10)) / 4)).Active = true;

            EditImg.CenterXAnchor.ConstraintEqualTo(EditView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            EditImg.CenterYAnchor.ConstraintEqualTo(EditView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            EditImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            EditImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblEditText.CenterXAnchor.ConstraintEqualTo(EditView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblEditText.TopAnchor.ConstraintEqualTo(EditImg.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblEditText.WidthAnchor.ConstraintEqualTo(70).Active = true;
            #endregion


            #region OptionView
            OptionView.BottomAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            OptionView.TopAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            OptionView.LeftAnchor.ConstraintEqualTo(EditView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            OptionView.WidthAnchor.ConstraintEqualTo((int)(((View.Frame.Width / 2) + ((View.Frame.Width * 2) / 10)) / 4)).Active = true;

            OptionImg.CenterXAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            OptionImg.CenterYAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.CenterYAnchor, -10).Active = true;
            OptionImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            OptionImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblOptionText.CenterXAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblOptionText.TopAnchor.ConstraintEqualTo(OptionImg.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblOptionText.WidthAnchor.ConstraintEqualTo(70).Active = true;
            #endregion

            #region DeleteView
            DeleteView.BottomAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            DeleteView.TopAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            DeleteView.LeftAnchor.ConstraintEqualTo(OptionView.SafeAreaLayoutGuide.RightAnchor, 1).Active = true;
            DeleteView.RightAnchor.ConstraintEqualTo(btnView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            DeleteImg.CenterXAnchor.ConstraintEqualTo(DeleteView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            DeleteImg.CenterYAnchor.ConstraintEqualTo(DeleteView.SafeAreaLayoutGuide.CenterYAnchor,-10).Active = true;
            DeleteImg.WidthAnchor.ConstraintEqualTo(28).Active = true;
            DeleteImg.HeightAnchor.ConstraintEqualTo(28).Active = true;

            lblDeleteText.CenterXAnchor.ConstraintEqualTo(DeleteView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblDeleteText.TopAnchor.ConstraintEqualTo(DeleteImg.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblDeleteText.WidthAnchor.ConstraintEqualTo(70).Active = true;
            #endregion

            #endregion
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            if (value != null && value != "")
            {
                ImageView.Image = UIImage.FromBundle(value);
            }
            else
            {
                ImageView.Image = null;
            }
        }

        [Export("Fav:")]
        public void Fav(UIGestureRecognizer sender)
        {

        }
        [Export("Edit:")]
        public void Edit(UIGestureRecognizer sender)
        {
            //Edit at SelectedPOSItemDetail
            DataCaching.DetailItemNavigation.DismissViewController(true, null);
            
            var additemPage = new AddItemControllerScroll(SelectedPOSItemDetail);
            
            DataCaching.DetailItemNavigation.PushViewController(additemPage, false);
        }
        [Export("Option:")]
        public void Option(UIGestureRecognizer sender)
        {

        }
        [Export("Delete:")]
        public void Delete(UIGestureRecognizer sender)
        {
            var okCancelAlertController = UIAlertController.Create("", "ต้องการจะลบ Item ?", UIAlertControllerStyle.Alert);
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                alert => DeleteItem()));
            okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

            //Present Alert

            PresentViewController(okCancelAlertController, true, null);
        }

        [Export("Clear:")]
        public void Clear(UIGestureRecognizer sender)
        {
            DataCaching.DetailItemNavigation.DismissViewController(false, null);
        }

        private async void DeleteItem()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            long SysItemId = SelectedPOSItemDetail.SysItemID;
            ItemManage itemManage = new ItemManage();
            Item DeleteItem = new Item();
            DeleteItem = await itemManage.GetItem((int)MainController.merchantlocal.MerchantID, (int)SysItemId);
            DeleteItem.DataStatus = 'D';
            DeleteItem.FWaitSending = 1;
            DeleteItem.WaitSendingTime = DateTime.UtcNow;
            DeleteItem.TrackStockDateTime = DateTime.UtcNow;
            DeleteItem.LastDateModified = DateTime.UtcNow;
            var result = await itemManage.UpdateItem(DeleteItem);
            if (result)
            {
                Utils.ShowAlert(this, "สำเร็จ !", "แก้ไขข้อมูลสำเร็จ");
            }
            else
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูลได้");
                return;
            }
            var getItem = await itemManage.GetItem((int)MainController.merchantlocal.MerchantID, (int)SysItemId);

            // senttocloud 
            if (await GabanaAPI.CheckNetWork())
            {
                JobQueue.Default.AddJobSendItem((int)DataCashingAll.MerchantId, (int)SysItemId);
            }
            DataCaching.DetailItemNavigation.DismissViewController(false, null);
        }
    }
}