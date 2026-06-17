using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Chat.Features.MarkSeen;

public static class MarkMessageSeenDependencyInjectionExtensions
{
    public static IServiceCollection AddMarkMessageSeen(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICommandHandler<MarkMessageSeenCommand, Guid>,
            MarkMessageSeenHandler>();

        return services;
    }
}
