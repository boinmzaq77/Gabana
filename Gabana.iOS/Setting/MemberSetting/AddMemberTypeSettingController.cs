using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
//using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class AddMemberTypeSettingController : UIViewController
    {
        Note Insertnote = new Note();
        UILabel lblNote, lblNoteCate;
        UITextField txtdiscount, txtMembertypename;
        UIView NoteView, BottomView1, NoteCateView;
        public int SubCount;
        UIImageView selectCate;
        Note noteData = new Note();
        ORM.Master.MemberType MastermemberType;
        ORM.MerchantDB.MemberType localmemberType;
        int catID = 0; 
        private MemberType memberType = new MemberType ();
        MemberTypeManage memberTypeManage = new MemberTypeManage();
        UIButton btnAddCategory, btnDeleteBranch;
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        List<NoteCategory> NoteCat = new List<NoteCategory>();
        NoteManage NoteManager = new NoteManage();
        NoteCategoryManage NoteCateManager = new NoteCategoryManage();
        int systemID = 60;
        decimal Discount;
        private bool Editchange = false;

        public AddMemberTypeSettingController(MemberType member) {
            memberType = member; 
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
        private void Button_TouchUpInside(object sender, EventArgs e)
        {
            if (Editchange)
            {
                var okCancelAlertController = UIAlertController.Create("", "มีการเปลี่ยนแปลต้องการบันทึกหรือไม่", UIAlertControllerStyle.Alert);
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
            if (memberType == null)
            {
                //AddMembertype
                if (await GabanaAPI.CheckNetWork())
                {
                    InsertMembertype();
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("checkinternet", "Please check the Internet"));
                    //Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }
            }
            else
            {
                //EditMembertype
                if (await GabanaAPI.CheckNetWork())
                {
                    UpdateMembertype();
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("checkinternet", "Please check the Internet"));
                    //Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }

            }
        }
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAddCategory.SetTitleColor(UIColor.White, UIControlState.Normal);
                btnAddCategory.Layer.CornerRadius = 5f;
                btnAddCategory.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            }
            else
            {
                btnAddCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                btnAddCategory.Layer.CornerRadius = 5f;
                btnAddCategory.BackgroundColor = UIColor.White;
                btnAddCategory.ClipsToBounds = true;
                btnAddCategory.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                btnAddCategory.Layer.BorderWidth = 0.5f;
            }
            btnAddCategory.Enabled = enable;
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
                lblNoteCate.Text = Utils.TextBundle("membertypename", "Member Type Name");
                NoteCateView.AddSubview(lblNoteCate);

                txtMembertypename = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(51, 170, 225),
                    TranslatesAutoresizingMaskIntoConstraints = false,


                };
                txtMembertypename.EditingChanged += TxtMembertypename_EditingChanged;
                txtMembertypename.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("membertypename", "Member Type Name"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtMembertypename.Font = txtMembertypename.Font.WithSize(15);
                NoteCateView.AddSubview(txtMembertypename);

                
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
                lblNote.Text = Utils.TextBundle("discount", "Discount");
                View.AddSubview(lblNote);

                txtdiscount = new UITextField
                {
                    BackgroundColor = UIColor.White,
                    TextColor = UIColor.FromRGB(51, 170, 225),
                    TranslatesAutoresizingMaskIntoConstraints = false,
                }; 
                txtdiscount.EditingDidBegin += Txtdiscount_EditingDidBegin;
                txtdiscount.EditingDidEnd += Txtdiscount_EditingDidEnd;
                txtdiscount.ShouldChangeCharacters = (textField, range, replacementString) => {
                    if (textField.Text.Contains(".") && replacementString == ".")
                    {
                        return false;
                    }
                    return true;
                };
                txtdiscount.EditingChanged += (object sender, EventArgs e) =>
                {
                    Editchange = true;
                    try
                    {
                        if (txtdiscount.Text.Length == 0)
                        {
                            Discount = 0;
                            return;
                        }

                        if (txtdiscount.Text.Contains('.'))
                        {
                            //var data = AllIndexOf(txtdiscount.Text, ".", StringComparison.Ordinal);
                            //if (data.Count > 1)
                            //{
                            //    return;
                            //}

                            var check = txtdiscount.Text.IndexOf('.');
                            if (check == 0)
                            {
                                txtdiscount.Text = "0.";
                                //txtdiscount.SetSelection(txtDiscount.Text.Length);
                            }
                        }

                        string discount = string.Empty;

                        if (txtdiscount.Text.Contains('%'))
                        {
                            discount = txtdiscount.Text.Replace("%", "");
                        }
                        else
                        {
                            discount = txtdiscount.Text;
                            
                        }

                        //discount = Regex.Replace(discount, @"\s+", "");

                        Discount = Convert.ToDecimal(discount);

                        decimal maxValue = 100;
                        if (maxValue < Discount)
                        {
                            //Toast.MakeText(this, GetString(Resource.String.maxdiscount) + " " + maxValue + " %", ToastLength.Short).Show();
                            Discount = 100;
                            txtdiscount.Text = Discount.ToString();
                            //txtdiscount.SetSelection(txtdiscount.Text.Length);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("TxtDiscount1_TextChanged at Membertype");
                        Utils.ShowMessage(ex.Message);
                        //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                };

                txtdiscount.ReturnKeyType = UIReturnKeyType.Done;
                txtdiscount.ShouldReturn = (tf) =>
                {
                    View.EndEditing(true);
                    return true;
                };
                txtdiscount.KeyboardType = UIKeyboardType.DecimalPad;
                txtdiscount.AttributedPlaceholder = new NSAttributedString("%", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
                txtdiscount.Font = txtdiscount.Font.WithSize(15);
                View.AddSubview(txtdiscount);
                #endregion

                #region BottomView 1 for add
                BottomView1 = new UIView();
                BottomView1.BackgroundColor = UIColor.FromRGB(248, 248, 248);
                BottomView1.TranslatesAutoresizingMaskIntoConstraints = false;
                View.AddSubview(BottomView1);

                btnAddCategory = new UIButton();
                btnAddCategory.SetTitle(Utils.TextBundle("addmembertype", "Add Member Type"), UIControlState.Normal);
                btnAddCategory.SetTitleColor(new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1), UIControlState.Normal);
                btnAddCategory.Layer.CornerRadius = 5f;
                btnAddCategory.Layer.BorderWidth = 0.5f;
                btnAddCategory.Layer.BorderColor = new UIColor(red: 0 / 255f, green: 149 / 255f, blue: 218 / 255f, alpha: 1).CGColor;
                btnAddCategory.BackgroundColor = UIColor.White;
                btnAddCategory.TranslatesAutoresizingMaskIntoConstraints = false;
                btnAddCategory.TouchUpInside += async (sender, e) => {
                    // sum items
                    Editchange = false;
                    if (memberType == null)
                    {
                        //AddMembertype
                        if (await GabanaAPI.CheckNetWork())
                        {
                            InsertMembertype();
                        }
                        else
                        {
                            Utils.ShowMessage(Utils.TextBundle("checkinternet", "Please check the Internet"));
                            //Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        //EditMembertype
                        if (await GabanaAPI.CheckNetWork())
                        {
                            UpdateMembertype();
                        }
                        else
                        {
                            Utils.ShowMessage(Utils.TextBundle("checkinternet", "Please check the Internet"));
                            //Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                        }

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
                    var okCancelAlertController = UIAlertController.Create("", Utils.TextBundle("wanttodelete", "Are you sure you want to delete?"), UIAlertControllerStyle.Alert);
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
                //SetupPicker();
                if (memberType != null)
                {
                    Discount = memberType.PercentDiscount;
                    txtdiscount.Text = ((double)memberType.PercentDiscount).ToString() + "%";//Utils.DisplayDecimal(Convert.ToDecimal(memberType.PercentDiscount))+"%";
                    txtMembertypename.Text = memberType.MemberTypeName;
                    //  txtNoteCate.Text = noteData.SysNoteCategoryID;
                    // btnAddCategory.Enabled = false;
                    btnAddCategory.SetTitle(Utils.TextBundle("editmembertype", "Edit Member Type"), UIControlState.Normal);
                    if (noteData.SysNoteCategoryID != null)
                    {
                        var cat = NoteCat.Where(x => x.SysNoteCategoryID == noteData.SysNoteCategoryID).FirstOrDefault();
                        txtMembertypename.Text = cat.Name;
                        catID = (int)cat.SysNoteCategoryID;
                    }
                    else
                    {
                        //txtNoteCate.Text = "None Category";
                        catID = 0;
                    }

                }
                else
                {
                    Utils.SetConstant(btnDeleteBranch.Constraints, NSLayoutAttribute.Width, 0);
                    btnDeleteBranch.LeftAnchor.ConstraintEqualTo(BottomView1.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
                    btnDeleteBranch.Hidden = true;
                    //btnAddCategory.Enabled = false;
                    btnAddCategory.BackgroundColor = UIColor.White;
                    btnAddCategory.SetTitleColor(UIColor.FromRGB(0, 149, 218), UIControlState.Normal);
                    txtMembertypename.Text = "";
                    catID = 0;
                }

            }
            catch (Exception ex)
            {

                await TinyInsights.TrackErrorAsync(ex);
            }

        }

        private void TxtMembertypename_EditingChanged(object sender, EventArgs e)
        {
            Editchange = true;
            if (txtMembertypename.Text!="")
            {
                SetButtonAdd(true);
            }
            else
            {
                SetButtonAdd(false);
            }
        }

        private void Txtdiscount_EditingDidEnd(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtdiscount.Text))
            {
                txtdiscount.Text = txtdiscount.Text.ToString() + "%";// Utils.DisplayDecimal(Convert.ToDecimal(txtdiscount.Text)) + "%";
            }
            
        }

        private void Txtdiscount_EditingDidBegin(object sender, EventArgs e)
        {
            txtdiscount.Text = txtdiscount.Text?.Replace("%","");
        }

        private async void InsertMembertype()
        {
            try
            {
                var lstmembertype = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                if (lstmembertype == null)
                {
                    return;
                }
                //กำหนด TypeNo
                int count = lstmembertype.Count;
                int MemberTypeNo = 0;

                var lstmember = new List<ORM.Master.MemberType>();
                if (!string.IsNullOrEmpty(txtMembertypename.Text))
                {
                    MastermemberType = new ORM.Master.MemberType()
                    {
                        DateModified = Utils.GetTranDate(DateTime.UtcNow),
                        LinkProMaxxID = null,
                        MemberTypeName = txtMembertypename.Text,
                        MemberTypeNo = MemberTypeNo,
                        MerchantID = DataCashingAll.MerchantId,
                        PercentDiscount = Discount
                    };
                    lstmember.Add(MastermemberType);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("checkinternet", "Please check the Internet"));
                    //Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                }

                //API
                var insertToAPI = await GabanaAPI.PostDataMemberType(lstmember);
                if (insertToAPI != null)
                {
                    localmemberType = new ORM.MerchantDB.MemberType()
                    {
                        DateModified = Utils.GetTranDate(insertToAPI[0].DateModified),
                        LinkProMaxxID = insertToAPI[0].LinkProMaxxID,
                        MemberTypeName = insertToAPI[0].MemberTypeName,
                        MemberTypeNo = insertToAPI[0].MemberTypeNo,
                        MerchantID = insertToAPI[0].MerchantID,
                        PercentDiscount = insertToAPI[0].PercentDiscount
                    };

                    //แล้วเพิ่มใหม่
                    var insert = await memberTypeManage.InsertMemberType(localmemberType);
                    if (insert)
                    {
                        Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                        this.NavigationController.PopViewController(false);
                        //Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                        //this.Finish();
                    }
                    else
                    {

                        //Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                        return;
                    }
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotsave", "Failed to save"));
                    
                    return;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("insertMemberType at Membertype");
                Utils.ShowMessage(Utils.TextBundle("cannotsave", "Failed to save"));
            }
        }

        private async void UpdateMembertype()
        {
            try
            {
                var lstmember = new List<ORM.Master.MemberType>();
                if (!string.IsNullOrEmpty(txtMembertypename.Text))
                {
                    MastermemberType = new ORM.Master.MemberType()
                    {
                        DateModified = Utils.GetTranDate(DateTime.UtcNow),
                        LinkProMaxxID = memberType.LinkProMaxxID,
                        MemberTypeName = txtMembertypename.Text,
                        MemberTypeNo = (int)memberType.MemberTypeNo,
                        MerchantID = (int)memberType.MerchantID,
                        PercentDiscount = Discount
                    };
                    lstmember.Add(MastermemberType);
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "Failed to edit"));
                    //Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                }

                //API
                var UpdateToAPI = await GabanaAPI.PutDataMemberType(lstmember);
                if (UpdateToAPI.Status)
                {
                    localmemberType = new ORM.MerchantDB.MemberType()
                    {
                        DateModified = Utils.GetTranDate(DateTime.UtcNow),
                        LinkProMaxxID = memberType.LinkProMaxxID,
                        MemberTypeName = txtMembertypename.Text,
                        MemberTypeNo = memberType.MemberTypeNo,
                        MerchantID = memberType.MerchantID,
                        PercentDiscount = Discount
                    };

                    //แล้วเพิ่มใหม่
                    var insert = await memberTypeManage.UpdateMemberType(localmemberType);
                    if (insert)
                    {
                        Utils.ShowMessage(Utils.TextBundle("editdatasuccessfully", "Edit data successfully"));
                        this.NavigationController.PopViewController(false);
                    }
                    else
                    {
                        Utils.ShowMessage(Utils.TextBundle("cannotedit", "Failed to edit"));
                        return;
                    }
                }
                else
                {
                    Utils.ShowMessage(Utils.TextBundle("cannotedit", "Failed to edit"));
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("insertMemberType at Membertype");
                Utils.ShowMessage(Utils.TextBundle("cannotedit", "Failed to edit"));
            }
        }
        private async void DeleteNote(Note noteData)
        {
            try
            {
                List<Gabana.ORM.Master.MemberType> lstmemberType = new List<Gabana.ORM.Master.MemberType>();

                var configlocal = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.MemberType, Gabana.ORM.Master.MemberType>();
                });

                var Imapperlocal = configlocal.CreateMapper();
                var Branchlocal = Imapperlocal.Map<ORM.MerchantDB.MemberType, Gabana.ORM.Master.MemberType>(memberType);

                lstmemberType.Add(Branchlocal);

                if (lstmemberType != null)
                {
                    var DeleteonCloud = await GabanaAPI.DeleteDataMemberType(lstmemberType);
                    if (DeleteonCloud == null)
                    {
                        await GabanaAPI.DeleteDataMemberType(lstmemberType);
                    }
                    if (!DeleteonCloud.Status)
                    {
                        Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                        return;
                    }
                    if (DeleteonCloud.Status)
                    {
                        //Set Null ที่ customer ที่มีการใช้ membertype 
                        CustomerManage customerManage = new CustomerManage();
                        var check = await customerManage.UpdateNullCustomerandDeleteMembeytype(DataCashingAll.MerchantId, lstmemberType[0].MemberTypeNo);
                        if (!check)
                        {
                            Utils.ShowMessage(Utils.TextBundle("failedtodelete", "Failed to delete"));
                            return;
                        }
                        if (check)
                        {
                            Utils.ShowMessage(Utils.TextBundle("successfullydeleted", "Successfully deleted"));
                            this.NavigationController.PopViewController(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowMessage(ex.Message);
                return;
            }
        }
        
        [Export("FocusCategory:")]
        public void FocusCategory(UIGestureRecognizer sender)
        {
            txtMembertypename.BecomeFirstResponder();
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
                txtMembertypename.Text = e.SelectedValue;
                catID = e.ID;
            };
            UIToolbar toolbar1 = new UIToolbar();
            toolbar1.Translucent = true;
            toolbar1.SizeToFit();



            toolbar1.SetItems(new UIBarButtonItem[] { flexible, doneButton1 }, true);
            var pickerView = new UIPickerView() { Model = model1, ShowSelectionIndicator = true };
            txtMembertypename.InputView = pickerView;
            if (noteData != null)
            {
                if (noteData.SysNoteCategoryID != null)
                {
                    var row = this.NoteCat.FindIndex(x => x.SysNoteCategoryID == noteData.SysNoteCategoryID);
                    pickerView.Select(row, 0, false);
                }

            }
            txtMembertypename.InputAccessoryView = toolbar1;
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

            txtMembertypename.TopAnchor.ConstraintEqualTo(lblNoteCate.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtMembertypename.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 50).Active = true;
            txtMembertypename.LeftAnchor.ConstraintEqualTo(NoteCateView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtMembertypename.HeightAnchor.ConstraintEqualTo(18).Active = true;

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

            txtdiscount.TopAnchor.ConstraintEqualTo(lblNote.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtdiscount.WidthAnchor.ConstraintEqualTo(View.Frame.Width - 20).Active = true;
            txtdiscount.LeftAnchor.ConstraintEqualTo(NoteView.SafeAreaLayoutGuide.LeftAnchor, 20).Active = true;
            txtdiscount.HeightAnchor.ConstraintEqualTo(18).Active = true;
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