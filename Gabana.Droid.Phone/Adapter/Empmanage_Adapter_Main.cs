using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace Gabana.Droid.Adapter
{
    public class Empmanage_Adapter_Main : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListEmployee listemployee;
        public string positionClick;
        char ActiveEmp;
        string emplogin;
        string LoginType;
        string username;
        string Language;
        Model.UserAccountInfo user;

        public Empmanage_Adapter_Main(ListEmployee l)
        {
            listemployee = l;
        }
        public override int ItemCount
        {
            get { return listemployee == null ? 0 : listemployee.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewEmployeeHolder vh = holder as ListViewEmployeeHolder;

                emplogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                Language = Preferences.Get("Language", "");

                var data = DataCashingAll.UserAccountInfo.Where(x => x.UserName.ToLower() == listemployee[position].UserName.ToLower()).FirstOrDefault();
                if (data != null)
                {
                    vh.EmpName.Text = listemployee[position].UserName?.ToString();

                    if (Language == "th")
                    {
                        switch (data.MainRoles)
                        {
                            case "Admin":
                                vh.EmpPosition.Text = "แอดมิน";
                                break;
                            case "Cashier":
                                vh.EmpPosition.Text = "พนักงานเก็บเงิน";
                                break;
                            case "Editor":
                                vh.EmpPosition.Text = "ผู้แก้ไข";
                                break;
                            case "Invoice":
                                vh.EmpPosition.Text = "พนักงานออกใบเสร็จ";
                                break;
                            case "Manager":
                                vh.EmpPosition.Text = "ผู้จัดการ";
                                break;
                            case "Officer":
                                vh.EmpPosition.Text = "พนักงานทั่วไป";
                                break;
                            default:
                                vh.EmpPosition.Text = "เจ้าของ";
                                break;
                        }
                    }
                    else
                    {
                        vh.EmpPosition.Text = data.MainRoles?.ToString();
                    }

                    //vh.EmpPosition.Text = data.MainRoles?.ToString();

                    //ไม่แสดง Owner
                    if (data.MainRoles?.ToLower() == "owner")
                    {
                        vh.ActiveSwich.Visibility = ViewStates.Gone;
                    }

                    if (data.UserAccessProduct)
                    {
                        vh.ActiveSwich.Checked = true;
                    }
                    else
                    {
                        vh.ActiveSwich.Checked = false;
                    }

                    //Owner
                    if (LoginType.ToLower() == "owner")
                    {
                        vh.ActiveSwich.Enabled = true;
                    }
                    else
                    {
                        //Employee
                        user = DataCashingAll.UserAccountInfo.Where(x => x.UserName == emplogin).FirstOrDefault();
                        if (user != null)
                        {
                            //Check LoginType
                            if (user.MainRoles == "Admin")
                            {
                                vh.ActiveSwich.Enabled = true;
                            }
                            else
                            {
                                vh.ActiveSwich.Enabled = false;
                            }
                        }
                    }

                    if (!EmpManageActivity.CheckNet)
                    {
                        vh.ActiveSwich.Checked = false;
                        vh.ActiveSwich.Enabled = false;
                    }
                }

                vh.ActiveSwich.CheckedChange += async (s, e) =>
                {
                    username = listemployee[position].UserName;
                    var userUpdate = DataCashingAll.UserAccountInfo.Where(x => x.UserName == username).FirstOrDefault();
                    if (vh.ActiveSwich.Checked)
                    {
                        ActiveEmp = 'A';

                        //เพิ่มข้อมูลพนักงานจาก Seauth

                        DataCashingAll.Merchant = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);

                        if (DataCashingAll.Merchant != null)
                        {
                            UserAccountInfoManage accountInfoManage = new UserAccountInfoManage();
                            var getUseraccount = await accountInfoManage.GetAllUserAccount();
                            var lstGabanaAPI = DataCashingAll.Merchant?.UserAccountInfo;

                            var results = DataCashingAll.Merchant.UserAccountInfo.Where(x => x.UserName == username).FirstOrDefault();
                            if (results == null)
                            {
                                List<BranchPolicy> listChooseBranch = new List<BranchPolicy>();
                                List<ORM.Master.BranchPolicy> lstbranchPolicies = new List<ORM.Master.BranchPolicy>();

                                if (userUpdate.MainRoles.ToLower() == "admin")
                                {
                                    listChooseBranch = new List<BranchPolicy>();
                                }
                                else
                                {
                                    ORM.Master.BranchPolicy branchPolicy = new ORM.Master.BranchPolicy()
                                    {
                                        MerchantID = (int)DataCashingAll.MerchantId,
                                        UserName = userUpdate.UserName,
                                        SysBranchID = (int)DataCashingAll.SysBranchId
                                    };
                                    lstbranchPolicies.Add(branchPolicy);
                                }

                                ORM.Master.UserAccountInfo gbnAPIUser = new ORM.Master.UserAccountInfo()
                                {
                                    MerchantID = DataCashingAll.MerchantId,
                                    UserName = userUpdate.UserName,
                                    FUsePincode = 0,
                                    PinCode = null,
                                    Comments = listemployee[position].Comments,
                                };

                                Gabana3.JAM.UserAccount.UserAccountResult userAccountResult = new Gabana3.JAM.UserAccount.UserAccountResult()
                                {
                                    branchPolicy = lstbranchPolicies,
                                    userAccountInfo = gbnAPIUser
                                };

                                var postgbnAPIUser = await GabanaAPI.PostDataUserAccount(userAccountResult);
                                if (postgbnAPIUser.Status)
                                {
                                    //Local
                                    ORM.MerchantDB.UserAccountInfo localUser = new ORM.MerchantDB.UserAccountInfo()
                                    {
                                        MerchantID = DataCashingAll.MerchantId,
                                        UserName = userUpdate.UserName,
                                        FUsePincode = 0,
                                        PinCode = null,
                                        Comments = listemployee[position].Comments,
                                    };

                                    //insert Local useraccount
                                    var Updatetlocal = await accountInfoManage.UpdateUserAccount(localUser);
                                    if (Updatetlocal)
                                    {
                                        //insert Local BranchPolicy
                                        ORM.MerchantDB.BranchPolicy branchPolicy = new BranchPolicy()
                                        {
                                            MerchantID = (int)DataCashingAll.MerchantId,
                                            UserName = userUpdate.UserName,
                                            SysBranchID = (int)DataCashingAll.SysBranchId
                                        };
                                        BranchPolicyManage policyManage = new BranchPolicyManage();
                                        var insertlocalbranchPolicy = await policyManage.InsertBranchPolicy(branchPolicy);

                                        Model.UserAccountInfo UserAccountInfo = new Model.UserAccountInfo()
                                        {
                                            UserName = userUpdate.UserName,
                                            MainRoles = userUpdate.MainRoles,
                                            UserAccessProduct = true,
                                            FullName = userUpdate.FullName,
                                        };

                                        var data = DataCashingAll.UserAccountInfo.Find(x => x.UserName == userUpdate.UserName);
                                        DataCashingAll.UserAccountInfo.Remove(data);
                                        DataCashingAll.UserAccountInfo.Add(UserAccountInfo);
                                    }

                                    if (!Updatetlocal)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        ActiveEmp = 'I';
                    }

                    var UpdateCloundProduct = await GabanaAPI.PutSeAuthDataUserAccessProducts(username.Trim(), ActiveEmp);
                    if (UpdateCloundProduct.Status)
                    {
                        Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.savesucess), ToastLength.Short).Show();
                        if (ActiveEmp == 'A')
                        {
                            userUpdate.UserAccessProduct = true;
                            vh.ActiveSwich.Checked = true;
                        }
                        else
                        {
                            userUpdate.UserAccessProduct = false;
                            vh.ActiveSwich.Checked = false;
                        }
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, UpdateCloundProduct.Message, ToastLength.Short).Show();
                        if (userUpdate.UserAccessProduct)
                        {
                            vh.ActiveSwich.Checked = true;
                        }
                        else
                        {
                            vh.ActiveSwich.Checked = false;
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.empmanage_adapter_main, parent, false);
            ListViewEmployeeHolder vh = new ListViewEmployeeHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }


    }
}