namespace CountriesInfo
{
    using Library.Classes;
    using Library.ClassServices;
    using Microsoft.Maps.MapControl.WPF;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using Svg;
    using System.Windows.Media.Imaging;
    using System.Drawing;
    using System.Windows.Interop;
    using Point = System.Windows.Point;

    public partial class MainWindow : Window
    {
        private List<Country> countries;
        private DataService dataService;
        private List<Currency> currencies;
        private NetworkService networkService;
        private DialogService dialogService;

        public MainWindow()
        {
            InitializeComponent();
            dataService = new DataService();
            networkService = new NetworkService();
            dialogService = new DialogService();
            LoadCountriesBD();
        }

        private async void LoadCountriesBD()
        {
            bool load;

            lbl_loadingInfo.Content = "Updating info...";

            var connetion = networkService.CheckConnection();

            if (!connetion.IsSuccess)
            {
                LoadLocalCountries();
                load = false;
                return;
            }
            else
            {
                await LoadCountriesAPI();
                load = true;
            }

            if (countries.Count == 0)
            {
                lbl_loadingInfo.Content = "There is no internet connection" + Environment.NewLine + "please try later";
                return;
            }

            lbl_loadingInfo.Content = "Countries updated";

            if (load)
            {
                lbl_status.Content = string.Format("Taxas carregadas da internet em {0:f}", DateTime.Now);
            }
            else
            {
                lbl_status.Content = string.Format("Taxas carregadas da dase de dados");
            }


            progressBar.Value = 100;
        }

        private void LoadLocalCountries()
        {
            countries = dataService.GetData();
        }

        private async Task LoadCountriesAPI()
        {
            progressBar.Value = 0;

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
            Point mousePosition = e.GetPosition(stackPanelMap);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = worldMap.ViewportPointToLocation(mousePosition);

            string baseURL = "http://dev.virtualearth.net";
            string controller = string.Format("/REST/v1/Locations/{0}?inclnb=1&incl=ciso2&key={1}", pinLocation.Latitude.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "," + pinLocation.Longitude.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), "Aq3R2llqn0c86NeqB - LbOwxVC8hodxNHBckFcfnxDFUA5MvYUU0CGClXctL6VTdX");

            Response response = await LocationAPIService.GetLocation(baseURL, controller);

            LocationResponse locationResponse = (LocationResponse)response.Result;

            try
            {
                string isoCode = locationResponse.ResourceSets[0].Resources[0].Address.CountryRegionIso2;

                Country countryCode = new Country();

                foreach (var country in countries)
                {
                    if (country.Alpha2Code == isoCode)
                    {
                        countryCode = country;
                    }
                }

                if (countryCode == null)
                {
                    dialogService.ShowMessage("Error", "Could not find a macth");
                }
                else
                {
                    lbl_countryName.Content = countryCode.Name;
                    lbl_countryCapital.Content = countryCode.Capital;
                    lbl_countryPopulation.Content = countryCode.Population;
                    lbl_regiao.Content = countryCode.Region;
                    lbl_subRegion.Content = countryCode.Subregion;
                    lbl_gini.Content = countryCode.Gini;
                    imgFlags.Source = Bitmap2BitmapImage(SvgDocument.Open($@"Data\imgs\{countryCode.Alpha2Code}.svg").Draw());
                }
            }
            catch (Exception r)
            {
                dialogService.ShowMessage("Error", r.Message);
            }
        }

        private void Btn_seacrh_Click(object sender, RoutedEventArgs e)
        {


            foreach (var country in countries)
            {
                if (txt_search.Text == country.Name)
                {
                    lbl_countryName.Content = country.Name;
                    lbl_countryCapital.Content = country.Capital;
                    lbl_countryPopulation.Content = country.Population;
                    lbl_regiao.Content = country.Region;
                    lbl_subRegion.Content = country.Subregion;
                    lbl_gini.Content = country.Gini;
                    imgFlags.Source = Bitmap2BitmapImage(SvgDocument.Open($@"Data\imgs\{country.Alpha2Code}.svg").Draw());
                }
            }
        }

        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
