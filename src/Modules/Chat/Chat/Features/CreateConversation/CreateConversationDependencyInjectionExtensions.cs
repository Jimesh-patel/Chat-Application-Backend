using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Chat.Features.CreateConversation;

internal static class CreateConversationDependencyInjectionExtensions
{
    public static IServiceCollection AddCreateConversation(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICommandHandler<CreateConversationCommand, CreateConversationResponse>,
            CreateConversationHandler>();

        return services;
    }
}
