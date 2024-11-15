using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddEmployeeActivity : AppCompatActivity
    {
        public static AddEmployeeActivity addEmployee;
        LinearLayout lnBack, lnRoles, lnPinCode, lnConfirmPass, lnBranch;
        RelativeLayout lnChangPass;
        FrameLayout lnShowDelete, lnDelete, lnPassword, frameEmprole, frameBranch;
        public TextView txtEmployeeUserName, textTitle, textBranch, textEmpRole;
        EditText txtEmployeePassword, txtEmployeeConfirmPassword, txtComment, txtEmployeeName, textNewPass, txtConfirm;
        ImageButton imgRoles;
        internal Button btnAddEmployee;
        Button btnChangePass;
        ImageButton btnDeleteChangePass;
        Switch switchShow;
        public static Gabana.ORM.MerchantDB.UserAccountInfo LocalEmployee /*=new ORM.MerchantDB.UserAccountInfo()*/;
        public static Gabana.Model.UserAccount EmployeeDetail = new Model.UserAccount();
        public static string EmployeeUsername = string.Empty;
        List<string> Branch;
        BranchManage branchManage = new BranchManage();
        bool changePass;
        int UsePincode;
        UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
        BranchPolicyManage policyManage = new BranchPolicyManage();
        Model.UserAccountInfo empDetail;
        public static string positionEmp, oldpositionEmp;
        public static List<BranchPolicy> listChooseBranch = new List<BranchPolicy>();
        private List<BranchPolicy> oldListBrach = new List<BranchPolicy>();
        string LoginType;

        public List<Gabana.Model.EmployeeRole> listEmpRoles { get; set; }
        bool first = true, flagdatachange = false;
        private string empDataComment = "";

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addemployee_activity_main);
                addEmployee = this;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnRoles = FindViewById<LinearLayout>(Resource.Id.lnRoles);
                lnConfirmPass = FindViewById<LinearLayout>(Resource.Id.lnConfirmPass);
                lnBranch = FindViewById<LinearLayout>(Resource.Id.lnBranch);
                txtEmployeeUserName = FindViewById<TextView>(Resource.Id.txtEmployeeUserName);
                txtEmployeeConfirmPassword = FindViewById<EditText>(Resource.Id.txtEmployeeConfirmPassword);
                txtEmployeeConfirmPassword.TextChanged += TxtEmployeeConfirmPassword_TextChanged;
                txtEmployeeName = FindViewById<EditText>(Resource.Id.txtEmployeeName);

                txtComment = FindViewById<EditText>(Resource.Id.txtComment);
                textNewPass = FindViewById<EditText>(Resource.Id.textNewPass);
                txtConfirm = FindViewById<EditText>(Resource.Id.txtConfirm);
                imgRoles = FindViewById<ImageButton>(Resource.Id.imgRoles);
                btnAddEmployee = FindViewById<Button>(Resource.Id.btnAddEmployee);
                textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                switchShow = FindViewById<Switch>(Resource.Id.switchShow);
                textBranch = FindViewById<TextView>(Resource.Id.textBranch);
                textEmpRole = FindViewById<TextView>(Resource.Id.textEmpRole);

                txtEmployeePassword = FindViewById<EditText>(Resource.Id.txtEmployeePassword);
                txtEmployeePassword.TextChanged += TxtEmployeePassword_TextChanged;
                txtComment.TextChanged += TxtComment_TextChanged;
                textNewPass.TextChanged += TextNewPass_TextChanged;
                txtConfirm.TextChanged += TxtConfirm_TextChanged;

                lnBack.Click += LnBack_Click;
                lnRoles.Click += LnRoles_Click;
                lnBranch.Click += LnBranch_Click;
                switchShow.CheckedChange += SwitchShow_CheckedChange;

                LoginType = Preferences.Get("LoginType", "");
                LoginType = LoginType.ToLower();

                CheckJwt();

                //สิทธิ์ Owner,Admin เข้าได้ทุก User และแก้ไขได้ทุกอย่าง ยกเว้น Password สามารถ Reset ได้อย่างเดียว (กรณีที่ พนักงานลืม Password)
                //-สิทธิ์อื่น จะเข้าไปแก้ไข Full name, Comment ของ User ตัวเองได้เท่านั้น
                //*หาก User ต้องการแก้ไขรหัสผ่านและ Pincode จะต้องจากหน้า Change Password เท่านั้น

                Branch = new List<string>();
                SelectBranch();
                btnChangePass = FindViewById<Button>(Resource.Id.btnChangePass);
                btnChangePass.Visibility = ViewStates.Gone;
                lnChangPass = FindViewById<RelativeLayout>(Resource.Id.lnChangPass);
                lnChangPass.Visibility = ViewStates.Gone;
                lnPinCode = FindViewById<LinearLayout>(Resource.Id.lnPinCode);
                lnShowDelete = FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
                lnDelete = FindViewById<FrameLayout>(Resource.Id.lnDelete);
                lnPassword = FindViewById<FrameLayout>(Resource.Id.lnPassword);
                frameEmprole = FindViewById<FrameLayout>(Resource.Id.frameEmprole);
                frameBranch = FindViewById<FrameLayout>(Resource.Id.frameBranch);
                btnDeleteChangePass = FindViewById<ImageButton>(Resource.Id.btnDeleteChangePass);
                btnDeleteChangePass.Click += BtnDeleteChangePass_Click;
                lnPinCode.Visibility = ViewStates.Gone;
                lnShowDelete.Visibility = ViewStates.Gone;
                lnShowDelete.Click += LnShowDelete_Click;
                lnDelete.Click += LnShowDelete_Click;
                txtEmployeePassword.Enabled = true;
                txtEmployeePassword.Hint = "********";
                txtEmployeePassword.Text = string.Empty;
                txtEmployeeName.TextChanged += TxtEmployeeName_TextChanged;

                //Update UserAcccount
                if (LocalEmployee != null)
                {
                    btnAddEmployee.Text = GetString(Resource.String.textsave);
                    textTitle.Text = GetString(Resource.String.employee_activity_edit);
                    txtEmployeePassword.Text = "********";
                    txtEmployeePassword.Enabled = false;
                    ShowUserAccountDetail();
                    btnAddEmployee.Click += BtnEditEmployee_Click;
                    btnChangePass.Visibility = ViewStates.Visible;
                    btnChangePass.Click += BtnChangePass_Click;
                    lnPinCode.Visibility = ViewStates.Gone;
                    lnShowDelete.Visibility = ViewStates.Visible;                    

                    //การแสดงของข้อมูล Owner โดยที่ log in เป็น Owner
                    if (LoginType == "owner" & positionEmp.ToLower() == "owner")
                    {
                        lnPassword.Visibility = ViewStates.Gone;
                        lnConfirmPass.Visibility = ViewStates.Gone;
                        frameEmprole.Visibility = ViewStates.Gone;
                        frameBranch.Visibility = ViewStates.Gone;
                        lnShowDelete.Visibility = ViewStates.Gone;
                    }

                    //การแสดงของข้อมูล Owner โดยที่ log in เป็น Admin
                    if (LoginType == "admin" & positionEmp.ToLower() == "owner")
                    {
                        lnPassword.Visibility = ViewStates.Gone;
                        lnConfirmPass.Visibility = ViewStates.Gone;
                        frameEmprole.Visibility = ViewStates.Gone;
                        frameBranch.Visibility = ViewStates.Gone;
                        lnShowDelete.Visibility = ViewStates.Gone;
                        btnAddEmployee.Visibility = ViewStates.Gone;
                        txtEmployeeName.Enabled = false;
                        txtComment.Enabled = false;
                        txtEmployeeUserName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
                        txtEmployeeName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
                        txtComment.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
                    }

                    //การแสดงของข้อมูล Admin คนอื่น โดยที่ log in เป็น Admin 
                    if (LoginType == "admin" & positionEmp.ToLower() == "admin")
                    {
                        lnShowDelete.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    //Insert UserAccount
                    //เพิ่มปกติ - กดที่ปุ่มเพิ่มพนักงาน
                    btnAddEmployee.Click += BtnAddEmployee_Click;
                    textTitle.Text = GetString(Resource.String.employee_activity_add);
                    btnAddEmployee.Text = Application.Context.GetString(Resource.String.employee_activity_add);

                    if (EmployeeDetail.UserName == null & !string.IsNullOrEmpty(EmployeeUsername))
                    {
                        txtEmployeeUserName.Text = EmployeeUsername;
                        textEmpRole.Text = "เลือกบทบาทพนักงาน";
                        positionEmp = string.Empty;
                    }
                    else
                    {
                        //เพิ่มข้อมูลจาก Seauth
                        txtEmployeeUserName.Text = EmployeeDetail.UserName;
                        txtEmployeeName.Text = EmployeeDetail.FullName;
                        positionEmp = EmployeeDetail.MainRoles;
                        if (positionEmp.ToLower() == "owner" | positionEmp.ToLower() == "admin")
                        {
                            textBranch.Text = GetString(Resource.String.addemployee_allbranch);
                            textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        }
                    }                    
                }
                                
                first = false;
                SetButtonAdd(false);
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddEmployeeActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackPageViewAsync("OnCreate at Add Emp");
                return;
            }
        }

        private void TxtEmployeeConfirmPassword_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtEmployeePassword_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtConfirm_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TextNewPass_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtComment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtEmployeeName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private async void CheckDataChange()
        {
            try
            {
                if (first)
                {
                    SetButtonAdd(false);
                    return;
                }
                if (LocalEmployee == null)
                {
                    if (string.IsNullOrEmpty(txtEmployeeName.Text))
                    {
                        SetButtonAdd(false);
                        flagdatachange = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(positionEmp))
                    {
                        SetButtonAdd(false);
                        flagdatachange = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(txtEmployeePassword.Text))
                    {
                        SetButtonAdd(false);
                        flagdatachange = true;
                        return;
                    }
                    if (txtEmployeePassword.Text.Length < 4)
                    {
                        SetButtonAdd(false);
                        flagdatachange = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(txtEmployeeConfirmPassword.Text))
                    {
                        SetButtonAdd(false);
                        flagdatachange = true;
                        return;
                    }
                    if (listChooseBranch.Count == 0 && positionEmp != "Admin")
                    {
                        SetButtonAdd(false);
                        flagdatachange = true;
                        return;
                    }
                    SetButtonAdd(true);
                    flagdatachange = true;
                }
                else
                {
                    if (txtEmployeeName.Text != empDetail.FullName)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (oldpositionEmp != positionEmp)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    oldListBrach = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, empDetail.UserName);
                    HashSet<long> sentIDs = new HashSet<long>(oldListBrach.Select(s => s.SysBranchID));
                    List<BranchPolicy> results = new List<BranchPolicy>();
                    results = listChooseBranch.Where(m => !sentIDs.Contains(m.SysBranchID)).ToList();
                    if (results.Count > 0)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    if (changePass)
                    {
                        if (string.IsNullOrEmpty(textNewPass.Text))
                        {
                            SetButtonAdd(false);
                            return;
                        }
                        if (string.IsNullOrEmpty(txtConfirm.Text))
                        {
                            SetButtonAdd(false);
                            return;
                        }
                        if (textNewPass.Text != txtConfirm.Text)
                        {
                            SetButtonAdd(false);
                            return;
                        }

                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    if (txtComment.Text != empDataComment)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    SetButtonAdd(false);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckDataChange at Add Emp");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAddEmployee.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAddEmployee.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAddEmployee.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAddEmployee.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnAddEmployee.Enabled = enable;
        }

        private void LnShowDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (LoginType == "owner" | LoginType == "admin")
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("deleteType", "employee");
                    bundle.PutString("username", LocalEmployee.UserName);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnShowDelete_Click at Add Emp");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SwitchShow_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (switchShow.Checked)
            {
                UsePincode = 1;
            }
            else
            {
                UsePincode = 0;
            }
        }

        private void LnBranch_Click(object sender, EventArgs e)
        {
            if (LoginType == "owner" | LoginType == "admin")
            {
                if (string.IsNullOrEmpty(positionEmp))
                {
                    Toast.MakeText(this, GetString(Resource.String.selectpositionforemp), ToastLength.Short).Show();
                    return;
                }

                if (positionEmp.ToLower() != "admin")
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(ChooseBranchActivity)));
                    ChooseBranchActivity.SetBranch(listChooseBranch, txtEmployeeUserName.Text);
                }
            }
        }

        private void BtnDeleteChangePass_Click(object sender, EventArgs e)
        {
            changePass = false;
            txtEmployeePassword.Enabled = true;
            btnChangePass.Enabled = true;
            btnChangePass.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            lnChangPass.Visibility = ViewStates.Gone;
        }

        private void BtnChangePass_Click(object sender, EventArgs e)
        {
            if (LoginType == "owner" | LoginType == "admin")
            {
                changePass = true;
                txtEmployeePassword.Enabled = false;
                btnChangePass.Enabled = false;
                btnChangePass.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
                lnChangPass.Visibility = ViewStates.Visible;
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        private async void BtnEditEmployee_Click(object sender, EventArgs e)
        {
            btnAddEmployee.Enabled = false;
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show(); btnAddEmployee.Enabled = true;
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show(); btnAddEmployee.Enabled = true;
                return;
            }

            UpdateUserAccount();
            btnAddEmployee.Enabled = true;
        }

        private async void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            btnAddEmployee.Enabled = false;
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                btnAddEmployee.Enabled = true;
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                btnAddEmployee.Enabled = true;
                return;
            }

            if (string.IsNullOrEmpty(txtEmployeeName.Text))
            {
                txtEmployeeName.RequestFocus();
                Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                btnAddEmployee.Enabled = true;
                return;
            }
            if (txtEmployeePassword.Text.Length < 4)
            {
                txtEmployeePassword.RequestFocus();
                Toast.MakeText(this, GetString(Resource.String.inputpasswordmin), ToastLength.Short).Show();
                btnAddEmployee.Enabled = true;
                return;
            }
            if (string.IsNullOrEmpty(txtEmployeePassword.Text))
            {
                txtEmployeePassword.RequestFocus();
                Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                btnAddEmployee.Enabled = true;
                return;
            }
            if (string.IsNullOrEmpty(txtEmployeeConfirmPassword.Text))
            {
                txtEmployeeConfirmPassword.RequestFocus();
                Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                return;
            }
            string password = txtEmployeePassword.Text.Trim();
            string confirm = txtEmployeeConfirmPassword.Text.Trim();
            if (password != confirm)
            {
                Toast.MakeText(this, GetString(Resource.String.passwordnotmath), ToastLength.Short).Show(); btnAddEmployee.Enabled = true;
                return;
            }
            InsertEmployee();
            btnAddEmployee.Enabled = true;
        }

        private void LnRoles_Click(object sender, EventArgs e)
        {
            if (LoginType == "owner" | LoginType == "admin")
            {

                StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeRoleActivity)));
                EmployeeRoleActivity.SelectPosition = textEmpRole.Text;
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            LocalEmployee = null;
            ClearText();
        }

        public override void OnBackPressed()
        {
            try
            {
                if (LocalEmployee == null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "employee");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "employee");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public async void InsertEmployee()
        {
            try
            {
                // No Content
                //Gabana API

                lnShowDelete.Visibility = ViewStates.Gone;

                //Send to Ownerchange
                string password = string.Empty;
                string confirm = string.Empty;
                string comment = string.Empty;

                comment = txtComment.Text;
                password = txtEmployeePassword.Text.Trim();
                confirm = txtEmployeeConfirmPassword.Text.Trim();

                if (password != confirm)
                {
                    Toast.MakeText(this, GetString(Resource.String.passwordnotmath), ToastLength.Short).Show();
                    return;
                }

                if (positionEmp.ToLower() == "invoice officer")
                {
                    positionEmp = positionEmp.Replace("Invoice Officer", "Invoice");
                }

                // Send to Seauth Post UserAccount
                Gabana.Model.UserAccount APIUser = new Model.UserAccount()
                {
                    UserName = txtEmployeeUserName.Text,
                    FullName = txtEmployeeName.Text, //Not null
                    MainRoles = positionEmp,
                    PasswordHash = password.ToString(),
                };

                //ชื่อตรงกับที่ Seauth2
                if (DataCashing.addEmployeefromSeauth)
                {
                    //สิทธิ์ในการ Login Branch
                    // Owner , Admin จะได้ All Branch ไม่ต้อง Add ข้อมูลที่ Table นี้
                    // Manager: กำหนดได้มากกว่า 1 row
                    // สิทธิ์ที่ต่ำกว่า Manager กำหนดได้แค่ 1 row เท่านั้น

                    List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();
                    //GabanaAPI

                    if (positionEmp.ToLower() == "admin")
                    {
                        listChooseBranch = new List<BranchPolicy>();
                    }
                    else
                    {
                        if (listChooseBranch.Count == 0)
                        {
                            Toast.MakeText(this, GetString(Resource.String.selectbrach), ToastLength.Short).Show();
                            return;
                        }
                    }

                    foreach (var item in listChooseBranch)
                    {
                        ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                        {
                            MerchantID = (int)item.MerchantID,
                            UserName = item.UserName,
                            SysBranchID = (int)item.SysBranchID
                        };
                        lstbranchPolicies.Add(branchPolicy);
                    }

                    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployeeUserName.Text,
                        FUsePincode = UsePincode,
                        PinCode = null,
                        Comments = comment,
                    };

                    Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                    {
                        branchPolicy = lstbranchPolicies,
                        userAccountInfo = gbnAPIUser
                    };

                    //GabanaAPI
                    var postgbnAPIUser = await GabanaAPI.PostDataUserAccount(userAccountResult);
                    //insert ที่ gabanaAPI ไม่เสร็จ ลบที่ Seauth ด้วย
                    if (!postgbnAPIUser.Status)
                    {
                        Toast.MakeText(this, postgbnAPIUser.Message, ToastLength.Short).Show();
                        var result = await GabanaAPI.DeleteSeAuthDataUserAccount(txtEmployeeUserName.Text);
                        return;
                    }

                    //Local
                    ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployeeUserName.Text,
                        FUsePincode = UsePincode,
                        PinCode = null,
                        Comments = comment
                    };

                    //insert Local useraccount
                    var Updatetlocal = await accountInfoManage.UpdateUserAccount(localUser);
                    if (!Updatetlocal)
                    {
                        return;
                    }

                    //insert Local BranchPolicy
                    foreach (var itembranch in lstbranchPolicies)
                    {
                        ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                        {
                            MerchantID = itembranch.MerchantID,
                            SysBranchID = (int)itembranch.SysBranchID,
                            UserName = itembranch.UserName,
                        };
                        var insertlocalbranchPolicy = await policyManage.InsertBranchPolicy(branchPolicy);
                    }

                    Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                    {
                        UserName = txtEmployeeUserName.Text,
                        MainRoles = positionEmp,
                        UserAccessProduct = true,
                        FullName = txtEmployeeName.Text,
                        MerchantID = DataCashingAll.MerchantId,
                    };

                    DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                    ClearText();
                    EmployeeActivity.SetFocusEmployee(localUser);
                    EmployeeActivity.employeeActivity.Finish();
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeActivity)));
                    this.Finish();
                }
                else
                {
                    //Seauth
                    var resultInsert = await GabanaAPI.PostSeAuthDataUserAccount(APIUser);
                    if (!resultInsert.Status)
                    {
                        //Pop up Error
                        Toast.MakeText(this, resultInsert.Message, ToastLength.Short).Show();
                        return;
                    }
                    //สิทธิ์ในการ Login Branch
                    // Owner , Admin จะได้ All Branch ไม่ต้อง Add ข้อมูลที่ Table นี้
                    // Manager: กำหนดได้มากกว่า 1 row
                    // สิทธิ์ที่ต่ำกว่า Manager กำหนดได้แค่ 1 row เท่านั้น

                    List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();
                    //GabanaAPI

                    if (positionEmp.ToLower() == "admin")
                    {
                        listChooseBranch = new List<BranchPolicy>();
                    }
                    else
                    {
                        if (listChooseBranch.Count == 0)
                        {
                            Toast.MakeText(this, GetString(Resource.String.selectbrach), ToastLength.Short).Show();
                            return;
                        }
                    }

                    foreach (var item in listChooseBranch)
                    {
                        ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                        {
                            MerchantID = (int)item.MerchantID,
                            UserName = item.UserName,
                            SysBranchID = (int)item.SysBranchID
                        };
                        lstbranchPolicies.Add(branchPolicy);
                    }

                    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployeeUserName.Text,
                        FUsePincode = UsePincode,
                        PinCode = null,
                        Comments = comment,
                    };

                    Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                    {
                        branchPolicy = lstbranchPolicies,
                        userAccountInfo = gbnAPIUser
                    };

                    //GabanaAPI
                    var postgbnAPIUser = await GabanaAPI.PostDataUserAccount(userAccountResult);
                    //insert ที่ gabanaAPI ไม่เสร็จ ลบที่ Seauth ด้วย
                    if (!postgbnAPIUser.Status)
                    {
                        Toast.MakeText(this, postgbnAPIUser.Message, ToastLength.Short).Show();
                        var result = await GabanaAPI.DeleteSeAuthDataUserAccount(txtEmployeeUserName.Text);
                        return;
                    }

                    //Local
                    ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployeeUserName.Text,
                        FUsePincode = UsePincode,
                        PinCode = null,
                        Comments = comment
                    };

                    //insert Local useraccount
                    var insertlocal = await accountInfoManage.InsertUserAccount(localUser);
                    if (!insertlocal)
                    {
                        return;
                    }

                    //insert Local BranchPolicy
                    foreach (var itembranch in lstbranchPolicies)
                    {
                        ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                        {
                            MerchantID = itembranch.MerchantID,
                            SysBranchID = (int)itembranch.SysBranchID,
                            UserName = itembranch.UserName,
                        };
                        var insertlocalbranchPolicy = await policyManage.InsertBranchPolicy(branchPolicy);
                    }

                    Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                    {
                        UserName = txtEmployeeUserName.Text,
                        MainRoles = positionEmp,
                        UserAccessProduct = true,
                        FullName = txtEmployeeName.Text,
                        MerchantID = DataCashingAll.MerchantId,
                    };

                    DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                    ClearText();
                    EmployeeActivity.employeeActivity.Finish();
                    EmployeeActivity.SetFocusEmployee(localUser);
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeActivity)));
                    this.Finish();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertEmployee at Add Emp");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async void ShowUserAccountDetail()
        {
            try
            {
                lnConfirmPass.Visibility = ViewStates.Gone;
                if (DataCashingAll.UserAccountInfo != null)
                {
                    empDetail = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == LocalEmployee.UserName.ToLower()).FirstOrDefault();
                    if (empDetail != null)
                    {
                        txtEmployeeUserName.Text = LocalEmployee.UserName.ToLower();
                        txtEmployeePassword.Text = string.Empty;
                        txtEmployeeConfirmPassword.Visibility = ViewStates.Gone;

                        var empData = await accountInfoManage.GetUserAccount(DataCashingAll.MerchantId, LocalEmployee.UserName.ToLower());
                        if (empData?.Comments != null)
                        {
                            empDataComment = empData?.Comments;
                        }
                        txtComment.Text = empData?.Comments;

                        txtEmployeeName.Text = empDetail.FullName;
                        positionEmp = empDetail.MainRoles;
                        oldpositionEmp = empDetail.MainRoles;


                        //Switch
                        if (LocalEmployee.FUsePincode == 1)
                        {
                            switchShow.Checked = true;
                        }
                        else
                        {
                            switchShow.Checked = false;
                        }

                        listChooseBranch = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, empDetail.UserName);
                        oldListBrach = listChooseBranch;
                        if (positionEmp.ToLower() == "owner" | positionEmp.ToLower() == "admin")
                        {
                            textBranch.Text = GetString(Resource.String.addemployee_allbranch);
                            textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        }
                    }
                    else
                    {
                        Utils.RestartApplication(addEmployee, "main", 1);
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("ShowUserAccountDetail at Add Emp");
                return;
            }
        }

        private async void UpdateUserAccount()
        {
            try
            {
                //Send to Ownerchange
                string password = string.Empty;
                string confirm = string.Empty;
                string comment = string.Empty;
                string Newpass = string.Empty;

                comment = txtComment.Text;
                password = txtEmployeePassword.Text.Trim();
                Newpass = textNewPass.Text.Trim();
                confirm = txtConfirm.Text.Trim();

                if (changePass)
                {
                    if (Newpass != confirm)
                    {
                        Toast.MakeText(this, GetString(Resource.String.passwordnotmath), ToastLength.Short).Show();
                        return;
                    }

                    Model.ChangePassword changePassword = new Model.ChangePassword();
                    changePassword.Username = txtEmployeeUserName.Text;
                    changePassword.OldPassword = string.Empty;
                    changePassword.Password = Newpass;
                    changePassword.ConfirmPassword = confirm;

                    var result = await GabanaAPI.PutSeAuthDataOnwerChangePassword(changePassword);
                    if (!result.Status)
                    {
                        Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                if (positionEmp.ToLower() == "owner")
                {
                    positionEmp = "Admin";
                }

                if (positionEmp.ToLower() == "invoice officer")
                {
                    var newstr = positionEmp.Replace("Invoice Officer", "Invoice");
                    positionEmp = newstr;
                }

                // Send to Seauth Put UserAccount
                Gabana.Model.UserAccount APIUser = new Model.UserAccount()
                {
                    UserName = txtEmployeeUserName.Text,
                    FullName = txtEmployeeName.Text, //Not null
                    MainRoles = positionEmp,
                    PasswordHash = password.ToString(),
                };

                Model.ResultAPI resultUpdate;
                if (LoginType == "owner" | LoginType == "admin")
                {
                    resultUpdate = await GabanaAPI.PutSeAuthDataUserAccount(APIUser);
                }
                else
                {
                    resultUpdate = await GabanaAPI.PutSeAuthEmployeeDataUserAccount(APIUser);
                }
                if (!resultUpdate.Status)
                {
                    //Pop up Error
                    Toast.MakeText(this, resultUpdate.Message, ToastLength.Short).Show();
                    return;
                }

                //GabanaAPI
                List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();

                if (positionEmp.ToLower() == "admin")
                {
                    listChooseBranch = new List<BranchPolicy>();
                }

                if (txtEmployeeUserName.Text.ToLower() == "owner")
                {
                    positionEmp = "Owner";
                }
                //การเปลี่ยนสิทธิ์ลดระดับลงจะทำการ Clear BranchPolicy ของ User นั้นทั้งหมด 
                //จาก admin , manager -> อื่นๆ

                if (DataCashing.isModifyBranch || DataCashing.isModifyRole)
                {
                    Model.ResultAPI result = await GabanaAPI.DeleteDataBranchPolicy(txtEmployeeUserName.Text);
                    if (result == null)
                    {
                        Toast.MakeText(this, "Result DeleteDataBranchPolicy = Null ", ToastLength.Short).Show();
                        return;
                    }

                    if (!result.Status)
                    {
                        Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                        return;
                    }

                    var lstbranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, txtEmployeeUserName.Text);
                    foreach (var item in lstbranchPolicy)
                    {
                        var delete = await policyManage.DeleteBranch(DataCashingAll.MerchantId, txtEmployeeUserName.Text);
                    }

                    foreach (var item in listChooseBranch)
                    {
                        ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            UserName = txtEmployeeUserName.Text,
                            SysBranchID = (int)item.SysBranchID
                        };
                        lstbranchPolicies.Add(branchPolicy);
                    }
                }

                ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    UserName = txtEmployeeUserName.Text,
                    FUsePincode = UsePincode,
                    PinCode = null,
                    Comments = comment
                };

                Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                {
                    branchPolicy = lstbranchPolicies,
                    userAccountInfo = gbnAPIUser
                };

                if (DataCashing.addEmployeefromSeauth)
                {
                    //GabanaAPI
                    Model.ResultAPI postgbnAPIUserFromSeauth = new Model.ResultAPI(false, "");
                    postgbnAPIUserFromSeauth = await GabanaAPI.PostDataUserAccount(userAccountResult);
                    //insert ที่ gabanaAPI ไม่เสร็จ ลบที่ Seauth ด้วย
                    if (!postgbnAPIUserFromSeauth.Status)
                    {
                        Toast.MakeText(this, postgbnAPIUserFromSeauth.Message, ToastLength.Short).Show();
                        var result = await GabanaAPI.DeleteSeAuthDataUserAccount(txtEmployeeUserName.Text);
                        return;
                    }
                    //Local
                    ORM.MerchantDB.UserAccountInfo localUseraddEmployeefromSeauth = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployeeUserName.Text,
                        FUsePincode = UsePincode,
                        PinCode = null,
                        Comments = comment
                    };

                    //insert Local useraccount
                    var Updatetlocal = await accountInfoManage.UpdateUserAccount(localUseraddEmployeefromSeauth);
                    if (!Updatetlocal)
                    {
                        return;
                    }

                    //insert Local BranchPolicy
                    foreach (var itembranch in lstbranchPolicies)
                    {
                        ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                        {
                            MerchantID = itembranch.MerchantID,
                            SysBranchID = (int)itembranch.SysBranchID,
                            UserName = itembranch.UserName,
                        };
                        var insertlocalbranchPolicy = await policyManage.InsertBranchPolicy(branchPolicy);
                    }

                    Model.UserAccountInfo UserAccountInfoaddEmployeefromSeauth = new Model.UserAccountInfo()
                    {
                        UserName = txtEmployeeUserName.Text,
                        MainRoles = positionEmp,
                        UserAccessProduct = true,
                        FullName = txtEmployeeName.Text,
                        MerchantID = DataCashingAll.MerchantId,
                    };

                    DataCashingAll.UserAccountInfo.Add(UserAccountInfoaddEmployeefromSeauth);
                    ClearText();
                    EmployeeActivity.employeeActivity.Finish();
                    DataCashing.addEmployeefromSeauth = false;
                    //EmployeeActivity.flagEmployeeChange = true;
                    EmployeeActivity.SetFocusEmployee(localUseraddEmployeefromSeauth);
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeActivity)));
                    this.Finish();
                    return;
                }

                //case Update ปกติ
                //ข้อมูลที่ Seauth มี GabanaAPI ไม่มี Insert Useraccount ที่ GabanAPI
                Gabana3.JAM.UserAccount.UserAccountResult getUserfromGabana = new Gabana3.JAM.UserAccount.UserAccountResult();
                getUserfromGabana = await GabanaAPI.GetDataUserAccount(txtEmployeeUserName.Text);
                if (getUserfromGabana == null)
                {
                    var ResultpostgbnAPI = await GabanaAPI.PostDataUserAccount(userAccountResult);
                }

                var postgbnAPIUser = await GabanaAPI.PutDataUserAccount(userAccountResult);
                if (!postgbnAPIUser.Status)
                {
                    Toast.MakeText(this, postgbnAPIUser.Message, ToastLength.Short).Show();
                    return;
                }
                // Updatelocal
                ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    UserName = txtEmployeeUserName.Text,
                    FUsePincode = UsePincode,
                    PinCode = null,
                    Comments = comment
                };

                var Updatelocal = await accountInfoManage.UpdateUserAccount(localUser);
                if (!Updatelocal)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                foreach (var itembranch in lstbranchPolicies)
                {
                    ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                    {
                        MerchantID = itembranch.MerchantID,
                        SysBranchID = (int)itembranch.SysBranchID,
                        UserName = itembranch.UserName,
                    };
                    var updatelocalbranchPolicy = await policyManage.UpdateBranchPolicy(branchPolicy);
                }

                Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                {
                    UserName = txtEmployeeUserName.Text,
                    MainRoles = positionEmp,
                    UserAccessProduct = empDetail.UserAccessProduct,
                    FullName = txtEmployeeName.Text,
                    MerchantID = DataCashingAll.MerchantId,
                };

                LocalEmployee = null;
                var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName.ToLower() == txtEmployeeUserName.Text.ToLower());
                DataCashingAll.UserAccountInfo.Remove(data);
                DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                ClearText();
                EmployeeActivity.SetFocusEmployee(localUser);
                EmployeeActivity.employeeActivity.Finish();
                StartActivity(new Android.Content.Intent(Application.Context, typeof(EmployeeActivity)));
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("UpdateUserAccount at Add Emp");
                return;
            }
        }

        public async void SelectBranch()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();

                var lstBranch = new List<Branch>();
                lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);

                for (int i = 0; i < lstBranch.Count; i++)
                {
                    temp = lstBranch[i].SysBranchID.ToString();
                    temp2 = lstBranch[i].BranchName.ToString();
                    items.Add(temp2);
                    Branch.Add(temp);
                }
                DataCashing.Branch = Branch;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SelectBranch at Add Emp");
            }
        }

        void ClearText()
        {
            listChooseBranch = new List<BranchPolicy>();
            textEmpRole.Text = string.Empty;
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                SetTextBranch();
                if (string.IsNullOrEmpty(positionEmp))
                {
                    textEmpRole.Text = "เลือกบทบาทพนักงาน";
                }
                else
                {
                    textEmpRole.Text = positionEmp;
                    if (positionEmp == "Admin")
                    {
                        listChooseBranch = new List<BranchPolicy>();
                    }
                }
                CheckDataChange();
            }
            catch (Exception)
            {
                base.OnRestart();
            }

        }

        private async void SetTextBranch()
        {
            try
            {
                //Manager ->
                if (string.IsNullOrEmpty(positionEmp))
                {
                    //textEmpRole.Text = "เลือกบทบาทพนักงาน";
                    textBranch.Text = "ระบุสาขา";
                    textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.secondaryText, null));
                    return;
                }

                if (positionEmp.ToLower() != "admin" | positionEmp.ToLower() != "owner")
                {
                    var lstBranch = new List<Branch>();
                    lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                    string branchName = "";
                    textBranch.Text = "ระบุสาขา";
                    textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.secondaryText, null));

                    if (listChooseBranch.Count > 0)
                    {
                        listChooseBranch = listChooseBranch.OrderBy(x => x.SysBranchID).ToList();
                        foreach (var item in listChooseBranch)
                        {
                            var branch = lstBranch.Where(x => x.SysBranchID == item.SysBranchID).Select(x => x.BranchName).FirstOrDefault();
                            if (branchName != "")
                            {
                                branchName += "," + branch;
                            }
                            else
                            {
                                branchName = branch;
                            }
                        }
                        textBranch.Text = branchName;
                        textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    }

                    if (listChooseBranch.Count == lstBranch.Count)
                    {
                        textBranch.Text = GetString(Resource.String.addemployee_allbranch);
                    }
                }

                if (positionEmp.ToLower() == "admin" | positionEmp.ToLower() == "owner")
                {
                    //Admin  , Owner                   
                    textBranch.Text = GetString(Resource.String.addemployee_allbranch);
                    textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SetTextBranch at Add Emp");
            }
        }

        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }
    }
}