using System;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class FindReplaceWindowTest {
        public class ClickingFind {
            private VisualElement Setup (string searchText, IFindResult[] result, bool matchCase = true) {
                FindReplaceWindow.CloseWindow();

                // @TODO This should probably be a constructed object for sanity reasons
                var search = Substitute.For<Func<string, IFindResult[]>>();
                search(searchText).Returns(result);
                if (!matchCase) {
                    search(searchText.ToLower()).Returns(result);
                }

                var root = FindReplaceWindow.ShowWindow(search).rootVisualElement;

                var searchInput = root.GetElement<TextField>("p-window__input-find-text");
                searchInput.value = searchText;

                var elMatchCase = root.GetElement<Toggle>("p-window__match-case");
                elMatchCase.value = matchCase;

                root.ClickButton("p-window__search");

                return root;
            }

            [TearDown]
            public void AfterEach () {
                FindReplaceWindow.CloseWindow();
            }

            public class Defaults : ClickingFind {
                [Test]
                public void It_should_find_given_text_and_display_results () {
                    const string searchText = "Lorem";
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns(searchText);

                    var root = Setup(searchText, new[] {findResult});
                    var result = root.GetText("m-search-result__text");

                    Assert.AreEqual(searchText, result);
                }

                [Test]
                public void It_should_clear_previous_results_with_additional_searches () {
                    const string searchText = "Lorem";
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns(searchText);

                    var root = Setup(searchText, new[] {findResult});
                    var result = root.Query<VisualElement>(null, "m-search-result");

                    Assert.AreEqual(1, result.ToList().Count);
                }

                [Test]
                public void It_should_not_display_any_results_if_there_is_no_search_results () {
                    var root = Setup("", new IFindResult[0]);

                    var result = root.Query<VisualElement>(null, "m-search-result");
                    Assert.AreEqual(0, result.ToList().Count);
                }

                [Test]
                public void It_should_display_multiple_results_for_strings_with_multiple_keywords () {
                    const string searchText = "Lorem";
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns("Lorem Lorem");

                    var root = Setup(searchText, new[] {findResult});
                    var results = root.Query<VisualElement>(null, "m-search-result");

                    Assert.AreEqual(2, results.ToList().Count);
                }

                [Test]
                public void It_should_run_the_show_logic_when_clicking_Show_on_a_result () {
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns("Lorem Ipsum");

                    var root = Setup("Lorem", new[] {findResult});
                    root.ClickButton("m-search-result__show");

                    findResult.Received(1).Show();
                }
            }

            public class WithNonCaseSensitiveSearch : ClickingFind {
                [Test]
                public void It_should_match_non_matching_case () {
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns("Lorem ipsum");

                    var root = Setup("lorem", new[] {findResult}, false);
                    var result = root.GetElement<VisualElement>("m-search-result");

                    Assert.IsNotNull(result);
                }
            }

            public class HandlingLongStrings : ClickingFind {
                private const string SEARCH_TEXT = "Lorem";
                private const string RESULT_TEXT =
                    "Chantey topmast shrouds carouser landlubber or just lubber spirits reef sails Spanish Main loot draft. American Main spyglass pillage driver bowsprit trysail splice the main brace killick loaded to the gunwalls Gold Road. Rigging furl coffer jack tackle rutters crimp spirits pink jib";

                private VisualElement SetupStrings (string resultText, bool matchCase = true) {
                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns(resultText);

                    return Setup(SEARCH_TEXT, new[] {findResult}, matchCase);
                }

                [Test]
                public void It_should_print_result_text_a_maximum_of_60_characters () {
                    var words = $"Lorem {RESULT_TEXT}";

                    var root = SetupStrings(words);
                    var result = root.GetText("m-search-result__text");

                    Assert.AreEqual($"{words.Substring(0, 60)}", result);
                }

                [Test]
                public void It_should_show_the_2nd_of_multiple_matches_with_the_correct_preview () {
                    var words = $"{SEARCH_TEXT} {RESULT_TEXT} {SEARCH_TEXT}";

                    var root = SetupStrings(words);
                    var result = root
                        .Query<TextElement>(null, "m-search-result__text")
                        .Last()
                        .text;

                    Assert.AreNotEqual(result.IndexOf(SEARCH_TEXT, StringComparison.Ordinal), 0);
                }

                [Test]
                public void It_should_keep_the_search_keyword_within_view () {
                    var root = SetupStrings($"{RESULT_TEXT} Lorem");
                    var result = root.GetText("m-search-result__text");

                    Assert.IsTrue(result.Contains(SEARCH_TEXT));
                }

                [Test]
                public void It_should_add_ellipses_to_nested_search_text_results () {
                    var root = SetupStrings($"{RESULT_TEXT} Lorem");
                    var result = root.GetText("m-search-result__text");

                    Assert.IsTrue(result.Contains("..."));
                }

                [Test]
                public void It_should_show_correct_preview_text_when_any_case () {
                    var root = SetupStrings($"{RESULT_TEXT} lorem", false);
                    var result = root.GetText("m-search-result__text");

                    Assert.IsTrue(result.Contains("lorem"));
                }
            }
        }
    }
}
