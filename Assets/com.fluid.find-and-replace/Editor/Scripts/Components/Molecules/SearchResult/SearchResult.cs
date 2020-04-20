using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class SearchResult : ComponentBase {
        public SearchResult (VisualElement container) : base(container) {
            var el = container.GetElement<TextElement>("m-search-result__text");
            el.text = "Lorem";
        }
    }
}
