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

namespace Gabana.iOS.ITEMS
{
    public partial class ItemsAddCategoryController : UIViewController
    {
        Gabana.ShareSource.Manage.CategoryManage categoryManager = new Gabana.ShareSource.Manage.CategoryManage();
        UILabel lblCategory;
        string oldName=null;
        UITextField txtCategory;
        ItemManage itemManager = new ItemManage();
        public static List<Item> Items;
        UIView CategoryView, BottomViewAdd, BottomViewEdit,line;
        UIButton btnAddCategory, btnSaveCategory,btnDelete;
        Category addCategory = new Category();
        public long sysCatID;
        private bool Editchange = false;

        public ItemsAddCategoryController() { this.sysCatID = 0;
                                              }
        public ItemsAddCategoryController(long SysCatID) {
            this.sysCatID = SysCatID;
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
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("havechage", "havechage"), UIAlertControllerStyle.Alert);
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    async alert => await BtnSave_Click3()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => this.NavigationController.PopViewController(true)));
                PresentViewController(okCancelAlertController, true, null);
            }
            else
            {
                this.NavigationController.PopViewController(true);
            }

        }
        private async Task BtnSave_Click3()
        {
            if (!string.IsNullOrEmpty(txtCategory.Text))
            {
                if (this.sysCatID ==0)
                {
                    addItemCategoryToDB();
                }
                else
                {
                    UpdateItemCategoryToDB();
                }
                
            }
            else
            {
                Utils.ShowMessage(Utils.TextBundle("enterall", "enterall"));
                btnAddCategory.Enabled = false;
            }
        }
        public async override void ViewDidLoad()
        {
            try
            {
                var view = new UIView();
                var button = new UIButton(UIButtonType.Custom);
                button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
                button.SetTitle(Utils.TextBundle("naviback", "naviback"), UIControlState.Normal);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
                button.TouchUpInside += Button_TouchUpInside;
                button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
                button.SizeToFit();
                view.AddSubview(button);
                view.Frame = button.Bounds;
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);
                this.NavigationController.SetNavigationBarHidden(false, false);
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                base.ViewDidLoad();

                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();

                if (this.sysCatID != null && this.sysCatID != 0)
                {
                    setCategoryData();
                    BottomViewEdit.Hidden = false;
                    BottomViewAdd.Hidden = true;
                }
                else
                {
                    BottomViewEdit.Hidden = true;
                    BottomViewAdd.Hidden = false;
                }

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(Utils.TextBundle("cannotdelete", "cannotdelete"));
            }
         }
        void initAttribute()
        {
            #region CategoryView
            CategoryView = new UIView();
            CategoryView.BackgroundColor = UIColor.White;
            CategoryView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(CategoryView);

            lblCategory = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblCategory.Font = lblCategory.Font.WithSize(15);
            lblCategory.Text = Utils.TextBundle("categoryname", "Category Name");
            CategoryView.AddSubview(lblCategory);

            txtCategory = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtCategory.ReturnKeyType = UIReturnKeyType.Done;
            txtCategory.EditingChanged += (object sender, EventArgs e) =>
            {
                if (!string.IsNullOrEmpty(txtCategory.Text))
                {
                    if (oldName != null && oldName != txtCategory.Text) // edit
                    {
                        btnSaveCategory.Enabled = true;
                        btnSaveCategory.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                        btnSaveCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
                    }
                    if (oldName != null && oldName == txtCategory.Text) // edit
                    {
                        btnSaveCategory.Enabled = false;
                        btnSaveCategory.BackgroundColor = UIColor.White;
                        btnSaveCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    }
                    else //add
                    {
                        btnAddCategory.Enabled = true;
                        btnAddCategory.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                        btnAddCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
                    }

                }
                else
                {
                    if (oldName != null)
                    {
                        btnSaveCategory.Enabled = false;
                        btnSaveCategory.BackgroundColor = UIColor.White;
                        btnSaveCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    }
                    else
                    {
                        btnAddCategory.Enabled = false;
                        btnAddCategory.BackgroundColor = UIColor.White;
                        btnAddCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    }

                }
            };
            txtCategory.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            txtCategory.AttributedPlaceholder = new NSAttributedString("Category", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(138, 211, 245) });
            txtCategory.Font = txtCategory.Font.WithSize(15);
            CategoryView.AddSubview(txtCategory);
            #endregion
            line = new UIView();
            line.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            line.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(line);
            #region BottomViewAdd
            BottomViewAdd = new UIView();
            BottomViewAdd.BackgroundColor = UIColor.Clear;
            BottomViewAdd.Hidden = true;
            BottomViewAdd.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomViewAdd);

            btnAddCategory = new UIButton();
            btnAddCategory.SetTitle(Utils.TextBundle("addcatagory", "Add Catagory"), UIControlState.Normal);
            btnAddCategory.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
            btnAddCategory.Layer.CornerRadius = 5f;
            btnAddCategory.Layer.BorderWidth = 0.5f;
            btnAddCategory.Enabled = false;
            btnAddCategory.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
            btnAddCategory.BackgroundColor = UIColor.White;
            btnAddCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnAddCategory.TouchUpInside += async (sender, e) => {
                // sum items
                btnAddCategory.Enabled = false;
                if (!string.IsNullOrEmpty(txtCategory.Text))
                {
                    addItemCategoryToDB();
                }
                else 
                {
                    Utils.ShowMessage(Utils.TextBundle("enterall", "enterall"));
                    btnAddCategory.Enabled = false;
                }
                    
            };
            BottomViewAdd.AddSubview(btnAddCategory);

            #endregion
            #region BottomViewEdit
            BottomViewEdit = new UIView();
            BottomViewEdit.Hidden = true;
            BottomViewEdit.BackgroundColor = UIColor.Clear;
            BottomViewEdit.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(BottomViewEdit);

            btnDelete = new UIButton();
            btnDelete.Layer.CornerRadius = 5f;
            btnDelete.BackgroundColor = UIColor.FromRGB(226, 226, 226);
            btnDelete.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            btnDelete.ImageEdgeInsets = new UIEdgeInsets(top: 10, left: 10, bottom: 10, right: 10);
            btnDelete.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDelete.TranslatesAutoresizingMaskIntoConstraints = false;
            btnDelete.TouchUpInside += (sender, e) => {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "wanttodelete"), UIAlertControllerStyle.Alert);

                //Add Actions
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                    alert => DeleteItemCategoryToDB()));
                okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));

                //Present Alert
                PresentViewController(okCancelAlertController, true, null);
            };
            BottomViewEdit.AddSubview(btnDelete);

            btnSaveCategory = new UIButton();
            btnSaveCategory.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSaveCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
            btnSaveCategory.Enabled = false;
            btnSaveCategory.Layer.BorderWidth = 1;
            btnSaveCategory.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSaveCategory.Layer.CornerRadius = 5f;
            btnSaveCategory.BackgroundColor = UIColor.White;
            btnSaveCategory.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSaveCategory.TouchUpInside += (sender, e) => {
                btnSaveCategory.Enabled = false;
                if (txtCategory.Text != null || txtCategory.Text != "")
                {
                    UpdateItemCategoryToDB();
                }
                else 
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("enterall", "enterall"));
                    btnSaveCategory.Enabled = true;
                }
                   
            };
            BottomViewEdit.AddSubview(btnSaveCategory);
            #endregion
        }
        private async void  setCategoryData()
        {
            try
            {
                var check = await categoryManager.GetCategory((int)DataCashingAll.MerchantId, (int)this.sysCatID);
               if(check!=null)
                {
                    addCategory = check;
                    oldName = check.Name;

                    txtCategory.Text = check.Name;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotload", "cannotload"));
            }
            
        }
        private async void addItemCategoryToDB()
        {
            try
            {


                DeviceSystemSeqNo deviceSystemSeq = new DeviceSystemSeqNo();
                DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();

                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 20);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                var result = false;
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                addCategory.MerchantID = DataCashingAll.MerchantId;
                addCategory.SysCategoryID = long.Parse(sys);
                addCategory.Ordinary = null;
                addCategory.Name = txtCategory.Text;
                addCategory.DateCreated = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.DataStatus = 'I';
                addCategory.FWaitSending = 1;
                addCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.LinkProMaxxID = null;

                result = await categoryManager.InsertCategory(addCategory);
                if (!result)
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotsave", "edititem"));
                    btnSaveCategory.Enabled = true;
                    return;
                }

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCatagory((int)addCategory.MerchantID, (int)addCategory.SysCategoryID);
                }
                else
                {
                    addCategory.FWaitSending = 2;
                    await categoryManager.UpdateCategory(addCategory);
                }

                ItemsController.Ismodify = true;
                Editchange = false;
                this.NavigationController.PopViewController(false);
                Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "edititem"));

            }
            catch (Exception ex)
            {
                await Utils.ReloadInitialData();
                await TinyInsights.TrackErrorAsync(ex);
                btnSaveCategory.Enabled = true;
                Utils.ShowMessage(Utils.TextBundle("cannotsave", "edititem"));
            }
        }
        private async void UpdateItemCategoryToDB()
        {
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                addCategory.Name = txtCategory.Text;
                addCategory.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.FWaitSending = 1;
                addCategory.DateCreated = Utils.GetTranDate(addCategory.DateCreated);
                addCategory.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                addCategory.DataStatus = 'M';

                var result = await categoryManager.UpdateCategory(addCategory);
                if (!result)
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "edititem"));
                    btnSaveCategory.Enabled = true;
                    return;
                }

                //test get category
                var getCate = await categoryManager.GetCategory((int)DataCashingAll.MerchantId, (int)sysCatID);

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCategory((int)addCategory.MerchantID, (int)addCategory.SysCategoryID);
                }
                else
                {
                    addCategory.FWaitSending = 2;
                    await categoryManager.UpdateCategory(addCategory);
                }
                ItemsController.Ismodify = true;
                Editchange = false;
                this.NavigationController.PopViewController(false); 
                Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "edititem"));
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                btnSaveCategory.Enabled = true;
            }
        }
        private async void DeleteItemCategoryToDB()
        {
            try
            {
                var cateDelte = await categoryManager.GetCategory((int)DataCashingAll.MerchantId, (int)sysCatID);
                var UpdateItem = await itemManager.GetItembyCategory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                if (UpdateItem != null)
                {
                    foreach (var update in UpdateItem)
                    {
                        update.SysCategoryID = null;
                        var resultUpdate = await itemManager.UpdateItem(update);
                    }
                }
                cateDelte.DataStatus = 'D';
                cateDelte.FWaitSending = 1;
                cateDelte.DateModified = Utils.GetTranDate(DateTime.UtcNow);
                var updateCate = await categoryManager.UpdateCategory(cateDelte);
                if (updateCate)
                {
                   
                    if (await GabanaAPI.CheckNetWork())
                    {
                        JobQueue.Default.AddJobSendCatagory((int)cateDelte.MerchantID, (int)cateDelte.SysCategoryID);
                    }
                    else
                    {
                        cateDelte.FWaitSending = 2;
                        await categoryManager.UpdateCategory(cateDelte);
                    }
                    ItemsController.Ismodify = true;
                    Editchange = false;
                    this.NavigationController.PopViewController(false);
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "edititem"));
                }
                else
                {
                    //this.NavigationController.PopViewController(false);
                    Utils.ShowMessage(Utils.TextBundle("cannotdelete", "edititem"));
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
        }
        void SetupAutoLayout()
        {
            #region CategoryViewLayout
            CategoryView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            CategoryView.HeightAnchor.ConstraintEqualTo(70).Active = true;
            CategoryView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            CategoryView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblCategory.BottomAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.CenterYAnchor, -2).Active = true;
            lblCategory.WidthAnchor.ConstraintEqualTo(View.Frame.Width-20).Active = true;
            lblCategory.LeftAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblCategory.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtCategory.TopAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.CenterYAnchor, 2).Active = true;
            txtCategory.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtCategory.LeftAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtCategory.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            line.TopAnchor.ConstraintEqualTo(CategoryView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            line.HeightAnchor.ConstraintEqualTo(0.4f).Active = true;
            line.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            line.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            #region BottomViewLayout
            BottomViewAdd.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomViewAdd.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomViewAdd.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomViewAdd.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnAddCategory.TopAnchor.ConstraintEqualTo(BottomViewAdd.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddCategory.BottomAnchor.ConstraintEqualTo(BottomViewAdd.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnAddCategory.LeftAnchor.ConstraintEqualTo(BottomViewAdd.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddCategory.RightAnchor.ConstraintEqualTo(BottomViewAdd.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            BottomViewEdit.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomViewEdit.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomViewEdit.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomViewEdit.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnDelete.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDelete.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDelete.LeftAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDelete.WidthAnchor.ConstraintEqualTo(45).Active = true;

            btnSaveCategory.TopAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSaveCategory.BottomAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSaveCategory.LeftAnchor.ConstraintEqualTo(btnDelete.SafeAreaLayoutGuide.RightAnchor, 10).Active = true;
            btnSaveCategory.RightAnchor.ConstraintEqualTo(BottomViewEdit.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            #endregion


        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }
   
}