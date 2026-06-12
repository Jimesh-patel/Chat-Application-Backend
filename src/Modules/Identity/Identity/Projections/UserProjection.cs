using Identity.Domain;
using Identity.Domain.Events;
using Identity.ReadModels;
using Marten.Events.Aggregation;

namespace Identity.Projections;

public sealed partial class UserProjection
    : SingleStreamProjection<UserReadModel, UserId>
{
    public static UserReadModel Create(
        UserRegistered @event)
    {
        return new UserReadModel
        {
            Id = @event.UserId.Value,
            Email = @event.Email,
            Username = @event.Username,
            DisplayName = @event.DisplayName,
            PasswordHash = @event.PasswordHash,
            CreatedAtUtc = @event.OccurredOnUtc
        };
    }
}