using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using PoultryPro_Portal.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<FirestoreDb>(sp =>
{
    var firestoreClientBuilder = new FirestoreClientBuilder
    {
        Credential = GoogleCredential.FromFile("./Database/Firebase/fir-test-9cec2-firebase-adminsdk-feja5-989a4e0208.json")
    };
    var firestoreClient = firestoreClientBuilder.Build();
    return FirestoreDb.Create("fir-test-9cec2", firestoreClient);
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IAdminService, AdminService>();
builder.Services.AddScoped<ICallCenterAgentService, CallCenterAgentService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();
app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=DashBoard}/{action=AgentDashboard}/{id?}");

app.Run();
