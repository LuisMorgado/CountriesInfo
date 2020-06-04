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

            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlCommand = "create table if not exists currencies (code varchar(30), name varchar(30), symbol varchar(5), countryCode varchar(50), foreign key(countryCode) references countries(numericCode))";

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
        public void SaveData(List<Currency> currencies, string alpha3Code)
        {
            try
            {
                foreach (var currency in currencies)
                {
                    string sql = string.Format("insert into currencies (code, name, symbol, countryCode) values ('{0}', \"{1}\", '{2}', '{3}')", currency.Code, currency.Name, currency.Symbol, alpha3Code);

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
                        Code = reader["code"].ToString(),
                        Name = reader["name"].ToString(),
                        Symbol = reader["symbol"].ToString(),
                        CountryCode = reader["countryCode"].ToString(),
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

        /// <summary>
        /// getting currencies through an alphaCod3 with a select
        /// </summary>
        /// <param name="alpha3Code"></param>
        /// <returns></returns>
        public List<Currency> GetCurrenciesByCountryCode(string alpha3Code)
        {
            List<Currency> currencies = new List<Currency>();
            
            try
            {
                string sql = $"select code, name, symbol, countryCode from currencies where countryCode = '{alpha3Code}'";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    currencies.Add(new Currency
                    {
                        Code = reader["code"].ToString(),
                        Name = reader["name"].ToString(),
                        Symbol = reader["symbol"].ToString(),
                        CountryCode = reader["countryCode"].ToString(),
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
    }
}
