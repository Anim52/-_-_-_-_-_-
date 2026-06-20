using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MaxiMed.Api.Filters
{
    public sealed class ActionAuditFilter : IAsyncActionFilter
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        private readonly ILogger<ActionAuditFilter> _logger;

        public ActionAuditFilter(
            IDbContextFactory<MaxiMedDbContext> dbFactory,
            ILogger<ActionAuditFilter> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executed = await next();

            try
            {
                var http = context.HttpContext;
                var routeValues = http.Request.RouteValues;

                var controller = routeValues.TryGetValue("controller", out var c)
                    ? c?.ToString() ?? "Unknown"
                    : "Unknown";

                var action = routeValues.TryGetValue("action", out var a)
                    ? a?.ToString() ?? http.Request.Method
                    : http.Request.Method;

                var statusCode = http.Response.StatusCode;

                int? userId = null;
                if (http.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) &&
                    int.TryParse(userIdHeader.FirstOrDefault(), out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var details = new
                {
                    Method = http.Request.Method,
                    Path = http.Request.Path.Value,
                    Query = http.Request.QueryString.Value,
                    StatusCode = statusCode,
                    Controller = controller,
                    Action = action,
                    HasException = executed.Exception is not null,
                    Exception = executed.Exception?.GetType().Name
                };

                await using var db = await _dbFactory.CreateDbContextAsync(http.RequestAborted);

                db.AuditLogs.Add(new AuditLog
                {
                    UserId = userId,
                    Action = $"API:{http.Request.Method}",
                    Entity = controller,
                    EntityId = action,
                    DetailsJson = JsonSerializer.Serialize(details)
                });

                await db.SaveChangesAsync(http.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Не удалось записать действие пользователя в журнал аудита.");
            }
        }
    }
}
