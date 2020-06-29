using DAN_XLI_Dejan_Prodanovic.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DAN_XLI_Dejan_Prodanovic.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        MainWindow main;

        #region Constructors
        public MainWindowViewModel(MainWindow mainOpen)
        {
            main = mainOpen;
        }
        #endregion

        #region Properties
        private string textToPrint;
        public string TextToPrint
        {
            get
            {
                return textToPrint;
            }
            set
            {
                textToPrint = value;
                OnPropertyChanged("TextToPrint");
            }
        }

        private string numberOfCopies;
        public string NumberOfCopies
        {
            get
            {
                return numberOfCopies;
            }
            set
            {
                numberOfCopies = value;
                OnPropertyChanged("NumberOfCopies");
            }
        }
        #endregion



        #region Commands
        private ICommand print;
        public ICommand Print
        {
            get
            {
                if (print == null)
                {
                    print = new RelayCommand(param => PrintExecute(), param => CanPrintExecute());
                }
                return print;
            }
        }

        private void PrintExecute()
        {
            try
            {
                //if (User != null)
                //{
                //    EditUser editUser = new EditUser(User);
                //    editUser.ShowDialog();

                //    //we read users from database in case we added new idcard
                //    UserList = service.GetAllUsers().ToList();
                //    FormatDatesForPresentation();
                //    UserList.Sort((x, y) => DateTime.Compare((DateTime)x.ExpiryDate, (DateTime)y.ExpiryDate));

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private bool CanPrintExecute()
        {
            if (String.IsNullOrWhiteSpace(TextToPrint)||String.IsNullOrWhiteSpace(NumberOfCopies))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
