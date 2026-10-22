public class RegistroService
{
    private readonly RegistroBusiness registroBusiness = new RegistroBusiness();
    private readonly RegistroDAO registroDAO = new RegistroDAO();

    public void AdicionarRegistro(Registro registro, int usuarioId)
{
        registroBusiness.TituloInformado(registro.Titulo);
        registroBusiness.TituloMax(registro.Titulo);
        registroBusiness.ConteudoMax(registro.Conteudo);

        registro.Data = DateTime.Now;              
        registroBusiness.DataAtual(registro.Data);  

        registro.UsuarioId = usuarioId;

        registroDAO.Inserir(registro);
}

    public List<Registro> ListarRegistros(int usuarioId) => registroDAO.ListarPorUsuario(usuarioId);

    public Registro? BuscarPorId(int id, int usuarioId)
    {
        var registro = registroDAO.BuscarPorId(id);
        return registro is not null && registro.UsuarioId == usuarioId ? registro : null;
    }

    public bool AtualizarRegistro(Registro registro, int usuarioId)
    {
        var registroExistente = registroDAO.BuscarPorId(registro.Id);
        if (registroExistente is null || registroExistente.UsuarioId != usuarioId) return false;

        registroBusiness.TituloInformado(registro.Titulo);
        registroBusiness.TituloMax(registro.Titulo);
        registroBusiness.ConteudoMax(registro.Conteudo);

        registroExistente.Titulo = registro.Titulo;
        registroExistente.Conteudo = registro.Conteudo;
        registroExistente.Data = DateTime.Now;
        registroDAO.Update(registroExistente);
        return true;
    }

    public bool RemoverRegistro(Registro registro, int usuarioId)
    {
        var registroExistente = registroDAO.BuscarPorId(registro.Id);
        if (registroExistente is null || registroExistente.UsuarioId != usuarioId) return false;

        registroDAO.Delete(registroExistente);
        return true;
    }
}
