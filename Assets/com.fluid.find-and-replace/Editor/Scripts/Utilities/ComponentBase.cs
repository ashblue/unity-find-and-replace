using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace {
    public abstract class ComponentBase {
        private readonly string _path;
        protected readonly VisualElement _container;

        protected ComponentBase (VisualElement container) {
            _container = container;

            if (_path == null) {
                var path = new System.Diagnostics.StackTrace(true)
                    .GetFrame(1)
                    .GetFileName()
                    ?.Replace("\\", "/");

                if (path.Contains("/PackageCache/")) {
                    var parts = path.Split(new[] { "/Editor/" }, StringSplitOptions.None);
                    _path = $"{AssetPath.BasePath}com.fluid.find-and-replace/Editor/{parts[1]}"
                        .Replace(".cs", ".uxml");
                } else {
                    var strings = path.Split(new string[] { "/Assets/" }, StringSplitOptions.None);
                    _path = $"{AssetPath.BasePath}/{strings[1]}"
                        .Replace(".cs", ".uxml");
                }
            }

            var markup = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_path);
            markup.CloneTree(container);
        }
    }
}
