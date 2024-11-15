using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using UIKit;
using UserNotifications;

namespace Gabana.iOS
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        List<SystemRevisionNo> listRivision = new List<SystemRevisionNo>();
        SystemRevisionNoManage systemRevisionNoManage;
        SystemRevisionNo SystemRevisionNo;
        #region Override Methods
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            try
            {

            
            NSDictionary userInfo = response.Notification.Request.Content.UserInfo;
            if (null != userInfo)
            {
                string action = string.Empty;
                string[] CodenoArray = null ;
                if (userInfo.ContainsKey(new NSString("action")))
                {
                    action = (userInfo[new NSString("action")] as NSString).ToString();
                    Console.WriteLine("action = " + action);
                }

                string codeno = string.Empty;
                if(userInfo.ContainsKey(new NSString("codeno")))
                {
                    codeno = (userInfo[new NSString("codeno")] as NSString).ToString();
                    Console.WriteLine("codeno = " + codeno);
                    CodenoArray = codeno.Split(',');
                    
                }

                switch (action.ToLower())
                {
                    case "item":
                        //ItemChange();
                        break;

                        //case "adjust":
                        //    MerchantsController.IsModifyMemberCards = true;
                        //    MerchantDetailController.IsModifyMemberCardsDetail = true;
                        //    SplashLoadingController.History(CodenoArray[0], CodenoArray[1]);
                        //    break;
                        //case "newsfeedpublic":
                        //    NewsFeedsController.IsModifyNewsFeed = true;
                        //    SplashLoadingController.NewsFeed(CodenoArray[0], CodenoArray[1]);
                        //    break;
                        //case "acceptmember":
                        //    MerchantsController.IsModifyMemberCards = true;
                        //    MerchantDetailController.IsModifyMemberCardsDetail = true;
                        //    SplashLoadingController.NewMember(CodenoArray[0], CodenoArray[1]);
                        //    break;
                        //case "usedmycoupon":
                        //    MyCouponController.IsModifyMyCoupon = true;
                        //    SplashLoadingController.UsedMyCoupon(CodenoArray[0], CodenoArray[1]);
                        //    break;
                        //case "voidearning":
                        //    MerchantsController.IsModifyMemberCards = true;
                        //    MerchantDetailController.IsModifyMemberCardsDetail = true;
                        //    SplashLoadingController.History(CodenoArray[0], CodenoArray[1]);
                        //    break;
                        //case "earning":
                        //    MerchantsController.IsModifyMemberCards = true;
                        //    MerchantDetailController.IsModifyMemberCardsDetail = true;
                        //    SplashLoadingController.History(CodenoArray[0], CodenoArray[1]);
                        //    break;
                        default:
                            
                        break;
                }
                
                



                //UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            }

            // Inform caller it has been handled
            completionHandler();
            }
            catch (Exception ex )
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                //Utils.ShowAlert( this.,"Error ." , "ไม่สามารถเปิดหน้าได้" );
            }
        }
        #endregion
        
    }
}