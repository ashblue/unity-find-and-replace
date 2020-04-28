using System;
using System.Collections.Generic;
using CleverCrow.Fluid.FindAndReplace.Editors;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CleverCrow.Fluid.FindAndReplace.Examples {
    public class SearchResult : IFindResult {
        private readonly Object _target;
        public string Text { get; }

        public SearchResult (string text, Object target) {
            _target = target;
            Text = text;
        }

        public void Show () {
            Selection.activeObject = _target;;
        }
    }

    public class SearchExample : FindReplaceWindowBase {
        private static SearchExample _window;

        [MenuItem("Examples/Find Replace")]
        public static void ShowFindReplace () {
            ShowWindow<SearchExample>();
        }

        protected override IFindResult[] GetFindResults (Func<string, bool> IsValid) {
            var results = new List<IFindResult>();

            var textIDs = AssetDatabase.FindAssets("t:TextExample");
            foreach (var textID in textIDs) {
                var path = AssetDatabase.GUIDToAssetPath(textID);
                var textExample = AssetDatabase.LoadAssetAtPath<TextExample>(path);

                if (IsValid(textExample.text)) {
                    results.Add(new SearchResult(textExample.text, textExample));
                }
            }

            return results.ToArray();
        }
    }
}
