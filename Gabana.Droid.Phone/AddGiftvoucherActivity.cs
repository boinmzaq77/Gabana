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
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddGiftvoucherActivity : AppCompatActivity
    {
        public static AddGiftvoucherActivity activity;
        EditText txtGiftCode, txtGiftName;
        static TextView textAmount;
        internal Button btnAdd;
        public static GiftVoucher GiftVoucher;
        string UserLogin, txtCurrency;
        GiftVoucherManage giftVoucherManage = new GiftVoucherManage();
        FrameLayout lnShowDelete, lnDelete;
        TextView textCurrency, textTitle;
        FrameLayout lnAddAmount;
        decimal Amount;
        bool first = true, flagdatachange = false;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addgiftvoucher_activity_main);

                activity = this;
                FrameLayout lnBack = FindViewById<FrameLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                txtGiftCode = FindViewById<EditText>(Resource.Id.txtGiftCode);
                txtGiftName = FindViewById<EditText>(Resource.Id.txtCategoryTotal);
                textAmount = FindViewById<TextView>(Resource.Id.textAmount);
                btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
                lnShowDelete = FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
                lnDelete = FindViewById<FrameLayout>(Resource.Id.lnDelete);
                lnAddAmount = FindViewById<FrameLayout>(Resource.Id.lnAddAmount);
                lnAddAmount.Click += LnAddAmount_Click;
                textCurrency = FindViewById<TextView>(Resource.Id.textCurrency);
                textCurrency.Visibility = ViewStates.Gone;
                textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                lnShowDelete.Click += LnShowDelete_Click;
                lnDelete.Click += LnShowDelete_Click;
                txtGiftCode.TextChanged += TxtGiftCode_TextChanged;
                txtGiftName.TextChanged += TxtGiftName_TextChanged;
                textAmount.Hint = Utils.DisplayDecimal(0);

                txtCurrency = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (txtCurrency == null) txtCurrency = "฿";
                UserLogin = Preferences.Get("User", "");

                CheckJwt();

                if (GiftVoucher != null)
                {
                    //edit
                    btnAdd.Text = GetString(Resource.String.textsave);
                    textTitle.Text = GetString(Resource.String.editgiftvoucher);
                    Showdata();
                    btnAdd.Click += BtnEdit_Click;
                    lnShowDelete.Visibility = ViewStates.Visible;
                    
                }
                else
                {
                    //insert
                    btnAdd.Text = GetString(Resource.String.addgiftvoucher);
                    textTitle.Text = GetString(Resource.String.addgiftvoucher);
                    btnAdd.Click += BtnAdd_Click;
                    lnShowDelete.Visibility = ViewStates.Gone;                    
                }
                SetButtonAdd(false);
                first = false;
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddGiftvoucherActivity");

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Add Gift");
                return;
            }
        }

        private void LnAddAmount_Click(object sender, EventArgs e)
        {
            try
            {
                if (GiftVoucher == null)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(GiftVoucherNumpadActivity)));
                    GiftVoucherNumpadActivity.SetAmount(textAmount.Text);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnAddAmount_Click at Add Gift");
                return;
            }
        }
        private void TxtGiftName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
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
            if (GiftVoucher == null)
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
                if (txtGiftName.Text != GiftVoucher.GiftVoucherName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
                return;
            }
        }
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnAdd.Enabled = enable;
        }       
        private void TxtGiftCode_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }
        async void Showdata()
        {
            try
            {
                lnAddAmount.Visibility = ViewStates.Gone;
                txtGiftCode.Enabled = false;
                txtGiftCode.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtGiftCode.Text = GiftVoucher.GiftVoucherCode;
                txtGiftName.Text = GiftVoucher.GiftVoucherName;
                string stringAmount;
                if (GiftVoucher.FmlAmount.Contains("%"))
                {
                    stringAmount = Utils.DisplayDecimal(Convert.ToDecimal(GiftVoucher.FmlAmount.Replace("%", ""))) + "%";
                }
                else
                {
                    stringAmount = txtCurrency + " " + Utils.DisplayDecimal(Convert.ToDecimal(GiftVoucher.FmlAmount));
                }
                textAmount.Text = stringAmount.Trim();
                textAmount.Enabled = false;
                textAmount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textblacklightcolor, null));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Showdata at Add Gift");
                return;
            }
        }
        private async void LnShowDelete_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("deleteType", "giftvoucher");
                bundle.PutString("giftVoucherCode", GiftVoucher.GiftVoucherCode);
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackPageViewAsync("LnShowDelete_Click at Add Gift");
                return;
            }
        }
        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                CheckNull();
                int? myInt = GiftVoucher.Ordinary == null ? (int?)null : Convert.ToInt32(GiftVoucher.Ordinary);

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
                    DateCreated = Utils.GetTranDate(GiftVoucher.DateCreated),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = Amount,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)GiftVoucher.MerchantID,
                    Ordinary = myInt,
                    UserNameModified = UserLogin
                };
                var result = await GabanaAPI.PutDataGiftVoucher(voucher);
                if (!result.Status) 
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                ORM.MerchantDB.GiftVoucher localvoucher = new ORM.MerchantDB.GiftVoucher()
                {
                    DateCreated = Utils.GetTranDate(GiftVoucher.DateCreated),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    FmlAmount = Amount,
                    GiftVoucherCode = txtGiftCode.Text,
                    GiftVoucherName = txtGiftName.Text,
                    MerchantID = (int)GiftVoucher.MerchantID,
                    Ordinary = myInt,
                    UserNameModified = UserLogin
                };
                var insert = await giftVoucherManage.UpdateGiftVoucher(localvoucher);

                GiftVoucher = null;
                //DataCashingAll.flagGiftVoucherChange = true;
                GiftvoucherActivity.SetFocusGiftVoucher(localvoucher);
                Toast.MakeText(this, GetString(Resource.String.editsucess), ToastLength.Short).Show();
                btnAdd.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnEdit_Click at Add Gift");
                return;
            }
        }
        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {       
                btnAdd.Enabled = false;
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show(); 
                    btnAdd.Enabled = true;
                    return;
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

                var data = GiftvoucherActivity.lstvouchers.Where(x => x.GiftVoucherCode == txtGiftCode.Text).FirstOrDefault();
                if (data != null)
                {
                    Toast.MakeText(this, GetString(Resource.String.repeatgvcode), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                var result = await GabanaAPI.PostDataGiftVoucher(voucher);
                if (!result.Status)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
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
                //DataCashingAll.flagGiftVoucherChange = true;
                GiftvoucherActivity.SetFocusGiftVoucher(localvoucher);
                Toast.MakeText(this, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                btnAdd.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdd_Click at Add Gift");
                return;
            }
        }
        void CheckNull()
        {
            if (string.IsNullOrEmpty(txtGiftCode.Text) == true | string.IsNullOrEmpty(txtGiftName.Text) == true | string.IsNullOrEmpty(txtGiftName.Text) == true)
            {
                Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                return;
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
            GiftVoucher = null;
        }
        public override void OnBackPressed()
        {
            try
            {
                if (GiftVoucher == null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "giftvoucher");
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
                    bundle.PutString("fromPage", "giftvoucher");
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
        public static void SetVoucherDetail(GiftVoucher gift)
        {
            GiftVoucher = gift;
        }
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                UserLogin = Preferences.Get("User", "");
            }
            catch (Exception)
            {
                base.OnRestart();
            }

        }
        internal static void SetAmount(string _amount)
        {
            try
            {
                textAmount.Text = _amount;
                AddGiftvoucherActivity.activity.CheckDataChange();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetAmount at add Gift");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
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