# ASP.NET Core Self-Hosting with .NET 4.6 Apps

Demonstrates how to self-host an ASP.NET Core app using .NET 4.6 apps.

## Self-Hosted MVC Core with Windows Forms 4.6

1. Add a new Windows Forms project.
  - Set framework to .NET 4.6

2. Edit the .cjproj file in a text editor
  - Using the Productivity Power Tools extension makes this easier
  - Add the following to the first `<PropertyGroup>`
  - This is necessary for NuGet to bring in libuv.dll

    ```xml
    <BaseNuGetRuntimeIdentifier>win7-x86</BaseNuGetRuntimeIdentifier>
    ```

3. Add project.json file.

    ```json
    {
      "dependencies": {
        "Microsoft.AspNetCore.Mvc.Core": "1.0.0-*",
        "Microsoft.AspNetCore.Mvc.Formatters.Json": "1.0.0-*",
        "Microsoft.AspNetCore.Server.Kestrel": "1.0.0-*",
        "Microsoft.NETCore.Platforms": "1.0.1-*"
      },
      "runtimes": {
        "win7-x86": {}
      },
      "frameworks": {
        "net46": {}
      }
    }
    ```

4. Add a `RunWebHost` method to Program.cs.

    ```csharp
    private static void RunWebHost()
    {
        var host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .Build();
        host.Run();
    }
    ```

  - Call the method asynchronously from Main.

    ```csharp
    Task.Run(() => RunWebHost());
    ```

5. Add a `Startup` class.
  - Configure Mvc core with Json formatters.

    ```csharp
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                    .AddJsonFormatters();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
    ```
6. Add a TextBox to the main form
  - Name it: nameTextBox
  - Add a NameText property
  - Use SynchronizationContext for the setter

    ```charp
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

7. Add a GreetingController with Get and Post methods.
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

9. Use the ConsoleClient to send requests
  - Add the NuGet package: Microsoft.AspNet.WebApi.Client
  - First run the WinForms project, then the console client

