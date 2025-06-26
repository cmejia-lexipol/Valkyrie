using MediatR;
using Valkyrie.Application.Common.DTOs;

namespace Valkyrie.Application.Features.Fields.Queries.GetFieldById;

public record GetFieldByIdQuery : IRequest<FieldDto?>
{
    public int Id { get; init; }
}