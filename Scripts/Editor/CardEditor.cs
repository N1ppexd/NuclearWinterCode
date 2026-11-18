using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace NuclearWinter
{

    public class CardEditor : OdinMenuEditorWindow
    {
        [MenuItem("Niilo's Tools/Card Settings")]
        private static void OpenWindow()
        {
            EditorWindow.GetWindow<CardEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.AddAllAssetsAtPath("Item Cards", "Assets/ScriptableObjects/Cards/ItemCards", typeof(CardSO));
            tree.AddAllAssetsAtPath("Event Cards", "Assets/ScriptableObjects/Cards/EventCards", typeof(CardSO));
            tree.AddAllAssetsAtPath("Event Actions", "Assets/ScriptableObjects/Cards/EventAction", typeof(ActionSO));

            return tree;
        }
    }
}
