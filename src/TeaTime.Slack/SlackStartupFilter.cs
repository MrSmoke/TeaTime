namespace TeaTime.Slack
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.FileProviders;

    public class SlackStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new ManifestEmbeddedFileProvider(ServiceCollectionExtension.Assembly),
                    RequestPath = "/slack",
                });

                next(app);
            };
        }
    }
}
