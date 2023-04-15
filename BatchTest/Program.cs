using BatchTest.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.Configure<BatchTaskSettings>(builder.Configuration.GetSection("BatchTaskSettings"));
builder.Services.AddControllers();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Your API Title", Version = "v1" });
});

var app = builder.Build();

bool enableSwagger = builder.Configuration.GetValue<bool>("Swagger:Enabled");
// Swagger UI‚ÌÝ’è‚ð’Ç‰Á
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Title v1"));
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
