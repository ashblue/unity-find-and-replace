using System;

namespace CleverCrow.Fluid.FindAndReplace.Editors {
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
}
