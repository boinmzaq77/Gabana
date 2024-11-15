using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.DataProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.iOS
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;

    }

    public class MySettings : ILinqToDBSettings
    {
        public static string pathDB;
        public MySettings(string Path)
        {
            pathDB = Path;

        }

        public IEnumerable<IDataProviderSettings> DataProviders
        {
            get { yield break; }
        }

        //Microsoft.Data.Sqlite.SqliteDataReader sqliteDataReader;

        public string DefaultConfiguration => "SQLite"; // lets set your configuration as default, so you can call just new DataContext() or new DataConnection()
        public string DefaultDataProvider => "SQLite"; // and set default database type

        //private static IDataProvider GetDataProvider()
        //{
        //    return new LinqToDB.DataProvider.SQLite.SQLiteDataProvider();
        //}

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return

                    new ConnectionStringSettings
                    {
                        Name = "MerchantDB",
                        ProviderName = ProviderName.SQLiteMS,
                        ConnectionString = "Data Source = " + pathDB + "; "
                        
                    };
            }
        }

    }
}
