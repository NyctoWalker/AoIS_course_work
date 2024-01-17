using AoIS_course_work.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace AoIS_course_work
{
    /// <summary>
    /// Логика взаимодействия для ParsedRedactWindow.xaml
    /// </summary>
    public partial class ParsedRedactWindow : Window
    {
        public ParsedRedactWindow(AppVM a)
        {
            InitializeComponent();
            RedactVM VM = new RedactVM(a);
            DataContext = VM;
            VM.RequestClose += () => this.Close();
        }
    }

    public class LengthValidationRule : ValidationRule
    {
        public int MaxLength { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;
            if (str == null || str=="")
                return new ValidationResult(false, "Пустая строка.");

            if (str.Length > MaxLength)
                return new ValidationResult(false, $"Ввод должен быть менее или равен {MaxLength} символам.");

            return ValidationResult.ValidResult;
        }
    }

    public class YearValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            short debug;
            var str = value as string;
            if (!short.TryParse(str, out debug))
                return new ValidationResult(false, "Не числовое значение (тип данных short).");
            else
                if (debug < 1900 || debug>2200)
                    return new ValidationResult(false, "Число должно быть от 1900 до 2200.");
            //Возможно, максимальным значением сделать текущий год, но есть так же страницы с анонсами фильмов, которые, однако, некорректно парсятся приложением

            return ValidationResult.ValidResult;
        }
    }

    public class StringJoinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as List<FilmGenre>;
            if (list == null)
            {
                return "Жанры не указаны";
            }
            var genreNames = list.Select(fg => fg.IdGenreNavigation.GenreName).ToList();
            return string.Join(", ", genreNames);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
