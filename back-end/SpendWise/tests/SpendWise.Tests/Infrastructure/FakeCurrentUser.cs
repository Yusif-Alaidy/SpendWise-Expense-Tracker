using SpendWise.Application.Common.Interfaces;

namespace SpendWise.Tests.Infrastructure;

public class FakeCurrentUser : ICurrentUserService
{
    public Guid? UserId { get; set; }
}
