using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OtlobLap.Auth;
using OtlobLap.Configuration;
using OtlobLap.Data;
using OtlobLap.Services;
using OtlobLap.Validations;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using OtlobLap.Resources;
using System.Reflection;
using OtlobLap.Models;
using Stripe;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<LocalizationService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization(/*LanguageViewLocationExpanderFormat.Suffix*/)
    .AddDataAnnotationsLocalization(options=>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
            return factory.Create("SharedResource", assemblyName.Name);
        };
    });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
    new CultureInfo("en-US"), // English (United States)
    new CultureInfo("ar-EG"), // Arabic (Egypt)

    // Add more languages here
    new CultureInfo("es-ES"),
    new CultureInfo("fr-FR"),
    new CultureInfo("zh-CN"),
    new CultureInfo("pt-BR"),
    new CultureInfo("de-DE"),
    new CultureInfo("ja-JP"),
    new CultureInfo("ru-RU"),
    new CultureInfo("hi-IN") 
};


    options.DefaultRequestCulture = new RequestCulture(culture: "ar-EG", uiCulture: "ar-EG");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
});

// Add services to the container.
builder.Services.AddControllersWithViews();
    

builder.Services.AddDbContext<AppDbContext>(options => 
                                            options.
                                            UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"),
                                            builder =>
                                            {
                                                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                                            }

                                            ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IProductService, OtlobLap.Services.ProductService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IUserManagerService, UserManagerService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ImageValidator, ImageValidator>();
builder.Services.AddScoped<IWishListService, WishListService>();
builder.Services.AddScoped<IDiscountService, OtlobLap.Services.DiscountService>();
builder.Services.AddScoped<IReviewService, OtlobLap.Services.ReviewService>();
builder.Services.AddScoped<ISellerHomeService, SellerHomeService>();
builder.Services.AddScoped<IAdminHomeService, AdminHomeService>();
builder.Services.AddScoped<IBannerViewService,BannerViewService>();
builder.Services.AddScoped<IBrandService,BrandService>();
builder.Services.AddScoped<IPaginationService,PaginationService>();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath= "/account/AccessDenied";
});

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.Zero
);


builder.Services.Configure<IdentityOptions>(options =>
{

    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;

});

builder.Services.Configure<ProductImageUploadOptions>(builder.Configuration.GetSection("ProductImageUploadOptions"));
builder.Services.Configure<ProductsViewOptions>(builder.Configuration.GetSection("ProductsViewOptions"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseSession();
app.UseRouting();

var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseAuthorization();

using (var serviceScope = app.Services.CreateScope())
{
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var imageServie = serviceScope.ServiceProvider.GetRequiredService<IImageService>();

    await Seeding.SeedRolesAsync(roleManager);
    await Seeding.SeedUserAdminAsync(imageServie, userManager, roleManager);
}
var localiztionService = app.Services.GetService<LocalizationService>();
Utility.localizationService = localiztionService;

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



//// Replace "Resource_ar.resx" with the path to your .resx file
string resourceFileName = "SharedResource.ar-EG.resx";

Dictionary<string, string> arabicLocalization = new Dictionary<string, string>()
{
       
    };

// Update translations
foreach (var translation in arabicLocalization)
{
    ResourceManager.UpdateTranslations(resourceFileName, translation.Key, translation.Value);
}


app.Run();

