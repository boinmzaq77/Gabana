using System;
using System.Collections.Generic;
using System.Linq;

using UIKit;
using Foundation;
using StoreKit;


using Gabana.iOS;

namespace Gabana.iOS
{
	public class InAppPurchaseManager : PurchaseManager
	{
		protected override void CompleteTransaction (string productId)
		{
			if (productId == MainController.Buy5ProductId)
				CreditManager.Add(5); // 5 * qty
			
			else
				Console.WriteLine ("Shouldn't happen, there are only two products");
		}

		public override void RestoreTransaction (SKPaymentTransaction transaction)
		{
			throw new InvalidProgramException ("Can't restore transaction");
		}
	}
}
