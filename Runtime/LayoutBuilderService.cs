using System;
using System.Linq;
using System.Reflection;
using LittleBit.Modules.CoreModule.MonoInterfaces;
using UnityEngine;

namespace LittleBit.Modules.UI
{
    public class LayoutBuilderService : ILayoutBuilderService
    {
        private readonly MethodInfo _buildLayoutOfTypeMethod;
        private Canvas _canvas;
        private readonly LayoutFactory _layoutFactory;

        public LayoutBuilderService(LayoutFactory layoutFactory)
        {
            _layoutFactory = layoutFactory;

            _buildLayoutOfTypeMethod = typeof(LayoutBuilderService).GetMethods()
                .Last(m => m.Name == "BuildLayoutOfType" && m.IsGenericMethodDefinition);
        }

        public T BuildLayout<T>(ILayout rootLayout, ILayout prefab) where T : ILayout
        {
            var layout = _layoutFactory.Create<T>(prefab);

            try
            {
                rootLayout.AppendChildElement(layout);

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(GetErrorRoot(rootLayout));
                Debug.LogError(GetErrorPrefab(prefab));
                throw;
            }

            return layout;
        }

        public T BuildLayoutOfType<T>() where T : ILayout
        {
            var layout = _layoutFactory.Create<T>();
            return layout;
        }

        public T BuildLayoutOfType<T>(string style) where T : ILayout
        {
            var layout = _layoutFactory.Create<T>(style);
            return layout;
        }

        public T BuildLayoutOfType<T>(ILayout rootLayout, string style = "Default") where T : ILayout
        {
            T layout;
            try
            {
                layout = _layoutFactory.Create<T>(style);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(GetErrorType<T>());
                Debug.LogError(GetErrorRoot(rootLayout));
                Debug.LogError(GetErrorStyle(style));
                throw;
            }

            try
            {
                rootLayout.AppendChildElement(layout);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError(GetErrorType<T>());
                Debug.LogError(GetErrorRoot(rootLayout));
                Debug.LogError(GetErrorStyle(style));
                throw;
            }
            
            return layout;
        }
        

        public Layout BuildLayoutOfType(Type type, ILayout rootLayout, string style = "Default")
        {
            return BuildLayoutOfTypeUsingReflections(rootLayout, type, style);
        }

        private Layout BuildLayoutOfTypeUsingReflections(ILayout rootLayout, Type type, string style)
        {
            MethodInfo generic = _buildLayoutOfTypeMethod.MakeGenericMethod(type);

            var layout = (Layout) generic.Invoke(this, new object[]
            {
                rootLayout,
                style
            });


            rootLayout.AppendChildElement(layout);

            return layout;
        }
        
        private string GetErrorRoot(ILayout rootLayout)
        {
            return "Root: " + rootLayout.GetGameObject().name + "\t";
        }

        private string GetErrorPrefab(ILayout prefab)
        {
            return "Prefab: " + prefab.GetGameObject().name + "\t";
        }

        private string GetErrorStyle(string style)
        {
            return "Style: " + style + "\t";
        }

        private string GetErrorType<T>()
        {
            return "Type: " + typeof(T);
        }
    }
}