using PolMedUMG.Model;
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

namespace PolMedUMG.View
{
    /// <summary>
    /// Interaction logic for DoctorScreen.xaml
    /// </summary>
    public partial class DoctorScreen : Window
    {
        public DoctorScreen( )
        {
            InitializeComponent();
            dUser.Text = Properties.Settings.Default.User;


        }
        public void dLoadContent(UserControl control)
        {

            if (RightContentPanel != null)
            {
                RightContentPanel.Children.Clear();
                RightContentPanel.Children.Add(control);
            }
        }
        private void dMyListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (NavList.SelectedItem is ListBoxItem selectedItem)
            {
                string selectedText = selectedItem.Content.ToString();

                switch (selectedText)
                {
                    case "Strona główna":
                        dLoadContent(new dMainView());
                        break;
                    case "Umów wizytę":
                        dLoadContent(new MakeAppointment());
                        break;
                    case "Kalendarz":
                        dLoadContent(new Calendar());
                        break;
                    case "Cennik":
                        dLoadContent(new Pricing());
                        break;

                    case "Wiadomości":
                        dLoadContent(new Messages());
                        break;
                    case "Ustawienia konta":
                        dLoadContent(new Settings());
                        break;
                    default:
                        break;
                }

            }
            }

            }

        }
