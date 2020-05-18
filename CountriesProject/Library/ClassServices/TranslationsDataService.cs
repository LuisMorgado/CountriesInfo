namespace Library.ClassServices
{
    using Library.Classes;
    using System;
    using System.Data.SQLite;
    using System.IO;

    public class TranslationsDataService
    {
        private DialogService dialogService;
        private SQLiteConnection connection;
        private SQLiteCommand command;

        /// <summary>
        /// SQL Table creation for Translations
        /// (Constroctor)
        /// </summary>
        public TranslationsDataService()
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

                string sqlCommand = "create table if not exists translations(De char(2), Es char(2), Fr char(2), Ja char(2), It char(2), Br char(2), Pt char(2), Nl char(2), Hr char(2), Fa char(2), countryCode)";

                command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
            }
        }

        /// <summary>
        /// Saving data of translations in SQL with insert command
        /// </summary>
        /// <param name="translations"></param>
        /// <param name="countryCode"></param>
        public void SaveData(Translations translations, string countryCode)
        {
            try
            {
                string sqlComand = string.Format("insert into translations(De, Es, Fr, Ja, It, Br, Pt, Nl, Hr, Fa, countryCode) values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')", translations.De, translations.Es, translations.Fr, translations.Ja, translations.It, translations.Br, translations.Pt, translations.Nl, translations.Hr, translations.Fa, countryCode);
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                throw;
            }
        }


        //Verificar se esta bem feito
        /// <summary>
        /// Get some data from SQL table Translations
        /// </summary>
        /// <returns>language list</returns>
        public Translations GetTranslations()
        {
            Translations translations = new Translations();

            try
            {
                string sql = "select De, Es, Fr, Ja, It, Br, Pt, Nl, Hr, Fa, countryCode";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    translations = new Translations
                    {
                        De = (string)reader["De"], 
                        Es = (string)reader["Es"],
                        Fr = (string)reader["Fr"],
                        Ja = (string)reader["Ja"],
                        It = (string)reader["It"],
                        Br = (string)reader["Br"],
                        Pt = (string)reader["Pt"],
                        Nl = (string)reader["Nl"],
                        Hr = (string)reader["Hr"],
                        Fa = (string)reader["Fa"],
                        CountryCode = (string)reader["countryCode"],
                    };
                }
                connection.Close();
                return translations;
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Delete data from SQL table Translations
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "delete from translations";
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
