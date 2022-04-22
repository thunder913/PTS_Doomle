using PTS_Doomle.Models;

namespace PTS_Doomle.Data.Interfaces
{
    public interface IExcelDataReader
    {
        public List<StudentsActivities> ReaderStudentsActivitiesFromExcel(string filePath);
        public List<Models.Results> ReaderResultsFromExcel(string filePath);
    }
}
