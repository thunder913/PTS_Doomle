using PTS_Doomle.Data.Interfaces;
using PTS_Doomle.Data.Responses;
using PTS_Doomle.Models;
using System.Text.RegularExpressions;

namespace PTS_Doomle.Data
{
    public class CalculationManager : ICalculationManager
    {
        private const string LogsFileName = "Data/Files/Logs_Course A_StudentsActivities.xlsx";
        private const string GradeFileName1 = "Data/Files/Course A_StudentsResults_Year 1.xlsx";
        private const string GradeFileName2 = "Data/Files/Course A_StudentsResults_Year 2.xlsx";
        private const string FileSubmissionsComponent = "File submissions";
        private const string CourseWorkEventContext = "Assignment: Качване на курсови задачи и проекти";
        private const string IdRegex = @"(?<=\')(.*?)(?=\')";
        private const string WikiComponent = "Wiki";
        private const string UpdateWikiEvent = "Wiki page updated";
        private readonly IDataManager dataManager;
        public CalculationManager(IDataManager dataManager)
        {
            this.dataManager = dataManager;
        }
        public List<T> CalculateValue<T>()
        {
            if (typeof(T) == typeof(FrequencyResponse))
            {
                // Get the data from excel
                var users = this.GetDataFromStudentsActivities();

                // Do the calculations
                var maxValue = users.Values.Max();
                var toReturn = new List<FrequencyResponse>();
                for (int i = 1; i <= maxValue; i++)
                {
                    toReturn.Add(new FrequencyResponse()
                    {
                        SolvedCount = i.ToString(),
                        AbsoluteFrequency = users.Count(x => x.Value == i),
                        RelativeFrequency = (decimal) users.Count(x => x.Value == i) / users.Count() * 100
                    });
                }
                toReturn.Add(new FrequencyResponse()
                {
                    SolvedCount = "Общо",
                    AbsoluteFrequency = users.Count(),
                    RelativeFrequency = 100,
                });

                return toReturn.Cast<T>().ToList();
            }
            else if (typeof(T) == typeof(CentralTrendResponse))
            {
                var users = this.GetDataFromStudentsActivities();
                var ordered = users.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                decimal median, mode, average;
                var middleIndex = ((ordered.Count+1) / 2) - 1;
                if (ordered.Count % 2 == 0)
                {
                    median = (ordered.ElementAt(middleIndex).Value + ordered.ElementAt(middleIndex + 1).Value) / 2;
                }
                else
                {
                    median = ordered.ElementAt(middleIndex).Value;
                }

                mode = ordered.ElementAt(ordered.Count - 1).Value;
                int count = 1;
                for (int i = ordered.Count - 2; i >= 0; i--)
                {
                    if (ordered.ElementAt(i).Value == mode)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }

                mode = mode / count;

                average = (decimal)ordered.Sum(x => x.Value) / ordered.Count;

                return new List<CentralTrendResponse>(){new CentralTrendResponse()
                {
                    AverageValue = average,
                    Median = median,
                    Mode = mode,
                } 
                }.Cast<T>().ToList();
            }
            else if (typeof(T) == typeof(DistractionResponse))
            {
                var users = this.GetDataFromStudentsActivities();
                var ordered = users.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                var scope = ordered.ElementAt(ordered.Count-1).Value - ordered.ElementAt(0).Value;

                var average = (decimal)ordered.Sum(x => x.Value) / ordered.Count();

                decimal variance = 0;
                foreach (var item in ordered)
                {
                    variance += (decimal)Math.Pow(decimal.ToDouble(item.Value - average), 2);
                }
                variance /= ordered.Count;
                var deviation = (decimal)Math.Sqrt(decimal.ToDouble(variance));

                return new List<DistractionResponse>(){new DistractionResponse()
                {
                    Scope = scope,
                    Variance = variance,
                    StandartDeviation = deviation,
                }
                }.Cast<T>().ToList();
            }
            else if(typeof(T) == typeof(CorrelationResponse))
            {
                var results = new List<CorrelationResponse>();
                var users = this.GetCorrelationUserData();
                var userGrades = this.dataManager.ReadResultsFromFile(new List<string>() { GradeFileName1, GradeFileName2 });
                var gradesCourseVisits = new Dictionary<decimal, int>();

                foreach (var item in users)
                {
                    var grade = userGrades.FirstOrDefault(x => x.StudentId.ToString() == item.Key);
                    if (grade is null)
                    {
                        continue;
                    }
                    var gradeDecimal = (decimal)grade.Result;
                    if (!gradesCourseVisits.ContainsKey(gradeDecimal))
                    {
                        gradesCourseVisits.Add(gradeDecimal, 0);
                    }
                    gradesCourseVisits[gradeDecimal] += item.Value;
                }

                foreach (var res in gradesCourseVisits)
                {
                    var result = new CorrelationResponse()
                    {
                        Grade = res.Key,
                        CourseWorkVisits = res.Value,
                    };

                    result.CourseWorkVisitsSquared = result.CourseWorkVisits * result.CourseWorkVisits;
                    result.GradeSquared = result.Grade * result.Grade;
                    result.Multiplied = result.CourseWorkVisits * result.Grade;
                    results.Add(result);
                }
                results = results.OrderBy(x => x.Grade).ToList();
                results.Add(new CorrelationResponse()
                {
                    CourseWorkVisits = results.Sum(x => x.CourseWorkVisits),
                    CourseWorkVisitsSquared = results.Sum(x => x.CourseWorkVisitsSquared),
                    Grade = results.Sum(x => x.Grade),
                    GradeSquared = results.Sum(x => x.GradeSquared),
                    Multiplied = results.Sum(x => x.Multiplied),
                });

                return results.Cast<T>().ToList();
            }
            //Correlation coef is 0.84, if needed
            return new List<T>();
        }

