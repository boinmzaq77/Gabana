using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Gabana.iOS;
using Gabana.ShareSource.Manage;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using UIKit;
using Xamarin.Essentials;
using ZXing.Mobile;

namespace Gabana.iOS
{
    public partial class POSScanBarcodeController : UIViewController
    {
        UIView topView, bottomView, leftView, rightView;
        private ZXingScannerView m_scannerView;
        string page;
        
        public POSScanBarcodeController()
        {
        }
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            m_scannerView.StopScanning();
            //m_scannerView.PauseAnalysis();
        }
        public POSScanBarcodeController(string page)
        {
            this.page = page;
        }
        public override  async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            await scannerInit();

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        }
        public override async void ViewDidLoad()
        {
            try
            {

            
                this.NavigationController.SetNavigationBarHidden(false, false);
                this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
                View.BackgroundColor = UIColor.White;
                base.ViewDidLoad();

                #region border
                topView = new UIView();
                topView.TranslatesAutoresizingMaskIntoConstraints = false;
                topView.Layer.Opacity = 0.4f;
                topView.BackgroundColor = UIColor.Black;
                View.AddSubview(topView);

                bottomView = new UIView();
                bottomView.TranslatesAutoresizingMaskIntoConstraints = false;
                bottomView.Layer.Opacity = 0.4f;
                bottomView.BackgroundColor = UIColor.Black;
                View.AddSubview(bottomView);

                leftView = new UIView();
                leftView.TranslatesAutoresizingMaskIntoConstraints = false;
                leftView.Layer.Opacity = 0.4f;
                leftView.BackgroundColor = UIColor.Black;
                View.AddSubview(leftView);

                rightView = new UIView();
                rightView.TranslatesAutoresizingMaskIntoConstraints = false;
                rightView.Layer.Opacity = 0.4f;
                rightView.BackgroundColor = UIColor.Black;
                View.AddSubview(rightView);
                #endregion
                m_scannerView =
                new ZXingScannerView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
                {
                    UseCustomOverlayView = true,
                };
                m_scannerView.TopText = "";
                m_scannerView.AutoFocus();
                m_scannerView.TranslatesAutoresizingMaskIntoConstraints = false;
                m_scannerView.ResizePreview(UIInterfaceOrientation.LandscapeLeft);
                View.AddSubview(m_scannerView);

               

                View.SendSubviewToBack(m_scannerView);
                View.BringSubviewToFront(topView);
                View.BringSubviewToFront(bottomView);
                View.BringSubviewToFront(leftView);
                View.BringSubviewToFront(rightView);
                View.SendSubviewToBack(m_scannerView);

                setupLayout();
            }
            catch (Exception ex)
            {
                _ = TinyInsightsLib.TinyInsights.TrackErrorAsync(ex, null);
                Utils.ShowAlert(this, "Error !", "10001");
            }
        }

        void setupLayout()
        {
            #region setlayout
            topView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            topView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height /3).Active = true;
            topView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            topView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            bottomView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 0).Active = true;
            bottomView.HeightAnchor.ConstraintEqualTo((int)View.Frame.Height /3).Active = true;
            bottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            bottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            leftView.TopAnchor.ConstraintEqualTo(topView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            leftView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            leftView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            leftView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width /9).Active = true;

            rightView.TopAnchor.ConstraintEqualTo(topView.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            rightView.BottomAnchor.ConstraintEqualTo(bottomView.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            rightView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
            rightView.WidthAnchor.ConstraintEqualTo((int)View.Frame.Width / 9).Active = true;
            #endregion

            m_scannerView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            m_scannerView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor,0).Active = true;
            m_scannerView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            m_scannerView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;
        }
        async Task scannerInit()
        {
            try
            {
                var barcodeOptions = new ZXing.Mobile.MobileBarcodeScanningOptions
                {
                    CameraResolutionSelector = HandleCameraResolutionSelectorDelegate,
                    AutoRotate = true,
                    //PossibleFormats = { ZXing.BarcodeFormat.QR_CODE, ZXing.BarcodeFormat.CODE_128 },
                    UseNativeScanning = true,
                    TryHarder = true,
                    DisableAutofocus = false,
                };
                
                bool scan = false;
                AVAuthorizationStatus authStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
                if (authStatus == AVAuthorizationStatus.Denied)
                {
                    //AVCaptureDevice.ShowSystemUserInterface(AVCaptureSystemUserInterface.VideoEffects);
                    Utils.ShowMessage("ไม่มีสิทการเข้าถึงกล้อง ");
                    return ;
                }
                else
                {
                    await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
                }
                
                m_scannerView.StartScanning(
                    async result =>
                    {

                        if (result != null && !string.IsNullOrEmpty(result.Text))
                        {
                            if (page == "ITEM")
                            {


                                ItemManage itemManage = new ItemManage();
                                var Itemresult = await itemManage.GetItemPOSfScanBarcode(result.Text);
                                if (Itemresult.Count != 0)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        ItemsController.isScanBarcode = true;
                                        ItemsController.txtBarcodeScan = result.Text;
                                        this.NavigationController.PopViewController(false);
                                    });
                                }
                                else
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        var duration = TimeSpan.FromSeconds(1);
                                        Vibration.Vibrate(duration);
                                        UIView.Animate(0.2, () =>
                                        {
                                            m_scannerView.Alpha = 0;
                                        }, () =>
                                        {
                                            m_scannerView.Alpha = 1;
                                        });
                                        Utils.ShowMessage("ไม่พบข้อมูล");
                                    });
                                }
                            }
                            else if (page == "ADDITEM")
                            {
                                InvokeOnMainThread(() =>
                                {
                                    AddItemControllerScroll.isScanBarcode = true;
                                    AddItemControllerScroll.txtBarcodeScan = result.Text;
                                    this.NavigationController?.PopViewController(false);
                                });
                            }
                        }
                        else
                        {
                            Utils.ShowMessage("ไม่พบรายการสินค้าดังกล่าว");
                        }

                    }, barcodeOptions);
                    
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                throw ex;
                
            }
        }
        CameraResolution HandleCameraResolutionSelectorDelegate(List<CameraResolution> availableResolutions)
        {
            //Don't know if this will ever be null or empty
            if (availableResolutions == null || availableResolutions.Count < 1)
                return new CameraResolution() { Width = 800, Height = 600 };

            //Debugging revealed that the last element in the list
            //expresses the highest resolution. This could probably be more thorough.
            return availableResolutions[availableResolutions.Count - 1];
        }
        async void HandleScanResult(ZXing.Result result)
        {
            if (page == "ITEM")
            {
                ItemsController.isScanBarcode = true;
                ItemsController.txtBarcodeScan = result.Text;
            }

        }
    }
}