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
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class ItemsAddNoteController : UIViewController
    {
        Note Insertnote = new Note();
        UILabel lblNote, lblNoteCate;
        UITextField txtNote, txtNoteCate;
        UIView NoteView, BottomView1, NoteCateView;
        public int SubCount;
        internal int SelectCategory; 
        UIImageView selectCate;
        Note noteData = new Note();
        int catID=0;
        UIButton btnAddCategory, btnDeleteBranch;
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        List<NoteCategory> NoteCat = new List<NoteCategory>();
        NoteManage NoteManager = new NoteManage();
        NoteCategoryManage NoteCateManager = new NoteCategoryManage();
        int systemID = 70;
        private bool Editchange = false;

        public ItemsAddNoteController() {
            this.noteData = null;
        }
        public ItemsAddNoteController(Note Data)
        {
            this.noteData = Data;
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
            //this.Title = "Add Note";
        }
        public override async void ViewDidLoad()
        {
            var view = new UIView();
            var button = new UIButton(UIButtonType.Custom);
            button.SetImage(UIImage.FromBundle("Backicon"), UIControlState.Normal);
            button.SetTitle("  Back", UIControlState.Normal);
            button.SetTitleColor(UIColor.Black, UIControlState.Normal);
            button.TouchUpInside += Button_TouchUpInside;
            button.TitleEdgeInsets = new UIEdgeInsets(top: 2, left: -8, bottom: 0, right: -0);
            button.SizeToFit();
            view.AddSubview(button);
            view.Frame = button.Bounds;
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(customView: view);

            this.NavigationController.SetNavigationBarHidden(false, false);
            this.NavigationController.NavigationBar.BarTintColor = new UIColor(51 / 255f, 172 / 255f, 225 / 255f, 1);
            base.ViewDidLoad();
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("en-US");
                System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                #region NoteCateView
                NoteCateView = new UIView();
                NoteCateView.BackgroundColor = UIColor.White;
                NoteCateView.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(NoteCateView);

                lblNoteCate = new UILabel
                {
                    TextColor = new UIColor(red: 64 / 225f, green: 64 / 255f, blue: 64 / 255f, alpha: 1),
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                lblNoteCate.Font = lblNoteCate.Font.WithSize(15);
                lblNoteCate.Text = Utils.TextBundle("notecat", "Note Category");
                NoteCateView.AddSubview(lblNoteCate);

                txtNoteCate = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(162,162,162),
                    TranslatesAutoresizingMaskIntoConstraints = false,

                    
                };
                txtNoteCate.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("notecat", "Note Category"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(162, 162, 162) });
                txtNoteCate.Font = txtNoteCate.Font.WithSize(15);
                NoteCateView.AddSubview(txtNoteCate);

                selectCate = new UIImageView();
                selectCate.TranslatesAutoresizingMaskIntoConstraints = false;
                selectCate.Image = UIImage.FromBundle("Next");
                NoteCateView.AddSubview(selectCate);

                selectCate.UserInteractionEnabled = true;
                var tapGesture0 = new UITapGestureRecognizer(this,
                new ObjCRuntime.Selector("FocusCategory:"))
                {
                    NumberOfTapsRequired = 1 // change number as you want 
                };
                selectCate.AddGestureRecognizer(tapGesture0);
                #endregion

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
                lblNote.Text = Utils.TextBundle("note", "Note");
                View.AddSubview(lblNote);

                txtNote = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB( 51, 170,225),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                };
                txtNote.EditingChanged += (object sender, EventArgs e) =>
                {
                    Editchange = true;
                    if (txtNote.Text.Length > 0)
                    {
                        btnAddCategory.Enabled = true;
                        btnAddCategory.BackgroundColor = UIColor.FromRGB(0, 149, 218);
                        btnAddCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
                    }
                    else
                    {
                        btnAddCategory.Enabled = false;
                        btnAddCategory.BackgroundColor = UIColor.White;
                        btnAddCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    }
                };
                txtNote.ReturnKeyType = UIReturnKeyType.Done;
                txtNote.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    return true;
                };

                txtNote.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("note", "Note"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtNote.Font = txtNote.Font.WithSize(15);
                View.AddSubview(txtNote);
                #endregion

                #region BottomView 1 for add
                BottomView1 = new UIView();
                BottomView1.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                BottomView1.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(BottomView1);

                btnAddCategory = new UIButton();
                btnAddCategory.SetTitle(Utils.TextBundle("addnote", "Add Note"), UIControlState.Normal);
                btnAddCategory.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
                btnAddCategory.Layer.CornerRadius = 5f;
                btnAddCategory.Layer.BorderWidth = 0.5f;
                btnAddCategory.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
                btnAddCategory.BackgroundColor = UIColor.White;
                btnAddCategory.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAddCategory.TouchUpInside += (sender, e) => {
                    // sum items
                    Editchange = false;
                    if(noteData!=null)
                    {
                        //edit
                        UpdateNote();
                    }
                    else
                    {
                        createNote();
                    }
                };
                View.AddSubview(btnAddCategory);

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
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("checkdeletenote", "Are you sure you want to delete note?"), UIAlertControllerStyle.Alert);
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textok", "OK"), UIAlertActionStyle.Default,
                        alert => DeleteNote(this.noteData)));
                    okCancelAlertController.AddAction(UIAlertAction.Create(Utils.TextBundle("textcancel", "Cancel"), UIAlertActionStyle.Cancel, alert => Console.WriteLine("Cancel was clicked")));
                    //Present Alert
                    PresentViewController(okCancelAlertController, true, null);
                };

                View.AddSubview(btnDeleteBranch);
                #endregion


                Textboxfocus(View);
                SetupAutoLayout();
                this.NoteCat = await NoteCateManager.GetAllNoteCategory();
                this.NoteCat.Insert(0, new NoteCategory() { SysNoteCategoryID = 0, Name = Utils.TextBundle("nonecategory", "None Category") });
                SetupPicker();
                if (noteData!=null)
                {
                    txtNote.Text = noteData.Message;
                    //  txtNoteCate.Text = noteData.SysNoteCategoryID;
                   // btnAddCategory.Enabled = false;
                    btnAddCategory.SetTitle(Utils.TextBundle("editnote", "Edit Note"), UIControlState.Normal);
                    if (noteData.SysNoteCategoryID != null)
                    {
                        var cat = NoteCat.Where(x => x.SysNoteCategoryID == noteData.SysNoteCategoryID).FirstOrDefault();
                        txtNoteCate.Text = cat.Name;
                        catID = (int)cat.SysNoteCategoryID;
                    }
                    else
                    {
                        txtNoteCate.Text = Utils.TextBundle("nonecategory", "None Category");
                        catID = 0;
                    }
                    
                }
                else
                {
                    Utils.SetConstant(btnDeleteBranch.Constraints, NSLayoutAttribute.Width, 0);
                    btnDeleteBranch.LeftAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                    btnDeleteBranch.Hidden = true;
                    btnAddCategory.Enabled = false;
                    btnAddCategory.BackgroundColor = UIColor.White;
                    btnAddCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    txtNoteCate.Text = Utils.TextBundle("nonecategory", "None Category");
                    catID = 0;
                    if (SelectCategory > 0)
                    {
                        txtNoteCate.Text = NoteCat.Where(x => x.SysNoteCategoryID == SelectCategory).FirstOrDefault().Name;
                        catID = SelectCategory;
                        SelectCategory = 0;
                    }
                }
                
            }
            catch (Exception ex)
            {

                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("havechage", "มีการเปลี่ยนแปลงต้องการบันทึกหรือไม่"), UIAlertControllerStyle.Alert);
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
            if (noteData != null)
            {
                //edit
                UpdateNote();
            }
            else
            {
                createNote();
            }
        }
        private async void DeleteNote(Note noteData)
        {
            NoteManage noteManage = new NoteManage();
            if (noteData != null)
            {
                Note Editnote = new Note();
                Editnote = noteData;
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
                    Utils.ShowMessage(Utils.TextBundle("deletesuccessfully", "Delete data successfully"));
                    this.NavigationController.PopViewController(false);
                    
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));

                }
            }
        }
        public async void UpdateNote()
        {
            try
            {
                //int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo((int)MainController.merchantlocal.MerchantID, DataCashingAll.DeviceNo, systemID);
                //var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                Insertnote.MerchantID = DataCashingAll.MerchantId;
                Insertnote.SysNoteID = Convert.ToInt64(noteData.SysNoteID);
                Insertnote.Ordinary = null;

                if (string.IsNullOrEmpty(txtNote.Text))
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณากรอกข้อมูลให้ครบถ้วน");
                    return;
                }
                if (catID == 0)
                {
                    Insertnote.SysNoteCategoryID = null;
                }
                else
                {
                    Insertnote.SysNoteCategoryID = catID;
                }
                Insertnote.Message = txtNote.Text;
                Insertnote.LastDateModified = DateTime.UtcNow;
                Insertnote.UserLastModified = Preferences.Get("User", "");
                Insertnote.DataStatus = 'M';
                Insertnote.FWaitSending = 1;
                Insertnote.WaitSendingTime = DateTime.UtcNow;


                var result = await NoteManager.UpdateNote(Insertnote);
                if (!result)
                {
                    Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), "กรุณาทำรายการใหม่ในภายหลัง");
                    return;
                }
                //JobQueue
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNote((int)Insertnote.MerchantID, (int)Insertnote.SysNoteID);
                }
                else
                {
                    Insertnote.FWaitSending = 2;
                    await NoteManager.UpdateNote(Insertnote);
                }
                Utils.ShowMessage(Utils.TextBundle("editnotesucceed", "Edit note succeed") );
                this.NavigationController.PopViewController(false);
                

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowAlert(this, Utils.TextBundle("failed", "Failed !"), Utils.TextBundle("cannotedit", "Failed to edit"));
            }
        }
        public async void createNote()
        {
            try
            {
                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo((int)MainController.merchantlocal.MerchantID, DataCashingAll.DeviceNo, systemID);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                Insertnote.MerchantID = DataCashingAll.MerchantId;
                Insertnote.SysNoteID = Convert.ToInt64(sys);

                Insertnote.Ordinary = null;

                if (string.IsNullOrEmpty(txtNote.Text))
                {
                    Utils.ShowAlert(this, Utils.TextBundle("unsuccessful", "Unsuccessful"), Utils.TextBundle("notcompletedata", "Please fill in all information."));
                    return;
                }

                //Insertnote.Message = txtNote.Text;
                
                if (catID == 0 )
                {
                    Insertnote.SysNoteCategoryID = null;
                }
                else
                {
                    Insertnote.SysNoteCategoryID = catID;
                }
                Insertnote.Message = txtNote.Text;
                Insertnote.LastDateModified = DateTime.UtcNow;
                Insertnote.UserLastModified = Preferences.Get("User", "");
                Insertnote.DataStatus = 'I';
                Insertnote.FWaitSending = 1;
                Insertnote.WaitSendingTime = DateTime.UtcNow;

                var result = NoteManager.InsertNote(Insertnote);
                if(result!=null)
                {
                    Utils.ShowMessage(Utils.TextBundle("addnotesucceed", "Add note succeed"));
                    
                    
                }
                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendNote((int)Insertnote.MerchantID, (int)Insertnote.SysNoteID);
                }
                else
                {
                    Insertnote.FWaitSending = 2;
                    await NoteManager.UpdateNote(Insertnote);
                }
                this.NavigationController.PopViewController(false);
                //JobQueue

            }
            catch (Exception ex)
            {
                await Utils.ReloadInitialData();
                await TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(Utils.TextBundle("cannotsave", "Failed to save"));
            }
        }
        [Export("FocusCategory:")]
        public void FocusCategory(UIGestureRecognizer sender)
        {
            txtNoteCate.BecomeFirstResponder();
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

            private readonly List<NoteCategory> values;
            public PickerModelCategory(List<NoteCategory> listCategory)
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
                    this.PickerChanged(this, new PickerChangedEventArgs
                    {
                        SelectedValue = values[Convert.ToInt32(row)].Name,
                        ID = (int)values[Convert.ToInt32(row)].SysNoteCategoryID
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
            var doneButton1 = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { View.EndEditing(true); });

            #region Category Picker
            PickerModelCategory model1 = new PickerModelCategory(this.NoteCat);
            model1.PickerChanged += (sender, e) => {
                txtNoteCate.Text = e.SelectedValue;
                catID = e.ID;
                Editchange = true;
            };
            UIToolbar toolbar1 = new UIToolbar();
            toolbar1.Translucent = true;
            toolbar1.SizeToFit();

           

            toolbar1.SetItems(new UIBarButtonItem[] { flexible, doneButton1 }, true);
            var pickerView = new UIPickerView() { Model = model1, ShowSelectionIndicator = true };
            txtNoteCate.InputView = pickerView;
            if (noteData != null)
            {
                if (noteData.SysNoteCategoryID != null)
                {
                    var row = this.NoteCat.FindIndex(x => x.SysNoteCategoryID == noteData.SysNoteCategoryID);
                    pickerView.Select(row, 0, false);
                }
               
            }
                txtNoteCate.InputAccessoryView = toolbar1;
            #endregion
        }
        void SetupAutoLayout()
        {
            #region NoteCateViewLayout
            NoteCateView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            NoteCateView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            NoteCateView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            NoteCateView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblNoteCate.TopAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblNoteCate.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            lblNoteCate.LeftAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNoteCate.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtNoteCate.TopAnchor.ConstraintEqualTo(lblNoteCate.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtNoteCate.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtNoteCate.LeftAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtNoteCate.HeightAnchor.ConstraintEqualTo(18).Active = true;

            selectCate.CenterYAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.CenterYAnchor).Active = true;
            selectCate.WidthAnchor.ConstraintEqualTo(28).Active = true;
            selectCate.RightAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            selectCate.HeightAnchor.ConstraintEqualTo(28).Active = true;
            #endregion

            #region NoteViewLayout
            NoteView.TopAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.BottomAnchor, 0.4f).Active = true;
            NoteView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            NoteView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            NoteView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblNote.TopAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.TopAnchor, 11).Active = true;
            lblNote.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            lblNote.LeftAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            lblNote.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtNote.TopAnchor.ConstraintEqualTo(lblNote.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtNote.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtNote.LeftAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtNote.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region BottomView 1 for add
            BottomView1.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView1.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView1.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView1.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;



            btnAddCategory.TopAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnAddCategory.BottomAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            //btnAddCategory.LeftAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnAddCategory.RightAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;

            btnDeleteBranch.TopAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnDeleteBranch.BottomAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnDeleteBranch.LeftAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnDeleteBranch.WidthAnchor.ConstraintEqualTo(45).Active = true;
            btnDeleteBranch.RightAnchor.ConstraintEqualTo(btnAddCategory.SafeAreaLayoutGuide.LeftAnchor, -10).Active = true;

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