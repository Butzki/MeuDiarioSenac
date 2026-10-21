public class RegistroDAO
{
    private MeuDiarioSENACContext conexao = new MeuDiarioSENACContext();

    public void Inserir(Registro registro)
    {
        conexao.Registros.Add(registro);
        conexao.SaveChanges();
    }

    public List<Registro> ListarPorUsuario(int usuarioId)
    {
        return conexao.Registros.Where(r => r.UsuarioId == usuarioId).ToList();
    }

    public Registro BuscarPorId(int id)
    {
        return conexao.Registros.Find(id);
    }

    public void Delete(Registro registro)
    {
        conexao.Registros.Remove(registro);
        conexao.SaveChanges();
    }

    public void Update(Registro registro)
    {
        conexao.Registros.Update(registro);
        conexao.SaveChanges();
    }
}