using Application.Common.Utils;
using MediatR;

namespace Application.UseCases.OrderCases.Commands.DeleteOrderCase;

public record DeleteOrderCommand(
    Guid Id)
    : IRequest<Result<Unit>>;