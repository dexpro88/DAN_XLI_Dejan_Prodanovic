using DAN_XLI_Dejan_Prodanovic.Commands;
using DAN_XLI_Dejan_Prodanovic.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DAN_XLI_Dejan_Prodanovic.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {      
        private static bool _isRunning;
        //private static bool _isRunningHelpVar;
        int copiesCounter = 0;
        private string _buttonLabel;
        private int currentProgress;
        //private ICommand _command;
        private BackgroundWorker worker = new BackgroundWorker();
        MainWindow main;

        public MainWindowViewModel(MainWindow mainOpen)
        {
            main = mainOpen;
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            CurrentProgress = 0;
            _isRunning = false;
          
        }

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

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (CurrentProgress >= 100)
            {
                CurrentProgress = 0;
            }

            while (CurrentProgress < 100 && _isRunning)
            {
                worker.ReportProgress(CurrentProgress);
                Thread.Sleep(1000);
                //CurrentProgress++;
                copiesCounter++;
                double curProgDouble = (copiesCounter / Double.Parse(numberOfCopies)) * 100;
                CurrentProgress = (int)curProgDouble;
                //MessageBox.Show(curProgDouble.ToString());
            }

            _isRunning = false;
        }
  
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
                if (!ValidationClass.NumberOfCoppiesIsValid(numberOfCopies))
                {
                    MessageBox.Show("Morate uneti ceo broj za broj kopija.");
                    return;
                }

                if (_isRunning)
                {
                    MessageBox.Show("Stampanje je vec u toku.");
                    return;
                }
                _isRunning = true;
                //_isRunningHelpVar = true;

                if (_isRunning)
                {
                    DoStuff();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool CanPrintExecute()
        {
            if (String.IsNullOrWhiteSpace(TextToPrint) || String.IsNullOrWhiteSpace(NumberOfCopies))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private ICommand stopPrinting;
        public ICommand StopPrinting
        {
            get
            {
                if (stopPrinting == null)
                {
                    stopPrinting = new RelayCommand(param => StopPrintingExecute(), param => CanStopPrintingxecute());
                }
                return stopPrinting;
            }
        }

        private void StopPrintingExecute()
        {
            try
            {       
                _isRunning = !_isRunning;          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool CanStopPrintingxecute()
        {
            if (!_isRunning)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //public ICommand Command
        //{
        //    get
        //    {
        //        return _command ?? (_command = new RelayCommand(x =>
        //        {
        //            _isRunning = !_isRunning;

        //            if (!_isRunning)
        //            {
        //                DoStuff();
        //            }
        //            else
        //            {
        //                ButtonLabel = "PAUSED";
        //            }
        //        }));
        //    }
        //}

        public int CurrentProgress
        {
            get { return currentProgress; }
            private set
            {
                if (currentProgress != value)
                {
                    currentProgress = value;
                    OnPropertyChanged("CurrentProgress");
                }
            }
        }

        public string ButtonLabel
        {
            get { return _buttonLabel; }
            private set
            {
                if (_buttonLabel != value)
                {
                    _buttonLabel = value;
                    OnPropertyChanged("ButtonLabel");
                }
            }
        }

        private void DoStuff()
        {
           
            worker.RunWorkerAsync();
        }
    }
}
