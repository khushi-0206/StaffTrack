using AutoMapper;
using EmployeeService.Application.Common.Exceptions;
using EmployeeService.Application.DTOs.Employees;
using EmployeeService.Application.Interfaces;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Employees.Queries.GetEmployeeSelf;

public class GetEmployeeSelfQueryHandler : IRequestHandler<GetEmployeeSelfQuery, EmployeeResponseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetEmployeeSelfQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<EmployeeResponseDto?> Handle(GetEmployeeSelfQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAppException();

        var email = _currentUser.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(email))
            throw new UnauthorizedAppException("Email claim is required to resolve employee profile.");

        var employee = await _unitOfWork.Employees.GetByEmailAsync(email, cancellationToken);
        return employee is null ? null : _mapper.Map<EmployeeResponseDto>(employee);
    }
}
