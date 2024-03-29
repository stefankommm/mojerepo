﻿using Microsoft.AspNetCore.Identity;
using Signee.Domain.Exceptions;
using Signee.Domain.Identity;
using Signee.Domain.RepositoryContracts.Areas.User;

namespace Signee.Infrastructure.PostgreSql.Areas.User;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        await GetByIdAsync(user.Id); // Check if user exists and if not throw exception
        await _userManager.UpdateAsync(user);
    }
        

    public async Task<ApplicationUser> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        
        if (user == null)
            throw new EntityNotExistException(typeof(ApplicationUser).ToString());

        return user;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
        => await _userManager.FindByEmailAsync(email);

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        => await _userManager.CreateAsync(user, password);

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        => await _userManager.CheckPasswordAsync(user, password);
}