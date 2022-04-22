using PTS_Doomle.Data;
using PTS_Doomle.Data.Interfaces;
using PTS_Doomle.Response;
using PTS_Doomle.Response.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ICalculationManager, CalculationManager>();
builder.Services.AddTransient<IDataManager, DataManager>();
builder.Services.AddTransient<IExcelDataReader, PTS_Doomle.Data.ExcelDataReader>();
builder.Services.AddTransient<IDataSerializationManager, DataSerializationManager>();
builder.Services.AddTransient<IResponseManager, ResponseManager>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;
app.Run();