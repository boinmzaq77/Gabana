using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class EmployeeRoleActivity : Activity
    {
        public static string SelectPosition;

        RecyclerView recyclerview_listemployeerole;
        ListEmpRole listEmpRole;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.employeerole_activity);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                recyclerview_listemployeerole = FindViewById<RecyclerView>(Resource.Id.recyclerview_listemployeerole);
                lnBack.Click += LnBack_Click;
                CheckJwt();
                SetEmployeeRole();

                Button btnAddEmployee = FindViewById<Button>(Resource.Id.btnAddEmployee);
                btnAddEmployee.Click += BtnAddEmployee_Click;

                _ = TinyInsights.TrackPageViewAsync("OnCreate : EmployeeRoleActivity");

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at employeeRole");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }

        }

        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            DataCashing.isModifyRole = true;
            AddEmployeeActivity.positionEmp = SelectPosition;
            this.Finish();
        }

        private void SetEmployeeRole()
        {
            List<Gabana.Model.EmployeeRole> listEmpRoles = new List<Gabana.Model.EmployeeRole>
            {
                //new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpRoleOwnerB , Position = "Owner" ,Details = "เจ้าของร้าน จัดการได้ทุกอย่าง"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpRoleAdminB , Position = "Admin" ,Details = "ผู้ดูแลระบบ จัดการระบบพนักงาน และการตั้งค่าได้"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpRoleAdminB  , Position = "Manager" ,Details = "ผู้จัดการร้าน จัดการข้อมูลบิลได้"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpRoleInvoiceOfficerB , Position = "Invoice Officer" ,Details = "พนักงาน ทำงานได้ทั้งบิลรับและบิลจ่าย"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpRoleCashierB , Position = "Cashier" ,Details = "พนักงาน ทำงานเกี่ยวข้องเฉพาะเอกสารจ่าย"  },
                new Model.EmployeeRole() { ImagePosition =  Resource.Mipmap.EmpRoleCashierB  , Position = "Editor" ,Details = "พนักงาน แก้ไขบิลได้ แต่ไม่สามารถลบบิลได้"  },
                new Model.EmployeeRole() { ImagePosition =  Resource.Mipmap.EmpRoleOfficerB  , Position = "Officer" ,Details = "พนักงาน ป้อนข้อมูลเข้าระบบ"  }
            };
            Model.GabanaModel.gabanaMain.empRole = listEmpRoles;


            listEmpRole = new ListEmpRole();
            EmployeeRole_Adapter_Main main_Adaptor = new EmployeeRole_Adapter_Main(listEmpRole);
            GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
            recyclerview_listemployeerole.SetLayoutManager(gridLayoutItem);
            recyclerview_listemployeerole.SetAdapter(main_Adaptor);
            recyclerview_listemployeerole.HasFixedSize = true;
            recyclerview_listemployeerole.SetItemViewCacheSize(20);
            main_Adaptor.ItemClick += Main_Adaptor_ItemClick;
        }

        private void Main_Adaptor_ItemClick(object sender, int e)
        {
            SelectPosition = listEmpRole[e].Position;
            EmployeeRole_Adapter_Main main_Adaptor = new EmployeeRole_Adapter_Main(listEmpRole);
            GridLayoutManager gridLayoutItem = new GridLayoutManager(this, 1, 1, false);
            recyclerview_listemployeerole.SetLayoutManager(gridLayoutItem);
            recyclerview_listemployeerole.SetAdapter(main_Adaptor);
            recyclerview_listemployeerole.HasFixedSize = true;
            recyclerview_listemployeerole.SetItemViewCacheSize(20);
            main_Adaptor.ItemClick += Main_Adaptor_ItemClick;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }
        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;

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

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

            
        }

    }
}

