using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource
{
    public class BLTrans
    {
        public static MemberTypeManage memberTypeManage = new MemberTypeManage();
        public static decimal FRound(Decimal val) 
        {
            return Math.Round(val, int.Parse(DataCashingAll.setmerchantConfig.DECIMAL_POINT_CALC));
        }
        public static TranWithDetailsLocal ChooseItemTran(TranWithDetailsLocal tranthis, TranDetailItemWithTopping Item)
        {
            var row = Checksameitem(tranthis, Item);
            if (row == -1)
            {
                tranthis = NewItem(tranthis, Item);
            }
            else
            {
                tranthis = OldItem(row, tranthis, Item);
            }
            return tranthis;
        }

        private static int Checksameitem(TranWithDetailsLocal tranthis, TranDetailItemWithTopping item)
        {
            var findlist = tranthis.tranDetailItemWithToppings.FindAll(x => x.tranDetailItem.SysItemID == item.tranDetailItem.SysItemID);
            if (findlist.Count == 0)
            {
                return -1;
            }
            else
            {
                int rowsame = -1;
                foreach (var itemfind in findlist)
                {
                    bool Chack = false; 
                    if (itemfind.tranDetailItem.ItemPrice != item.tranDetailItem.ItemPrice)
                    {
                        continue;
                    }
                    if (itemfind.tranDetailItem.Comments != item.tranDetailItem.Comments)
                    {
                        continue;
                    }
                    if (itemfind.tranDetailItemToppings.Count != item.tranDetailItemToppings.Count)
                    {
                        continue;
                    }
                    for (int i = 0; i < itemfind.tranDetailItemToppings.Count; i++)
                    {
                        var itemid = itemfind.tranDetailItemToppings[i].SysItemID;
                        var rowfind = item.tranDetailItemToppings.FindIndex(x => x.SysItemID == itemid);
                        if (rowfind == -1)
                        {
                            Chack = true; 
                            continue;
                            
                        }
                        else
                        {
                            if (itemfind.tranDetailItemToppings[i].Quantity != item.tranDetailItemToppings[rowfind].Quantity)
                            {
                                Chack = true;
                                continue;
                                
                            }
                        }
                    }
                    if (!Chack)
                    {
                        rowsame = tranthis.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.DetailNo == itemfind.tranDetailItem.DetailNo);
                    }
                    
                }
                if (rowsame >= 0)
                {
                    return rowsame;
                }
                else
                {
                    return -1;
                }

            }
        }

        private static TranWithDetailsLocal OldItem(int row, TranWithDetailsLocal tranthis, TranDetailItemWithTopping item)
        {

            tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity + item.tranDetailItem.Quantity;
            var subamount = FRound((decimal)tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity * tranthis.tranDetailItemWithToppings[row].tranDetailItem.Price) + FRound(tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity * tranthis.tranDetailItemWithToppings[row].tranDetailItem.SumToppingPrice);
            tranthis.tranDetailItemWithToppings[row].tranDetailItem.SubAmount = subamount;

            tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.SubAmount - tranthis.tranDetailItemWithToppings[row].tranDetailItem.Discount;
            //tranthis.tranDetailItems[row].VatAmount = tranthis.tranDetailItems[row].Amount * 7 / 100;
            if (tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxType == 'V')
            {
                if (tranthis.tran.TranTaxType == 'I')
                {
                    //vatamount = amount - (amont * 100) /107
                    var calvat =  FRound(tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount * 100 / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - calvat; //tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - FRound(tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount * 100 / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = calvat;
                }
                else
                {
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = FRound((tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount * UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)) / 100);
                }

            }
            else
            {
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = 0;
            }

            if (tranthis.tran.TranTaxType == 'I')
            {
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount;
            }
            else
            {
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
            }
            return tranthis;
        }

        private static TranWithDetailsLocal NewItem(TranWithDetailsLocal tranthis, TranDetailItemWithTopping item)
        {
            var trandetail = new TranDetailItemWithTopping()
            {
                tranDetailItem = item.tranDetailItem,
                tranDetailItemToppings = item.tranDetailItemToppings

            };
            //trandetail.tranDetailItemToppings.ConvertAll(x => x.TranNo = tranthis.tran.TranNo);
            decimal SumToppingPrice = 0;
            decimal SumToppingEstimateCost = 0;
            if (item.tranDetailItemToppings?.Count != 0)
            {
                trandetail.tranDetailItemToppings.ConvertAll(x => x.TranNo = tranthis.tran.TranNo);
                decimal thisSumTopingPrice = 0;
                decimal thisSumToppingEstimateCost = 0;
                foreach (var itemtopping in item.tranDetailItemToppings)
                {
                    thisSumTopingPrice += FRound(itemtopping.ToppingPrice * itemtopping.Quantity);
                    thisSumToppingEstimateCost += FRound(itemtopping.EstimateCost * itemtopping.Quantity);
                }
                SumToppingPrice = thisSumTopingPrice;
                SumToppingEstimateCost = thisSumToppingEstimateCost;
            }

            int max;
            if (tranthis.tranDetailItemWithToppings.Count != 0)
            {
                max = (int)tranthis.tranDetailItemWithToppings.Max(t => t.tranDetailItem.DetailNo);
            }
            else
            {
                max = 0;
            }
            max++;
            trandetail.tranDetailItem.DetailNo = max;
            trandetail.tranDetailItemToppings.ConvertAll(x => x.DetailNo = max);
            trandetail.tranDetailItem.SumToppingPrice = SumToppingPrice;
            trandetail.tranDetailItem.SumToppingEstimateCost = SumToppingEstimateCost;
            trandetail.tranDetailItem.SubAmount = FRound((decimal)trandetail.tranDetailItem.Quantity * item.tranDetailItem.Price) + FRound((decimal)trandetail.tranDetailItem.Quantity * SumToppingPrice);
            trandetail.tranDetailItem.Amount = trandetail.tranDetailItem.SubAmount - trandetail.tranDetailItem.Discount;

            if (trandetail.tranDetailItem.TaxType == 'V')
            {
                if (tranthis.tran.TranTaxType == 'I')
                {
                    //vatamount = amount - (amont * 100) /107
                    var calvat = FRound(trandetail.tranDetailItem.Amount * 100 / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                    trandetail.tranDetailItem.VatAmount = trandetail.tranDetailItem.Amount - calvat;
                    trandetail.tranDetailItem.TaxBaseAmount = calvat;// trandetail.tranDetailItem.Amount - trandetail.tranDetailItem.VatAmount;
                }
                else
                {
                    // type E
                    // vatamount = amount * 7/100
                    trandetail.tranDetailItem.TaxBaseAmount = trandetail.tranDetailItem.Amount;
                    trandetail.tranDetailItem.VatAmount = FRound((trandetail.tranDetailItem.Amount * UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)) / 100);
                }

            }
            else
            {
                trandetail.tranDetailItem.TaxBaseAmount = trandetail.tranDetailItem.Amount;
                trandetail.tranDetailItem.VatAmount = 0;
            }

            //trandetail.WeightTranDisc 
            //trandetail.WeightTranDisc = 0;
            //trandetail.TotalPrice = trandetail.TaxBaseAmount - trandetail.WeightTranDisc;
            //unitprice
            //var unitprice = trandetail.TotalPrice / trandetail.Quantity;
            //trandetail.ProfitAmount = unitprice - trandetail.EstimateCost;
            tranthis.tranDetailItemWithToppings.Add(trandetail);
            return tranthis;
        }
        public static TranWithDetailsLocal EditToppingRow(TranWithDetailsLocal tranthis, TranDetailItemWithTopping item)
        {
            var row = Checksameitem(tranthis, item);
            var index = tranthis.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.DetailNo == item.tranDetailItem.DetailNo);
            if (row == -1 || row == index)
            {

                decimal SumToppingPrice = 0;
                decimal SumToppingEstimateCost = 0;
                if (item.tranDetailItemToppings?.Count != 0)
                {
                    decimal thisSumTopingPrice = 0;
                    decimal thisSumToppingEstimateCost = 0;
                    foreach (var itemtopping in item.tranDetailItemToppings)
                    {
                        thisSumTopingPrice += FRound(itemtopping.ToppingPrice * itemtopping.Quantity);
                        thisSumToppingEstimateCost += FRound(itemtopping.EstimateCost * itemtopping.Quantity);
                    }
                    SumToppingPrice = thisSumTopingPrice;
                    SumToppingEstimateCost = thisSumToppingEstimateCost;
                }



                item.tranDetailItem.SumToppingPrice = SumToppingPrice;
                item.tranDetailItem.SumToppingEstimateCost = SumToppingEstimateCost;
                item.tranDetailItem.SubAmount = FRound((decimal)item.tranDetailItem.Quantity * item.tranDetailItem.Price) + FRound((decimal)item.tranDetailItem.Quantity * SumToppingPrice);
                item.tranDetailItem.Amount = item.tranDetailItem.SubAmount - item.tranDetailItem.Discount;

                if (item.tranDetailItem.TaxType == 'V')
                {
                    if (tranthis.tran.TranTaxType == 'I')
                    {
                        var calvat = FRound((item.tranDetailItem.Amount * 100) / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                        item.tranDetailItem.VatAmount = item.tranDetailItem.Amount - calvat;  //FRound((item.tranDetailItem.Amount * 100) / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                        item.tranDetailItem.TaxBaseAmount = calvat; //item.tranDetailItem.Amount - item.tranDetailItem.VatAmount;
                    }
                    else
                    {
                        item.tranDetailItem.TaxBaseAmount = item.tranDetailItem.Amount;
                        item.tranDetailItem.VatAmount = FRound((item.tranDetailItem.Amount * UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)) / 100);
                    }

                }
                else
                {
                    item.tranDetailItem.TaxBaseAmount = item.tranDetailItem.Amount;
                    item.tranDetailItem.VatAmount = 0;
                }
                tranthis.tranDetailItemWithToppings[index] = item;
            }
            else
            {
                tranthis = OldItem(row, tranthis, item);
                tranthis.tranDetailItemWithToppings.RemoveAt(index);


            }
            return tranthis;

        }
        public static TranDetailItemNew SetTaxBaseAmountandVatAmount(TranWithDetailsLocal tranthis, TranDetailItemNew Item)
        {
            if (Item.TaxType == 'V')
            {
                if (tranthis.tran.TranTaxType == 'I')
                {
                    //vatamount = amount - (amont * 100) /107
                    var calvat = FRound(Item.Amount * 100 / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                    Item.VatAmount = Item.Amount - calvat;
                    Item.TaxBaseAmount = calvat;// trandetail.tranDetailItem.Amount - trandetail.tranDetailItem.VatAmount;
                }
                else
                {
                    // type E
                    // vatamount = amount * 7/100
                    Item.TaxBaseAmount = Item.Amount;
                    Item.VatAmount = FRound((Item.Amount * UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)) / 100);
                }

            }
            else
            {
                Item.TaxBaseAmount = Item.Amount;
                Item.VatAmount = 0;
            }
            return Item;
        }

        public static TranWithDetailsLocal ChangePrice(TranWithDetailsLocal tranthis, TranDetailItemNew Item, decimal price)
        {
            var row = tranthis.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.DetailNo == Item.DetailNo);
            if (Item.Price != price)
            {
                Item.Price = price;

                tranthis.tranDetailItemWithToppings[row].tranDetailItem.Price = price;
                var SubAmount = FRound((decimal)tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity * tranthis.tranDetailItemWithToppings[row].tranDetailItem.Price) + FRound(tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity * tranthis.tranDetailItemWithToppings[row].tranDetailItem.SumToppingPrice);
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.SubAmount = SubAmount;
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.SubAmount - tranthis.tranDetailItemWithToppings[row].tranDetailItem.Discount;
                //tranthis.tranDetailItemWithToppings[row].tranDetailItem = SetTaxBaseAmountandVatAmount(tranthis, tranthis.tranDetailItemWithToppings[row].tranDetailItem);


                //tranthis.tranDetailItems[row].VatAmount = tranthis.tranDetailItems[row].Amount * 7 / 100;
                //if (tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxType == 'V')
                //{
                //    if (tranthis.tran.TranTaxType == 'I')
                //    {
                //        var calvat = FRound(tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount * 100 / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                //        tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - calvat;
                //        tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = calvat;// tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount;
                //    }
                //    else
                //    {
                //        tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                //        tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = FRound((tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount * UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)) / 100);
                //    }

                //}
                //else
                //{
                //    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                //    tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = 0;
                //}

                //if (tranthis.tran.TranTaxType == 'I')
                //{
                //    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount;
                //}
                //else
                //{
                //    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                //}

            }
            return tranthis;
        }

        public static TranWithDetailsLocal AddDiscountDetailItem(TranWithDetailsLocal tranthis, TranDetailItemNew Item, decimal discount, char type)
        {
            string FmlDiscountRow;
            decimal Discount;
            var row = tranthis.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.DetailNo == Item.DetailNo);
            if (type == 'P')
            {
                FmlDiscountRow = discount + "%";
            }
            else
            {
                FmlDiscountRow = discount.ToString();
            }
            tranthis.tranDetailItemWithToppings[row].tranDetailItem.FmlDiscountRow = FmlDiscountRow;
            tranthis.tranDetailItemWithToppings[row].tranDetailItem = CalDiscountDetailItem(tranthis.tranDetailItemWithToppings[row].tranDetailItem);
            return tranthis;
        }

        public static TranDetailItemNew CalDiscountDetailItem(TranDetailItemNew tranDetailItem)
        {
            decimal discount;

            var checkdiscount = tranDetailItem.FmlDiscountRow.IndexOf('%');
            var fmldis = tranDetailItem.FmlDiscountRow;
            if (checkdiscount == -1)
            {
                discount = Convert.ToDecimal(fmldis);
            }
            else
            {
                discount = FRound((tranDetailItem.SubAmount * Convert.ToDecimal(fmldis.Remove(checkdiscount))) / 100);
            }
            tranDetailItem.Discount = discount;
            tranDetailItem.Amount = tranDetailItem.SubAmount - tranDetailItem.Discount;
            //tranDetailItem = SetTaxBaseAmountandVatAmount(tranthis, tranDetailItem);

            return tranDetailItem;
        }

        public static TranWithDetailsLocal Caltran(TranWithDetailsLocal tranthis)
        {
            decimal SubTotalNoneVat = 0;
            decimal SubTotalHaveVat = 0;
            decimal TotalTradDiscNoneVat = 0;
            decimal TotalTradDiscHaveVat = 0;
            decimal TotalNoneVat = 0;
            decimal TotalHaveVat = 0;
            decimal Total = 0;
            decimal ServiceCharge = 0;
            decimal TotalVat = 0;
            decimal GrandTotal = 0;
            decimal TradDiscHaveVats = 0;
            decimal TradDiscNonVats = 0;
            decimal SubAmount = 0;
            decimal Amount = 0;

            foreach (var item in tranthis.tranDetailItemWithToppings)
            {
                if (item.tranDetailItem.TaxType == 'N')
                {
                    SubTotalNoneVat += item.tranDetailItem.Amount;
                }
                else
                {
                    SubTotalHaveVat += item.tranDetailItem.Amount;
                }
                if (! string.IsNullOrEmpty( item.tranDetailItem.FmlDiscountRow))
                {
                    item.tranDetailItem = BLTrans.CalDiscountDetailItem(item.tranDetailItem);
                }
                item.tranDetailItem = SetTaxBaseAmountandVatAmount(tranthis, item.tranDetailItem);

                SubAmount += item.tranDetailItem.SubAmount;
                Amount += item.tranDetailItem.Amount;
            }
            var sumtotal = SubTotalNoneVat + SubTotalHaveVat;


            //trantreddiscount 
            if (sumtotal == 0)
            {
                foreach (var item in tranthis.tranTradDiscounts)
                {
                    item.TradeDiscHaveVat = 0;
                    item.TradeDiscNoneVat = 0;
                }
            }
            if (sumtotal != 0)
            {
                var indexPS = tranthis.tranTradDiscounts.FindIndex(x => x.DiscountType == "PS");
                decimal memberDiscountHaveVat = 0;
                decimal memberDiscountNoneVat = 0;

                decimal SubNoneVat = SubTotalNoneVat;
                decimal SubHaveVat = SubTotalHaveVat;

                if (indexPS != -1)
                {
                    var checkDisPS = tranthis.tranTradDiscounts[indexPS].FmlDiscount.IndexOf('%') ;
                    var disPS = tranthis.tranTradDiscounts[indexPS].FmlDiscount;
                    if (checkDisPS == -1)
                    {
                        memberDiscountHaveVat = FRound((SubTotalHaveVat * Convert.ToDecimal(disPS)) / sumtotal);
                        memberDiscountNoneVat = FRound((SubNoneVat * Convert.ToDecimal(disPS)) / sumtotal);
                        tranthis.tranTradDiscounts[indexPS].TradeDiscHaveVat = memberDiscountHaveVat;
                        tranthis.tranTradDiscounts[indexPS].TradeDiscNoneVat = memberDiscountNoneVat;
                        TotalTradDiscHaveVat += memberDiscountHaveVat;
                        TotalTradDiscNoneVat += memberDiscountNoneVat;

                    }
                    else
                    {
                        disPS = tranthis.tranTradDiscounts[indexPS].FmlDiscount.Remove(checkDisPS);
                        var discountthis = FRound(sumtotal * Convert.ToDecimal(disPS) / 100);
                        memberDiscountHaveVat = FRound((SubHaveVat * discountthis) / sumtotal);
                        memberDiscountNoneVat = FRound((SubNoneVat * discountthis) / sumtotal);
                        tranthis.tranTradDiscounts[indexPS].TradeDiscHaveVat = memberDiscountHaveVat;
                        tranthis.tranTradDiscounts[indexPS].TradeDiscNoneVat = memberDiscountNoneVat;
                        TotalTradDiscHaveVat += memberDiscountHaveVat;
                        TotalTradDiscNoneVat += memberDiscountNoneVat;

                    }

                    SubNoneVat -= memberDiscountNoneVat;
                    SubHaveVat -= memberDiscountHaveVat;
                }
                decimal sumtotalDifFS = SubNoneVat + SubHaveVat;
                if (sumtotalDifFS != 0)
                {
                    foreach (var item in tranthis.tranTradDiscounts)
                    {
                        if (item.DiscountType != "PS")
                        {
                            var checkdiscount = item.FmlDiscount.IndexOf('%');
                            var fmldis = item.FmlDiscount;
                            if (checkdiscount == -1)
                            {
                                var TradDiscHaveVatthis = FRound((SubHaveVat * Convert.ToDecimal(fmldis)) / sumtotalDifFS);
                                var TradDiscNoneVatthis = FRound((SubNoneVat * Convert.ToDecimal(fmldis)) / sumtotalDifFS);
                                item.TradeDiscHaveVat = TradDiscHaveVatthis;
                                item.TradeDiscNoneVat = TradDiscNoneVatthis;
                                TotalTradDiscHaveVat += TradDiscHaveVatthis;
                                TotalTradDiscNoneVat += TradDiscNoneVatthis;

                            }
                            else
                            {
                                var discountthis = FRound((sumtotalDifFS * Convert.ToDecimal(fmldis.Remove(checkdiscount))) / 100);
                                var TradDiscHaveVatthis = FRound((SubHaveVat * discountthis) / sumtotalDifFS);
                                var TradDiscNoneVatthis = FRound((SubNoneVat * discountthis) / sumtotalDifFS);
                                item.TradeDiscHaveVat = TradDiscHaveVatthis;
                                item.TradeDiscNoneVat = TradDiscNoneVatthis;
                                TotalTradDiscHaveVat += TradDiscHaveVatthis;
                                TotalTradDiscNoneVat += TradDiscNoneVatthis;

                            }
                        }
                       
                    }
                }
                else
                {
                    foreach (var item in tranthis.tranTradDiscounts)
                    {
                        if (item.DiscountType != "PS")
                        {
                            item.TradeDiscHaveVat = 0;
                            item.TradeDiscNoneVat = 0;
                        }

                    }
                }
            }
            //discount tran 
            TotalNoneVat = SubTotalNoneVat - TotalTradDiscNoneVat;
            TotalHaveVat = SubTotalHaveVat - TotalTradDiscHaveVat;

            if (string.IsNullOrEmpty(tranthis.tran.FmlServiceCharge))
            {
                ServiceCharge = 0;
            }
            else
            {
                var check = tranthis.tran.FmlServiceCharge.IndexOf('%');
                if (check == -1)
                {
                    ServiceCharge = Convert.ToDecimal(tranthis.tran.FmlServiceCharge);
                }
                else
                {
                    if (DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE == "A")
                    {
                        ServiceCharge = FRound((Amount * Convert.ToDecimal(tranthis.tran.FmlServiceCharge.Remove(check))) / 100);
                    }
                    else
                    {
                        ServiceCharge = FRound((sumtotal * Convert.ToDecimal(tranthis.tran.FmlServiceCharge.Remove(check))) / 100);
                    }


                }
            }
            if (tranthis.tran.TaxRate == null)
            {
                tranthis.tran.TaxRate = 0;
            }
            var vat = tranthis.tran.TaxRate ;

            if (tranthis.tran.TranTaxType == 'E')
            {
                Total = TotalNoneVat + TotalHaveVat;
                if (TotalHaveVat <= 0) TotalHaveVat = 0;
                //TotalVat = (TotalHaveVat * 7 / 100) + (ServiceCharge * 7 / 100);
                if (Total== 0)
                {
                    TotalVat = 0;
                }
                else
                {
                    TotalVat = FRound((TotalHaveVat + ServiceCharge) * ((decimal)vat / 100));

                }
                GrandTotal = Total + TotalVat + ServiceCharge;
            }
            else
            {
                GrandTotal = TotalNoneVat + TotalHaveVat + ServiceCharge;
                //TotalVat = (TotalHaveVat + ServiceCharge) * 7 / 107;
                if (GrandTotal == 0)
                {
                    TotalVat = 0;
                }
                else
                {
                TotalVat = FRound((TotalHaveVat + ServiceCharge) * ((decimal)vat / (100 + (decimal)vat)));

                }
                Total = GrandTotal - TotalVat;
            }
            tranthis.tran.SubTotalNoneVat = SubTotalNoneVat;
            tranthis.tran.SubTotalHaveVat = SubTotalHaveVat;
            tranthis.tran.TotalNoneVat = TotalNoneVat;
            tranthis.tran.TotalHaveVat = TotalHaveVat;
            tranthis.tran.TotalTradDiscNoneVat = TotalTradDiscNoneVat;
            tranthis.tran.TotalTradDiscHaveVat = TotalTradDiscHaveVat;
            tranthis.tran.TotalNoneVat = TotalNoneVat;
            tranthis.tran.TotalHaveVat = TotalHaveVat;
            tranthis.tran.Total = Total;
            tranthis.tran.ServiceCharge = ServiceCharge;
            tranthis.tran.TotalVat = TotalVat;
            tranthis.tran.GrandTotal = GrandTotal;
            Calprofititemdetail(tranthis);
            return tranthis;
        }
        public static TranWithDetailsLocal Calprofititemdetail(TranWithDetailsLocal tranthis)
        {
            decimal SumProfitAmount = 0;
            foreach (var item in tranthis.tranDetailItemWithToppings)
            {
                decimal weightTranDisc = 0;
                decimal ProfitAmount = 0;

                if (item.tranDetailItem.Amount == 0) weightTranDisc = 0; 
                else if (item.tranDetailItem.TaxType == 'N')
                {
                    weightTranDisc = FRound(item.tranDetailItem.Amount * tranthis.tran.TotalTradDiscNoneVat / tranthis.tran.SubTotalNoneVat);
                }
                else
                {
                    weightTranDisc = FRound(item.tranDetailItem.Amount * tranthis.tran.TotalTradDiscHaveVat / tranthis.tran.SubTotalHaveVat);
                }
                item.tranDetailItem.WeightTranDisc = weightTranDisc;
                item.tranDetailItem.TotalPrice = item.tranDetailItem.TaxBaseAmount - weightTranDisc;

                ProfitAmount = item.tranDetailItem.TotalPrice - FRound(item.tranDetailItem.Quantity * item.tranDetailItem.EstimateCost);
                item.tranDetailItem.ProfitAmount = ProfitAmount;
                SumProfitAmount += ProfitAmount;
            }

            tranthis.tran.TotalProfit = SumProfitAmount;
            tranthis.tran.GrandPayment = tranthis.tran.GrandTotal - tranthis.tran.PaymentFractional;
            return tranthis;
        }

        public static TranWithDetailsLocal RemoveDetailItem(TranWithDetailsLocal tranWithDetails, TranDetailItemWithTopping item)
        {
            var row = tranWithDetails.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.DetailNo == item.tranDetailItem.DetailNo);
            if (row!= -1)
            {
                tranWithDetails.tranDetailItemWithToppings.RemoveAt(row);
            }
            
            return tranWithDetails;
        }

        public static TranWithDetailsLocal ChangeQuantity(TranWithDetailsLocal tranthis, TranDetailItemNew Item, int quantity)
        {
            var row = tranthis.tranDetailItemWithToppings.FindIndex(x => x.tranDetailItem.DetailNo == Item.DetailNo);
            if (Item.Quantity != quantity)
            {
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity = quantity;
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.SubAmount = FRound((decimal)tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity * Item.Price) + FRound(tranthis.tranDetailItemWithToppings[row].tranDetailItem.Quantity * tranthis.tranDetailItemWithToppings[row].tranDetailItem.SumToppingPrice);
                tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.SubAmount - tranthis.tranDetailItemWithToppings[row].tranDetailItem.Discount;
                //tranthis.tranDetailItems[row].VatAmount = tranthis.tranDetailItems[row].Amount * 7 / 100;
                if (tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxType == 'V')
                {
                    if (tranthis.tran.TranTaxType == 'I')
                    {
                        var calvat =  FRound(tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount * 100 / (100 + UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)));
                        tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - calvat;
                        tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = calvat;
                    }
                    else
                    {
                        tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                        tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = FRound((tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount * UtilsAll.Stringtodecimal(DataCashingAll.setmerchantConfig.TAXRATE)) / 100);
                    }

                }
                else
                {
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount = 0;
                }

                if (tranthis.tran.TranTaxType == 'I')
                {
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount - tranthis.tranDetailItemWithToppings[row].tranDetailItem.VatAmount;
                }
                else
                {
                    tranthis.tranDetailItemWithToppings[row].tranDetailItem.TaxBaseAmount = tranthis.tranDetailItemWithToppings[row].tranDetailItem.Amount;
                }

            }
            return tranthis;
        }
        public static TranWithDetailsLocal AddDiscount(TranWithDetailsLocal tranthis, TranTradDiscount discount)
        {

            if (discount.TradDiscountNo == 0)
            {
                if (tranthis.tranTradDiscounts.Count == 0)
                {
                    discount.TradDiscountNo = 1;
                }
                else
                {
                    discount.TradDiscountNo = tranthis.tranTradDiscounts.Max(x => x.TradDiscountNo) + 1;
                }
                tranthis.tranTradDiscounts.Add(discount);
            }
            tranthis = Caltran(tranthis);
            return tranthis;
        }
        public async static Task<TranWithDetailsLocal> ChoosePerson(TranWithDetailsLocal tranthis, Customer customer)
        {
            var find = tranthis.tranTradDiscounts.FindIndex(x => x.DiscountType == "PS");
            if (find != -1)
            {
               await RemovePerson(tranthis);
            }

            tranthis.tran.CustomerName = customer.CustomerName;
            tranthis.tran.SysCustomerID = customer.SysCustomerID;
            // check ส่วนลดไม่เท่ากับ 0 ไปยิง adddiscount ต่อ
            if (!(customer.MemberTypeNo is null))
            {
                var getcustomerType = await memberTypeManage.GetMemberType(DataCashingAll.MerchantId, Convert.ToInt32(customer.MemberTypeNo));
                string fmlDiscount = "0";
                if (getcustomerType.PercentDiscount != 0)
                {
                    fmlDiscount = getcustomerType.PercentDiscount.ToString() + "%";
                }
                TranTradDiscount dis = new TranTradDiscount()
                {
                    MerchantID = tranthis.tran.MerchantID,
                    SysBranchID = tranthis.tran.SysBranchID,
                    TranNo = tranthis.tran.TranNo,
                    PriorityNo = 0,
                    FOnTop = 0,
                    DiscountType = "PS",
                    FmlDiscount = fmlDiscount
                };
                tranthis = AddDiscount(tranthis, dis);
            }
            return tranthis;
        }
        public static TranWithDetailsLocal RemoveDiscount(TranWithDetailsLocal tranthis, String type)
        {
            var index = tranthis.tranTradDiscounts.FindIndex(x => x.DiscountType == type);
            if (index != -1)
            {
                tranthis.tranTradDiscounts.RemoveAt(index);
            }
            tranthis = Caltran(tranthis);
            return tranthis;
        }
        public async static Task<TranWithDetailsLocal> RemovePerson(TranWithDetailsLocal tranthis)
        {
            tranthis.tran.CustomerName = "";
            tranthis.tran.SysCustomerID = 999;
            tranthis = RemoveDiscount(tranthis, "PS");
            return tranthis;
        }

        public async static Task<TranWithDetailsLocal> CalDecimal(TranWithDetailsLocal tran)
        {
            var grandTotal = tran.tran.GrandTotal; 
            decimal grandFloor = Math.Floor(grandTotal);
            decimal fractionGrand = grandTotal - grandFloor;
            decimal fractionSum;


            //ไม่มีเศษ หรือ เคยปัดเศษแล้ว
            if (fractionGrand <= 0)
            {
                return tran;
            }
            switch (DataCashingAll.setmerchantConfig.OPTION_ROUNDING_STRING)
            {
                // ไม่ปัดเศษ
                case "0":
                    tran.tran.PaymentFractional = 0;

                    break;
                case "1":
                    if (fractionGrand < (decimal)0.2500)
                    {
                        fractionSum = (decimal)0.2500 - fractionGrand;
                    }
                    else if (fractionGrand < (decimal)0.50)
                    {
                        fractionSum = (decimal)0.5000 - fractionGrand;
                    }
                    else if (fractionGrand < (decimal)0.75)
                    {
                        fractionSum = (decimal)0.7500 - fractionGrand;
                    }
                    else
                    {
                        fractionSum = (decimal)1.0000 - fractionGrand;
                    }

                    if (fractionGrand == (decimal)0.25 || fractionGrand == (decimal)0.50 || fractionGrand == (decimal)0.75 || fractionGrand == 0 )
                    {
                        tran.tran.PaymentFractional = 0;
                    }
                    else
                    {
                        tran.tran.PaymentFractional = (decimal)fractionSum;
                        tran.tran.GrandPayment = grandTotal + (decimal)fractionSum;
                    }
                    break;
                case "2":
                    fractionSum = (decimal)1.0000 - fractionGrand;
                    tran.tran.PaymentFractional = (decimal)fractionSum;
                    tran.tran.GrandPayment = grandTotal + (decimal)fractionSum;
                    break;
                case "3":
                    if (fractionGrand < (decimal)0.5)
                    {
                        tran.tran.PaymentFractional = (decimal)fractionGrand * -1;
                        tran.tran.GrandPayment = grandTotal - fractionGrand;
                    }
                    else
                    {
                        fractionSum = (decimal)1.0000 - fractionGrand;
                        tran.tran.PaymentFractional = (decimal)fractionSum;
                        tran.tran.GrandPayment = grandTotal + (decimal)fractionSum;
                    }
                    break;
                case "4":
                    int OPTION_ROUNDING_INT = Convert.ToInt32(DataCashingAll.setmerchantConfig.OPTION_ROUNDING_INT);
                    if (OPTION_ROUNDING_INT > 1)
                    {
                        fractionGrand += grandFloor % OPTION_ROUNDING_INT;
                    }
                    if (fractionGrand < ((decimal)OPTION_ROUNDING_INT * (decimal)0.5))
                    {
                        tran.tran.PaymentFractional = (decimal)fractionGrand * -1;
                        tran.tran.GrandPayment = grandTotal - fractionGrand;
                    }
                    else
                    {
                        fractionSum = OPTION_ROUNDING_INT - fractionGrand;
                        tran.tran.PaymentFractional = (decimal)fractionSum;
                        tran.tran.GrandPayment = grandTotal + (decimal)fractionSum;
                    }
                    break;
                case "5":
                    tran.tran.PaymentFractional = fractionGrand * -1;
                    tran.tran.GrandPayment = grandFloor;
                    break;
                default:

                    break;
            }

            return tran;
        }
    }

}
