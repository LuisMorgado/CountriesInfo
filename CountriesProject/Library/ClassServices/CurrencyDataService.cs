namespace Library.ClassServices
{
    using Library.Classes;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class CurrencyDataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;


        /// <summary>
        /// SQL Table creation for Currencies
        /// </summary>
        public CurrencyDataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Currencies.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlCommand = "create table if not exists currencies (code varchar(30), name varchar(30), symbol varchar(5), countryCode varchar(50))";

                command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Saving data of Currencies in SQL with insert command
        /// </summary>
        /// <param name="currencies"></param>
        public void SaveData(List<Currency> currencies, string countryCode)
        {
            try
            {
                foreach (var currency in currencies)
                {
                    string sql = string.Format("insert into currencies (code, name, symbol, countryCode) select '{0}', \"{1}\", '{2}', '{3}' where not exists(select 1 from currencies where code = '{0}' and name like \"{1}\")", currency.Code, currency.Name, currency.Symbol, countryCode);

                    command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Get same data from SQL table Currencies
        /// </summary>
        /// <returns>Currency list</returns>
        public List<Currency> GetData()
        {
            List<Currency> currencies = new List<Currency>();

            try
            {
                string sql = "select code, name, symbol, countryCode from currencies";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    currencies.Add(new Currency
                    {
                        Code = (string)reader["code"],
                        Name = (string)reader["name"],
                        Symbol = (string)reader["symbol"],
                        CountryCode = (string)reader["countryCode"],
                    });
                }
                connection.Close();
                return currencies;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Delete data from SQL table Currencies
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "delete from currencies";
                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }

        }
    }
}
