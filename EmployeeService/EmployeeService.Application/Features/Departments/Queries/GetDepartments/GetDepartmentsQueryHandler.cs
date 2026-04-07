using AutoMapper;
using EmployeeService.Application.DTOs.Departments;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, IReadOnlyList<DepartmentResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDepartmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DepartmentResponseDto>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _unitOfWork.Departments.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DepartmentResponseDto>>(list);
    }
}
