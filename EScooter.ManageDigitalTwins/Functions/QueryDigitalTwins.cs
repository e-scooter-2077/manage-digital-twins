using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EasyDesk.Tools;
using EScooter.ManageDigitalTwins.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace EScooter.ManageDigitalTwins.Functions;

public class QueryDigitalTwins
{
    private readonly IDigitalTwinsService _digitalTwinsService;
    private readonly ILogger<QueryDigitalTwins> _logger;

    public QueryDigitalTwins(IDigitalTwinsService digitalTwinsService, ILogger<QueryDigitalTwins> logger)
    {
        _digitalTwinsService = digitalTwinsService;
        _logger = logger;
    }

    [FunctionName("customers")]
    public async Task<IActionResult> GetCustomers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        return await HandleRequest(_digitalTwinsService.GetCustomers);
    }

    [FunctionName("scooters")]
    public async Task<IActionResult> GetScooters(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        return await HandleRequest(_digitalTwinsService.GetScooters);
    }

    private async Task<IActionResult> HandleRequest<T>(AsyncFunc<IEnumerable<T>> result)
    {
        try
        {
            return new OkObjectResult(await result());
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {message}", ex.Message);
            return new ObjectResult(new { Error = ex.Message })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
