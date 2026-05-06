using System;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling
{
    /// <summary>
    /// <see cref="AutoLoad"/> is an <see cref="Attribute"/> that allows you to easily load certain classes.
    /// For instance, if you need your <see cref="MonoBehaviour"/> to be present in every scene of your project,
    /// instead of manually adding it to every scene, simply place the <see cref="AutoLoad"/> attribute above your
    /// <see cref="MonoBehaviour"/> class. This can be especially useful for classes like <c>SoundManager</c>
    /// or <c>GameManager</c>. Additionally, any class marked with this <see cref="Attribute"/> will be
    /// instantiated with a priority, which effectively means it will load before your other <see cref="GameObject"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoLoad : Attribute, ILoadable
    {
        public LoadType LoadType { get; set; } = LoadType.None;
    }

    [Flags]
    public enum LoadType
    {
        None = 0,

        /// <summary>
        /// if <see langword="true"></see> is going to <see cref= "UnityEngine.Object.Destroy"></see> 
        /// your object on <c>Scene</c> Reload and then create another one
        /// </summary>
        DestroyOnLoad = 1 << 1,

        /// <summary>
        /// Will instantiate this separately if marked as <see langword="true"></see>.
        /// If your <see cref="MonoBehaviour"></see> need to interact with  <see cref="Transform"></see> for instance
        /// or with the <see cref="GameObject"></see> in general, Set this to <see langword="true"></see>
        /// </summary>
        AsUniqueGameObject = 1 << 2,

        /// <summary>
        /// Used for the <c>AutoLoad Window</c> Mostly useless in a different context. You cannot toggle the 
        /// <c>Active</c> bit with the <see cref="AutoLoad"></see> Attribute directly from your code
        /// </summary>
        Active = 1 << 3
    }

    public enum RuntimeAutoLoadType { BeforeAutoLoad, AfterAutoLoad }
}