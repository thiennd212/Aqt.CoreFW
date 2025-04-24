using Aqt.CoreFW.OrganizationUnits;
using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Aqt.CoreFW.EntityFrameworkCore;

public static class CoreFWEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        CoreFWGlobalFeatureConfigurator.Configure();
        CoreFWModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            /* You can configure extra properties for the
             * entities defined in the modules used by your application.
             *
             * This class can be used to map these extra properties to table fields in the database.
             *
             * USE THIS CLASS ONLY TO CONFIGURE EF CORE RELATED MAPPING.
             * USE CoreFWModuleExtensionConfigurator CLASS (in the Domain.Shared project)
             * FOR A HIGH LEVEL API TO DEFINE EXTRA PROPERTIES TO ENTITIES OF THE USED MODULES
             *
             * Example: Map a property to a table field:

                 ObjectExtensionManager.Instance
                     .MapEfCoreProperty<IdentityUser, string>(
                         "MyProperty",
                         (entityBuilder, propertyBuilder) =>
                         {
                             propertyBuilder.HasMaxLength(128);
                         }
                     );

             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
             */
            // --- MapEfCoreProperty cho OrganizationUnit ---
            // Lưu ý: Việc gọi AddOrUpdateProperty nên được thực hiện ở Domain Module

            ObjectExtensionManager.Instance.MapEfCoreProperty<OrganizationUnit, string>(
                OrganizationUnitExtensionProperties.ManualCode,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxManualCodeLength)
                                   .HasColumnName(OrganizationUnitExtensionProperties.ManualCode);
                }
            );

            ObjectExtensionManager.Instance.MapEfCoreProperty<OrganizationUnit, OrganizationUnitStatus>(
                OrganizationUnitExtensionProperties.Status,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.IsRequired()
                                   .HasDefaultValue(OrganizationUnitStatus.Active)
                                   .HasConversion<byte>()
                                   .HasColumnName(OrganizationUnitExtensionProperties.Status);
                }
            );

            ObjectExtensionManager.Instance.MapEfCoreProperty<OrganizationUnit, int>(
                OrganizationUnitExtensionProperties.Order,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.IsRequired()
                                   .HasDefaultValue(0)
                                   .HasColumnName(OrganizationUnitExtensionProperties.Order);
                }
            );

            ObjectExtensionManager.Instance.MapEfCoreProperty<OrganizationUnit, string>(
                OrganizationUnitExtensionProperties.Description,
                (entityBuilder, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxDescriptionLength)
                                   .HasColumnName(OrganizationUnitExtensionProperties.Description);
                }
            );

            ObjectExtensionManager.Instance.MapEfCoreProperty<OrganizationUnit, DateTime?>(
                OrganizationUnitExtensionProperties.LastSyncedTime,
                 (entityBuilder, propertyBuilder) =>
                 {
                     propertyBuilder.HasColumnName(OrganizationUnitExtensionProperties.LastSyncedTime);
                 }
            );

            ObjectExtensionManager.Instance.MapEfCoreProperty<OrganizationUnit, string>(
                OrganizationUnitExtensionProperties.SyncRecordId,
                 (entityBuilder, propertyBuilder) =>
                 {
                     propertyBuilder.HasMaxLength(CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxSyncRecordIdLength)
                                    .HasColumnName(OrganizationUnitExtensionProperties.SyncRecordId);
                 }
            );

            ObjectExtensionManager.Instance.MapEfCoreProperty<OrganizationUnit, string>(
                OrganizationUnitExtensionProperties.SyncRecordCode,
                 (entityBuilder, propertyBuilder) =>
                 {
                     propertyBuilder.HasMaxLength(CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxSyncRecordCodeLength)
                                    .HasColumnName(OrganizationUnitExtensionProperties.SyncRecordCode);
                 }
            );
        });
    }
}
