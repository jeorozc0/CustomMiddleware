using webx.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<MyAuth>();
app.Run(async context =>
{
await context.Response.WriteAsync("User details: "+context.Request.HttpContext.Items["userdetails"]);
});



app.MapGet("/", () => "Hello World Hi!");

app.Run();
