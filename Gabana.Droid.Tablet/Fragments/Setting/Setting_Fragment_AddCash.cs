using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.ORM.Master;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_AddCash : AndroidX.Fragment.App.Fragment
    {
        public static Setting_Fragment_AddCash fragment_addcash;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_AddCash NewInstance()
        {
            Setting_Fragment_AddCash frag = new Setting_Fragment_AddCash();
            return frag;
        }

        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_addcash, container, false);
            try
            {
                fragment_addcash = this;
                ComBineUI();
                SetBtnClear();
                SetButtonAdd(false);
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            if (DataCashing.EditCashGuide == null)
            {
                SetClearTextField();
                txtDisCount.Text = "";
                txtDisCount.Hint = Utils.DisplayDecimal(0);
                txtTitle.Text = GetString(Resource.String.addcashguide_activity_title);
            }
            else
            {
                cashguide = DataCashing.EditCashGuide;
                txtDisCount.Text = "";
                txtDisCount.Hint = Utils.DisplayDecimal(cashguide.Amount);
                txtTitle.Text = GetString(Resource.String.addcashguide_activity_titleedit);

                //txtDisCount.Text = Utils.DisplayDecimal(cashguide.Amount);
            }

            strValue = txtDisCount.Text;
            DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
            currency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

            if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
            {
                if (DECIMALPOINTDISPLAY == "4")
                {
                    DECIMALPOINT = 4;
                }
                else
                {
                    DECIMALPOINT = 2;
                }
            }
            else
            {
                DECIMALPOINT = 2;
            }
            SetBtnClear();
            SetButtonAdd(false);
        }

        private void TxtDisCount_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SetBtnClear();
        }

        TextView txtTitle;
        LinearLayout lnBack;
        TextView txtDisCount, textSignMoney;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, 
                btnnumber8, btnnumber9, btnnumber0, btnClear;
        internal Button btnApplyCash;
        ImageButton btndeletenumber;
        string strValue;
        string currency, DECIMALPOINTDISPLAY;
        decimal dec;
        int count = 0, DECIMALPOINT = 2;
        string newDiscount;
        public ORM.MerchantDB.CashTemplate cashguide;
        CashTemplateManage CashTemplateManage = new CashTemplateManage();
        public bool flagdatachange = false;

        private void ComBineUI()
        {
            txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            btnpoint = view.FindViewById<Button>(Resource.Id.btnpoint);
            txtDisCount = view.FindViewById<TextView>(Resource.Id.txtDisCount);
            textSignMoney = view.FindViewById<TextView>(Resource.Id.textSignMoney);

            btnnumber0 = view.FindViewById<Button>(Resource.Id.btnnumber0);
            btnnumber1 = view.FindViewById<Button>(Resource.Id.btnnumber1);
            btnnumber2 = view.FindViewById<Button>(Resource.Id.btnnumber2);
            btnnumber3 = view.FindViewById<Button>(Resource.Id.btnnumber3);
            btnnumber4 = view.FindViewById<Button>(Resource.Id.btnnumber4);
            btnnumber5 = view.FindViewById<Button>(Resource.Id.btnnumber5);
            btnnumber6 = view.FindViewById<Button>(Resource.Id.btnnumber6);
            btnnumber7 = view.FindViewById<Button>(Resource.Id.btnnumber7);
            btnnumber8 = view.FindViewById<Button>(Resource.Id.btnnumber8);
            btnnumber9 = view.FindViewById<Button>(Resource.Id.btnnumber9);
            btndeletenumber = view.FindViewById<ImageButton>(Resource.Id.btndeletenumber);
            btnApplyCash = view.FindViewById<Button>(Resource.Id.btnApplyCash);

            btnApplyCash.Click += BtnApplyCash_Click;
            lnBack.Click += LnBack_Click;
            btndeletenumber.Click += Btndeletenumber_Click;
            btnpoint.Click += Btnpoint_Click; ;
            btnnumber0.Click += Btnnumber0_Click;
            btnnumber1.Click += Btnnumber1_Click;
            btnnumber2.Click += Btnnumber2_Click;
            btnnumber3.Click += Btnnumber3_Click;
            btnnumber4.Click += Btnnumber4_Click;
            btnnumber5.Click += Btnnumber5_Click;
            btnnumber6.Click += Btnnumber6_Click;
            btnnumber7.Click += Btnnumber7_Click;
            btnnumber8.Click += Btnnumber8_Click;
            btnnumber9.Click += Btnnumber9_Click;
            btnClear.Click += BtnClear_Click;
            txtDisCount.TextChanged += TxtDisCount_TextChanged;
        }

        private void BtnApplyCash_Click(object sender, EventArgs e)
        {
            btnApplyCash.Enabled = false;

            if (!DataCashing.CheckNet)
            {
                btnApplyCash.Enabled = true;
                Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                return;
            }

            ManageCashguide();
            btnApplyCash.Enabled = true;
        }

        private async void ManageCashguide()
        {
            bool check;
            if (DataCashing.EditCashGuide == null)
            {
                check = await InsertCashGuide();
                if (!check) return;
            }
            else
            {
                check = await UpdateCashGuide();
                if (!check) return;
            }
            SetClearData();
            Setting_Fragment_CashGuild.fragment_main.flagLoadData = true;
        }

        private async Task<bool> UpdateCashGuide()
        {
            try
            {
                List<ORM.Master.CashTemplate> lstcash = new List<ORM.Master.CashTemplate>();
                if (string.IsNullOrEmpty(txtDisCount.Text))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                ORM.Master.CashTemplate cash = new ORM.Master.CashTemplate()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    Amount = Convert.ToDecimal(strValue),
                    CashTemplateNo = (int)cashguide.CashTemplateNo,
                    MerchantID = DataCashingAll.MerchantId
                };
                lstcash.Add(cash);

                //API
                var insertToAPI = await GabanaAPI.PutDataCashTemplate(lstcash);
                if (!insertToAPI.Status)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                ORM.MerchantDB.CashTemplate cashLocal = new ORM.MerchantDB.CashTemplate()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    Amount = Convert.ToDecimal(strValue),
                    CashTemplateNo = (int)cashguide.CashTemplateNo,
                    MerchantID = DataCashingAll.MerchantId
                };

                //แล้วเพิ่มใหม่
                var update = await CashTemplateManage.UpdateCashTemplate(cashLocal);
                if (!update)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                //Setting_Fragment_CashGuild.fragment_main.ReloadCashGuide(cashLocal);
                return true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateCashGuide at Cashguid");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private  async Task<bool> InsertCashGuide()
        {
            try
            {
                List<ORM.MerchantDB.CashTemplate> lst = new List<ORM.MerchantDB.CashTemplate>();
                lst = await CashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                if (lst == null)
                {
                    return false;
                }
                //กำหนด TypeNo
                int count = lst.Count;

                List<ORM.Master.CashTemplate> lstcash = new List<ORM.Master.CashTemplate>();
                if (string.IsNullOrEmpty(txtDisCount.Text))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                ORM.Master.CashTemplate cash = new ORM.Master.CashTemplate()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    Amount = Convert.ToDecimal(strValue),
                    CashTemplateNo = count,
                    MerchantID = DataCashingAll.MerchantId
                };
                lstcash.Add(cash);

                //API
                var insertToAPI = await GabanaAPI.PostDataCashTemplate(lstcash);
                if (insertToAPI == null)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                ORM.MerchantDB.CashTemplate cashLocal = new ORM.MerchantDB.CashTemplate()
                {
                    DateModified = insertToAPI[0].DateModified,
                    Amount = Convert.ToDecimal(insertToAPI[0].Amount),
                    CashTemplateNo = insertToAPI[0].CashTemplateNo,
                    MerchantID = insertToAPI[0].MerchantID
                };

                //แล้วเพิ่มใหม่
                var insert = await CashTemplateManage.InsertCashTemplate(cashLocal);
                if (!insert)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                //Setting_Fragment_CashGuild.fragment_main.ReloadCashGuide(cashLocal);
                return true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertCashGuide at Cashguid");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                //ถ้าลบตัวเลขหมดแล้ว ให้เป็น 0.00 ที่ตัวเลขที่แสดง
                //txtDisCount.Hint = 0.00;
                //txtDisCount.Text = string.empty ;

                int indexpoint = strValue.LastIndexOf(".");
                int indexclear = 0;
                decimal damount;
                string amount;

                if (strValue.Contains('%'))
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    txtDisCount.Text = Utils.DisplayDecimal(damount);
                    strValue = txtDisCount.Text;
                    txtDisCount.Focusable = true;
                    indexclear = txtDisCount.Text.LastIndexOf(".");
                    return;
                }                

                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = strValue.Remove(strValue.Length - 1);
                    string str = "";
                    str = strValue;
                    txtDisCount.Text = Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    txtDisCount.Text = "";
                    txtDisCount.Hint = Utils.DisplayDecimal(0);
                    txtDisCount.Focusable = true;
                }
                SetBtnClear();
                CheckDataChange();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetBtnClear()
        {
            if (!string.IsNullOrEmpty(txtDisCount.Text) && Convert.ToDecimal(txtDisCount.Text) > 0)
            {
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
            }
        }
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnApplyCash.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApplyCash.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApplyCash.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApplyCash.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnApplyCash.Enabled = enable;
        }
        private void BtnClear_Click(object sender, EventArgs e)
        {
            strValue = "0";
            txtDisCount.Text = "";
            txtDisCount.Hint = Utils.DisplayDecimal(0);
            SetBtnClear();
            CheckDataChange();


            //test delete cashguide
            //await Delete();            
        }

        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber0);
        }

        bool clickPoint = false;
        private void Btnpoint_Click(object sender, EventArgs e)
        {
            clickPoint = true;
            SetValue(btnpoint);
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            if (!flagdatachange)
            {
                SetClearData(); return;
            }

            if (DataCashing.EditCashGuide == null)
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.add_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Add_Dialog_Back.SetPage("cash");
                Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();
                add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
            else
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Edit_Dialog_Back.SetPage("cash");
                Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
        }
        public void SetValue(Button btn)
        {
            try
            {
                string str = "";
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtDisCount.Text = strValue;
                }

                if (clickPoint)
                {
                    strValue += ".";
                    var data = Utils.AllIndexOf(strValue, ".", StringComparison.Ordinal);
                    if (data.Count > 1)
                    {
                        strValue = strValue.Substring(0, strValue.Length - 1);
                        clickPoint = false;
                        return;
                    }
                    str = strValue;
                    clickPoint = false;
                }
                else
                {
                    if (strValue.Contains("."))
                    {
                        var val = strValue.Split(".");
                        var sp = val[1];
                        if (sp.Length == DECIMALPOINT)
                        {
                            return;
                        }
                    }
                    str = strValue + btn.Text.ToString();
                }
                strValue = str;
                txtDisCount.Text = Utils.DisplayComma(str);

                CheckDataChange();

                string maxdata;
                if (DECIMALPOINTDISPLAY == "4")
                {
                    maxdata = Utils.DisplayDecimal((decimal)999999.9999);
                }
                else
                {
                    maxdata = Utils.DisplayDecimal((decimal)999999.99);
                }

                if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.maxamount) + " " + maxdata, ToastLength.Short).Show();
                    txtDisCount.Text = maxdata;
                    strValue = txtDisCount.Text;
                    newDiscount = strValue;
                    return;
                }

                SetBtnClear();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at addDiscount");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void CheckDataChange()
        {
           
            if (DataCashing.EditCashGuide == null)
            {
                if (!string.IsNullOrEmpty(txtDisCount.Text))
                {
                    if (Convert.ToDecimal(txtDisCount.Text) == 0)
                    {
                        SetButtonAdd(false);
                        return;
                    }
                    else
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                }
                SetButtonAdd(false);
            }
            else
            {
                if (string.IsNullOrEmpty(txtDisCount.Text))
                {
                    SetButtonAdd(false);
                    return;
                }
                if (txtDisCount.Text != Utils.DisplayDecimal(DataCashing.EditCashGuide.Amount))
                {
                    if (Convert.ToDecimal(txtDisCount.Text) == 0)
                    {
                        SetButtonAdd(false);
                        return;
                    }
                    else
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                }
                SetButtonAdd(false);
            }
        }

        public async void SetClearData()
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "cashguild");
        }

        private void SetClearTextField()
        {
            txtTitle.Text = string.Empty;
            txtDisCount.Text = string.Empty;
            textSignMoney.Text = string.Empty;
        }

        private async Task<bool> Delete()
        {
            try
            {
                if (DataCashing.EditCashGuide == null)
                {
                    return false;
                }

                List<ORM.Master.CashTemplate> lstcash = new List<ORM.Master.CashTemplate>();

                var MasterCashTemplate = new ORM.Master.CashTemplate()
                {
                    Amount = DataCashing.EditCashGuide.Amount,
                    CashTemplateNo = (int)DataCashing.EditCashGuide.CashTemplateNo,
                    MerchantID = (int)DataCashing.EditCashGuide.MerchantID,
                    DateModified = DataCashing.EditCashGuide.DateModified,
                };
                lstcash.Add(MasterCashTemplate);

                var DeleteonCloud = await GabanaAPI.DeleteDataCashTemplate(lstcash);
                if (!DeleteonCloud.Status)
                {

                    Toast.MakeText(this.Activity, DeleteonCloud.Message, ToastLength.Short).Show();
                    MainDialog.CloseDialog();
                    return false;
                }

                CashTemplateManage cashTemplateManage = new CashTemplateManage();
                var data = await cashTemplateManage.DeleteCashTemplate(DataCashingAll.MerchantId, (int)DataCashing.EditCashGuide.CashTemplateNo);
                if (!data)
                {
                    return false;
                }
                
                Setting_Fragment_CashGuild.fragment_main.DeleteCashGuid(DataCashing.EditCashGuide);
                Setting_Fragment_CashGuild.fragment_main.flagLoadData = true;
                flagdatachange = false;
                cashguide = null;
                DataCashing.EditCashGuide = null;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "cashguild");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}