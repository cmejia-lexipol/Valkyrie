using MediatR;
using Valkyrie.Domain.Interfaces;

namespace Valkyrie.Application.Features.Fields.Commands.DeleteField;

public class DeleteFieldCommandHandler : IRequestHandler<DeleteFieldCommand, Unit>
{
    private readonly IFieldRepository _fieldRepository;

    public DeleteFieldCommandHandler(IFieldRepository fieldRepository)
    {
        _fieldRepository = fieldRepository;
    }

    public async Task<Unit> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("Field ID must be greater than 0", nameof(request.Id));

        var existingField = await _fieldRepository.GetByIdAsync(request.Id);
        if (existingField == null)
            throw new InvalidOperationException($"Field with ID {request.Id} not found");

        await _fieldRepository.DeleteAsync(request.Id);

        return Unit.Value;
    }
}