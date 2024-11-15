using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class ChangePasswordActivity : AppCompatActivity
    {
        LinearLayout lnOldPass, lnNewPass, lnConfirmPass;
        EditText textOld, textNew, textConfirm;
        Button btnSave, btnChange;
        Switch switchShow;
        Model.UserAccountInfo useraccountData = new Model.UserAccountInfo();
        ORM.MerchantDB.UserAccountInfo LocalUseraccount = new ORM.MerchantDB.UserAccountInfo();
        UserAccountInfoManage UserAccountInfoManage = new UserAccountInfoManage();
        BranchPolicyManage policyManage = new BranchPolicyManage();
        List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();
        string OldPass = string.Empty;
        string emplogin = string.Empty;
        string LoginType = string.Empty;
        private static int UsePincode = 0;
        private static string NewPincode = null;
        FrameLayout lnChange;
        bool DataChang = false;
        public static string typepagepincode = null;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.changpassword_activity);

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                lnOldPass = FindViewById<LinearLayout>(Resource.Id.lnOldPass);
                lnNewPass = FindViewById<LinearLayout>(Resource.Id.lnNewPass);
                lnConfirmPass = FindViewById<LinearLayout>(Resource.Id.lnConfirmPass);
                textOld = FindViewById<EditText>(Resource.Id.textOld);
                textNew = FindViewById<EditText>(Resource.Id.textNew);
                textConfirm = FindViewById<EditText>(Resource.Id.textConfirm);
                lnChange = FindViewById<FrameLayout>(Resource.Id.lnChange);
                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnSave.Click += BtnSave_Click;
                btnChange = FindViewById<Button>(Resource.Id.btnChange);
                btnChange.Click += BtnChange_Click;
                switchShow = FindViewById<Switch>(Resource.Id.switchShow);
                
                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");

                CheckJwt();
                await GetDetail();
                switchShow.CheckedChange += SwitchShow_CheckedChange;

                if (LoginType.ToLower() == "owner")
                {
                    lnOldPass.Visibility = ViewStates.Gone;
                    lnNewPass.Visibility = ViewStates.Gone;
                    lnConfirmPass.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("changePass");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private async void CheckJwt()
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

        private void BtnChange_Click(object sender, EventArgs e)
        {
            typepagepincode = "ChangePincode";
            StartActivity(new Intent(Application.Context, typeof(PinCodeActitvity)));
            PinCodeActitvity.SetPincode(typepagepincode);

        }
        async Task GetDetail()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {
                //Load Data ใหม่ เนื่องจาก ถ้ามีการแก้ไข จะไม่ใช่ข้อมูลล่าสุด จาก Seauth
                DataCashingAll.UserAccountInfo = await GabanaAPI.GetSeAuthDataListUserAccount();
                useraccountData = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == emplogin.ToLower()).FirstOrDefault();
                //Load Data ใหม่ เนื่องจาก ถ้ามีการแก้ไข จะไม่ใช่ข้อมูลล่าสุด จาก GabanaAPI 
                var UserAccocunt = await GabanaAPI.GetDataUserAccount(useraccountData.UserName);
                if (UserAccocunt != null)
                {
                    ORM.MerchantDB.UserAccountInfo UpdatelocalUser = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = UserAccocunt.userAccountInfo.MerchantID,
                        UserName = UserAccocunt.userAccountInfo.UserName.ToLower(),
                        FUsePincode = UserAccocunt?.userAccountInfo.FUsePincode ?? 0,
                        PinCode = UserAccocunt?.userAccountInfo.PinCode ?? null,
                        Comments = UserAccocunt?.userAccountInfo.Comments,
                    };
                    await UserAccountInfoManage.InsertorReplaceUserAccount(UpdatelocalUser);
                }
                LocalUseraccount = await UserAccountInfoManage.GetUserAccount(DataCashingAll.MerchantId, useraccountData.UserName);
                if (LocalUseraccount == null)
                {                   
                    LocalUseraccount = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        UserName = emplogin,
                        Comments = "",
                        FUsePincode = 0,
                        PinCode = null
                    };
                }
                UsePincode = (int)LocalUseraccount?.FUsePincode;

                if (UsePincode == 1 && LocalUseraccount.PinCode != null)
                {
                    switchShow.Checked = true;
                    lnChange.Visibility = ViewStates.Visible;
                }
                else
                {
                    UsePincode = 0;
                    switchShow.Checked = false;
                    lnChange.Visibility = ViewStates.Gone;
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDetail at changePass");
            }
        }
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                if (UsePincode == 1)
                {
                    switchShow.Checked = true;
                }
                if (UsePincode == 0)
                {
                    switchShow.Checked = false;
                    lnChange.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception)
            {
                base.OnRestart();
            }

        }
        private void SwitchShow_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            try
            {
                if (switchShow.Checked)
                {
                    if (LocalUseraccount.FUsePincode == 0 | UsePincode == 0)
                    {
                        typepagepincode = "NewPincode";
                        StartActivity(new Intent(Application.Context, typeof(PinCodeActitvity)));
                        PinCodeActitvity.SetPincode(typepagepincode);
                    }
                    UsePincode = 1;

                }
                else
                {
                    if (LocalUseraccount.FUsePincode == 1)
                    {
                        typepagepincode = "OffPincode";

                        StartActivity(new Intent(Application.Context, typeof(PinCodeActitvity)));
                        PinCodeActitvity.SetPincode(typepagepincode);
                    }
                    UsePincode = 0;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SwitchShow_CheckedChange at changePass");
            }
        }
        //พนักงานเปลี่ยนรหัสผ่านของตัวเอง 
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string NewPass = textNew.Text;
                string ConfirmPass = textConfirm.Text;
                string OldPass = textOld.Text;

                if (!string.IsNullOrEmpty(NewPass) | !string.IsNullOrEmpty(ConfirmPass) | !string.IsNullOrEmpty(OldPass))
                {
                    // User ที่เข้าใช้งานเป็น Admin, Manager, Invoice Officer, Cashier, Editor และ Officer จะสามารถเข้ามาแก้ไขรหัสผ่าน และตั้งค่า Pin code ได้

                    if (NewPass.Length < 4)
                    {
                        Toast.MakeText(this, GetString(Resource.String.inputpasswordmin), ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(NewPass) | string.IsNullOrEmpty(ConfirmPass) | string.IsNullOrEmpty(OldPass))
                    {
                        Toast.MakeText(this, GetString(Resource.String.inputpassword), ToastLength.Short).Show();
                        return;
                    }

                    if (NewPass != ConfirmPass)
                    {
                        Toast.MakeText(this, Application.Context.GetString(Resource.String.passwordnotmath), ToastLength.Short).Show();
                        return;
                    }

                    //รหัสผ่าน
                    DataChang = true;
                    Model.ChangePassword changePassword = new ChangePassword();
                    changePassword.Username = emplogin;
                    changePassword.OldPassword = OldPass;
                    changePassword.Password = NewPass;
                    changePassword.ConfirmPassword = ConfirmPass;
                    //Employee all 
                    var checkresult = await GabanaAPI.PutSeAuthDataChangePassword(changePassword);
                    if (!checkresult.Status)
                    {
                        Toast.MakeText(this, Application.Context.GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                        return;
                    }
                    Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                    this.Finish();
                }

                if (LocalUseraccount.FUsePincode != UsePincode || LocalUseraccount.PinCode != NewPincode)
                {
                    DataChang = true;

                    if (UsePincode == 1 && NewPincode == null)
                    {
                        return;
                    }
                    if (UsePincode == 0)
                    {
                        NewPincode = null;
                    }
                    //useraccount
                    ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                    {
                        MerchantID = (int)LocalUseraccount?.MerchantID,
                        UserName = LocalUseraccount.UserName.ToLower(),
                        FUsePincode = UsePincode,
                        PinCode = NewPincode,
                        Comments = LocalUseraccount?.Comments
                    };
                    //Branch policy
                    var BranchPolicy = await policyManage.GetlstBranchPolicy(DataCashingAll.MerchantId, useraccountData?.UserName);
                    if (BranchPolicy != null)
                    {
                        foreach (var item in BranchPolicy)
                        {
                            ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                            {
                                MerchantID = (int)item.MerchantID,
                                UserName = item.UserName.ToLower(),
                                SysBranchID = (int)item.SysBranchID,
                            };
                            lstbranchPolicies.Add(branchPolicy);
                        }
                    }

                    Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                    {
                        branchPolicy = lstbranchPolicies,
                        userAccountInfo = gbnAPIUser
                    };

                    var postgbnAPIUser = await GabanaAPI.PutDataUserAccount(userAccountResult);
                    if (!postgbnAPIUser.Status)
                    {
                        Toast.MakeText(this, postgbnAPIUser.Message, ToastLength.Short).Show();
                        return;
                    }

                    // Updatelocal
                    ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                    {
                        MerchantID = (int)LocalUseraccount?.MerchantID,
                        UserName = useraccountData?.UserName.ToLower(),
                        FUsePincode = UsePincode,
                        PinCode = NewPincode,
                        Comments = LocalUseraccount?.Comments,
                    };

                    var Updatelocal = await UserAccountInfoManage.UpdateUserAccount(localUser);
                    if (!Updatelocal)
                    {
                        Toast.MakeText(this, Application.Context.GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                        return;
                    }
                    Toast.MakeText(this, Application.Context.GetString(Resource.String.savesucess), ToastLength.Short).Show();

                    foreach (var itembranch in lstbranchPolicies)
                    {
                        ORM.MerchantDB.BranchPolicy branchPolicy = new ORM.MerchantDB.BranchPolicy()
                        {
                            MerchantID = itembranch.MerchantID,
                            SysBranchID = (int)itembranch.SysBranchID,
                            UserName = itembranch.UserName.ToLower(),
                        };
                        var updatelocalbranchPolicy = await policyManage.UpdateBranchPolicy(branchPolicy);
                    }

                    Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                    {
                        UserName = useraccountData?.UserName,
                        MainRoles = useraccountData?.MainRoles,
                        UserAccessProduct = useraccountData.UserAccessProduct,
                        FullName = useraccountData?.FullName,
                        MerchantID = useraccountData.MerchantID,
                        CreatedTime = useraccountData.CreatedTime,
                        FStatus = useraccountData.FStatus,
                        LastAccessTime = useraccountData.LastAccessTime,
                        LastModifyTime = useraccountData.LastModifyTime,
                        ListSeniorStaff = useraccountData.ListSeniorStaff,
                        ListUserAccessProduct = useraccountData.ListUserAccessProduct,
                        Mobile = useraccountData.Mobile,
                        PasswordHash = useraccountData.PasswordHash,
                        PasswordSalt = useraccountData.PasswordSalt,
                        ShopID = useraccountData.ShopID,
                        TryNextTime = useraccountData.TryNextTime,
                        TryPassCount = useraccountData.TryPassCount,
                    };

                    var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName.ToLower() == useraccountData?.UserName.ToLower());
                    DataCashingAll.UserAccountInfo.Remove(data);
                    DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                    if (typepagepincode == "ChangePincode")
                    {
                        StartActivity(new Intent(Application.Context, typeof(SplashActivity)));
                    }
                    this.Finish();
                }

                if (!DataChang)
                {
                    Toast.MakeText(this, "ไม่มีการเปลี่ยนแปลงข้อมูลที่ต้องบันทึก", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at changePass");
            }
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }
        internal static void SetPinCode(string pincode, int v)
        {
            NewPincode = pincode;
            UsePincode = v;
        }
    }
}