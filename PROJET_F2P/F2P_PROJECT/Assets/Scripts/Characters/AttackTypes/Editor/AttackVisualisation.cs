//using Com.IsartDigital.HealerSurvivor.AttackTypes.Targeting;
//using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
//using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Shared;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class AttackVisualisation : EditorWindow
//{
//    Queue<AttackDrawerStruct> attackToVisualize = new();
//    private const string WINDOW_NAME = "Attack";

//    //[MenuItem("Tools/" + WINDOW_NAME)]
//    public static void ShowExample()
//    {
//        AttackVisualisation lWindow = GetWindow<AttackVisualisation>();
//        lWindow.titleContent = new GUIContent(WINDOW_NAME);
//    }

//    private void OnEnable()
//    {
//        AttackType.LaunchAttack += AddAttack;
//        SceneView.duringSceneGui += Drawing;
//    }

//    private void OnDisable()
//    {
//        AttackType.LaunchAttack -= AddAttack;
//        SceneView.duringSceneGui -= Drawing;
//    }

//    public void AddAttack(AttackStatsSO pAttack, Transform pTransform, float pRange, Vector3 pDirection)
//    {
//        AttackDrawerStruct lAttack = new(pAttack, pTransform, pRange, pDirection);
//        attackToVisualize.Enqueue(lAttack);
//    }

//    public void Drawing(SceneView pView)
//    {
//        while (attackToVisualize.Count > 0)
//        {
//            AttackDrawerStruct lAttack = attackToVisualize.Dequeue();
//            DrawCorrect(lAttack.attack, lAttack.transform, lAttack.range, lAttack.direction);
//            pView.Repaint();
//        }
//    }

//    private void DrawCorrect(AttackStatsSO pAttackStats, Transform pTransform, float pRange,  Vector3 pDirection)
//    {
//        switch (pAttackStats.paternEnum)
//        {
//            case ScanPaternEnum.Cone:
//                TargetScan.DrawCone(pTransform.position, pDirection, pAttackStats.angle, pRange);
//                break;
//            case ScanPaternEnum.Box:
//                TargetScan.DrawBox(pTransform.position, pDirection, pRange, pAttackStats.width);
//                break;
//            case ScanPaternEnum.Ray:
//                TargetScan.DrawRay(pTransform.position, pDirection, pRange);
//                break;
//            case ScanPaternEnum.Sphere:
//                TargetScan.DrawCircle(pTransform.position, pRange);
//                break;
//        }
//    }

//    public struct AttackDrawerStruct
//    {
//        public AttackStatsSO attack;
//        public float range;
//        public Vector3 direction;
//        public Transform transform;

//        public AttackDrawerStruct(AttackStatsSO pAttack, Transform pTransform, float pRange, Vector3 pDirection)
//        {
//            attack = pAttack;
//            range = pRange;
//            direction = pDirection;
//            transform = pTransform;
//        }
//    }
//}