        public List<T> GetValueFromCache<T>()
        {
            switch (typeof(T).ToString())
            {
                case "StudentActivities":
                    return DataManager.Activities.Cast<T>().ToList();
                case "Results":
                    return DataManager.Results.Cast<T>().ToList();
                default:
                    return new List<T>();
            }
        }

        private Dictionary<string, int> GetDataFromStudentsActivities()
        {
            // Get the data from excel
            var users = new Dictionary<string, int>();
            var values = this.GetValueFromCache<StudentsActivities>();
            if (!values.Any())
            {
                values = this.dataManager.ReadActivitiesFromFile(LogsFileName);
            }
            // Get only the required values
            values = values.Where(x => x.Context == CourseWorkEventContext && x.Component == FileSubmissionsComponent).ToList();
            var regex = new Regex(IdRegex);

            // Add the values to dictionary
            foreach (var item in values)
            {
                if (item != null && item.Description != null)
                {
                    var id = regex.Match(item.Description).Value;
                    if (!users.ContainsKey(id))
                    {
                        users.Add(id, 0);
                    }
                    users[id]++;
                }
            }

            return users;
        }

        private Dictionary<string, int> GetCorrelationUserData()
        {
            var users = new Dictionary<string, int>();
            var values = this.GetValueFromCache<StudentsActivities>();
            if (!values.Any())
            {
                values = this.dataManager.ReadActivitiesFromFile(LogsFileName);
            }
            // Get only the required values
            values = values.Where(x => x.Component == WikiComponent && x.Name == UpdateWikiEvent).ToList();
            var regex = new Regex(IdRegex);

            // Add the values to dictionary
            foreach (var item in values)
            {
                if (item != null && item.Description != null)
                {
                    var id = regex.Match(item.Description).Value;
                    if (!users.ContainsKey(id))
                    {
                        users.Add(id, 0);
                    }
                    users[id]++;
                }
            }

            return users;
        }
    }
}
