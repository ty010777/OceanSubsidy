using EmailApi.Biz;
using Microsoft.EntityFrameworkCore;
using OperationLibrary.Operation.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddDbContext<EmailApiDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<EmailBiz>();

builder.Services.AddControllers();

var app = builder.Build();

// 從設定檔讀取 PathBase (若有設定才套用)
var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseAuthorization();
app.MapControllers();
app.Run();
