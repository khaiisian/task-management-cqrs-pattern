# Task Management — CQRS + MediatR

A practice project for learning the **CQRS pattern** with **MediatR**, **EF Core (Database First)**, and **SQL Server** in an ASP.NET Core Web API.

## Stack

- ASP.NET Core Web API (.NET 10)
- MediatR (CQRS)
- EF Core — Database First (scaffolded from SQL Server)
- SQL Server (`TaskManagementDb`)
- Swagger UI

## Solution structure

```
TaskManagement_CQRS_Pattern/
├── TaskManagement_CQRS_Pattern.Api/        # Web API + CQRS handlers
│   ├── Controllers/
│   │   └── TasksController.cs              # injects ONLY IMediator, forwards messages
│   ├── Features/
│   │   └── Tasks/
│   │       ├── Commands/                   # writes (create / update / delete)
│   │       │   ├── CreateTasks/
│   │       │   ├── UpdateTask/
│   │       │   └── DeleteTask/
│   │       └── Queries/                    # reads (get all / get by id)
│   │           ├── GetAllTasks/
│   │           └── GetTaskById/
│   ├── appsettings.json                    # DefaultConnection -> TaskManagementDb
│   └── Program.cs                          # DI: AddDbContext + AddMediatR
│
└── TaskManagement_CQRS_Pattern.Db/         # Scaffolded EF Core layer (Database First)
    └── AppDbContextModels/
        ├── AppDbContext.cs                 # generated from the database
        └── TaskItem.cs                     # entity generated from the Tasks table
```

The EF Core entity and `DbContext` were generated from the existing SQL Server database using `Scaffold-DbContext` (Database First), then kept in a separate `.Db` project for a clean separation between the data layer and the API.

## What CQRS looks like here

* Command = changes data (write). Example: `CreateTaskCommand`, `UpdateTaskCommand`, `DeleteTaskCommand`.
* Query = reads data, never changes it. Example: `GetAllTasksQuery`, `GetTaskByIdQuery`.
* Each command/query is a small message; each has exactly one handler that does the work.
* The controller injects only `IMediator` and just calls `_mediator.Send(...)` — it has no `DbContext`, repository, or service. All logic lives in the handlers.

Flow of a request:

```
HTTP request -> TasksController -> _mediator.Send(command/query)
                                        |
                                MediatR finds the matching handler (by type)
                                        |
                                Handler injects AppDbContext, does the work
                                        |
                                Result returned back to the controller
```

## Endpoints

| Method | Route | Type |
|---|---|---|
| GET | `/api/tasks` | Query |
| GET | `/api/tasks/{id}` | Query |
| POST | `/api/tasks` | Command |
| PATCH | `/api/tasks` | Command |
| DELETE | `/api/tasks` | Command |

## What I practiced

- Splitting reads/writes into **queries** and **commands**, each with one handler
- Keeping the controller thin (only `IMediator`, no DbContext)
- EF Core Database First scaffolding into a separate project
- Async + `CancellationToken` on all DB calls
- 404 handling for not-found, and a partial update with a DTO

## Run

1. Make sure SQL Server is running and `TaskManagementDb` exists.
2. Check the connection string in `Api/appsettings.json`.
3. Set the `.Api` project as startup and press F5 — Swagger opens automatically.

## TODO

- [ ] FluentValidation + validation behavior
- [ ] Return DTOs from queries
- [ ] Global exception handler
