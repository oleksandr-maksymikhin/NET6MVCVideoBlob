using Azure.Storage.Blobs;

namespace NET6MVCVideoBlob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton(provider => {
                IConfigurationRoot configuration = new ConfigurationBuilder().
                AddJsonFile("appsettings.json").
                Build();
                return configuration;
            });

            IConfigurationRoot configuration = new ConfigurationBuilder().
                AddJsonFile("appsettings.json").
                Build();

            string connectionString = configuration["StorageConnectionString"];

            builder.Services.AddSingleton(provider => {
                BlobServiceClient serviceClient = new BlobServiceClient(connectionString);
                return serviceClient;
            });

            //???
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}