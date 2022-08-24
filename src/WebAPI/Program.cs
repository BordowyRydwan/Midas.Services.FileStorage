new WebAPI.Startup(args)
    .AddLoggerConfig()
    .SetOpenCors()
    .SetDbContext()
    .SetBuilderOptions()
    .SetMapperConfig()
    .AddInternalServices()
    .AddInternalRepositories()
    .Run();