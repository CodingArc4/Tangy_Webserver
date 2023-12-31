﻿namespace TangyWeb_Server.Classes;

public interface IBlazorUserEmailStore<TUser> : IBlazorUserStore<TUser> where TUser : class
{
    Task SetEmailAsync(TUser user, string? email, CancellationToken cancellationToken);
}
