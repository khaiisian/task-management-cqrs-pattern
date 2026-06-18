using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement_CQRS_Pattern.Db.AppDbContextModels;

namespace TaskManagement_CQRS_Pattern.Api.Features.Tasks.Queries.GetAllTasks;

public record GetAllTasksQuery : IRequest<List<TaskItem>>;

public class GetAllTasksHandler: IRequestHandler<GetAllTasksQuery, List<TaskItem>>
{
    private readonly AppDbContext _appDbContext;

    public GetAllTasksHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<TaskItem>> Handle (GetAllTasksQuery query, CancellationToken ct)
    {
        return await _appDbContext.TaskItems.ToListAsync(ct);
    }
}
