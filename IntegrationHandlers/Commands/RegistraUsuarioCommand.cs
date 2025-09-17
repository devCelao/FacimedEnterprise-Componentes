using Core.Messages.Command;
using FluentValidation;

namespace IntegrationHandlers.Commands;

public class RegistraUsuarioCommand : Command
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public RegistraUsuarioCommand(Guid id, string nome, string email)
    {
        AggregatedId = id;
        Id = id;
        Nome = nome;
        Email = email;
    }
    public override bool Valido()
    {
        ValidationResult = new RegistrarClienteValidation().Validate(this);

        return ValidationResult.IsValid;
    }

    public class RegistrarClienteValidation : AbstractValidator<RegistraUsuarioCommand>
    {
        public RegistrarClienteValidation()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Id do cliente inválido.");

            RuleFor(c => c.Nome)
               .NotEmpty()
               .WithMessage("O nome do cliente não foi informado.");

            RuleFor(c => c.Email)
                .Must(TerEmailValido)
                .WithMessage("O e-mail informado não é válido.");

        }

        protected static bool TerEmailValido(string email)
        {
            return Core.DomainObjects.Email.Validar(email);
        }
    }
}
