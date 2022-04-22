using PTS_Doomle.Data;
using PTS_Doomle.Data.Interfaces;
using PTS_Doomle.Response;
using PTS_Doomle.Response.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ICalculationManager, CalculationManager>();
builder.Services.AddTransient<IDataManager, DataManager>();
builder.Services.AddTransient<IExcelDataReader, PTS_Doomle.Data.ExcelDataReader>();
builder.Services.AddTransient<IDataSerializationManager, DataSerializationManager>();
builder.Services.AddTransient<IResponseManager, ResponseManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();