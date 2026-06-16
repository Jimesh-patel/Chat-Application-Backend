using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Queries;

namespace Chat.Features.FindAllConversations;

internal static class FindAllConversationsDependencyInjectionExtensions
{
    public static IServiceCollection AddFindAllConversations(
        this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<FindAllConversationsQuery, IReadOnlyList<ConversationResponse>>,
            FindAllConversationsHandler>();

        return services;
    }
}
