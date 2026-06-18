using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement_CQRS_Pattern.Db.AppDbContextModels;

namespace TaskManagement_CQRS_Pattern.Api.Features.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery (int id) : IRequest<TaskItem?>;

public class GetTaskByIdHandler: IRequestHandler<GetTaskByIdQuery, TaskItem?>
{
    private readonly AppDbContext _appDbContext;

    public GetTaskByIdHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<TaskItem?> Handle(GetTaskByIdQuery request, CancellationToken ct)
    {
         return await _appDbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == request.id, ct);
    }
}