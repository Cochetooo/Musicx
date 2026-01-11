using System.Text;
using System.Text.Json;
using Musicx.Application.Api.Interfaces.Workers;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Security;
using RabbitMQ.Client;

namespace Musicx.Infrastructure.API.Workers;

public sealed class RabbitMqAuditPublisher : IAuditPublisher, IDisposable
{
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMqAuditPublisher()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
        };
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync("audit_log_exchange", ExchangeType.Fanout, durable: true)
            .GetAwaiter()
            .GetResult();
        
        _channel.QueueDeclareAsync("audit_log_queue", durable: true, exclusive: false, autoDelete: false)
            .GetAwaiter()
            .GetResult();
        
        _channel.QueueBindAsync("audit_log_queue", "audit_log_exchange", "")
            .GetAwaiter()
            .GetResult();
    }

    public async Task PublishAsync(InAuditLog evt)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));
        await _channel.BasicPublishAsync("audit_log_exchange", "", body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}