using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
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
    public partial class OptionController : UIViewController
    {
        TranDetailItemWithTopping item = new TranDetailItemWithTopping();
        UIScrollView _scrollView;
        UIView _contentView, CommentView, BottomView, SizeView, ToppingView, noteView;
        UIButton btnAddToCart;
        UILabel lblSize, lblSizeChooseOne, lblExtraTopping, lblChooseExtraTopping, lblNote, lblChooseNote, lblComment;
        UITextField txtComment;
        UICollectionView SizeCollectionView, MenuExtraTopppingCollectionView, ExtratoppingCollectionView, MenuNoteCollectionView, NoteCollectionView;
        public List<ItemExSize> allExtra = new List<ItemExSize>();
        public ItemExSize allExtrachoose;
        public List<Item> Topping = new List<Item>();
        public List<Item> Toppingshow = new List<Item>();
        public List<Item> Toppingchoose = new List<Item>();
        public List<Category> MenuTopping = new List<Category>();
        public List<Note> NoteList = new List<Note>();
        List<NoteCategory> MenuNote = new List<NoteCategory>();
        ItemManage itemManager = new ItemManage();
        public static int Select;
        public bool edit; 
        public static int Selectextra;
        NoteManage Notemanager = new NoteManage();
        public static List<TranDetailItemTopping> lstTranSelectTopping = new List<TranDetailItemTopping>();
        public OptionController()
        {
        }
        public OptionController(TranDetailItemWithTopping item , bool edit)
        {
            this.item = item;
            lstTranSelectTopping = item.tranDetailItemToppings;
            this.edit = edit;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            View.BackgroundColor = UIColor.White;



            //  this.NavigationController.SetNavigationBarHidden(false, false);
        }
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            double x = 99.99;
            var x2 = Math.Ceiling(x);
            string x1 = x.ToString("N0");

            this.NavigationController.SetNavigationBarHidden(false, false);
            try
            {
                Select = 0;
                Selectextra = 0;
                MenuTopping.Add(new Category { MerchantID = DataCashingAll.MerchantId, SysCategoryID = 0, Name = "None", Ordinary = null, DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 0, WaitSendingTime = DateTime.UtcNow, LinkProMaxxID = null });

                var MenuTopping1 = await GetMenuTopping();
                MenuTopping.AddRange(MenuTopping1);
                //MenuTopping.Add(new Category { MerchantID = DataCashingAll.MerchantId, SysCategoryID = 0, Name = "None", Ordinary = null, DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 0, WaitSendingTime = DateTime.UtcNow, LinkProMaxxID = null });
                 Topping = await itemManager.GetToppingOnlyNoneGroup();
                Toppingshow = Topping;
                initAttribute();
                setupAutoLayout();
                if (allExtra == null || allExtra.Count == 0)
                {
                    SizeView.Hidden = true;
                    SizeView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    //ExtratoppingCollectionView.Hidden = true;
                    //ExtratoppingCollectionView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                }
                else
                {
                    var ex = allExtra.Where(b => b.ExSizeName == item.tranDetailItem.SizeName).FirstOrDefault();
                    allExtrachoose = ex;
                }
                var toppingcheck = await itemManager.GetToppingItem();
                if (toppingcheck == null || toppingcheck.Count == 0)
                {
                    ToppingView.Hidden = true;
                    ToppingView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    MenuExtraTopppingCollectionView.Hidden = true;
                    MenuExtraTopppingCollectionView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    ExtratoppingCollectionView.Hidden = true;
                    ExtratoppingCollectionView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                }
                var notecheck = await Notemanager.GetAllNote(DataCashingAll.MerchantId);
                if (notecheck == null || notecheck.Count == 0)
                {
                    noteView.Hidden = true;
                    noteView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    MenuNoteCollectionView.Hidden = true;
                    MenuNoteCollectionView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                    NoteCollectionView.Hidden = true;
                    NoteCollectionView.HeightAnchor.ConstraintEqualTo(0).Active = true;
                }
                txtComment.Text = item.tranDetailItem.Comments ?? "";
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }

        }
        async void initAttribute()
        {
            _scrollView = new UIScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

            _contentView = new UIView();
            _contentView.BackgroundColor = UIColor.White;
            _contentView.TranslatesAutoresizingMaskIntoConstraints = false;

            #region SizeView
            SizeView = new UIView();
            SizeView.BackgroundColor = UIColor.White;
            SizeView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblSize = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSize.Font = lblSize.Font.WithSize(15);
            lblSize.Text = "Size *";
            SizeView.AddSubview(lblSize);

            lblSizeChooseOne = new UILabel
            {
                TextColor = UIColor.FromRGB(200, 200, 200),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSizeChooseOne.Font = lblSizeChooseOne.Font.WithSize(15);
            lblSizeChooseOne.Text = "เลือกได้ 1";
            SizeView.AddSubview(lblSizeChooseOne);
            #endregion

            #region SizeCollectionView
            allExtra = await setSizeOptionBind();

            if (allExtra.Count != 0)
            {
                var itemthis = await itemManager.GetItem((int)item.tranDetailItem.MerchantID , (int)item.tranDetailItem.SysItemID);
                if (item!=null)
                {
                    var defaultSize = new ItemExSize() { ExSizeNo = 999, ExSizeName = "Default Size", Price = itemthis.Price };
                    allExtra.Insert(0, defaultSize);
                }
               
            }




            UICollectionViewFlowLayout SizeFllowLayout = new UICollectionViewFlowLayout();
            SizeFllowLayout.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 20);
            SizeFllowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            SizeCollectionView = new UICollectionView(frame: View.Frame, layout: SizeFllowLayout);
            SizeCollectionView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            SizeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            SizeCollectionView.ScrollEnabled = false;
            SizeCollectionView.RegisterClassForCell(cellType: typeof(OptionSizeCollectionViewCell), reuseIdentifier: "optionSizeCollectionViewCell");
            OptionSizeDetailDataSource itemExtraDetailDataSource = new OptionSizeDetailDataSource(this.allExtra, item);
            itemExtraDetailDataSource.OnExtraSizeSelectIndex += (indexPath) =>
            {
                var x = this.allExtra[indexPath.Row];
                allExtrachoose = x;
            };
            SizeCollectionView.DataSource = itemExtraDetailDataSource;
            #endregion

            #region ToppingView
            ToppingView = new UIView();
            ToppingView.BackgroundColor = UIColor.White;
            ToppingView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblExtraTopping = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblExtraTopping.Font = lblExtraTopping.Font.WithSize(15);
            lblExtraTopping.Text = Utils.TextBundle("extratopping", "Items");
            ToppingView.AddSubview(lblExtraTopping);

            lblChooseExtraTopping = new UILabel
            {
                TextColor = UIColor.FromRGB(200, 200, 200),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblChooseExtraTopping.Font = lblChooseExtraTopping.Font.WithSize(15);
            lblChooseExtraTopping.Text = Utils.TextBundle("choosemore1", "Items");
            ToppingView.AddSubview(lblChooseExtraTopping);
            #endregion

            #region ExtratoppingCollectionView
            var flowLayout = new POS.MenuBarCollectionViewLayout();
            flowLayout.SizeForItem += (collectionView, layout, indexPath) =>
            {
                NSString nSString = new NSString((MenuExtraTopppingCollectionView.DataSource as MenuToppingDataSource).GetItem(indexPath.Row));
                UIFont font = UIFont.SystemFontOfSize(13);
                CGSize cGSize = nSString.StringSize(font);

                return new CGSize(cGSize.Width + 40, 38);
            };

            MenuExtraTopppingCollectionView = new UICollectionView(frame: View.Frame, layout: flowLayout);
            MenuExtraTopppingCollectionView.BackgroundColor = UIColor.White;
            MenuExtraTopppingCollectionView.ShowsHorizontalScrollIndicator = false;
            MenuExtraTopppingCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuExtraTopppingCollectionView.RegisterClassForCell(cellType: typeof(POSMenuCollectionViewCell), reuseIdentifier: "menuToppingCell");
            //MenuTopping = await GetMenuTopping();
            //MenuTopping.Add(new Category { MerchantID = DataCashingAll.MerchantId, SysCategoryID = 0, Name = "None", Ordinary = null, DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 0, WaitSendingTime = DateTime.UtcNow, LinkProMaxxID = null });

            MenuToppingDataSource menuToppingData = new MenuToppingDataSource(MenuTopping);
            MenuExtraTopppingCollectionView.DataSource = menuToppingData;
            POSMenuCollectionDelegate menuToppingCollectionDelegate = new POSMenuCollectionDelegate();
            menuToppingCollectionDelegate.OnItemSelected += async (indexPath) =>
            {
                // do somthing
                Selectextra = indexPath.Row;
                if ((int)MenuTopping[(int)indexPath.Item].SysCategoryID == 0)
                {
                    Toppingshow = await itemManager.GetToppingOnlyNoneGroup();
                }
                else
                {
                    Toppingshow = await itemManager.GetToppingItemByCategory((int)MenuTopping[(int)indexPath.Item].SysCategoryID);
                }

                ((OptionToppingDataSource)ExtratoppingCollectionView.DataSource).ReloadData(Toppingshow);
                ExtratoppingCollectionView.ReloadData();
                Utils.SetConstant(ExtratoppingCollectionView.Constraints, NSLayoutAttribute.Height, Toppingshow.Count * 40);

            };
            MenuExtraTopppingCollectionView.Delegate = menuToppingCollectionDelegate;
            //Topping = await InitGetTopping();
            //Topping = await itemManager.GetToppingItemByCategory((int)MenuTopping[0]?.SysCategoryID);
            //Toppingshow = Topping;
            //Topping = await itemManager.GetToppingItemByCategory((int)MenuTopping[0].SysCategoryID);
            UICollectionViewFlowLayout ToppingListFlowLayout = new UICollectionViewFlowLayout();
            ToppingListFlowLayout.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 40);
            ToppingListFlowLayout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            ToppingListFlowLayout.MinimumLineSpacing = 1f;
            ToppingListFlowLayout.MinimumInteritemSpacing = 1f;
            ExtratoppingCollectionView = new UICollectionView(frame: View.Frame, layout: ToppingListFlowLayout);
            ExtratoppingCollectionView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            ExtratoppingCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            ExtratoppingCollectionView.ScrollEnabled = false;
            ExtratoppingCollectionView.RegisterClassForCell(cellType: typeof(OptionToppingCollectionViewCell), reuseIdentifier: "optionToppingCollectionViewCell");
            Utils.SetConstant(ExtratoppingCollectionView.Constraints, NSLayoutAttribute.Height, Toppingshow.Count * 40);
            OptionToppingDataSource ToppingDataSource = new OptionToppingDataSource(Toppingshow);
            ToppingDataSource.OnExtraToppingSelectIndex += (indexPath) =>
             {
                 var x = (int)(indexPath).Item;
             };

            OptionToppingCollectionDelegate toppingCollectionDelegate = new OptionToppingCollectionDelegate();
            toppingCollectionDelegate.OnItemSelected += async (indexPath) =>
            {
                // do somthing
                OptionToppingCollectionViewCell cell = ExtratoppingCollectionView.CellForItem(indexPath) as OptionToppingCollectionViewCell;
                cell.Selected = true;
                ExtratoppingCollectionView.ReloadItems(new NSIndexPath[] { indexPath });

            };
            ExtratoppingCollectionView.Delegate = toppingCollectionDelegate;
            ExtratoppingCollectionView.DataSource = ToppingDataSource;
            #endregion

            #region noteView
            noteView = new UIView();
            noteView.BackgroundColor = UIColor.White;
            noteView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblNote = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblNote.Font = lblNote.Font.WithSize(15);
            lblNote.Text = Utils.TextBundle("note", "Items");
            noteView.AddSubview(lblNote);

            lblChooseNote = new UILabel
            {
                TextColor = UIColor.FromRGB(200, 200, 200),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblChooseNote.Font = lblChooseNote.Font.WithSize(15);
            lblChooseNote.Text = Utils.TextBundle("choosemore1", "Items");
            noteView.AddSubview(lblChooseNote);
            #endregion

            #region NoteCollectionView
            var MenuflowLayout = new POS.MenuBarCollectionViewLayout();
            MenuflowLayout.SizeForItem += (collectionView, layout, indexPath) =>
            {
                NSString nSString = new NSString((MenuNoteCollectionView.DataSource as MenuNoteDataSource).GetItem(indexPath.Row));
                UIFont font = UIFont.SystemFontOfSize(13);
                CGSize cGSize = nSString.StringSize(font);
                return new CGSize(cGSize.Width + 40, 38);
            };


            MenuNoteCollectionView = new UICollectionView(frame: View.Frame, layout: MenuflowLayout);
            MenuNoteCollectionView.BackgroundColor = UIColor.White;
            MenuNoteCollectionView.ShowsHorizontalScrollIndicator = false;
            MenuNoteCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            MenuNoteCollectionView.RegisterClassForCell(cellType: typeof(POSMenuCollectionViewCell), reuseIdentifier: "menuNoteCell");
            MenuNote = await GetMenuNote();
            MenuNote.Add(new NoteCategory { MerchantID = DataCashingAll.MerchantId, SysNoteCategoryID = 0, Ordinary = null, Name = "None", DateCreated = DateTime.UtcNow, DateModified = DateTime.UtcNow, DataStatus = 'I', FWaitSending = 1, WaitSendingTime = DateTime.UtcNow });

            MenuNoteDataSource menuNoteData = new MenuNoteDataSource(MenuNote);
            MenuNoteCollectionView.DataSource = menuNoteData;
            POSMenuCollectionDelegate menuNoteCollectionDelegate = new POSMenuCollectionDelegate();
            menuNoteCollectionDelegate.OnItemSelected += async (indexPath) =>
            {
                Select = indexPath.Row;

                if ((int)MenuNote[(int)indexPath.Item].SysNoteCategoryID == 0)
                {
                    NoteList = await Notemanager.GetNoteOnlyNoneGroup((int)MainController.merchantlocal.MerchantID);
                }
                else
                {
                    NoteList = await Notemanager.GetNoteBYCategory((int)MainController.merchantlocal.MerchantID, (int)MenuNote[(int)indexPath.Item].SysNoteCategoryID);
                }
                // do somthing
                //var cell = NoteCollectionView.DequeueReusableCell("optionNoteViewCellList", indexPath) as OptionNoteCollectionViewCell;
                //NoteList = await Notemanager.GetNoteBYCategory((int)MainController.merchantlocal.MerchantID, (int)MenuNote[(int)indexPath.Item].SysNoteCategoryID);
                if (NoteList.Count == 0)
                {
                    Utils.SetConstant(NoteCollectionView.Constraints, NSLayoutAttribute.Height, 0);
                }
                else
                {
                    Utils.SetConstant(NoteCollectionView.Constraints, NSLayoutAttribute.Height, 45);
                }
                ((OptionNoteDataSource)NoteCollectionView.DataSource).ReloadData(NoteList);
                NoteCollectionView.ReloadData();

                //NSIndexPath nSIndexPath = NSIndexPath.FromItemSection(0, NoteList.Count-1);
                //var cell = NoteCollectionView.CellForItem(nSIndexPath) as OptionNoteCollectionViewCell;
                //UIView.Animate(0.7, () =>
                //{
                //    Utils.SetConstant(NoteCollectionView.Constraints, NSLayoutAttribute.Height, (int)cell?.Frame.Y + 50);
                //});
            };
            MenuNoteCollectionView.Delegate = menuNoteCollectionDelegate;

            TopAlignedCollectionViewFlowLayout NoteLayoutList = new TopAlignedCollectionViewFlowLayout();
            //NoteLayoutList.ItemSize = new CoreGraphics.CGSize(width: (int)View.Frame.Width/4, height: 40);
            NoteLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            NoteLayoutList.SectionInset = UIEdgeInsets.Zero;
            NoteLayoutList.MinimumLineSpacing = 1f;
            NoteLayoutList.MinimumInteritemSpacing = 5f;
            NoteLayoutList.EstimatedItemSize = new CoreGraphics.CGSize(View.Frame.Width - 50, 40);




            NoteCollectionView = new UICollectionView(frame: View.Frame, layout: NoteLayoutList);
            NoteList = await InitGetNoteByCat();
            NoteCollectionView.BackgroundColor = UIColor.White;
            NoteCollectionView.ShowsVerticalScrollIndicator = false;
            NoteCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;

            NoteCollectionView.RegisterClassForCell(cellType: typeof(OptionNoteCollectionViewCell), reuseIdentifier: "optionNoteViewCellList");
            OptionNoteDataSource NoteDataList = new OptionNoteDataSource(NoteList, NoteCollectionView.Frame);
            NoteCollectionView.DataSource = NoteDataList;
            OptionNoteCollectionDelegate NoteCollectionDelegate = new OptionNoteCollectionDelegate();
            NoteCollectionDelegate.OnItemSelected += (indexPath) =>
            {
                txtComment.Text = txtComment.Text + NoteList[(int)indexPath.Item].Message + " ";
            };
            NoteCollectionView.Delegate = NoteCollectionDelegate;
            #endregion

            #region CommentView
            CommentView = new UIView();
            CommentView.BackgroundColor = UIColor.White;
            CommentView.TranslatesAutoresizingMaskIntoConstraints = false;


            lblComment = new UILabel
            {
                TextColor = UIColor.FromRGB(247, 86, 0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblComment.Font = lblComment.Font.WithSize(15);
            lblComment.Text = Utils.TextBundle("comment", "Items");
            CommentView.AddSubview(lblComment);

            txtComment = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtComment.ReturnKeyType = UIReturnKeyType.Done;
            txtComment.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtComment.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("comment", "Items"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtComment.Font = txtComment.Font.WithSize(15);
            CommentView.AddSubview(txtComment);
            #endregion

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.White;
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;


            btnAddToCart = new UIButton();
            btnAddToCart.SetTitle(Utils.TextBundle("addcart", "Items"), UIControlState.Normal);
            btnAddToCart.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnAddToCart.Layer.CornerRadius = 5f;
            btnAddToCart.BackgroundColor = new UIColor(red: 51 / 255f, green: 170 / 255f, blue: 225 / 255f, alpha: 1);
            btnAddToCart.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddToCart.TouchUpInside += (sender, e) =>
            {
                // save update customer
                ClickAdd();
            };
            BottomView.AddSubview(btnAddToCart);
            #endregion

            _contentView.AddSubview(SizeView);
            _contentView.AddSubview(SizeCollectionView);
            _contentView.AddSubview(ToppingView);
            _contentView.AddSubview(MenuExtraTopppingCollectionView);
            _contentView.AddSubview(ExtratoppingCollectionView);
            _contentView.AddSubview(noteView);
            _contentView.AddSubview(MenuNoteCollectionView);
            _contentView.AddSubview(NoteCollectionView);
            _contentView.AddSubview(CommentView);



            _scrollView.AddSubview(_contentView);
            View.AddSubview(BottomView);
            View.AddSubview(_scrollView);
        }
        private async Task<List<Note>> InitGetNoteByCat()
        {
            try
            {
                var size = new List<Note>();

                if (MenuNote.Count > 0)
                {
                    size = await Notemanager.GetNoteBYCategory((int)MainController.merchantlocal.MerchantID, (int)MenuNote[0].SysNoteCategoryID);

                    return size;
                }
                else
                {
                    return new List<Note>();
                }

            }
            catch (Exception ex)
            {
                return new List<Note>();
                //throw;
            }

        }
        private async Task<List<Item>> InitGetTopping()
        {
            try
            {
                var size = new List<Item>();

                if (MenuTopping.Count > 0)
                {
                    size = await itemManager.GetToppingItemByCategory((int)MenuTopping[0].SysCategoryID);

                    return size;
                }
                else
                {
                    return new List<Item>();
                }

            }
            catch (Exception ex)
            {
                return new List<Item>();
                //throw;
            }

        }
        private async Task<List<NoteCategory>> GetMenuNote()
        {
            var Cate = new List<NoteCategory>();
            NoteCategoryManage Exmanager = new NoteCategoryManage();
            Cate = await Exmanager.GetNoteCategoryOption();
            //Cate = await Exmanager.GetAllNoteCategory();

            return Cate;
        }
        private async Task<List<Category>> GetMenuTopping()
        {
            var size = new List<Category>();
            CategoryManage Exmanager = new CategoryManage();
            var Cate = await Exmanager.GetCategoryOption();
            //var Cate = await Exmanager.GetAllCategory();

            return Cate;
        }
        private async Task<List<ItemExSize>> setSizeOptionBind()
        {
            var size = new List<ItemExSize>();
            ItemExSizeManage Exmanager = new ItemExSizeManage();
            size = await Exmanager.GetItemSize((int)MainController.merchantlocal.MerchantID, (int)item.tranDetailItem.SysItemID);
            //size.Add(new ItemExSize());


            return size;
        }
        void ClickAdd()
        {

            string size = null;
            decimal itemPrice = 0;
            TranDetailItemNew DetailItem = new TranDetailItemNew();
            DetailItem = new TranDetailItemNew();
            if (allExtra == null)
            {
                DetailItem.ItemPrice = item.tranDetailItem.ItemPrice;
                DetailItem.Price = item.tranDetailItem.Price;
            }
            else
            {
                if (allExtrachoose == null)
                {
                    //size = allExtrachoose.ExSizeName;
                    DetailItem.ItemPrice = item.tranDetailItem.ItemPrice;
                    DetailItem.Price = item.tranDetailItem.Price;
                }
                else if (allExtrachoose.ExSizeNo == 999)
                {
                    size = null;
                    DetailItem.ItemPrice = allExtrachoose.Price;
                    DetailItem.Price = allExtrachoose.Price;
                }
                else
                {
                    size = allExtrachoose.ExSizeName;
                    DetailItem.ItemPrice = allExtrachoose.Price;
                    DetailItem.Price = allExtrachoose.Price;
                }
            }

            //Note
            //foreach (var note in SelectNote)
            //{
            //    insertNote += note.Message + " ";
            //}


            DetailItem.SysItemID = item.tranDetailItem.SysItemID;
            DetailItem.MerchantID = DataCashingAll.MerchantId;
            DetailItem.SysBranchID = DataCashingAll.SysBranchId;
            DetailItem.TranNo = POSController.tranWithDetails.tran.TranNo;
            DetailItem.ItemName = item.tranDetailItem.ItemName;
            DetailItem.SaleItemType = item.tranDetailItem.SaleItemType;
            DetailItem.FProcess = 1;
            DetailItem.TaxType = item.tranDetailItem.TaxType;
            if (edit)
            {
                DetailItem.Quantity = item.tranDetailItem.Quantity;
                
                DetailItem.FmlDiscountRow = item.tranDetailItem.FmlDiscountRow;
                
            }
            else
            {
                DetailItem.Quantity = (decimal)POSController.Quantity;
                DetailItem.Discount = 0;
                DetailItem.FmlDiscountRow = null;
            }
           
            
            DetailItem.EstimateCost = item.tranDetailItem.EstimateCost;
            DetailItem.SizeName = size;
            DetailItem.Comments = txtComment.Text;
            DetailItem.DetailNo = item.tranDetailItem.DetailNo;


            List<TranDetailItemTopping> lstitemDetail = new List<TranDetailItemTopping>();
            //Topping
            //lstSelectTopping
            for (int i = 0; i < lstTranSelectTopping.Count; i++)
            {
                TranDetailItemTopping itemDetail = new TranDetailItemTopping()
                {
                    MerchantID = POSController.tranWithDetails.tran.MerchantID,
                    SysBranchID = POSController.tranWithDetails.tran.SysBranchID,
                    TranNo = POSController.tranWithDetails.tran.TranNo,
                    DetailNo = item.tranDetailItem.DetailNo,
                    ToppingNo = lstTranSelectTopping[i].ToppingNo,
                    ItemName = lstTranSelectTopping[i].ItemName,//toppping
                    SysItemID = lstTranSelectTopping[i].SysItemID,
                    UnitName = lstTranSelectTopping[i].UnitName,
                    
                    RegularSizeName = null,
                    Quantity = lstTranSelectTopping[i].Quantity,
                    ToppingPrice = lstTranSelectTopping[i].ToppingPrice,
                    EstimateCost = lstTranSelectTopping[i].EstimateCost,
                    Comments = lstTranSelectTopping[i].Comments
                };

                lstitemDetail.Add(itemDetail);
            }
            item.tranDetailItem = DetailItem;
            item.tranDetailItemToppings = lstitemDetail;

            if (edit)
            {
                POSController.tranWithDetails = BLTrans.EditToppingRow(POSController.tranWithDetails, item);
            }
            else
            {
                POSController.tranWithDetails = BLTrans.ChooseItemTran(POSController.tranWithDetails, item);
            }
            
            //DataCashing.ModifyTranOrder = true;
            POSController.tranWithDetails = BLTrans.Caltran(POSController.tranWithDetails);
            //if (FmlDiscountRow != null)
            //{
            //    DetailItem = BLTrans.CalDiscountDetailItem(DetailItem);
            //}
            POSController.Quantity = 1;
            //PosActivity.tranDetailItemWithTopping = detailItemWithTopping;
            lstTranSelectTopping = new List<TranDetailItemTopping>();
            this.NavigationController.PopViewController(false);
            //SelectSize = new ItemExSize();
            //SelectNote = new List<Note>();
            //ExSizeNo = 0;
            //sysitemIDToppping = 0;
            //NoteID = 0;
            //PosActivity.SetTranDetail(tranWithDetails);
            //PosActivity.pos.SelectItemtoCart(PositionClick);
            //OptionActivity.POSItem = null;
            //OptionActivity.ItemShopOption = 0;
            //OptionActivity.DataItem = new Item();
            //OptionActivity.flagLoadSize = false;

        }
        void setupAutoLayout()
        {
            //UIScrollView can be any size 
            _scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            _scrollView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            _scrollView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            _scrollView.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            //Inner UIView has to be attached to all UIScrollView constraints
            _contentView.TopAnchor.ConstraintEqualTo(_contentView.Superview.TopAnchor).Active = true;
            _contentView.RightAnchor.ConstraintEqualTo(_contentView.Superview.RightAnchor).Active = true;
            _contentView.LeftAnchor.ConstraintEqualTo(_contentView.Superview.LeftAnchor).Active = true;
            _contentView.BottomAnchor.ConstraintEqualTo(_contentView.Superview.BottomAnchor).Active = true;
            _contentView.WidthAnchor.ConstraintEqualTo(_contentView.Superview.WidthAnchor).Active = true;

            #region SizeView
            SizeView.TopAnchor.ConstraintEqualTo(_contentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            SizeView.LeftAnchor.ConstraintEqualTo(SizeView.Superview.LeftAnchor, 0).Active = true;
            SizeView.RightAnchor.ConstraintEqualTo(SizeView.Superview.RightAnchor, 0).Active = true;
            SizeView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            lblSize.LeftAnchor.ConstraintEqualTo(lblSize.Superview.LeftAnchor, 15).Active = true;
            lblSize.CenterYAnchor.ConstraintEqualTo(lblSize.Superview.CenterYAnchor).Active = true;
            lblSize.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblSize.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblSizeChooseOne.LeftAnchor.ConstraintEqualTo(lblSize.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblSizeChooseOne.CenterYAnchor.ConstraintEqualTo(lblSizeChooseOne.Superview.CenterYAnchor).Active = true;
            lblSizeChooseOne.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblSizeChooseOne.WidthAnchor.ConstraintEqualTo(100).Active = true;
            #endregion

            #region SizeCollectionView
            SizeCollectionView.TopAnchor.ConstraintEqualTo(SizeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            SizeCollectionView.LeftAnchor.ConstraintEqualTo(SizeCollectionView.Superview.LeftAnchor, 0).Active = true;
            SizeCollectionView.RightAnchor.ConstraintEqualTo(SizeCollectionView.Superview.RightAnchor, 0).Active = true;
            SizeCollectionView.HeightAnchor.ConstraintEqualTo(30 * allExtra.Count).Active = true;
            #endregion

            #region ToppingView
            ToppingView.TopAnchor.ConstraintEqualTo(SizeCollectionView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            ToppingView.LeftAnchor.ConstraintEqualTo(ToppingView.Superview.LeftAnchor, 0).Active = true;
            ToppingView.RightAnchor.ConstraintEqualTo(ToppingView.Superview.RightAnchor, 0).Active = true;
            ToppingView.HeightAnchor.ConstraintEqualTo(45).Active = true;
            ToppingView.WidthAnchor.ConstraintEqualTo(ToppingView.Superview.WidthAnchor).Active = true;
            //   ToppingView.BottomAnchor.ConstraintEqualTo(ToppingView.Superview.BottomAnchor, 0).Active = true;

            lblExtraTopping.LeftAnchor.ConstraintEqualTo(lblExtraTopping.Superview.LeftAnchor, 15).Active = true;
            lblExtraTopping.CenterYAnchor.ConstraintEqualTo(lblExtraTopping.Superview.CenterYAnchor).Active = true;
            lblExtraTopping.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblExtraTopping.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblChooseExtraTopping.LeftAnchor.ConstraintEqualTo(lblExtraTopping.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblChooseExtraTopping.CenterYAnchor.ConstraintEqualTo(lblChooseExtraTopping.Superview.CenterYAnchor).Active = true;
            lblChooseExtraTopping.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblChooseExtraTopping.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region Topping Collection
            MenuExtraTopppingCollectionView.TopAnchor.ConstraintEqualTo(ToppingView.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            MenuExtraTopppingCollectionView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            MenuExtraTopppingCollectionView.LeftAnchor.ConstraintEqualTo(MenuExtraTopppingCollectionView.Superview.LeftAnchor, 0).Active = true;
            MenuExtraTopppingCollectionView.RightAnchor.ConstraintEqualTo(MenuExtraTopppingCollectionView.Superview.RightAnchor, 0).Active = true;

            ExtratoppingCollectionView.TopAnchor.ConstraintEqualTo(MenuExtraTopppingCollectionView.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            ExtratoppingCollectionView.HeightAnchor.ConstraintEqualTo(40 * Topping.Count).Active = true;
            ExtratoppingCollectionView.LeftAnchor.ConstraintEqualTo(ExtratoppingCollectionView.Superview.LeftAnchor, 0).Active = true;
            ExtratoppingCollectionView.RightAnchor.ConstraintEqualTo(ExtratoppingCollectionView.Superview.RightAnchor, 0).Active = true;
            //ExtratoppingCollectionView.BackgroundColor = UIColor.Red;
            #endregion

            #region noteView
            noteView.TopAnchor.ConstraintEqualTo(ExtratoppingCollectionView.BottomAnchor, 5).Active = true;
            noteView.LeftAnchor.ConstraintEqualTo(ToppingView.Superview.LeftAnchor, 0).Active = true;
            noteView.RightAnchor.ConstraintEqualTo(ToppingView.Superview.RightAnchor, 0).Active = true;
            noteView.HeightAnchor.ConstraintEqualTo(45).Active = true;
            noteView.WidthAnchor.ConstraintEqualTo(ToppingView.Superview.WidthAnchor).Active = true;

            lblNote.LeftAnchor.ConstraintEqualTo(lblNote.Superview.LeftAnchor, 15).Active = true;
            lblNote.CenterYAnchor.ConstraintEqualTo(lblNote.Superview.CenterYAnchor).Active = true;
            lblNote.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblNote.WidthAnchor.ConstraintEqualTo(100).Active = true;

            lblChooseNote.LeftAnchor.ConstraintEqualTo(lblNote.SafeAreaLayoutGuide.RightAnchor, 15).Active = true;
            lblChooseNote.CenterYAnchor.ConstraintEqualTo(lblChooseNote.Superview.CenterYAnchor).Active = true;
            lblChooseNote.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblChooseNote.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region Note Collection
            MenuNoteCollectionView.TopAnchor.ConstraintEqualTo(noteView.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            MenuNoteCollectionView.HeightAnchor.ConstraintEqualTo(40).Active = true;
            MenuNoteCollectionView.LeftAnchor.ConstraintEqualTo(MenuNoteCollectionView.Superview.LeftAnchor, 0).Active = true;
            MenuNoteCollectionView.RightAnchor.ConstraintEqualTo(MenuNoteCollectionView.Superview.RightAnchor, 0).Active = true;

            NoteCollectionView.TopAnchor.ConstraintEqualTo(MenuNoteCollectionView.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            NoteCollectionView.HeightAnchor.ConstraintGreaterThanOrEqualTo(44).Active = true;
            NoteCollectionView.LeftAnchor.ConstraintEqualTo(NoteCollectionView.Superview.LeftAnchor, 5).Active = true;
            NoteCollectionView.RightAnchor.ConstraintEqualTo(NoteCollectionView.Superview.RightAnchor, -5).Active = true;
            //NoteCollectionView.BackgroundColor = UIColor.Red;

            NoteCollectionView.BottomAnchor.ConstraintEqualTo(CommentView.SafeAreaLayoutGuide.TopAnchor, -5).Active = true;

            #endregion

            #region CommentView
            //CommentView.TopAnchor.ConstraintEqualTo(NoteCollectionView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            CommentView.LeftAnchor.ConstraintEqualTo(CommentView.Superview.LeftAnchor, 0).Active = true;
            CommentView.RightAnchor.ConstraintEqualTo(CommentView.Superview.RightAnchor, 0).Active = true;
            CommentView.HeightAnchor.ConstraintEqualTo(80).Active = true;
            CommentView.WidthAnchor.ConstraintEqualTo(CommentView.Superview.WidthAnchor).Active = true;
            CommentView.BottomAnchor.ConstraintEqualTo(CommentView.Superview.BottomAnchor, 0).Active = true;

            lblComment.LeftAnchor.ConstraintEqualTo(lblComment.Superview.LeftAnchor, 15).Active = true;
            lblComment.TopAnchor.ConstraintEqualTo(lblComment.Superview.TopAnchor, 15).Active = true;
            lblComment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            lblComment.WidthAnchor.ConstraintEqualTo(100).Active = true;

            txtComment.LeftAnchor.ConstraintEqualTo(txtComment.Superview.LeftAnchor, 15).Active = true;
            txtComment.RightAnchor.ConstraintEqualTo(txtComment.Superview.RightAnchor, -15).Active = true;
            txtComment.TopAnchor.ConstraintEqualTo(lblComment.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            //txtComment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            txtComment.WidthAnchor.ConstraintEqualTo(150).Active = true;
            #endregion

            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddToCart.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddToCart.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAddToCart.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddToCart.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion
        }
    }


    public class TopAlignedCollectionViewFlowLayout : UICollectionViewFlowLayout
    {
        //public override UIEdgeInsets SectionInset 
        //{
        //    get => base.SectionInset; 
        //    set => base.SectionInset = new UIEdgeInsets(0, 100, 0, 0);
        //}
        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CoreGraphics.CGRect rect)
        {

            var attributes = base.LayoutAttributesForElementsInRect(rect);

            if (attributes.Length == 0)
            {
                return null;
            }
            //Add these lines to change the first cell's position of the collection view.
            var firstCellFrame = attributes[0].Frame;
            firstCellFrame.X = 0;
            attributes[0].Frame = firstCellFrame;


            for (var i = 1; i < attributes.Length; ++i)
            {
                var currentLayoutAttributes = attributes[i];
                var previousLayoutAttributes = attributes[i - 1];
                var maximumSpacing = MinimumInteritemSpacing;
                var previousLayoutEndPoint = previousLayoutAttributes.Frame.Right;
                if (previousLayoutEndPoint + maximumSpacing + currentLayoutAttributes.Frame.Size.Width >= CollectionViewContentSize.Width)
                {
                    continue;
                }
                var frame = currentLayoutAttributes.Frame;
                frame.X = previousLayoutEndPoint + maximumSpacing;
                currentLayoutAttributes.Frame = frame;
            }
            return attributes;
        }
    }
}
