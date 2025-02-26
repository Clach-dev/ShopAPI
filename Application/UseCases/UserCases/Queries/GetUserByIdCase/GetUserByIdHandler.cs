using Application.Common.Dtos.User;
using Application.Common.Utils;
using AutoMapper;
using Domain.Interfaces.IRepositories;
using MediatR;

namespace Application.UseCases.UserCases.Queries.GetUserByIdCase;

public class GetUserByIdHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetUserByIdQuery, Result<ReadUserDto>>
{
    public async Task<Result<ReadUserDto>> Handle(
        GetUserByIdQuery getUserByIdQuery,
        CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByIdAsync(getUserByIdQuery.Id, cancellationToken);
        if (user is null)
        {
            ResultBuilder.NotFoundResult<ReadUserDto>(ErrorMessages.UserIdNotFoundError);
        }
        
        var userReadDto = mapper.Map<ReadUserDto>(user);

        return ResultBuilder.SuccessResult(userReadDto);
    }
}