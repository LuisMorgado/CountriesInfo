namespace Library.ClassServices
{
    using Library.Classes;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class LanguageDataService
    {
        private SQLiteConnection connection;
        private SQLiteCommand command;
        private DialogService dialogService;

        /// <summary>
        /// SQL Table creation for languages
        /// </summary>
        public LanguageDataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Languages.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlCommand = "create table if not exists languages(Iso639_1 varchar(25), Iso639_2 varchar(25), name varchar(50), nativeName varchar(50), countryCode varchar(50))";

                command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Saving data of languages in SQL with insert command
        /// </summary>
        /// <param name="languages"></param>
        public void SaveData(List<Language> languages, string countryCode)
        {
            try
            {
                foreach (var language in languages)
                {
                    string sql = string.Format("insert into languages(Iso639_1, Iso639_2, name, nativeName, countryCode) values ('{0}', '{1}', \"{2}\", \"{3}\", '{4}')", language.Iso639_1, language.Iso639_2, language.Name, language.NativeName, countryCode);

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
        /// Get some data from SQL table languages
        /// </summary>
        /// <returns>language list</returns>
        public List<Language> GetData()
        {
            List<Language> languages = new List<Language>();

            try
            {
                string sql = "select Iso639_1, Iso639_2, name, nativeName, countryCode from languages";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    languages.Add(new Language
                    {
                        Iso639_1 = (string)reader["Iso639_1"],
                        Iso639_2 = (string)reader["Iso639_2"],
                        Name = (string)reader["name"],
                        NativeName = (string)reader["nativeName"],
                        CountryCode = (string)reader["countryCode"],
                    });
                }

                connection.Close();
                return languages;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Delete data from SQL table languages
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "delete from languages";
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
