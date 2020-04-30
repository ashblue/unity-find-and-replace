using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CleverCrow.Fluid.FindAndReplace.Editors;
using UnityEngine;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace {
    public class PageFindReplace : ComponentBase {
        private readonly List<SearchResult> _results = new List<SearchResult>();
        private Func<Func<string, bool>, IFindResult[]> _search;

        public PageFindReplace (VisualElement container) : base(container) {
            BindClickFind();
        }

        private void BindClickFind () {
            var btn = _container.GetElement<Button>("p-window__search");
            btn.clicked += ClickFindButton;
        }

        private void ClickFindButton () {
            var searchText = _container.GetElement<TextField>("p-window__input-find-text").value;
            var resultContainer = _container.GetElement<VisualElement>("p-window__results");
            var notFoundMessage = _container.GetElement<VisualElement>("p-window__no-result");

            var matchCase = _container.GetElement<Toggle>("p-window__match-case");
            if (!matchCase.value) searchText = searchText.ToLower();

            resultContainer.Clear();
            _results.Clear();

            Debug.Assert(_search != null, "Please reload the search window");
            if (!string.IsNullOrEmpty(searchText)) {
                AddResults(searchText, matchCase, resultContainer);
            }

            notFoundMessage.RemoveFromClassList("hide");
            if (_results.Count != 0) {
                notFoundMessage.AddToClassList("hide");
            }
        }

        private void AddResults (string searchText, Toggle matchCase, VisualElement resultContainer) {
            foreach (var result in _search.Invoke(IsValid(searchText, matchCase.value))) {
                var text = result.Text;
                if (!matchCase.value) {
                    text = text.ToLower();
                }

                var matches = Regex.Matches(text, $"{searchText}");
                for (var i = 0; i < matches.Count; i++) {
                    var resultElement = new SearchResult(resultContainer, searchText, result, i, matchCase.value);
                    resultElement.OnClickReplace.AddListener(ClickReplace);
                    _results.Add(resultElement);
                }
            }
        }

        private void ClickReplace (SearchResult result) {
            var replaceText = _container
                .GetElement<TextField>("p-window__input-replace-text").value;

            result.ReplaceText(replaceText);
        }

        private Func<string, bool> IsValid (string searchText, bool matchCase) {
            return (text) => {
                var textFilter = text;
                if (!matchCase) {
                    textFilter = text.ToLower();
                }

                return textFilter.Contains(searchText);
            };
        }

        public void SetSearch (Func<Func<string, bool>, IFindResult[]> search) {
            _search = search;
        }
    }
}
