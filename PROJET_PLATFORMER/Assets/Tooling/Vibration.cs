using System;
using System.Collections;
using System.Collections.Generic; //do not remove
using UnityEngine;
using UnityEngine.Android;

// Author : Camille SMOLARSKI

namespace Platformer.Utils
{
    public static class Vibration
    {
        public enum PredefinedEffect
        {
            CLICK,
            DOUBLE_CLICK,
            HEAVY_CLICK,
            TICK
        }
        private static int GetAPILevel()
        {
            using AndroidJavaClass androidVersionClass = new("android.os.Build$VERSION");
            return androidVersionClass.GetStatic<int>("SDK_INT");
        }

        private static AndroidJavaObject GetSystemService(string pServiceName)
            => AndroidApplication.currentActivity.Call<AndroidJavaObject>("getSystemService", pServiceName);

#if UNITY_ANDROID && !UNITY_EDITOR
        private const string ANDROID_DEFAULT_AMP_CONST_NAME = "DEFAULT_AMPLITUDE";
        private const string ANDROID_HAS_VIBRATOR_FIELD_NAME = "hasVibrator";
        private const string ANDROID_AMPLITUDE_CONTROL_FIELD_NAME = "hasAmplitudeControl";
        
        private const string ANDROID_VIBRATE_METHOD_NAME = "vibrate";
        private const string ANDROID_CANCEL_METHOD_NAME = "cancel";
        
        private const string ANDROID_CREATE_ONE_SHOT_METHOD_NAME = "createOneShot";
        private const string ANDROID_CREATE_WAVEFORM_METHOD_NAME = "createWaveform";
        private const string ANDROID_CREATE_PREDEFINED_METHOD_NAME = "createPredefined";
        
        
        private const string ANDROID_VIBRATOR_SERVICE_NAME = "vibrator";
        private const string ANDROID_VIBRATION_EFFECT_CLASS_NAME = "android.os.VibrationEffect";
        
        private const string PREDEFINED_EFFECT_PREFIX = "EFFECT_";
        
        private const int VIBRATION_EFFECT_MIN_API_LEVEL = 26;
        private const int PREDEFINED_EFFECT_MIN_API_LEVEL = 29;
        
        public static float DefaultAmplitude { get; private set; }
        
        private static AndroidJavaObject _AndroidVibrationService;
        private static AndroidJavaClass _VibrationEffectJavaClass;
        
        private static bool _CanVibrate;
        private static bool _SupportsVibrationEffect;
        private static bool _SupportsVibrationControl;
        private static bool _SupportsPredefinedEffect;

        private static Dictionary<PredefinedEffect, int> _PredefinedEffectIdTable;
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            //Unity auto adds vibration permission to the app manifest if a script contains Handheld.Vibrate()
            if (!Application.isPlaying)
                Handheld.Vibrate();
            
            _AndroidVibrationService = GetSystemService(ANDROID_VIBRATOR_SERVICE_NAME);

            if (_AndroidVibrationService == null)
            {
                Debug.LogWarning("Vibrator system service not found.");
                return;
            }

            _CanVibrate = _AndroidVibrationService.Call<bool>(ANDROID_HAS_VIBRATOR_FIELD_NAME);
            Debug.Log($"Device vibrator initialization success result : {_CanVibrate}");
            if (!_CanVibrate)
                return;
            
            int lAPILevel = GetAPILevel();
            
            _SupportsVibrationEffect = lAPILevel >= VIBRATION_EFFECT_MIN_API_LEVEL;
            Debug.Log($"Vibration effect support : {_SupportsVibrationEffect}");
            if (!_SupportsVibrationEffect)
                return;

            _VibrationEffectJavaClass = new(ANDROID_VIBRATION_EFFECT_CLASS_NAME);
            _SupportsVibrationControl = _AndroidVibrationService.Call<bool>(ANDROID_AMPLITUDE_CONTROL_FIELD_NAME);
            DefaultAmplitude = Mathf.Clamp(_VibrationEffectJavaClass.GetStatic<int>(ANDROID_DEFAULT_AMP_CONST_NAME), 0, byte.MaxValue) / (float)byte.MaxValue;
            
            _SupportsPredefinedEffect = lAPILevel >= PREDEFINED_EFFECT_MIN_API_LEVEL;
            Debug.Log($"Predefined effect support : {_SupportsPredefinedEffect}");
            if (!_SupportsPredefinedEffect)
                return;

