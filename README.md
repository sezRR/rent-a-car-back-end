<br>
<br>
<br>
 
<p align="center">
 <img width="100px" src="https://www.seekpng.com/png/full/419-4199738_final-product-image-isometric-car-illustration-png.png" align="center" alt="GitHub Readme Stats" />
 <h2 align="center">RENTAR (RENT CAR) - BACKEND</h2>

 <p align="center">Rentar 'Backend' project with C# - ASP.NET Core</p>
 <p align="center">Click for <a href="https://github.com/sezRR/rent-a-car-front-end">The Frontend</a></p>
</p>



<br>
<br>
<br>
<br>
<br>



# RENTAR BACKEND

<br>

**Getting Started**

Requirements: .NET 10 SDK, Docker.

```bash
# 1. start PostgreSQL (defaults: rentar/rentar/rentar on port 5432, override via .env)
docker compose up -d

# 2. run the API - in Development it applies every module's pending migrations on startup
dotnet run --project src/Bootstrapper/Rentar.Api
```

Connection string lives in `src/Bootstrapper/Rentar.Api/appsettings.json` under `ConnectionStrings:RentarDb`, and can be
overridden with the `ConnectionStrings__RentarDb` environment variable. When neither is present the
modules fall back to the docker-compose defaults, which is also what `dotnet ef` uses.

Migrating on startup is controlled by `Database:MigrateOnStartup` (on in `appsettings.Development.json`, off otherwise).
To migrate by hand instead:

```bash
dotnet tool restore
for module in Identity Fleet Customers Rentals; do
  project=src/Modules/$module/Rentar.Modules.$module.Infrastructure
  dotnet ef database update --project $project --startup-project $project
done
```

Adding a migration after changing a module's entities (e.g. Fleet):

```bash
project=src/Modules/Fleet/Rentar.Modules.Fleet.Infrastructure
dotnet ef migrations add <Name> --project $project --startup-project $project --output-dir Persistence/Migrations
```

> Upgrading a database created before the modular monolith: its tables live in `public` and are not moved.
> Recreate it (`docker compose down -v && docker compose up -d`) so each module creates its own schema.

**Tests**

- `src/Tests/Rentar.Api.IntegrationTests` drives the API in memory (`WebApplicationFactory`) against a throwaway
  PostgreSQL container started by Testcontainers, so Docker has to be running. Every module's schema is migrated once;
  each test then truncates all module tables and clears the aspect cache.
- `src/Tests/Rentar.ArchitectureTests` enforces the module boundaries and schema ownership described below. No Docker needed.

```bash
dotnet test --project src/Tests/Rentar.ArchitectureTests
dotnet test --project src/Tests/Rentar.Api.IntegrationTests
```

`global.json` opts the repository into the Microsoft.Testing.Platform runner that .NET 10 requires.
Package versions are managed centrally in `Directory.Packages.props`.

Code style lives in `.editorconfig`: file-scoped namespaces, no unused usings, and primary constructors. Running
`dotnet format RentarBackend.sln` applies all of them; for example, it turns a constructor that only stores its
parameters into a primary constructor.

<br>

**API versions**

The version is part of the URL (`/api/v1/...`, `/api/v2/...`). In Development, Scalar at `/docs` has both
versions in a dropdown, and their OpenAPI documents are at `/openapi/v1.json` and `/openapi/v2.json`.
Bruno requests for both versions are in `api-docs/`.

- **v1** is the original API and is unchanged. It uses action-style routes (`/brands/getall`, `/brands/add`,
  `POST /brands/delete`), answers 200 or 400, and wraps every body in `{ success, message, data }`.
- **v2** is RESTful:
  - Routes name resources, and the HTTP method says what to do with them.
  - Ids are in the path and filters are in the query string.
  - Bodies are the resources themselves, with no envelope.
  - Errors are [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457) problem details (`application/problem+json`);
    validation errors are listed under `errors`.
  - `POST` answers `201 Created` with a `Location` header. `PUT` and `DELETE` answer `204 No Content`.
  - An unknown id answers `404`. The 22:00 maintenance hour answers `503`.
  - Users never include the password hash or salt.

