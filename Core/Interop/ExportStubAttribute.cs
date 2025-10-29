using System;

namespace BX.Tweening.Interop
{
    /// <summary>
    /// Stub attribute used to notify that the given field is to be exported.
    /// <br>This isn't required if your framework : </br>
    /// <br>* Serialize <see langword="public"/> <b>properties</b> <i>(not fields)</i> by default</br>
    /// <br>---</br>
    /// <br>
    /// Otherwise, you may need to target this attribute as to export or serialize <see langword="private"/> or <see langword="protected"/> fields, 
    /// or modify <see cref="BXSTweenable"/> source (it's the only base that contains data to serialize, in the default implementations)
    /// </br>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportStubAttribute : Attribute { }
}
