using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class SearchResult : ComponentBase {
        public SearchResult (
            VisualElement container,
            string search,
            IFindResult result,
            int index,
            bool matchCase
        ) : base(container) {
            var resultText = result.Text;
            if (!matchCase) resultText = resultText.ToLower();

            var searchWordIndex = Mathf.Max(resultText.IndexOf(search, index, StringComparison.Ordinal) - 15, 0);
            var maxLength = Mathf.Min(60, result.Text.Length - searchWordIndex);

            var preText = "";
            if (searchWordIndex > 0) {
                preText = "...";
            }

            var elText = container.GetElementLast<TextElement>("m-search-result__text");
            elText.text = preText + result.Text.Substring(searchWordIndex, maxLength);

            var elButton = container.GetElementLast<Button>("m-search-result__show");
            elButton.clicked += result.Show;
        }
    }
}
