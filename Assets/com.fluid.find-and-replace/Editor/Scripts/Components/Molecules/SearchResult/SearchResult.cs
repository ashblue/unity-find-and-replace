using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class SearchResult : ComponentBase {
        public SearchResult (VisualElement container, string search, IFindResult result) : base(container) {
            var el = container.GetElement<TextElement>("m-search-result__text");
            var searchWordIndex = Mathf.Max(result.Text.IndexOf(search, StringComparison.Ordinal) - 15, 0);
            var maxLength = Mathf.Min(60, result.Text.Length - searchWordIndex);

            var preText = "";
            if (searchWordIndex > 0) {
                preText = "...";
            }

            el.text = preText + result.Text.Substring(searchWordIndex, maxLength);
        }
    }
}
