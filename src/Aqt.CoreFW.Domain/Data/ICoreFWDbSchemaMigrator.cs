using System.Threading.Tasks;

namespace Aqt.CoreFW.Data;

public interface ICoreFWDbSchemaMigrator
{
    Task MigrateAsync();
}
