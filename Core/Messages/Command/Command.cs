using FluentValidation.Results;
using MediatR;

namespace Core.Messages.Command;

public abstract class Command : Message, IRequest<ValidationResult>
{
    public DateTime Timestamp { get; private set; } = DateTime.Now;
    public ValidationResult? ValidationResult { get; set; }

    public virtual bool Valido() => throw new NotImplementedException();
}
