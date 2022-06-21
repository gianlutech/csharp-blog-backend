//per il context aggiungiamo:
using Microsoft.EntityFrameworkCore;
using csharp_blog_backend.Models;
//end per il context aggiungiamo:


var builder = WebApplication.CreateBuilder(args);


// MODIFICA AGGIUNTA PER IL CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowAnyMethod();
        });
});


builder.Services.AddControllers();


string sConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogContext>(options =>
  options.UseSqlServer(sConnectionString));



//per il context aggiungiamo
//builder.Services.AddDbContext<BlogContext>(opt =>
  //  opt.UseInMemoryDatabase("posts"));
//end per il context aggiungiamo:


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(); // MODIFICA AGGIUNTA PER IL CORS

app.UseHttpsRedirection();
app.UseStaticFiles();   // permette di vedere le immagini da URL in questo caso

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())

{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<BlogContext>();

    context.Database.EnsureCreated();  //crea il Db

    
}


app.Run();
