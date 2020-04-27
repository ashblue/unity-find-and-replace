using UnityEngine;

namespace CleverCrow.Fluid.FindAndReplace.Examples {
    [CreateAssetMenu(fileName = "TextExample", menuName = "Examples/Text")]
    public class TextExample : ScriptableObject {
        [TextArea]
        public string text;
    }
}
