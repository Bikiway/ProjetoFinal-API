using ClassLibrary;
using ClassLibrary.APINetwork;
using ClassLibrary.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace ProjetoFinal_API
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkService networkservice;
        private ServiceApi apiservice;
        private Root root = new Root();
        private ObservableCollection<Root> roots = new ObservableCollection<Root>();
        private ICollectionView view;
        private Dialogservice dialogservice;
        private DataLog dataLog;
        private bool loading = false;

        public ObservableCollection<Root> Roots
        {
            get { return roots; }
            set { roots = value; }
        }
        public MainWindow()
        {           
            InitializeComponent();
            networkservice = new NetworkService();
            apiservice = new ServiceApi();
            dialogservice = new Dialogservice();
            dataLog = new DataLog();

            string about = "Versão: 1.0.0";
            lbl_about.Content = about;
            NetworkService.AvailabilityChanged += new NetworkStatusChangedHandler(DoAvailabilityChanged);
            if(!NetworkService.IsAvailable)
            {
                ShowNoInternetWarning();
            }
            LoadRoots();
            cbx_paises.DataContext = this;
        }

        private async Task<bool> LoadApiPaises()
        {
            if (!NetworkService.IsNetworkAvailable())
            {
                LoadPaises();
                return false;
            }
            else
            {
                await LoadApiRoots();
                return true;
            }
        }
        private void LoadPaises()
        {
            roots = (ObservableCollection<Root>)DataLog.GetData().Result;
            progressBar.Value = 100;
            string v = "Loading Completo";
            lbl_status.Content = v;

            Thread.Sleep(100);
        }
        private void InitializeSQLiteProvider()
        {
            SQLitePCL.Batteries.Init();
        }

        private async Task LoadApiRoots()
        {
            var progresso = new Progress<int>(complete =>
            {
                progressBar.Value = complete;
                switch(complete)
                {
                    case 25:
                        string P = "A atualizar os países";
                        lbl_informacao.Content = P;
                        break;

                    case 50: string A = "Loading, aguarde...";
                        lbl_informacao.Content = A;
                        break;

                    case 75: string I = "Só mais um momento";
                        lbl_informacao.Content = I;
                        break;

                    case 100: string S = "Países carregados com sucesso!!";
                        lbl_informacao.Content = S;
                        break;
                }
            });

            var response = await apiservice.GetRates("https://restcountries.com", "/v3.1/all", progresso); //Carrega a API

            roots = (ObservableCollection<Root>)response.Result;

            foreach (Root root in roots)
            {
                root.Flags.LocalImage = Directory.GetCurrentDirectory() + @"/Flags/Bandeira.sqlite/" + $"{root.CCA3}.png";
                return;
            }
            DataLog.DeleteData();
            DataLog.SaveData(roots);
        }

        private async void LoadRoots()
        {
            bool fezConexao = await LoadApiPaises();
            
            InicializarView();
            InicializarFlags();

            if (roots.Count == 0)
            {
                string message1 = "Não há ligação à internet" + Environment.NewLine +
                    "e não foram previamente carregadas os paises." + Environment.NewLine +
                    "Tente mais tarde!";
                lbl_informacao.Content = message1;

                return;
            }

            if (fezConexao)
            {
                string form = string.Format($"Paises carregados da internet, { DateTime.Now:g}");
                lbl_informacao.ContentStringFormat = form;
            }
            else
            {
                string form1 = string.Format($"Paises carregados da Base de Dados, {DateTime.Now:g}");
                lbl_informacao.ContentStringFormat = form1;
            }
            loading = true;
        }

        //Inicializar UI
        private void InicializarView()
        {
            view = CollectionViewSource.GetDefaultView(Roots);
            view.SortDescriptions.Add(new SortDescription("name.official", ListSortDirection.Ascending));
            cbx_paises.ItemsSource = view;
            cbx_paises.DisplayMemberPath = "name.official";
        }

        private async void InicializarFlags()
        {
            var progress = new Progress<int>(

                percentagemCompleta =>
                {
                    lbl_status.Content = $"Download de Bandeiras: {percentagemCompleta}%";
                });

            var download = await dataLog.DownloadFlags(roots, progress);

            lbl_status.Content = download.Message;
            view.Refresh();

            await Task.Delay(1500);
            lbl_status.Content = string.Empty;
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        public void DisplayCountryData(Root countryToDisplay)
        {
            DisplayImages(countryToDisplay);
            DisplayCapital(countryToDisplay);
            DisplayGini(countryToDisplay);
            DisplayCodes(countryToDisplay);
        }
        //Imagem
        public void DisplayImages(Root countryToDisplay)
        {
            imageBox.Source = new BitmapImage(new Uri(countryToDisplay.Flags.ShowFlags));         
        }

        public void DisplayCodes(Root countryToDisplay)
        {
            lbl_about.Content = countryToDisplay.CCA3;
        }

        public void DisplayCapital(Root countryToDisplay)
        {
            int iteration = 0;

            lbl_capital.Content = string.Empty;

            // REGION, SUBREGION
            lbl_regiao.Content = countryToDisplay.region;
            lbl_subRegiao.Content = countryToDisplay.subregion;

            // CAPITAL
            foreach (string capital in countryToDisplay.capital)
            {
                lbl_capital.Content += capital;

                if (!(iteration == countryToDisplay.capital.Count - 1))
                    lbl_capital.Content += Environment.NewLine;

                iteration++;
            }   
            
            
            //Continentes
            int iterations = 0;

            lbl_continente.Content = string.Empty;
           
            foreach(string continent in countryToDisplay.continents)
            {
                lbl_continente.Content += continent;

                if(!(iterations == countryToDisplay.continents.Count() - 1))               
                    lbl_continente.Content += Environment.NewLine;
                    
                iterations++;             
            }
        }

        //Display Gini e a População
        public void DisplayGini(Root countryToDisplay)
        {
            int iteration = 0;

            lbl_indiceGini.Content = string.Empty;

            // POPULAção
            lbl_populacao.Content = countryToDisplay.population.ToString("N0");

            // Indice gini
            foreach (var gini in countryToDisplay.Gini)
            {
                if (!(gini.Key == "default"))
                {
                    lbl_indiceGini.Content += $"{gini.Key}: ";
                }

                lbl_indiceGini.Content += $"{gini.Value}";

                iteration++;
            }
        }

        // NETWORK CHECKING METHOD
        void DoAvailabilityChanged(object sender, NetworkStatus e)
        {
            ReportAvailability();
        }

        private void ReportAvailability()
        {
            // INTERNET CONNECTION WENT UP
            if (NetworkService.IsAvailable)
            {
               
                // DOWNLOAD REMAINING FLAGS
                string[] flagsDownloaded = Directory.GetFiles(@"Flags");

                if (flagsDownloaded.Length != Roots.Count)
                {
                    this.Dispatcher.Invoke(() =>
                    {                   
                        InicializarFlags();
                    });
                }

                // INITIALIZE DATA
                if (!loading)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoadRoots();
                    });
                }
            }

            // INTERNET CONNECTION GONE DOWN
            if (!NetworkService.IsAvailable)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ShowNoInternetWarning();
                });
            }
        }

        public void ShowNoInternetWarning()
        {
            string no = "NO INTERNET CONNECTION";
            lbl_informacao.Content = no;
        }

        //Botão Pesquisar
        private void btn_pesquisar_Click(object sender, RoutedEventArgs e)
        {
            if (cbx_paises.SelectedItem == null)
            {
                dialogservice.ShowMessage("Tem que escolher um país.", "Erro");
                return;
            }

            var selectedCountry = (Root)cbx_paises.SelectedItem;
            root = selectedCountry;

            DisplayCountryData(selectedCountry);
        }
    }
}
