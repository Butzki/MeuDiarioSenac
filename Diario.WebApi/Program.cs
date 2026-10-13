using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (ArgumentException ex)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { erro = ex.Message });
    }
    catch (Exception)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { erro = "Ocorreu um erro interno." });
    }
});

app.MapPost("/login/auth", () =>
{
    TokenService authService = new TokenService(builder.Configuration);
    return authService.GerarToken(null);

});

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
    new RegistroService().AdicionarRegistro(registro, registro.UsuarioId);
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

var usuariosGroup = app.MapGroup("/usuarios");

usuariosGroup.MapPost("/registrar", ([FromBody] Usuario usuario) =>
{
    new UsuarioService().RegistrarUsuario(usuario.Nome, usuario.Senha);
    return Results.Ok("Usuário registrado com sucesso!");
});

usuariosGroup.MapPost("/login", ([FromBody] Usuario usuario) =>
{
    var usuarioAutenticado = new UsuarioService().AutenticarUsuario(usuario.Nome, usuario.Senha);
    return Results.Ok($"Login realizado com sucesso! Bem-vindo, {usuarioAutenticado.Nome}.");
});

app.Run();