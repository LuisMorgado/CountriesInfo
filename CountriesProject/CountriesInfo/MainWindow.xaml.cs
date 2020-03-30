namespace CountriesInfo
{
    using Library.Classes;
    using Library.ClassServices;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Documents;

    public partial class MainWindow : Window
    {
        private List<Country> countries;
        private DataService dataService;

        public MainWindow()
        {
            InitializeComponent();
            dataService = new DataService();

            LoadCountries();

        }

        private async void LoadCountries()
        {
            Response response = await APIService.GetCountries("http://restcountries.eu", "/rest/v2/all");
            countries = (List<Country>)response.Result;

            cbb_countries.ItemsSource = countries;

            dataService.DeleteData();

            dataService.SaveData(countries);
        }

    }
}
