using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System.Threading.Tasks;
using Gabana.ShareSource;
using TinyInsightsLib;

namespace Gabana.iOS
{
    public class CartDataSource : UICollectionViewDataSource
    {
       ItemManage itemmanager = new ItemManage();
        CoreGraphics.CGRect frame;
        private List<TranDetailItemWithTopping> tranDetailItems;
        UICollectionView collectionView;
        nfloat height = 0 ;
        string CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        //List<string> demo = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "g", "g", "g", "g", "g", "g", "g", "g", "g" };

        public CartDataSource(List<TranDetailItemWithTopping> tranDetailItems, CoreGraphics.CGRect frame)
        {
            this.frame = frame;
            this.tranDetailItems = tranDetailItems;
            
        }
        public void ReloadData(List<TranDetailItemWithTopping> tranDetailItems)
        {
            this.tranDetailItems = tranDetailItems;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            UICollectionViewCell cell = new UICollectionViewCell();
            try
            {
                this.collectionView = collectionView;
                
                    
                var cell2 = collectionView.DequeueReusableCell("CartCell2", indexPath) as CartCollectionViewCell2;
                if (tranDetailItems[indexPath.Row].tranDetailItem.SysItemID == null)
                {
                    cell2.Dummy = true;
                }
                 

                cell2.Name = tranDetailItems[indexPath.Row].tranDetailItem.ItemName+ " " + tranDetailItems[indexPath.Row].tranDetailItem.SizeName;
                cell2.amount = tranDetailItems[indexPath.Row].tranDetailItem.Quantity.ToString("#,##0 x");
                cell2.price = Utils.DisplayDecimal(tranDetailItems[indexPath.Row].tranDetailItem.Amount);
                cell2.priceperamount = CURRENCYSYMBOLS+Utils.DisplayDecimal(tranDetailItems[indexPath.Row].tranDetailItem.ItemPrice);
                cell2.Height = tranDetailItems[indexPath.Row].tranDetailItemToppings.Count * 25;
                cell2.size = frame.Width;
                //.imgchangepriceset = true;
                
                cell2.toppinglist = tranDetailItems[indexPath.Row].tranDetailItemToppings;
                //cell2.showhead = 50; 
                cell2.OnCardCellQRCodeBtn0 -= Cell2_OnCardCellQRCodeBtn0;
                cell2.OnCardCellQRCodeBtn0 += Cell2_OnCardCellQRCodeBtn0;

                cell2.OnCardCellQRCodeBtn1 -= Cell2_OnCardCellQRCodeBtn1;
                cell2.OnCardCellQRCodeBtn1 += Cell2_OnCardCellQRCodeBtn1;

                cell2.OnCardCellQRCodeBtn2 -= Cell2_OnCardCellQRCodeBtn2;
                cell2.OnCardCellQRCodeBtn2 += Cell2_OnCardCellQRCodeBtn2;

                cell2.OnCardCellQRCodeBtn3 -= Cell2_OnCardCellQRCodeBtn3;
                cell2.OnCardCellQRCodeBtn3 += Cell2_OnCardCellQRCodeBtn3;

                cell2.OnCardCellQRCodeBtn4 -= Cell2_OnCardCellQRCodeBtn4;
                cell2.OnCardCellQRCodeBtn4 += Cell2_OnCardCellQRCodeBtn4; 
                  
                if (tranDetailItems[indexPath.Row].tranDetailItem.choose)
                {
                    cell2.showhead = true;
                }
                else
                {
                    cell2.showhead = false ;
                }
                //if (!string.IsNullOrEmpty( tranDetailItems[indexPath.Row].tranDetailItem.FmlDiscountRow))
                //{
                //    cell2.imgdiscountset = true;
                //}
                //else
                //{
                //    cell2.imgdiscountset = false;
                //}
                if (tranDetailItems[indexPath.Row].tranDetailItem.ItemPrice != tranDetailItems[indexPath.Row].tranDetailItem.Price)
                {
                    cell2.pricenew = CURRENCYSYMBOLS+Utils.DisplayDecimal(tranDetailItems[indexPath.Row].tranDetailItem.Price);
                }
                else
                {
                    cell2.pricenew = "";
                }
                cell2.comment = tranDetailItems[indexPath.Row].tranDetailItem.Comments;
                if (tranDetailItems[indexPath.Row].tranDetailItem.Discount == 0)
                {
                    cell2.discount = "";
                    cell2.imgdiscountset = false;
                }
                else
                {
                    cell2.discount =  "Discount ("+CURRENCYSYMBOLS+Utils.DisplayDecimal(tranDetailItems[indexPath.Row].tranDetailItem.Discount)+")";
                    cell2.imgdiscountset = true;

                }



                cell = cell2;
                return cell;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                return null;
            }
        }

        private void Cell2_OnCardCellQRCodeBtn4(CartCollectionViewCell2 merchantCollectionViewCell)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(merchantCollectionViewCell);
            Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
            OnCardCellbtnIndex4?.Invoke(indexPathQRcode);
        }

        private void Cell2_OnCardCellQRCodeBtn3(CartCollectionViewCell2 merchantCollectionViewCell)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(merchantCollectionViewCell);
            Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
            OnCardCellbtnIndex3?.Invoke(indexPathQRcode);
        }

        private void Cell2_OnCardCellQRCodeBtn2(CartCollectionViewCell2 merchantCollectionViewCell)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(merchantCollectionViewCell);
            Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
            OnCardCellbtnIndex2?.Invoke(indexPathQRcode);
        }

        private void Cell2_OnCardCellQRCodeBtn1(CartCollectionViewCell2 merchantCollectionViewCell)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(merchantCollectionViewCell);
            Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
            OnCardCellbtnIndex1?.Invoke(indexPathQRcode);
        }

        private void Cell2_OnCardCellQRCodeBtn0(CartCollectionViewCell2 merchantCollectionViewCell)
        {
            NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(merchantCollectionViewCell);
            Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
            OnCardCellbtnIndex0?.Invoke(indexPathQRcode);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return tranDetailItems.Count;
        }
        #region Events
        public delegate void CardCellQRCodeIndexDelegate0(NSIndexPath indexPath);
        public event CardCellQRCodeIndexDelegate0 OnCardCellbtnIndex0;

        public delegate void CardCellQRCodeIndexDelegate1(NSIndexPath indexPath);
        public event CardCellQRCodeIndexDelegate1 OnCardCellbtnIndex1;

        public delegate void CardCellQRCodeIndexDelegate2(NSIndexPath indexPath);
        public event CardCellQRCodeIndexDelegate2 OnCardCellbtnIndex2;

        public delegate void CardCellQRCodeIndexDelegate3(NSIndexPath indexPath);
        public event CardCellQRCodeIndexDelegate3 OnCardCellbtnIndex3;

        public delegate void CardCellQRCodeIndexDelegate4(NSIndexPath indexPath);
        public event CardCellQRCodeIndexDelegate4 OnCardCellbtnIndex4;
        #endregion
    }
}