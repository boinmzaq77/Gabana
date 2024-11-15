using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_AddMemberType : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Setting_Fragment_AddMemberType NewInstance()
        {
            Setting_Fragment_AddMemberType frag = new Setting_Fragment_AddMemberType();
            return frag;
        }
        View view;
        public static Setting_Fragment_AddMemberType fragment_addmembertype;
        string  UserLogin;
        MemberType memberType;
        bool flagdatachange = false;
        private bool first;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_addmembertype, container, false);
            try
            {
                fragment_addmembertype = this;
                CombineUI();
                SetUIEvent();
                UserLogin = Preferences.Get("User", "");
                DecimalDisplay = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;
                first = false;
                SetButtonAdd(false);
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            txtDiscount.KeyPress += TxtDiscount_KeyPress;
            txtDiscount.TextChanged += TxtDiscount_TextChanged;
            txtDiscount.FocusChange += TxtDiscount_FocusChange;
            txtTypeName.TextChanged += TxtTypeName_TextChanged;
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        void SetClearTextField()
        {
            textTitle.Text = string.Empty;
            txtTypeName.Text = string.Empty;
            txtDiscount.Text = string.Empty;
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAdd.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAdd.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAdd.Enabled = enable;
        }

        double Discount;
        private void ShowdetailMembertype()
        {
            try
            {
                first = true;
                txtTypeName.Text = memberType.MemberTypeName;
                string decimalPercent = Utils.DisplayDecimal(memberType.PercentDiscount);
                Discount = Convert.ToDouble(decimalPercent);
                txtDiscount.Text = Discount + " %";
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowdetailMembertype");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        TextView textTitle;
        FrameLayout lnBack;
        EditText txtTypeName , txtDiscount;
        FrameLayout btnDelete;
        internal Button btnAdd;
        string DecimalDisplay;
        private void CombineUI()
        {
            textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
            lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
            txtTypeName = view.FindViewById<EditText>(Resource.Id.txtTypeName);
            txtDiscount = view.FindViewById<EditText> (Resource.Id.txtDiscount);
            btnDelete = view.FindViewById<FrameLayout>(Resource.Id.btnDelete);
            btnAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
        }

        private void TxtDiscount_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                //remove %
                if (txtDiscount.Text.Contains('%'))
                {
                    txtDiscount.Text = txtDiscount.Text.Replace("%", "").Trim();
                }
                else
                {
                    txtDiscount.Text = txtDiscount.Text.Trim();
                }
            }
        }

        private void TxtDiscount_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (txtDiscount.Text.Length == 0)
                {
                    Discount = 0;
                    return;
                }

                int checkIndex = 0;
                if (txtDiscount.Text.Contains('.'))
                {
                    checkIndex = txtDiscount.Text.IndexOf('.');
                    if (checkIndex == -1)
                    {
                        return;
                    }

                    if (DecimalDisplay == "2")
                    {
                        checkIndex = checkIndex + 2;
                    }

                    if (DecimalDisplay == "4")
                    {
                        checkIndex = checkIndex + 4;
                    }

                    int limitTxt = 0;
                    limitTxt = checkIndex + 1;
                    string[] txt = txtDiscount.Text.Split('.');
                    if (!string.IsNullOrEmpty(txt[1]))
                    {
                        if ((txt[1].Length) > Convert.ToInt32(DecimalDisplay) && !txtDiscount.Text.Contains('%'))
                        {
                            string DiscountAmount = txtDiscount.Text;
                            txtDiscount.Text = DiscountAmount.Remove(DiscountAmount.Length - 1);
                            txtDiscount.SetSelection(txtDiscount.Text.Length);
                            return;
                        }
                    }
                }

                if (txtDiscount.Text.Contains('.'))
                {
                    var data = AllIndexOf(txtDiscount.Text, ".", StringComparison.Ordinal);
                    if (data.Count > 1)
                    {
                        return;
                    }

                    var check = txtDiscount.Text.IndexOf('.');
                    if (check == 0)
                    {
                        txtDiscount.Text = "0.";
                        txtDiscount.SetSelection(txtDiscount.Text.Length);
                    }
                }

                string discount = string.Empty;

                if (txtDiscount.Text.Contains('%'))
                {
                    discount = txtDiscount.Text.Replace("%", "");
                }
                else
                {
                    discount = txtDiscount.Text;
                }

                discount = Regex.Replace(discount, @"\s+", "");

                Discount = Convert.ToDouble(discount);

                double maxValue = 100;
                if (maxValue < Discount)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.maxdiscount) + " " + maxValue + " %", ToastLength.Short).Show();
                    Discount = 100;
                    txtDiscount.Text = Discount.ToString();
                    txtDiscount.SetSelection(txtDiscount.Text.Length);
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtDiscount1_TextChanged at Membertype");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public IList<int> AllIndexOf(string text, string str, StringComparison comparisonType)
        {
            IList<int> allIndexOf = new List<int>();
            int index = text.IndexOf(str, comparisonType);
            while (index != -1)
            {
                allIndexOf.Add(index);
                index = text.IndexOf(str, index + 1, comparisonType);
            }
            return allIndexOf;
        }

        public static List<ORM.Master.MemberType> lstMemberType = new List<ORM.Master.MemberType>();

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                lstMemberType = new List<ORM.Master.MemberType>();
                var MastermemberType = new ORM.Master.MemberType()
                {
                    DateModified = memberType.DateModified,
                    LinkProMaxxID = memberType.LinkProMaxxID,
                    MemberTypeName = memberType.MemberTypeName,
                    MemberTypeNo = (int)memberType.MemberTypeNo,
                    MerchantID = (int)memberType.MerchantID,
                    PercentDiscount = memberType.PercentDiscount
                };
                lstMemberType.Add(MastermemberType);

                var fragment = new Setting_Dialog_DeleteMemberType();
                Setting_Dialog_DeleteMemberType dialog = new Setting_Dialog_DeleteMemberType();
                fragment.Show(Activity.SupportFragmentManager, nameof(Setting_Dialog_DeleteMemberType));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at addMember");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!DataCashing.CheckNet)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                return;
            }

            bool check;
            if (memberType == null)
            {
                check = await InsertMembertype();
                if (!check) return;
            }
            else
            {
                check = await UpdateMembertype();
                if (!check) return;
            }
            SetClearData();
        }

        MemberTypeManage memberTypeManage = new MemberTypeManage();
        ORM.Master.MemberType MastermemberType;
        ORM.MerchantDB.MemberType localmemberType;
        public static List<MemberType> lstmembertype = new List<MemberType>();

        private async Task<bool> InsertMembertype()
        {
            try
            {
                lstmembertype = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                if (lstmembertype == null)
                {
                    return false;
                }
                //กำหนด TypeNo
                int count = lstmembertype.Count;
                int MemberTypeNo = 0;

                lstMemberType = new List<ORM.Master.MemberType>();
                if (string.IsNullOrEmpty(txtTypeName.Text))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                MastermemberType = new ORM.Master.MemberType()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    LinkProMaxxID = null,
                    MemberTypeName = txtTypeName.Text,
                    MemberTypeNo = MemberTypeNo,
                    MerchantID = DataCashingAll.MerchantId,
                    PercentDiscount = (decimal)Discount
                };
                lstMemberType.Add(MastermemberType);

                //API
                var insertToAPI = await GabanaAPI.PostDataMemberType(lstMemberType);
                if (insertToAPI == null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

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
                if (!insert)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                    
                }

                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "membertype");
                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                return true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("insertMemberType at Membertype");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private async Task<bool> UpdateMembertype()
        {
            try
            {
                var lstmember = new List<ORM.Master.MemberType>();
                if (string.IsNullOrEmpty(txtTypeName.Text))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                MastermemberType = new ORM.Master.MemberType()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    LinkProMaxxID = memberType.LinkProMaxxID,
                    MemberTypeName = txtTypeName.Text,
                    MemberTypeNo = (int)memberType.MemberTypeNo,
                    MerchantID = (int)memberType.MerchantID,
                    PercentDiscount = (decimal)Discount
                };
                lstmember.Add(MastermemberType);

                //API
                var UpdateToAPI = await GabanaAPI.PutDataMemberType(lstmember);
                if (!UpdateToAPI.Status) 
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
                }

                localmemberType = new ORM.MerchantDB.MemberType()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    LinkProMaxxID = memberType.LinkProMaxxID,
                    MemberTypeName = txtTypeName.Text,
                    MemberTypeNo = memberType.MemberTypeNo,
                    MerchantID = memberType.MerchantID,
                    PercentDiscount = (decimal)Discount
                };

                //แล้วเพิ่มใหม่
                var insert = await memberTypeManage.UpdateMemberType(localmemberType);
                if (!insert)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;                     
                }

                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "membertype");
                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                return true;
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMembertype at Membertype");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private void TxtTypeName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtDiscount_KeyPress(object sender, View.KeyEventArgs e)
        {
            try
            {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    if (!txtDiscount.Text.Contains("%") && Discount != 0)
                    {
                        Discount = Convert.ToDouble(txtDiscount.Text);
                        txtDiscount.Text = txtDiscount.Text + " %";
                        txtDiscount.SetSelection(txtDiscount.Text.Length);
                    }

                    if (Discount == 0)
                    {
                        txtDiscount.Text = "0";
                    }
                }
                else
                {
                    e.Handled = false; //if you want that character appeared on the screen
                }
                CheckDataChange();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtDiscount_KeyPress at Membertype");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (memberType == null)
            {
                if (!string.IsNullOrEmpty(txtTypeName.Text))
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
            }
            else
            {
                if (txtTypeName.Text != memberType.MemberTypeName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }

                if (!string.IsNullOrEmpty(txtDiscount.Text))
                {
                    double discountmember = 0;
                    string txtDistmem;

                    if (txtDiscount.Text.Contains('%'))
                    {
                        txtDistmem = txtDiscount.Text.Replace("%", "");
                    }
                    else
                    {
                        txtDistmem = txtDiscount.Text;
                    }
                    double.TryParse(txtDistmem, out discountmember);
                    if (discountmember != (double)memberType.PercentDiscount)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                }
                SetButtonAdd(false);
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            if (!flagdatachange)
            {
                SetClearData(); return;
            }

            if (DataCashing.EditMemberType == null)
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.add_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Add_Dialog_Back.SetPage("membertype");
                Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();
                add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
            else
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Edit_Dialog_Back.SetPage("membertype");
                Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
        }

        public async void SetClearData()
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "membertype");
        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            flagdatachange = false;
            memberType = DataCashing.EditMemberType;
            if (memberType == null)
            {
                btnDelete.Visibility = ViewStates.Gone;
                textTitle.Text = GetString(Resource.String.addmembertype_activity_add);
                btnAdd.Text = GetString(Resource.String.addmembertype_activity_add);
                SetClearTextField();
            }
            else
            {
                btnDelete.Visibility = ViewStates.Visible;
                textTitle.Text = GetString(Resource.String.addmembertype_activity_edit);
                btnAdd.Text = GetString(Resource.String.textsave);
                ShowdetailMembertype();
            }
            first = false;
            SetButtonAdd(false);
        }
    }
}