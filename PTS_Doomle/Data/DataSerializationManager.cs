using PTS_Doomle.Data.Interfaces;
using Newtonsoft.Json;
namespace PTS_Doomle.Data
{
    public class DataSerializationManager : IDataSerializationManager
    {
        public string SerializeResponseData<T>(T response)
        {
            return JsonConvert.SerializeObject(response);
        }
    }
}
