using EmployeeService.Application.Interfaces.Persistence;
using EmployeeService.Domain.Entities;
using MediatR;

namespace EmployeeService.Application.Features.Holidays.Commands.CreateHoliday;

public class CreateHolidayCommandHandler : IRequestHandler<CreateHolidayCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHolidayCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
        var entity = new Holiday
        {
            Name = request.Dto.Name.Trim(),
            Date = request.Dto.Date
        };

        _unitOfWork.Holidays.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
