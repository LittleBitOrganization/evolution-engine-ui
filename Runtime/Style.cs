using System;
using LittleBit.Modules.CoreModule.MonoInterfaces;
using UnityEngine;

namespace LittleBit.Modules.UI
{
    [Serializable]
    public class Style
    {
        public string name;
        [SerializeField] private LayoutInterfaceContainer _layout;
        public int poolSize;

        public ILayout Layout => _layout.Result;
    }
}