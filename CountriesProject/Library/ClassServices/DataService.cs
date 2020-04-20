namespace Library.ClassServices
{
    using Library.Classes;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class DataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;
        private CurrencyDataService currencyDataService;
        private LanguageDataService languageDataService;
        private RegionalBlocsDataService regionalBlocsDataService;

        /// <summary>
        /// SQL Table creation for Countries
        /// </summary>
        public DataService()
        {
            dialogService = new DialogService();
           
            //languageDataService = new LanguageDataService();

            //Library who work with folders
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlCommand = "create table if not exists countries(name varchar(30), alpha2Code varchar(50), alpha3Code varchar(50), capital varchar(30), region varchar(30), subregion varchar(30), population int, demonym varchar(30), area numeric, gini numeric, nativeName varchar(50), numericCode varchar(50) primary key, flag blob, cioc varchar(50))";

                command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Saving data of Countries in SQL with insert command
        /// </summary>
        /// <param name="countries"></param>
        public void SaveData(List<Country> countries)
        {
            try
            {
                foreach (var country in countries)
                {   
                    string sql = string.Format("insert into countries (name, alpha2Code, alpha3Code, capital, region, subregion, population, demonym, area, gini, nativeName, numericCode, flag, cioc) values (\"{0}\", '{1}', '{2}', \"{3}\", '{4}', '{5}', {6}, '{7}', '{8}', '{9}', \"{10}\", '{11}', '{12}', '{13}')", country.Name, country.Alpha2Code, country.Alpha3Code, country.Capital, country.Region, country.Subregion, country.Population, country.Demonym, country.Area, country.Gini, country.NativeName, country.NumericCode, country.Flag, country.Cioc);

                    command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();

                    currencyDataService = new CurrencyDataService();
                    currencyDataService.SaveData(country.Currencies, country.NumericCode);

                    languageDataService = new LanguageDataService();
                    languageDataService.SaveData(country.Languages, country.NumericCode);

                    regionalBlocsDataService = new RegionalBlocsDataService();
                    regionalBlocsDataService.SaveData(country.RegionalBlocs, country.NumericCode);
                }
                connection.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Get some data from SQL table Countries
        /// </summary>
        /// <returns>Countries list</returns>
        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();

            try
            {
                string sql = "select name, alpha2Code, alpha3Code, capital, region, subregion, population, demonym, area, gini, nativeName, numericCode, flag, cioc from countries";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    countries.Add
                        (new Country
                        {
                            Name = (string) reader["name"],
                            Alpha2Code = (string) reader["alpha2Code"],
                            Alpha3Code = (string) reader["alpha3Code"],
                            Capital = (string) reader["capital"],
                            Region = (string) reader["region"],
                            Subregion = (string) reader["subregion"],
                            Population = (int) reader["population"],
                            Demonym = (string) reader["demonym"],
                            Area = (double) reader["area"],
                            Gini = (double) reader["gini"],
                            NativeName = (string) reader["nativeLanguage"],
                            NumericCode = (string) reader["numericCode"],
                            Flag = (Uri) reader["flag"],
                            Cioc = (string) reader["cioc"],
                        });
                }

                connection.Close();
                return countries;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Delete data from SQL table Countries
        /// </summary>
        public void DeleteData()
        {
            
            try
            {
                string sql = "delete from countries";

                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                currencyDataService = new CurrencyDataService();
                currencyDataService.DeleteData();
                languageDataService = new LanguageDataService();
                languageDataService.DeleteData();
                regionalBlocsDataService = new RegionalBlocsDataService();
                regionalBlocsDataService.DeleteData();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }
    }
}
