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
using static Gabana.iOS.CustomerController;
using static Gabana.iOS.BillHistoryController;
using System.Globalization;
using Gabana.ShareSource;

namespace Gabana.iOS
{
    public class StockDataSource : UICollectionViewDataSource
    {
        //  List<tra>

        List<Gabana.Model.ItemMovement> stock ;
        UIViewController uIView;
        public StockDataSource(List<Gabana.Model.ItemMovement> list,UIViewController uIView)
        {
            this.stock = list;
            this.uIView = uIView;
        }
        public void ReloadData(List<Gabana.Model.ItemMovement> cus)
        {
            try
            {
                this.stock = cus;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("StockCollectionViewCell", indexPath) as StockCollectionViewCell;
            switch (stock[indexPath.Row].MovementType)
            {
                case 'S':
                    cell.Image = "StockDecrease";
                    cell.Type = "Sale";
                        
                    break;
                case 'C':
                    cell.Image = "StockIncrease";
                    cell.Type = "Canceled Sale";
                    break;
                case 'A':
                    cell.Image = "StockIncrease";
                    cell.Type = "Add Stock";
                    break;
                case 'R':
                    cell.Image = "StockIncrease";
                    cell.Type = "Remove Stock";
                    break;
                case 'B':
                    cell.Image = "StockDecrease";
                    cell.Type = "Begin Balance";
                    break;
                default:
                    break;
            }
          
            cell.By = stock[indexPath.Row].UserName?.ToString();
            cell.Date = stock[indexPath.Row].MovementDate.ToString("dd/MM/yyyy HH:mm");
            cell.Balance = stock[indexPath.Row].Quantity.ToString("#,###");
            
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return 1;
        }
    }
}