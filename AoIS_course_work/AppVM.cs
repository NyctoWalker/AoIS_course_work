using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AoIS_course_work.Models;

namespace AoIS_course_work
{
    public class AppVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Редактирование данных из БД(пересмотено, так как функция добавления их может переписывать, но решено что лучше парсить свежие данные и редактировать их при необходимости)
        /// Выбор вариантов отчёта
        /// </summary>



        #region Variables
        //Списки
        //Ссылки на страницы с фильмами
        public UniqueObservableCollection<string> FilmLinks { get; set; }

        private string selectedLink;
        public string SelectedLink
        {
            get { return selectedLink; }
            set
            {
                if (selectedLink != value)
                {
                    selectedLink = value;
                    OnPropertyChanged(nameof(SelectedLink));
                }
            }
        }

        //Записи для просмотра/удаления в БД
        public UniqueObservableCollection<Film> Records { get; set; }

        private Film selectedRecord;
        public Film SelectedRecord
        {
            get { return selectedRecord; }
            set
            {
                if (selectedRecord != value)
                {
                    selectedRecord = value;

                    if (value.FilmGenres.Count() == 0)
                        SelectedRecordGenres = "Жанры не указаны";
                    else
                    {
                        var genreNames = value.FilmGenres.Select(fg => fg.IdGenreNavigation.GenreName).ToList();
                        SelectedRecordGenres = string.Join(", ", genreNames);
                    }

                    OnPropertyChanged(nameof(SelectedRecord));
                }
            }
        }

        private string selectedRecordGenres;
        public string SelectedRecordGenres
        {
            get { return selectedRecordGenres; }
            set
            {
                if (selectedRecordGenres != value)
                {
                    selectedRecordGenres = value;
                    OnPropertyChanged(nameof(SelectedRecordGenres));
                }
            }
        }


        //Записи для редактирования/удаления/добавления в БД после парсинга
        public UniqueObservableCollection<Film> ParsedRecords { get; set; }

        private Film selectedParsedRecord;
        public Film SelectedParsedRecord
        {
            get { return selectedParsedRecord; }
            set
            {
                if (selectedParsedRecord != value)
                {
                    selectedParsedRecord = value;
                    OnPropertyChanged(nameof(SelectedParsedRecord));
                }
            }
        }

        //TextBox string
        private string linkText;
        public string LinkText
        {
            get { return linkText; }
            set
            {
                if (linkText != value)
                {
                    linkText = value;
                    OnPropertyChanged(nameof(LinkText));
                }
            }
        }

        //CheckBox bools
        private bool clearLinksAfterParsing;
        public bool ClearLinksAfterParsing
        {
            get { return clearLinksAfterParsing; }
            set
            {
                if (clearLinksAfterParsing != value)
                {
                    clearLinksAfterParsing = value;
                    OnPropertyChanged(nameof(ClearLinksAfterParsing));
                }
            }
        }

        private bool rewriteDBRecords;
        public bool RewriteDBRecords
        {
            get { return rewriteDBRecords; }
            set
            {
                if (rewriteDBRecords != value)
                {
                    rewriteDBRecords = value;
                    OnPropertyChanged(nameof(RewriteDBRecords));
                }
            }
        }

        private bool rewriteParsedRecords;
        public bool RewriteParsedRecords
        {
            get { return rewriteParsedRecords; }
            set
            {
                if (rewriteParsedRecords != value)
                {
                    rewriteParsedRecords = value;
                    OnPropertyChanged(nameof(RewriteParsedRecords));
                }
            }
        }

        //Команды
        private Command deleteSelectedLinkCommand;
        private Command clearLinksCommand;
        private Command addLinkCommand;

        private Command deleteSelectedParsedRecordCommand;
        private Command clearParsedRecordsCommand;
        private Command redactParsedRecordCommand;
        private Command addSelectedParsedRecordCommand;
        private Command addParsedRecordsCommand;

        private Command parseLinksCommand;
        private Command createExcelFileCommand;
        private Command createWordFileCommand;

        private Command deleteSelectedRecordCommand;
        private Command loadDBDataCommand;

        //Другие переменные
        private const string imdbLink = "https://www.imdb.com/title/tt";
        private Parser parser;
        private DBManipulator db;
        private ExcelManipulator em;
        private WordManipulator wm;
        public AppVM singleton;
        #endregion



