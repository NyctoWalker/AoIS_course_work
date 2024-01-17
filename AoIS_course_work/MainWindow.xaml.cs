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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AoIS_course_work
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppVM VM = new();
            DataContext = VM;
        }
    }

    public class StringGenresConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable<FilmGenre>;

            if (enumerable == null)
            {
                return "Жанры не указаны";
            }

            var list = enumerable.ToList();
            var genreNames = list
                .Where(fg => fg != null && fg.IdGenreNavigation != null)
                .Select(fg => fg.IdGenreNavigation.GenreName)
                .ToList();

            return "Жанры:" + string.Join(", ", genreNames);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
