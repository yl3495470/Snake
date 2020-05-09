using UnityEngine;
using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using System;
using UnityEngine.SceneManagement;

namespace Basic.Managers.Helper
{
    public abstract class BaseModule
    {
        public static IFacade facade = Facade.Instance;

        public virtual float prepareTime { get { return 0f; } }
        public virtual bool isLoading { get { return false; } }

        protected Action _initComplete;
		protected AsyncOperation _loadAsyn;
        protected object _data;

        protected string _scene;
        public BaseModule(string scene = null)
        {
            _scene = scene;
        }

        public void Init(object data, Action initComplete = null)
        {
            _initComplete = initComplete;
            _data = data;

            _Init();
        }

        private void _OnSceneLoaded()
        {
            SceneCtrl.OnSceneLoaded -= _OnSceneLoaded;

            _Start();

            if (_initComplete != null)
            {
                _initComplete();
                _initComplete = null;
            }
        }
			
        protected abstract void _Start();

        protected virtual void _Init()
        {
            if (!string.IsNullOrEmpty(_scene) && Application.loadedLevelName != _scene)
            {
                SceneCtrl.OnSceneLoaded += _OnSceneLoaded;
                _LoadLevel();
                //AutoFade.LoadLevel(_scene.ToString(), 2f, 2f, Color.black);
            }
            else
            {
                _OnSceneLoaded();
            }
        }

        protected virtual void _LoadLevel()
        {
			_loadAsyn = SceneManager.LoadSceneAsync (_scene);
		}

        public void Destroy()
        {
            _Dispose();
            _Destroy();
        }

        protected abstract void _Dispose();

        private void _Destroy()
        {
            SceneCtrl.OnSceneLoaded -= _OnSceneLoaded;
            _initComplete = null;
            _data = null;
        }
    }
}