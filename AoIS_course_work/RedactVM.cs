using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AoIS_course_work.Models;

namespace AoIS_course_work
{
    public class RedactVM : INotifyPropertyChanged
    {
        private Film _film;
        public Film film
        {
            get { return _film; }
            set
            {
                if (_film != value)
                {
                    _film = value;
                    OnPropertyChanged(nameof(film));
                }
            }
        }

        private AppVM singleton;
        /*private string originalName;
        private string translatedName;
        private string director;
        private string duration;
        private short releaseYear;
        private string rating;
        private DateTime addedDate;*/

        private Command submitChangesCommand;
        private Command exitCommand;

        public event Action RequestClose;

        protected virtual void OnRequestClose()
        {
            RequestClose?.Invoke();
        }

        public RedactVM(AppVM a)
        {
            singleton = a;
            film = singleton.SelectedParsedRecord;
        }

        #region Commands
        public Command SubmitChangesCommand
        {
            get => submitChangesCommand ??= new Command(obj =>
            {
                singleton.ParsedRecords.Remove(singleton.SelectedParsedRecord);
                singleton.SelectedParsedRecord = film;
                singleton.ParsedRecords.Add(singleton.SelectedParsedRecord);

                CloseWindow();
            });
        }

        public Command ExitCommand
        {
            get => exitCommand ??= new Command(obj =>
            {
                CloseWindow();
            });
        }
        #endregion

        #region Validators
        

        #endregion

        public void CloseWindow()
        {
            OnRequestClose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
