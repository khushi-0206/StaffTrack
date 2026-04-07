using AutoMapper;
using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeesByManager;

public class GetEmployeesByManagerQueryHandler
    : IRequestHandler<GetEmployeesByManagerQuery, IReadOnlyList<EmployeeListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEmployeesByManagerQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeListItemDto>> Handle(
        GetEmployeesByManagerQuery request,
        CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Employees.GetByIdAsync(request.ManagerId, cancellationToken: cancellationToken) is null)
            throw new NotFoundException("Manager not found.");

        var list = await _unitOfWork.Employees.GetByManagerAsync(request.ManagerId, cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeListItemDto>>(list);
    }
}
