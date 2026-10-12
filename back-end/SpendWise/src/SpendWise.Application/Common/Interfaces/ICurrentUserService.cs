// SpendWise.Application/Common/Interfaces/ICurrentUserService.cs
namespace SpendWise.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}