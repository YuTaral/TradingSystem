
using SharedData.Models;
using System.Net;
using System.Text.Json;
using static SharedData.Constants;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient(); 

var app = builder.Build();

app.UseRouting();

app.Map("/{service}/{*rest}", async (HttpContext context, string service, string rest, IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient();
    var response = new HttpResponseMessage();
    var responseContent = "";

    // Set the target
    var targetUrl = "";

    if (service == ORDER_CONTROLLER_ROUTE)
    {
        targetUrl = $"http://orderservice:8080/{rest}";
    }
    else if (service == PORTFOLIO_CONTROLLER_ROUTE)
    {
        targetUrl = $"http://portfolioservice:8080/{rest}";
    }

    // Check if it's valid
    if (string.IsNullOrEmpty(targetUrl))
    {
        var result = new ServiceActionResult<string>(HttpStatusCode.BadRequest, WRONG_URI, []);

        context.Response.StatusCode = result.Code;
        responseContent = JsonSerializer.Serialize(result);
    } 
    else
    {
        // Check if it's a post and set the content if so
        var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);

        if (context.Request.ContentLength > 0)
        {
            var content = new StreamContent(context.Request.Body);
            content.Headers.Add("Content-Type", context.Request.ContentType);
            requestMessage.Content = content;
        }

        // Execute the request
        try
        {
            response = await client.SendAsync(requestMessage);
            responseContent = await response.Content.ReadAsStringAsync();
            context.Response.StatusCode = (int)response.StatusCode;
        }
        catch (Exception)
        {
            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            responseContent = UNEXPECTED_ERROR_OCCURRED;
        }
    }

    context.Response.ContentType = "application/json";  
    await context.Response.WriteAsync(responseContent);
});

app.Run();
