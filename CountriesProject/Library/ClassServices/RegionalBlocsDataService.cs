namespace Library.ClassServices
{
    using Library.Classes;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class RegionalBlocsDataService
    {
        private SQLiteCommand command;
        private SQLiteConnection connection;
        private DialogService dialogService;

        /// <summary>
        /// SQL Table creation for RegionalBlocs
        /// </summary>
        public RegionalBlocsDataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\RegionalBlocs.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source=" + path);
                connection.Open();

                string sqlCreate = "create table if not exists regionalBlocs(acronym varchar(20), name varchar(25), countryCode varchar(50))";

                command = new SQLiteCommand(sqlCreate, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Saving data of RegionalBlocs in SQL with insert command
        /// </summary>
        /// <param name="regionalBlocs"></param>
        public void SaveData(List<RegionalBlocs> regionalBlocs, string countryCode)
        {
            try
            {
                foreach (var bloc in regionalBlocs)
                {
                    string sqlInsert = string.Format("insert into regionalBlocs(acronym, name, countryCode) values ('{0}', \"{1}\", '{2}')", bloc.Acronym, bloc.Name, countryCode);

                    command = new SQLiteCommand(sqlInsert, connection);
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
        /// Get some data from SQL table RegionalBlocs
        /// </summary>
        /// <returns>regionalBlocs</returns>
        public List<RegionalBlocs> GetData()
        {
            List<RegionalBlocs> regionalBlocs = new List<RegionalBlocs>();

            try
            {
                string sqlSelect = "select acronym, name, countryCode from regionalBlocs";

                command = new SQLiteCommand(sqlSelect, connection);
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    regionalBlocs.Add(new RegionalBlocs
                    {
                        Acronym = (string) reader["acronym"],
                        Name = (string)reader["name"],
                        CountryCode = (string)reader["countryCode"],
                    });
                }
                connection.Close();
                return regionalBlocs;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Delete data from SQL table RegionalBlocs
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "delete from regionalBlocs";

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
