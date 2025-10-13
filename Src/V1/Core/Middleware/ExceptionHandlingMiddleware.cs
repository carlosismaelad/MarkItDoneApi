using MarkItDoneApi.V1.Core.DomainExceptions;
using MarkItDoneApi.Src.V1.Core.DomainExceptions;

namespace MarkItDoneApi.V1.Core.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(ex.ToJson());
        }
        catch (ServiceException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(ex.ToJson());
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(ex.ToJson());
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { 
                name = "UnauthorizedAccessException",
                message = ex.Message,
                action = !string.IsNullOrEmpty(ex.Message) ? ex.Message : "Faça login para acessar este recurso.",
                status_code = 401
            });
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { 
                name = "InternalServerError",
                message = "Erro interno do servidor.",
                action = "Tente novamente mais tarde ou entre em contato com o suporte.",
                status_code = 500
            });
        }
    }
}