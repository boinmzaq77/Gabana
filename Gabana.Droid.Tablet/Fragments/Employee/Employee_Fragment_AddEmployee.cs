using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Employee
{
    public class Employee_Fragment_AddEmployee : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Employee_Fragment_AddEmployee NewInstance()
        {
            Employee_Fragment_AddEmployee frag = new Employee_Fragment_AddEmployee();
            return frag;
        }
        public static Employee_Fragment_AddEmployee fragment_main;
        View view;
        TextView textTitle, txtEmployeeUserName;
        public static TextView textBranch;
        public static TextView textEmpRole;
        LinearLayout lnBack, lnBranch, lnPinCode, lnRoles, lnConfirmPass;
        EditText txtEmployeeName, txtEmployeePassword, textNewPass, txtConfirm, txtComment, txtConfirmPassword;
        Button btnChangePass;
        internal Button btnAddEmployee;
        ImageButton btnDeleteChangePass, imgRoles;
        FrameLayout frameEmprole, framebranch , lnDelete;
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
        string LoginType, emplogin;
        public List<Gabana.Model.EmployeeRole> listEmpRoles { get; set; }
        bool first = true;
        public static bool flagdatachange = false;
        private string empDataComment = "";
        FrameLayout lnPassword;
        RelativeLayout lnChangPass;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.employee_fragment_addemp, container, false);
            try
            {
                fragment_main = this;
                if (DataCashing.EditEmployee != null)
                {
                    LocalEmployee = DataCashing.EditEmployee;
                }

                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "").ToLower();

                CombineUI();
                SetEventUI();
                first = false;
                return view;

            }
            catch (Exception)
            {
                return view;
            }
        }

        private void SetEventUI()
        {
            lnBack.Click += LnBack_Click;
            lnRoles.Click += LnRoles_Click;
            lnBranch.Click += LnBranch_Click;
            switchShow.CheckedChange += SwitchShow_CheckedChange;
            txtEmployeePassword.TextChanged += TxtEmployeePassword_TextChanged;
            txtComment.TextChanged += TxtComment_TextChanged;
            textNewPass.TextChanged += TextNewPass_TextChanged;
            txtConfirm.TextChanged += TxtConfirm_TextChanged;
            btnChangePass.Click += BtnChangePass_Click;
            btnDeleteChangePass.Click += BtnDeleteChangePass_Click;
            txtEmployeeName.TextChanged += TxtEmployeeName_TextChanged;
            btnAddEmployee.Click += BtnAddEmployee_Click;
            lnDelete.Click += LnShowDelete_Click;
            lnShowDelete.Click += LnShowDelete_Click;
        }

        List<BranchPolicy> lstbranchpolicy = new List<BranchPolicy>();
        BranchPolicyManage branchPolicyManage = new BranchPolicyManage();


        public override async void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                first = true;
                UINewEmployee();
                await SetUIFromData();
                first = false;
                flagdatachange = false;
                SetButtonAdd(false);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task SetUIFromData()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                if (DataCashing.EditEmployee != null)
                {
                    LocalEmployee = DataCashing.EditEmployee;
                }
                else
                {
                    LocalEmployee = null;
                }

                if (LocalEmployee != null)
                {
                    btnAddEmployee.Text = GetString(Resource.String.textsave);
                    textTitle.Text = GetString(Resource.String.employee_activity_edit);
                    lstbranchpolicy = new List<BranchPolicy>();

                    if (Employee_Fragment_Main.getlstbranchPolicy.Count == 0)
                    {
                        lstbranchpolicy = await branchPolicyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, LocalEmployee.UserName);
                    }
                    else
                    {
                        var masterbranchPolicy = Employee_Fragment_Main.getlstbranchPolicy.Where(x => x.MerchantID == LocalEmployee.MerchantID && x.UserName.ToLower() == LocalEmployee.UserName.ToLower()).ToList();
                        foreach (var itembranch in masterbranchPolicy)
                        {
                            ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                            {
                                MerchantID = itembranch.MerchantID,
                                SysBranchID = (int)itembranch.SysBranchID,
                                UserName = itembranch.UserName.ToLower(),
                            };
                            lstbranchpolicy.Add(branchPolicy);
                        }
                    }

                    await ShowUserAccountDetail();
                                        
                    txtEmployeePassword.Text = "********";
                    txtEmployeePassword.Enabled = false;                    
                    btnChangePass.Visibility = ViewStates.Visible;
                    lnPinCode.Visibility = ViewStates.Gone;
                    lnShowDelete.Visibility = ViewStates.Visible;

                    //การแสดงของข้อมูล Owner โดยที่ log in เป็น Owner
                    if (LoginType == "owner" & positionEmp.ToLower() == "owner")
                    {
                        //การแสดงของข้อมูล Owner โดยที่ log in เป็น Admin
                        lnPassword.Visibility = ViewStates.Gone;
                        lnConfirmPass.Visibility = ViewStates.Gone;
                        frameEmprole.Visibility = ViewStates.Gone;
                        framebranch.Visibility = ViewStates.Gone;
                        lnShowDelete.Visibility = ViewStates.Gone;
                        btnAddEmployee.Visibility = ViewStates.Visible;
                        lnBranch.Visibility = ViewStates.Gone;
                        txtEmployeeName.Enabled = true;
                        txtComment.Enabled = true;
                        txtEmployeeUserName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                        txtEmployeeName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        txtComment.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    } 
                    else if (LoginType == "admin" & positionEmp.ToLower() == "owner")
                    {
                        //การแสดงของข้อมูล Owner โดยที่ log in เป็น Admin
                        lnPassword.Visibility = ViewStates.Gone;
                        lnConfirmPass.Visibility = ViewStates.Gone;
                        frameEmprole.Visibility = ViewStates.Gone;
                        lnShowDelete.Visibility = ViewStates.Gone;
                        btnAddEmployee.Visibility = ViewStates.Gone;
                        lnBranch.Visibility = ViewStates.Gone;
                        txtEmployeeName.Enabled = false;
                        txtComment.Enabled = false;
                        txtEmployeeUserName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                        txtEmployeeName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                        txtComment.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                    }                   
                    else //Admin ดูคนอื่นที่ไม่ใช้เจ้าของ
                    {
                        lnPassword.Visibility = ViewStates.Visible;
                        lnConfirmPass.Visibility = ViewStates.Gone;
                        frameEmprole.Visibility = ViewStates.Visible;
                        btnChangePass.Visibility = ViewStates.Visible;
                        lnChangPass.Visibility = ViewStates.Gone;
                        lnPinCode.Visibility = ViewStates.Gone;
                        lnDelete.Visibility = ViewStates.Visible;
                        lnShowDelete.Visibility = ViewStates.Visible;
                        btnAddEmployee.Visibility = ViewStates.Visible;
                        txtComment.Enabled = true;
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
                    textTitle.Text = GetString(Resource.String.employee_activity_add);
                    btnAddEmployee.Text = Application.Context.GetString(Resource.String.employee_activity_add);

                    txtEmployeeName.Enabled = true;
                    txtEmployeePassword.Enabled = true;
                    btnChangePass.Visibility = ViewStates.Gone;
                    lnPinCode.Visibility = ViewStates.Gone;
                    lnPassword.Visibility = ViewStates.Visible;
                    lnConfirmPass.Visibility = ViewStates.Visible;
                    txtConfirmPassword.Visibility = ViewStates.Visible;
                    frameEmprole.Visibility = ViewStates.Visible;
                    lnBranch.Enabled = true;
                    txtComment.Enabled = true;
                    txtEmployeeUserName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    txtEmployeeName.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    txtComment.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    
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
                        if (!string.IsNullOrEmpty(positionEmp))
                        {
                            if (positionEmp.ToLower() == "owner" | positionEmp.ToLower() == "admin")
                            {
                                textBranch.Text = GetString(Resource.String.addemployee_allbranch);
                                textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                            }
                        }
                    }
                }

                first = false;
                SetButtonAdd(false);
                SetTextBranch();
                SelectBranch();
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

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        FrameLayout lnShowDelete;
        private void CombineUI()
        {           
            textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            txtEmployeeUserName = view.FindViewById<TextView>(Resource.Id.txtEmployeeUserName);
            txtEmployeeName  = view.FindViewById<EditText>(Resource.Id.txtEmployeeName); //Fullname
            lnPassword = view.FindViewById<FrameLayout>(Resource.Id.lnPassword);
            txtEmployeePassword = view.FindViewById<EditText>(Resource.Id.txtEmployeePassword);
            btnChangePass = view.FindViewById<Button>(Resource.Id.btnChangePass);
            btnChangePass.Visibility = ViewStates.Gone;
            lnChangPass = view.FindViewById<RelativeLayout>(Resource.Id.lnChangPass);
            lnChangPass.Visibility = ViewStates.Gone;
            textNewPass = view.FindViewById<EditText>(Resource.Id.textNewPass);
            txtConfirm = view.FindViewById<EditText>(Resource.Id.txtConfirm);
            btnDeleteChangePass = view.FindViewById<ImageButton>(Resource.Id.btnDeleteChangePass);
            frameEmprole = view.FindViewById<FrameLayout>(Resource.Id.frameEmprole);
            framebranch = view.FindViewById<FrameLayout>(Resource.Id.framebranch);
            textEmpRole = view.FindViewById<TextView>(Resource.Id.textEmpRole);
            imgRoles = view.FindViewById<ImageButton>(Resource.Id.imgRoles);
            textBranch = view.FindViewById<TextView>(Resource.Id.textBranch);
            lnBranch = view.FindViewById<LinearLayout>(Resource.Id.lnBranch);
            txtComment = view.FindViewById<EditText>(Resource.Id.txtComment);
            lnPinCode = view.FindViewById<LinearLayout>(Resource.Id.lnPinCode);
            switchShow = view.FindViewById<Switch>(Resource.Id.switchShow);
            lnShowDelete = view.FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
            lnDelete = view.FindViewById<FrameLayout>(Resource.Id.lnDelete);
            btnAddEmployee = view.FindViewById<Button>(Resource.Id.btnAddEmployee);
            lnConfirmPass = view.FindViewById<LinearLayout>(Resource.Id.lnConfirmPass);
            txtConfirmPassword = view.FindViewById<EditText>(Resource.Id.txtConfirmPassword);
            lnRoles = view.FindViewById<LinearLayout>(Resource.Id.lnRoles);

            LoginType = Preferences.Get("LoginType", "");
            LoginType = LoginType.ToLower();
            Branch = new List<string>();

            //lnPinCode.Visibility = ViewStates.Gone;
            //lnShowDelete.Visibility = ViewStates.Gone;
            //txtEmployeePassword.Enabled = true;
            //txtEmployeePassword.Hint = "********";
            //txtEmployeePassword.Text = string.Empty;

            _ = TinyInsights.TrackPageViewAsync("OnCreate : Employee_Fragment_AddEmployee");
        }

        private void BtnChangePass_Click(object sender, EventArgs e)
        {
            btnChangePass.Enabled = false;
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
                Toast.MakeText(this.Activity, GetString(Resource.String.notperm), ToastLength.Short).Show();
                btnChangePass.Enabled = true;
                return;
            }
            btnChangePass.Enabled = true;
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.passwordnotmath), ToastLength.Short).Show();
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
                        Toast.MakeText(this.Activity, result.Message, ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, resultUpdate.Message, ToastLength.Short).Show();
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
                        Toast.MakeText(this.Activity, "Result DeleteDataBranchPolicy = Null ", ToastLength.Short).Show();
                        return;
                    }
                    
                    if (!result.Status)
                    {
                        Toast.MakeText(this.Activity, result.Message, ToastLength.Short).Show();
                        return;
                    }

                    var lstbranchPolicy = lstbranchpolicy;
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

                //Update
                #region addEmployeefromSeauth
                if (DataCashing.addEmployeefromSeauth)
                {
                    //GabanaAPI
                    Model.ResultAPI postgbnAPIUserFromSeauth = new Model.ResultAPI(false, "");
                    postgbnAPIUserFromSeauth = await GabanaAPI.PostDataUserAccount(userAccountResult);
                    //insert ที่ gabanaAPI ไม่เสร็จ ลบที่ Seauth ด้วย
                    if (!postgbnAPIUserFromSeauth.Status)
                    {
                        Toast.MakeText(this.Activity, postgbnAPIUserFromSeauth.Message, ToastLength.Short).Show();
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

                    DataCashing.EditEmployee = null;
                    DataCashingAll.UserAccountInfo.Add(UserAccountInfoaddEmployeefromSeauth);
                    DataCashing.addEmployeefromSeauth = false;
                    SetClearData();
                    Employee_Fragment_Main.fragment_main.ReloadEmployee(localUseraddEmployeefromSeauth);
                    return;
                }
                #endregion

                #region case Update ปกติ                
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
                    Toast.MakeText(this.Activity, postgbnAPIUser.Message, ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
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
                SetClearData();
                Employee_Fragment_Main.fragment_main.ReloadEmployee(localUser);
                return; 
                #endregion
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("UpdateUserAccount at Add Emp");
                return;
            }
        }   

        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataCashing.EditEmployee == null)
                {
                    btnAddEmployee.Enabled = false;
                    //AddUser ที่ Seauth 
                    if (!DataCashing.CheckNet)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                        btnAddEmployee.Enabled = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(txtEmployeeName.Text))
                    {
                        txtEmployeeName.RequestFocus();
                        Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                        btnAddEmployee.Enabled = true;
                        return;
                    }
                    if (txtEmployeePassword.Text.Length < 4)
                    {
                        txtEmployeePassword.RequestFocus();
                        Toast.MakeText(this.Activity, GetString(Resource.String.inputpasswordmin), ToastLength.Short).Show();
                        btnAddEmployee.Enabled = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(txtEmployeePassword.Text))
                    {
                        txtEmployeePassword.RequestFocus();
                        Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                        btnAddEmployee.Enabled = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(txtConfirmPassword.Text))
                    {
                        txtConfirmPassword.RequestFocus();
                        Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                        btnAddEmployee.Enabled = true;
                        return;
                    }

                    string password = txtEmployeePassword.Text.Trim();
                    string confirm = txtConfirmPassword.Text.Trim();
                    if (password != confirm)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.passwordnotmath), ToastLength.Short).Show();
                        btnAddEmployee.Enabled = true;
                        return;
                    }
                    InsertEmployee();
                    btnAddEmployee.Enabled = true;
                }
                else
                {
                    btnAddEmployee.Enabled = false;
                    if (!DataCashing.CheckNet)
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                        btnAddEmployee.Enabled = true;
                        return;
                    }

                    UpdateUserAccount();
                    btnAddEmployee.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                btnAddEmployee.Enabled = true;
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAddEmployee_Click at Add Emp");
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
                confirm = txtConfirmPassword.Text.Trim();

                if (password != confirm)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.passwordnotmath), ToastLength.Short).Show();
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

                //insert Employee

                //ชื่อตรงกับที่ Seauth2
                #region addEmployeefromSeauth
                if (DataCashing.addEmployeefromSeauth)
                {
                    //สิทธิ์ในการ Login Branch
                    // Owner , Admin จะได้ All Branch ไม่ต้อง Add ข้อมูลที่ Table นี้
                    // Manager: กำหนดได้มากกว่า 1 row
                    // สิทธิ์ที่ต่ำกว่า Manager กำหนดได้แค่ 1 row เท่านั้น

                    List<ORM.Master.BranchPolicy> lstbranchPoliciesaddEmployeefromSeauth = new List<ORM.Master.BranchPolicy>();
                    //GabanaAPI

                    if (positionEmp.ToLower() == "admin")
                    {
                        listChooseBranch = new List<BranchPolicy>();
                    }
                    else
                    {
                        if (listChooseBranch.Count == 0)
                        {
                            Toast.MakeText(this.Activity, GetString(Resource.String.selectbrach), ToastLength.Short).Show();
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
                        lstbranchPoliciesaddEmployeefromSeauth.Add(branchPolicy);
                    }

                    ORM.Master.UserAccountInfo gbnAPIUseraddEmployeefromSeauth = new ORM.Master.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = txtEmployeeUserName.Text,
                        FUsePincode = UsePincode,
                        PinCode = null,
                        Comments = comment,
                    };

                    Gabana3.JAM.UserAccount.UserAccountResult userAccountResultaddEmployeefromSeauth = new Gabana3.JAM.UserAccount.UserAccountResult()
                    {
                        branchPolicy = lstbranchPoliciesaddEmployeefromSeauth,
                        userAccountInfo = gbnAPIUseraddEmployeefromSeauth
                    };

                    //GabanaAPI
                    var postgbnAPIUseraddEmployeefromSeauth = await GabanaAPI.PostDataUserAccount(userAccountResultaddEmployeefromSeauth);
                    //insert ที่ gabanaAPI ไม่เสร็จ ลบที่ Seauth ด้วย
                    if (!postgbnAPIUseraddEmployeefromSeauth.Status)
                    {
                        Toast.MakeText(this.Activity, postgbnAPIUseraddEmployeefromSeauth.Message, ToastLength.Short).Show();
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
                    foreach (var itembranch in lstbranchPoliciesaddEmployeefromSeauth)
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
                    SetClearData();
                    Employee_Fragment_Main.fragment_main.ReloadEmployee(localUseraddEmployeefromSeauth);
                    return;
                }
                #endregion

                #region ปกติ
                //Seauth
                var resultInsert = await GabanaAPI.PostSeAuthDataUserAccount(APIUser);
                if (!resultInsert.Status)
                {
                    //Pop up Error
                    Toast.MakeText(this.Activity, resultInsert.Message, ToastLength.Short).Show();
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.selectbrach), ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, postgbnAPIUser.Message, ToastLength.Short).Show();
                    var result = await GabanaAPI.DeleteSeAuthDataUserAccount(txtEmployeeUserName.Text);
                    return;
                }//Local
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
                SetClearData();
                Employee_Fragment_Main.fragment_main.ReloadEmployee(localUser);
                return; 
                #endregion
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertEmployee at Add Emp");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async Task ShowUserAccountDetail()
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
                        txtConfirmPassword.Visibility = ViewStates.Gone;

                        txtComment.Text = LocalEmployee.Comments?.ToString();

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
                         
                        oldListBrach = lstbranchpolicy;
                        listChooseBranch = lstbranchpolicy;
                        if (positionEmp.ToLower() == "owner" | positionEmp.ToLower() == "admin")
                        {
                            textBranch.Text = GetString(Resource.String.addemployee_allbranch);
                            textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("ShowUserAccountDetail at Add Emp");
            }
        }
        
        private void LnShowDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var fragment = new Employee_Dialog_Delete();
                Employee_Dialog_Delete dialog = new Employee_Dialog_Delete();
                fragment.Show(Activity.SupportFragmentManager, nameof(Employee_Dialog_Delete));
                Employee_Dialog_Delete.SetEmployeeDetail(LocalEmployee);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnShowDelete_Click at Add Emp");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnDeleteChangePass_Click(object sender, EventArgs e)
        {
            changePass = false;
            txtEmployeePassword.Enabled = true;
            btnChangePass.Enabled = true;
            btnChangePass.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            lnChangPass.Visibility = ViewStates.Gone;
        }
        private void TxtConfirm_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TextNewPass_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtEmployeeName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmployeeName.Text))
            {
                CheckDataChange();
            }            
        }

        private void TxtEmployeePassword_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }
        private void TxtComment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAddEmployee.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAddEmployee.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAddEmployee.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAddEmployee.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAddEmployee.Enabled = enable;
        }

        public void CheckDataChange()
        {
            try
            {
                if (first)
                {
                    SetButtonAdd(false);
                    flagdatachange = false;
                    return;
                }
                if (LocalEmployee == null)
                {
                    if (!string.IsNullOrEmpty(txtEmployeeName.Text))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (!string.IsNullOrEmpty(positionEmp))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (!string.IsNullOrEmpty(txtEmployeePassword.Text))
                    {
                        SetButtonAdd(false);
                        flagdatachange = true;
                        return;
                    }
                    if (txtEmployeePassword.Text.Length < 4)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (!string.IsNullOrEmpty(txtConfirmPassword.Text))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (listChooseBranch.Count == 0 )
                    {
                        if (positionEmp.ToLower() == "admin")
                        {
                            SetButtonAdd(false);
                            flagdatachange = false;
                            return;
                        }                        
                    }
                    else
                    {
                        if (positionEmp.ToLower() != "admin")
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            return;
                        }
                    }

                    SetButtonAdd(false);
                    flagdatachange = false;
                    return;
                }
                else
                {
                    if (empDetail != null)
                    {
                        if (string.IsNullOrEmpty(txtEmployeeName.Text))
                        {
                            return;
                        }
                        if (txtEmployeeName.Text != empDetail.FullName)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            return;
                        }

                        oldListBrach = lstbranchpolicy;
                        HashSet<long> sentIDs = new HashSet<long>(oldListBrach.Select(s => s.SysBranchID));
                        List<BranchPolicy> results = new List<BranchPolicy>();
                        results = listChooseBranch.Where(m => !sentIDs.Contains(m.SysBranchID)).ToList();
                        if (results.Count > 0)
                        {
                            SetButtonAdd(true);
                            flagdatachange = true;
                            return;
                        }
                    }
                    if (!string.IsNullOrEmpty(oldpositionEmp) && oldpositionEmp != positionEmp)
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.selectpositionforemp), ToastLength.Short).Show();
                    return;
                }

                if (positionEmp.ToLower() != "admin")
                {
                    Employee_Dialog_SelectBranch.SetBranch(listChooseBranch, txtEmployeeUserName.Text);
                    var fragment = new Employee_Dialog_SelectBranch();
                    Employee_Dialog_SelectBranch dialog = new Employee_Dialog_SelectBranch();
                    fragment.Show(Activity.SupportFragmentManager, nameof(Employee_Dialog_SelectBranch));
                }
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            if (!flagdatachange)
            {                
                SetClearData(); return;
            }

            if (DataCashing.EditEmployee == null)
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.add_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Add_Dialog_Back.SetPage("employee");
                Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();
                add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
            else
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Edit_Dialog_Back.SetPage("employee");
                Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
        }

        private void LnRoles_Click(object sender, EventArgs e)
        {
            if (LoginType == "owner" | LoginType == "admin")
            {
                var fragment = new Employee_Dialog_EmpRole();
                Employee_Dialog_EmpRole dialog = new Employee_Dialog_EmpRole();
                fragment.Show(Activity.SupportFragmentManager, nameof(Employee_Dialog_EmpRole));
                Employee_Dialog_EmpRole.SelectPosition = textEmpRole.Text;
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
                lstBranch = Employee_Fragment_Main.lstBranch;                    

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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SelectBranch at Add Emp");
            }
        }

        public static async void SetTextBranch()
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

                var lstBranch = new List<Branch>();
                lstBranch = Employee_Fragment_Main.lstBranch;
                string branchName = "";
                textBranch.Text = "ระบุสาขา";
                textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.secondaryText, null));

                if (positionEmp.ToLower() == "admin" | positionEmp.ToLower() == "owner")
                {
                    //Admin  , Owner                   
                    textBranch.Text = Application.Context.GetString(Resource.String.addemployee_allbranch);
                    textBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    return;
                }

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
                    textBranch.Text = Application.Context.GetString(Resource.String.addemployee_allbranch);
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("SetTextBranch at Add Emp");
            }
        }

        public void UINewEmployee()
        {
            try
            {
                textTitle.Text = string.Empty;
                txtEmployeeUserName.Text = string.Empty;
                txtEmployeeName.Text = string.Empty;
                txtEmployeePassword.Text = string.Empty;
                btnChangePass.Visibility = ViewStates.Gone;
                lnChangPass.Visibility = ViewStates.Gone;
                textNewPass.Text = string.Empty;
                txtConfirm.Text = string.Empty;
                textEmpRole.Text = string.Empty;
                textBranch.Text = string.Empty;
                txtComment.Text = string.Empty;
                lnPinCode.Visibility = ViewStates.Gone;
                switchShow.Checked = false;
                lnShowDelete.Visibility = ViewStates.Gone;
                lnDelete.Visibility = ViewStates.Gone;
                txtConfirmPassword.Text = string.Empty;

                listChooseBranch = new List<BranchPolicy>();
                textEmpRole.Text = string.Empty;

                textEmpRole.Text = "เลือกบทบาทพนักงาน";
                textBranch.Text = "ระบุสาขา";
                oldpositionEmp = string.Empty;
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UINewCustomer at Add Emp");
            }
        }

        public void SetClearData()
        {
            UINewEmployee();
            flagdatachange = false;
            DataCashing.EditEmployee = null;
            LocalEmployee = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnEmployee, "employee", "default");
        }
        
    }
}