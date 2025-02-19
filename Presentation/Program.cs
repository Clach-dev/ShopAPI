using Presentation.Common.Extensions;

ProgramExtension
    .CreateBuilder(args)
    .ConfigureServices()
    .Build()
    .ConfigureMiddleware()
    .Run();