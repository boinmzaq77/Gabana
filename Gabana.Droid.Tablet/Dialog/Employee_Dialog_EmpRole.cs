using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Employee;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana3.JAM.Trans;
using Java.Lang.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Employee_Dialog_EmpRole : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Employee_Dialog_EmpRole NewInstance()
        {
            var frag = new Employee_Dialog_EmpRole { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public static string SelectPosition;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.employee_dialog_emprole, container, false);
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();
                SetEmployeeRole();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Option");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }
        LinearLayout lnBack;
        RecyclerView rcvlistemployeerole;
        Button btnApply;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            rcvlistemployeerole = view.FindViewById<RecyclerView>(Resource.Id.rcvlistemployeerole);
            btnApply = view.FindViewById<Button>(Resource.Id.btnApply);
            lnBack.Click += LnBack_Click;
            btnApply.Click += BtnApply_Click;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            DataCashing.isModifyRole = true;
            Employee_Fragment_AddEmployee.positionEmp = SelectPosition;
            Employee_Fragment_AddEmployee.textEmpRole.Text = SelectPosition;
            Employee_Fragment_AddEmployee.listChooseBranch = new List<ORM.MerchantDB.BranchPolicy>(); 
            Employee_Fragment_AddEmployee.SetTextBranch();
            Employee_Fragment_AddEmployee.fragment_main.CheckDataChange();
            Dialog.Dismiss();
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            Dialog.Dismiss();
        }

        ListEmpRole listEmpRole;
        private void SetEmployeeRole()
        {
            List<Gabana.Model.EmployeeRole> listEmpRoles = new List<Gabana.Model.EmployeeRole>
            {
                //new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpRoleOwnerB , Position = "Owner" ,Details = "เจ้าของร้าน จัดการได้ทุกอย่าง"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpAdminB , Position = "Admin" ,Details = "ผู้ดูแลระบบ จัดการระบบพนักงาน และการตั้งค่าได้"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpAdminB  , Position = "Manager" ,Details = "ผู้จัดการร้าน จัดการข้อมูลบิลได้"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpInvoiceOfficerB , Position = "Invoice Officer" ,Details = "พนักงาน ทำงานได้ทั้งบิลรับและบิลจ่าย"  },
                new Model.EmployeeRole() { ImagePosition = Resource.Mipmap.EmpCashierB , Position = "Cashier" ,Details = "พนักงาน ทำงานเกี่ยวข้องเฉพาะเอกสารจ่าย"  },
                new Model.EmployeeRole() { ImagePosition =  Resource.Mipmap.EmpCashierB  , Position = "Editor" ,Details = "พนักงาน แก้ไขบิลได้ แต่ไม่สามารถลบบิลได้"  },
                new Model.EmployeeRole() { ImagePosition =  Resource.Mipmap.EmpOfficerB  , Position = "Officer" ,Details = "พนักงาน ป้อนข้อมูลเข้าระบบ"  }
            };
            Model.GabanaModel.gabanaMain.empRole = listEmpRoles;


            listEmpRole = new ListEmpRole();
            Employee_Adapter_EmpRole main_Adaptor = new Employee_Adapter_EmpRole(listEmpRole);
            GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
            rcvlistemployeerole.SetLayoutManager(gridLayoutItem);
            rcvlistemployeerole.SetAdapter(main_Adaptor);
            rcvlistemployeerole.HasFixedSize = true;
            rcvlistemployeerole.SetItemViewCacheSize(20);
            main_Adaptor.ItemClick += Main_Adaptor_ItemClick; ;
        }

        private void Main_Adaptor_ItemClick(object sender, int e)
        {
            try
            {
                SelectPosition = listEmpRole[e].Position;
                Employee_Adapter_EmpRole main_Adaptor = new Employee_Adapter_EmpRole(listEmpRole);
                GridLayoutManager gridLayoutItem = new GridLayoutManager(this.Activity, 1, 1, false);
                rcvlistemployeerole.SetLayoutManager(gridLayoutItem);
                rcvlistemployeerole.SetAdapter(main_Adaptor);
                rcvlistemployeerole.HasFixedSize = true;
                rcvlistemployeerole.SetItemViewCacheSize(20);
                main_Adaptor.ItemClick += Main_Adaptor_ItemClick;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }
    }
}