# Task Management — CQRS + MediatR Practice Project

A learning project for practicing the **CQRS pattern** with **MediatR**, **Entity Framework Core (Database First)**, and **SQL Server**, exposed through an **ASP.NET Core Web API**.

The goal of this project was not to ship a product, but to **understand and test** how CQRS and MediatR work in a real Web API against a real database.

---

## Tech stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 10) |
| Pattern | CQRS (Command Query Responsibility Segregation) |
| Mediator | MediatR (request/handler routing + pipeline behaviors) |
| Data access | Entity Framework Core — **Database First** (scaffolded) |
| Database | SQL Server (`TaskManagementDb`) |
| API docs / testing | Swagger UI (Swashbuckle) |

---

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

> The EF Core entity and `DbContext` were **generated from the existing SQL Server database** using `Scaffold-DbContext` (Database First), then kept in a separate `.Db` project for a clean separation between the data layer and the API.

---

## What CQRS looks like here

- **Command** = changes data (write). Example: `CreateTaskCommand`, `UpdateTaskCommand`, `DeleteTaskCommand`.
- **Query** = reads data, never changes it. Example: `GetAllTasksQuery`, `GetTaskByIdQuery`.
- Each command/query is a small **message**; each has **exactly one handler** that does the work.
- The **controller injects only `IMediator`** and just calls `_mediator.Send(...)` — it has no `DbContext`, repository, or service. All logic lives in the handlers.

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

---

## API endpoints (all built and tested in Swagger)

| Method | Route | CQRS type | Handler |
|---|---|---|---|
| GET | `/api/tasks` | Query | `GetAllTasksHandler` |
| GET | `/api/tasks/{id}` | Query | `GetTaskByIdHandler` |
| POST | `/api/tasks` | Command | `CreateTaskHandler` |
| PATCH | `/api/tasks` | Command | `UpdateTaskHandler` |
| DELETE | `/api/tasks` | Command | `DeleteTaskHandler` |

---

## What I accomplished and tested successfully

- ✅ Set up a Web API with **MediatR** registered via `RegisterServicesFromAssembly` (handlers auto-discovered).
- ✅ Connected to **SQL Server** with EF Core using the **Database First** approach (`Scaffold-DbContext`).
- ✅ Moved the connection string to `appsettings.json` and registered `AppDbContext` through DI in `Program.cs`.
- ✅ Built the full **CRUD** set as CQRS commands and queries, each with its own handler.
- ✅ Kept the controller **thin** — it only sends MediatR messages (true CQRS).
- ✅ Used **async/await** with `CancellationToken` passed into every EF Core call (`ToListAsync(ct)`, `FirstOrDefaultAsync(..., ct)`, `SaveChangesAsync(ct)`).
- ✅ Handled the **not-found** case for `GetById` (returns `404 NotFound` instead of an empty `200`).
- ✅ Implemented a **partial update** with a DTO (only non-null fields are updated).
- ✅ Added **Swagger UI** and configured the project to launch straight to it.
- ✅ **Tested all five endpoints end-to-end in Swagger** against the live SQL Server database (create, read all, read by id, update, delete).

### Concepts I learned along the way

- The difference between `Send` (one handler, returns a value) and `Publish` (many handlers, no return).
- How MediatR routes by **request type**, not by data.
- That a **handler is just a service** that MediatR auto-registers and calls for me.
- **Pipeline behaviors** (middleware that wraps every request) — practiced with a logging behavior.
- Why `CancellationToken` matters: it lets a request abort early if the caller goes away, and is **cooperative** (only works if I pass it into the async calls).

---

## How to run

1. Make sure SQL Server is running and the `TaskManagementDb` database exists (with the `Tasks` table).
2. Confirm the connection string in `TaskManagement_CQRS_Pattern.Api/appsettings.json` points to your server and `TaskManagementDb`.
3. Set `TaskManagement_CQRS_Pattern.Api` as the startup project.
4. Press **F5** (or `dotnet run` in the Api project) — the browser opens Swagger automatically.
5. Try the endpoints from the Swagger UI.

---

## Next steps (planned, not yet implemented)

- [ ] **FluentValidation** + a `ValidationBehavior` pipeline to reject invalid commands with a clean `400` before the handler runs.
- [ ] Return **DTOs** from queries instead of raw EF entities.
- [ ] A global **exception handler** (`IExceptionHandler`) for consistent error responses.
- [ ] An **async uniqueness rule** (e.g. task title must be unique) using `MustAsync`.

---

*Built as a hands-on learning exercise to understand CQRS + MediatR in ASP.NET Core.*
