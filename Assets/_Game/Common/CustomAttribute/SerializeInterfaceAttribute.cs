using System;
using UnityEngine;

namespace Assets._Game.Common.CustomAttribute
{
    public class SerializeInterfaceAttribute : PropertyAttribute
    {
        public Type Type { get;  }

        public SerializeInterfaceAttribute(Type type)
        {
            Type = type;
        }
    }
}