using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Chat.Features.UserTyping;

internal static class SetUserTypingDependencyInjectionExtensions
{
    public static IServiceCollection AddSetUserTyping(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<SetUserTypingCommand, Guid>, SetUserTypingHandler>();
        return services;
    }
}
