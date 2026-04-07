using AutoMapper;
using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeesByDepartment;

public class GetEmployeesByDepartmentQueryHandler
    : IRequestHandler<GetEmployeesByDepartmentQuery, IReadOnlyList<EmployeeListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEmployeesByDepartmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EmployeeListItemDto>> Handle(
        GetEmployeesByDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId, cancellationToken) is null)
            throw new NotFoundException("Department not found.");

        var list = await _unitOfWork.Employees.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeListItemDto>>(list);
    }
}
