﻿namespace CountriesInfo
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
    using System.Net;
    using System.Linq;

    public partial class MainWindow : Window
    {
        private List<Country> countries;
        private DataService dataService;
        private NetworkService networkService;
        private DialogService dialogService;
        private APIService apiService;

        public MainWindow()
        {
            InitializeComponent();
            dataService = new DataService();
            networkService = new NetworkService();
            dialogService = new DialogService();
            apiService = new APIService();
            countries = new List<Country>();
            LoadCountriesAPI_DB();
        }

        /// <summary>
        /// Load countries from API or Database in case the download from API doesnt't work, countries will be downloaded from Database
        /// </summary>
        private async void LoadCountriesAPI_DB()
        {
            bool load;

            var connetion = networkService.CheckConnection();

            if (!connetion.IsSuccess)
            {
                LoadLocalCountries();
                lbl_loadingInfo.Content = "Countries loades from Data Base";
                progressBar.Visibility = Visibility.Hidden;
                progressPercentage.Visibility = Visibility.Hidden;
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
                lbl_status.Content = string.Format("Counties loaded from API in {0:f}", DateTime.Now);
            }
            else
            {
                lbl_status.Content = string.Format("Counties loaded from Data Base");
            }
        }

        private void LoadLocalCountries()
        {
            countries = dataService.GetData();
        }

        private void ReportProgress(object sender, ProgressReport e)
        {
            progressBar.Value = e.Percentage;
            if (e.Percentage != 100)
            {
                lbl_loadingInfo.Content = "Loading countries from API...";
            }
            else
            {
                lbl_loadingInfo.Content = "Countries loaded from API";
            }
        }

        private void ReportProgressDB(object sender, ProgressReport e)
        {
            progressBar.Value = e.Percentage;

            if (e.Percentage != 100)
            {
                lbl_loadingInfo.Content = "Saving countries to Database...";
            }
            else
            {
                lbl_loadingInfo.Content = "Countries saved to Database";
            }
        }

        /// <summary>
        /// Load countries from API
        /// </summary>
        /// <returns></returns>
        private async Task LoadCountriesAPI()
        {

            Progress<ProgressReport> apiProgress = new Progress<ProgressReport>();
            apiProgress.ProgressChanged += ReportProgress;

            Response response = await apiService.GetCountries("http://restcountries.eu", "/rest/v2/all", apiProgress);

            countries = (List<Country>)response.Result;

            dataService.DeleteData();

            Progress<ProgressReport> bdProgress = new Progress<ProgressReport>();
            bdProgress.ProgressChanged += ReportProgressDB;

            await dataService.SaveData(countries, bdProgress);
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
                    lbl_countryName.Content = MissInfo(countryCode.Name);
                    lbl_countryCapital.Content = MissInfo(countryCode.Capital);
                    lbl_countryPopulation.Content = MissInfo(countryCode.Population.ToString());
                    lbl_regiao.Content = MissInfo(countryCode.Region);
                    lbl_subRegion.Content = MissInfo(countryCode.Subregion);
                    lbl_gini.Content = MissInfo(countryCode.Gini.ToString());
                    list_currencies.ItemsSource = countryCode.Currencies;
                    lbl_symbol.Content = string.Empty;
                    foreach (var currency in countryCode.Currencies)
                    {
                        lbl_symbol.Content += currency.Symbol;
                    }

                    try
                    {
                        imgFlags.Source = Bitmap2BitmapImage(SvgDocument.Open($@"Data\imgs\{countryCode.Alpha2Code}.svg").Draw());
                    }
                    catch (Exception)
                    {

                        dialogService.ShowMessage("Error", "Flag not found");
                        string fileName = $"{Environment.CurrentDirectory}\\Data\\Imgs\\Default.svg";
                        imgFlags.Source = Bitmap2BitmapImage(SvgDocument.Open(fileName).Draw());
                    }

                }
                txt_search.Text = string.Empty;
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Error", ex.Message);
            }
        }

        private void Btn_seacrh_Click(object sender, RoutedEventArgs e)
        {
            foreach (var country in countries)
            {
                if (txt_search.Text == country.Name)
                {
                    lbl_countryName.Content = MissInfo(country.Name);
                    lbl_countryCapital.Content = MissInfo(country.Capital);
                    lbl_countryPopulation.Content = MissInfo(country.Population.ToString());
                    lbl_regiao.Content = MissInfo(country.Region);
                    lbl_subRegion.Content = MissInfo(country.Subregion);
                    lbl_gini.Content = MissInfo(country.Gini.ToString());
                    list_currencies.ItemsSource = country.Currencies;
                    lbl_symbol.Content = string.Empty;
                    foreach (var currency in country.Currencies)
                    {
                        lbl_symbol.Content += currency.Symbol;
                    }

                    try
                    {
                        imgFlags.Source = Bitmap2BitmapImage(SvgDocument.Open($@"Data\imgs\{country.Alpha2Code}.svg").Draw());
                    }
                    catch (Exception)
                    {
                        dialogService.ShowMessage("Error", "Flag not found");
                        string fileName = $"{Environment.CurrentDirectory}\\Data\\Imgs\\Default.svg";
                        imgFlags.Source = Bitmap2BitmapImage(SvgDocument.Open(fileName).Draw());
                    }

                    return;
                }
            }

            dialogService.ShowMessage("Info", "Country not found");
        }

        /// <summary>
        ///Convert flags 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// In case the info doesn't exist
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string MissInfo(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                return "No info";
            }
            else if (info == "0")
            {
                return "No info";
            }
            else
            {
                return info;
            }
        }

    }
}
