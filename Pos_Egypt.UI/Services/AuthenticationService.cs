using HeCashier.Data;
using HeCashier.Security;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Services;

public sealed record AuthenticatedUser(int Id, string UserName);

public sealed class AuthenticationService(IDbContextFactory<HeCashierDbContext> factory)
{
    public async Task<AuthenticatedUser?> AuthenticateAsync(string userName, string password)
    {
        await using var db = await factory.CreateDbContextAsync();
        var user = await db.Users.SingleOrDefaultAsync(x => x.UserName == userName && x.IsActive);
        return user is not null && PasswordHasher.Verify(password, user.PasswordHash)
            ? new AuthenticatedUser(user.Id, user.UserName) : null;
    }
}
