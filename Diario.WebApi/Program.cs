using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

var registrosGroup = app.MapGroup("/registros");

registrosGroup.MapGet("/", () =>
{
    return new RegistroService().ListarRegistros();
});

registrosGroup.MapGet("/{id}", (int id) =>
{
    var registro = new RegistroService().BuscarPorId(id);
    return registro is null ? Results.NotFound() : Results.Ok(registro);
});

registrosGroup.MapPost("/", ([FromBody] Registro registro) =>
{
    new RegistroService().AdicionarRegistro(registro);
    return Results.Ok("Registro inserido com sucesso!");
});

registrosGroup.MapPut("/{id}", (int id, [FromBody] Registro registro) =>
{
    registro.Id = id;
    new RegistroService().AtualizarRegistro(registro);
    return Results.Ok("Registro atualizado com sucesso!");
});

registrosGroup.MapDelete("/{id}", (int id) =>
{
    var registro = new RegistroService().BuscarPorId(id);
    if (registro is null) return Results.NotFound();

    new RegistroService().RemoverRegistro(registro);
    return Results.Ok("Registro removido com sucesso!");
});

app.Run();