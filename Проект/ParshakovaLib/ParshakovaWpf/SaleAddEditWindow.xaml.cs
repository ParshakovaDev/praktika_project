using ParshakovaLib;
using ParshakovaLib.Context;
using ParshakovaLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ParshakovaWpf
{
    /// <summary>
    /// Логика взаимодействия для SaleAddEditWindow.xaml
    /// </summary>
    public partial class SaleAddEditWindow : Window
    {
        // Контекст БД для операций с продажами
        private readonly ApplicationContext _databaseContext;
        // Текущая редактируемая/создаваемая продажа
        private Sale _currentSale;
        // Флаг режима редактирования
        private bool _isEditing;
        // Сервис для сохранения изменений в БД
        private readonly Service _partnerManager;
        // Партнёр, к которому относится продажа
        private Partner _currentPartner;

        public SaleAddEditWindow(ApplicationContext context, Sale sale = null, Partner partner = null)
        {
            InitializeComponent();
            _databaseContext = context;
            _partnerManager = new Service();
            _currentPartner = partner ?? throw new ArgumentNullException(nameof(partner), "Партнер не выбран.");

            // Если передана продажа — заполняем форму для редактирования
            if (sale != null)
            {
                _isEditing = true;
                _currentSale = sale;
                Title = "Редактирование продажи";
                LoadSaleDetails();
            }
            else
            {
                _isEditing = false;
                Title = "Добавление продажи";
                _currentSale = new Sale();
            }
        }

        // Заполнение полей формы данными продажи
        private void LoadSaleDetails()
        {
            ProductNameTextBox.Text = _currentSale.ProductName;
            QuantityTextBox.Text = _currentSale.Quantity.ToString();
            SaleDatePicker.SelectedDate = _currentSale.Date;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Базовая валидация введённых данных
                if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                {
                    MessageBox.Show("Введите корректное наименование продукта.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(QuantityTextBox.Text, out int saleQuantity) || saleQuantity <= 0)
                {
                    MessageBox.Show("Количество должно быть положительным числом.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SaleDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату продажи.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (SaleDatePicker.SelectedDate > DateTime.Today)
                {
                    MessageBox.Show("Дата продажи не может быть в будущем.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохранение изменений продажи
                _currentSale.ProductName = ProductNameTextBox.Text;
                _currentSale.Quantity = saleQuantity;
                _currentSale.Date = SaleDatePicker.SelectedDate.Value;
                _currentSale.Partnerid = _currentPartner.Id;

                if (_isEditing)
                {
                    _partnerManager.UpdateSale(_databaseContext, _currentSale);
                }
                else
                {
                    _partnerManager.AddSale(_databaseContext, _currentSale);
                }

                _partnerManager.SaveChanges(_databaseContext);
                DialogResult = true;
                Close();
            }
            catch (Exception error)
            {
                MessageBox.Show($"Произошла ошибка: {error.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
