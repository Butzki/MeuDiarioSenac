# MeuDiarioSenac
## Funcionalidades

- **CRUD completo** de registros (inserir, listar, buscar por ID, atualizar e remover)
- **Persistência de dados** em banco MySQL, usando Entity Framework Core como ORM
- **Camada de acesso a dados (DAO)** separada da lógica de interface, isolando as operações de banco das interações com o usuário

## Tecnologias

- C# / .NET
- Entity Framework Core
- MySQL

## Antes de rodar: é necessário executar as Migrations

O banco de dados **não está incluído neste repositório**. 
Para que o projeto funcione, é obrigatório gerar e aplicar as migrations do Entity Framework Core antes de tentar rodar ou testar o CRUD. 
Sem isso, o banco `MeuDiarioSenac` e suas tabelas não vão existir, e a aplicação vai falhar ao tentar se conectar.

### Pré-requisitos

- .NET SDK instalado
- MySQL Server rodando localmente
- Ferramenta `dotnet-ef` instalada globalmente:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Passo a passo

1. **Clone o repositório**
   ```bash
   git clone https://github.com/Butzki/MeuDiarioSenac.git
   cd MeuDiarioSenac
   ```

2. **Confira a connection string** em `diario.data/banco.cs`. Por padrão, o projeto espera:
   ```
   server=localhost;database=MeuDiarioSenac;uid=root;pwd=1234
   ```
   Ajuste usuário, senha ou servidor conforme o seu ambiente MySQL local.

3. **Restaure os pacotes do projeto**
   ```bash
   dotnet restore
   ```

4. **Gere as migrations** (cria os arquivos que descrevem a estrutura do banco a partir do modelo)
   ```bash
   dotnet ef migrations add InitialCreate
   ```

5. **Aplique as migrations no banco de dados** (cria o banco `MeuDiarioSenac` e as tabelas `Usuarios` e `Registros`)
   ```bash
   dotnet ef database update
   ```

6. **Rode a aplicação**
   ```bash
   dotnet run --project diario
   ```

Depois desses passos, o menu do CRUD (inserir, listar, buscar, atualizar e remover registros) deve funcionar normalmente.
