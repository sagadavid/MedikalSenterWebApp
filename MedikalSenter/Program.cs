using MedikalSenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration
//.GetConnectionString("DefaultConnection");//injected to options below

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
//.UseSqlite(connectionString)    
//.UseSqlServer(connectionString));


//name connection string as context
//move builder.configuration service as paramater to database use service/usesqlite
builder.Services.AddDbContext<MedikalSenterContext>(options =>
    options
    .UseSqlite(builder.Configuration.GetConnectionString("MedikalSenterContext")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

IdentityBuilder identityBuilder = builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//lep//debug/debug properties/appurl (change order, take http first)
//lep//after that come and comment it out to enable kestrel service
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//for a good start with builtin names/data
MSInitializer.Seed(app);

//lep//port 1025 is always empty to use, if a port is not avaliable, debug/properties/appurl 
app.Run();
