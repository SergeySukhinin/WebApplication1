using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using WebApplication1.CustomConstraints;
using WebApplication1.Middleware;

//var builder = WebApplication.CreateBuilder(args); //without using 
var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    WebRootPath = "mywebroot"
});

builder.Services.AddRouting(options =>
{
    options.ConstraintMap.Add("client-types", typeof(CustomClientTypeConstraint));
});

builder.Services.AddTransient<MyCustomMiddleware>();    //very important to use AddTransient method in order to make the extension method
                                                        // working and being able to get executed
var app = builder.Build();
app.UseStaticFiles();   // works with the web root path (either with "wwwroot" or custom name as "mywebroot"-if explicitly described using WebApplicationOptions)

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "mywebroot")) //allows to add additional directories of static files other than one (usually "wwwroot" or custom name as "mywebroot"-if explicitly described using WebApplicationOptions)
});

app.UseRouting();
/*
app.Use(async (context, next) =>
{
    Endpoint endpoint = context.GetEndpoint();
    if (endpoint != null)
    {
        await context.Response.WriteAsync($"Endpoint: {endpoint.DisplayName}\n");
    }
    await next(context);
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("map1", async (context) =>
    {
        await context.Response.WriteAsync("In map 1");
    });
    endpoints.MapPost("map2", async (context) =>
    {
        await context.Response.WriteAsync("In map 2");
    });
});*/

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.Map("files/{filename}.{extension:length(3)=txt}", async context =>          // "=txt" - is the supplied default value with the constraint for length as length=3. Also minlength and maxlength can be used
    {
        string? fileName = Convert.ToString(context.Request.RouteValues["filename"]);
        string? extension = Convert.ToString(context.Request.RouteValues["extension"]);
        await context.Response.WriteAsync("In files: "+fileName+"."+extension);
    });
    endpoints.Map("employee/profile/{employeename:minlength(3):alpha?}", async context =>          // "?" - optional parameter, alpha - is a constraint for accepting only alphabetical values
    {
        
        if (context.Request.RouteValues.ContainsKey("employeename"))
        {
            string? employeeName = Convert.ToString(context.Request.RouteValues["employeename"]);
            await context.Response.WriteAsync($"In profiles. The employee's name: {employeeName}");
        }
        else
        {
            await context.Response.WriteAsync($"In profiles. The employee's name is not provided");
        }
    });
    endpoints.Map("sales-report/{year:int:min(1900)}/{month:regex(^(apr|jul|oct|jan)$)}", // used regex directly in the endpoint's definition (another way is to create a custom route constraint class => see next endpoint's definition)
        async context =>
    {
        int year = Convert.ToInt32(context.Request.RouteValues["year"]);
        string? month = Convert.ToString(context.Request.RouteValues["month"]);
        await context.Response.WriteAsync($"sales report - {year} - {month}");
        /*
        if (month == "apr" || month=="jul" || month=="oct" || month=="jan")
        {
            await context.Response.WriteAsync($"sales report - {year} - {month}");
        } else
        {
            await context.Response.WriteAsync($"the requested month: {month} is not supported. Only apr, jul, oct or jan is allowed.");
        }*/
    });
    endpoints.Map("clients/{client-type:alpha:client-types}/{id:int:min(1)?}",
        async context =>
        {
            int id = Convert.ToInt32(context.Request.RouteValues["id"]);
            string? clientType = Convert.ToString(context.Request.RouteValues["client-type"]);
            await context.Response.WriteAsync($"Client: {clientType}, id = {id}");
        }
    );
    endpoints.Map("clients/{client-type}/{id?}",
        async context =>
        {
            await context.Response.WriteAsync("Client type is not acceptable. There are only: physical, business, or corporate types available");
        });
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.Run(async context =>
{
    await context.Response.WriteAsync($"There are no existing endpoints that would match to: {context.Request.Path}");
});

//app.MapGet("/", () => "Hello World!");
/*Middleware 1
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("Hello from Middleware 1\n");
    await next(context);
});

app.UseWhen(context => context.Request.Query.ContainsKey("firstName"), app =>
{
    app.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("Hello from branch middleware\n ");
        await next();
    });
});

//Middleware 2
//app.UseMiddleware<MyCustomMiddleware>(); - a call of the function without using an extension method
//app.UseMyCustomMiddleware();    //using Extension method
app.UseMyCustomConventionalMiddleware();    //using an extension method of the conventional custom middleware class method

//Middleware 3
app.Run(async (context) => {
    //await context.Response.WriteAsync($"{nameof(MyCustomMiddleware)}");
    await context.Response.WriteAsync("Hello from middleware 3\n");
});*/

app.Run();