            _PredefinedEffectIdTable = new();
            foreach (PredefinedEffect lValue in Enum.GetValues(typeof(PredefinedEffect)))
                _PredefinedEffectIdTable.Add(lValue, _VibrationEffectJavaClass.GetStatic<int>(PREDEFINED_EFFECT_PREFIX + lValue));
#endif

        }

        public static void Vibrate(VibrationStruct pStruct)
            => Vibrate(pStruct.duration, pStruct.strength);

        /// <summary></summary>
        /// <param name="pDuration">Duration in seconds.</param>
        /// <param name="pStrength">Vibration strength, given in a range from 0 to 1.</param>
        public static void Vibrate(float pDuration, float pStrength)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            InternalVibrationStruct lStruct = new(pDuration, pStrength);
            if (!_CanVibrate || ! lStruct.IsValid)
                return;

            if (!_SupportsVibrationEffect)
            {
                _AndroidVibrationService.Call(ANDROID_VIBRATE_METHOD_NAME, lStruct.milliseconds);
                return;
            }
            
            using AndroidJavaObject lEffect = _VibrationEffectJavaClass.CallStatic<AndroidJavaObject>(
                ANDROID_CREATE_ONE_SHOT_METHOD_NAME,
                lStruct.milliseconds,
                _SupportsVibrationControl ? lStruct.amplitude : byte.MaxValue);
            
            _AndroidVibrationService.Call(ANDROID_VIBRATE_METHOD_NAME, lEffect);
#endif
        }

        public static void Vibrate(VibrationStruct[] pPattern, int pRepeat = -1)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!_CanVibrate)
                return;

            InternalVibrationStruct lInternalStruct;
            long[] lMilliseconds = new long[pPattern.Length];
            int[] lAmplitudes = new int[pPattern.Length];

            for (int i = 0; i < pPattern.Length; i++)
            {
                lInternalStruct = pPattern[i];
                lMilliseconds[i] = lInternalStruct.milliseconds;
                lAmplitudes[i] = lInternalStruct.amplitude;
            }

            if (!_SupportsVibrationEffect)
            {
                _AndroidVibrationService.Call(ANDROID_VIBRATE_METHOD_NAME, lMilliseconds, pRepeat);
                return;
            }

            if (!_SupportsVibrationControl)
            {
                using AndroidJavaObject effect = _VibrationEffectJavaClass.CallStatic<AndroidJavaObject>(
                    ANDROID_CREATE_WAVEFORM_METHOD_NAME, lMilliseconds, pRepeat);
                _AndroidVibrationService.Call(ANDROID_VIBRATE_METHOD_NAME, effect);
                return;
            }
            
            using AndroidJavaObject lEffect = _VibrationEffectJavaClass.CallStatic<AndroidJavaObject>(
                ANDROID_CREATE_WAVEFORM_METHOD_NAME, lMilliseconds, lAmplitudes, pRepeat);
            _AndroidVibrationService.Call(ANDROID_VIBRATE_METHOD_NAME, lEffect);
#endif
        }

        public static void Vibrate(PredefinedEffect pEffect)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!_SupportsPredefinedEffect)
                return;
            
            using AndroidJavaObject lEffect = _VibrationEffectJavaClass.CallStatic<AndroidJavaObject>(
                ANDROID_CREATE_PREDEFINED_METHOD_NAME, _PredefinedEffectIdTable[pEffect]);
            _AndroidVibrationService.Call(ANDROID_VIBRATE_METHOD_NAME, lEffect);
#endif
        }

        private static void Cancel()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_CanVibrate)
                _AndroidVibrationService.Call(ANDROID_CANCEL_METHOD_NAME);
#endif
        }

        public struct VibrationStruct
        {
            public float duration;
            public float strength;

            public VibrationStruct(float pDuration, float pStrength)
            {
                duration = pDuration;
                strength = pStrength;
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private readonly struct InternalVibrationStruct
        {
            public bool IsValid => milliseconds > 0 && amplitude > 0;
            
            public readonly long milliseconds;
            public readonly int amplitude;
            
            public InternalVibrationStruct(float pDuration, float pStrength)
            {
                milliseconds = pDuration > 0 ? (int)(pDuration * 1000) : default;
                amplitude = (byte)(Mathf.Clamp01(pStrength) * byte.MaxValue);
            }

            public static implicit operator InternalVibrationStruct(VibrationStruct lStruct)
                => new(lStruct.duration, lStruct.strength);
        }
#endif
    }
}