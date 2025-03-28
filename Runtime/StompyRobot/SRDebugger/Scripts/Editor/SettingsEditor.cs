﻿#if !DISABLE_SRDEBUGGER
using UnityEditor;
using UnityEngine;

namespace SRDebugger.Editor
{
    [CustomEditor(typeof(Settings))]
    internal class SettingsEditor : UnityEditor.Editor
    {
        private bool _override;

        public override void OnInspectorGUI()
        {
            SRInternalEditorUtil.DrawLogo(SRInternalEditorUtil.GetLogo());

            GUILayout.Label(
                "This asset contains the runtime settings used by SRDebugger. It is recommended that this asset be edited only via the SRDebugger Settings window.",
                EditorStyles.wordWrappedLabel);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Open SRDebugger Settings Window"))
            {
                SRDebuggerSettingsWindow.Open();
            }

            if (!this._override)
            {
                if (GUILayout.Button("Override Warning"))
                {
                    this._override = true;
                }
            }
            else
            {
                GUILayout.Label(
                    "You have been warned...",
                    EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.Separator();

            if (this._override)
            {
                base.OnInspectorGUI();
            }
        }
    }
}
#endif