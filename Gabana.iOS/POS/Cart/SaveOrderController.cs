using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using UIKit;

namespace Gabana.iOS
{
    public partial class SaveOrderController : UIViewController
    {
        UIView orderView, commentView, CommentView, BottomView;
        UILabel lblorder, lblcomment, lblComment;
        UITextField txtorder, txtcomment, txtComment;
        UIButton btnSave;
        TransManage transManage = new TransManage();
        Model.TranWithDetailsLocal tranWithDetails;
        public SaveOrderController() {
            this.tranWithDetails = POSController.tranWithDetails;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
            this.NavigationController.SetNavigationBarHidden(false, false);
            if (POSController.SelectedCustomer != null)
            {
                UIImageView uIImageView = new UIImageView(new CGRect(0, 0, 50, 50));
                uIImageView.Image = UIImage.FromBundle("CustB");
                UIButton btn = new UIButton();
                //btn.SetImage(UIImage.FromBundle("Cust"), default);
                btn.ImageView.BackgroundColor = UIColor.Black;
                btn.Frame = new CGRect(0, 0, 200, 50);
                btn.Layer.CornerRadius = 5f;
                btn.Layer.BorderWidth = 0.5f;
                btn.Layer.BorderColor = UIColor.FromRGB(0, 149, 218).CGColor;
                //btn.BackgroundColor = UIColor.Red;
                UILabel lab = new UILabel();
                lab.TextColor = UIColor.FromRGB(0, 149, 218);
                lab.Text = POSController.SelectedCustomer.CustomerName;
                lab.TextAlignment = UITextAlignment.Right;
                lab.TranslatesAutoresizingMaskIntoConstraints = false;
                uIImageView.TranslatesAutoresizingMaskIntoConstraints = false;
                btn.AddSubview(uIImageView);
                btn.AddSubview(lab);

                lab.RightAnchor.ConstraintEqualTo(btn.RightAnchor, -5).Active = true;
                lab.HeightAnchor.ConstraintEqualTo(50).Active = true;
                lab.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;
                uIImageView.RightAnchor.ConstraintEqualTo(lab.LeftAnchor).Active = true;
                uIImageView.LeftAnchor.ConstraintEqualTo(btn.LeftAnchor).Active = true;
                uIImageView.CenterYAnchor.ConstraintEqualTo(btn.CenterYAnchor).Active = true;

                UIBarButtonItem selectCustomer = new UIBarButtonItem(btn);
                btn.TouchUpInside += (sender, e) => {
                    var selectCustomerPage = new POSCustomerController();
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
            else
            {
                UIBarButtonItem selectCustomer = new UIBarButtonItem();
                selectCustomer.Image = UIImage.FromBundle("Cust");
                selectCustomer.Clicked += (sender, e) => {
                    var selectCustomerPage = new POSCustomerController();
                    this.NavigationController.PushViewController(selectCustomerPage, false);
                };
                this.NavigationItem.RightBarButtonItem = selectCustomer;
            }
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            POSController.tranWithDetails = this.tranWithDetails;
            this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
        }
        public override async void ViewDidLoad()
        {
            this.NavigationController.SetNavigationBarHidden(false, false);
            base.ViewDidLoad();
            try
            {
                View.BackgroundColor = UIColor.FromRGB(248, 248, 248);

                initAttribute();
                Textboxfocus(View);
                SetupAutoLayout();


                if (tranWithDetails.tran.TranType == 'O' && !string.IsNullOrEmpty( tranWithDetails.tran.Comments))
                {
                    txtcomment.Text = tranWithDetails.tran.Comments;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
            }
            
        }
        void initAttribute()
        {
            #region DeviceNoView
            orderView = new UIView();
            orderView.BackgroundColor = UIColor.White;
            orderView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblorder = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblorder.Font = lblorder.Font.WithSize(15);
            lblorder.Text = Utils.TextBundle("ordername", "Items");
            orderView.AddSubview(lblorder);

            txtorder = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            txtorder.ReturnKeyType = UIReturnKeyType.Next;
            txtorder.ShouldReturn = (tf) =>
            {
                txtcomment.BecomeFirstResponder();
                return true;
            };
            txtorder.EditingChanged += (object sender, EventArgs e) =>
            {

            };
            txtorder.AttributedPlaceholder = new NSAttributedString("", new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtorder.Text = "Order-" + DateTime.Now.ToString("HH:mm");
            txtorder.Font = txtorder.Font.WithSize(15);
            orderView.AddSubview(txtorder);
            #endregion

            #region UDIDViewView
            commentView = new UIView();
            commentView.BackgroundColor = UIColor.White;
            commentView.TranslatesAutoresizingMaskIntoConstraints = false;

            lblcomment = new UILabel
            {
                TextColor = UIColor.FromRGB(64, 64, 64),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            lblcomment.Font = lblcomment.Font.WithSize(15);
            lblcomment.TextAlignment = UITextAlignment.Left;
            lblcomment.Text = Utils.TextBundle("comment", "Items");
            
            commentView.AddSubview(lblcomment);

            txtcomment = new UITextField
            {
                BackgroundColor = UIColor.White,
                TextColor = UIColor.FromRGB(0, 149, 218),
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            
            txtcomment.ReturnKeyType = UIReturnKeyType.Done;
            txtcomment.ShouldReturn = (tf) =>
            {
                View.EndEditing(true);
                return true;
            };
            
            txtcomment.AttributedPlaceholder = new NSAttributedString(Utils.TextBundle("saveorder", "Items"), new UIStringAttributes() { ForegroundColor = UIColor.FromRGB(134, 206, 239) });
            txtcomment.Font = txtcomment.Font.WithSize(15);

            commentView.AddSubview(txtcomment);
            #endregion

            

            #region BottomView
            BottomView = new UIView();
            BottomView.BackgroundColor = UIColor.FromRGB(248, 248, 248);
            BottomView.TranslatesAutoresizingMaskIntoConstraints = false;


            btnSave = new UIButton();
            btnSave.SetTitle(Utils.TextBundle("saveorder", "Items"), UIControlState.Normal);
            btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnSave.Layer.CornerRadius = 5f;
            btnSave.BackgroundColor = UIColor.FromRGB(0, 149, 218);
            btnSave.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSave.TouchUpInside += async (sender, e) => {
                try
                {
                    btnSave.Enabled = false;

                    CultureInfo.CurrentCulture = new CultureInfo("en-US");

                    //แก้ไข order เดิมและพักบิลอีกรอบ
                     if (tranWithDetails.tran.TranType == 'O')
                    {
                        //ทำการสร้าง Order ใหม่ขึ้นมาแทน
                        tranWithDetails.tran.Status = 120;
                        tranWithDetails.tran.FWaitSending = 0;
                        var updatetTran = await transManage.UpdateTran(tranWithDetails.tran);
                        //New Tran 
                        Model.TranWithDetailsLocal TranWithDetailsnewTran = await Utils.initialData();
                        string NewTranOrder = "#" + DataCashingAll.DeviceNo.ToString() + "-" + Utils.ChangeDateTimeTranOrder(DateTime.UtcNow);

                        if (TranWithDetailsnewTran != null)
                        {
                            TranWithDetailsnewTran.tran = tranWithDetails.tran;
                            TranWithDetailsnewTran.tran.TranNo = NewTranOrder;
                            TranWithDetailsnewTran.tran.TranType = 'O';
                            TranWithDetailsnewTran.tran.Status = 100;
                            TranWithDetailsnewTran.tran.LocalDataStatus = 'I';
                            TranWithDetailsnewTran.tran.FWaitSending = 1;
                            TranWithDetailsnewTran.tran.OrderName = txtorder.Text;
                            TranWithDetailsnewTran.tran.Comments = txtcomment.Text;
                            TranWithDetailsnewTran.tran.WaitSendingTime = DateTime.UtcNow;
                            TranWithDetailsnewTran.tranDetailItemWithToppings = tranWithDetails.tranDetailItemWithToppings;
                            TranWithDetailsnewTran.tranPayments = tranWithDetails.tranPayments;
                            TranWithDetailsnewTran.tranTradDiscounts = tranWithDetails.tranTradDiscounts;

                            //แก้ไข tranNo ให้เป็นตัวใหม่
                            foreach (var item in TranWithDetailsnewTran.tranDetailItemWithToppings)
                            {
                                //TranDetail
                                item.tranDetailItem.TranNo = NewTranOrder;

                                //TranDetailTopping
                                foreach (var itemTopping in item.tranDetailItemToppings)
                                {
                                    itemTopping.TranNo = NewTranOrder;
                                }
                            }

                            //TranDiscount
                            foreach (var item in TranWithDetailsnewTran.tranTradDiscounts)
                            {
                                //TranDetail
                                item.TranNo = NewTranOrder;
                            }

                            //TranPayment
                            foreach (var item in TranWithDetailsnewTran.tranPayments)
                            {
                                //TranDetail
                                item.TranNo = NewTranOrder;
                            }

                            var insertTran = await transManage.InsertTran(TranWithDetailsnewTran);
                            if (await GabanaAPI.CheckNetWork())
                            {


                                JobQueue.Default.AddJobSendTrans((int)DataCashingAll.MerchantId, DataCashingAll.SysBranchId, TranWithDetailsnewTran.tran.TranNo);
                            }
                            else
                            {
                                TranWithDetailsnewTran.tran.Status = 100;
                                TranWithDetailsnewTran.tran.FWaitSending = 2;
                                var insertTranoffline = await transManage.UpdateTran(tranWithDetails.tran);
                            }
                        }

                        //Old Tran
                        
                    }

                    if (tranWithDetails.tran.TranType == 'B')
                    {
                        //TranType จาก 'B' -> 'O'
                        //Save order click
                        string NewTranOrder = "#" + DataCashingAll.DeviceNo.ToString() + "-" + Utils.ChangeDateTimeTranOrder(DateTime.UtcNow);
                        tranWithDetails.tran.TranNo = NewTranOrder;
                        tranWithDetails.tran.TranType = 'O';
                        tranWithDetails.tran.Status = 100;
                        tranWithDetails.tran.LocalDataStatus = 'I';
                        tranWithDetails.tran.OrderName = txtorder.Text;
                        tranWithDetails.tran.Comments = txtcomment.Text;
                        tranWithDetails.tran.FWaitSending = 1;
                        tranWithDetails.tran.WaitSendingTime = DateTime.UtcNow;
                        tranWithDetails.tran.TranDate = DateTime.UtcNow;

                        //แก้ไข tranNo ให้เป็นตัวใหม่
                        foreach (var item in tranWithDetails.tranDetailItemWithToppings)
                        {
                            //TranDetail
                            item.tranDetailItem.TranNo = NewTranOrder;

                            //TranDetailTopping
                            foreach (var itemTopping in item.tranDetailItemToppings)
                            {
                                itemTopping.TranNo = NewTranOrder;
                            }
                        }

                        //TranDiscount
                        foreach (var item in tranWithDetails.tranTradDiscounts)
                        {
                            //TranDetail
                            item.TranNo = NewTranOrder;
                        }

                        //TranPayment
                        foreach (var item in tranWithDetails.tranPayments)
                        {
                            //TranDetail
                            item.TranNo = NewTranOrder;
                        }

                        var insertToTran = await transManage.InsertTran(tranWithDetails);

                        //Cloud JobQueue
                        if (await GabanaAPI.CheckNetWork())
                        {
                            JobQueue.Default.AddJobSendTrans((int)DataCashingAll.MerchantId, DataCashingAll.SysBranchId, tranWithDetails.tran.TranNo);
                        }
                        else
                        {
                            tranWithDetails.tran.FWaitSending = 2;
                            await transManage.UpdateTran(tranWithDetails.tran);
                        }
                    }
                    NewSale();
                    btnSave.Enabled = true;
                    POSController.tranWithDetails = null;
                    POSController.SelectedCustomer = null;
                    this.NavigationController.PopToViewController(DataCaching.posPage, false);
                    
                }
                catch (Exception ex)
                {
                    btnSave.Enabled = true;
                    _ = TinyInsights.TrackErrorAsync(ex);
                    //_ = TinyInsights.TrackErrorAsync(ex);
                    //Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                }
            };
            BottomView.AddSubview(btnSave);
            #endregion

            // UIView DeviceNoView, UDIDViewView, CommentView, BottomView;
            View.AddSubview(orderView);
            View.AddSubview(commentView);
            //View.AddSubview(CommentView);
            View.AddSubview(BottomView);
            BottomView.BringSubviewToFront(btnSave);


        }
        void NewSale()
        {
            #region NewSale
            //StartActivity(new Intent(Application.Context, typeof(PosActivity)));
            //PosActivity.totlaItems = 0;
            //DataCashing.setQuantityToCart = 1;
            //POSController.tranWithDetails = null;
            //DataCashing.SysCustomerID = null;

            //if (CartActivity.cart != null)
            //{
            //    CartActivity.addRemark = false;
            //    CartActivity.cart.lnRemark.Visibility = Android.Views.ViewStates.Gone;
            //    this.Finish();
            //} 

            //if (CartScanActivity.scan != null)
            //{
            //    CartScanActivity.addRemark = false;
            //    CartScanActivity.lnRemark.Visibility = Android.Views.ViewStates.Gone;
            //    this.Finish();
            //}
            #endregion
        }
        void SetupAutoLayout()
        {
            #region BottomView
            BottomView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, 0).Active = true;
            BottomView.HeightAnchor.ConstraintEqualTo(65).Active = true;
            BottomView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            BottomView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            btnSave.TopAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.TopAnchor, 10).Active = true;
            btnSave.BottomAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.BottomAnchor, -10).Active = true;
            btnSave.LeftAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.LeftAnchor, 10).Active = true;
            btnSave.RightAnchor.ConstraintEqualTo(BottomView.SafeAreaLayoutGuide.RightAnchor, -10).Active = true;
            #endregion

            #region DeviceNoView
            orderView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 0).Active = true;
            orderView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            orderView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            orderView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblorder.CenterYAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblorder.WidthAnchor.ConstraintEqualTo(View.Frame.Height-50).Active = true;
            lblorder.LeftAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblorder.HeightAnchor.ConstraintEqualTo(18).Active = true;

            txtorder.TopAnchor.ConstraintEqualTo(lblorder.SafeAreaLayoutGuide.BottomAnchor,2).Active = true;
            txtorder.RightAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtorder.LeftAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtorder.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            #region UDIDViewView
            commentView.TopAnchor.ConstraintEqualTo(orderView.SafeAreaLayoutGuide.BottomAnchor, 1).Active = true;
            commentView.HeightAnchor.ConstraintEqualTo(60).Active = true;
            commentView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor, 0).Active = true;
            commentView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor, 0).Active = true;

            lblcomment.CenterYAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.CenterYAnchor, -12).Active = true;
            lblcomment.WidthAnchor.ConstraintEqualTo(View.Frame.Height - 50).Active = true;
            lblcomment.LeftAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            lblcomment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            //lblcomment.BackgroundColor = UIColor.Red;

            txtcomment.TopAnchor.ConstraintEqualTo(lblcomment.SafeAreaLayoutGuide.BottomAnchor, 2).Active = true;
            txtcomment.RightAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.RightAnchor, -15).Active = true;
            txtcomment.LeftAnchor.ConstraintEqualTo(commentView.SafeAreaLayoutGuide.LeftAnchor, 15).Active = true;
            txtcomment.HeightAnchor.ConstraintEqualTo(18).Active = true;
            #endregion

            
        }
        public void Textboxfocus(UIView view)
        {
            var g = new UITapGestureRecognizer(() => view.EndEditing(true));
            g.CancelsTouchesInView = false;
            view.AddGestureRecognizer(g);
        }
    }
   
}