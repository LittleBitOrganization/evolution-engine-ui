using System;
using System.Collections.Generic;
using System.Linq;
using LittleBit.Modules.CoreModule.AssetManagement;
using LittleBit.Modules.CoreModule.MonoInterfaces;
using UnityEngine;

namespace LittleBit.Modules.UI
{
    public class LayoutFactory
    {
        private readonly ICreator _creator;
        private AssetProvider _assetProvider;

        private Dictionary<Type, Dictionary<string, ILayout>> _layoutStylesConfigs;
        //private Dictionary<Type, Dictionary<string, ObjectPool>> _pooledLayouts;

        private const string PathToStylesConfigs = "Configs/Layout Styles Configs";

        public LayoutFactory(ICreator creator, AssetProvider assetProvider)
        {
            _layoutStylesConfigs = new Dictionary<Type, Dictionary<string, ILayout>>();
            //_pooledLayouts = new Dictionary<Type, Dictionary<string, ObjectPool>>();


            _creator = creator;
            _assetProvider = assetProvider;

            LoadConfigs();
        }

        public void Initialize()
        {
            //InitPools();
        }

        private void LoadConfigs()
        {
            _layoutStylesConfigs = new Dictionary<Type, Dictionary<string, ILayout>>();
            var configs = _assetProvider.GetAssets<LayoutStylesConfig>(PathToStylesConfigs);

            foreach (var config in configs)
            {
                string fileName = config.name;
                try
                {
                    foreach (var style in config.Styles)
                    {
                        ILayout layout = style.Layout;
                        Type type = layout.GetType();
                        string styleName = style.name;

                        if (_layoutStylesConfigs.ContainsKey(type) && _layoutStylesConfigs[type].ContainsKey(styleName))
                        {
                            string errorMessageHead = "Error! Style repeats!:\n";
                            string errorMessage = "StyleFile: " + fileName + " StyleName: " + styleName +
                                                  "GameObject: " +
                                                  layout.GetGameObject().name + "\n";
                            string errorMessage2 = _layoutStylesConfigs[type][styleName].GetGameObject().name;
                            throw new Exception(errorMessageHead + errorMessage + errorMessage2);
                        }


                        if (!_layoutStylesConfigs.ContainsKey(type))
                        {
                            _layoutStylesConfigs.Add(type, new Dictionary<string, ILayout>());
                        }

                        _layoutStylesConfigs[type][style.name] = style.Layout;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.LogError(config.name);
                    throw;
                }
            }
        }


        // private void InitPools()
        // {
        //
        //     foreach (var layoutKvp in _layoutStylesConfigs)
        //     {
        //         if (!layoutKvp.Value.Styles.Any(x => x.poolSize > 0)) continue;
        //
        //         var layoutType = layoutKvp.Key;
        //
        //         //_pooledLayouts.Add(layoutType, new Dictionary<string, ObjectPool>());
        //
        //         foreach (var style in layoutKvp.Value.Styles)
        //         {
        //             var template = Resources.Load<GameObject>(style.prefabReference.PrefabPathInResources);
        //             var objectPool = ObjectPool.Create(template,
        //                 layoutKvp.Key.Name + " " + style.name, style.poolSize, style.poolSize);
        //
        //             //objectPool.container = _diContainer;
        //             
        //             objectPool.Initialize();
        //             
        //             foreach (var layout in objectPool.ObjectParent.GetComponentsInChildren<Layout>(true))
        //             {
        //                 layout.OnStartDestroy += delegate
        //                 {
        //                     OnPooledLayoutDestroy(layoutKvp.Key, style.name, layout.GetGameObject());
        //                 };
        //
        //                 layout.SetDestroyType(DestroyType.Deactivate);
        //             }
        //
        //             //_pooledLayouts[layoutType].Add(style.name, objectPool);
        //         }
        //     }
        // }

        public T Create<T>() where T : ILayout
        {
            return Create<T>(GetConfigByType<T>().Keys.First());
        }

        public T Create<T>(ILayout prefab) where T : ILayout
        {
            return _creator.InstantiatePrefab(prefab.GetGameObject()).GetComponent<T>();
        }

        public T Create<T>(string styleName) where T : ILayout
        {
            var style = GetConfigByType<T>().Keys.FirstOrDefault(x => x == styleName);

            if (style == null)
            {
                return default;
            }

            var prefabLayout = _layoutStylesConfigs[typeof(T)][styleName];

            //var isPooledLayout = _pooledLayouts.ContainsKey(type);

            // var layout = isPooledLayout
            //     ? _pooledLayouts[type][styleName].GetObject().GetComponent<T>()
            //     : _diContainer.InstantiatePrefabResource(layoutPathInResources).GetComponent<T>();


            return _creator.InstantiatePrefab(prefabLayout.GetGameObject()).GetComponent<T>();
        }

        /*private void OnPooledLayoutDestroy(Type type, string styleName, GameObject layout)
        {
            _pooledLayouts[type][styleName].Release(layout);
        }*/

        private Dictionary<string, ILayout> GetConfigByType<T>()
        {
            return _layoutStylesConfigs[typeof(T)];
        }
    }
}