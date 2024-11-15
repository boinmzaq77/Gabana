using AutoMapper;
using CoreGraphics;
using Foundation;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using StoreKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Essentials;

namespace Gabana.iOS
{
	public class ProductViewController : UIViewController
	{

		UIButton viewButton, view2Button, view3Button, view4Button;

		UIButton oldwayButton, oldway2Button, oldway3Button;

		UILabel infoLabel;

		public ProductViewController() : base()
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Title = "Store Product View";
			View.BackgroundColor = UIColor.White;

			viewButton = UIButton.FromType(UIButtonType.RoundedRect);
			viewButton.SetTitle("Download iBooks App", UIControlState.Normal);
			viewButton.Frame = new CGRect(10, 10, 200, 40);
			viewButton.TouchUpInside += (sender, e) => {
				Buy(364709193); // iBooks
			};
			Add(viewButton);

			view2Button = UIButton.FromType(UIButtonType.RoundedRect);
			view2Button.SetTitle("Download MWC12 App", UIControlState.Normal);
			view2Button.Frame = new CGRect(10, 60, 200, 40);
			view2Button.TouchUpInside += (sender, e) => {
				Buy(496963922); // MWC
			};
			Add(view2Button);

			view3Button = UIButton.FromType(UIButtonType.RoundedRect);
			view3Button.SetTitle("Buy Timomatic Album", UIControlState.Normal);
			view3Button.Frame = new CGRect(10, 110, 200, 40);
			view3Button.TouchUpInside += (sender, e) => {
				Buy(553243272); // timomatic
			};
			Add(view3Button);

			view4Button = UIButton.FromType(UIButtonType.RoundedRect);
			view4Button.SetTitle("Read Kangaroo Book", UIControlState.Normal);
			view4Button.Frame = new CGRect(10, 160, 200, 40);
			view4Button.TouchUpInside += (sender, e) => {
				Buy(510970593); // Dot
			};
			Add(view4Button);

			#region The Old Way
			infoLabel = new UILabel(new CGRect(10, 220, 300, 40));
			infoLabel.Text = "The Old Way (OpenUrl) exits your app";

			// THE OLD WAY, OPENURL TO APPSTORE
			// This works in iOS5 and earlier, for backwards compatibility
			// It also works with the affiliate program parameters
			//      partnerId=xxxxxx&affToken=xxxxxxxx
			// http://www.apple.com/itunes/affiliates/
			oldwayButton = UIButton.FromType(UIButtonType.RoundedRect);
			oldwayButton.SetTitle("Open AppStore", UIControlState.Normal);
			oldwayButton.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
			oldwayButton.Frame = new CGRect(10, 270, 200, 40);
			oldwayButton.TouchUpInside += (sender, e) => {
				var nsurl = new NSUrl("http://itunes.apple.com/us/app/angry-birds/id343200656?mt=8");
				UIApplication.SharedApplication.OpenUrl(nsurl);
			};
			Add(oldwayButton);

			oldway2Button = UIButton.FromType(UIButtonType.RoundedRect);
			oldway2Button.SetTitle("Open iBookstore", UIControlState.Normal);
			oldway2Button.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
			oldway2Button.Frame = new CGRect(10, 320, 200, 40);
			oldway2Button.TouchUpInside += (sender, e) => {
				var nsurl = new NSUrl("http://itunes.apple.com/au/book/dot-and-the-kangaroo/id510970593?mt=11");
				UIApplication.SharedApplication.OpenUrl(nsurl);
			};
			Add(oldway2Button);

			oldway3Button = UIButton.FromType(UIButtonType.RoundedRect);
			oldway3Button.SetTitle("Open iTunes", UIControlState.Normal);
			oldway3Button.SetTitleColor(UIColor.DarkTextColor, UIControlState.Normal);
			oldway3Button.Frame = new CGRect(10, 370, 200, 40);
			oldway3Button.TouchUpInside += (sender, e) => {
				var nsurl = new NSUrl("http://itunes.apple.com/au/album/incredible/id553243272?i=553243280");
				UIApplication.SharedApplication.OpenUrl(nsurl);
			};
			Add(oldway3Button);
			#endregion

			View.AddSubview(infoLabel);
		}

		/// <summary>
		/// iOS 6 SKStoreProductViewController display
		/// </summary>
		void Buy(int productId)
		{
			Console.WriteLine("Buy " + productId);

			var spp = new StoreProductParameters(productId);

			var productViewController = new SKStoreProductViewController();
			productViewController.Finished += (sender, err) => {
				Console.WriteLine("ProductViewDelegate Finished");

				// Apple's docs says to use this
				this.DismissModalViewController(true);
			};

			productViewController.LoadProduct(spp, (ok, err) => {
				Console.WriteLine("load product");
				if (ok)
				{
					PresentModalViewController(productViewController, true);
				}
				else
				{
					Console.WriteLine(" failed ");
					if (err != null)
						Console.WriteLine(" with error " + err);
				}
			});
		}
	}
}