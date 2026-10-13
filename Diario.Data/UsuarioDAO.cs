public class UsuarioDAO
{
    private MeuDiarioSENACContext conexao = new MeuDiarioSENACContext();

    public void Inserir(Usuario usuario)
    {
        conexao.Usuarios.Add(usuario);
        conexao.SaveChanges();
    }

    public Usuario BuscarPorNome(string nome)
    {
        return conexao.Usuarios.FirstOrDefault(u => u.Nome == nome);
    }

    public Usuario BuscarPorId(int id)
    {
        return conexao.Usuarios.Find(id);
    }
}