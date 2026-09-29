using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
using UnityEditor;
using UnityEditor.Rendering;

namespace Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes
{
    [CustomEditor(typeof(AttackStatsSO))]
    public class AttackSoDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("knockback"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseAttackSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AttackPreparationTime"));

            SerializedProperty lPatern = serializedObject.FindProperty("paternEnum");
            EditorGUILayout.PropertyField(lPatern);
            


            ScanPaternEnum lPaternEnum = (ScanPaternEnum)lPatern.GetEnumValue<ScanPaternEnum>();

            DrawAdditionalInfo(lPaternEnum);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRange"));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAdditionalInfo(ScanPaternEnum lPatern)
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            switch (lPatern)
            {
                case ScanPaternEnum.Target:
                    EditorGUILayout.LabelField("Tape en mono cible");
                    break;
                case ScanPaternEnum.Box:
                    EditorGUILayout.LabelField("Tape dans une box instancié en direction de l'ennemie");
                    EditorGUILayout.LabelField("width = épaisseur de l'attaque", EditorStyles.toolbarTextField);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("width"));
                    break;
                case ScanPaternEnum.Sphere:
                    EditorGUILayout.LabelField("Tape dans une sphere autour du joueur");
                    break;
                case ScanPaternEnum.Cone:
                    EditorGUILayout.LabelField("Tape dans une sphere autour du joueur");
                    EditorGUILayout.LabelField("angle = max angle du cone (180 = une demi sphere)", EditorStyles.toolbarTextField);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("angle"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("toTarget"));
                    break;
                case ScanPaternEnum.Ray:
                    EditorGUILayout.LabelField("Tire un laser qui traverse les enemies");
                    EditorGUILayout.LabelField("max Range = range du laser qui part du player", EditorStyles.toolbarTextField);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRange"));
                    EditorGUILayout.LabelField("range = range de détection pour lancer l'attaque", EditorStyles.toolbarTextField);
                    break;
                case ScanPaternEnum.CustomObject:
                    EditorGUILayout.LabelField("Tire un laser qui traverse les enemies");
                    EditorGUILayout.LabelField("custom Attack = object à instancier", EditorStyles.toolbarTextField);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("customAttack"));
                    EditorGUILayout.LabelField("isInstanciateOnSelf : si faux instancie sur la target", EditorStyles.toolbarTextField);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("isInstanciateOnSelf"));
                    EditorGUILayout.LabelField("range = range de détection pour lancer l'attaque", EditorStyles.toolbarTextField);
                    break;
            }
        }
    } 
}
