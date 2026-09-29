using System;

//Author : MERFOUD Kelyan

namespace Tooling
{
    /// <summary>
    /// Put <see cref="RuntimeAutoLoadMethod"></see> on a <c><see langword="static"/></c> <see langword="method"/> to call it <c>before</c> or <c>after</c>
    /// <see cref="AutoLoad"></see>, you can use it for hierarchy purposes or to mimic a "fake" Start on a <see langword="class"/>.
    /// <see cref="AutoLoad"></see>, you can use it for hierarchy purposes or to mimic a "fake" Start on a <see langword="class"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RuntimeAutoLoadMethod : Attribute
    {
        public RuntimeAutoLoadType RuntimeAutoLoadType = RuntimeAutoLoadType.AfterAutoLoad;
    }
}