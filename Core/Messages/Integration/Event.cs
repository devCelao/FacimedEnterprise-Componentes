using Core.Messages;
using MediatR;

namespace Core.Messages.Integration;

public class Event : Message, INotification
{
    public DateTime Timestamp { get; private set; } = DateTime.Now;
}
