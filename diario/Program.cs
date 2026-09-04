var diario = new RegistroDAO();

RegistroBusiness registroBusiness = new RegistroBusiness();

void CriarRegistro(RegistroDAO diario)
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

    DateTime data;
    try
    {
        Console.WriteLine("Data (yyyy-MM-dd): ");
        data = DateTime.Parse(Console.ReadLine());
        registroBusiness.DataAtual(data);
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Erro capturado: {ex.Message}");
        return;
    }
    catch (FormatException)
    {
        Console.WriteLine("\nFormato de data inválido. Por favor, use o formato yyyy-MM-dd, com mascara.\n");
        return;
    }

    Console.WriteLine("Conteudo: ");
    string conteudo = Console.ReadLine();

    diario.Inserir(new Registro
    {
        Titulo = titulo,
        Data = data,
        Conteudo = conteudo,
    });
    Console.WriteLine("\nRegistro inserido com sucesso!\n");
}

void ListarRegistros(RegistroDAO diario)
{
    var registros = diario.ListarTodos();

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

void BuscarRegistro(RegistroDAO diario)
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

void DeletarRegistro(RegistroDAO diario)
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
            diario.Delete(r);
            Console.WriteLine("\nRegistro removido com sucesso!\n");
            return;
        }
        else
        {
            Console.WriteLine("\nRegistro não encontrado.\n");
        }
}

void AtualizarRegistro(RegistroDAO diario)
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

                DateTime data;
                try
                    {
                        Console.WriteLine("Data (yyyy-MM-dd): ");
                        data = DateTime.Parse(Console.ReadLine());
                    } 
                catch (FormatException)
                    {
                Console.WriteLine("\nFormato de data inválido. Por favor, use o formato yyyy-MM-dd, com mascara.\n");
                return;
                    }
                Console.WriteLine("Conteudo: ");
                string conteudo = Console.ReadLine();

                r.Titulo = titulo;
                r.Data = data;
                r.Conteudo = conteudo;
                
                diario.Update(r);

                Console.WriteLine("\nRegistro atualizado com sucesso!\n");
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