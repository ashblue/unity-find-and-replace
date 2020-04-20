using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public static class UiHelpers {
        public static void ClickButton (this VisualElement root, string className) {
            var elView = root.GetElement<Button>(className);
            var viewClick = elView.clickable;
            var viewInvoke = viewClick
                .GetType()
                .GetMethod(
                    "Invoke",
                    BindingFlags.NonPublic | BindingFlags.Instance
                );

            viewInvoke?.Invoke(viewClick, new object[] { MouseDownEvent.GetPooled() });
        }

        public static string GetText (this VisualElement root, string className) {
            return root
                .GetElement<TextElement>(className)
                .text;
        }

        public static T GetElement<T> (this VisualElement root, string className) where T : VisualElement {
            var el = root
                .Query<T>(null, className)
                .First();

            Debug.Assert(el != null, $"Element {className} not found");

            return el;
        }
    }
}
