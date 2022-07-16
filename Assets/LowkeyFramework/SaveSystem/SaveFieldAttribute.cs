using System;

namespace LowkeyFramework.AttributeSaveSystem
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SaveFieldAttribute : Attribute { }
}