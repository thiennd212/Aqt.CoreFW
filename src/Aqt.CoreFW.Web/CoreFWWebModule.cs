using Aqt.CoreFW.Application.Workflows.Interfaces;
using Aqt.CoreFW.Application.Workflows.Providers;
using Aqt.CoreFW.EntityFrameworkCore;
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.MultiTenancy;
using Aqt.CoreFW.Web.HealthChecks;
using Aqt.CoreFW.Web.Menus;
using Aqt.CoreFW.Web.Workflows;
using EasyAbp.FileManagement;
using EasyAbp.FileManagement.Containers;
using EasyAbp.FileManagement.EntityFrameworkCore;
using EasyAbp.FileManagement.Files;
using EasyAbp.FileManagement.Options;
using EasyAbp.FileManagement.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using OptimaJet.Workflow.Core.Runtime;
using System;
using System.IO;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace Aqt.CoreFW.Web;

[DependsOn(
    typeof(CoreFWHttpApiModule),
    typeof(CoreFWApplicationModule),
    typeof(CoreFWEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpFeatureManagementWebModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(FileManagementWebModule),
    typeof(FileManagementApplicationModule),
    typeof(FileManagementDomainModule),
    typeof(FileManagementHttpApiModule),
    typeof(FileManagementEntityFrameworkCoreModule),
    typeof(AbpBlobStoringFileSystemModule)
)]
public class CoreFWWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(CoreFWResource),
                typeof(CoreFWDomainModule).Assembly,
                typeof(CoreFWDomainSharedModule).Assembly,
                typeof(CoreFWApplicationModule).Assembly,
                typeof(CoreFWApplicationContractsModule).Assembly,
                typeof(CoreFWWebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("CoreFW");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });

            Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });
        }

        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureHealthChecks(context);
        ConfigureAuthentication(context);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(context.Services);

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = true;
        });

        Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<LocalFileSystemBlobContainer>(container =>
            {
                container.IsMultiTenant = true;
                container.UseFileSystem(fileSystem =>
                {
                    // fileSystem.BasePath = "C:\\my-files";
                    fileSystem.BasePath = Path.Combine(hostingEnvironment.ContentRootPath, "FileUploads");
                });
            });
        });

        Configure<FileManagementOptions>(options =>
        {
            options.DefaultFileDownloadProviderType = typeof(LocalFileDownloadProvider);
            options.Containers.Configure<CommonFileContainer>(container =>
            {
                // private container never be used by non-owner users (except user who has the "File.Manage" permission).
                container.FileContainerType = FileContainerType.Public;
                container.AbpBlobContainerName = BlobContainerNameAttribute.GetContainerName<LocalFileSystemBlobContainer>();
                container.AbpBlobDirectorySeparator = "/";

                container.RetainUnusedBlobs = false;
                container.EnableAutoRename = true;

                container.MaxByteSizeForEachFile = 5 * 1024 * 1024;
                container.MaxByteSizeForEachUpload = 10 * 1024 * 1024;
                container.MaxFileQuantityForEachUpload = 2;

                container.AllowOnlyConfiguredFileExtensions = true;
                container.FileExtensionsConfiguration.Add(".jpg", true);
                container.FileExtensionsConfiguration.Add(".PNG", true);
                container.FileExtensionsConfiguration.Add(".doc", true);
                container.FileExtensionsConfiguration.Add(".docx", true);
                container.FileExtensionsConfiguration.Add(".xls", true);
                container.FileExtensionsConfiguration.Add(".xlsx", true);
                container.FileExtensionsConfiguration.Add(".pdf", true);
                // container.FileExtensionsConfiguration.Add(".tar.gz", true);
                // container.FileExtensionsConfiguration.Add(".exe", false);

                container.GetDownloadInfoTimesLimitEachUserPerMinute = 10;
            });
        });


        #region Config workflow
        context.Services.AddTransient<IWorkflowRuleProvider, WorkflowRuleProvider>();
        context.Services.AddTransient<IWorkflowActionProvider, WorkflowActionProvider>();
        context.Services.AddTransient<IDesignerAutocompleteProvider, AutoCompleteProvider>();
        context.Services.AddSingleton<WorkflowRuntime>(sp =>
        {
            return WorkflowRuntimeConfigurator.CreateEmpty(sp);
        });
        context.Services.AddSingleton<WorkflowRuntimeAccessor>();
        #endregion
    }


    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddCoreFWHealthChecks();
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-scripts.js");
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CoreFWWebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CoreFWWebModule>();

            if (hostingEnvironment.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<CoreFWDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Aqt.CoreFW.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<CoreFWDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Aqt.CoreFW.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<CoreFWApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Aqt.CoreFW.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<CoreFWApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Aqt.CoreFW.Application", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<CoreFWHttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Aqt.CoreFW.HttpApi", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<CoreFWWebModule>(hostingEnvironment.ContentRootPath);
            }
        });
    }

    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new CoreFWMenuContributor());
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new CoreFWToolbarContributor());
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(CoreFWApplicationModule).Assembly);
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreFW API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();
        app.UseRouting();
        app.UseAbpSecurityHeaders();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreFW API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();

        #region Start workflow runtime
        var sp = context.ServiceProvider;
        var runtime = sp.GetRequiredService<WorkflowRuntime>();

        var accessor = sp.GetRequiredService<WorkflowRuntimeAccessor>();

        var basicPlugin = sp.GetRequiredService<IBasicPluginFactory>().Create();
        var approvalPlugin = sp.GetRequiredService<IApprovalPluginFactory>().Create();
        var actionProvider = sp.GetRequiredService<IWorkflowActionProvider>();
        var ruleProvider = sp.GetRequiredService<IWorkflowRuleProvider>();
        var autoCompleteProvider = sp.GetRequiredService<IDesignerAutocompleteProvider>();

        runtime
            .WithPlugin(basicPlugin)
            .WithPlugin(approvalPlugin)
            .WithActionProvider(actionProvider)
            .WithRuleProvider(ruleProvider)
            .WithDesignerAutocompleteProvider(autoCompleteProvider)
            //.RunMigrations()
            .Start();

        accessor.Runtime = runtime;
        #endregion
    }
}
