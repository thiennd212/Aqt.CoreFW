using OptimaJet.Workflow.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Application.Workflows.Interfaces
{
    public interface IBasicPluginFactory : ITransientDependency
    {
        BasicPlugin Create();
    }
}
