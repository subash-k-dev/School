using Microsoft.EntityFrameworkCore;
using School.Application.Interfaces.Auth;
using School.Domain.Entities;
using School.Domain.Security;
using School.Infrastructure.Persistence;

namespace School.Api.Seed;

public static class DbSeeder
{
    private static bool LooksLikeHashed(string value)
        => !string.IsNullOrWhiteSpace(value) && value.StartsWith("AQAAAA"); // Identity format

    public static async Task SeedAsync(AppDbContext db, IPasswordHasherService hasher)
    {
        await db.Database.MigrateAsync();

        // Admin user
        var admin = await db.Users.FirstOrDefaultAsync(x => x.UserName == "admin");
        if (admin is null)
        {
            db.Users.Add(new User
            {
                UserName = "admin",
                PasswordHash = hasher.Hash("admin123"),
                Role = Roles.Admin
            });
        }
        else if (!LooksLikeHashed(admin.PasswordHash))
        {
            // Upgrade old plain password to hash
            admin.PasswordHash = hasher.Hash(admin.PasswordHash);
            db.Users.Update(admin);
        }

        // Teacher user
        var teacher = await db.Users.FirstOrDefaultAsync(x => x.UserName == "teacher");
        if (teacher is null)
        {
            db.Users.Add(new User
            {
                UserName = "teacher",
                PasswordHash = hasher.Hash("teacher123"),
                Role = Roles.Teacher
            });
        }
        else if (!LooksLikeHashed(teacher.PasswordHash))
        {
            teacher.PasswordHash = hasher.Hash(teacher.PasswordHash);
            db.Users.Update(teacher);
        }

        await db.SaveChangesAsync();
    }
}