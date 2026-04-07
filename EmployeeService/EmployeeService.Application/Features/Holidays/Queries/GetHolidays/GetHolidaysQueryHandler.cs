using AutoMapper;
using EmployeeService.Application.DTOs.Holidays;
using EmployeeService.Application.Interfaces.Persistence;
using MediatR;

namespace EmployeeService.Application.Features.Holidays.Queries.GetHolidays;

public class GetHolidaysQueryHandler : IRequestHandler<GetHolidaysQuery, IReadOnlyList<HolidayResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetHolidaysQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<HolidayResponseDto>> Handle(
        GetHolidaysQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _unitOfWork.Holidays.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<HolidayResponseDto>>(list);
    }
}
