using System;
using NUnit.Framework;
using NSubstitute;
using UnityEngine.UIElements;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
    public class FindReplaceWindowTest {
        public class FindReplaceWindowStub : FindReplaceWindowBase {
            public string searchText;
            public IFindResult[] result;

            protected override IFindResult[] GetFindResults (Func<string, bool> isValid) {
                if (isValid(searchText)) {
                    return result;
                }

                return null;
            }
        }

        public class ClickingFind {
            private VisualElement Setup (string searchText, IFindResult[] result, bool matchCase = true) {
                FindReplaceWindowBase.CloseWindow();

                var window = FindReplaceWindowBase.ShowWindow<FindReplaceWindowStub>();
                window.searchText = searchText;
                window.result = result;

                var root = window.rootVisualElement;

                var searchInput = root.GetElement<TextField>("p-window__input-find-text");
                searchInput.value = searchText;

                var elMatchCase = root.GetElement<Toggle>("p-window__match-case");
                elMatchCase.value = matchCase;

                root.ClickButton("p-window__search");

                return root;
            }

            [TearDown]
            public void AfterEach () {
                FindReplaceWindowBase.CloseWindow();
            }

            private Func<Func<string, bool>, IFindResult[]> Search (string searchText, IFindResult[] result) {
                return (isValid) => {
                    if (isValid(searchText)) return result;
                    return null;
                };
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

            public class EmptySearches : ClickingFind {
                [Test]
                public void It_should_not_display_any_results () {
                    var root = Setup("", new IFindResult[0]);

                    var result = root.Query<VisualElement>(null, "m-search-result");
                    Assert.AreEqual(0, result.ToList().Count);
                }

                [Test]
                public void It_should_not_display_a_message_nothing_was_found_by_default () {
                    FindReplaceWindowBase.CloseWindow();

                    var root = FindReplaceWindowBase
                        .ShowWindow<FindReplaceWindowStub>()
                        .rootVisualElement;
                    var result = root.GetElement<TextElement>("p-window__no-result");

                    Assert.IsTrue(result.ClassListContains("hide"));
                }

                [Test]
                public void It_should_display_a_message_nothing_was_found () {
                    var root = Setup("", new IFindResult[0]);
                    var result = root.GetElement<TextElement>("p-window__no-result");

                    Assert.IsFalse(result.ClassListContains("hide"));
                }

                [Test]
                public void It_should_clear_not_found_message_when_searching_again () {
                    Setup("", new IFindResult[0]);
                    FindReplaceWindowBase.CloseWindow();

                    var findResult = Substitute.For<IFindResult>();
                    findResult.Text.Returns("Lorem Ipsum");
                    var root = Setup("Lorem", new [] {findResult});

                    var result = root.GetElement<TextElement>("p-window__no-result");

                    Assert.IsTrue(result.ClassListContains("hide"));
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

                public class WhenStrictCase : HandlingLongStrings {
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
                }

                public class WhenAnyCase : HandlingLongStrings {
                    [Test]
                    public void It_should_show_correct_preview_text () {
                        var root = SetupStrings($"{RESULT_TEXT} lorem", false);
                        var result = root.GetText("m-search-result__text");

                        Assert.IsTrue(result.Contains("lorem"));
                    }

                    [Test]
                    public void It_should_show_the_correct_preview_for_multiple_results () {
                        var root = SetupStrings($"Lorem {RESULT_TEXT} lorem", false);
                        var result = root.GetText("m-search-result__text");

                        Assert.IsTrue(result.Contains("Lorem"));
                    }
                }
            }
        }
    }
}
