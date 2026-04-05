using AutoMapper;
using EmployeeService.Application.DTOs.Roles;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Roles.Queries.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IReadOnlyList<RoleResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRolesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RoleResponseDto>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _unitOfWork.Roles.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<RoleResponseDto>>(list);
    }
}
