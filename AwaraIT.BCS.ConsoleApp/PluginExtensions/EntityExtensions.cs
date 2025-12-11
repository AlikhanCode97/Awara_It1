using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaraIT.BCS.ConsoleApp.PluginExtensions
{
    public static class EntityExtensions
    {
        public static bool IsEntityValid(this Entity entity)
        {
            return entity != null && entity.Id != null && entity.Id != Guid.Empty;
        }
    }
}
