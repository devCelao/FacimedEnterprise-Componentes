

using FluentValidation.Results;

namespace Core.Messages;

public class ResponseMessage(ValidationResult validationResult) : Message
{
    public ValidationResult ValidationResult { get; set; } = validationResult;
}