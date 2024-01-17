using Aspose.Cells;
using Aspose.Cells.Charts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AoIS_course_work.Models;

namespace AoIS_course_work
{
    public class ExcelManipulator
    {
        private readonly string projPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\";

        public void ExcelCreateFile(List<Film> films)
        {
            string path = projPath + "Excel - " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss.fff") + ".xlsx";
            
            var filmsByYear = films.GroupBy(x => x.ReleaseYear);
            Dictionary<string, int> filmsCountByYear = new Dictionary<string, int>();
            foreach (var group in filmsByYear)
            {
                filmsCountByYear[group.Key.ToString()] = group.Count();
            }

            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Name = "График";

            // Значения из словаря в ячейки
            Cells cells = sheet.Cells;
            int rowIndex = 0;
            foreach (var pair in filmsCountByYear)
            {
                Cell cell = cells[rowIndex, 0];
                cell.Value = pair.Key;
                cell = cells[rowIndex, 1];
                cell.Value = pair.Value;
                rowIndex++;
            }

            // Круговая диаграмма
            int chartIndex = sheet.Charts.Add(ChartType.Pie, 5, 0, 15, 5);
            Chart chart = sheet.Charts[chartIndex];
            chart.SetChartDataRange("A1:B" + (rowIndex), true);

            workbook.Save(path);
        }
    }
}
