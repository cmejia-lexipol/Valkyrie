using MediatR;
using FieldBank.Application.Common.DTOs;

namespace FieldBank.Application.Features.Fields.Queries.GetFieldById;

public record GetFieldByIdQuery : IRequest<FieldDto?>
{
    public int Id { get; init; }
} 