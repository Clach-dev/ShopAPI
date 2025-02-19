using Application.Common.Dtos.User;
using Application.Common.Utils;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.UserCases.Queries.GetAllUsersCase;

public class GetAllUsersHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<ReadUserDto>>>
{
    public async Task<Result<IEnumerable<ReadUserDto>>> Handle(
        GetAllUsersQuery getAllUsersQuery,
        CancellationToken cancellationToken)
    {
        var users = await unitOfWork.Users.GetAllAsync(mapper.Map<PageInfo>(getAllUsersQuery.PageInfoDto), cancellationToken);

        var usersReadDto = mapper.Map<IEnumerable<ReadUserDto>>(users);

        return ResultBuilder.SuccessResult(usersReadDto);
    }
}