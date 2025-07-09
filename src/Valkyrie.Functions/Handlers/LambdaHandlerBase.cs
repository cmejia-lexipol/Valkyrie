using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Valkyrie.Functions.Handlers;

public abstract class LambdaHandlerBase
{
    protected readonly IMediator _mediator;

    protected LambdaHandlerBase(IMediator? mediator = null)
    {
        if (mediator != null)
        {
            _mediator = mediator;
        }
        else
        {
            // Build the Generic Host using FunctionsStartup
            var host = FunctionsStartup.BuildHost();
            _mediator = host.Services.GetRequiredService<IMediator>();
        }
    }
}