using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TVProgViewer.Core.Configuration;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Services.Tasks;
using TVProgViewer.Web.Framework.Infrastructure.Extensions;

namespace TVProgViewer.WebUI
{
    /// <summary>
    /// ������������ ��������� ����� ����������
    /// </summary>
    public class Startup
    {
        #region ����

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private AppSettings _appSettings;
        private IEngine _engine;

        #endregion

        #region �����������

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        /// <summary>
        /// ���������� �������� � ���������� � ���������������� ���������� ��������
        /// </summary>
        /// <param name="services">��������� ���������� �� �������</param>
        public void ConfigureServices(IServiceCollection services)
        {
            (_engine, _appSettings) = services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
        }

        /// <summary>
        /// ���������������� DI-���������� 
        /// </summary>
        /// <param name="builder">Container builder</param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            _engine.RegisterDependencies(builder, _appSettings);
        }

        /// <summary>
        /// ���������������� ���������� ��������� HTTP-��������
        /// </summary>
        /// <param name="applicaton">��������� ��� ���������������� ���������� ��������� ��������</param>
        public void Configure(IApplicationBuilder application)
        {
            application.ConfigureRequestPipeline();
            application.StartEngine();
        }
    }
}