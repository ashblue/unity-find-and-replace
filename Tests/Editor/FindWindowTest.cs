using System;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class FindReplaceWindowTest {
        public class ClickingFind {
            private FindReplaceWindow _window;

            [Test]
            public void It_should_find_given_text_and_display_results () {
                FindReplaceWindow.CloseWindow();
                const string searchText = "Lorem";

                var findResult = Substitute.For<IFindResult>();
                findResult.Text.Returns(searchText);

                // @TODO This should probably be a constructed object for sanity reasons
                var search = Substitute.For<Func<string, IFindResult[]>>();
                search(searchText).Returns(new [] { findResult });

                _window = FindReplaceWindow.ShowWindow(search);
                var root = _window.rootVisualElement;
                var searchInput = root.GetElement<TextField>("p-window__input-find-text");

                searchInput.value = searchText;
                root.ClickButton("p-window__search");

                var result = root.GetText("m-search-result__text");
                Assert.AreEqual(searchText, result);
            }

            public void It_should_not_display_any_results_if_there_is_no_search () {

            }

            public void It_should_display_multiple_results () {

            }
        }
    }
}
