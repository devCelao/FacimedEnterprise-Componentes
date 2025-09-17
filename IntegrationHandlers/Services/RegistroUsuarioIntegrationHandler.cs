using Microsoft.Extensions.Hosting;
using MessageBus.Interfaces;
using Core.Messages;
using IntegrationHandlers.Events;
using IntegrationHandlers.Commands;
using Core.Mediator;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationHandlers.Services;

public class RegistroUsuarioIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus) : BackgroundService
{
    private readonly IMessageBus message = bus;
    private readonly IServiceProvider service = serviceProvider;
    private void SetResponder() => message.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request => await RegistrarCliente(request));
    private void OnConnect(object? sender, EventArgs e) => SetResponder();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetResponder();

        message.AdvancedBus.Connected += OnConnect;

        return Task.CompletedTask;
    }
    private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
    {
        var clienteCommand = new RegistraUsuarioCommand(message.Id, message.Nome, message.Email);
        ValidationResult sucesso;

        using (var scope = service.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
            sucesso = await mediator.EnviarComando(clienteCommand);
        }

        return new ResponseMessage(sucesso);
    }
}
