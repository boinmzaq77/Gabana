using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Gabana.Model;
using Gabana.ShareSource.Manage;
using Gabana.ORM.MerchantDB;
using System.Threading.Tasks;

namespace Gabana.iOS
{
    public class OptionToppingDataSource : UICollectionViewDataSource
    {
        public List<Item> allitem = new List<Item>();
        

        public UICollectionView collectionView; 
        public OptionToppingDataSource(List<Item> all)
        {
            this.allitem = all;
            
        }
        public void ReloadData(List<Item> all)
        {
            this.allitem = all;
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("optionToppingCollectionViewCell", indexPath) as OptionToppingCollectionViewCell;
            
            this.collectionView = collectionView;
            cell.SizeName = allitem[(int)indexPath.Row].ItemName ?? "";
            cell.Price = Utils.DisplayDecimal(allitem[(int)indexPath.Row].Price);
            var indexfind = OptionController.lstTranSelectTopping.FindIndex(x => x.SysItemID == allitem[(int)indexPath.Row].SysItemID);
            if (indexfind >= 0)
            {
                cell.SelectSize = true;
                cell.setCount = OptionController.lstTranSelectTopping[indexfind].Quantity.ToString();
            }
            else
            {
                cell.SelectSize = false;
                cell.setCount = "1";
            }



            cell.OnToppingCellSelectBtn -= Cell_OnToppingCellSelectBtn;
            cell.OnToppingCellSelectBtn += Cell_OnToppingCellSelectBtn;
            cell.OnToppingCellplus -= Cell_OnToppingCellplus;
            cell.OnToppingCellplus += Cell_OnToppingCellplus; ;
            cell.OnToppingCellminus -= Cell_OnToppingCellminus;
            cell.OnToppingCellminus += Cell_OnToppingCellminus;
            return cell;
        }

        private void Cell_OnToppingCellplus(OptionToppingCollectionViewCell optionToppingCollectionViewCell)
        {
            NSIndexPath indexcode = collectionView.IndexPathForCell(optionToppingCollectionViewCell);
            if (OptionController.lstTranSelectTopping.Count == 0)
            {
                return;
            }
            var indexfind = OptionController.lstTranSelectTopping.FindIndex(x => x.SysItemID == allitem[indexcode.Row].SysItemID);
            if (indexfind == -1)
            {
                return;
            }
            else
            {
                OptionController.lstTranSelectTopping[indexfind].Quantity++;
                optionToppingCollectionViewCell.setCount = OptionController.lstTranSelectTopping[indexfind].Quantity.ToString();
            }
        }

        private void Cell_OnToppingCellminus(OptionToppingCollectionViewCell optionToppingCollectionViewCell)
        {
            NSIndexPath indexcode = collectionView.IndexPathForCell(optionToppingCollectionViewCell);
            if (OptionController.lstTranSelectTopping.Count == 0)
            {
                return;
            }
            var indexfind = OptionController.lstTranSelectTopping.FindIndex(x => x.SysItemID == allitem[indexcode.Row].SysItemID);
            if (indexfind == -1)
            {
                return;
            }
            else
            {
                if (OptionController.lstTranSelectTopping[indexfind].Quantity == 1)
                {
                    return;
                }
                OptionController.lstTranSelectTopping[indexfind].Quantity--;
                optionToppingCollectionViewCell.setCount = OptionController.lstTranSelectTopping[indexfind].Quantity.ToString();
            }
        }

        private void Cell_OnToppingCellSelectBtn(OptionToppingCollectionViewCell optionToppingCollectionViewCell)
        {
            NSIndexPath indexcode = collectionView.IndexPathForCell(optionToppingCollectionViewCell);
            if (OptionController.lstTranSelectTopping.Count == 0 )
            {
                var topping = new TranDetailItemTopping()
                {
                    ItemName = allitem[indexcode.Row].ItemName,//toppping
                    SysItemID = allitem[indexcode.Row].SysItemID,
                    UnitName = allitem[indexcode.Row].UnitName,
                    RegularSizeName = allitem[indexcode.Row].RegularSizeName,
                    Quantity = 1,
                    ToppingPrice = allitem[indexcode.Row].Price,
                    EstimateCost = allitem[indexcode.Row].EstimateCost,
                    Comments = allitem[indexcode.Row].Comments,
                    ToppingNo = 1,
                };
                OptionController.lstTranSelectTopping.Add(topping);
                optionToppingCollectionViewCell.SelectSize = true;
                optionToppingCollectionViewCell.setCount = "1";
            }
            else
            {
                var indexfind = OptionController.lstTranSelectTopping.FindIndex(x => x.SysItemID == allitem[indexcode.Row].SysItemID);
                if (indexfind == -1)
                {
                    var topping = new TranDetailItemTopping()
                    {
                        ItemName = allitem[indexcode.Row].ItemName,//toppping
                        SysItemID = allitem[indexcode.Row].SysItemID,
                        UnitName = allitem[indexcode.Row].UnitName,
                        RegularSizeName = allitem[indexcode.Row].RegularSizeName,
                        Quantity = 1,
                        ToppingPrice = allitem[indexcode.Row].Price,
                        EstimateCost = allitem[indexcode.Row].EstimateCost,
                        Comments = allitem[indexcode.Row].Comments,
                        ToppingNo = OptionController.lstTranSelectTopping.Max(x=>x.ToppingNo)+1,
                    };
                    OptionController.lstTranSelectTopping.Add(topping);
                    optionToppingCollectionViewCell.SelectSize = true;
                    optionToppingCollectionViewCell.setCount = "1";
                }
                else
                {
                    OptionController.lstTranSelectTopping.RemoveAt(indexfind);
                    optionToppingCollectionViewCell.SelectSize = false;
                    optionToppingCollectionViewCell.setCount = "1"; 
                }
            }
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return allitem.Count;
        }
        #region Events
        public delegate void ExtraToppingSelectIndexDelegate(NSIndexPath indexPath);
        public event ExtraToppingSelectIndexDelegate OnExtraToppingSelectIndex;

        #endregion
    }
}