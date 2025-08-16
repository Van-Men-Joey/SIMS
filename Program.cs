using SIMS.Data;
using SIMS.Interfaces;
using SIMS.Middlewares;
using SIMS.Repositories;
using SIMS.Services;
// ❌ Không dùng EF Core và DbContext nữa
// using Microsoft.EntityFrameworkCore;
// using SIMS.Data;

var builder = WebApplication.CreateBuilder(args);

// ====================
// Logging chi tiết
// ====================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// ====================
// ❌ Cấu hình Database (ĐÃ BỎ - dùng CSV thay thế)
// ====================
// builder.Services.AddDbContext<SIMSContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ====================
// Đăng ký Repository (bản CSV)
// ====================
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepositoryCsv>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepositoryCsv>();
builder.Services.AddScoped<ICourseRepository, CourseRepositoryCsv>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepositoryCsv>();
builder.Services.AddScoped<IReportRepository, ReportRepositoryCsv>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepositoryCsv>();
builder.Services.AddScoped<IUserRepository, UserRepositoryCsv>();

// ====================
// Đăng ký Service
// ====================
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<SIMS.Services.IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IUserService, UserService>();

// ====================
// Razor Pages + Controllers
// ====================
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// ====================
// Cấu hình Session
// ====================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// ====================
// Build app
// ====================
var app = builder.Build();

// ====================
// Seed dữ liệu CSV
// ====================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedDataCsv.InitializeAsync(services);
        Console.WriteLine("✅ Seed dữ liệu CSV thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi khi seed dữ liệu CSV: {ex.Message}");
    }
}

// ====================
// Middleware pipeline
// ====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Bật session trước middleware custom
app.UseSession();

// Bảo vệ role song song với [Authorize]
app.UseMiddleware<RoleBasedAccessMiddleware>();

app.UseAuthorization();

// Map Razor Pages và Controller
app.MapRazorPages();
app.MapControllers();

// Redirect mặc định về trang login
app.MapGet("/", (HttpContext context) =>
{
    context.Response.Redirect("/Auth/Login");
    return Task.CompletedTask;
});

app.Run();
