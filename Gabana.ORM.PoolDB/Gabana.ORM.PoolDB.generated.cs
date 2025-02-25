//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1591

using System;
using System.Linq;
using Gabana.ORM.MerchantDB;
using LinqToDB;
using LinqToDB.Mapping;

namespace Gabana.ORM.PoolDB
{
	/// <summary>
	/// Database       : PoolDBT4
	/// Data Source    : PoolDBT4
	/// Server Version : 3.24.0
	/// </summary>
	public partial class PoolDB : LinqToDB.Data.DataConnection
	{
		public ITable<Amphure>      Amphures     { get { return this.GetTable<Amphure>(); } }
		public ITable<DataBaseInfo> DataBaseInfo { get { return this.GetTable<DataBaseInfo>(); } }
		public ITable<District>     Districts    { get { return this.GetTable<District>(); } }
		public ITable<Geography>    Geographies  { get { return this.GetTable<Geography>(); } }
		public ITable<Province>     Provinces    { get { return this.GetTable<Province>(); } }

		public PoolDB(string pathDB)
			: base(LinqToDB.ProviderName.SQLiteMS, DbHelper.PoolDB(pathDB))
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();
	}

	[Table("Amphures")]
	public partial class Amphure
	{
		[PrimaryKey, NotNull] public long   AmphuresId     { get; set; } // integer
		[Column,     NotNull] public string AmphuresCode   { get; set; } // varchar(10)
		[Column,     NotNull] public string AmphuresNameTH { get; set; } // varchar(150)
		[Column,     NotNull] public string AmphuresNameEN { get; set; } // varchar(150)
		[Column,     NotNull] public long   ProvincesId    { get; set; } // integer
	}

	[Table("DataBaseInfo")]
	public partial class DataBaseInfo
	{
		[PrimaryKey, NotNull    ] public string KeyDBInfo  { get; set; } // varchar(50)
		[Column,        Nullable] public string DataDBInfo { get; set; } // varchar(255)
	}

	[Table("Districts")]
	public partial class District
	{
		[PrimaryKey, NotNull] public long   DistrictsId     { get; set; } // integer
		[Column,     NotNull] public string ZipCode         { get; set; } // varchar(10)
		[Column,     NotNull] public string DistrictsNameTH { get; set; } // varchar(150)
		[Column,     NotNull] public string DistrictsNameEN { get; set; } // varchar(150)
		[Column,     NotNull] public long   AmphuresId      { get; set; } // integer
	}

	[Table("Geographies")]
	public partial class Geography
	{
		[PrimaryKey, NotNull] public long   GeographyId { get; set; } // integer
		[Column,     NotNull] public string Name        { get; set; } // varchar(50)
	}

	[Table("Provinces")]
	public partial class Province
	{
		[PrimaryKey, NotNull] public long   ProvincesId     { get; set; } // integer
		[Column,     NotNull] public string ProvincesCode   { get; set; } // varchar(10)
		[Column,     NotNull] public string ProvincesNameTH { get; set; } // varchar(150)
		[Column,     NotNull] public string ProvincesNameEN { get; set; } // varchar(150)
		[Column,     NotNull] public long   GeographyId     { get; set; } // integer
	}

	public static partial class TableExtensions
	{
		public static Amphure Find(this ITable<Amphure> table, long AmphuresId)
		{
			return table.FirstOrDefault(t =>
				t.AmphuresId == AmphuresId);
		}

		public static DataBaseInfo Find(this ITable<DataBaseInfo> table, string KeyDBInfo)
		{
			return table.FirstOrDefault(t =>
				t.KeyDBInfo == KeyDBInfo);
		}

		public static District Find(this ITable<District> table, long DistrictsId)
		{
			return table.FirstOrDefault(t =>
				t.DistrictsId == DistrictsId);
		}

		public static Geography Find(this ITable<Geography> table, long GeographyId)
		{
			return table.FirstOrDefault(t =>
				t.GeographyId == GeographyId);
		}

		public static Province Find(this ITable<Province> table, long ProvincesId)
		{
			return table.FirstOrDefault(t =>
				t.ProvincesId == ProvincesId);
		}
	}
}

#pragma warning restore 1591
 