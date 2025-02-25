using Application.Common.Dtos.OrderItem;
using Application.Common.Dtos.User;
using Application.Common.Utils;
using Application.UseCases.UserCases.Queries.GetAllUsersCase;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.OrderItemCases.Queries.GetAllOrderItemsQuery;

public class GetAllOrderItemsHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAllOrderItemsQuery, Result<ReadOrderItemsDto>>
{
    public async Task<Result<ReadOrderItemsDto>> Handle(
        GetAllOrderItemsQuery getAllOrderItemsQuery,
        CancellationToken cancellationToken)
    {
        var orderItems = await unitOfWork.OrderItems.GetAllAsync(
            mapper.Map<PageInfo>(getAllOrderItemsQuery.PageInfoDto),
            cancellationToken);

        var orderItemsReadDto = new ReadOrderItemsDto(mapper.Map<IEnumerable<ReadOrderItemDto>>(orderItems.Item1), orderItems.Item2);
        
        return ResultBuilder.SuccessResult(orderItemsReadDto);
    }
}