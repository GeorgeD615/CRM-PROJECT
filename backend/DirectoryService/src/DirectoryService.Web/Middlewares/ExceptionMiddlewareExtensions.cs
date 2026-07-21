namespace DirectoryService.Web.Middlewares;

/// <summary>
/// Регистрация <see cref="ExceptionMiddleware"/> в pipeline приложения.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionMiddleware>();
}
