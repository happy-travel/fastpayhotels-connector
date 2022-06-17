using HappyTravel.BaseConnector.Api.Infrastructure.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;
using HappyTravel.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureInfrastructure(options =>
{
    options.ConsulKey = Connector.Name;
});
builder.ConfigureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureBaseConnector(builder.Configuration);
app.Run();