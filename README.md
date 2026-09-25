# MeuDiarioSenac

Aplicação de console em C# para gerenciamento de um diário pessoal, com CRUD completo de registros, persistidos em banco de dados MySQL via Entity Framework Core.

## Funcionalidades

- **CRUD completo** de registros (inserir, listar, buscar por ID, atualizar e remover)
- **Validações de regra de negócio** (título obrigatório e limitado a 50 caracteres, conteúdo limitado a 3000 caracteres, data obrigatoriamente atual)
- **Persistência de dados** em banco MySQL, usando Entity Framework Core como ORM
- **Camada de acesso a dados (DAO)** separada da lógica de interface, isolando as operações de banco das interações com o usuário

## Estrutura do projeto

O projeto é dividido em múltiplos projetos .NET:

- **`diario`** — projeto de console (executável), com o menu interativo (`Program.cs`)
- **`diario.data`** — camada de acesso a dados: `RegistroDAO` e o contexto do Entity Framework (`banco.cs`)
- **`diario.business`** — regras de negócio e validações (`RegistroBusiness`)
- **`diario.model`** — entidades do domínio (`Registro`, `Usuario`)

## Tecnologias

- C# / .NET
- Entity Framework Core
- MySQL

## Antes de rodar: é necessário ter o MySQL Server e executar as Migrations

O banco de dados **não está incluído neste repositório** — apenas o código-fonte. Para que o projeto funcione, é obrigatório ter um servidor MySQL rodando localmente e gerar/aplicar as migrations do Entity Framework Core antes de tentar rodar ou testar o CRUD. Sem isso, o banco `MeuDiarioSenac` e suas tabelas não vão existir, e a aplicação vai falhar ao tentar se conectar.

### Pré-requisitos

- .NET SDK instalado
- **MySQL Server** instalado e rodando localmente (baixe em [dev.mysql.com/downloads/installer](https://dev.mysql.com/downloads/installer/) — não é necessário criar conta na Oracle, use a opção "No thanks, just start my download")
- Ferramenta `dotnet-ef` instalada globalmente:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- Pacote `Microsoft.EntityFrameworkCore.Design` instalado no projeto `diario` (necessário para o comando `dotnet ef` funcionar em uma estrutura multi-projeto). Use a mesma versão principal do EF Core já usada no `diario.data`:
  ```bash
  dotnet add diario package Microsoft.EntityFrameworkCore.Design --version <versão do EF Core do projeto>
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

4. **Gere as migrations** (cria os arquivos que descrevem a estrutura do banco a partir do modelo). Como o `DbContext` fica em `diario.data` e o executável é `diario`, é necessário indicar os dois projetos:
   ```bash
   dotnet ef migrations add InitialCreate --project diario.data --startup-project diario
   ```

5. **Aplique as migrations no banco de dados** (cria o banco `MeuDiarioSenac` e a tabela `Registros`)
   ```bash
   dotnet ef database update --project diario.data --startup-project diario
   ```

6. **Rode a aplicação**
   ```bash
   dotnet run --project diario
   ```

### Usando o menu

Ao rodar a aplicação, o menu apresenta as seguintes opções:

```
1 - Inserir registro
2 - Listar registros
3 - Buscar registro por ID
4 - Remover registro por ID
5 - Atualizar registro por ID
6 - Sair
```

> **Observação:** todo registro criado é associado a um `UsuarioId` fixo (`1`), já que ainda não há um cadastro de usuários implementado nesta versão do projeto.