using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Gabana.iOS.ITEMS
{
    public partial class ItemsAddNoteController : UIViewController
    {
        UILabel lblNote, lblBtnText;
        UITextField txtNote;
        UIView NoteView, BottomView1, btnAddMore, BottomView2;
        public int SubCount;
        UICollectionView SubnoteCollectionView;
        public List<Item> subnote;
        UIButton btnAddCategory, btnDelete,btnSave;
        UIImageView plusImg;
        int NoteStatus = 0;
        ItemManage Note = new ItemManage();
        UIScrollView scrollNoteView;
        public ItemsAddNoteController(int note) {
            this.NoteStatus = note;
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
            this.NavigationController.NavigationBar.BarTintColor = new UIColor(51 / 255f, 172 / 255f, 225 / 255f, 1);
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

            #region NoteView
                NoteView = new UIView();
                NoteView.BackgroundColor = UIColor.White;
                NoteView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(NoteView);

                lblNote = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false  
                };
                lblNote.Font = lblNote.Font.WithSize(15);
                lblNote.Text = "Note";
                View.AddSubview(lblNote);

                txtNote = new UITextField
                {
                    Placeholder = "Note",
                    BackgroundColor = UIColor.White,
                    TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false,  
                };
                txtNote.Font = txtNote.Font.WithSize(15);
                View.AddSubview(txtNote);
            #endregion

            #region BottomView

            #region BottomView 1 for add
            BottomView1 = new UIView();
            BottomView1.BackgroundColor = UIColor.FromRGB(248,248,248);
            BottomView1.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomView1);

                btnAddCategory = new UIButton();
                btnAddCategory.SetTitle("Add Note", UIControlState.Normal);
                btnAddCategory.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
                btnAddCategory.Layer.CornerRadius = 5f;
                btnAddCategory.Layer.BorderWidth = 0.5f;
                btnAddCategory.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
                btnAddCategory.BackgroundColor = UIColor.White;
                btnAddCategory.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAddCategory.TouchUpInside += (sender, e) => {
                    // sum items
                    this.NavigationController.PopViewController(false);
                };
                View.AddSubview(btnAddCategory);
            #endregion

            #region BottomView 2 for edit
            BottomView2 = new UIView();
            BottomView2.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            BottomView2.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomView2);

            btnDelete = new UIButton();
            btnDelete.Layer.CornerRadius = 5f;
            btnDelete.BackgroundColor = UIColor.FromRGB(226, 226, 226);
         //   btnDelete.SetBackgroundImage(UIImage.FromBundle("Trash"),UIControlState.Normal);

            UIImage image = UIImage.FromBundle("Trash");
            btnDelete.SetImage(image, UIControlState.Normal);
            btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDelete.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDelete.TouchUpInside += (sender, e) => {

            };
            BottomView2.AddSubview(btnDelete);

            btnSave = new UIButton();
            btnSave.SetTitle("Save", UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.BackgroundColor = UIColor.FromRGB(51,170,225);
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += (sender, e) => {

            };
            BottomView2.AddSubview(btnSave);
            #endregion

            #endregion

            #region SubnoteCollectionView

            getSubNote();
            //SubCount = this.subnote.Count;
            //if (this.subnote.Count == 0)
            //{
            //    this.subnote.Add(new Item());
            //}
            scrollNoteView = new UIScrollView();
            scrollNoteView.TranslatesAutoresizingMaskIntoConstraints = false;
            scrollNoteView.BackgroundColor = UIColor.FromRGB(248,248,248);
            View.AddSubview(scrollNoteView);

            btnAddMore = new UIView();
            btnAddMore.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddMore.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnAddMore.Layer.CornerRadius = 5;
            btnAddMore.ClipsToBounds = true;
            scrollNoteView.AddSubview(btnAddMore);

            lblBtnText = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblBtnText.Font = lblBtnText.Font.WithSize(15);
            lblBtnText.Text = "Add Extra Size";
            btnAddMore.AddSubview(lblBtnText);

            plusImg = new UIImageView();
            plusImg.Image = UIImage.FromBundle("AddItem");
            plusImg.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddMore.AddSubview(plusImg);

            btnAddMore.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("Addmore:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAddMore.AddGestureRecognizer(tapGesture0);


            UICollectionViewFlowLayout SubNoteflowLayoutList = new UICollectionViewFlowLayout();
            SubNoteflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: (int)View.Frame.Height * 9 / 100);
            SubNoteflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            SubnoteCollectionView = new UICollectionView(frame: View.Frame, layout: SubNoteflowLayoutList);
            SubnoteCollectionView.BackgroundColor = UIColor.FromRGB(248,248,248);
            SubnoteCollectionView.ShowsVerticalScrollIndicator = false;
            SubnoteCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            SubnoteCollectionView.RegisterClassForCell(cellType: typeof(SubNoteCollectionViewCell), reuseIdentifier: "subNoteCollectionViewCell");
            
            
            ItemSubNoteDataSource NoteDataList = new ItemSubNoteDataSource(subnote);
            NoteDataList.OnSubNoteDeleteIndex += (indexPath) =>
            {
                var x = (int)(indexPath).Item;
            };
            SubnoteCollectionView.DataSource = NoteDataList;
            scrollNoteView.AddSubview(SubnoteCollectionView);

            #endregion
            Textboxfocus(View);
            SetupAutoLayout();
            if (NoteStatus == 0)
            {
                BottomView2.Hidden = true;
                BottomView1.Hidden = false;
            }
            else
            {
                BottomView1.Hidden = true;
                BottomView2.Hidden = false;
            }
        }
        public async void getSubNote()
        {
            // this.subnote = await Note.GetNote();
        }
        void SetupAutoLayout()
        {
            #region NoteViewLayout
            NoteView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            NoteView.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height*90)/1000).Active = true;
            NoteView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            NoteView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblNote.TopAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.TopAnchor, ((int)View.Frame.Height*17)/1000).Active = true;
            lblNote.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblNote.LeftAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblNote.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtNote.TopAnchor.ConstraintEqualTo(lblNote.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtNote.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtNote.LeftAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtNote.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region SubnoteCollectionView
            scrollNoteView.TopAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            scrollNoteView.BottomAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            scrollNoteView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            scrollNoteView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddMore.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnAddMore.TopAnchor.ConstraintEqualTo(SubnoteCollectionView.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnAddMore.LeftAnchor.ConstraintEqualTo(scrollNoteView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddMore.RightAnchor.ConstraintEqualTo(scrollNoteView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            SubnoteCollectionView.TopAnchor.ConstraintEqualTo(scrollNoteView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SubnoteCollectionView.HeightAnchor.ConstraintEqualTo(80).Active = true;
            SubnoteCollectionView.LeftAnchor.ConstraintEqualTo(scrollNoteView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SubnoteCollectionView.RightAnchor.ConstraintEqualTo(scrollNoteView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            plusImg.CenterXAnchor.ConstraintEqualTo(btnAddMore.SafeAreaLayoutGuide.CenterXAnchor, -60).Active = true;
            plusImg.CenterYAnchor.ConstraintEqualTo(btnAddMore.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            plusImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            plusImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBtnText.LeftAnchor.ConstraintEqualTo(plusImg.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            lblBtnText.CenterYAnchor.ConstraintEqualTo(btnAddMore.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            #endregion

            #region BottomViewLayout
            #region BottomView 1 for add
            BottomView1.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView1.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 97) / 1000).Active = true;
            BottomView1.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView1.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddCategory.TopAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddCategory.BottomAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAddCategory.LeftAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddCategory.RightAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #region  BottomView 2 for edit
            BottomView2.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView2.HeightAnchor.ConstraintEqualTo(((int)View.Frame.Height * 97) / 1000).Active = true;
            BottomView2.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView2.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo((((int)View.Frame.Height * 68) / 1000)).Active = true;

            btnSave.TopAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSave.BottomAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(BottomView2.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion
            #endregion
        }
        [Export("Addmore:")]
        public void Addmore(UIGestureRecognizer sender)
        {
          //  this.subnote.Add(new Item());
            SubnoteCollectionView.HeightAnchor.ConstraintEqualTo(80*(int)subnote.Count).Active = true;
            SubnoteCollectionView.ReloadData();
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }
   
}