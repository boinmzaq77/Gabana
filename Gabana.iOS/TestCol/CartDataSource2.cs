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
using System.Drawing;

namespace Gabana.iOS
{
    public class CartDataSource2 : UICollectionViewDataSource
    {
       ItemManage itemmanager = new ItemManage();
        CoreGraphics.CGRect frame;
        private List<TranDetailItemWithTopping> tranDetailItems;

        public CartDataSource2(List<TranDetailItemWithTopping> tranDetailItems, CoreGraphics.CGRect frame)
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
            UICollectionViewCell cell = new UICollectionViewCell() ;
            if (tranDetailItems[indexPath.Row].tranDetailItem.choose)
            {
                var cell2 = collectionView.DequeueReusableCell("CartCell3", indexPath) as CartCollectionViewCell3;
                cell2.Name = tranDetailItems[indexPath.Row].tranDetailItem.ItemName + " " + tranDetailItems[indexPath.Row].tranDetailItem.SizeName;
                cell2.amount = tranDetailItems[indexPath.Row].tranDetailItem.Quantity.ToString("#,##0 x");
                cell2.price = tranDetailItems[indexPath.Row].tranDetailItem.Amount.ToString("#,##0.00");
                cell2.priceperamount = tranDetailItems[indexPath.Row].tranDetailItem.Price.ToString("฿#,##0.00");
                cell2.size = frame.Width;
                cell2.OnCardCellQRCodeBtn0 += (cart3) =>
                {
                    NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(cart3);
                    Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
                    OnCardCellbtnIndex0?.Invoke(indexPathQRcode);
                };
                cell2.OnCardCellQRCodeBtn1 += (cart3) =>
                {
                    NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(cart3);
                    Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
                    OnCardCellbtnIndex1?.Invoke(indexPathQRcode);
                };
                cell2.OnCardCellQRCodeBtn2 += (cart3) =>
                {
                    NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(cart3);
                    Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
                    OnCardCellbtnIndex2?.Invoke(indexPathQRcode);
                };
                cell2.OnCardCellQRCodeBtn3 += (cart3) =>
                {
                    NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(cart3);
                    Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
                    OnCardCellbtnIndex3?.Invoke(indexPathQRcode);
                };
                cell2.OnCardCellQRCodeBtn4 += (cart3) =>
                {
                    NSIndexPath indexPathQRcode = collectionView.IndexPathForCell(cart3);
                    Console.WriteLine("indexPathQRcode = " + indexPathQRcode.ToString());
                    OnCardCellbtnIndex4?.Invoke(indexPathQRcode);
                };
                cell = cell2;
               
            }
            else
            {
                var cell2 = collectionView.DequeueReusableCell("CartCell", indexPath) as CartCollectionViewCell;
                cell2.Name = tranDetailItems[indexPath.Row].tranDetailItem.ItemName + " " + tranDetailItems[indexPath.Row].tranDetailItem.SizeName;
                cell2.amount = tranDetailItems[indexPath.Row].tranDetailItem.Quantity.ToString("#,##0 x");
                cell2.price = tranDetailItems[indexPath.Row].tranDetailItem.Amount.ToString("#,##0.00");
                cell2.priceperamount = tranDetailItems[indexPath.Row].tranDetailItem.Price.ToString("฿#,##0.00");
                cell2.size = frame.Width;

                cell = cell2;

                
            }
            return cell;

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