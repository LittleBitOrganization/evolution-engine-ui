using System;
using System.Collections.Generic;
using System.Linq;
using LittleBit.Modules.CoreModule.AssetManagement;
using LittleBit.Modules.CoreModule.MonoInterfaces;
using QFSW.MOP2;
using UnityEngine;

namespace LittleBit.Modules.UI
{
    public class LayoutFactory
    {
        private readonly ICreator _creator;
        private AssetProvider _assetProvider;

        private Dictionary<Type, Dictionary<string, ILayout>> _layoutContainer;
        private Dictionary<Type, Dictionary<string, ObjectPool>> _pooledLayouts;

        private const string PathToStylesConfigs = "Configs/Layout Styles Configs";

        public LayoutFactory(ICreator creator, AssetProvider assetProvider)
        {
            _layoutContainer = new Dictionary<Type, Dictionary<string, ILayout>>();
            _pooledLayouts = new Dictionary<Type, Dictionary<string, ObjectPool>>();


            _creator = creator;
            _assetProvider = assetProvider;

            LoadConfigs();
            InitPools();
        }

        public void Initialize()
        {
            InitPools();
        }

        private LayoutStylesConfig[] _configs;

        private void LoadConfigs()
        {
            _layoutContainer = new Dictionary<Type, Dictionary<string, ILayout>>();
            _configs = _assetProvider.GetAssets<LayoutStylesConfig>(PathToStylesConfigs);

            foreach (var config in _configs)
            {
                string fileName = config.name;
                try
                {
                    foreach (var style in config.Styles)
                    {
                        ILayout layout = style.Layout;
                        Type type = layout.GetType();
                        string styleName = style.name;

                        if (_layoutContainer.ContainsKey(type) && _layoutContainer[type].ContainsKey(styleName))
                        {
                            string errorMessageHead = "Error! Style repeats!:\n";
                            string errorMessage = "StyleFile: " + fileName + " StyleName: " + styleName +
                                                  "GameObject: " +
                                                  layout.GetGameObject().name + "\n";
                            string errorMessage2 = _layoutContainer[type][styleName].GetGameObject().name;
                            throw new Exception(errorMessageHead + errorMessage + errorMessage2);
                        }


                        if (!_layoutContainer.ContainsKey(type))
                        {
                            _layoutContainer.Add(type, new Dictionary<string, ILayout>());
                        }

                        _layoutContainer[type][style.name] = style.Layout;
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


        private void InitPools()
        {
            foreach (var config in _configs)
            {
                string fileName = config.name;
                try
                {
                    foreach (var style in config.Styles)
                    {
                        int poolSize = style.poolSize;
                        if (poolSize > 0)
                        {
                            ILayout layout = style.Layout;
                            Type type = layout.GetType();
                            string styleName = style.name;
                            
                            if (_pooledLayouts.ContainsKey(type) && _pooledLayouts[type].ContainsKey(styleName))
                            {
                                string errorMessageHead = "Error! Style repeats!:\n";
                                string errorMessage = "StyleFile: " + fileName + " StyleName: " + styleName +
                                                      "GameObject: " +
                                                      layout.GetGameObject().name + "\n";
                                string errorMessage2 = _pooledLayouts[type][styleName].GetObject().name;
                                throw new Exception(errorMessageHead + errorMessage + errorMessage2);
                            }
                            
                            if (!_pooledLayouts.ContainsKey(type))
                            {
                                _pooledLayouts.Add(type, new Dictionary<string, ObjectPool>());
                            }

                            var objectPool = ObjectPool.Create(style.Layout.GetGameObject(),
                                style.Layout.GetGameObject().name, poolSize);
                            objectPool.Initialize();
                            
                            foreach (var layoutPooled in objectPool.ObjectParent.GetComponentsInChildren<Layout>(true))
                            {
                                layoutPooled.OnStartDestroy += delegate
                                {
                                    OnPooledLayoutDestroy(layoutPooled.GetType(), styleName, layoutPooled.GetGameObject());
                                };
                            
                                layoutPooled.SetDestroyType(DestroyType.Deactivate);
                            }
                            
                            _pooledLayouts[type].Add(styleName, objectPool);

                        }
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

            Type type = typeof(T);

            var prefabLayout = _layoutContainer[type][styleName];
            var isPooledLayout = _pooledLayouts.ContainsKey(type);


            if (isPooledLayout)
            {
                return _pooledLayouts[type][styleName].GetObject().GetComponent<T>();
            }
            else
            {
                return _creator.InstantiatePrefab(prefabLayout.GetGameObject()).GetComponent<T>();
            }
        }

        private void OnPooledLayoutDestroy(Type type, string styleName, GameObject layout)
        {
            _pooledLayouts[type][styleName].Release(layout);
        }

        private Dictionary<string, ILayout> GetConfigByType<T>()
        {
            return _layoutContainer[typeof(T)];
        }
    }
}