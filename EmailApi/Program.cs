using EmailApi.Biz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton<EmailBiz>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
