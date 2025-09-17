using Core.Messages.Integration;

namespace IntegrationHandlers.Events;

public class UsuarioRegistradoIntegrationEvent(Guid id, string nome, string email) : IntegrationEvent
{
    public Guid Id { get; private set; } = id;
    public string Nome { get; private set; } = nome;
    public string Email { get; private set; } = email;
}
