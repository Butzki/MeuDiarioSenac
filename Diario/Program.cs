UsuarioService usuarioService = new UsuarioService();
Usuario usuarioLogado = IniciarSessao();

var diario = new RegistroService();

RegistroBusiness registroBusiness = new RegistroBusiness();

Usuario IniciarSessao()
{
    while (true)
    {
        Console.WriteLine("=== Meu Diário ===");
        Console.WriteLine("1 - Entrar");
        Console.WriteLine("2 - Criar conta");
        var opcao = Console.ReadLine();

        switch (opcao)
        {
            case "1":
                return Login();
            case "2":
                Registrar();
                break;
            default:
                Console.WriteLine("\nOpção inválida.\n");
                break;
        }
    }
}

Usuario Login()
{
    while (true)
    {
        Console.WriteLine("=== Login ===");
        Console.WriteLine("Nome: ");
        string nome = Console.ReadLine();

        Console.WriteLine("Senha: ");
        string senha = Console.ReadLine();

        try
        {
            var usuario = usuarioService.AutenticarUsuario(nome, senha);
            Console.WriteLine($"\nBem-vindo, {usuario.Nome}!\n");
            return usuario;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\nErro: {ex.Message}\n");
        }
    }
}

void Registrar()
{
    Console.WriteLine("=== Criar conta ===");
    Console.WriteLine("Nome: ");
    string nome = Console.ReadLine();

    Console.WriteLine("Senha: ");
    string senha = Console.ReadLine();

    try
    {
        usuarioService.RegistrarUsuario(nome, senha);
        Console.WriteLine("\nConta criada com sucesso! Faça login para continuar.\n");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"\nErro: {ex.Message}\n");
    }
}

void CriarRegistro(RegistroService diario)
{
    Console.WriteLine("Titulo: ");
    string titulo = Console.ReadLine();

    try
        {
        registroBusiness.TituloInformado(titulo);
        registroBusiness.TituloMax(titulo);
        Console.WriteLine("Título cadastrado com sucesso!");
        }
    catch (ArgumentException ex)
        {
        Console.WriteLine($"Erro capturado: {ex.Message}");
        return;
        }

    Console.WriteLine("Conteudo: ");
    string conteudo = Console.ReadLine();

    try
    {
        diario.AdicionarRegistro(new Registro
        {
            Titulo = titulo,
            Data = DateTime.Now,
            Conteudo = conteudo,
        }, usuarioLogado.Id);
        Console.WriteLine("\nRegistro inserido com sucesso!\n");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"\nErro capturado: {ex.Message}\n");
    }
}

void ListarRegistros(RegistroService diario)
{
    var registros = diario.ListarRegistros();

    if (registros.Count == 0)
    {
        Console.WriteLine("\nNenhum registro encontrado.\n");
        return;
    }

    foreach (var r in registros)
    {
        Console.WriteLine($"ID: {r.Id}, Titulo: {r.Titulo}, Data: {r.Data}, Conteudo: {r.Conteudo}");
    }
}

void BuscarRegistro(RegistroService diario)
{
    Console.WriteLine("Digite o ID do registro que deseja encontrar: ");
    int id = int.Parse(Console.ReadLine());

    var r = diario.BuscarPorId(id);
        if (r == null)
        {
                    Console.WriteLine("\nRegistro não encontrado.\n");
        }
        else if (r.Id == id)
        {
            Console.WriteLine($"ID: {r.Id}, Titulo: {r.Titulo}, Data: {r.Data}, Conteudo: {r.Conteudo}");
            return;
        }
        else
        {
            Console.WriteLine("\nRegistro não encontrado.\n");
        }
}

void DeletarRegistro(RegistroService diario)
{
    Console.WriteLine("Digite o ID do registro que deseja remover: ");
    int id = int.Parse(Console.ReadLine());

    var r = diario.BuscarPorId(id);
        if (r == null)
        {
                    Console.WriteLine("\nRegistro não encontrado.\n");
        }
        else if (r.Id == id)
        {
            diario.RemoverRegistro(r);
            Console.WriteLine("\nRegistro removido com sucesso!\n");
            return;
        }
        else
        {
            Console.WriteLine("\nRegistro não encontrado.\n");
        }
}

void AtualizarRegistro(RegistroService diario)
{
    Console.WriteLine("Digite o ID do registro que deseja atualizar: ");
    int id = int.Parse(Console.ReadLine());

    var r = diario.BuscarPorId(id);
        if (r == null)
        {
                    Console.WriteLine("\nRegistro não encontrado.\n");
        }

        else if (r.Id == id)
        {
                Console.WriteLine("Titulo: ");
                string titulo = Console.ReadLine();

                Console.WriteLine("Conteudo: ");
                string conteudo = Console.ReadLine();

                r.Titulo = titulo;
                r.Data = DateTime.Now;
                r.Conteudo = conteudo;

                try
                {
                    diario.AtualizarRegistro(r);
                    Console.WriteLine("\nRegistro atualizado com sucesso!\n");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"\nErro capturado: {ex.Message}\n");
                }
                return;
                }       

            else
            {
                Console.WriteLine("\nRegistro não encontrado.\n");
            }
}

while (true)
{
    Console.WriteLine("Escolha uma opção:");
    Console.WriteLine("1 - Inserir registro");
    Console.WriteLine("2 - Listar registros");
    Console.WriteLine("3 - Buscar registro por ID");
    Console.WriteLine("4 - Remover registro por ID");
    Console.WriteLine("5 - Atualizar registro por ID");
    Console.WriteLine("6 - Sair");

    var opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1":
            CriarRegistro(diario);
            Continue();
            break;
        case "2":
            ListarRegistros(diario);
            Continue();
            break;
        case "3":
            BuscarRegistro(diario);
            Continue();
            break;
        case "4":
            DeletarRegistro(diario);
            Continue();
            break;
        case "5":
            AtualizarRegistro(diario);
            Continue();
            break;
        case "6":
            return;
        default:
            Console.WriteLine("\nOpção inválida.\n");
            break;
    }
}

void Continue()
{
    Console.WriteLine("\nPressione enter para continuar...");
    Console.ReadLine();
    Console.Clear();
}