using System;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// Stub attribute used to notify that the given field has it's name changed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormerNameAttribute : Attribute
    {
        public readonly string name;

        public FormerNameAttribute(string name)
        {
            this.name = name;
        }
    }
}
