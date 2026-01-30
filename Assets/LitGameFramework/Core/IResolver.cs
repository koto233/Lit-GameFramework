using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LitGameFramework.Core
{
    public interface IResolver
    {
        T Resolve<T>() where T : class;
    }

}
