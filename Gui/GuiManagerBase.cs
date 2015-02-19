using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HPG.Gui
{
    /// <summary>
    /// Менеджер ГУИ экранов
    /// </summary>
    public abstract class GuiManagerBase
    {
        private class DisplayVO
        {
            public GuiControllerBase Controller { get; internal set; }
            public DisplayMode DisplayMode { get; internal set; }
        }

        private readonly Transform _root;
        private GuiFactory _factory;

        private readonly LinkedList<DisplayVO> _guiStack = new LinkedList<DisplayVO>();
        private GameObject _modal = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root">Рут скриновой системы, трансформ, чайлдами которого бдут все скрины ГУИ менеджера</param>
        protected GuiManagerBase(Transform root)
        {
            _root = root;
        }

        /// <summary>
        /// Инициализирует ГУИ систему
        /// </summary>
        /// <param name="factory">Фабрика объектов ГУИ</param>
        public void Init(GuiFactory factory)
        {
            _factory = factory;
        }
        /// <summary>
        /// Возвращает диалог из стека. Если его не существует, создает
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode"></param>
        /// <returns></returns>
        public virtual T Get<T>(DisplayMode mode = DisplayMode.DialogAddFront) where T : GuiControllerBase
        {
            T controller = GetFromStack<T>();
            if (controller != null)
                return controller;

            return Show<T>(mode);
        }

        /// <summary>
        /// Создает и показывает диалог
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mode"></param>
        /// <returns></returns>
        public virtual T Show<T>(DisplayMode mode = DisplayMode.DialogAddFront) where T : GuiControllerBase
        {
            // если показываем в режиме одного скрина, закрываем имеющийся
            if (mode == DisplayMode.Screen)
                CloseInStack(vo => IsScreen(vo.DisplayMode) || IsDialog(vo.DisplayMode));

            // Надо ли прятать диалог до закрытия другого диалогового окна
            bool enqueue = 
                mode == DisplayMode.DialogQueued && 
                _guiStack.Any(gui => gui.DisplayMode == DisplayMode.DialogQueued);

            // создаем объект сцены и добавляем в массив
            GameObject obj = _factory.CreateGuiObject<T>();
            T controller = obj.GetComponent<T>();
            SetEnvironment(controller);
            ResetRectTransform(obj.transform);

            DisplayVO state = new DisplayVO
                {
                    Controller = controller,
                    DisplayMode = mode
                };

            if (enqueue)
            {
                _guiStack.AddFirst(state);
                controller.gameObject.transform.SetSiblingIndex(0);
                controller.gameObject.transform.position = new Vector3(-100000000, 0, 0);
            }
            else
            {
                DisplayVO vo = null;
                if (_guiStack.Count > 0)
                    vo = _guiStack.FirstOrDefault(gui => gui.DisplayMode == DisplayMode.Persistent);

                if (mode == DisplayMode.Screen && vo != null)
                {
                    _guiStack.AddBefore( _guiStack.Find(vo), state);
                    controller.gameObject.transform.SetSiblingIndex( vo.Controller.gameObject.transform.GetSiblingIndex() );
                }
                else
                    _guiStack.AddLast(state);

                if (IsDialog(mode))
                {
                    CreateModal();
                    UpdateModal();
                }
            }

            // если в режиме закрывания других диалогов, то закрываем все другие диалоги
            if (mode == DisplayMode.DialogCloseOthers)
                CloseInStack(vo => IsDialog(vo.DisplayMode) && vo.Controller != controller);

            return controller;
        }

        /// <summary>
        /// Устанавливает данные в контроллер ГУИ
        /// </summary>
        /// <param name="controller"></param>
        protected abstract void SetEnvironment(GuiControllerBase controller);

        private void CloseInStack(Func<DisplayVO, bool> predicate)
        {
            IEnumerable<DisplayVO> screens =
                new List<DisplayVO>(_guiStack.Where(predicate));
            foreach (DisplayVO screen in screens)
                Close(screen.Controller);
        }

        private T GetFromStack<T>() where T : GuiControllerBase
        {
            foreach (var vo in _guiStack)
            {
                if (vo.Controller is T)
                {
                    return vo.Controller as T;
                }
            }
            return null;
        }

        private bool IsDialog(DisplayMode mode)
        {
            return (mode == DisplayMode.DialogAddFront ||
                    mode == DisplayMode.DialogCloseOthers ||
                    mode == DisplayMode.DialogQueued);
        }

        private bool IsScreen(DisplayMode mode)
        {
            return (mode == DisplayMode.Screen);
        }

        private bool IsQueued(DisplayMode mode)
        {
            return (mode == DisplayMode.DialogQueued);
        }

        private void CreateModal()
        {
            if (_modal != null)
                return;
            _modal = _factory.CreateModalObject();
            if (_modal != null)
                ResetRectTransform(_modal.transform);
        }

        private void UpdateModal()
        {
            if (_modal == null)
                return;
            int index = Mathf.Max(_root.childCount - 2, 0);
            _modal.transform.SetSiblingIndex(index);
        }

        /// <summary>
        /// Закрывает выбранное окно
        /// </summary>
        /// <param name="controller"></param>
        public virtual void Close(GuiControllerBase controller)
        {
            DisplayVO vo = _guiStack.FirstOrDefault(state => state.Controller == controller);
            if (vo == null)
            {
                Debug.LogWarning(string.Format("Cannot close screen {0}. It's not in the GUI stack", controller.GetType()));
                return;
            }

            // открываем отложенные ГУИ
            if (vo.DisplayMode == DisplayMode.DialogQueued)
            {
                DisplayVO enqueued = _guiStack.LastOrDefault(state =>
                    IsQueued(state.DisplayMode) &&
                    state.Controller != controller);
                if (enqueued != null)
                {
                    enqueued.Controller.transform.SetAsLastSibling();
                    ResetRectTransform(enqueued.Controller.transform);
                    enqueued.Controller.gameObject.GetComponent<Animator>().Play(0);
                    _guiStack.Remove(enqueued);
                    _guiStack.AddLast(enqueued);
                }
            }

            // Убираем из списка диалог и уничтожаем объект
            _guiStack.Remove(vo);
            Destroy(vo.Controller);

            // Обновляем или уничтожаем модальный блокер
            if (IsDialog(vo.DisplayMode))
            {
                // Если нет больше диалогов в стеке
                if (_guiStack.Count(displayVO => IsDialog(displayVO.DisplayMode)) <= 0)
                {
                    Object.Destroy(_modal);
                    _modal = null;
                }
                else
                    UpdateModal();
            }

            // проигрываем анимацию рефокуса окна
//            var openedDialogState = _guiStack.LastOrDefault(state =>
//                (state.DisplayMode == DisplayMode.DialogAddFront ||
//                 state.DisplayMode == DisplayMode.DialogCloseOthers ||
//                 state.DisplayMode == DisplayMode.DialogQueued));
//            if (openedDialogState != null)
//                openedDialogState.Controller.gameObject.GetComponent<Animator>().Play(0);
        }

        /// <summary>
        /// Закрывает все объекты ГУИ в руте
        /// </summary>
        public void CloseAll()
        {
            if (_modal != null)
                Object.Destroy(_modal);
            _modal = null;
            foreach (var vo in _guiStack)
                Destroy(vo.Controller);
            _guiStack.Clear();
        }

        private void Destroy(GuiControllerBase controller)
        {
            controller.Dispose();
            controller.gameObject.transform.SetParent(null);
            Object.Destroy(controller.gameObject);
        }

        private void ResetRectTransform(Transform transform)
        {
            RectTransform rect = (RectTransform) transform;
            rect.SetParent(_root);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }
    }
}
