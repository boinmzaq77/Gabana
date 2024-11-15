using Gabana.ORM;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Controller
{
    public class GeneratedTable
    {
		#region CreateTable
		public string createTable = @"Create Table AppConfig
									(
										CfgKey				Varchar(50)		Not NULL , 
										CfgString			Varchar(255)	         , 
										CfgInteger			Integer			         , 
										CfgFloat			numeric(18,4)	         , 
										CfgDate				Date			         ,
										CONSTRAINT XPKAppConfig PRIMARY KEY (CfgKey Asc)
									) ;

									Create Table DataBaseInfo
									(
										KeyDBInfo			Varchar(50)		Not NULL , 
										DataDBInfo			Varchar(255)	         ,
										CONSTRAINT XPKDataBaseInfo PRIMARY KEY (KeyDBInfo Asc)
									) ;

									Create Table Merchant
									(
										MerchantID			Integer			Not NULL , 
										Name				Varchar(250)	Not NULL , 
										FMasterMerchant		numeric(1,0)	Not NULL , 
										RefMasterMerchantID	Integer			Not NULL , 
										LogoPath				Varchar(250)	         , 
										Status				Varchar(1)		Not NULL , 
										DateOpenMerchant	Date			Not NULL , 
										DateCloseMerchant	Date			         , 
										RefPackageID		Varchar(25)		Not NULL , 
										DayOfPeriod			Integer			Not NULL , 
										DueDate				Date			Not NULL , 
										LanguageCountryCode	Varchar(50)		Not NULL , 
										TimeZoneName		Varchar(50)		Not NULL , 
										TimeZoneUTCOffset	Numeric(8,4)	Not NULL , 
										DateCreated			DateTime		Not NULL , 
										DateModified		DateTime		Not NULL , 
										UserNameModified	Varchar(50)		Not NULL ,
										CONSTRAINT XPKMerchant PRIMARY KEY (MerchantID Asc)
									) ;

									Create Table RevisionSequeceNo
									(
										MerchantID			Integer			Not NULL , 
										SystemID			Integer			Not NULL , 
										LastRevisionNo		Integer			Not NULL ,
										CONSTRAINT XPKRevisionSequeceNo PRIMARY KEY (MerchantID Asc, SystemID Asc)
									) ;

									Create Table Category
									(
										MerchantID			Integer			Not NULL , 
										SysCategoryID		Integer			Not NULL , 
										Ordinary			Integer			         , 
										Name				Varchar(100)	Not NULL , 
										DateCreated			DateTime		Not NULL , 
										DateModified		DateTime		Not NULL , 
										DataStatus			Varchar(1)		Not NULL , 
										FWaitSending		numeric(1,0)	Not NULL , 
										LinkProMaxxID		Varchar(50)		         ,
										CONSTRAINT XPKCategory PRIMARY KEY (MerchantID Asc, SysCategoryID Asc),
										CONSTRAINT R1Category FOREIGN KEY (merchantid) REFERENCES merchant(merchantid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Table Customer
									(
										MerchantID			Integer			Not NULL , 
										SysCustomerID		Integer			Not NULL , 
										CustomerName		Varchar(255)	Not NULL , 
										Ordinary			Integer			         , 
										CustomerID			Varchar(50)		         , 
										ShortName			Varchar(50)		         , 
										PicturePath			Varchar(255)	         , 
										EMail				Varchar(320)	         , 
										Mobile				Varchar(50)		         , 
										Gender				Varchar(1)		Not NULL , 
										BirthDate			Date			Not NULL , 
										Address				Varchar(255)	         , 
										ProvincesId			Integer			         , 
										AmphuresId			Integer			         , 
										DistrictsId			Integer			         , 
										PicturePath			Varchar(255)	         , 
										IDCard				Varchar(20)		         , 
										Comments			Varchar(255)	         , 
										LastDateModified	DateTime		Not NULL , 
										UserLastModified	Varchar(50)		Not NULL , 
										LinkProMaxxID		Varchar(50)		         ,
										CONSTRAINT XPKCustomer PRIMARY KEY (MerchantID Asc, SysCustomerID Asc),
										CONSTRAINT R1Customer FOREIGN KEY (merchantid) REFERENCES merchant(merchantid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Index XIE1Customer On Customer
										(CustomerName Asc);

									Create Index XIE2Customer On Customer
										(CustomerID Asc);

									Create Table Device
									(
										MerchantID			Integer			Not NULL , 
										DeviceNo			Integer			Not NULL , 
										Platform			Varchar(4)		Not NULL , 
										UDID				Varchar(50)		Not NULL , 
										DeviceInfo			Varchar(255)	         , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKDevice PRIMARY KEY (MerchantID Asc, DeviceNo Asc),
										CONSTRAINT R1Device FOREIGN KEY (merchantid) REFERENCES merchant(merchantid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Index XIE1Device On Device
										(MerchantID Asc, UDID Asc);

									Create Table UserAccountInfo
									(
										MerchantID			Integer			Not NULL , 
										UserName			Varchar(50)		Not NULL , 
										FUsePincode			Integer			Not NULL , 
										PinCode				Varchar(100)	         , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKUserAccountInfo PRIMARY KEY (MerchantID Asc, UserName Asc),
										CONSTRAINT R1UserAccountInfo FOREIGN KEY (merchantid) REFERENCES merchant(merchantid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Table Item
									(
										MerchantID			Integer			Not NULL , 
										SysItemID			Integer			Not NULL , 
										ItemName			Varchar(255)	Not NULL , 
										Ordinary			Integer			         , 
										SysCategoryID		Integer			         , 
										ItemCode			Varchar(50)		         , 
										ShortName			Varchar(50)		         , 
										PicturePath			Varchar(255)	         , 
										Colors				Integer			         , 
										FavoriteNo			Integer			Not NULL , 
										UnitName			Varchar(50)		         , 
										RegularSizeName		Varchar(50)		         , 
										Price				numeric(18,4)	Not NULL , 
										OptSalePrice		Varchar(1)		Not NULL , 
										TaxType				Varchar(1)		Not NULL , 
										SellBy				Varchar(1)		Not NULL , 
										FTrackStock			numeric(1,0)	Not NULL , 
										TrackStockDateTime	DateTime		Not NULL , 
										MinimumStock		numeric(18,4)	Not NULL , 
										Cost				numeric(18,4)	Not NULL , 
										SaleItemType		Varchar(1)		Not NULL , 
										Comments			Varchar(255)	         , 
										LastDateModified	DateTime		Not NULL , 
										UserLastModified	Varchar(50)		Not NULL , 
										DataStatus			Varchar(1)		Not NULL , 
										FWaitSending		numeric(1,0)	Not NULL , 
										LinkProMaxxItemID	Varchar(50)		         , 
										LinkProMaxxItemUnit	Varchar(50)		         ,
										CONSTRAINT XPKItem PRIMARY KEY (MerchantID Asc, SysItemID Asc),
										CONSTRAINT R1Item FOREIGN KEY (merchantid) REFERENCES merchant(merchantid)
											ON DELETE No Action
											ON UPDATE No Action,
										CONSTRAINT R2Item FOREIGN KEY (merchantid, syscategoryid) REFERENCES category(merchantid, syscategoryid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Index XIE1Item On Item
										(ItemName Asc);

									Create Index XIE2Item On Item
										(ItemCode Asc);

									Create Table ItemExSize
									(
										MerchantID			Integer			Not NULL , 
										SysItemID			Integer			Not NULL , 
										ExSizeNo			Integer			Not NULL , 
										ExSizeName			Varchar(50)		Not NULL , 
										Price				numeric(18,4)	Not NULL , 
										EstimateCost		numeric(18,4)	Not NULL , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKItemExSize PRIMARY KEY (MerchantID Asc, SysItemID Asc, ExSizeNo Asc),
										CONSTRAINT R1ItemExSize FOREIGN KEY (merchantid, sysitemid) REFERENCES item(merchantid, sysitemid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Table Branch
									(
										MerchantID			Integer			Not NULL , 
										BranchID			Integer			Not NULL , 
										Ordinary			Integer			         , 
										BranchName			Varchar(100)	Not NULL , 
										Address				Varchar(250)	         , 
										ProvincesId			Integer			         , 
										AmphuresId			Integer			         , 
										DistrictsId			Integer			         , 
										DisplayLanguage		Varchar(1)		Not NULL , 
										Lat					Numeric(10,6)	         , 
										Lng					Numeric(10,6)	         , 
										Email				Varchar(100)	         , 
										Tel					Varchar(100)	         , 
										Line				Varchar(100)	         , 
										Facebook			Varchar(100)	         , 
										Instagram			Varchar(100)	         , 
										TaxBranchName		Varchar(255)	Not NULL , 
										TaxBranchID			Varchar(50)		         , 
										TaxID				Varchar(50)		         , 
										RegMark				Varchar(50)		         , 
										LinkProMaxxID		Varchar(50)		         , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKBranch PRIMARY KEY (MerchantID Asc, BranchID Asc),
										CONSTRAINT R1Branch FOREIGN KEY (merchantid) REFERENCES merchant(merchantid)
											ON DELETE No Action
											ON UPDATE No Action,
										CONSTRAINT R2Branch FOREIGN KEY (provincesid) REFERENCES provinces(provincesid)
											ON DELETE No Action
											ON UPDATE No Action,
										CONSTRAINT R3Branch FOREIGN KEY (amphuresid) REFERENCES amphures(amphuresid)
											ON DELETE No Action
											ON UPDATE No Action,
										CONSTRAINT R4Branch FOREIGN KEY (districtsid) REFERENCES districts(districtsid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Table RunningNo
									(
										MerchantID			Integer			Not NULL , 
										BranchID			Integer			Not NULL , 
										DeviceNo			Integer			Not NULL , 
										TranLastRunningNo	Integer			Not NULL ,
										CONSTRAINT XPKRunningNo PRIMARY KEY (MerchantID Asc, BranchID Asc, DeviceNo Asc),
										CONSTRAINT R1RunningNo FOREIGN KEY (merchantid, branchid) REFERENCES branch(merchantid, branchid)
											ON DELETE No Action
											ON UPDATE No Action,
										CONSTRAINT R2RunningNo FOREIGN KEY (merchantid, deviceno) REFERENCES device(merchantid, deviceno)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Table Trans
									(
										MerchantID			Integer			Not NULL , 
										BranchID			Integer			Not NULL , 
										TranNo				Varchar(25)		Not NULL , 
										TranDate			DateTime		Not NULL , 
										TranType			Varchar(1)		Not NULL , 
										POSID				Varchar(50)		         , 
										DeviceNo			Integer			Not NULL , 
										SysCustomerID		Integer			Not NULL , 
										CustomerName		Varchar(255)	Not NULL , 
										GiftMemberCardNo	Varchar(25)		         , 
										EarningInfo			Varchar(100)	         , 
										SellerName			Varchar(50)		Not NULL , 
										LastDateModified	DateTime		Not NULL , 
										LastUserModified	Varchar(50)		Not NULL , 
										FCancel				numeric(1,0)	Not NULL , 
										TranTaxType			Varchar(1)		Not NULL , 
										TaxRate				numeric(8,4)	         , 
										CountTradDisc		Integer			Not NULL , 
										SubTotalNoneVat		numeric(18,4)	Not NULL , 
										TotalTradDiscNoneVat	numeric(18,4)	Not NULL , 
										TotalNoneVat		numeric(18,4)	Not NULL , 
										SubTotalHaveVat		numeric(18,4)	Not NULL , 
										TotalTradDiscHaveVat	numeric(18,4)	Not NULL , 
										TotalHaveVat		numeric(18,4)	Not NULL , 
										Total				numeric(18,4)	Not NULL , 
										FmlServiceCharge	Varchar(100)	         , 
										ServiceCharge		numeric(18,4)	Not NULL , 
										TotalVat			numeric(18,4)	Not NULL , 
										GrandTotal			numeric(18,4)	Not NULL , 
										PaymentFractional	numeric(18,4)	Not NULL , 
										GrandPayment		numeric(18,4)	Not NULL , 
										SummaryPayment		numeric(18,4)	Not NULL , 
										Change				numeric(18,4)	Not NULL , 
										Tips				numeric(18,4)	Not NULL , 
										TotalPointEarning	numeric(18,4)	Not NULL , 
										PrintCounter		Integer			Not NULL , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKTrans PRIMARY KEY (MerchantID Asc, BranchID Asc, TranNo Asc),
										CONSTRAINT R1Trans FOREIGN KEY (merchantid, branchid) REFERENCES branch(merchantid, branchid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Index XIE1Trans On Trans
										(MerchantID Asc, SysCustomerID Asc);

									Create Index XIE2Trans On Trans
										(MerchantID Asc, CustomerName Asc);

									Create Index XIE3Trans On Trans
										(MerchantID Asc, BranchID Asc, TranDate Asc);

									Create Table TranDetailItem
									(
										MerchantID			Integer			Not NULL , 
										BranchID			Integer			Not NULL , 
										TranNo				Varchar(25)		Not NULL , 
										DetailNo			Integer			Not NULL , 
										CumulativeSum		Integer			         , 
										ItemName			Varchar(255)	Not NULL , 
										SysItemID			Integer			         , 
										SaleItemType		Varchar(1)		Not NULL , 
										FProcess			numeric(1,0)	Not NULL , 
										TaxType				Varchar(1)		Not NULL , 
										UnitName			Varchar(50)		         , 
										SizeName			Varchar(50)		         , 
										Quantity			numeric(18,4)	Not NULL , 
										Weight				numeric(18,4)	         , 
										WeightUnitName		Varchar(50)		         , 
										PricePerWeight		numeric(18,4)	         , 
										Price				numeric(18,4)	Not NULL , 
										SubAmount			numeric(18,4)	Not NULL , 
										FmlDiscountRow		Varchar(50)		         , 
										Discount			numeric(18,4)	Not NULL , 
										Amount				numeric(18,4)	Not NULL , 
										VatAmount			numeric(18,4)	Not NULL , 
										TaxBaseAmount		numeric(18,4)	Not NULL , 
										TotalCost			numeric(18,4)	Not NULL , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKTranDetailItem PRIMARY KEY (MerchantID Asc, BranchID Asc, TranNo Asc, DetailNo Asc),
										CONSTRAINT R1TranDetailItem FOREIGN KEY (merchantid, branchid, tranno) REFERENCES trans(merchantid, branchid, tranno)
											ON DELETE No Action
											ON UPDATE No Action,
										CONSTRAINT R2TranDetailItem FOREIGN KEY (merchantid, sysitemid) REFERENCES item(merchantid, sysitemid)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Table TranPayment
									(
										MerchantID			Integer			Not NULL , 
										BranchID			Integer			Not NULL , 
										TranNo				Varchar(25)		Not NULL , 
										PaymentNo			Integer			Not NULL , 
										PaymentType			Varchar(2)		Not NULL , 
										PaymentAmount		numeric(18,4)	Not NULL , 
										CreditCardType		Varchar(10)		         , 
										CardNo				Varchar(25)		         , 
										ExprieDateYYYYMM	Varchar(6)		         , 
										ApproveCode			Varchar(25)		         , 
										TotalRedeemPoint	numeric(18,4)	         , 
										EPaymentType		Varchar(4)		Not NULL , 
										RequestNum			Varchar(50)		         , 
										RequestDateTime		DateTime		         , 
										FEPaymentCancel		numeric(1,0)	         , 
										ReferenceNo1		Varchar(255)	         , 
										ReferenceNo2		Varchar(255)	         , 
										ReferenceNo3		Varchar(255)	         , 
										ReferenceNo4		Varchar(255)	         , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKTranPayment PRIMARY KEY (MerchantID Asc, BranchID Asc, TranNo Asc, PaymentNo Asc),
										CONSTRAINT R1TranPayment FOREIGN KEY (merchantid, branchid, tranno) REFERENCES trans(merchantid, branchid, tranno)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Create Table TranTradDiscount
									(
										MerchantID			Integer			Not NULL , 
										BranchID			Integer			Not NULL , 
										TranNo				Varchar(25)		Not NULL , 
										TradDiscountNo		Integer			Not NULL , 
										PriorityNo			Integer			Not NULL , 
										FOnTop				numeric(1,0)	Not NULL , 
										DiscountType		Varchar(2)		Not NULL , 
										FmlDiscount			Varchar(50)		Not NULL , 
										TradeDiscNoneVat	numeric(18,4)	Not NULL , 
										TradeDiscHaveVat	numeric(18,4)	Not NULL , 
										TotalRedeemPoint	numeric(18,4)	         , 
										ReferenceNo1		Varchar(255)	         , 
										ReferenceNo2		Varchar(255)	         , 
										ReferenceNo3		Varchar(255)	         , 
										Comments			Varchar(255)	         ,
										CONSTRAINT XPKTranTradDiscount PRIMARY KEY (MerchantID Asc, BranchID Asc, TranNo Asc, TradDiscountNo Asc),
										CONSTRAINT R1TranTradDiscount FOREIGN KEY (merchantid, branchid, tranno) REFERENCES trans(merchantid, branchid, tranno)
											ON DELETE No Action
											ON UPDATE No Action
									) ;

									Insert into DataBaseInfo(KeyDBInfo,DataDBInfo) values ('AppDB','MerchantDB');
									Insert into DataBaseInfo(KeyDBInfo,DataDBInfo) values ('ProMaxxErVersion', '3.0.106')";
		#endregion

		public bool CreateTable(string DatabaseName)
		{
			try
			{
				if (DatabaseName == "MerchantDB.db")
				{
					using (var db = new MerchantDB(DataCashingAll.Pathdb))
					{
						if (!TableExists(DatabaseName))
						{
							var y = db.Query<MerchantDB>(createTable);
							return true;
						}
						else
						{
							Console.WriteLine("Table has exist");
							return false;
						}
					}
				}
                else
                {
					using (var db = new PoolDB(DataCashingAll.Pathdbpool))
					{
						if (!TableExists("PoolDB.db"))
						{
							var y = db.Query<PoolDB>(createTable);
							return true;
						}
						else
						{
							Console.WriteLine("Table has exist");
							return false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			};
		}


		public Boolean TableExists( string DatabaseName)
		{
			try
			{
                if (DatabaseName == "MerchantDB.db")
                {
                    using (var db = new MerchantDB(DataCashingAll.Pathdb))
                    {
                        var tableInfo = db.Query<MerchantDB>("SELECT * FROM sqlite_master WHERE type = 'table'  AND name != 'sqlite_sequence';").Count();
                        if (Convert.ToInt32(tableInfo) > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    } 
                }
                else
                {
					using (var db = new PoolDB(DataCashingAll.Pathdbpool))
					{
						var tableInfo = db.Query<PoolDB>("SELECT * FROM sqlite_master WHERE type = 'table'  AND name != 'sqlite_sequence';").Count();
						if (Convert.ToInt32(tableInfo) > 0)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				var x = ex.Message;
				return false;
			}

		}
	}
}
