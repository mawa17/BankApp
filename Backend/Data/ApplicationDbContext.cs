using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUserEx>(options)
{
    public DbSet<AccountModel> Accounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure AccountModel -> IdentityUserEx to be auto-included
        builder.Entity<AccountModel>()
               .Navigation(a => a.User)
               .AutoInclude();
    }
}