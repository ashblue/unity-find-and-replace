using System;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class FindReplaceWindowTest {
        public class ClickingFind {
            private VisualElement Setup (string searchText, IFindResult[] result) {
                FindReplaceWindow.CloseWindow();

                // @TODO This should probably be a constructed object for sanity reasons
                var search = Substitute.For<Func<string, IFindResult[]>>();
                search(searchText).Returns(result);

                var root = FindReplaceWindow.ShowWindow(search).rootVisualElement;
                var searchInput = root.GetElement<TextField>("p-window__input-find-text");
                searchInput.value = searchText;
                root.ClickButton("p-window__search");

                return root;
            }

            public class Defaults : ClickingFind {
                [Test]
                public void It_should_find_given_text_and_display_results () {
                    const string searchText = "Lorem";
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns(searchText);

                    var root = Setup(searchText, new[] {findResult});
                    var searchInput = root.GetElement<TextField>("p-window__input-find-text");
                    searchInput.value = searchText;
                    root.ClickButton("p-window__search");

                    var result = root.GetText("m-search-result__text");
                    Assert.AreEqual(searchText, result);
                }

                [Test]
                public void It_should_clear_previous_results_with_additional_searches () {
                    const string searchText = "Lorem";
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns(searchText);

                    var root = Setup(searchText, new[] {findResult});
                    root.ClickButton("p-window__search");

                    var result = root.Query<VisualElement>(null, "m-search-result__text");
                    Assert.AreEqual(1, result.ToList().Count);
                }

                [Test]
                public void It_should_not_display_any_results_if_there_is_no_search_results () {
                    var root = Setup("", new IFindResult[0]);

                    root.ClickButton("p-window__search");

                    var result = root.Query<VisualElement>(null, "m-search-result__text");
                    Assert.AreEqual(0, result.ToList().Count);
                }
            }

            public class HandlingLongStrings : ClickingFind {
                private const string SEARCH_TEXT = "Lorem";
                private const string RESULT_TEXT =
                    "Chantey topmast shrouds carouser landlubber or just lubber spirits reef sails Spanish Main loot draft. American Main spyglass pillage driver bowsprit trysail splice the main brace killick loaded to the gunwalls Gold Road. Rigging furl coffer jack tackle rutters crimp spirits pink jib";

                private string SetupStrings (string resultText) {
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns(resultText);

                    var root = Setup(SEARCH_TEXT, new[] {findResult});
                    return root.GetText("m-search-result__text");
                }

                [Test]
                public void It_should_print_result_text_a_maximum_of_60_characters () {
                    var words = $"Lorem {RESULT_TEXT}";
                    var result = SetupStrings(words);

                    Assert.AreEqual($"{words.Substring(0, 60)}", result);
                }

                [Test]
                public void It_should_keep_the_search_keyword_within_view () {
                    var result = SetupStrings($"{RESULT_TEXT} Lorem");

                    Assert.IsTrue(result.Contains(SEARCH_TEXT));
                }

                [Test]
                public void It_should_add_ellipses_to_nested_search_text_results () {
                    var result = SetupStrings($"{RESULT_TEXT} Lorem");

                    Assert.IsTrue(result.Contains("..."));
                }
            }
        }
    }
}