        public AppVM()
        {
            singleton = this;

            parser = new();
            db = new();
            em = new();
            wm = new();

            LinkText = "Введите ссылку на фильм...";

            ClearLinksAfterParsing = true;
            RewriteParsedRecords = true;
            RewriteDBRecords = false;

            FilmLinks = new UniqueObservableCollection<string>
            {
                "0111161",
                "0111161",
                "0068646",
                "0468569",
            };
            ParsedRecords = new UniqueObservableCollection<Film>
            {
                new Film { OriginalName="Test", TranslatedName="тест", Rating="12+", ReleaseYear=2024, Duration="2h", Director="Christopher Nolan" },
                new Film { OriginalName="Test1", TranslatedName="тест1", Rating="R", ReleaseYear=2023, Duration="2h", Director="Christopher Nolan" },
                new Film { OriginalName="Test2", TranslatedName="тест2", Rating="R", ReleaseYear=2023, Duration="2h", Director="Christopher Nolan" },
                new Film { OriginalName="Test3", TranslatedName="тест3", Rating="R", ReleaseYear=2021, Duration="2h", Director="Christopher Nolan" },
            };
            Records = new();
        }



        #region Commands

        public Command DeleteSelectedLinkCommand => deleteSelectedLinkCommand ??= new Command(obj =>
            {FilmLinks.Remove(SelectedLink);});

        public Command ClearLinksCommand => clearLinksCommand ??= new Command(obj =>
            {FilmLinks.Clear(); });

        public Command AddLinkCommand
        {
            get => addLinkCommand ??= new Command(obj =>
            {
                if (LinkText != "" && LinkText is not null)
                {
                    if (LinkText.Contains(imdbLink))
                    {
                        FilmLinks.Add(LinkText.Replace(imdbLink, ""));
                        LinkText = "";
                    }
                    else
                    { LinkText = "Введите ссылку на фильм с сайта IMDB!"; }
                }
            });
        }

        public Command DeleteSelectedParsedRecordCommand => deleteSelectedParsedRecordCommand ??= new Command(obj =>
            {ParsedRecords.Remove(SelectedParsedRecord);});

        public Command ClearParsedRecordsCommand => clearParsedRecordsCommand ??= new Command(obj =>
            {ParsedRecords.Clear();});

        public Command RedactParsedRecordCommand
        {
            get => redactParsedRecordCommand ??= new Command(obj =>
            {
                if (SelectedParsedRecord is not null)
                {
                    var redactWindow = new ParsedRedactWindow(singleton);
                    redactWindow.ShowDialog();
                }
            });
        }

        public Command AddSelectedParsedRecordCommand
        {
            get => addSelectedParsedRecordCommand ??= new Command(obj =>
            {
                if (SelectedParsedRecord is not null)
                {
                    db.AddDBRecord(SelectedParsedRecord, rewriteDBRecords);
                    ParsedRecords.Remove(SelectedParsedRecord);
                }
            });
        }

        public Command AddParsedRecordsCommand
        {
            get => addParsedRecordsCommand ??= new Command(obj =>
            {
                foreach (Film f in ParsedRecords)
                {
                    db.AddDBRecord(f, rewriteDBRecords);
                }
                ParsedRecords.Clear();
            });
        }

        public Command DeleteSelectedRecordCommand
        {
            get => deleteSelectedRecordCommand ??= new Command(obj =>
            {
                if (SelectedRecord is not null)
                {
                    db.DeleteDBRecord(SelectedRecord);
                    Records.Remove(SelectedRecord);
                    //Film f = SelectedRecord;
                }
            });
        }

        //Возможно с ограничением на top(n) записей
        public Command LoadDBDataCommand
        {
            get => loadDBDataCommand ??= new Command(obj =>
            {
                if (Records.Count() != 0)
                    Records.Clear();

                foreach (Film f in db.GetDBValues())
                {
                    Records.Add(f);
                }
            });
        }

        public Command ParseLinksCommand
        {
            get => parseLinksCommand ??= new Command(obj =>
            {
                if (FilmLinks.Count != 0)
                {
                    foreach (Film f in parser.Parse(FilmLinks.ToList()))
                    {
                        var sameFilm = ParsedRecords.FirstOrDefault(x => x.OriginalName == f.OriginalName);

                        if (sameFilm != null)
                        {
                            if (rewriteParsedRecords)
                            {
                                ParsedRecords.Remove(sameFilm);
                                ParsedRecords.Add(f);
                            }
                        }
                        else
                            ParsedRecords.Add(f);
                    }

                    if (ClearLinksAfterParsing)
                        FilmLinks.Clear();
                }
            });
        }

        public Command CreateExcelFileCommand
        {
            get => createExcelFileCommand ??= new Command(obj =>
            {
                if (ParsedRecords.Count() != 0)
                {
                    em.ExcelCreateFile(ParsedRecords.ToList());
                    MessageBox.Show("Отчёт создан!");
                }
            });
        }

        public Command CreateWordFileCommand
        {
            get => createWordFileCommand ??= new Command(obj =>
            {
                if (ParsedRecords.Count() != 0)
                {
                    wm.WordCreateFile(ParsedRecords.ToList());
                    MessageBox.Show("Отчёт создан!");
                }
            });
        }

        #endregion



        public class UniqueObservableCollection<T> : ObservableCollection<T>
        {
            public new void Add(T item)
            {
                if (!this.Contains(item))
                {
                    base.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
