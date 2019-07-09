# ASP.NET Core Self-Hosting with .NET 4.x Apps

Demonstrates how to self-host an ASP.NET Core app using .NET 4.x apps.

## Self-Hosted MVC Core with Windows Forms 4.x

1. Add a new Windows Forms project.
    - Set framework to .NET 4.7.2

2. Add the following NuGet packages
     - Microsoft.AspNetCore
     - Microsoft.AspNetCore.Mvc.Core
     - Microsoft.AspNetCore.Mvc.Formatters.Json

3. Add a `Startup` class.
     - Configure Mvc core with Json formatters.

    ```csharp
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonFormatters();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
    ```

4. Add a `CreateWebHostBuilder` method to Program.cs.

    ```csharp
    public static IWebHostBuilder CreateWebHostBuilder() =>
        WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>();
    ```

     - Call the method asynchronously from Main.

    ```csharp
    Task.Run(() => CreateWebHostBuilder().Build().Run());
    ```

     - Add a static `MainForm` property.

    ```csharp
    public static MainForm Form { get; private set; }
    ```

5. Add a TextBox to the main form.
     - Name it: nameTextBox
     - Add a NameText property
     - Use SynchronizationContext for the setter

    ```csharp
    public partial class MainForm : Form
    {
        private SynchronizationContext _syncRoot;

        public MainForm()
        {
            InitializeComponent();
            _syncRoot = SynchronizationContext.Current;
        }

        public string NameText
        {
            get { return nameTextBox.Text; }
            set { _syncRoot.Post(SetName, value); }
        }

        private void SetName(object arg)
        {
            string name = arg as string;
            if (name != null)
                nameTextBox.Text = name;
        }
    }
    ```

6. Add a GreetingController with Get and Post methods.
     - Get and set the NameText property on the main form

    ```csharp
    [Route("/hello")]
    public class GreetingController
    {
        [HttpGet]
        public IActionResult Get()
        {
            var name = Program.Form.NameText;
            var greeting = "Hello " + name;
            return new JsonResult(greeting);
        }

        [HttpPost]
        public IActionResult Post([FromBody] string name)
        {
            Program.Form.NameText = name;
            return new NoContentResult();
        }
    }
    ``` 

8. Use an http client such as Fiddler or Postman to send requests
     - Uri: `http://localhost:5000/hello/`
     - Add Accept and Content-Type headers: application/json

9. Add a .NET Core console app to send requests.
     - Add NuGet packages:
       - Microsoft.Extensions.Http
       - Newtonsoft.Json
     - Use `HttpClient` to send Post and Get requests.

