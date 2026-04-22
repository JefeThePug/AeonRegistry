

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGet("/api/Welcome", () =>
{
    var response = new
    {
        Message = "Welcome to the Aeon Registry API",
        Version = "1.0.0",
        TimeOnly = DateTime.Now.ToString("T")
    };
    return Results.Ok(response);
}).WithName("WelcomeMessage");

app.Run();
