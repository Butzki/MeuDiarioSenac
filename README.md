# MeuDiarioSenac — Back-end

API para gerenciamento de um diário pessoal, com autenticação de usuários e CRUD completo de registros. Construída em C#/.NET com arquitetura em camadas, persistência via Entity Framework Core (MySQL) e exposta tanto por uma aplicação de console quanto por uma Web API.

## Tecnologias

| Tecnologia | Uso neste projeto |
|---|---|
| **.NET 9 / C#** | Linguagem e runtime |
| **ASP.NET Core Minimal APIs** | Endpoints HTTP (`Diario.WebApi`) |
| **Entity Framework Core 9 (Pomelo.MySql)** | ORM e migrations contra MySQL |
| **MySQL** | Banco de dados relacional |
| **System.IdentityModel.Tokens.Jwt** | Geração de token JWT |

## Estrutura do projeto

Arquitetura em camadas, cada uma só referenciando a camada imediatamente abaixo — sem dependências circulares:

```
Diario            (Console)   ─┬─→ Diario.Business ─┬─→ Diario.Model
Diario.WebApi      (API)      ─┤                    │
                                ├─→ Diario.Data ──────┘
                                └─→ Diario.Service ──┬─→ Diario.Business
                                                       ├─→ Diario.Data
                                                       └─→ Diario.Model
```

- **`Diario.Model`** — entidades de domínio puras (`Registro`, `Usuario`), sem lógica nem dependência de banco.
- **`Diario.Data`** — acesso a dados: `MeuDiarioSENACContext` (o `DbContext` do EF Core) e os DAOs (`RegistroDAO`, `UsuarioDAO`).
- **`Diario.Business`** — regras de validação de negócio (`RegistroBusiness`, `UsuarioBusiness`), cada regra lança `ArgumentException` com mensagem explicando o motivo.
- **`Diario.Service`** — orquestra Business + Data numa única operação (ex: validar e depois persistir). É a camada que tanto o Console quanto a WebApi consomem, evitando duplicar lógica entre as duas interfaces. Também contém o `TokenService`, responsável por gerar o JWT.
- **`Diario`** — aplicação de console com menu interativo, consome `Diario.Service` diretamente.
- **`Diario.WebApi`** — expõe os mesmos recursos via HTTP (Minimal APIs), também consumindo `Diario.Service`.

## Funcionalidades atuais

- CRUD completo de registros (inserir, listar, buscar por ID, atualizar, remover)
- Cadastro e autenticação de usuários (`POST /usuarios/registrar`, `POST /usuarios/login`)
- Geração de token JWT a partir das configurações em `appsettings.json`
- Validações de regra de negócio antes de qualquer persistência (título obrigatório/máx. 50 caracteres, conteúdo máx. 3000 caracteres, data obrigatoriamente atual; nome de usuário obrigatório/máx. 50 caracteres, senha mínima de 6 caracteres)

## Endpoints da WebApi

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/registros` | Lista todos os registros |
| `GET` | `/registros/{id}` | Busca um registro por ID |
| `POST` | `/registros` | Cria um novo registro |
| `PUT` | `/registros/{id}` | Atualiza um registro existente |
| `DELETE` | `/registros/{id}` | Remove um registro |
| `POST` | `/usuarios/registrar` | Cria um novo usuário |
| `POST` | `/usuarios/login` | Autentica um usuário |
| `POST` | `/login/auth` | Endpoint de teste do `TokenService` (ver seção de pendências) |

## Como rodar

### Pré-requisitos

- .NET SDK 9 instalado
- MySQL Server rodando localmente
- Ferramenta `dotnet-ef` instalada globalmente:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Passo a passo

```bash
git clone https://github.com/Butzki/MeuDiarioSenac.git
cd MeuDiarioSenac
dotnet restore
```

Configure a connection string em `Diario.Data/banco.cs` (ver seção de pendências sobre isso) e aplique as migrations:

```bash
cd Diario.Data
dotnet ef database update
cd ..
```

Para rodar a **WebApi**:
```bash
dotnet run --project Diario.WebApi
```

Para rodar o **Console**:
```bash
dotnet run --project Diario
```

## Boas práticas já aplicadas neste projeto (referência rápida)

Para quem está começando em C#/.NET, estes são os padrões que este repositório já segue e vale manter em qualquer projeto novo:

- **Nomenclatura PascalCase** para projetos, pastas e classes (`Diario.Business`, não `diario.business`) — é a convenção oficial do .NET.
- **Uma responsabilidade por camada**: o Model nunca sabe de banco; o Data nunca valida regra de negócio; o Business nunca persiste nada.
- **Validação lança exceção com mensagem clara**, em vez de retornar `bool`/`null` silenciosamente — quem chama sempre sabe exatamente o que deu errado.
- **Migrations como histórico versionado do banco**, nunca alterar uma tabela manualmente fora desse fluxo.
- **Nenhuma biblioteca adicionada sem necessidade concreta** — o projeto evolui com o mínimo de dependências que resolve o problema atual.

## Pendências conhecidas

Ver o documento de acompanhamento publicado durante o desenvolvimento, ou a conversa com o time, para a lista priorizada — resumo rápido:

- Senha de usuário ainda salva em texto puro (decisão consciente, a revisitar)
- `TokenService` ainda não inclui os dados do usuário (claims) dentro do token
- Sem middleware de tratamento de exceções (erros de validação retornam 500 em vez de 400)
- Connection string do MySQL hardcoded em `banco.cs`
- **`bin/` e `obj/` de todos os projetos estão versionados no Git** (não há `.gitignore`) — precisa de limpeza
