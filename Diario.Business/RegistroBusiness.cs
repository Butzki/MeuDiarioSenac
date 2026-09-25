// métodos bool, implementar try/catch para validar regra de negócio
// titulo obrigatorio e max 50 char, Conteudo max 3000 caracteres, data somente atual
 
public class RegistroBusiness {
public bool TituloInformado(string entrada)
{
    if (string.IsNullOrEmpty(entrada))
    {
        throw new ArgumentException("O título não pode estar vazio ou nulo.");
    }
    return true;
}

public bool TituloMax(string entrada)
{
    if (entrada.Length > 50)
    {
        throw new ArgumentException("O título não pode ser maior do que 50 caracteres.");
    }
    return true;
}

public bool ConteudoMax(string entrada)
{
    if (entrada.Length > 3000)
    {
        throw new ArgumentException("O conteúdo não pode ser maior do que 3000 caracteres.");
    }
    return true;
}

public bool DataAtual(DateTime entrada)
{
    if (entrada.Date != DateTime.Now.Date)
    {
        throw new ArgumentException("A data deve ser a data atual.");
    }

    return true;
}

}


