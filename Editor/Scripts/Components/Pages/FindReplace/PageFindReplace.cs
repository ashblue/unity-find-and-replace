using System.Collections.Generic;
using CleverCrow.Fluid.FindAndReplace.Editors;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace {
    public class PageFindReplace : ComponentBase {
        private List<SearchResult> _results = new List<SearchResult>();

        public PageFindReplace (VisualElement container) : base(container) {
            var resultsEl = container.GetElement<VisualElement>("p-window__results");
            _results.Add(new SearchResult(resultsEl));
        }
    }
}
