using Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviors;
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validator)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        if (!validator.Any())
            return await next();
        
        var context = new ValidationContext<TRequest>(request);
        var validationResult = await Task.WhenAll(validator.Select(t => t.ValidateAsync(context, cancellationToken)));

        var errors = validationResult.SelectMany(t => t.Errors).Where(t => t != null).ToList();

        if (errors.Any())
            throw new ValidationException(errors);

        return await next();
    }
}
