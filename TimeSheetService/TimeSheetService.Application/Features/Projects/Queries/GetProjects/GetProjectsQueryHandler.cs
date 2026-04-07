using AutoMapper;
using TimeSheetService.Application.DTOs.Projects;
using TimeSheetService.Application.Interfaces.Persistence;
using MediatR;

namespace TimeSheetService.Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectResponseDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProjectsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProjectResponseDto>> Handle(
        GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _uow.Projects.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ProjectResponseDto>>(list.Where(x => !x.IsDeleted).ToList());
    }
}
