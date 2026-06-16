using Chat.Features.GetMessages;
using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Chat.Features.SendMessage;

/// <summary>
/// Registers the <see cref="SendMessageHandler"/> with the DI container.
/// </summary>
internal static class SendMessageDependencyInjectionExtensions
{
    public static IServiceCollection AddSendMessage(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICommandHandler<SendMessageCommand, MessageDto>,
            SendMessageHandler>();

        return services;
    }
}
