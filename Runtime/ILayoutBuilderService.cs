using System;
using LittleBit.Modules.CoreModule.MonoInterfaces;

namespace LittleBit.Modules.UI
{
    public interface ILayoutBuilderService
    {
        public T BuildLayout<T>(ILayout rootLayout, ILayout prefab) where T : ILayout;
        public T BuildLayoutOfType<T>() where T : ILayout;
        public T BuildLayoutOfType<T>(string style) where T : ILayout;
        public T BuildLayoutOfType<T>(ILayout rootLayout, string style = "Default") where T : ILayout;
        public Layout BuildLayoutOfType(Type type, ILayout rootLayout, string style = "Default");
    }
}