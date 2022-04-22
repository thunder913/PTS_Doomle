namespace PTS_Doomle.Data.Interfaces
{
    public interface IDataSerializationManager
    {
        public string SerializeResponseData<T>(T response);
    }
}
