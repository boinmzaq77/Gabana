using Gabana.Model;
using Gabana.ShareSource.ClassStructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Print
{
    class PrintTrans
    {
        //public PrintTrans(TranWithDetailsLocal t)
        //{
        //    print(t);
        //}

        //public async Task print(TranWithDetailsLocal t)
        //{
        //    var d = t.tranDetailItemWithToppings;
        //    List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
        //    for (int i = 0; i < d.Count; i++)
        //    {
        //        items.Add(new List<KeyValuePair<string, string>>()
        //        {
        //            new KeyValuePair<string, string>("@ItemQuantity",(i+1).ToString()),
        //            new KeyValuePair<string, string>("@ItemName",d[i].tranDetailItem.ItemName.ToString()),
        //            new KeyValuePair<string, string>("@ItemPrice",d[i].tranDetailItem.Price.ToString())
        //        });
        //    }

        //    //List<List<KeyValuePair<string, string>>> items2 = new List<List<KeyValuePair<string, string>>>();
        //    //for (int i = 0; i < p.Count; i++)
        //    //{
        //    //    items2.Add(new List<KeyValuePair<string, string>>()
        //    //    {

        //    //        new KeyValuePair<string, string>("@PayName",p[i].PAYMENTTYPE.ToString()),
        //    //        new KeyValuePair<string, string>("@PayPrice",p[i].PAYMENTAMOUNT.ToString())
        //    //    });
        //    //}

        //    ParamSlip paramSlip = new ParamSlip()
        //    {
        //        Header = new List<KeyValuePair<string, string>>()
        //        {
        //            new KeyValuePair<string, string>("@Billno", t.tran.TranNo.ToString()),
        //            new KeyValuePair<string, string>("@Time", t.tran.TranDate.ToString()),
        //            new KeyValuePair<string, string>("@Member", t.tran.CustomerName.ToString()),
        //            new KeyValuePair<string, string>("@Vat", t.tran.TotalVat.ToString()),
        //            new KeyValuePair<string, string>("@Total", t.tran.GrandTotal.ToString())
        //            //,
        //            //new KeyValuePair<string, string>("@Payment", t.GRANDPAYMENT.ToString()),
        //            //new KeyValuePair<string, string>("@Charge", t.CHANGE.ToString())

        //        },
        //        Details = items

        //    };


        //    Abstracts.Printer wifiPrinter = new WifiPrinter();
        //    await wifiPrinter.Open();
        //    await wifiPrinter.Print("SlipTrans.xml", paramSlip);
        //    await wifiPrinter.Close();
        //}
    }
}
