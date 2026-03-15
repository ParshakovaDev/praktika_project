using ParshakovaLib;
using ParshakovaLib.Context;
using ParshakovaLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для PartnerAddEditWindow.xaml
    /// </summary>
    public partial class PartnerAddEditWindow : Window
    {
        // Контекст БД, используемый окном
        private readonly ApplicationContext _databaseContext;
        // Сервис для операций с партнёрами
        private readonly Service _partnerManager;
        // Текущий редактируемый партнёр (или null при добавлении)
        private Partner _currentPartner;

        public PartnerAddEditWindow(ApplicationContext context, Partner partnerToEdit = null)
        {
            InitializeComponent();
            _databaseContext = context;
            _partnerManager = new Service();
            _currentPartner = partnerToEdit;

            // Предзаполняем поля, если партнёр редактируется
            if (_currentPartner != null)
            {
                Title = "Редактирование партнера";
                TypeComboBox.Text = _currentPartner.Type;
                NameTextBox.Text = _currentPartner.Name;
                DirectorTextBox.Text = _currentPartner.Director;
                EmailTextBox.Text = _currentPartner.Email;
                PhoneTextBox.Text = _currentPartner.Phone;
                LegalAddressTextBox.Text = _currentPartner.Legaladdress;
                RatingTextBox.Text = _currentPartner.Rating.ToString();
            }
            else
            {
                Title = "Добавление партнера";
            }
        }

        // Валидация и сохранение партнёра (добавление/редактирование)
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Поле 'Название' не может быть пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(TypeComboBox.Text))
                {
                    MessageBox.Show("Поле 'Тип' не может быть пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(DirectorTextBox.Text))
                {
                    MessageBox.Show("Поле 'Директор' не может быть пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !IsValidEmail(EmailTextBox.Text))
                {
                    MessageBox.Show("Введите корректный адрес электронной почты (пример: example@mail.com).", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(LegalAddressTextBox.Text))
                {
                    MessageBox.Show("Поле 'Юридический адрес' не может быть пустым", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(PhoneTextBox.Text) || !IsValidPhone(PhoneTextBox.Text))
                {
                    MessageBox.Show("Введите номер телефона в формате +7XXXXXXXXXX (пример: +79504693322).", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(RatingTextBox.Text, out int partnerRating) || partnerRating <= 0)
                {
                    MessageBox.Show("Рейтинг должен быть положительным числом.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var partnerType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (_currentPartner == null)
                {
                    var newPartnerRecord = new Partner
                    {
                        Type = partnerType,
                        Name = NameTextBox.Text,
                        Director = DirectorTextBox.Text,
                        Email = EmailTextBox.Text,
                        Phone = PhoneTextBox.Text,
                        Legaladdress = LegalAddressTextBox.Text,
                        Rating = partnerRating
                    };

                    _partnerManager.AddPartner(_databaseContext, newPartnerRecord);
                }
                else
                {
                    _currentPartner.Type = partnerType;
                    _currentPartner.Name = NameTextBox.Text;
                    _currentPartner.Director = DirectorTextBox.Text;
                    _currentPartner.Email = EmailTextBox.Text;
                    _currentPartner.Phone = PhoneTextBox.Text;
                    _currentPartner.Legaladdress = LegalAddressTextBox.Text;
                    _currentPartner.Rating = partnerRating;

                    _partnerManager.UpdatePartner(_databaseContext, _currentPartner);
                }

                this.DialogResult = true;
            }
            catch (Exception error)
            {
                MessageBox.Show("Произошла ошибка: " + error.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Проверка валидности email по регулярному выражению
        private bool IsValidEmail(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Проверка валидности номера телефона по регулярному выражению
        private bool IsValidPhone(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\+\d{11}$");
        }
    }
}
