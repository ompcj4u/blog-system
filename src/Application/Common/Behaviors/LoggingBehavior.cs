using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) :
    IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();
        var requestName = typeof(TRequest).Name;
        stopWatch.Start();
        logger.LogInformation("Processing {RequestName} at {Time}", requestName, DateTimeOffset.UtcNow);
        try
        {
            var response = await next();
            stopWatch.Stop();
            logger.LogInformation("Completed {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, stopWatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing {RequestName}", requestName);
            // NOTE: We should not post this error to client => Security Mismatch
            var failure = new ValidationFailure(requestName, ex.Message);
            var list = new List<ValidationFailure>() { failure};
            throw new ValidationException(list);
        }
    }

}
