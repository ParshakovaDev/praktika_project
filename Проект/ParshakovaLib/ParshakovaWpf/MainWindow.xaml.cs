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
using ParshakovaLib;
using ParshakovaLib.Context;
using ParshakovaLib.Models;


namespace ParshakovaWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Основной контекст подключения к базе данных
        private readonly ApplicationContext _databaseContext;
        // Сервис управления партнёрами и продажами
        private Service _partnerManager;
        public MainWindow()
        {
            InitializeComponent();
            _databaseContext = new ApplicationContext();
            _partnerManager = new Service();
            PartnersList.SelectionChanged += (s, e) => RefreshSalesView();
            RefreshPartnersView();
            RefreshSalesView();
        }

        // Обновление списка партнёров и пересчёт скидок
        private void RefreshPartnersView()
        {
            var currentSelectedPartner = PartnersList.SelectedItem as Partner;
            _partnerManager.UpdateDiscounts(_databaseContext);
            var partnerCollection = _partnerManager.LoadPartners(_databaseContext);
            PartnersList.ItemsSource = partnerCollection;
            if (currentSelectedPartner != null)
            {
                PartnersList.SelectedItem = partnerCollection.FirstOrDefault(p => p.Name == currentSelectedPartner.Name);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Обновление списка продаж выбранного партнёра
        private void RefreshSalesView()
        {
            var currentSelectedPartner = PartnersList.SelectedItem as Partner;
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name);
                if (partnerData != null)
                {
                    SalesDataGrid.ItemsSource = _partnerManager.GetSales(_databaseContext, partnerData);
                }
            }
            else
            {
                SalesDataGrid.ItemsSource = null;
            }
        }

        // Открыть окно добавления нового партнёра
        private void AddPartner_Click(object sender, RoutedEventArgs e)
        {
            PartnerAddEditWindow partnerDialog = new PartnerAddEditWindow(_databaseContext);
            partnerDialog.Owner = this;
            if (partnerDialog.ShowDialog() == true)
            {
                RefreshPartnersView();
            }
        }

        // Открыть окно редактирования выбранного партнёра
        private void EditPartner_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as Partner;
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name);
                PartnerAddEditWindow partnerDialog = new PartnerAddEditWindow(_databaseContext, partnerData);
                partnerDialog.Owner = this;
                if (partnerDialog.ShowDialog() == true)
                {
                    RefreshPartnersView();
                }
            }
        }

        // Удалить выбранного партнёра после подтверждения
        private void DeletePartner_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as Partner;
            Window parentWindow = Window.GetWindow(this);
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name);
                var confirmationResult = MessageBox.Show(parentWindow, "Вы действительно хотите удалить выбранного партнера?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmationResult == MessageBoxResult.Yes)
                {
                    _partnerManager.DeletePartner(_databaseContext, partnerData);
                }
            }
            RefreshPartnersView();
        }

        // Открыть окно добавления продажи для выбранного партнёра
        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as Partner;
            if (currentSelectedPartner != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name);
                if (partnerData != null)
                {
                    SaleAddEditWindow saleDialog = new SaleAddEditWindow(_databaseContext, null, partnerData);
                    saleDialog.Owner = this;
                    if (saleDialog.ShowDialog() == true)
                    {
                        RefreshSalesView();
                        RefreshPartnersView();
                    }
                }
            }
        }

        // Открыть окно редактирования выбранной продажи
        private void EditSale_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPartner = PartnersList.SelectedItem as Partner;
            var currentSelectedSale = SalesDataGrid.SelectedItem as Sale;
            if (currentSelectedPartner != null && currentSelectedSale != null)
            {
                var partnerData = _partnerManager.GetPartnerByProperties(_databaseContext, currentSelectedPartner.Name);
                if (partnerData != null)
                {
                    SaleAddEditWindow saleDialog = new SaleAddEditWindow(_databaseContext, currentSelectedSale, partnerData);
                    saleDialog.Owner = this;
                    if (saleDialog.ShowDialog() == true)
                    {
                        RefreshSalesView();
                        RefreshPartnersView();
                    }
                }
            }
        }

        // Удалить выбранную продажу после подтверждения
        private void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedSale = SalesDataGrid.SelectedItem as Sale;
            if (currentSelectedSale != null)
            {
                var confirmationResult = MessageBox.Show("Вы действительно хотите удалить выбранную продажу?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmationResult == MessageBoxResult.Yes)
                {
                    _partnerManager.DeleteSale(_databaseContext, currentSelectedSale);
                    RefreshSalesView();
                    RefreshPartnersView();
                }
            }
        }
    }
}
