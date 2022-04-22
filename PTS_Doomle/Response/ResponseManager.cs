using PTS_Doomle.Data.Interfaces;
using PTS_Doomle.Response.Interfaces;

namespace PTS_Doomle.Response
{
    public class ResponseManager : IResponseManager
    {
        private readonly IDataSerializationManager dataSerializationManager;
        private readonly ICalculationManager calculationManager;

        public ResponseManager(IDataSerializationManager dataSerializationManager, ICalculationManager calculationManager)
        {
            this.dataSerializationManager = dataSerializationManager;
            this.calculationManager = calculationManager;
        }


        public string GenerateResponse<T>()
        {
            return dataSerializationManager.SerializeResponseData<List<T>>(calculationManager.CalculateValue<T>());
        }
    }
}
