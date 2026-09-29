using System;

//Author : MERFOUD Kelyan

namespace Tooling
{
    /// <summary>
    /// <see cref="ILoadable"/> is an <see langword="interface"/> which contains <see cref="Tooling.LoadType"/> and some pre-filled <see langword="method"/>
    /// </summary>
    public interface ILoadable
    {
        public LoadType LoadType { get; set; }
        public virtual bool IsUniqueGameObject() => (LoadType & LoadType.AsUniqueGameObject) != 0;
        public virtual bool IsActive() => (LoadType & LoadType.Active) != 0;
        public virtual bool IsDestroyedOnLoad() => (LoadType & LoadType.DestroyOnLoad) != 0;
    }
}