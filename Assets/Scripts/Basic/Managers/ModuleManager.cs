using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Basic.Managers;
using Basic.Managers.Helper;

namespace Basic.Managers
{
    public class ModuleManager
    {
        public const string ADDITIONALMODULE_ADDED = "ADDITIONALMODULE_ADDED";
        public const string ADDITIONALMODULE_REMOVED = "ADDITIONALMODULE_REMOVED";

        public bool canGotoSameModule = true;
        private BaseModule _currentModule;
        private List<BaseModule> _additionalModules = new List<BaseModule>();
        public BaseModule currentModule
        {
            get
            {
                return _currentModule;
            }
        }
        private static ModuleManager _instance;
        public static ModuleManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModuleManager();
                }
                return _instance;
            }
        }

        public ModuleManager()
        {
            if (_instance != null)
            {
                throw new UnityException("Error: Please use instance to get ModuleManager!");
            }
        }

        public void GotoModule(Type moduleClass, object data = null, Action initComplete = null)
        {
            if (_currentModule != null)
            {
                if (!canGotoSameModule && _currentModule.GetType() == moduleClass)
                {
                    Debug.LogError("Don't goto the same module twice.");
                    return;
                }

                if (_currentModule.isLoading)
                {
                    Debug.LogError("current module is loading");
                    return;
                }
            }

            BaseModule lastModule = _currentModule;
            _currentModule = moduleClass.Assembly.CreateInstance(moduleClass.Name) as BaseModule;
            if (lastModule != null)
            {
                if (lastModule.prepareTime > 0)
                {
                    Engine.Instance.DoAfter(_currentModule.prepareTime, () =>
                    {
                        lastModule.Destroy();
                        lastModule = null;
                    });
                }
                else
                {
                    lastModule.Destroy();
                    lastModule = null;
                }
            }
            _currentModule.Init(data, initComplete);
        }

        public T FindAdditionalModule<T>() where T : BaseModule
        {
            return (T)_additionalModules.Find(m => m.GetType() == typeof(T));
        }

        public bool ExistAdditionalModule(Type moduleClass)
        {
            return _additionalModules.Exists(m => m.GetType() == moduleClass);
        }

        public void AddAdditionalModule(Type moduleClass, object data = null)
        {
            if (_additionalModules.Exists(m => m.GetType() == moduleClass))
            {
                Debug.LogError("Don't add the same addtional module twice.");
            }
            else
            {
                BaseModule module = moduleClass.Assembly.CreateInstance(moduleClass.Name) as BaseModule;
                _additionalModules.Add(module);
                module.Init(data, null);

                EventManager.instance.DispatchEvent(this, ModuleManager.ADDITIONALMODULE_ADDED, moduleClass);
            }
        }

        public void RemoveAdditionalModule(Type moduleClass)
        {
            BaseModule module = _additionalModules.Find(m => m.GetType() == moduleClass);
            if (module == null)
            {
                Debug.LogError("Don't remove the not exist addtional module.");
            }
            else
            {
                _additionalModules.Remove(module);
                module.Destroy();

                EventManager.instance.DispatchEvent(this, ModuleManager.ADDITIONALMODULE_REMOVED, moduleClass);
            }
        }

        public void RemoveAllAdditionalModules()
        {
            for (int i = _additionalModules.Count - 1; i >= 0; i--)
            {
                BaseModule module = _additionalModules[i];
                _additionalModules.RemoveAt(i);
                module.Destroy();
            }
        }
    }
}

