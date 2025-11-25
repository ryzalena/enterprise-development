var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Optional: Disable HTTPS redirection to remove warning
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// Add a default endpoint for root URL
app.MapGet("/", () => "WebAPI is running! Visit /swagger for API documentation.");

app.Run();