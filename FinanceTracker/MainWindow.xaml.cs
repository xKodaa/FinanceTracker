using FinanceTracker.Config;
using FinanceTracker.Graphics.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestSharp;
using FinanceTracker.Utility;
using System.Net.Http;
using System.Net.Http.Headers;


namespace FinanceTracker
{
    public partial class MainWindow : Window
    {
        static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            TestCryptoApiAsync();
            //Launcher launcher = new(this);
            //launcher.Launch();
            
            //InitializeComponent();
        }
        // 21c34367-d5a8-40af-99dd-e7952855e87f

        private async void TestCryptoApiAsync()
        {
            /*var options = new RestClientOptions("https://api.tokenmetrics.com/v2/tokens?token_id=3375%2C3306&symbol=BTC%2CETH&category=yield%20farming%2Cdefi&exchange=binance%2Cgate&limit=1000&page=0");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("api_key", "tm-fb2e32a1-6465-4218-8e40-be1e9507de67");
            var response = await client.GetAsync(request);

            Util.ShowInfoMessageBox(response.Content.ToString());*/

            client.BaseAddress = new Uri("https://api.coincap.io");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("/v2/assets");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // Výpis odpovědi (JSON string obsahující informace o kryptoměnách a jejich cenách)
            Console.WriteLine(responseBody);
            Util.ShowInfoMessageBox(responseBody);
        }

        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ProfilePage()); 
        }
    }
}