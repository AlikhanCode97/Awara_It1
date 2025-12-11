using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using System;

namespace AwaraIT.BCS.Plugins.PluginExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class AssertionConditionAttribute : Attribute
    {
        public AssertionConditionAttribute(AssertionConditionType conditionType)
        {
            this.ConditionType = conditionType;
        }

        public AssertionConditionType ConditionType { get; private set; }
    }
}
