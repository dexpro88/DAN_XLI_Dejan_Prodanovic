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
        private string _buttonLabel;
        private int currentProgress;
        private BackgroundWorker worker = new BackgroundWorker();
        private bool firstPrinting = true;

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


            if (Directory.Exists("../../PrintedCopies"))
            {
                Directory.Delete("../../PrintedCopies", true);
            }
            Directory.CreateDirectory("../../PrintedCopies");

        }

        #region Properties
        /// <summary>
        /// text that we will print , it is binded to a textbox 
        /// </summary>
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

        /// <summary>
        /// number of coppies that we want to print to a file
        /// it si binded to a textbox
        /// </summary>
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
        /// <summary>
        /// percentage of files in which text has already been written
        /// it is binded to a label lblPercentage
        /// </summary>
        private string percentage;
        public string Percentage
        {
            get
            {
                return percentage;
            }
            set
            {
                percentage = value;
                OnPropertyChanged("Percentage");
            }
        }

        /// <summary>
        /// current progress of writing to files
        /// </summary>
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

        #endregion  

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            int copiesCounter = 0;

            CurrentProgress = 0;

            int wantedNumberOfCoppies = Int32.Parse(numberOfCopies);

            while (copiesCounter < wantedNumberOfCoppies)
            {

                worker.ReportProgress(CurrentProgress);

                //we format a path and a name for file
                DateTime today = DateTime.Now;
                string filePath = String.Format("../../PrintedCopies/{0}.{1}_{2}_{3}_{4}_{5}.txt", (copiesCounter + 1),
                    today.Day, today.Month, today.Year, today.Hour, today.Minute);

                FileActions.WriteToFile(filePath, TextToPrint);

                Thread.Sleep(1000);

                copiesCounter++;

                //we count current percentage of files in which text is alredy written
                double curProgDouble = (copiesCounter / (double)wantedNumberOfCoppies) * 100;
                CurrentProgress = (int)curProgDouble;

                //we format perctentage for label where it will be displayed
                Percentage = ((int)curProgDouble).ToString() + "%";


                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

            }


        }

        /// <summary>
        /// method that is executed when BackgroundWorker is completed or canceled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show("Ponistili ste stampanje");
            else if (e.Error != null)
                Console.WriteLine("Printer exception " + e.Error.ToString());
            else
                MessageBox.Show("Stampanje je zavrseno" + e.Result);

        }

        /// <summary>
        /// command that is called when we click on buttno Stampaj
        /// </summary>
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
                //we check if user enter valid value for number of coppies
                if (!ValidationClass.NumberOfCoppiesIsValid(numberOfCopies))
                {
                    MessageBox.Show("Morate uneti pozitivan ceo broj manji od 100.");
                    return;
                }
                //we check if user enter valid value for number of coppies
                if (Int32.Parse(numberOfCopies) <= 0 || Int32.Parse(numberOfCopies) >= 100)
                {
                    MessageBox.Show("Morate uneti pozitivan ceo broj manji od 100.");
                    return;
                }
                //if the Background is running we cant call Print commad 
                //program will print a message to user and command will end here
                if (worker.IsBusy)
                {
                    MessageBox.Show("Stampanje je vec u toku.");
                    return;
                }

                firstPrinting = false;

                if (!worker.IsBusy)
                {
                    //this method call DoWork method of Background Worker
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
            //button Stampaj is available if there is text in a textboxs TextToPrint and NumberOfCopies
            if (String.IsNullOrWhiteSpace(TextToPrint) || String.IsNullOrWhiteSpace(NumberOfCopies))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// command that is executed when user press button Prekid stampanja
        /// </summary>
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

                if (worker.IsBusy) worker.CancelAsync();

                //we delete folder in which we store files in which we print the text
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
            //user can press button Prekid stampanja only if BackgroundWorker is running 
            //(while printing is being performed)

            if (!worker.IsBusy)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private void DoStuff()
        {
            //if it is not the first printing  after program is started we have to create folder for storing
            //files in which we will print text from the text box
            if (!firstPrinting)
            {
                Directory.CreateDirectory("../../PrintedCopies");
            }
            worker.RunWorkerAsync();

        }
    }
}
