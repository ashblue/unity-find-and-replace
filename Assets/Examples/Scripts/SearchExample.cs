using System;
using System.Collections.Generic;
using CleverCrow.Fluid.FindAndReplace.Editors;
using UnityEditor;
using UnityEngine;

namespace CleverCrow.Fluid.FindAndReplace.Examples {
    public class SearchResult : IFindResult {
        public string Text { get; }
        public void Show () {
            throw new NotImplementedException();
        }

        public SearchResult (string text) {
            Text = text;
        }
    }

    public class SearchExample {
        [MenuItem("Examples/Find Replace")]
        public static void ShowFindReplace () {
            FindReplaceWindow.ShowWindow(GetFindResults);
        }

        private static IFindResult[] GetFindResults (Func<string, bool> IsValid) {
            var results = new List<IFindResult>();

            var textIDs = AssetDatabase.FindAssets("t:TextExample");
            foreach (var textID in textIDs) {
                var path = AssetDatabase.GUIDToAssetPath(textID);
                var textExample = AssetDatabase.LoadAssetAtPath<TextExample>(path);

                if (IsValid(textExample.text)) {
                    results.Add(new SearchResult(textExample.text));
                }
            }

            return results.ToArray();
        }
    }
}
