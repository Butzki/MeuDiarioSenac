# MeuDiarioSenac — Back-end

API para gerenciamento de um diário pessoal, com autenticação de usuários e CRUD completo de registros. Construída em C#/.NET com arquitetura em camadas, persistência via Entity Framework Core (MySQL) e exposta tanto por uma aplicação de console quanto por uma Web API.

## Tecnologias

| Tecnologia | Uso neste projeto |
|---|---|
| **.NET 9 / C#** | Linguagem e runtime |
| **ASP.NET Core Minimal APIs** | Endpoints HTTP (`Diario.WebApi`) |
| **Entity Framework Core 9 (Pomelo.MySql)** | ORM e migrations contra MySQL |
| **MySQL** | Banco de dados relacional |
| **System.IdentityModel.Tokens.Jwt** | Geração e validação de token JWT, com claims do usuário autenticado |
| **.NET User Secrets** | Armazenamento local de credenciais sensíveis, fora do controle de versão |

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
- **`Diario.Service`** — orquestra Business + Data numa única operação (ex: validar, atribuir metadados e depois persistir). É a camada que tanto o Console quanto a WebApi consomem, evitando duplicar lógica entre as duas interfaces. Também contém o `TokenService`, responsável por gerar o JWT com as claims do usuário.
- **`Diario`** — aplicação de console com menu interativo, consome `Diario.Service` diretamente.
- **`Diario.WebApi`** — expõe os mesmos recursos via HTTP (Minimal APIs), também consumindo `Diario.Service`, com autenticação JWT obrigatória no grupo `/registros`.

## Funcionalidades atuais

- CRUD completo de registros (inserir, listar, buscar por ID, atualizar, remover), sempre restrito ao usuário autenticado
- Cadastro e autenticação de usuários (`POST /usuarios/registrar`, `POST /usuarios/login`)
- Geração de token JWT com claims (`ClaimTypes.NameIdentifier`, `ClaimTypes.Name`) — o `usuarioId` usado em todas as operações de registro vem do token validado, **nunca** de um parâmetro de URL ou do corpo da requisição, evitando acesso indevido a dados de outro usuário (IDOR)
- **Data do registro atribuída automaticamente pelo servidor** (`DateTime.Now`, no momento da criação), não pelo cliente — elimina problemas de fuso horário/relógio divergente e a necessidade de o cliente calcular ou validar a data
- Validações de regra de negócio antes de qualquer persistência (título obrigatório/máx. 50 caracteres, conteúdo máx. 3000 caracteres; nome de usuário obrigatório/máx. 50 caracteres, senha mínima de 6 caracteres)
- Middleware global de tratamento de exceções: `ArgumentException` (erro de validação de negócio) vira `400` com a mensagem explicando o motivo; qualquer outra exceção não tratada vira `500` genérico, sem detalhes internos expostos

## Endpoints da WebApi

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| `POST` | `/usuarios/registrar` | Não | Cria um novo usuário |
| `POST` | `/usuarios/login` | Não | Autentica um usuário e retorna o token JWT |
| `GET` | `/registros` | Sim (Bearer) | Lista os registros do usuário autenticado |
| `GET` | `/registros/{id}` | Sim (Bearer) | Busca um registro por ID (só retorna se pertencer ao usuário autenticado) |
| `POST` | `/registros` | Sim (Bearer) | Cria um novo registro para o usuário autenticado |
| `PUT` | `/registros/{id}` | Sim (Bearer) | Atualiza um registro existente do usuário autenticado |
| `DELETE` | `/registros/{id}` | Sim (Bearer) | Remove um registro do usuário autenticado |

Para os endpoints autenticados, envie o header:
```
Authorization: Bearer <token recebido no login>
```

## Contratos de requisição/resposta (DTOs ainda não implementados formalmente)

Os endpoints hoje recebem o model de domínio diretamente no corpo (`[FromBody] Registro`/`Usuario`). Enquanto DTOs formais não são implementados (ver Pendências), **estes são os únicos campos que devem ser enviados** — qualquer outro campo do model (`Id`, `Data`, `UsuarioId`) é ignorado ou sobrescrito pelo servidor e não deve ser informado pelo cliente.

**`POST /usuarios/registrar`** e **`POST /usuarios/login`**
```json
{
  "nome": "string",
  "senha": "string"
}
```

**`POST /registros`** e **`PUT /registros/{id}`**
```json
{
  "titulo": "string",
  "conteudo": "string"
}
```
> Não envie `data`: ela é sempre atribuída pelo servidor no momento da criação (`DateTime.Now`), e qualquer valor enviado pelo cliente é ignorado. Enviar a data em formato que não seja ISO 8601 (ex: `dd/MM/yyyy`) causa falha de desserialização e retorna `500` — por isso o campo deve simplesmente ser omitido do corpo.

**Resposta de um registro** (`GET`, `POST`, `PUT`)
```json
{
  "id": 11,
  "titulo": "string",
  "data": "2026-10-21T21:14:09.366279",
  "conteudo": "string"
}
```
> O campo `usuario` (entidade completa relacionada) não é retornado — fica marcado com `[JsonIgnore]` no model, propositalmente, para nunca expor dados do usuário (incluindo a senha, hoje em texto puro) nas respostas de registro.

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

### Configuração do ambiente (obrigatória, uma vez por máquina)

As credenciais sensíveis (connection string do MySQL e chave de assinatura do JWT) **não estão no código** — ficam fora do repositório via [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), para nunca serem commitadas por engano. Cada pessoa que for rodar o projeto, em qualquer máquina, precisa configurar isso localmente antes do primeiro `dotnet run`:

```bash
cd Diario.Data
dotnet user-secrets set "ConnectionStrings:Default" "server=localhost;database=MeuDiarioSenac;uid=root;pwd=SUA_SENHA_LOCAL"

cd ../Diario.WebApi
dotnet user-secrets set "JWTSettings:SecretKey" "$(openssl rand -base64 32)"
```

> O `UserSecretsId` de cada projeto já vem commitado no `.csproj` — só o `set` acima é necessário. Se você pular esse passo, a aplicação lança um erro claro avisando que a connection string não foi configurada, em vez de falhar silenciosamente.

Depois de configurado, aplique as migrations:

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
- **Middleware central de tratamento de exceções**, em vez de `try/catch` espalhado em cada endpoint — um único lugar decide o status code e o formato do erro.
- **Dados derivados do servidor não confiam no cliente**: a data do registro e o `usuarioId` das operações são sempre atribuídos a partir do servidor/token, nunca do corpo da requisição ou da URL.
- **Credenciais sensíveis fora do código** (User Secrets local, nunca commitadas) — cada máquina configura as suas.
- **Migrations como histórico versionado do banco**, nunca alterar uma tabela manualmente fora desse fluxo.
- **Nenhuma biblioteca adicionada sem necessidade concreta** — o projeto evolui com o mínimo de dependências que resolve o problema atual.

## Pendências conhecidas

Ver o documento de acompanhamento publicado durante o desenvolvimento, ou a conversa com o time, para a lista priorizada — resumo rápido:

- Senha de usuário ainda salva em texto puro (decisão consciente, a revisitar)
- DTOs formais ainda não implementados — o contrato de request/response está documentado manualmente acima até isso ser feito; até lá, o cliente não deve enviar `id`, `data` ou `usuarioId` nos corpos de requisição, mesmo que o model aceite
- **`bin/` e `obj/` de todos os projetos estão versionados no Git** (não há `.gitignore`) — precisa de limpeza
- Processo de setup/migração do banco (SQL) ainda não totalmente formalizado para novos ambientes — próximo item a revisitar
