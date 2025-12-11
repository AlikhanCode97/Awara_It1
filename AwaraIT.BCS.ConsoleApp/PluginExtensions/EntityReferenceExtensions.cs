using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaraIT.BCS.ConsoleApp.PluginExtensions
{
    public static class EntityReferenceExtensions
    {
        public static bool EntityReferenceIsValid(this EntityReference entityReference)
        {
            return entityReference != null && entityReference.Id != Guid.Empty;
        }
    }
}
