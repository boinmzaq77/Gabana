using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Items;
using LinqToDB.DataProvider.Firebird;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_AddGiftVoucher : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_AddGiftVoucher NewInstance()
        {
            Setting_Fragment_AddGiftVoucher frag = new Setting_Fragment_AddGiftVoucher();
            return frag;
        }

        View view;
        public static Setting_Fragment_AddGiftVoucher fragment_giftvoucher;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_addgiftvoucher, container, false);
            try
            {
                fragment_giftvoucher = this;
                CheckJwt();
                ComBineUI();
                SetUIEvent();
                SetButtonAdd(false);
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at AddGiftVoucher");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        private void SetUIEvent()
        {
            try
            {
                lnDelete.Click += LnDelete_Click;
                lnBack.Click += LnBack_Click;
                lnAddAmount.Click += LnAddAmount_Click;
                btnAdd.Click += BtnAdd_Click;
                txtGiftName.TextChanged += TxtGiftName_TextChanged;
                txtGiftCode.TextChanged += TxtGiftName_TextChanged;
                textAmount.TextChanged += TxtGiftName_TextChanged;
            }
            catch (Exception ex) 
            { 
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show(); 
            }
        }

        private void TxtGiftName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
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

        FrameLayout lnBack, lnAddAmount, lnShowDelete, lnDelete;
        EditText txtGiftCode, txtGiftName;
        TextView textAmount, textTitle;
        string UserLogin, txtCurrency;
        internal Button btnAdd;
        public static bool flagdatachange = false;
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();

        private void ComBineUI()
        {
            try
            {
                lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
                txtGiftCode = view.FindViewById<EditText>(Resource.Id.txtGiftCode);
                txtGiftName = view.FindViewById<EditText>(Resource.Id.txtGiftName);
                textAmount = view.FindViewById<TextView>(Resource.Id.textAmount);
                lnAddAmount = view.FindViewById<FrameLayout>(Resource.Id.lnAddAmount);
                btnAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
                lnShowDelete = view.FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
                textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
                lnDelete = view.FindViewById<FrameLayout>(Resource.Id.lnDelete);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }                       
        }

        private void LnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var fragment = new Giftvoucher_Dialog_Delete();
                fragment.Show(Activity.SupportFragmentManager, nameof(Giftvoucher_Dialog_Delete));                
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);                
                _ = TinyInsights.TrackPageViewAsync("LnShowDelete_Click at Add Gift");
                return;
            }
        }        

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAdd.Enabled = enable;
        }
                       
        void CheckNull()
        {
            if (string.IsNullOrEmpty(txtGiftCode.Text) == true | string.IsNullOrEmpty(txtGiftName.Text) == true | string.IsNullOrEmpty(txtGiftName.Text) == true)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                return;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                ManageGiftVoucher();
                btnAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("BtnAdd_Click at Add Gift");
                btnAdd.Enabled = true;
                return;
            }
        }

        private async void ManageGiftVoucher()
        {
            bool check = false;
            if (DataCashing.EditGiftVoucher == null)
            {                
                check = await InsertGiftVoucher();
                if (!check) return;                
            }
            else
            {
                check = await UpdateGiftVoucher();
                if (!check) return;
            }
            SetClearData();            
        }        

        public async void SetClearData()
        {
            try
            {
                UINewGiftVoucher();
                DataCashing.EditGiftVoucher = null;
                DataCashing.flagAmountGiftVoucher = false;
                flagdatachange = false;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "giftvoucher");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetClearData at Add Gift");
            }
        }

        public void UINewGiftVoucher()
        {
            try
            {
                textTitle.Text = string.Empty;
                txtGiftCode.Text = string.Empty;
                txtGiftName.Text = string.Empty;
                textAmount.Text = string.Empty;
                txtGiftCode.Enabled = true;
                textAmount.Enabled = true;
                lnAddAmount.Enabled = true;
                lnAddAmount.Visibility = ViewStates.Visible;
                textAmount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                txtGiftCode.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task<bool> InsertGiftVoucher()
        {
            try
            {
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return false;
                }

                CheckNull();

                string Amount = string.Empty;
                if (!string.IsNullOrEmpty(txtCurrency))
                {
                    if (textAmount.Text.Contains(txtCurrency))
                    {
                        Amount = textAmount.Text.Replace(txtCurrency, "");
                    }
                    else
                    {
                        Amount = textAmount.Text;
                    }
                }
                else
                {
                    Amount = textAmount.Text;
                }
                ORM.Master.GiftVoucher voucher = new ORM.Master.GiftVoucher()
                {
                    DateCreated = Utils.GetTranDate(DateTime.UtcNow),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = Amount,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)DataCashingAll.MerchantId,
                    Ordinary = null,
                    UserNameModified = UserLogin
                };

                var data = Setting_Fragment_GiftVoucher.lstvouchers.Where(x => x.GiftVoucherCode == txtGiftCode.Text).FirstOrDefault();
                if (data != null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.repeatgvcode), ToastLength.Short).Show();
                    return false;
                }

                var result = await GabanaAPI.PostDataGiftVoucher(voucher);
                if (!result.Status)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }

                ORM.MerchantDB.GiftVoucher localvoucher = new ORM.MerchantDB.GiftVoucher()
                {
                    DateCreated = Utils.GetTranDate(DateTime.UtcNow),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = Amount,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)DataCashingAll.MerchantId,
                    Ordinary = null,
                    UserNameModified = UserLogin
                };
                var insert = await giftVoucherManage.InsertGiftVoucher(localvoucher);
                if (!insert)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }
                Toast.MakeText(this.Activity, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                Setting_Fragment_GiftVoucher.fragment_giftvoucher.ReloadGiftVoucher(localvoucher);
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message ,ToastLength.Short).Show();
                return false;
            }
        }

        private async Task<bool> UpdateGiftVoucher()
        {
            try
            {
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return false;
                }

                CheckNull();
                int? myInt = DataCashing.EditGiftVoucher.Ordinary == null ? (int?)null : Convert.ToInt32(DataCashing.EditGiftVoucher.Ordinary);

                string Amount = string.Empty;
                if (!string.IsNullOrEmpty(txtCurrency))
                {
                    if (textAmount.Text.Contains(txtCurrency))
                    {
                        Amount = textAmount.Text.Replace(txtCurrency, "");
                    }
                    else
                    {
                        Amount = textAmount.Text;
                    }
                }
                else
                {
                    Amount = textAmount.Text;
                }

                ORM.Master.GiftVoucher voucher = new ORM.Master.GiftVoucher()
                {
                    DateCreated = Utils.GetTranDate(DataCashing.EditGiftVoucher.DateCreated),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = Amount,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)DataCashing.EditGiftVoucher.MerchantID,
                    Ordinary = myInt,
                    UserNameModified = UserLogin
                };
                var result = await GabanaAPI.PutDataGiftVoucher(voucher);
                if (!result.Status)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
                }

                ORM.MerchantDB.GiftVoucher localvoucher = new ORM.MerchantDB.GiftVoucher()
                {
                    DateCreated = Utils.GetTranDate(DataCashing.EditGiftVoucher.DateCreated),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = Amount,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)DataCashing.EditGiftVoucher.MerchantID,
                    Ordinary = myInt,
                    UserNameModified = UserLogin
                };
                var insert = await giftVoucherManage.UpdateGiftVoucher(localvoucher);
                if (!insert)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }
                Toast.MakeText(this.Activity, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                Setting_Fragment_GiftVoucher.fragment_giftvoucher.ReloadGiftVoucher(localvoucher);
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateGiftVoucher at Add Gift");
                return false;
            }
        }

        void Showdata()
        {
            try
            {
                if (DataCashing.EditGiftVoucher != null)
                {
                    lnAddAmount.Visibility = ViewStates.Gone;
                    txtGiftCode.Enabled = false;
                    txtGiftCode.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                    txtGiftCode.Text = DataCashing.EditGiftVoucher.GiftVoucherCode;
                    txtGiftName.Text = DataCashing.EditGiftVoucher.GiftVoucherName;
                    string stringAmount;
                    if (DataCashing.EditGiftVoucher.FmlAmount.Contains("%"))
                    {
                        stringAmount = Utils.DisplayDecimal(Convert.ToDecimal(DataCashing.EditGiftVoucher.FmlAmount.Replace("%", ""))) + "%";
                    }
                    else
                    {
                        stringAmount = txtCurrency + " " + Utils.DisplayDecimal(Convert.ToDecimal(DataCashing.EditGiftVoucher.FmlAmount));
                    }
                    textAmount.Text = stringAmount.Trim();
                    textAmount.Enabled = false;
                    textAmount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.nobel, null));
                }                
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Showdata at Add Gift");
                return;
            }
        }

        private async void LnAddAmount_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataCashing.EditGiftVoucher == null)
                {
                    MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "giftvouchernum");
                    Setting_Fragment_GiftVoucherNumpad.SetAmount(textAmount.Text);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnAddAmount_Click at Add Gift");
                return;
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            if (!flagdatachange)
            {
                SetClearData();
                return;
            }
             
            if (DataCashing.EditBranch == null)
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.add_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Add_Dialog_Back.SetPage("giftvoucher");
                Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();
                add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
            else
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Edit_Dialog_Back.SetPage("giftvoucher");
                Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt();
                if (DataCashing.flagAmountGiftVoucher) 
                {
                    return;
                }
                UINewGiftVoucher();
                SetDetailGiftvoucher();
                flagdatachange = false;
                SetButtonAdd(false);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetDetailGiftvoucher()
        {
            try
            {
                textAmount.Hint = Utils.DisplayDecimal(0);
                txtCurrency = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (txtCurrency == null) txtCurrency = "฿";

                UserLogin = Preferences.Get("User", "");

                if (DataCashing.EditGiftVoucher != null)
                {
                    //edit
                    btnAdd.Text = GetString(Resource.String.textsave);
                    textTitle.Text = GetString(Resource.String.editgiftvoucher);
                    lnShowDelete.Visibility = ViewStates.Visible;
                    Showdata();
                }
                else
                {
                    //insert                
                    btnAdd.Text = GetString(Resource.String.addgiftvoucher);
                    textTitle.Text = GetString(Resource.String.addgiftvoucher);
                    lnShowDelete.Visibility = ViewStates.Gone;
                }
                SetButtonAdd(false);
            }
            catch (Exception ex) 
            { 
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show(); 
            }
        }

        string strtype = string.Empty;
        internal void SetAddAmount(string _amount,string btntype)
        {
            try
            {
                decimal resultamount = 0;
                string value = string.Empty;
                value = _amount;
                if (value.Contains("%"))
                {
                    value = value.Replace("%","");
                }
                else
                {
                    value = value.Replace(txtCurrency, "");
                }

                if (btntype == "%")
                {
                    decimal.TryParse(value, out  resultamount);
                    var dec = DataCashingAll.setmerchantConfig?.DECIMAL_POINT_DISPLAY;
                    if (dec == "4")
                    {
                        textAmount.Text = resultamount.ToString("#,###.####") + btntype;
                    }
                    else
                    {
                        textAmount.Text = resultamount.ToString("#,###.##") + btntype;
                    }
                }
                else
                {
                    decimal.TryParse(value, out resultamount);
                    textAmount.Text = btntype + " " + Utils.DisplayDecimal(resultamount);
                }

                CheckDataChange();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetAddAmount at add Gift");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void CheckDataChange()
        {
            if (DataCashing.EditGiftVoucher == null)
            {
                if (!string.IsNullOrEmpty(txtGiftCode.Text) || !string.IsNullOrEmpty(txtGiftName.Text) || !string.IsNullOrEmpty(textAmount.Text))
                {
                    flagdatachange = true;
                }

                if (string.IsNullOrEmpty(txtGiftCode.Text))
                {
                    SetButtonAdd(false);
                    return;
                }
                if (string.IsNullOrEmpty(txtGiftName.Text))
                {
                    SetButtonAdd(false);
                    return;
                }
                if (string.IsNullOrEmpty(textAmount.Text))
                {
                    SetButtonAdd(false);
                    return;
                }
                SetButtonAdd(true);
            }
            else
            {
                if (txtGiftName.Text != DataCashing.EditGiftVoucher.GiftVoucherName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
                return;
            }
        }
    }
}