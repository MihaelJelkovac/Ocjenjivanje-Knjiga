using Microsoft.AspNetCore.Authorization;

namespace Lab5.Authorization;

/// <summary>
/// Brzi authorizacijski atributi za česte kombinacije rola
/// </summary>
public class AuthorizeAdminManagerAttribute : AuthorizeAttribute
{
    public AuthorizeAdminManagerAttribute() => Roles = "Admin,Manager";
}

public class AuthorizeAdminAttribute : AuthorizeAttribute
{
    public AuthorizeAdminAttribute() => Roles = "Admin";
}
