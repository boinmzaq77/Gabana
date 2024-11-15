using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.iOS.ITEMS;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;

namespace Gabana.iOS
{
    public class ItemAddDatailViewCell : UICollectionViewCell
    {
       // AddItemController main = AddItemController.ToModal;
        UIView toppingView;
        UILabel lblTopping;
        UITextField txtTopping;
        UIImageView btnSelectTopping;
        public static long addColor;
        public static long CatID;
        UIAlertController VatTypeMenuSheet;
        public List<Category> CatList;
        UIButton btnToggleDetail, btnSelectVatType;
        UIButton btnSelectCategory;
        UIView ItemCodeView, CategoryView, VatView, CostView, ExtraSizeView, DetailClickView;
        UILabel lblDetail, lblCategory, lblVat, lblBtnText;
        UIView btnAddMore;
        public static UILabel lblVatMode;
        public static UITextField lblSelectedCategory, txtNote;
        public static UITextField txtItemCode, txtItemCost;
        UILabel lblItemCode, lblItemCost, lblExtraSize, lblNote;
        UISwitch switchExtraSize;
        UIImagePickerController imagePicker;
        private static byte[] picture;
        UIAlertController selectPhotoMenuSheet;
        UIView imageView, setColorView, setItemNameView, SetItemPriceView;
        UIView itemCardFooter, NoteView;
        UICollectionView ExtraSizeCollectionView;
        UIImageView itemCardView, btnSelectNote,plusImg;
        UILabel lblItemName, lblItemPrice;
        public List<Gabana.ORM.MerchantDB.ItemExSize> extraList = new List<Gabana.ORM.MerchantDB.ItemExSize>();
        public static int extraListCount = 0;
        UIToolbar toolbar1,toolbar2, toolbar3;
        public static UILabel lblItemCardName, lblItemCardShortName, LblItemCardPrice;
        public static UITextField txtItemName, txtItemPrice;
        ItemExSizeManage extra = new ItemExSizeManage();
        public List<Item> NoteList;
        UIButton btnColor1, btnColor2, btnColor3, btnColor4, btnColor5, btnColor6, btnColor7, btnColor8, btnColor9, btnChangeItemPhoto;
        public ItemAddDatailViewCell(IntPtr handle) : base(handle)
        {
            ContentView.BackgroundColor = UIColor.FromRGB(248,248,248);
           

            #region SetItemImage
            imageView = new UIView();
            imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            imageView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(imageView);

            itemCardView = new UIImageView();
            itemCardView.TranslatesAutoresizingMaskIntoConstraints = false;
            itemCardView.Layer.CornerRadius = 5;
            itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            imageView.AddSubview(itemCardView);

            itemCardFooter = new UIView();
            itemCardFooter.TranslatesAutoresizingMaskIntoConstraints = false;
            itemCardFooter.BackgroundColor = UIColor.Black;
            itemCardFooter.Layer.Opacity = 0.2f;
            itemCardView.AddSubview(itemCardFooter);

            lblItemCardName = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCardName.Font = lblItemCardName.Font.WithSize(13);
            lblItemCardName.Text = "Item Name";
            itemCardView.AddSubview(lblItemCardName);

            lblItemCardShortName = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCardShortName.Font = lblItemCardShortName.Font.WithSize(24);
            lblItemCardShortName.Text = "Item";
            itemCardView.AddSubview(lblItemCardShortName);

            LblItemCardPrice = new UILabel
            {
                TextColor = UIColor.White,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            LblItemCardPrice.Font = LblItemCardPrice.Font.WithSize(13);
            LblItemCardPrice.Text = "฿ xx.xx";
            itemCardView.AddSubview(LblItemCardPrice);

            #endregion

            #region SetColor
            setColorView = new UIView();
            setColorView.TranslatesAutoresizingMaskIntoConstraints = false;
            setColorView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(setColorView);
            #endregion

            #region SetItemName
            setItemNameView = new UIView();
            setItemNameView.TranslatesAutoresizingMaskIntoConstraints = false;
            setItemNameView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(setItemNameView);

            lblItemName = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemName.Font = lblItemName.Font.WithSize(15);
            lblItemName.Text = "Item Name";
            setItemNameView.AddSubview(lblItemName);

            txtItemName = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(51, 170, 225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtItemName.ReturnKeyType = UIReturnKeyType.Next;
            txtItemName.ShouldReturn = (tf) =>
            {
                txtItemPrice.BecomeFirstResponder();
                return true;
            };
            txtItemName.AttributedPlaceholder = new NSAttributedString("Item Name", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtItemName.Font = txtItemName.Font.WithSize(15);
            txtItemName.EditingChanged += (object sender, EventArgs e) =>
            {

                lblItemCardName.Text = txtItemName.Text;
                if (txtItemName.Text.Length > 5)
                {
                    lblItemCardShortName.Text = txtItemName.Text.Substring(0, 5);
                }
                else
                {
                    lblItemCardShortName.Text = txtItemName.Text;
                }

            };
            setItemNameView.AddSubview(txtItemName);
            #endregion

            #region SetItemPrice
            SetItemPriceView = new UIView();
            SetItemPriceView.TranslatesAutoresizingMaskIntoConstraints = false;
            SetItemPriceView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(SetItemPriceView);

            lblItemPrice = new UILabel
            {
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemPrice.Font = lblItemPrice.Font.WithSize(15);
            lblItemPrice.Text = "Price";
            SetItemPriceView.AddSubview(lblItemPrice);

            UIToolbar NumpadToolbar = new UIToolbar(new RectangleF(0.0f, 0.0f,(float)ContentView.Frame.Width, 44.0f));
            NumpadToolbar.Translucent = true;
            NumpadToolbar.Items = new UIBarButtonItem[]{
            new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
            new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate {
                 ContentView.EndEditing(true);
            })
            };
            NumpadToolbar.SizeToFit();

            txtItemPrice = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(51,170,225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtItemPrice.InputAccessoryView = NumpadToolbar;
            txtItemPrice.AttributedPlaceholder = new NSAttributedString("฿ 0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtItemPrice.KeyboardType = UIKeyboardType.DecimalPad;
            txtItemPrice.Font = txtItemPrice.Font.WithSize(15);
            txtItemPrice.EditingChanged += (object sender, EventArgs e) =>
            {
                LblItemCardPrice.Text = txtItemPrice.Text;
            };
            SetItemPriceView.AddSubview(txtItemPrice);
            #endregion

            #region DetailItem
            DetailClickView = new UIView();
            DetailClickView.TranslatesAutoresizingMaskIntoConstraints = false;
            DetailClickView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(DetailClickView);

            lblDetail = new UILabel
            {
                TextColor = UIColor.FromRGB(247,86,0),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblDetail.Font = lblDetail.Font.WithSize(15);
            lblDetail.Text = "Details";
            DetailClickView.AddSubview(lblDetail);

            btnToggleDetail = new UIButton();
            btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
            btnToggleDetail.TranslatesAutoresizingMaskIntoConstraints = false;
            btnToggleDetail.TouchUpInside += (sender, e) =>
            {
                //Detail Not Show
                if (AddItemController.flagDetail == 0)
                {
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                    setDetailShow(true);
                    AddItemController.flagDetail = 1;
                }
                else
                {
                    btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                    setDetailShow(false);
                    AddItemController.flagDetail = 0;
                }
            };
            DetailClickView.AddSubview(btnToggleDetail);

            #region itemcodeField
            ItemCodeView = new UIView();
            ItemCodeView.TranslatesAutoresizingMaskIntoConstraints = false;
            ItemCodeView.BackgroundColor = UIColor.White;
            ItemCodeView.Hidden = true;
            ContentView.AddSubview(ItemCodeView);

            lblItemCode = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCode.Font = lblItemCode.Font.WithSize(15);
            lblItemCode.Text = "Item Code";
            ItemCodeView.AddSubview(lblItemCode);

            txtItemCode = new UITextField
            {
                TextColor = UIColor.FromRGB(51,170,225),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtItemCode.ReturnKeyType = UIReturnKeyType.Done;
            txtItemCode.ShouldReturn = (tf) =>
            {
                ContentView.EndEditing(true);
                return true;
            };
            txtItemCode.AttributedPlaceholder = new NSAttributedString("Code", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168, 211, 245) });
            txtItemCode.Font = txtItemCode.Font.WithSize(15);
            ItemCodeView.AddSubview(txtItemCode);
            #endregion

            #region CategoryField
            CategoryView = new UIView();
            CategoryView.TranslatesAutoresizingMaskIntoConstraints = false;
            CategoryView.BackgroundColor = UIColor.White;
            CategoryView.Hidden = true;
            ContentView.AddSubview(CategoryView);

            lblCategory = new UILabel
            {
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCategory.Font = lblCategory.Font.WithSize(15);
            lblCategory.Text = "Category";
            CategoryView.AddSubview(lblCategory);

            lblSelectedCategory = new UITextField
            {
                Placeholder = "None",
                TextColor =  UIColor.FromRGB(172,172,172),
                BorderStyle = UITextBorderStyle.None,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblSelectedCategory.Font = lblSelectedCategory.Font.WithSize(15);
            CategoryView.AddSubview(lblSelectedCategory);

            btnSelectCategory = new UIButton();
            btnSelectCategory.SetBackgroundImage(UIImage.FromBundle("Next"), UIControlState.Normal);
            btnSelectCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectCategory.TouchUpInside += (sender, e) => {
                lblSelectedCategory.BecomeFirstResponder();
            };
            CategoryView.AddSubview(btnSelectCategory);
            #endregion

            #region NoteField
            NoteView = new UIView();
            NoteView.TranslatesAutoresizingMaskIntoConstraints = false;
            NoteView.BackgroundColor = UIColor.White;
            NoteView.Hidden = true;
            ContentView.AddSubview(NoteView);

            lblNote = new UILabel
            {
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblNote.Font = lblNote.Font.WithSize(15);
            lblNote.Text = "Note";
            NoteView.AddSubview(lblNote);

            txtNote = new UITextField
            {
                Placeholder = "Note",
                TextColor = UIColor.FromRGB(51, 170, 225),
                BorderStyle = UITextBorderStyle.None,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txtNote.Font = txtNote.Font.WithSize(15);
            NoteView.AddSubview(txtNote);

            btnSelectNote = new UIImageView();
            btnSelectNote.Image = UIImage.FromBundle("Next");
            btnSelectNote.TranslatesAutoresizingMaskIntoConstraints = false;
            NoteView.AddSubview(btnSelectNote);
            #endregion

            #region ToppingField
            toppingView = new UIView();
            toppingView.TranslatesAutoresizingMaskIntoConstraints = false;
            toppingView.BackgroundColor = UIColor.White;
            toppingView.Hidden = true;
            ContentView.AddSubview(toppingView);

            lblTopping = new UILabel
            {
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblTopping.Font = lblTopping.Font.WithSize(15);
            lblTopping.Text = "Topping";
            toppingView.AddSubview(lblTopping);

            txtTopping = new UITextField
            {
                Placeholder = "Topping",
                TextColor = UIColor.FromRGB(51, 170, 225),
                BorderStyle = UITextBorderStyle.None,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            txtTopping.Font = txtTopping.Font.WithSize(15);
            toppingView.AddSubview(txtTopping);

            btnSelectTopping = new UIImageView();
            btnSelectTopping.Image = UIImage.FromBundle("Next");
            btnSelectTopping.TranslatesAutoresizingMaskIntoConstraints = false;
            toppingView.AddSubview(btnSelectTopping);
            #endregion

            #region vatField
            VatView = new UIView();
            VatView.TranslatesAutoresizingMaskIntoConstraints = false;
            VatView.BackgroundColor = UIColor.White;
            VatView.Hidden = true;
            ContentView.AddSubview(VatView);

            lblVat = new UILabel
            {
                TextColor = UIColor.FromRGB(64,64,64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblVat.Font = lblVat.Font.WithSize(15);
            lblVat.Text = "Vat";
            VatView.AddSubview(lblVat);

            lblVatMode = new UILabel
            {
                TextColor =  UIColor.FromRGB(162,162,162),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblVatMode.Font = lblVatMode.Font.WithSize(15);
            lblVatMode.Text = "Include Vat";
            VatView.AddSubview(lblVatMode);


            btnSelectVatType = new UIButton();
            btnSelectVatType.SetBackgroundImage(UIImage.FromBundle("Next"), UIControlState.Normal);
            btnSelectVatType.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectVatType.TouchUpInside += (sender, e) => {
                // select type
                #region DiscountTypeActionSheet

                VatTypeMenuSheet = UIAlertController.Create("Vat", null, UIAlertControllerStyle.ActionSheet);
                VatTypeMenuSheet.AddAction(UIAlertAction.Create("Include Vat", UIAlertActionStyle.Default,
                                                        Action => lblVatMode.Text = "Include Vat"));
                VatTypeMenuSheet.AddAction(UIAlertAction.Create("None Vat", UIAlertActionStyle.Default,
                                                        Action => lblVatMode.Text = "None Vat"));
                VatTypeMenuSheet.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, Action => Console.WriteLine("Cancel clicked")));

                // Show the alert
                //main.PresentViewController(VatTypeMenuSheet, true, null);
              //  main.PresentModalViewController(VatTypeMenuSheet, true);
                #endregion
            };
            VatView.AddSubview(btnSelectVatType);
            #endregion

            #region costField
            CostView = new UIView();
            CostView.TranslatesAutoresizingMaskIntoConstraints = false;
            CostView.BackgroundColor = UIColor.White;
            CostView.Hidden = true;
            ContentView.AddSubview(CostView);

            lblItemCost = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblItemCost.Font = lblItemCost.Font.WithSize(15);
            lblItemCost.Text = "Cost";
            CostView.AddSubview(lblItemCost);


            txtItemCost = new UITextField
            {
                TextColor =  UIColor.FromRGB(51,170,225),
            };
            txtItemCost.ReturnKeyType = UIReturnKeyType.Done;
            txtItemCost.ShouldReturn = (tf) =>
            {
                ContentView.EndEditing(true);
                return true;
            };
            txtItemCost.InputAccessoryView = NumpadToolbar;
            txtItemCost.AttributedPlaceholder = new NSAttributedString("0.00", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(168,211,245) });
            txtItemCost.TranslatesAutoresizingMaskIntoConstraints = false;
            txtItemCost.Font = txtItemCost.Font.WithSize(15);
            txtItemCost.KeyboardType = UIKeyboardType.DecimalPad;
            CostView.AddSubview(txtItemCost);

            #endregion


            #endregion

            #region extraSizeField
            #region ExtraSetView
                getExtraSize();
                extraListCount = this.extraList.Count;
                if (this.extraList.Count == 0)
                {
                    this.extraList.Add(new ItemExSize());
                }
            UICollectionViewFlowLayout itemflowLayoutListExtra = new UICollectionViewFlowLayout();
            itemflowLayoutListExtra.ItemSize = new CoreGraphics.CGSize(width: (ContentView.Frame.Width), height:210* this.extraList.Count);
            itemflowLayoutListExtra.ScrollDirection = UICollectionViewScrollDirection.Vertical;

            ExtraSizeCollectionView = new UICollectionView(frame: ContentView.Frame, layout: itemflowLayoutListExtra);
            ExtraSizeCollectionView.BackgroundColor = UIColor.FromRGB(248,248,248);
          //  ExtraSizeCollectionView.BackgroundColor = UIColor.FromRGB(0, 248, 248);
            ExtraSizeCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            ExtraSizeCollectionView.Hidden = true;
            ExtraSizeCollectionView.ScrollEnabled = false;
            ExtraSizeCollectionView.RegisterClassForCell(cellType: typeof(ExtraSizeCollectionViewCell), reuseIdentifier: "extraSizeCollectionViewCell");
            ItemExtraSizeDetailDataSource itemExtraDetailDataSource = new ItemExtraSizeDetailDataSource(this.extraList);
            itemExtraDetailDataSource.OnExtraSizeDeleteIndex += (indexPath) =>
            {
                var x = (int)(indexPath).Item;
            };
            ExtraSizeCollectionView.DataSource = itemExtraDetailDataSource;
            ContentView.AddSubview(ExtraSizeCollectionView);

            btnAddMore = new UIView();
            btnAddMore.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddMore.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnAddMore.Layer.CornerRadius = 5;
            btnAddMore.ClipsToBounds = true;
            btnAddMore.Hidden = true;
            ContentView.AddSubview(btnAddMore);

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
            #endregion

            ExtraSizeView = new UIView();
            ExtraSizeView.TranslatesAutoresizingMaskIntoConstraints = false;
            ExtraSizeView.Hidden = true;
            ExtraSizeView.BackgroundColor = UIColor.White;
            ContentView.AddSubview(ExtraSizeView);

            lblExtraSize = new UILabel
            {
                TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblExtraSize.Font = lblExtraSize.Font.WithSize(15);
            lblExtraSize.Text = "Extra Size";
            ExtraSizeView.AddSubview(lblExtraSize);

            switchExtraSize = new UISwitch();
            switchExtraSize.TranslatesAutoresizingMaskIntoConstraints = false;
            switchExtraSize.SetState(switchExtraSize.On, false);
            switchExtraSize.ValueChanged += (sender, e) =>
            {
                if (switchExtraSize.On)
                {
                    //open                   
                    ExtraSizeCollectionView.Hidden = false;
                    btnAddMore.Hidden = false;
                    if (extraListCount == 0)
                    {
                        AddItemController.ChangeDetailTall(extraListCount +1);
                    }
                    else
                    {
                        AddItemController.ChangeDetailTall(extraListCount);
                    }
                }
                else if (!switchExtraSize.On)
                {
                    //close
                    ExtraSizeCollectionView.Hidden = true;
                    btnAddMore.Hidden = true;
                }
            };
            ExtraSizeView.AddSubview(switchExtraSize);

            #endregion

            setColorButton();
            setupAutoLayout();
            setupListView();
            getCategoryAsync();
         //   getNoteAsync();
            SetupPicker();
        }
        public async System.Threading.Tasks.Task getCategoryAsync()
        {
            try
            {
                CategoryManage CatagoryManager = new CategoryManage();
                this.CatList = await CatagoryManager.GetAllCategory();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public async System.Threading.Tasks.Task getNoteAsync()
        {
            try
            {
                ItemManage itemManager = new ItemManage();
                // this.NoteList = await itemManager.Getnote;
                this.NoteList = null;
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public class PickerModelCategory : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;

            private readonly List<Category> values;
            public PickerModelCategory(List<Category> listCategory)
            {
                this.values = listCategory;
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
                return values[Convert.ToInt32(row)].Name;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs { SelectedValue = values[Convert.ToInt32(row)].Name,
                    ID = (int)values[Convert.ToInt32(row)].SysCategoryID});
                }
            }
            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        public class PickerModelNote : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;

            private readonly List<Item> values;
            public PickerModelNote(List<Item> Note)
            {
                this.values = Note;
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
                return values[Convert.ToInt32(row)].ItemName;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].ItemName,
                        ID = (int)values[Convert.ToInt32(row)].SysItemID
                    });
                }
            }
            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }

        public class PickerModelTopping : UIPickerViewModel
        {
            public class PickerChangedEventArgs : EventArgs
            {
                public string SelectedValue { get; set; }
                public int ID { get; set; }
            }

            public event EventHandler<PickerChangedEventArgs> PickerChanged;

            private UILabel personLabel;

            private readonly List<Item> values;
            public PickerModelTopping(List<Item> Note)
            {
                this.values = Note;
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
                return values[Convert.ToInt32(row)].ItemName;
            }

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (this.PickerChanged != null)
                {
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].ItemName,
                        ID = (int)values[Convert.ToInt32(row)].SysItemID
                    });
                }
            }
            public override nfloat GetRowHeight(UIPickerView picker, nint component)
            {
                return 40f;
            }


        }
        private void SetupPicker()
        {
            // Setup the picker and model
            var flexible = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace, target: this, action: null);
            var doneButton1 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { ContentView.EndEditing(true); });

            #region Category Picker
            PickerModelCategory model1 = new PickerModelCategory(this.CatList);
            model1.PickerChanged += (sender, e) => {
                lblSelectedCategory.Text = e.SelectedValue;
                CatID = e.ID;
            };
            toolbar1 = new UIToolbar();
            toolbar1.Translucent = true;
            toolbar1.SizeToFit();
           

            toolbar1.SetItems(new UIBarButtonItem[] { flexible, doneButton1 }, true);

            lblSelectedCategory.InputView = new UIPickerView() { Model = model1, ShowSelectionIndicator = true };
            lblSelectedCategory.InputAccessoryView = toolbar1;
            #endregion
        }
        [Export("Addmore:")]
        public void Addmore(UIGestureRecognizer sender)
        {
            if(extraList.Count < 6)
            {
                extraList.Add(new ItemExSize());
                AddItemController.ChangeDetailTall(extraList.Count);
               // ((ItemExtraSizeDetailDataSource)ExtraSizeCollectionView.DataSource).ReloadData(this.extraList);
                ExtraSizeCollectionView.ReloadData();
            }
            
        }
        public async void getExtraSize()
        {
            this.extraList = await extra.GetItemSize((int)MainController.merchantlocal.MerchantID,(int)AddItemController.sysItemIDEdit);
        }
        void setDetailShow(bool x)
        {
            if (x) // show
            {
                btnToggleDetail.SetImage(UIImage.FromBundle("DetailShow"), UIControlState.Normal);
                ItemCodeView.Hidden = false;
                CategoryView.Hidden = false;
                VatView.Hidden = false;
                CostView.Hidden = false;
                NoteView.Hidden = false;
                toppingView.Hidden = false;
                ExtraSizeView.Hidden = false;
                
            }
            else // close
            {
                btnToggleDetail.SetImage(UIImage.FromBundle("DetailNotShow"), UIControlState.Normal);
                ItemCodeView.Hidden = true;
                CategoryView.Hidden = true;
                VatView.Hidden = true;
                NoteView.Hidden = true;
                toppingView.Hidden = true;
                CostView.Hidden = true;
                ExtraSizeView.Hidden = true;
            }
        }
        void setColorButton()
        {
            setColorView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            setColorView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            setColorView.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            setColorView.HeightAnchor.ConstraintEqualTo(140).Active = true;
            //btnColor1, btnColor2, btnColor3, btnColor4, btnColor5, btnColor6, btnColor7, btnColor8, btnColor9, btnChangeItemPhoto;
            #region Buttom Blue
            btnColor1 = new UIButton();
            btnColor1.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnColor1.Layer.CornerRadius = 3;
            btnColor1.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor1.TouchUpInside += (sender, e) =>
            {
                //Blue
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                addColor = int.Parse("0095DA", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor1);

            btnColor1.TopAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.TopAnchor, 25).Active = true;
            btnColor1.LeftAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            btnColor1.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor1.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Yellow
            btnColor2 = new UIButton();
            btnColor2.BackgroundColor = UIColor.FromRGB(248, 151, 29);
            btnColor2.Layer.CornerRadius = 3;
            btnColor2.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor2.TouchUpInside += (sender, e) =>
            {
                //Yellow
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(248, 151, 29);
                addColor = int.Parse("F8971D", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor2);

            btnColor2.TopAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.TopAnchor, 25).Active = true;
            btnColor2.LeftAnchor.ConstraintEqualTo(btnColor1.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor2.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor2.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Red
            btnColor3 = new UIButton();
            btnColor3.BackgroundColor = UIColor.FromRGB(227, 45, 73);
            btnColor3.Layer.CornerRadius = 3;
            btnColor3.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor3.TouchUpInside += (sender, e) =>
            {
                //Red
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(227, 45, 73);
                addColor = int.Parse("E32D49", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor3);

            btnColor3.TopAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.TopAnchor, 25).Active = true;
            btnColor3.LeftAnchor.ConstraintEqualTo(btnColor2.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor3.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor3.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Green
            btnColor4 = new UIButton();
            btnColor4.BackgroundColor = UIColor.FromRGB(55, 172, 82);
            btnColor4.Layer.CornerRadius = 3;
            btnColor4.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor4.TouchUpInside += (sender, e) =>
            {
                //Green
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(55, 172, 82);
                addColor = int.Parse("37AA52", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor4);

            btnColor4.TopAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.TopAnchor, 25).Active = true;
            btnColor4.LeftAnchor.ConstraintEqualTo(btnColor3.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor4.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor4.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Orange
            btnColor5 = new UIButton();
            btnColor5.BackgroundColor = UIColor.FromRGB(247, 86, 0);
            btnColor5.Layer.CornerRadius = 3;
            btnColor5.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor5.TouchUpInside += (sender, e) =>
            {
                //Orange
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(247, 86, 0);
                addColor = int.Parse("F75600", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor5);

            btnColor5.TopAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.TopAnchor, 25).Active = true;
            btnColor5.LeftAnchor.ConstraintEqualTo(btnColor4.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor5.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor5.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion

            #region Buttom Purple
            btnColor6 = new UIButton();
            btnColor6.BackgroundColor = UIColor.FromRGB(63, 81, 181);
            btnColor6.Layer.CornerRadius = 3;
            btnColor6.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor6.TouchUpInside += (sender, e) =>
            {
                //Purple
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(63, 81, 181);
                addColor = int.Parse("3F51B5", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor6);

            btnColor6.TopAnchor.ConstraintEqualTo(btnColor1.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor6.LeftAnchor.ConstraintEqualTo(setColorView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            btnColor6.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor6.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom DarkGreen
            btnColor7 = new UIButton();
            btnColor7.BackgroundColor = UIColor.FromRGB(0, 121, 167);
            btnColor7.Layer.CornerRadius = 3;
            btnColor7.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor7.TouchUpInside += (sender, e) =>
            {
                //Dark Green
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(0, 121, 167);
                addColor = int.Parse("00796B", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor7);

            btnColor7.TopAnchor.ConstraintEqualTo(btnColor2.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor7.LeftAnchor.ConstraintEqualTo(btnColor6.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor7.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor7.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom LightGreen
            btnColor8 = new UIButton();
            btnColor8.BackgroundColor = UIColor.FromRGB(139, 195, 74);
            btnColor8.Layer.CornerRadius = 3;
            btnColor8.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor8.TouchUpInside += (sender, e) =>
            {
                //LightGreen
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(139, 195, 74);
                addColor = int.Parse("8BC34A", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor8);

            btnColor8.TopAnchor.ConstraintEqualTo(btnColor3.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor8.LeftAnchor.ConstraintEqualTo(btnColor7.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor8.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor8.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom Pink
            btnColor9 = new UIButton();
            btnColor9.BackgroundColor = UIColor.FromRGB(221, 82, 126);
            btnColor9.Layer.CornerRadius = 3;
            btnColor9.TranslatesAutoresizingMaskIntoConstraints = false;
            btnColor9.TouchUpInside += (sender, e) =>
            {
                //PINK
                itemCardView.Image = null;
                itemCardView.BackgroundColor = UIColor.FromRGB(221, 82, 126);
                addColor = int.Parse("DD527E", System.Globalization.NumberStyles.HexNumber);
            };
            ContentView.AddSubview(btnColor9);

            btnColor9.TopAnchor.ConstraintEqualTo(btnColor4.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnColor9.LeftAnchor.ConstraintEqualTo(btnColor8.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnColor9.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnColor9.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
            #region Buttom ChangePIC
            btnChangeItemPhoto = new UIButton();
            btnChangeItemPhoto.SetImage(UIImage.FromBundle("Album"), UIControlState.Normal);
            btnChangeItemPhoto.Layer.CornerRadius = 3;
            btnChangeItemPhoto.TranslatesAutoresizingMaskIntoConstraints = false;
            btnChangeItemPhoto.TouchUpInside += (sender, e) =>
            {
                //btnChangeItemPhoto
                addColor = 0;
                #region PhotoEditActionSheet

                selectPhotoMenuSheet = UIAlertController.Create("Add Logo", null, UIAlertControllerStyle.ActionSheet);
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Take a picture", UIAlertActionStyle.Default,
                                                alert => Pic("Take")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Choose your picture", UIAlertActionStyle.Default,
                                                alert => Pic("Choose")));
                selectPhotoMenuSheet.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel clicked")));

                // Show the alert
                //main.PresentModalViewController(selectPhotoMenuSheet, true);
                #endregion
            };
            ContentView.AddSubview(btnChangeItemPhoto);

            btnChangeItemPhoto.TopAnchor.ConstraintEqualTo(btnColor5.SafeAreaLayoutGuide.BottomAnchor, 10).Active = true;
            btnChangeItemPhoto.LeftAnchor.ConstraintEqualTo(btnColor9.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnChangeItemPhoto.HeightAnchor.ConstraintEqualTo(32).Active = true;
            btnChangeItemPhoto.WidthAnchor.ConstraintEqualTo(32).Active = true;
            #endregion
        }
        private void Pic(string v)
        {
            imagePicker = new UIImagePickerController();
            if (v == "Take")
            {
                imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
            }
            else
            {
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            }

            imagePicker.AllowsEditing = true;
            //imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;

            imagePicker.Canceled += Handle_Canceled;
            imagePicker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            //main.PresentModalViewController(imagePicker, true);
        }
        private void Handle_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissModalViewController(true);
        }
        protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    Console.WriteLine("Image selected");
                    isImage = true;
                    break;
                case "public.video":
                    Console.WriteLine("Video selected");
                    break;
            }

            // get common info (shared between images and video)
            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine("Url:" + referenceURL.ToString());

            // if it was an image, get the other image info
            if (isImage)
            {
                var x = e.Info[UIImagePickerController.OriginalImage];

                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {

                    originalImage.Scale(new CoreGraphics.CGSize(200, 200));
                    nfloat quality = (nfloat)0.7;
                    // do something with the image
                    itemCardView.Image = originalImage; // display
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = originalImage.AsJPEG(xx).AsStream();
                }

                UIImage editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                if (editedImage != null)
                {
                    // do something with the image
                    Console.WriteLine("got the edited image");
                    nfloat quality = (nfloat)0.7;
                    itemCardView.Image = editedImage;
                    picture = ReadFully(originalImage.AsJPEG(quality).AsStream());
                    //picture = imageprofile.Image.AsJPEG(quality).AsStream();

                }

            }
            imagePicker.DismissModalViewController(true);
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
        void setupAutoLayout()
        {
            #region ImageCardView
            imageView.TopAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            imageView.WidthAnchor.ConstraintEqualTo(150).Active = true;
            imageView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(140).Active = true;

            itemCardView.TopAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.TopAnchor, 20).Active = true;
            itemCardView.BottomAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.BottomAnchor, -20).Active = true;
            itemCardView.LeftAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            itemCardView.RightAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.RightAnchor, -20).Active = true;

            lblItemCardName.TopAnchor.ConstraintEqualTo(itemCardFooter.SafeAreaLayoutGuide.TopAnchor, 3).Active = true;
            lblItemCardName.WidthAnchor.ConstraintEqualTo(150).Active = true;
            lblItemCardName.LeftAnchor.ConstraintEqualTo(itemCardFooter.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            lblItemCardName.HeightAnchor.ConstraintEqualTo(15).Active = true;

            LblItemCardPrice.TopAnchor.ConstraintEqualTo(lblItemCardName.SafeAreaLayoutGuide.BottomAnchor, 3).Active = true;
            LblItemCardPrice.WidthAnchor.ConstraintEqualTo(150).Active = true;
            LblItemCardPrice.LeftAnchor.ConstraintEqualTo(itemCardFooter.SafeAreaLayoutGuide.LeftAnchor, 5).Active = true;
            LblItemCardPrice.HeightAnchor.ConstraintEqualTo(15).Active = true;

            lblItemCardShortName.CenterXAnchor.ConstraintEqualTo(itemCardView.SafeAreaLayoutGuide.CenterXAnchor).Active = true;
            lblItemCardShortName.TopAnchor.ConstraintEqualTo(itemCardView.SafeAreaLayoutGuide.TopAnchor, 15).Active = true;
            lblItemCardShortName.HeightAnchor.ConstraintEqualTo(29).Active = true;

            itemCardFooter.HeightAnchor.ConstraintEqualTo(40).Active = true;
            itemCardFooter.TopAnchor.ConstraintEqualTo(lblItemCardShortName.SafeAreaLayoutGuide.BottomAnchor, 26).Active = true;
            itemCardFooter.BottomAnchor.ConstraintEqualTo(itemCardView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            itemCardFooter.LeftAnchor.ConstraintEqualTo(itemCardView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            itemCardFooter.RightAnchor.ConstraintEqualTo(itemCardView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #endregion

            #region setItemNameView
            setItemNameView.TopAnchor.ConstraintEqualTo(imageView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            setItemNameView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            setItemNameView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            setItemNameView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemName.TopAnchor.ConstraintEqualTo(setItemNameView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblItemName.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            lblItemName.LeftAnchor.ConstraintEqualTo(setItemNameView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblItemName.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtItemName.TopAnchor.ConstraintEqualTo(lblItemName.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemName.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            txtItemName.LeftAnchor.ConstraintEqualTo(setItemNameView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtItemName.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region SetItemPriceView
            SetItemPriceView.TopAnchor.ConstraintEqualTo(setItemNameView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            SetItemPriceView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            SetItemPriceView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            SetItemPriceView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemPrice.TopAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblItemPrice.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            lblItemPrice.LeftAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblItemPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtItemPrice.TopAnchor.ConstraintEqualTo(lblItemPrice.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemPrice.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            txtItemPrice.LeftAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtItemPrice.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion
        }
        private void setupListView()
        {
            #region detail
            DetailClickView.TopAnchor.ConstraintEqualTo(SetItemPriceView.SafeAreaLayoutGuide.BottomAnchor, 5).Active = true;
            DetailClickView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            DetailClickView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            DetailClickView.HeightAnchor.ConstraintEqualTo(45).Active = true;

            lblDetail.TopAnchor.ConstraintEqualTo(DetailClickView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblDetail.WidthAnchor.ConstraintEqualTo(100).Active = true;
            lblDetail.LeftAnchor.ConstraintEqualTo(DetailClickView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblDetail.HeightAnchor.ConstraintEqualTo(18).Active = true;

            btnToggleDetail.TopAnchor.ConstraintEqualTo(DetailClickView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            btnToggleDetail.WidthAnchor.ConstraintEqualTo(28).Active = true;
            btnToggleDetail.RightAnchor.ConstraintEqualTo(DetailClickView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnToggleDetail.HeightAnchor.ConstraintEqualTo(28).Active = true;

            #region ItemCodeView
            ItemCodeView.TopAnchor.ConstraintEqualTo(DetailClickView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            ItemCodeView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ItemCodeView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ItemCodeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemCode.TopAnchor.ConstraintEqualTo(ItemCodeView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblItemCode.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblItemCode.LeftAnchor.ConstraintEqualTo(ItemCodeView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtItemCode.TopAnchor.ConstraintEqualTo(lblItemCode.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemCode.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            txtItemCode.LeftAnchor.ConstraintEqualTo(ItemCodeView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            #endregion
            #region CategoryView
            CategoryView.TopAnchor.ConstraintEqualTo(ItemCodeView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CategoryView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CategoryView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CategoryView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblCategory.TopAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblCategory.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblCategory.LeftAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            lblSelectedCategory.TopAnchor.ConstraintEqualTo(lblCategory.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblSelectedCategory.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            lblSelectedCategory.LeftAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            btnSelectCategory.CenterYAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnSelectCategory.WidthAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectCategory.RightAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectCategory.HeightAnchor.ConstraintEqualTo(24).Active = true;
            #endregion
            #region NoteView
            NoteView.TopAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            NoteView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            NoteView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            NoteView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblNote.TopAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblNote.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblNote.LeftAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtNote.TopAnchor.ConstraintEqualTo(lblNote.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtNote.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            txtNote.LeftAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            btnSelectNote.CenterYAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnSelectNote.WidthAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectNote.RightAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectNote.HeightAnchor.ConstraintEqualTo(24).Active = true;
            #endregion
            #region toppingView
            toppingView.TopAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            toppingView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            toppingView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            toppingView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblTopping.TopAnchor.ConstraintEqualTo(toppingView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblTopping.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblTopping.LeftAnchor.ConstraintEqualTo(toppingView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtTopping.TopAnchor.ConstraintEqualTo(lblTopping.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtTopping.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            txtTopping.LeftAnchor.ConstraintEqualTo(toppingView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            btnSelectTopping.CenterYAnchor.ConstraintEqualTo(toppingView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnSelectTopping.WidthAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectTopping.RightAnchor.ConstraintEqualTo(toppingView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectTopping.HeightAnchor.ConstraintEqualTo(24).Active = true;
            #endregion
            #region VatView
            VatView.TopAnchor.ConstraintEqualTo(toppingView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            VatView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            VatView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            VatView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblVat.TopAnchor.ConstraintEqualTo(VatView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblVat.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblVat.LeftAnchor.ConstraintEqualTo(VatView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            lblVatMode.TopAnchor.ConstraintEqualTo(lblVat.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            lblVatMode.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            lblVatMode.LeftAnchor.ConstraintEqualTo(VatView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            btnSelectVatType.CenterYAnchor.ConstraintEqualTo(VatView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            btnSelectVatType.WidthAnchor.ConstraintEqualTo(24).Active = true;
            btnSelectVatType.RightAnchor.ConstraintEqualTo(VatView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            btnSelectVatType.HeightAnchor.ConstraintEqualTo(24).Active = true;
            #endregion
            #region CostView
            CostView.TopAnchor.ConstraintEqualTo(VatView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            CostView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            CostView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CostView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblItemCost.TopAnchor.ConstraintEqualTo(CostView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblItemCost.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblItemCost.LeftAnchor.ConstraintEqualTo(CostView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            txtItemCost.TopAnchor.ConstraintEqualTo(lblItemCost.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtItemCost.WidthAnchor.ConstraintEqualTo(ContentView.Frame.Width - 20).Active = true;
            txtItemCost.LeftAnchor.ConstraintEqualTo(CostView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            #endregion
            #region ExtraSizeView
            ExtraSizeView.TopAnchor.ConstraintEqualTo(CostView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            ExtraSizeView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ExtraSizeView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ExtraSizeView.HeightAnchor.ConstraintEqualTo(60).Active = true;

            lblExtraSize.CenterYAnchor.ConstraintEqualTo(ExtraSizeView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            lblExtraSize.WidthAnchor.ConstraintEqualTo(200).Active = true;
            lblExtraSize.LeftAnchor.ConstraintEqualTo(ExtraSizeView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;

            switchExtraSize.CenterYAnchor.ConstraintEqualTo(ExtraSizeView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            switchExtraSize.WidthAnchor.ConstraintEqualTo(28).Active = true;
            switchExtraSize.RightAnchor.ConstraintEqualTo(ExtraSizeView.SafeAreaLayoutGuide.RightAnchor, -38).Active = true;
            switchExtraSize.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion
            #region ExtraSetView
            ExtraSizeCollectionView.TopAnchor.ConstraintEqualTo(ExtraSizeView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            ExtraSizeCollectionView.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            ExtraSizeCollectionView.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            ExtraSizeCollectionView.BottomAnchor.ConstraintEqualTo(btnAddMore.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;

            btnAddMore.HeightAnchor.ConstraintEqualTo(45).Active = true;
            btnAddMore.BottomAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.BottomAnchor, -15).Active = true;
            btnAddMore.LeftAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddMore.RightAnchor.ConstraintEqualTo(ContentView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            plusImg.CenterXAnchor.ConstraintEqualTo(btnAddMore.SafeAreaLayoutGuide.CenterXAnchor,-60).Active = true;
            plusImg.CenterYAnchor.ConstraintEqualTo(btnAddMore.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            plusImg.HeightAnchor.ConstraintEqualTo(28).Active = true;
            plusImg.WidthAnchor.ConstraintEqualTo(28).Active = true;

            lblBtnText.LeftAnchor.ConstraintEqualTo(plusImg.SafeAreaLayoutGuide.RightAnchor,10).Active = true;
            lblBtnText.CenterYAnchor.ConstraintEqualTo(btnAddMore.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            #endregion
            #endregion
        }
        public string Image
        {
            get { return itemCardView.Image.ToString(); }
            set
            {
                SetImage(itemCardView, value);
            }
        }
        public string ItemCardName
        {
            get { return lblItemCardName.Text; }
            set { lblItemCardName.Text = value; }
        }
        public string ItemName
        {
            get { return txtItemName.Text; }
            set { txtItemName.Text = value; }
        }
        public string ItemPrice
        {
            get { return txtItemPrice.Text; }
            set { txtItemPrice.Text = value; }
        }
        public string ItemCost
        {
            get { return txtItemCost.Text; }
            set { txtItemCost.Text = value; }
        }
        public string ItemCode
        {
            get { return txtItemCode.Text; }
            set { txtItemCode.Text = value; }
        }
        public string ItemCardPrice
        {
            get { return LblItemCardPrice.Text; }
            set { LblItemCardPrice.Text = value; }
        }
        public string ItemCardShortName
        {
            get { return lblItemCardShortName.Text; }
            set { lblItemCardShortName.Text = value; }
        }
        public string ItemSelectedCategory
        {
            get { return lblSelectedCategory.Text; }
            set { lblSelectedCategory.Text = value; }
        }
        public string ItemVatMode
        {
            get { return lblVatMode.Text; }
            set { lblVatMode.Text = value; }
        }
        public long Colors
        {
            get { return Colors; }
            set { SetColor(itemCardView, value); }
        }
        public static void SetColor(UIView itemCardView, long value)
        {
            int R, G, B;
            B = Convert.ToInt32(value / 65536);
            G = Convert.ToInt32((value - B * 65536) / 256);
            R = B;
            B = Convert.ToInt32(value - B * 65536 - G * 256);
            if (value != null && (R != 0 || G != 0 || B != 0))
            {
                itemCardView.BackgroundColor = UIColor.FromRGB(R, G, B);
            }
            else
            {
                itemCardView.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
        }
        public static void SetImage(UIImageView ImageView, string value)
        {
            if (value != null && value != "")
            {
                ImageView.Image = UIImage.FromBundle(value);
            }
        }
    }
}