using AutoMapper;
using EmployeeService.Application.DTOs.LeaveTypes;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.LeaveTypes.Queries.GetLeaveTypes;

public class GetLeaveTypesQueryHandler : IRequestHandler<GetLeaveTypesQuery, IReadOnlyList<LeaveTypeResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLeaveTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LeaveTypeResponseDto>> Handle(
        GetLeaveTypesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _unitOfWork.LeaveTypes.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<LeaveTypeResponseDto>>(list);
    }
}
