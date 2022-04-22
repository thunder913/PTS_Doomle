using PTS_Doomle.Data.Interfaces;
using PTS_Doomle.Models;

namespace PTS_Doomle.Data
{
    public class DataManager : IDataManager
    {
        private readonly IExcelDataReader excelReader;
        public static List<StudentsActivities> Activities { get; set; } = new List<StudentsActivities>();
        public static List<Models.Results> Results { get; set; } = new List<Models.Results>();
        public DataManager(IExcelDataReader excelReader)
        {
            this.excelReader = excelReader;
        }
        public List<StudentsActivities> ReadActivitiesFromFile(string fileName)
        {
            Activities = excelReader.ReaderStudentsActivitiesFromExcel(fileName).ToList();
            return Activities;
        }

        public List<Models.Results> ReadResultsFromFile(List<string> files)
        {
            foreach (var file in files)
            {
                Results.AddRange(excelReader.ReaderResultsFromExcel(file).ToList());
            }
            return Results;
        }
    }
}
