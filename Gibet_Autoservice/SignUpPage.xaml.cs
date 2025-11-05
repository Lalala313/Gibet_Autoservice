using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gibet_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;
            DataContext = _currentService;

            var _currentClient = ГибетАвтосервисEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentClient;
        }

        public ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if(ComboClient.SelectedItem == null)
            {
                errors.AppendLine("Укажите ФИО клиента");
            }

            if (StartDate.Text== "")
            {
                errors.AppendLine("Укажите дату услуги");
            }

            if(TBStart.Text == "")
            {
                errors.AppendLine("Укажите время начала услуги");
            }
            else if (!IsValidTime(TBStart.Text))
            {
                errors.AppendLine("Укажите действительное время в формате ЧЧ:ММ");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            //_currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            var selectedClient = (dynamic)ComboClient.SelectedItem;
            _currentClientService.ClientID = selectedClient.ID;

            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                ГибетАвтосервисEntities.GetContext().ClientService.Add(_currentClientService);


            try
            {
                ГибетАвтосервисEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private bool IsValidTime(string time)
        {
            if (string.IsNullOrWhiteSpace(time) || !time.Contains(':'))
                return false;

            string[] parts = time.Split(':');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out int hours) || !int.TryParse(parts[1], out int minutes))
                return false;

            // Часы от 0 до 23, минуты от 0 до 59
            return hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59;
        }
        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;
            if (s.Length < 3 || !s.Contains(':')) 
                TBEnd.Text = "";
            else
            {
                try
                {
                    string[] start = s.Split(new char[] { ':' });
                    int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                    int startMin = Convert.ToInt32(start[1].ToString());

                    int sum = startHour + startMin + _currentService.Duration;

                    int EndHour = sum / 60;
                    int EndMin = sum % 60;

                    if (EndHour >=24) EndHour %= 24;
                    
                    s = $"{EndHour}:{EndMin:D2}";
                    TBEnd.Text = s;
                }
                catch
                {
                    TBEnd.Text = "";

                }

            }
        }
    }
}
