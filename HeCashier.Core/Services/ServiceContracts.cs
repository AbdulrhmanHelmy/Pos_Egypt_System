using HeCashier.Models;

namespace HeCashier.Services
{
    public sealed record AuthenticatedUser(int Id, string UserName);

    public interface IAuthenticationService
    {
        Task<AuthenticatedUser?> AuthenticateAsync(string userName, string password);
    }

    public interface IShiftService
    {
        Task<Shift?> GetActiveShiftAsync(int userId);
        Task<Shift> StartShiftAsync(int userId);
    }
}
