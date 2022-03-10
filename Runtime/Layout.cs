using System;
using System.Collections.Generic;
using System.Linq;
using LittleBit.Modules.CoreModule.MonoInterfaces;
using UnityEngine;

namespace LittleBit.Modules.UI
{
    public class Layout : MonoBehaviour, ILayout
    {
        public event Action<IDestroyable> Destroyed;
        public event Action OnStartDestroy;
        
        protected Layout()
        {
            _elements = new List<ILayout>();
        }

        private List<ILayout> _elements;
        private DestroyType _destroyType;
        private ILayout Parent { get; set; }

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out ILayout layout))
                {
                    AppendChildElement(layout);
                }
            }
        }

        public virtual void AppendChildElement(ILayout layout)
        {
            if (!_elements.Contains(layout))
            {
                _elements.Add(layout);
                layout.GetTransform().SetParent(transform, false);
                layout.SetParent(this);
            }
        }

        public void RemoveElement(ILayout layout)
        {
            _elements.Remove(layout);
        }

        public void SetParent(ILayout layout)
        {
            Parent = layout;
        }

        public ILayout GetChildElementLayout(int index)
        {
            return _elements[index];
        }

        public List<ILayout> GetAllChildrenElements()
        {
            return _elements.ToList();
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public virtual void Destroy()
        {
            while (_elements.Count > 0)
            {
                var element = _elements.Last();
                element.Destroy();
            }
            
            OnStartDestroy?.Invoke();

            Parent?.RemoveElement(this);

            

            switch (_destroyType)
            {
                case DestroyType.Deactivate:
                    gameObject.SetActive(false);
                    break;
                case DestroyType.Destroy:
                    GameObject.Destroy(gameObject);
                    break;
            }
        }

        public void SetDestroyType(DestroyType destroyType)
        {
            _destroyType = destroyType;
        }

        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }

        public void SetGameObjectActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ShowChildElements()
        {
            foreach (var element in _elements)
            {
                element.SetGameObjectActive(true);
            }
        }

        public void HideChildElements()
        {
            foreach (var element in _elements)
            {
                element.SetGameObjectActive(false);
            }
        }

        public T GetLayoutInChildren<T>() where T : Layout
        {
            return gameObject.GetComponentInChildren<T>(true);
        }
    }
}