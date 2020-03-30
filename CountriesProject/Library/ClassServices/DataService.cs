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

        public DataService()
        {
            dialogService = new DialogService();

            //Directory é a biblioteca que trabalha com pastas

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Countries.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlCommand = "create table if not exists countries(name varchar(30), alpha2Code varchar(50), alpha3Code varchar(50), capital varchar(30), region varchar(30), subregion varchar(30), population int, demonym varchar(30), area numeric, gini numeric, nativeName varchar(50), numericCode varchar(50), flag blob, cioc varchar(50))";

                command = new SQLiteCommand(sqlCommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        public void SaveData(List<Country> Countries)
        {
            try
            {
                foreach (var country in Countries)
                {
                    //string sql = string.Format("insert into countries(name, alpha2Code, alpha3Code, capital, region, subregion, population, demonym, area, gini, nativeName, numericCode, flag, cioc) values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}')", country.Name, country.Alpha2Code, country.Alpha3Code, country.Capital, country.Region, country.Subregion, country.Population, country.Demonym, country.Area, country.Gini, country.NativeName, country.NumericCode, country.Flag, country.Cioc);

                    string sql = string.Format("insert into countries (name) values ('{{0}}')", country.Name);

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

        public void DeleteData()
        {
            try
            {
                string sql = "delete from countries";

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
