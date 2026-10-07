public class UsuarioBusiness
{
    public bool NomeInformado(string entrada)
    {
        if (string.IsNullOrEmpty(entrada))
        {
            throw new ArgumentException("O nome não pode estar vazio ou nulo.");
        }
        return true;
    }

    public bool NomeMax(string entrada)
    {
        if (entrada.Length > 50)
        {
            throw new ArgumentException("O nome não pode ser maior do que 50 caracteres.");
        }
        return true;
    }

    public bool SenhaMin(string entrada)
    {
        if (string.IsNullOrEmpty(entrada) || entrada.Length < 6)
        {
            throw new ArgumentException("A senha deve ter no mínimo 6 caracteres.");
        }
        return true;
    }
}