using PTS_Doomle.Models;

namespace PTS_Doomle.Data.Interfaces
{
    public interface IDataManager
    {
        public List<StudentsActivities> ReadActivitiesFromFile(string fileName);
        public List<Models.Results> ReadResultsFromFile(List<string> files);
    }
}
