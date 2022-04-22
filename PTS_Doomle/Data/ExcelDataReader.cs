using PTS_Doomle.Data.Interfaces;
using ExcelDataReader;
using System.Data;
using PTS_Doomle.Models;

namespace PTS_Doomle.Data
{
    public class ExcelDataReader : Interfaces.IExcelDataReader
    {
        public List<StudentsActivities> ReaderStudentsActivitiesFromExcel(string filePath)
        {
            var toReturn = new List<StudentsActivities>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);            
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables.Cast<DataTable>().FirstOrDefault();
                    if (table != null)
                    {
                        for (int i = 1; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            DateTime date;
                            var sucess = DateTime.TryParseExact(row?.ItemArray[0]?.ToString(), "dd/MM/yy, HH:mm", null, System.Globalization.DateTimeStyles.None, out date);
                            if (!sucess)
                            {
                                sucess = DateTime.TryParseExact(row?.ItemArray[0]?.ToString(), "d/MM/yy, HH:mm", null, System.Globalization.DateTimeStyles.None, out date);
                                if (!sucess)
                                {
                                    continue;
                                }
                            }
                            var context = row?.ItemArray[1]?.ToString();
                            var component = row?.ItemArray[2]?.ToString();
                            var name = row?.ItemArray[3]?.ToString();
                            var description = row?.ItemArray[4]?.ToString();

                            toReturn.Add(new StudentsActivities()
                            {
                                Time = date,
                                Component = component,
                                Context = context,
                                Description = description,
                                Name = name,
                            });
                        }
                    }

                }
            }
            return toReturn;
        }

        public List<Models.Results> ReaderResultsFromExcel(string filePath)
        {
            var toReturn = new List<Models.Results>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var table = result.Tables.Cast<DataTable>().FirstOrDefault();
                    if (table != null)
                    {
                        for (int i = 1; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];

                            int.TryParse(row?.ItemArray[0]?.ToString(), out int id);
                            double.TryParse(row?.ItemArray[1]?.ToString(), out double grade);

                            toReturn.Add(new Models.Results()
                            {
                                Result = grade,
                                StudentId = id,
                            });
                        }
                    }

                }
            }
            return toReturn;
        }
    }
}
