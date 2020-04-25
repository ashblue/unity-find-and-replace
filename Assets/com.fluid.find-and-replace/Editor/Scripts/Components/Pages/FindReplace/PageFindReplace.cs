using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CleverCrow.Fluid.FindAndReplace.Editors;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace {
    public class PageFindReplace : ComponentBase {
        private List<SearchResult> _results = new List<SearchResult>();
        private Func<string, IFindResult[]> _search;

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

            resultContainer.Clear();
            _results.Clear();
            foreach (var result in _search.Invoke(searchText)) {
                var matches = Regex.Matches(result.Text, $"{searchText}");

                for (var i = 0; i < matches.Count; i++) {
                    _results.Add(new SearchResult(resultContainer, searchText, result, i));
                }
            }
        }

        public void SetSearch (Func<string, IFindResult[]> search) {
            _search = search;
        }
    }
}
