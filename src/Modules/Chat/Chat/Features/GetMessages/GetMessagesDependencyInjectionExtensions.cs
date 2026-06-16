using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Queries;

namespace Chat.Features.GetMessages;

internal static class GetMessagesDependencyInjectionExtensions
{
    public static IServiceCollection AddGetMessages(
        this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<GetMessagesQuery, IReadOnlyList<MessageDto>>,
            GetMessagesHandler>();

        return services;
    }
}
