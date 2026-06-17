using Akka.Actor;
using Akka.Hosting;
using Chat.Features.CreateConversation;
using Chat.Features.FindAllConversations;
using Chat.Features.GetMessages;
using Chat.Features.MarkSeen;
using Chat.Features.SendMessage;
using Chat.Features.UserTyping;
using Chat.Infrastructure.Actors;
using Chat.Projections;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Akka.Actors;
using Platform.Contracts.Modules;

namespace Chat;

public sealed class ChatModule : IModule
{
    public IServiceCollection RegisterModule(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCreateConversation();
        services.AddSendMessage();
        services.AddFindAllConversations();
        services.AddGetMessages();
        services.AddMarkMessageSeen();
        services.AddSetUserTyping();
        services.AddScoped<Platform.Realtime.Abstractions.IChatHubDispatcher, Chat.Infrastructure.Realtime.ChatHubDispatcher>();
        services.AddConversationProjection();
        services.AddActorSystem();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(
        IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCreateConversation();
        endpoints.MapSendMessage();
        endpoints.MapFindAllConversations();
        endpoints.MapGetMessages();
        return endpoints;
    }
}
