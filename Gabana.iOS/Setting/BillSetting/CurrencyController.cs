using AutoMapper;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
    public partial class CurrencyController : UIViewController
    {
        UICollectionView CurrencyCollectionView;

        UIView bottomView;
        UIButton btnSelect;
        public static List<Currency> currencies = new List<Currency>();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string CURRENCYSYMBOLS;
        public CurrencyController() {
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            string currencySelec = "";

            if (CURRENCYSYMBOLS != null)
            {
                currencySelec = CURRENCYSYMBOLS;
            }
            currencies.ConvertAll(x => x.Choose = false);
            var indexPath =  currencies.FindIndex(x => x.CurrencyType == currencySelec);
            currencies[indexPath].Choose = true;
            ((CurrencyDataSource)CurrencyCollectionView.DataSource).ReloadData(currencies);
            CurrencyCollectionView.ReloadData();
        }
        public async override void ViewDidLoad()
        {
            
            if (currencies == null || currencies.Count == 0)
            {
                currencies.Add(new Currency { Name = "US Dollar" ,Image = "Currency-USDg", Imagechoose = "Currency-USD", Choose = false,CurrencyType = "$" }) ;
                currencies.Add(new Currency { Name = "Thai Baht", Imagechoose = "Currency-THB", Image = "Currency-THBg", Choose = false,CurrencyType = "฿" });
                currencies.Add(new Currency { Name = "Euro", Imagechoose = "Currency-EUR", Image = "Currency-EURg", Choose = false, CurrencyType = "€" });
                currencies.Add(new Currency { Name = "Japanese Yen", Image = "Currency-JPYg", Imagechoose = "Currency-JPY", Choose = false, CurrencyType = "¥" });
                currencies.Add(new Currency { Name = "Not Displayed", Image = "Currency-NoG", Imagechoose = "Currency-No", Choose = false, CurrencyType = " " });
            }

            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            #region CurrencyCollectionView
            UICollectionViewFlowLayout itemflowLayoutList = new UICollectionViewFlowLayout();
            itemflowLayoutList.ItemSize = new CoreGraphics.CGSize(width: (View.Frame.Width), height: 80);
            itemflowLayoutList.ScrollDirection = UICollectionViewScrollDirection.Vertical;
            itemflowLayoutList.MinimumInteritemSpacing = 1;
            itemflowLayoutList.MinimumLineSpacing = 1;

            CurrencyCollectionView = new UICollectionView(frame: View.Frame, layout: itemflowLayoutList);
            CurrencyCollectionView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            CurrencyCollectionView.ShowsVerticalScrollIndicator = false;
            CurrencyCollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            CurrencyCollectionView.RegisterClassForCell(cellType: typeof(CurrencyViewCell), reuseIdentifier: "CurrencyViewCell");


            CurrencyDataSource currencyDataList = new CurrencyDataSource(currencies);
            CurrencyCollectionView.DataSource = currencyDataList;
            CurrencyCollectionDelegate CurrencyCollectionDelegate = new CurrencyCollectionDelegate();
            CurrencyCollectionDelegate.OnItemSelected += (indexPath) => {
                // do somthing
                currencies.ConvertAll(x => x.Choose = false);
                currencies[indexPath.Row].Choose = true;
                ((CurrencyDataSource)CurrencyCollectionView.DataSource).ReloadData(currencies);
                CurrencyCollectionView.ReloadData();

                btnSelect.Enabled = true;
                btnSelect.BackgroundColor = UIColor.FromRGB(51, 170, 225);
                btnSelect.SetTitleColor(UIColor.White, UIControlState.Normal);
            };
            CurrencyCollectionView.Delegate = CurrencyCollectionDelegate;
            View.AddSubview(CurrencyCollectionView);
            #endregion
            #region BottomView
            bottomView = new UIView();
            bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomView.BackgroundColor = UIColor.White;
            View.AddSubview(bottomView);

            btnSelect = new UIButton();
            btnSelect.SetTitleColor( UIColor.FromRGB(0,149,218), UIControlState.Normal);
            btnSelect.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
            btnSelect.BackgroundColor = UIColor.White;
            btnSelect.Layer.CornerRadius = 5f;
            btnSelect.Layer.BorderWidth = 0.5f;
            btnSelect.Enabled = true;
            btnSelect.SetTitle(Utils.TextBundle("save", "Save"), UIControlState.Normal);
            btnSelect.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelect.TouchUpInside += (sender, e) => {
                SelectCurrency();
            };
            View.AddSubview(btnSelect);
            #endregion
            SetupAutoLayout();
        }

        private async void SelectCurrency()
        {
            var choose = currencies.Where(x => x.Choose == true).FirstOrDefault();
            try 
            {
                var lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                var merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "CURRENCY_SYMBOLS",
                    CfgString = choose.CurrencyType
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (update.Status)
                {
                    //Insert to Local DB
                    MerchantConfig localConfig = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "CURRENCY_SYMBOLS",
                        CfgString = choose.CurrencyType
                    };
                    var localVAT = await configManage.InsertorReplacrMerchantConfig(localConfig);
                    if (localVAT)
                    {
                        //CURRENCYSYMBOLS = DataCashingAll.merchantConfig.Where(x => x.CfgKey == "CURRENCY_SYMBOLS").FirstOrDefault();
                        CURRENCYSYMBOLS = choose.CurrencyType;
                        Utils.ShowMessage(Utils.TextBundle("savedatasuccessfully", "Save data successfully"));
                        ItemsController.Ismodify = true; 
                        DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS = CURRENCYSYMBOLS;
                        //Toast.MakeText(this, "Saved", ToastLength.Short).Show();
                    }
                    this.NavigationController.PopViewController(false);
                }
                else
                {
                    Utils.ShowMessage(update.Message);
                }
            }
            catch (Exception ex)
            {
                //_ = TinyInsights.TrackErrorAsync(ex);
                Utils.ShowMessage(ex.Message);
                //Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
            //set preference
        }
        void SetupAutoLayout()
        {
            CurrencyCollectionView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            CurrencyCollectionView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            CurrencyCollectionView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            CurrencyCollectionView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;

            btnSelect.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSelect.RightAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            btnSelect.LeftAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSelect.TopAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
    public  class Currency 
    {
        public  string Name { get; set; }
        public  string Image { get; set; }
        public  bool Choose { get; set; }
        public string CurrencyType { get; set; }
        
            public string Imagechoose { get; set; }
    }
}