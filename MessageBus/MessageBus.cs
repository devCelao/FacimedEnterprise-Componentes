using Core.Messages;
using Core.Messages.Integration;
using EasyNetQ;
using EasyNetQ.Internals;
using MessageBus.Interfaces;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace MessageBus;

public class MessageBus : IMessageBus
{
    private IBus _bus = default!;
    private IAdvancedBus _advancedBus = default!;
    private readonly string _connectionString;
    public bool IsConnected => _bus?.Advanced?.IsConnected ?? false;
    public IAdvancedBus AdvancedBus => _bus.Advanced;


    private static readonly Policy _retryPolicy = Policy.Handle<EasyNetQException>()
                                                        .Or<BrokerUnreachableException>()
                                                        .WaitAndRetry(3, retryAttempt =>
                                                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    private static readonly Policy _foreverPolicy = Policy.Handle<EasyNetQException>()
                                                          .Or<BrokerUnreachableException>()
                                                          .RetryForever();

    private void OnDisconnect(object? s, EventArgs e) => _foreverPolicy.Execute(TryConnect);
    public void Dispose() => _bus.Dispose();
    public MessageBus(string connectionString)
    {
        _connectionString = connectionString;
        TryConnect();
    }

    private void TryConnect()
    {
        if (IsConnected) return;

        _retryPolicy.Execute(() =>
        {
            _bus = RabbitHutch.CreateBus(_connectionString);
            _advancedBus = _bus.Advanced;
            _advancedBus.Disconnected += OnDisconnect;
        });
    }

    public void Publish<T>(T message) where T : IntegrationEvent
    {
        TryConnect();
        _bus.PubSub.Publish(message);
    }
    public async Task PublishAsync<T>(T message) where T : IntegrationEvent
    {
        TryConnect();
        await _bus.PubSub.PublishAsync(message);
    }
    public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
    {
        TryConnect();
        _bus.PubSub.Subscribe(subscriptionId, onMessage);
    }
    public void SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
    {
        TryConnect();
        _bus.PubSub.SubscribeAsync(subscriptionId, onMessage);
    }
    public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.Request<TRequest, TResponse>(request);
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return await _bus.Rpc.RequestAsync<TRequest, TResponse>(request);
    }

    public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.Respond(responder);
    }

    public AwaitableDisposable<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
        where TRequest : IntegrationEvent where TResponse : ResponseMessage
    {
        TryConnect();
        return _bus.Rpc.RespondAsync(responder);
    }
}
