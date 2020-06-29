using DAN_XLI_Dejan_Prodanovic.Commands;
using DAN_XLI_Dejan_Prodanovic.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        int copiesCounter = 0;
        private string _buttonLabel;
        private int currentProgress;
        private BackgroundWorker worker = new BackgroundWorker();
        MainWindow main;

        public MainWindowViewModel(MainWindow mainOpen)
        {
            main = mainOpen;
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            CurrentProgress = 0;
            _isRunning = false;

            if (Directory.Exists("../../PrintedCopies"))
            {
                Directory.Delete("../../PrintedCopies", true);
            }
            Directory.CreateDirectory("../../PrintedCopies");

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
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                worker.ReportProgress(CurrentProgress);
                //CurrentProgress++;
                DateTime today = DateTime.Now;
                string filePath = String.Format("../../PrintedCopies/{0}.{1}_{2}_{3}_{4}_{5}.txt", (copiesCounter + 1),
                    today.Day, today.Month, today.Year, today.Hour, today.Minute);

                FileActions.WriteToFile(filePath, TextToPrint);
                copiesCounter++;
                double curProgDouble = (copiesCounter / Double.Parse(numberOfCopies)) * 100;
                CurrentProgress = (int)curProgDouble;
                //MessageBox.Show(curProgDouble.ToString());
                Thread.Sleep(1000);
               
            }

            _isRunning = false;
        }

        static void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show("Ponistili ste stampanje");
            else if (e.Error != null)
                Console.WriteLine("Printer exception " + e.Error.ToString());
            else
                MessageBox.Show("Stampanje je zavrseno"+e.Result);
              
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

                if (Int32.Parse(numberOfCopies)<=0 || Int32.Parse(numberOfCopies)>=100)
                {
                    MessageBox.Show("Morate uneti pozitivan ceo broj manji od 100.");
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
                if (worker.IsBusy) worker.CancelAsync();
               

                if (Directory.Exists("../../PrintedCopies"))
                {
                    Directory.Delete("../../PrintedCopies", true);
                }

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
