using System;
using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public interface IFindResult {
        string Text { get; }
        void Show ();
        void Replace (int startIndex, int charactersToReplace, string replaceText);
    }

    public abstract class FindReplaceWindowBase : EditorWindow {
        private static EditorWindow _window;

        public static T ShowWindow<T> () where T : EditorWindow {
            var window = GetWindow<T>();
            _window = window;
            _window.titleContent = new GUIContent("Find Replace");

            return window;
        }

        protected abstract IFindResult[] GetFindResults (Func<string, bool> IsValid);

        public static void CloseWindow () {
            if (_window == null) return;
            _window.Close();
        }

        private void OnEnable () {
            var root = rootVisualElement;
            var page = new PageFindReplace(root);
            page.SetSearch(GetFindResults);
        }
    }
}
