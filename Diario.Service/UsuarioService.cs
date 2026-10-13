public class UsuarioService
{
    private readonly UsuarioBusiness usuarioBusiness = new UsuarioBusiness();
    private readonly UsuarioDAO usuarioDAO = new UsuarioDAO();

    public void RegistrarUsuario(string nome, string senha)
    {
        usuarioBusiness.NomeInformado(nome);
        usuarioBusiness.NomeMax(nome);
        usuarioBusiness.SenhaMin(senha);

        if (usuarioDAO.BuscarPorNome(nome) != null)
        {
            throw new ArgumentException("Já existe um usuário com esse nome.");
        }

        var usuario = new Usuario
        {
            Nome = nome,
            Senha = senha
        };

        usuarioDAO.Inserir(usuario);
    }

    public Usuario AutenticarUsuario(string nome, string senha)
    {
        var usuario = usuarioDAO.BuscarPorNome(nome);
        if (usuario == null || usuario.Senha != senha)
        {
            throw new ArgumentException("Usuário ou senha inválidos.");
        }

        return usuario;
    }
}