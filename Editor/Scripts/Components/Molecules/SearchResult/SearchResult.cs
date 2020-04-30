using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class SearchResult : ComponentBase {
        private readonly IFindResult _result;
        private readonly int _wordStartIndex;
        private readonly string _search;
        private readonly Button _elReplace;

        private class SearchResultEvent : UnityEvent<SearchResult> {}
        public UnityEvent<SearchResult> OnClickReplace { get; } = new SearchResultEvent();

        public SearchResult (
            VisualElement container,
            string search,
            IFindResult result,
            int wordMatchIndex,
            bool matchCase
        ) : base(container) {
            _search = search;
            _result = result;

            var resultText = result.Text;
            if (!matchCase) resultText = resultText.ToLower();
            _wordStartIndex = resultText.IndexOf(search, wordMatchIndex, StringComparison.Ordinal);

            var searchWordIndex = Mathf.Max(_wordStartIndex - 15, 0);
            var maxLength = Mathf.Min(60, result.Text.Length - searchWordIndex);

            var preText = "";
            if (searchWordIndex > 0) {
                preText = "...";
            }

            var elText = container.GetElementLast<TextElement>("m-search-result__text");
            elText.text = preText + result.Text.Substring(searchWordIndex, maxLength);

            var elShow = container.GetElementLast<Button>("m-search-result__show");
            elShow.clicked += result.Show;

            _elReplace = container.GetElementLast<Button>("m-search-result__replace");
            _elReplace.clicked += () => {
                OnClickReplace.Invoke(this);
            };
        }

        public void ReplaceText (string replaceText) {
            _result.Replace(_wordStartIndex, _search.Length, replaceText);

            _elReplace.AddToClassList("hide");
        }
    }
}
