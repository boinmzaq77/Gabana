using CoreGraphics;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class itemNoteController : UIViewController 
    {
        int row=0;
        UIView SearchbarView;
        public static List<Note> CateLIST;
        UITextField txtsearch;
        int systemID = 60;
        public static int Select;
        NoteCategoryManage noteCategoryManage = new NoteCategoryManage();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        public static List<NoteCategory> NoteCategoryList;
        public static int NoteStatus = 0; // 0 add - 1 edit
        NoteManage noteManager = new NoteManage();
        UIButton btnSearch,btnAddCategory;
        UICollectionView NotetListCollectionView,NoteCategoryCollectionView;
        UIView dialogView,dialogBackgroundView,DialogTitleView,NoteCateView,AddCateView;
        UIButton btnCreateNoteCate, btnDeleteBranch;
        UIImageView btnBackToNote, btnAddNote;
        NoteCategory AddNoteCategory = new NoteCategory();
        NoteCategory EditNoteCategory;
        UILabel AddNoteTitle, lblNoteCate;
        UITextField txtNoteCate;
        public static NSIndexPath Last;
        UIBarButtonItem backButton;
        NotificationManager notificationManager = new NotificationManager();
        UIImageView emptyView;
        UILabel lbl_empty_Note;
        public itemNoteController()
        {
        }
        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
             // setAnimationsEnabled(false)
            this.NavigationController.SetNavigationBarHidden(false, false);
            //row = 0; // set default
            CateLIST = await FilterNoteLIST();
            
            
            NoteCategoryList = await GetNoteCategory();
            if (NoteCategoryList != null)
            {
                ((ItemNoteCategoryDataSource)NoteCategoryCollectionView.DataSource).ReloadData(NoteCategoryList);
            }
            NoteCategoryCollectionView.ReloadData();
            if (CateLIST == null || CateLIST.Count == 0)
            {
                NotetListCollectionView.Hidden = true;
                emptyView.Hidden = false;
                lbl_empty_Note.Hidden = false;
            }
            else
            {
                //Select = 0;
                ((ItemNoteDataSourceList)NotetListCollectionView.DataSource).ReloadData(NoteCategoryList, CateLIST);
                NotetListCollectionView.ReloadData();
                NotetListCollectionView.Hidden = false;
                emptyView.Hidden = true;
                lbl_empty_Note.Hidden = true;
            }
            NSIndexPath nSIndexPath;
            //if (Last == null)
            //{
            //     nSIndexPath = NSIndexPath.FromItemSection(0, 0);
            //}
            //else
            //{
            //    nSIndexPath = Last;
            //}
            
            //NoteMenuCollectionViewCell cell = NoteCategoryCollectionView.CellForItem(nSIndexPath) as NoteMenuCollectionViewCell;
            //(NoteCategoryCollectionView.Delegate as NoteMenuCollectionDelegate).lastSelectedCell = cell;
        }

        public async override void ViewDidLoad()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                base.ViewDidLoad();
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                initAttribute();
                SetupAutoLayout();

                if (CateLIST == null || CateLIST.Count == 0)
                {
                    NotetListCollectionView.Hidden = true;
                    emptyView.Hidden = false;
                    lbl_empty_Note.Hidden = false;
                }
                else
                {
                    Select = 0; 
                    ((ItemNoteDataSourceList)NotetListCollectionView.DataSource).ReloadData(NoteCategoryList,CateLIST);
                    NotetListCollectionView.Hidden = false;
                    emptyView.Hidden = true;
                    lbl_empty_Note.Hidden = true;
                }

                var refreshControl = new UIRefreshControl();
                refreshControl.AttributedTitle = new NSAttributedString(Utils.TextBundle("pulltorefresh", "Pull to refresh"));
                refreshControl.AddTarget(async (obj, sender) => {
                    
                    if (await GabanaAPI.CheckNetWork())
                    {
                        await notificationManager.NoteChange();
                        await notificationManager.NoteCategoryChange();
                    }
                    NoteCategoryList = await GetNoteCategory();
                    CateLIST = await FilterNoteLIST();
                    ((ItemNoteDataSourceList)NotetListCollectionView.DataSource).ReloadData(NoteCategoryList,CateLIST);
                    NotetListCollectionView.ReloadData();
                    
                    ((ItemNoteCategoryDataSource)NoteCategoryCollectionView.DataSource).ReloadData(NoteCategoryList);
                    NoteCategoryCollectionView.ReloadData();
                    refreshControl.EndRefreshing();
                }, UIControlEvent.ValueChanged);
                NotetListCollectionView.AlwaysBounceVertical = true;
                NotetListCollectionView.AddSubview(refreshControl);

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
           
        }
        async void initAttribute()
        {
            #region NoteCategoryCollectionView
            var flowLayout = new POS.MenuBarCollectionViewLayout();
            flowLayout.SizeForItem += (collectionView, layout, indexPath) =>
            {
                NSString nSString = new NSString((NoteCategoryCollectionView.DataSource as ItemNoteCategoryDataSource).GetItem(indexPath.Row));
                UIFont font = UIFont.SystemFontOfSize(13);
                CGSize cGSize = nSString.StringSize(font);

                return new CGSize(cGSize.Width + 40, 38);
            };

            NoteCategoryCollectionView = new UICollectionView(frame: View.Frame, layout: flowLayout);
            NoteCategoryCollectionView.BackgroundColor = UIColor.White;
            NoteCategoryCollectionView.ShowsHorizontalScrollIndicator = false;
            NoteCategoryCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            NoteCategoryCollectionView.RegisterClassForCell(cellType: typeof(NoteMenuCollectionViewCell), reuseIdentifier: "menuPosCell");
            NoteCategoryList = await GetNoteCategory();



            ItemNoteCategoryDataSource NoteCateData = new ItemNoteCategoryDataSource(NoteCategoryList);
            NoteCateData.OnLong += NoteCateData_OnLong;
            NoteCategoryCollectionView.DataSource = NoteCateData;

            NoteMenuCollectionDelegate posCollectionDelegate = new NoteMenuCollectionDelegate();
            posCollectionDelegate.OnItemSelected += async (indexPath) => {
                // do somthing
                Last = indexPath;
                Select = indexPath.Row;
                var category = NoteCategoryList.Where(x => x.Name == NoteCategoryList[(int)indexPath.Row].Name).FirstOrDefault();
                row = (int)category.SysNoteCategoryID;
                CateLIST = await FilterNoteLIST();
               ((ItemNoteDataSourceList)NotetListCollectionView.DataSource).ReloadData(NoteCategoryList,CateLIST);
                NotetListCollectionView.ReloadData();
            };
            NoteCategoryCollectionView.Delegate = posCollectionDelegate;
            
            View.AddSubview(NoteCategoryCollectionView);
            #endregion

            btnAddCategory = new UIButton();
            btnAddCategory.SetImage(UIImage.FromBundle("AddNoteCate"), UIControlState.Normal);
            btnAddCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddCategory.TouchUpInside += (sender, e) =>
            {
                //add Note Cate
                dialogBackgroundView.Hidden = false;
                dialogView.Hidden = false;

                btnCreateNoteCate.SetTitle("Add Note Category", UIControlState.Normal);
                txtNoteCate.Text = "";
                AddNoteTitle.Text = "Add Note Category";
                //dialogBackgroundView.Hidden = false;

                Utils.SetConstant(btnDeleteBranch.Constraints, NSLayoutAttribute.Width, 0);
                //btnDeleteBranch.LeftAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                var x = btnDeleteBranch.GetConstraintsAffectingLayout(UILayoutConstraintAxis.Horizontal);
                Utils.SetConstant(x, NSLayoutAttribute.Left, 0);
                btnDeleteBranch.Hidden = true;
            };
            btnAddCategory.BackgroundColor = UIColor.White;
            View.AddSubview(btnAddCategory);

            #region SearchbarView
            SearchbarView = new UIView();
            SearchbarView.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            SearchbarView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(SearchbarView);

            btnSearch = new UIButton();
            btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
            btnSearch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSearch.TouchUpInside += (sender, e) =>
            {
                //search
                txtsearch.BecomeFirstResponder();
            };
            SearchbarView.AddSubview(btnSearch);


            txtsearch = new UITextField
            {
                Placeholder = "",
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtsearch.BackgroundColor = UIColor.Clear;
            txtsearch.Font = txtsearch.Font.WithSize(15);
            txtsearch.ReturnKeyType = UIReturnKeyType.Done;
            txtsearch.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                GetFilterItemList();
                return true;
            };
            txtsearch.Placeholder = Utils.TextBundle("noteplace", "");
            txtsearch.EditingChanged += (object sender, EventArgs e) =>
            {
                if (txtsearch.Text != "")
                {
                    btnSearch.SetImage(UIImage.FromBundle("DeleteSize"), UIControlState.Normal);
                    btnSearch.TouchUpInside += (sender2, e2) =>
                    {
                        //search
                        txtsearch.Text = "";
                        btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                        btnSearch.TouchUpInside += (sender22, e22) =>
                        {
                            //search
                            txtsearch.BecomeFirstResponder();
                        };
                    };
                }
                else
                {
                    btnSearch.SetImage(UIImage.FromBundle("Search"), UIControlState.Normal);
                    btnSearch.TouchUpInside += (sender2, e2) =>
                    {
                        //search
                        txtsearch.BecomeFirstResponder();
                    };
                }
                
                
            };
            SearchbarView.AddSubview(txtsearch);


            #endregion


            #region emptyView
            emptyView = new UIImageView();
            emptyView.Hidden = true;
            emptyView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            emptyView.Image = UIImage.FromBundle("DefaultNote");
            emptyView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(emptyView);

            lbl_empty_Note = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.FromRGB(160, 160, 160),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lbl_empty_Note.Hidden = true;
            lbl_empty_Note.Lines = 2;
            lbl_empty_Note.Font = lbl_empty_Note.Font.WithSize(16);
            lbl_empty_Note.Text = "คุณยังไม่มี Note สามารถเพิ่ม\n ได้ที่ปุ่ม Add Note ด้านล่าง";
            View.AddSubview(lbl_empty_Note);
            #endregion

            #region NotetListCollectionView
            UICollectionViewFlowLayout NoteflowLayoutList = new UICollectionViewFlowLayout();
            NoteflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width) + 60, height: 60);
            NoteflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            NoteflowLayoutList.MinimumInteritemSpacing = 1;
            NoteflowLayoutList.MinimumLineSpacing = 1;


            NotetListCollectionView = new UICollectionView(frame: View.Frame, layout: NoteflowLayoutList);
            NotetListCollectionView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            NotetListCollectionView.AlwaysBounceVertical = true;
            NotetListCollectionView.ShowsVerticalScrollIndicator = false;
            NotetListCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            NotetListCollectionView.RegisterClassForCell(cellType: typeof(itemNoteCollectionViewCellList), reuseIdentifier: "itemNoteCellList");
            row = 0;
            CateLIST = await FilterNoteLIST();
            ItemNoteDataSourceList NoteDataList = new ItemNoteDataSourceList(CateLIST);

            NoteDataList.OnCardCellDelete += NoteDataList_OnCardCellDelete;
            //var gr = new UILongPressGestureRecognizer();
            //gr.AddTarget(() => this.ButtonLongPressed(gr));
            //NotetListCollectionView.AddGestureRecognizer(gr);

            ItemNoteCollectionDelegate itemNoteCollectionDelegate = new ItemNoteCollectionDelegate();
            itemNoteCollectionDelegate.OnItemSelected += (indexPath) =>
            {
                // do somthing
                
                Utils.SetTitle(this.NavigationController, "Edit Note");
                ItemsAddNoteController itemsAddNote = new ItemsAddNoteController(CateLIST[(int)indexPath.Row]);
                
                this.NavigationController.PushViewController(itemsAddNote, false);
            };
            NotetListCollectionView.Delegate = itemNoteCollectionDelegate;
            NotetListCollectionView.DataSource = NoteDataList;
            View.AddSubview(NotetListCollectionView);
            #endregion



            #region GotoAddNote
            btnAddNote = new UIImageView();
            btnAddNote.Image = UIImage.FromBundle("Add");
            btnAddNote.TranslatesAutoresizingMaskIntoConstraints = false;

            btnAddNote.UserInteractionEnabled = true;
            var tapGesture = new UITapGestureRecognizer(this,
                    new ObjCRuntime.Selector("AddBranch:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnAddNote.AddGestureRecognizer(tapGesture);
            View.AddSubview(btnAddNote);

            
            #endregion

             //------------------------------------------------
            #region add note CategoryView

            dialogBackgroundView = new UIView();
            dialogBackgroundView.BackgroundColor = UIColor.Black;
            dialogBackgroundView.Hidden = true;
            dialogBackgroundView.TranslatesAutoresizingMaskIntoConstraints = false;
            dialogBackgroundView.Layer.Opacity = 0.4f;
            View.AddSubview(dialogBackgroundView);

            dialogView = new UIView();
            dialogView.Hidden = true;
            dialogView.BackgroundColor = UIColor.FromRGB(226,226,226);
            dialogView.Layer.CornerRadius = 5;
            dialogView.ClipsToBounds = true;
            dialogView.TranslatesAutoresizingMaskIntoConstraints = false;
            // dialogBackgroundView.AddSubview(dialogView);
            View.AddSubview(dialogView);


            DialogTitleView = new UIView();
            DialogTitleView.BackgroundColor = UIColor.White;
            DialogTitleView.TranslatesAutoresizingMaskIntoConstraints = false;
            dialogView.AddSubview(DialogTitleView);

            btnBackToNote = new UIImageView();
            btnBackToNote.TranslatesAutoresizingMaskIntoConstraints = false;
            btnBackToNote.Image =UIImage.FromBundle("BackB");
            DialogTitleView.AddSubview(btnBackToNote);
            //-------------------------------------
            btnBackToNote.UserInteractionEnabled = true;
            var tapGesture0 = new UITapGestureRecognizer(this,
              new ObjCRuntime.Selector("Back:"))
            {
                NumberOfTapsRequired = 1 // change number as you want 
            };
            btnBackToNote.AddGestureRecognizer(tapGesture0);
            //-------------------------------------

            AddNoteTitle = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            AddNoteTitle.Font = UIFont.BoldSystemFontOfSize(15);
            AddNoteTitle.Font = AddNoteTitle.Font.WithSize(15);
            AddNoteTitle.Text = "Add Note Category";
            DialogTitleView.AddSubview(AddNoteTitle);

            NoteCateView = new UIView();
            NoteCateView.BackgroundColor = UIColor.White;
            NoteCateView.TranslatesAutoresizingMaskIntoConstraints = false;
            dialogView.AddSubview(NoteCateView);

            lblNoteCate = new UILabel
            {
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblNoteCate.Font = lblNoteCate.Font.WithSize(15);
            lblNoteCate.Text = "Note Category";
            NoteCateView.AddSubview(lblNoteCate);

            txtNoteCate = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB( 0 , 149 ,  218 ),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtNoteCate.ReturnKeyType = UIReturnKeyType.Done;
            txtNoteCate.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtNoteCate.EditingChanged += (object sender, EventArgs e) =>
            {
                btnCreateNoteCate.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                btnCreateNoteCate.SetTitleColor(UIColor.White, UIControlState.Normal);
            };
            txtNoteCate.AttributedPlaceholder = new NSAttributedString("Note Category", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtNoteCate.Font = txtNoteCate.Font.WithSize(15);
            NoteCateView.AddSubview(txtNoteCate);

            AddCateView = new UIView();
            AddCateView.BackgroundColor = UIColor.White;
            AddCateView.TranslatesAutoresizingMaskIntoConstraints = false;
            dialogView.AddSubview(AddCateView);

            btnCreateNoteCate = new UIButton();
            btnCreateNoteCate.Enabled = true;
            btnCreateNoteCate.TranslatesAutoresizingMaskIntoConstraints = false;
            btnCreateNoteCate.SetTitle("Add Note Category",UIControlState.Normal);
            btnCreateNoteCate.Layer.BorderColor = UIColor.FromRGB(0,149,218).CGColor;
            btnCreateNoteCate.Layer.BorderWidth = 1;
            btnCreateNoteCate.Layer.CornerRadius = 5;
            btnCreateNoteCate.ClipsToBounds = true;
            btnCreateNoteCate.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnCreateNoteCate.TouchUpInside += (sender, e) =>
            {
                btnCreateNoteCate.Enabled = false;
                //add
                if (txtNoteCate.Text!="" || txtNoteCate.Text !=null)
                {
                    if (EditNoteCategory != null)
                    {
                        editNoteCategory();
                        
                    }
                    else
                    {
                        createNoteCategory();
                    }
                    
                }
                btnCreateNoteCate.Enabled = true;
            };
            AddCateView.AddSubview(btnCreateNoteCate);

            btnDeleteBranch = new UIButton();
            //btnDeleteBranch.SetTitle("Save", UIControlState.Normal);
            //btnDeleteBranch.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnDeleteBranch.Layer.CornerRadius = 5f;
            btnDeleteBranch.BackgroundColor = UIColor.FromRGB(232, 232, 232);
            btnDeleteBranch.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDeleteBranch.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDeleteBranch.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDeleteBranch.Layer.CornerRadius = 5f;
            //btnDeleteBranch.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnDeleteBranch.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDeleteBranch.TouchUpInside += (sender, e) => {
                try
                {
                    btnDeleteBranch.Enabled = false;
                    var okCancelAlertController = UIAlertController.Create("", "Are you sure you want to delete note catagory ?", UIAlertControllerStyle.Alert);
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => DeleteNoteCat(EditNoteCategory)));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                    //Present Alert
                    PresentViewController(okCancelAlertController, true, null);
                    btnDeleteBranch.Enabled = true;
                }
                catch (Exception ex)
                {
                    Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                }
            };

            AddCateView.AddSubview(btnDeleteBranch);


            #endregion
        }
        [Export("AddBranch:")]
        public void AddItem(UIGestureRecognizer sender)
        {
            Utils.SetTitle(this.NavigationController, "Add Note");
            ItemsAddNoteController itemsAddNote = new ItemsAddNoteController();
            itemsAddNote.SelectCategory = row;
            this.NavigationController.PushViewController(itemsAddNote, false);
        }
        private async void DeleteNoteCat(NoteCategory editNoteCategory)
        {
            try
            {
                NoteCategory noteCategory = editNoteCategory;
                if (noteCategory == null)
                {
                    return;
                }
                noteCategory.DataStatus = 'D';
                //NoteCategory = 'D'
                NoteCategoryManage NoteCategoryManage = new NoteCategoryManage();
                var check = await NoteCategoryManage.UpdateNoteCategory(noteCategory);

                
                if (!check)
                {
                    Utils.ShowAlert(this, "ลบไม่สำเร็จ!", "ไม่สามารถลบข้อมูลได้");
                    return;
                }

                //Note ที่มี sysNoteCategory  = ตัวที่ลบ ก็ต้องอัพเดตให้ datastatus = 'D'
                NoteManage noteManage = new NoteManage();
                var updatetoNote = await noteManage.GetNoteBYCategory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);

                foreach (var note in updatetoNote)
                {
                    note.DataStatus = 'D';
                    await noteManage.UpdateNote(note);
                }

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNoteCatagory((int)noteCategory.MerchantID, (int)noteCategory.SysNoteCategoryID);
                }
                else
                {
                    noteCategory.FWaitSending = 2;
                    await NoteCategoryManage.UpdateNoteCategory(noteCategory);
                }
                dialogBackgroundView.Hidden = true;
                dialogView.Hidden = true;
                txtNoteCate.Text = "";
                EditNoteCategory = null;
                NoteCategoryList = await GetNoteCategory();
                if (NoteCategoryList != null)
                {
                    ((ItemNoteCategoryDataSource)NoteCategoryCollectionView.DataSource).ReloadData(NoteCategoryList);
                }
                NoteCategoryCollectionView.ReloadData();
                CateLIST = await FilterNoteLIST();
                ((ItemNoteDataSourceList)NotetListCollectionView.DataSource).ReloadData(NoteCategoryList, CateLIST);
                NotetListCollectionView.ReloadData();
                Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                
                return;
            }
        }

        private async void editNoteCategory()
        {
            
           

            if (string.IsNullOrEmpty(txtNoteCate.Text))
            {
                Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                return;
            }

            EditNoteCategory.Name = txtNoteCate.Text;
            EditNoteCategory.DateModified = DateTime.UtcNow;
            EditNoteCategory.DataStatus = 'M';
            EditNoteCategory.FWaitSending = 1;
            EditNoteCategory.WaitSendingTime = DateTime.UtcNow;
            NoteCategoryManage NoteCategoryManage = new NoteCategoryManage();
            var check = await NoteCategoryManage.UpdateNoteCategory(EditNoteCategory);
            if (!check)
            {
                Utils.ShowAlert(this, "ไม่สำเร็จ!", "ไม่สามารถเพิ่มข้อมูลได้");
                return;
            }
            if (await GabanaAPI.CheckNetWork())
            {
                JobQueue.Default.AddJobSendNoteCatagory((int)EditNoteCategory.MerchantID, (int)EditNoteCategory.SysNoteCategoryID);
            }
            else
            {
                EditNoteCategory.FWaitSending = 2;
                await noteCategoryManage.UpdateNoteCategory(EditNoteCategory);
            }

            dialogBackgroundView.Hidden = true;
            dialogView.Hidden = true;
            txtNoteCate.Text = "";
            EditNoteCategory = null;
            NoteCategoryCollectionView.ReloadData();
            Utils.ShowMessage("แก้ไข Note Category สำเร็จ");
        }

        private void NoteCateData_OnLong(NSIndexPath indexPath)
        {
            EditNoteCategory = NoteCategoryList[indexPath.Row];
            btnCreateNoteCate.SetTitle("Save", UIControlState.Normal);
            txtNoteCate.Text = EditNoteCategory.Name;
            AddNoteTitle.Text = "Edit Note Category";
            dialogBackgroundView.Hidden = false;
            Utils.SetConstant(btnDeleteBranch.Constraints, NSLayoutAttribute.Width, 45);
            //btnDeleteBranch.LeftAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            var x = btnDeleteBranch.GetConstraintsAffectingLayout(UILayoutConstraintAxis.Horizontal);
            Utils.SetConstant(x, NSLayoutAttribute.Left, 5);
            btnDeleteBranch.Hidden = false;
            dialogView.Hidden = false;
        }

        public void ButtonLongPressed(UILongPressGestureRecognizer longPressGestureRecognizer)
        {
            if (longPressGestureRecognizer.State != UIGestureRecognizerState.Began)
            {
                return;
            }

            UIAlertView alert = new UIAlertView("Test", "Button was long pressed", null, "Dismiss", null);
            alert.Show();
        }

        private void NoteDataList_OnCardCellDelete(NSIndexPath indexPath)
        {
            try
            {
                var okCancelAlertController = UIAlertController.Create("", "Are you sure you want to delete note?", UIAlertControllerStyle.Alert);

                //Add Actions
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => Deletenoth(indexPath)));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                //Present Alert
                PresentViewController(okCancelAlertController, true, null);
            }
            catch (Exception ex)
            {
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถลบข้อมูล Customer ได้");
                return;
            }
        }

        private async void Deletenoth(NSIndexPath indexPath)
        {
            try
            {
                NoteManage noteManage = new NoteManage();
                if (CateLIST[indexPath.Row] != null)
                {
                    Note Editnote = new Note();
                    Editnote = CateLIST[indexPath.Row];
                    Note noteDelete = new Note();
                    noteDelete = await noteManage.GetNote((int)Editnote.MerchantID, (int)Editnote.SysNoteID);
                    noteDelete.DataStatus = 'D';
                    noteDelete.FWaitSending = 1;
                    noteDelete.LastDateModified = DateTime.UtcNow;
                    var update = await noteManage.UpdateNote(noteDelete);
                    if (update)
                    {
                        //Toast.MakeText(this.Activity, Utils.TextBundle("deletesuccessfully", "Delete data successfully"), ToastLength.Short).Show();
                        //Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
                        if (await GabanaAPI.CheckNetWork())
                        {
                            JobQueue.Default.AddJobSendNote((int)Editnote.MerchantID, (int)Editnote.SysNoteID);
                        }
                        else
                        {
                            noteDelete.FWaitSending = 2;
                            await noteManage.UpdateNote(noteDelete);
                        }
                        CateLIST = await FilterNoteLIST();
                        ((ItemNoteDataSourceList)NotetListCollectionView.DataSource).ReloadData(NoteCategoryList,CateLIST);
                        NotetListCollectionView.ReloadData();
                        Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        async void GetFilterItemList()
        {
            try
            {
                if (string.IsNullOrEmpty(txtsearch.Text))
                {
                    CateLIST = await noteManager.GetNoteBYCategory((int)MainController.merchantlocal.MerchantID,row);
                }
                CateLIST = await noteManager.GetNoteBYCategorySearch((int)MainController.merchantlocal.MerchantID,row, txtsearch.Text);
                if (CateLIST == null)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "ไม่สามารถเรียกข้อมูลได้");
            }
        }

        [Export("Back:")]
        public void Back(UIGestureRecognizer sender)
        {
            dialogBackgroundView.Hidden = true;
            dialogView.Hidden = true;
            txtNoteCate.Text = "";
        }
        async void createNoteCategory()
        {
            try 
            {
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, systemID);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                string NoteCategory = txtNoteCate.Text;

                AddNoteCategory.MerchantID = MainController.merchantlocal.MerchantID;
                AddNoteCategory.SysNoteCategoryID = Convert.ToInt64(sys);
                AddNoteCategory.Ordinary = null;

                if (string.IsNullOrEmpty(txtNoteCate.Text))
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "กรุณากรอกข้อมูลให้ครบถ้วน");
                    return;
                }

                AddNoteCategory.Name = txtNoteCate.Text;
                AddNoteCategory.DateCreated = DateTime.UtcNow;
                AddNoteCategory.DateModified = DateTime.UtcNow;
                AddNoteCategory.DataStatus = 'I';
                AddNoteCategory.FWaitSending = 1;
                AddNoteCategory.WaitSendingTime = DateTime.UtcNow;

                var result = await noteCategoryManage.InsertNoteCategory(AddNoteCategory);
                if (!result)
                {
                    Utils.ShowAlert(this, "ไม่สำเร็จ!", "ไม่สามารถเพิ่มข้อมูลได้");
                    return;
                }
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNoteCatagory((int)AddNoteCategory.MerchantID, (int)AddNoteCategory.SysNoteCategoryID);
                }
                else
                {
                    AddNoteCategory.FWaitSending = 2;
                    await noteCategoryManage.UpdateNoteCategory(AddNoteCategory);
                }
                dialogBackgroundView.Hidden = true;
                dialogView.Hidden = true;
                txtNoteCate.Text = "";
                NoteCategoryList = await GetNoteCategory();
                if (NoteCategoryList != null)
                {
                    ((ItemNoteCategoryDataSource)NoteCategoryCollectionView.DataSource).ReloadData(NoteCategoryList);
                }
                NoteCategoryCollectionView.ReloadData();
                Utils.ShowMessage("เพิ่ม Note Category สำเร็จ");
            }
            catch (Exception ex) 
            {
                await Utils.ReloadInitialData();
            }
            
        }
        private async Task<List<Note>> FilterNoteLIST()
        {
            var item = new List<Note>();
            if (row == 0) // all
            {
                item = await noteManager.GetAllNote((int)MainController.merchantlocal.MerchantID);
            }
            else
            {
                item = await noteManager.GetNoteBYCategory(Convert.ToInt32(MainController.merchantlocal.MerchantID), row);
            }
            return item;
        }
        private async Task<List<NoteCategory>> GetNoteCategory()
        {
            NoteCategoryManage noteCateManager = new NoteCategoryManage();

            var menu = new List<NoteCategory>();
            menu.Add(new NoteCategory
            {
                MerchantID = MainController.merchantlocal.MerchantID,
                SysNoteCategoryID = 0,
                Ordinary = null,
                Name = "All",
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                DataStatus = 'A',
                FWaitSending = 0,
                WaitSendingTime = DateTime.UtcNow
            });
            var tmp = await noteCateManager.GetAllNoteCategory();
            for (int i = 0; i < tmp.Count; i++)
            {
                menu.Add(tmp[i]);
            }
            return menu;
        }
        void SetupAutoLayout()
        {
            #region NoteCategory
            NoteCategoryCollectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            NoteCategoryCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            NoteCategoryCollectionView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            NoteCategoryCollectionView.RightAnchor.ConstraintEqualTo(btnAddCategory.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            btnAddCategory.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            btnAddCategory.WidthAnchor.ConstraintEqualTo(40).Active = true;
            btnAddCategory.HeightAnchor.ConstraintEqualTo(40).Active = true;
            btnAddCategory.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;


            #endregion

            #region SearchBar
            SearchbarView.TopAnchor.ConstraintEqualTo(NoteCategoryCollectionView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            SearchbarView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SearchbarView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            SearchbarView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSearch.CenterYAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.CenterYAnchor, 0).Active = true;
            btnSearch.WidthAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.HeightAnchor.ConstraintEqualTo(26).Active = true;
            btnSearch.LeftAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;

            txtsearch.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.TopAnchor, 7).Active = true;
            txtsearch.RightAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtsearch.LeftAnchor.ConstraintEqualTo(btnSearch.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            txtsearch.BottomAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, -7).Active = true;

            #endregion

            #region emptyView
            emptyView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 40).Active = true;
            emptyView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            emptyView.WidthAnchor.ConstraintEqualTo(300).Active = true;
            emptyView.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;

            lbl_empty_Note.TopAnchor.ConstraintEqualTo(emptyView.SafeAreaLayoutGuide.BottomAnchor, 22).Active = true;
            lbl_empty_Note.HeightAnchor.ConstraintEqualTo(42).Active = true;
            lbl_empty_Note.WidthAnchor.ConstraintEqualTo(266).Active = true;
            lbl_empty_Note.CenterXAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            #endregion

            #region NoteLayout
            NotetListCollectionView.TopAnchor.ConstraintEqualTo(SearchbarView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            NotetListCollectionView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            NotetListCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            NotetListCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;

            btnAddNote.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -((int)View.Frame.Height * 29) / 1000).Active = true;
            btnAddNote.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnAddNote.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnAddNote.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -((int)View.Frame.Height * 29) / 1000).Active = true;
            #endregion

            //--------------------------------

            #region Dialog
            dialogBackgroundView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 0).Active = true;
            dialogBackgroundView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 0).Active = true;
            dialogBackgroundView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
            dialogBackgroundView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 0).Active = true;
            View.BringSubviewToFront(dialogBackgroundView);

            dialogView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor,(int)View.Frame.Height/4).Active = true;
            dialogView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 47).Active = true;
            dialogView.HeightAnchor.ConstraintEqualTo(175).Active = true;
            dialogView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -47).Active = true;
            View.BringSubviewToFront(dialogView);

            //dialogView,dialogBackgroundView,DialogTitleView,NoteCateView,AddCateView;
            DialogTitleView.TopAnchor.ConstraintEqualTo(dialogView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            DialogTitleView.LeftAnchor.ConstraintEqualTo(dialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DialogTitleView.HeightAnchor.ConstraintEqualTo(50).Active = true;
            DialogTitleView.RightAnchor.ConstraintEqualTo(dialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnBackToNote.CenterYAnchor.ConstraintEqualTo( DialogTitleView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnBackToNote.LeftAnchor.ConstraintEqualTo(DialogTitleView.SafeAreaLayoutGuide.LeftAnchor, 9).Active = true;
            btnBackToNote.HeightAnchor.ConstraintEqualTo(28).Active = true;
            btnBackToNote.WidthAnchor.ConstraintEqualTo(28).Active = true;

            AddNoteTitle.CenterYAnchor.ConstraintEqualTo(DialogTitleView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            AddNoteTitle.LeftAnchor.ConstraintEqualTo(btnBackToNote.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            AddNoteTitle.HeightAnchor.ConstraintEqualTo(18).Active = true;
            AddNoteTitle.RightAnchor.ConstraintEqualTo(DialogTitleView.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;

            #region Note Category
            NoteCateView.TopAnchor.ConstraintEqualTo(DialogTitleView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            NoteCateView.LeftAnchor.ConstraintEqualTo(dialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            NoteCateView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            NoteCateView.RightAnchor.ConstraintEqualTo(dialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblNoteCate.TopAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.TopAnchor,11).Active = true;
            lblNoteCate.LeftAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNoteCate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblNoteCate.RightAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            txtNoteCate.TopAnchor.ConstraintEqualTo(lblNoteCate.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtNoteCate.LeftAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtNoteCate.HeightAnchor.ConstraintEqualTo(18).Active = true;
            txtNoteCate.RightAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            #endregion

            #region button
            AddCateView.TopAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            AddCateView.LeftAnchor.ConstraintEqualTo(dialogView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            AddCateView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            AddCateView.RightAnchor.ConstraintEqualTo(dialogView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnCreateNoteCate.BottomAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //btnCreateNoteCate.LeftAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnCreateNoteCate.TopAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnCreateNoteCate.RightAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            btnDeleteBranch.TopAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDeleteBranch.BottomAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDeleteBranch.LeftAnchor.ConstraintEqualTo(AddCateView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDeleteBranch.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnDeleteBranch.RightAnchor.ConstraintEqualTo(btnCreateNoteCate.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;
            #endregion
            #endregion
        }
    }
}