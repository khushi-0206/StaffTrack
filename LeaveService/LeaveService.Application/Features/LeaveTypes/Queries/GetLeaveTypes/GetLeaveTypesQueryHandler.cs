using AutoMapper;
using LeaveService.Application.DTOs.LeaveTypes;
using LeaveService.Application.Interfaces.Persistence;
using MediatR;

namespace LeaveService.Application.Features.LeaveTypes.Queries.GetLeaveTypes;

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
