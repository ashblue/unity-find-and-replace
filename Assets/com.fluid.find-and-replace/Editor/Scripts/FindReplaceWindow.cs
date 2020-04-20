using System;
using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public interface IFindResult {
        string Text { get; }
    }

    public class FindReplaceWindow : EditorWindow {
        private static FindReplaceWindow _window;
        private PageFindReplace _page;

        public static FindReplaceWindow ShowWindow (Func<string, IFindResult[]> search) {
            _window = GetWindow<FindReplaceWindow>();
            _window.titleContent = new GUIContent("Find Replace");

            return _window;
        }

        public static void CloseWindow () {
            if (_window == null) return;
            _window.Close();
        }

        private void OnEnable () {
            var root = rootVisualElement;
            _page = new PageFindReplace(root);
        }
    }
}
