using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using AoIS_course_work.Models;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace AoIS_course_work
{
    public class WordManipulator
    {
        private readonly string projPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\";

        public void WordCreateFile(List<Film> films)
        {
            string path = projPath + "Word - " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss.fff") + ".docx";

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();

                Body body = new Body();

                Paragraph para = new Paragraph(
                   new Run(
                       new Text($"Всего фильмов в списке: {films.Count()}")
                   )
                );
                body.Append(para);

                short minYear = films.Min(f => f.ReleaseYear);
                short maxYear = films.Max(f => f.ReleaseYear);
                para = new Paragraph(
                   new Run(
                       new Text($"Самый ранний год выпуска: {minYear}, самый поздний: {maxYear}")
                   )
                );
                body.Append(para);

                para = new Paragraph(
                   new Run(
                       new Text($"Этот отчёт был сгенерирован автоматически на основе фильмов, полученных при парсинге с сайта IMDB {DateTime.Now.ToString("dd.MM.yyyy")} в {DateTime.Now.ToString("HH:mm")}.")
                   )
                );
                body.Append(para);

                mainPart.Document = new Document(body);

                Table table = new Table();

                TableProperties tblProperties = new TableProperties();
                TableBorders tblBorders = new TableBorders();

                TopBorder topBorder = new TopBorder();
                topBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
                topBorder.Color = "000000";
                tblBorders.AppendChild(topBorder);

                BottomBorder bottomBorder = new BottomBorder();
                bottomBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
                bottomBorder.Color = "000000";
                tblBorders.AppendChild(bottomBorder);

                RightBorder rightBorder = new RightBorder();
                rightBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
                rightBorder.Color = "000000";
                tblBorders.AppendChild(rightBorder);

                LeftBorder leftBorder = new LeftBorder();
                leftBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
                leftBorder.Color = "000000";
                tblBorders.AppendChild(leftBorder);

                InsideHorizontalBorder insideHBorder = new InsideHorizontalBorder();
                insideHBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
                insideHBorder.Color = "000000";
                tblBorders.AppendChild(insideHBorder);

                InsideVerticalBorder insideVBorder = new InsideVerticalBorder();
                insideVBorder.Val = new EnumValue<BorderValues>(BorderValues.Thick);
                insideVBorder.Color = "000000";
                tblBorders.AppendChild(insideVBorder);

                tblProperties.AppendChild(tblBorders);
                table.AppendChild(tblProperties);

                // Заголовочная строка
                TableRow headerRow = new TableRow();
                headerRow.Append(
                   new TableCell(new Paragraph(new Run(new Text("Название")))),
                   new TableCell(new Paragraph(new Run(new Text("Оригинальное имя")))),
                   new TableCell(new Paragraph(new Run(new Text("Длительность")))),
                   new TableCell(new Paragraph(new Run(new Text("Возрастной рейтинг")))),
                   new TableCell(new Paragraph(new Run(new Text("Год выпуска")))),
                   new TableCell(new Paragraph(new Run(new Text("Режиссёр"))))
                );
                table.Append(headerRow);

                // Записи с данными
                foreach (var film in films)
                {
                    TableRow row = new TableRow();
                    row.Append(
                        new TableCell(new Paragraph(new Run(new Text(film.TranslatedName)))),
                        new TableCell(new Paragraph(new Run(new Text(film.OriginalName)))),
                        new TableCell(new Paragraph(new Run(new Text(film.Duration)))),
                        new TableCell(new Paragraph(new Run(new Text(film.Rating)))),
                        new TableCell(new Paragraph(new Run(new Text(film.ReleaseYear.ToString())))),
                        new TableCell(new Paragraph(new Run(new Text(film.Director))))
                    );
                    table.Append(row);
                }

                //body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                body.Append(table);

                var plt = new ScottPlot.Plot(600, 400);
                var groupedFilms = films.GroupBy(f => f.ReleaseYear);
                var labels = groupedFilms.Select(g => g.Key.ToString()).ToArray();
                var values = groupedFilms.Select(g => g.Count()).Select(i => (double)i).ToArray();

                string imgPath = projPath + "piechart.png";

                plt.PlotPie(values, labels);
                plt.SaveFig(imgPath);

                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

                using (FileStream stream = new FileStream(imgPath, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                body.Append(new Paragraph());
                AddImageToBody(wordDoc, mainPart.GetIdOfPart(imagePart));
                body.Append(new Paragraph());

                File.Delete(imgPath);

                mainPart.Document.Save();
            }
        }

        //https://learn.microsoft.com/en-us/office/open-xml/word/how-to-insert-a-picture-into-a-word-processing-document?tabs=cs-0%2Ccs-1%2Ccs-2%2Ccs-3%2Ccs
        static void AddImageToBody(WordprocessingDocument wordDoc, string relationshipId)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            if (wordDoc.MainDocumentPart is null || wordDoc.MainDocumentPart.Document.Body is null)
            {
                throw new ArgumentNullException("MainDocumentPart and/or Body is null.");
            }

            DW.Extent extent = element.Inline.Extent;
            extent.Cx = 6480000;
            extent.Cy = 4320000;

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(element)));
        }
    }
}
