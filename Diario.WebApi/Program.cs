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

registrosGroup.MapGet("/", () =>
{
    return new RegistroService().ListarRegistros();
});

registrosGroup.MapGet("/{id}", (int id) =>
{
    var registro = new RegistroService().BuscarPorId(id);
    return registro is null ? Results.NotFound() : Results.Ok(registro);
});

registrosGroup.MapPost("/", ([FromBody] Registro registro, ClaimsPrincipal user) =>
{
    var usuarioId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    new RegistroService().AdicionarRegistro(registro, usuarioId);
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