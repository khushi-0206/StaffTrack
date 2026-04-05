using AutoMapper;
using EmployeeService.Application.Common.Models;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEmployeesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<EmployeeListItemDto>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var result = await _unitOfWork.Employees.SearchAsync(
            page,
            pageSize,
            request.Search,
            request.DepartmentId,
            request.RoleId,
            request.Status,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        var items = _mapper.Map<IReadOnlyList<EmployeeListItemDto>>(result.Items);
        return new PagedResult<EmployeeListItemDto>(items, page, pageSize, result.TotalCount);
    }
}
