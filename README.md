# ASP.NET Core Self-Hosting with a .NET 5 Windows Forms App

Demonstrates how to self-host an ASP.NET Core app using a .NET 5 Windows Forms app.

## Self-Hosted MVC Core with a .NET 5 Windows Forms App

1. Add a new .NET 5 Windows Forms project.

2. Update the Project SDK in .csproj file to **Web**.
   - Add .Web to  Microsoft.NET.Sdk
    ```xml
    <Project Sdk="Microsoft.NET.Sdk.Web">
        <PropertyGroup>
            <OutputType>WinExe</OutputType>
            <TargetFramework>net5.0-windows</TargetFramework>
            <UseWindowsForms>true</UseWindowsForms>
        </PropertyGroup>
    </Project>
    ```

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    ```

4. Add a `CreateWebHostBuilder` method to Program.cs.

    ```csharp
    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    ```

    - Add a static `Form` property.

    ```csharp
    public static MainForm Form { get; private set; }
    ```

    - Call the method asynchronously from Main.
    - Set `Form` property and pass to `Application.Run`.

    ```csharp
    static void Main()
    {
        Task.Run(() => CreateHostBuilder().Build().Run());
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Form = new MainForm();
        Application.Run(Form);
    }
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

7. Use an http client such as Fiddler or Postman to send requests
     - Uri: `http://localhost:5000/hello/`
     - Add Accept and Content-Type headers: application/json

8. Add a .NET Core console app to send requests.
     - Add NuGet packages:
       - Microsoft.Extensions.Http
       - Newtonsoft.Json
     - Use `HttpClient` to send Post and Get requests.

