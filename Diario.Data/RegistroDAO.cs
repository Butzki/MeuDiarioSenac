
public class RegistroDAO
{
    private MeuDiarioSENACContext  conexao = new MeuDiarioSENACContext();

    public void Inserir(Registro registro)
    {
        conexao.Registros.Add(registro);
        conexao.SaveChanges();
    }

    public List<Registro> ListarTodos()
    {
        return conexao.Registros.ToList();
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
