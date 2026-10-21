public class RegistroService
{
    private readonly RegistroBusiness registroBusiness = new RegistroBusiness();
    private readonly RegistroDAO registroDAO = new RegistroDAO();

public void AdicionarRegistro(Registro registro, int usuarioId)
{
    registroBusiness.TituloInformado(registro.Titulo);
    registroBusiness.TituloMax(registro.Titulo);
    registroBusiness.ConteudoMax(registro.Conteudo);
    registroBusiness.DataAtual(registro.Data);

    registro.UsuarioId = usuarioId;

    registroDAO.Inserir(registro);
}

    public List<Registro> ListarRegistros() => registroDAO.ListarTodos();

    public Registro? BuscarPorId(int id) => registroDAO.BuscarPorId(id);

    public void AtualizarRegistro(Registro registro)
    {
        registroBusiness.TituloInformado(registro.Titulo);
        registroBusiness.TituloMax(registro.Titulo);
        registroBusiness.ConteudoMax(registro.Conteudo);

        registroDAO.Update(registro);
    }

    public void RemoverRegistro(Registro registro) => registroDAO.Delete(registro);
}