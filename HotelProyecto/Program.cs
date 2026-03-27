using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseRouting();

var contentPath = Path.Combine(app.Environment.ContentRootPath, "Content");
if (Directory.Exists(contentPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(contentPath),
        RequestPath = "/Content"
    });
}

var imagenesPath = Path.Combine(app.Environment.ContentRootPath, "Imagenes");
if (Directory.Exists(imagenesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imagenesPath),
        RequestPath = "/Imagenes"
    });
}

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
