using WebApplication1.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<MyCustomMiddleware>();    //very important to use AddTransient method in order to make the extension method
                                                        // working and being able to get executed
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
//Middleware 1
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("From Middleware 1\n");
    await next(context);
});

//Middleware 2
//app.UseMiddleware<MyCustomMiddleware>(); - a call of the function without using an extension method
//app.UseMyCustomMiddleware();    //using Extension method
app.UseMyCustomConventionalMiddleware();    //using an extension method of the conventional custom middleware class method


//Middleware 3
app.Run(async (HttpContext context) => {
    //await context.Response.WriteAsync($"{nameof(MyCustomMiddleware)}");
    await context.Response.WriteAsync("Middleware 3\n");
});


app.Run();
