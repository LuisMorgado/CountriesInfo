namespace CountriesInfo
{
    using Library.Classes;
    using Library.ClassServices;
    using Microsoft.Maps.MapControl.WPF;
    using System.Collections.Generic;
    using System.Windows;

    public partial class MainWindow : Window
    {
        private List<Country> countries;
        private DataService dataService;

        public MainWindow()
        {
            InitializeComponent();
            dataService = new DataService();

            //LoadCountries();
        }

        private async void LoadCountries()
        {
            Response response = await APIService.GetCountries("http://restcountries.eu", "/rest/v2/all");
            countries = (List<Country>)response.Result;

            dataService.DeleteData();

            dataService.SaveData(countries);
        }


        private async void WorldMap_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = worldMap.ViewportPointToLocation(mousePosition);

            string baseURL = "http://dev.virtualearth.net";
            string controller = string.Format("/REST/v1/Locations/{0}?inclnb=1&incl=ciso2&key={1}", pinLocation.Latitude.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "," + pinLocation.Longitude.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), "Aq3R2llqn0c86NeqB - LbOwxVC8hodxNHBckFcfnxDFUA5MvYUU0CGClXctL6VTdX");

            Response response = await LocationAPIService.GetLocation(baseURL, controller);

            LocationResponse locationResponse = (LocationResponse)response.Result;
        }

        //private void LoadRegionalBlocs()
        //{
        //    RegionalBlocsDataService regionalBlocsDataService = new RegionalBlocsDataService();

        //    List<RegionalBlocs> regionalBlocs = regionalBlocsDataService.GetData();

        //    cbb_regionalBlocs.ItemsSource = regionalBlocs;
        //}

    }
}
