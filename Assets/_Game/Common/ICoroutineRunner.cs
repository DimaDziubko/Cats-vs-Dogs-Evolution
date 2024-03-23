using System.Collections;
using UnityEngine;

namespace _Game.Common
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}