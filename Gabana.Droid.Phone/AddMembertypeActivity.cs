using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddMembertypeActivity : AppCompatActivity
    {
        public static AddMembertypeActivity addMembertype;
        EditText txtTypeName, txtDiscount;
        FrameLayout btnDelete;
        internal Button btnAdd;
        double Discount;
        TextView textTitle;
        private static MemberType memberType;
        MemberTypeManage memberTypeManage = new MemberTypeManage();
        private bool first;
        ORM.Master.MemberType MastermemberType;
        ORM.MerchantDB.MemberType localmemberType;
        string DecimalDisplay;
        bool flagdatachange = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addmembertype_activity_main);
                addMembertype = this;
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);
                btnBack.Click += BtnBack_Click;
                btnDelete = FindViewById<FrameLayout>(Resource.Id.btnDelete);
                btnDelete.Click += BtnDelete_Click;
                btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
                btnAdd.Click += BtnAdd_Click;
                txtTypeName = FindViewById<EditText>(Resource.Id.txtTypeName);
                txtDiscount = FindViewById<EditText>(Resource.Id.txtDiscount);
                txtDiscount.KeyPress += TxtDiscount_KeyPress;
                txtDiscount.TextChanged += TxtDiscount_TextChanged;
                txtDiscount.FocusChange += TxtDiscount_FocusChange;
                textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                txtTypeName.TextChanged += TxtTypeName_TextChanged;

                CheckJwt();
                DecimalDisplay = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY;

                if (memberType == null)
                {
                    textTitle.Text = GetString(Resource.String.addmembertype_activity_add);
                    btnAdd.Text = GetString(Resource.String.addmembertype_activity_add);
                    btnDelete.Visibility = ViewStates.Gone;                    
                }
                else
                {
                    textTitle.Text = GetString(Resource.String.addmembertype_activity_edit);
                    btnAdd.Text = GetString(Resource.String.textsave);
                    ShowdetailMembertype();
                    btnDelete.Visibility = ViewStates.Visible;                    
                }
                first = false;
                SetButtonAdd(false);
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddMembertypeActivity");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("AddMembertypeActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
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
                    var data = Utils.AllIndexOf(txtDiscount.Text, ".", StringComparison.Ordinal);
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
                    Toast.MakeText(this, GetString(Resource.String.maxdiscount) + " " + maxValue + " %", ToastLength.Short).Show();
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public string CheckLenghtValue(string strValue)
        {
            try
            {
                string pattern = "[ ]";
                string replacement = "";

                System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern);
                var check = System.Text.RegularExpressions.Regex.Replace(regEx.Replace(strValue, replacement), @"\s+", "");
                return check;
            }
            catch (Exception)
            {
                return strValue;
            }
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                List<ORM.Master.MemberType> lstmemberType = new List<ORM.Master.MemberType>();
                var MastermemberType = new ORM.Master.MemberType()
                {
                    DateModified = memberType.DateModified,
                    LinkProMaxxID = memberType.LinkProMaxxID,
                    MemberTypeName = memberType.MemberTypeName,
                    MemberTypeNo = (int)memberType.MemberTypeNo,
                    MerchantID = (int)memberType.MerchantID,
                    PercentDiscount = memberType.PercentDiscount
                };
                lstmemberType.Add(MastermemberType);
                var json = JsonConvert.SerializeObject(lstmemberType);

                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("membertype", "membertype");
                bundle.PutString("membertypedata", json);
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDelete_Click at addMember");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            btnAdd.Enabled = false;
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                btnAdd.Enabled = true;
                return;
            }
            else if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                btnAdd.Enabled = true;
                return;
            }
            else
            {
                if (memberType == null)
                {
                    InsertMembertype();
                }
                else
                {
                    UpdateMembertype();
                }
            }
            btnAdd.Enabled = true;
        }
        private async void InsertMembertype()
        {
            try
            {
                List<MemberType> lstmembertype = new List<MemberType>();
                lstmembertype = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                if (lstmembertype == null)
                {
                    return;
                }
                //กำหนด TypeNo
                int count = lstmembertype.Count;
                int MemberTypeNo = 0;
                            
                if (string.IsNullOrEmpty(txtTypeName.Text))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }

                List<ORM.Master.MemberType> lstmember = new List<ORM.Master.MemberType>();
                MastermemberType = new ORM.Master.MemberType()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    LinkProMaxxID = null,
                    MemberTypeName = txtTypeName.Text,
                    MemberTypeNo = MemberTypeNo,
                    MerchantID = DataCashingAll.MerchantId,
                    PercentDiscount = (decimal)Discount
                };
                lstmember.Add(MastermemberType);

                //API
                List<ORM.Master.MemberType> insertToAPI = new List<ORM.Master.MemberType>();
                insertToAPI = await GabanaAPI.PostDataMemberType(lstmember);
                if (insertToAPI == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
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
                bool insert = await memberTypeManage.InsertMemberType(localmemberType);
                if (!insert)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                //MemberTypeActivity.flagData = true;
                MemberTypeActivity.SetFocusMembertype(localmemberType);
                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("insertMemberType at Membertype");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private async void UpdateMembertype()
        {
            try
            {
                List<ORM.Master.MemberType> lstmember = new List<ORM.Master.MemberType>();
                lstmember = new List<ORM.Master.MemberType>();
                if (string.IsNullOrEmpty(txtTypeName.Text))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
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
                ResultAPI UpdateToAPI = await GabanaAPI.PutDataMemberType(lstmember);
                if (!UpdateToAPI.Status)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
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
                bool insert = await memberTypeManage.UpdateMemberType(localmemberType);
                if (!insert) 
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return;
                }

                //MemberTypeActivity.flagData = true;
                MemberTypeActivity.SetFocusMembertype(localmemberType);
                Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("insertMemberType at Membertype");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void TxtTypeName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
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
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAdd.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAdd.SetTextColor(Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnAdd.Enabled = enable;
        }
        private void ShowdetailMembertype()
        {
            try
            {
                txtTypeName.Text = memberType.MemberTypeName;
                string decimalPercent = Utils.DisplayDecimal(memberType.PercentDiscount);
                Discount = Convert.ToDouble(decimalPercent);
                txtDiscount.Text = Discount + " %";
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowdetailMembertype");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void BtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            MemberTypeActivity.flagData = false;
        }
        public override void OnBackPressed()
        {
            try
            {
                if (memberType == null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "membertype");
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
                    bundle.PutString("fromPage", "membertype");
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
        internal static void SetMembertype(MemberType m)
        {
            memberType = m;
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

