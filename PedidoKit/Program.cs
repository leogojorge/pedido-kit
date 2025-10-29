using PedidoKit.Models;
using PedidoKit.Csv;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:7071", "https://localhost:7072");

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "Tabela_Kits_Borracha_Escapamento.csv");
var csvService = new CsvService(csvPath);

app.MapGet("/api/kits", () =>
{
    return Results.Json(csvService.GetAll());
});

app.MapGet("/api/kits/{id:int}", (int id) =>
{
    var kit = csvService.GetById(id);
    return kit is not null ? Results.Json(kit) : Results.NotFound();
});

app.MapPost("/api/kits", async (HttpRequest request) =>
{
    var form = await request.ReadFromJsonAsync<Kit>();
    if (form == null) return Results.BadRequest("Dados inválidos");

    csvService.Add(form);
    return Results.Ok();
});

app.MapPut("/api/kits/{id:int}", async (int id, HttpRequest request) =>
{
    var form = await request.ReadFromJsonAsync<Kit>();
    if (form == null) return Results.BadRequest("Dados inválidos");

    csvService.Update(id, form);
    return Results.Ok();
});


app.MapDelete("/api/kits/{id:int}", (int id) =>
{
    try
    {
        csvService.Delete(id);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.Run();