| v2 route | v1 equivalent |
|----------|---------------|
| `POST /auth/tokens` (log in, `401` on bad credentials) | `POST /auth/login`, `POST /auth/checkpassword` |
| `GET/POST /users`, `GET/PUT /users/{id}`<br>filters: `?email=`, `?minFindeksRating=` | `users/getall`, `getbymail`, `getuserfindeksrating`, `getuserbyid`, `add`, `update`;<br>`POST /users` (`409` if the email is taken) also replaces `auth/register` |
| `GET/POST /brands`, `GET/PUT/DELETE /brands/{id}` | `brands/*` |
| `GET/POST /colors`, `GET/PUT/DELETE /colors/{id}` | `colors/*` |
| `GET/POST /cars`, `GET/PUT/DELETE /cars/{id}`<br>filters: `?brandId=`, `?colorId=` | `cars/getall`, `getcardetails`, `getcardetailsbyid`, `getcarsby*`, `add`, `addreturnabledata`, `update`, `delete` |
| `GET/POST/DELETE /cars/{carId}/images`, `GET/PUT/DELETE /cars/{carId}/images/{imageId}`, `GET /car-images` | `carimages/*` (form field `file`) |
| `GET/POST /customers`, `GET /customers/{id}`<br>filter: `?userId=` | `customers/*` |
| `GET/POST /rentals`, `GET /rentals/{id}`, `GET /rentals/details`, `GET /rentals/availability?carId=&rentDate=&returnDate=`<br>filter: `?carId=` | `rentals/*`, `isrentable` |
| `POST /payments` (`204`, or `402` when declined) | `GET /payments/pay` |
| `GET /findeks/rating` | `GET /findeks/calculate` |

Registering in v2 takes two calls: `POST /users`, then `POST /auth/tokens`.

<br>

**Architecture: modular monolith**

One deployable host, split into modules that each own their data and talk to each other only through contracts.

```
src/
  Bootstrapper/Rentar.Api           host: composes the modules, nothing else
  Shared/
    Rentar.Shared.Kernel            exceptions, entity markers (no dependencies)
    Rentar.Shared.Application       aspects (validation, caching, transaction, performance, security) + abstractions
    Rentar.Shared.Infrastructure    EF repository base, per-module DbContext wiring, cache, IModule
    Rentar.Shared.Presentation      endpoint discovery, versioned route groups, exception handler, v1 envelope
  Modules/<Module>/
    Rentar.Modules.<Module>.Contracts       public API other modules may call
    Rentar.Modules.<Module>.Domain          entities
    Rentar.Modules.<Module>.Application     services, validators, repository interfaces, contract implementation
    Rentar.Modules.<Module>.Presentation    minimal-API endpoints
    Rentar.Modules.<Module>.Infrastructure  DbContext, migrations, repositories, adapters, module registration
  Tests/
```

| Module    | Owns                                   | Schema      | Uses contracts of            |
|-----------|----------------------------------------|-------------|------------------------------|
| Identity  | users, operation claims, auth (JWT)    | `identity`  | -                            |
| Fleet     | brands, colors, cars, car images       | `fleet`     | -                            |
| Customers | customer accounts                      | `customers` | -                            |
| Rentals   | rentals                                | `rentals`   | Customers, Identity, Fleet   |
| Payments  | payments (stateless)                   | -           | -                            |
| Findeks   | credit ratings (stateless)             | -           | -                            |

Every module with state has its own `DbContext`, its own schema and its own `__EFMigrationsHistory` table inside that
schema, so modules migrate independently. Data in another module is referenced by id only: there are no cross-schema
foreign keys or joins. For example, rental details are put together by `RentalManager` from the Customers, Identity and
Fleet contracts.

Boundary rules (checked by `Rentar.ArchitectureTests` on the compiled assemblies):

- Domain uses only the Kernel. Contracts use nothing but the Kernel.
- Application uses its own Domain and Contracts, `Shared.Application`, and the **Contracts** of other modules - nothing else of theirs.
- Presentation uses its own Application/Domain and `Shared.Presentation`, never Infrastructure.
- Infrastructure is the module's composition root (`IModule`); it is the only module project the host references.
- Shared projects never reference a module.

Adding a module: create the five projects, implement `IModule` in its Infrastructure (register its context with
`AddModuleDbContext<TContext>("<schema>")`), and add it to the `modules` array in `Program.cs`.

<br>

**Cross-cutting**

- Errors: services return plain values and throw the Kernel exceptions (`BusinessRuleException`, `NotFoundException`,
  `ConflictException`, `InvalidCredentialsException`, `UnavailableException`) when they refuse a request.
  `ApiExceptionHandler`, an ASP.NET Core `IExceptionHandler`, turns them into problem details for v2 and into the
  `{ success, message, data }` body for v1. `V1ContractTests` pins every v1 response.
- Aspects via Autofac interceptors: validation (FluentValidation), caching, transaction scope, performance, secured operation
- Security: HMACSHA512 password hashing, JWT access tokens (Identity module)
- Uploaded car images are stored under `src/Bootstrapper/Rentar.Api/wwwroot/uploads` (`Paths:DefaultCarImagePath`)

**Dependency Resolver**: _Autofac_

**Validation**: _FluentValidation_

**Database Operations & Data Access Operations**: _EntityFrameworkCore (PostgreSQL / Npgsql)_
