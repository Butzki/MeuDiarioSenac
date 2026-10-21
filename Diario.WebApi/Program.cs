using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var jwtSettings = builder.Configuration.GetSection("JWTSettings");
var chaveSecretaBytes = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(chaveSecretaBytes)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- Middleware de tratamento de exceções ---
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

app.UseAuthentication();
app.UseAuthorization();

var usuariosGroup = app.MapGroup("/usuarios");

usuariosGroup.MapPost("/registrar", ([FromBody] Usuario usuario) =>
{
    new UsuarioService().RegistrarUsuario(usuario.Nome, usuario.Senha);
    return Results.Ok("Usuário registrado com sucesso!");
});

usuariosGroup.MapPost("/login", ([FromBody] Usuario usuario) =>
{
    var usuarioAutenticado = new UsuarioService().AutenticarUsuario(usuario.Nome, usuario.Senha);
    var token = new TokenService(builder.Configuration).GerarToken(usuarioAutenticado);
    return Results.Ok(new { token });
});

var registrosGroup = app.MapGroup("/registros").RequireAuthorization();

int ObterUsuarioId(ClaimsPrincipal user) => int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

registrosGroup.MapGet("/", (ClaimsPrincipal user) =>
{
    return new RegistroService().ListarRegistros(ObterUsuarioId(user));
});

registrosGroup.MapGet("/{id}", (int id, ClaimsPrincipal user) =>
{
    var registro = new RegistroService().BuscarPorId(id, ObterUsuarioId(user));
    return registro is null ? Results.NotFound() : Results.Ok(registro);
});

registrosGroup.MapPost("/", ([FromBody] Registro registro, ClaimsPrincipal user) =>
{
    new RegistroService().AdicionarRegistro(registro, ObterUsuarioId(user));
    return Results.Ok("Registro inserido com sucesso!");
});

registrosGroup.MapPut("/{id}", (int id, [FromBody] Registro registro, ClaimsPrincipal user) =>
{
    registro.Id = id;
    var atualizado = new RegistroService().AtualizarRegistro(registro, ObterUsuarioId(user));
    return atualizado ? Results.Ok("Registro atualizado com sucesso!") : Results.NotFound();
});

registrosGroup.MapDelete("/{id}", (int id, ClaimsPrincipal user) =>
{
    var registro = new Registro { Id = id };
    var removido = new RegistroService().RemoverRegistro(registro, ObterUsuarioId(user));
    return removido ? Results.Ok("Registro removido com sucesso!") : Results.NotFound();
});

app.Run();