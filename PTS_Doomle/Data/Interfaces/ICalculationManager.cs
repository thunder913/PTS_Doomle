namespace PTS_Doomle.Data.Interfaces
{
    public interface ICalculationManager
    {
        public List<T> CalculateValue<T>();

        public List<T> GetValueFromCache<T>();
    }
}
