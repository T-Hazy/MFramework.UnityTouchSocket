/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated January 1, 2020. Replaces all prior versions.
 *
 * Copyright (c) 2013-2020, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THE SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Editor
{
    using Icons = SpineEditorUtilities.Icons;

    public class SkeletonBakingWindow : EditorWindow
    {
        private const bool IsUtilityWindow = true;

        [MenuItem("CONTEXT/SkeletonDataAsset/Skeleton Baking", false, 5000)]
        public static void Init(MenuCommand command)
        {
            var window = EditorWindow.GetWindow<SkeletonBakingWindow>(IsUtilityWindow);
            window.minSize = new Vector2(330f, 530f);
            window.maxSize = new Vector2(600f, 1000f);
            window.titleContent = new GUIContent("Skeleton Baking", Icons.spine);
            window.skeletonDataAsset = command.context as SkeletonDataAsset;
            window.Show();
        }

        public SkeletonDataAsset skeletonDataAsset;
        [SpineSkin(dataField: "skeletonDataAsset")]
        public string skinToBake = "default";

        // Settings
        private bool bakeAnimations = false;
        private bool bakeIK = true;
        private SendMessageOptions bakeEventOptions;

        private SerializedObject so;
        private Skin bakeSkin;


        private void DataAssetChanged()
        {
            this.bakeSkin = null;
        }

        private void OnGUI()
        {
            this.so = this.so ?? new SerializedObject(this);

            EditorGUIUtility.wideMode = true;
            EditorGUILayout.LabelField("Spine Skeleton Prefab Baking", EditorStyles.boldLabel);

            const string BakingWarningMessage = "\nSkeleton baking is not the primary use case for Spine skeletons." +
                "\nUse baking if you have specialized uses, such as simplified skeletons with movement driven by physics." +

                "\n\nBaked Skeletons do not support the following:" +
                "\n\tDisabled rotation or scale inheritance" +
                "\n\tLocal Shear" +
                "\n\tAll Constraint types" +
                "\n\tWeighted mesh verts with more than 4 bound bones" +

                "\n\nBaked Animations do not support the following:" +
                "\n\tMesh Deform Keys" +
                "\n\tColor Keys" +
                "\n\tDraw Order Keys" +

                "\n\nAnimation Curves are sampled at 60fps and are not realtime." +
                "\nConstraint animations are also baked into animation curves." +
                "\nSee SkeletonBaker.cs comments for full details.\n";

            EditorGUILayout.HelpBox(BakingWarningMessage, MessageType.Info, true);

            EditorGUI.BeginChangeCheck();
            var skeletonDataAssetProperty = this.so.FindProperty("skeletonDataAsset");
            EditorGUILayout.PropertyField(skeletonDataAssetProperty, SpineInspectorUtility.TempContent("SkeletonDataAsset", Icons.spine));
            if (EditorGUI.EndChangeCheck())
            {
                this.so.ApplyModifiedProperties();
                this.DataAssetChanged();
            }
            EditorGUILayout.Space();

            if (this.skeletonDataAsset == null) return;
            var skeletonData = this.skeletonDataAsset.GetSkeletonData(false);
            if (skeletonData == null) return;
            var hasExtraSkins = skeletonData.Skins.Count > 1;

            using (new SpineInspectorUtility.BoxScope(false))
            {
                EditorGUILayout.LabelField(this.skeletonDataAsset.name, EditorStyles.boldLabel);
                using (new SpineInspectorUtility.IndentScope())
                {
                    EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Bones: " + skeletonData.Bones.Count, Icons.bone));
                    EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Slots: " + skeletonData.Slots.Count, Icons.slotRoot));

                    if (hasExtraSkins)
                    {
                        EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Skins: " + skeletonData.Skins.Count, Icons.skinsRoot));
                        EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Current skin attachments: " + (this.bakeSkin == null ? 0 : this.bakeSkin.Attachments.Count), Icons.skinPlaceholder));
                    }
                    else if (skeletonData.Skins.Count == 1)
                    {
                        EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Skins: 1 (only default Skin)", Icons.skinsRoot));
                    }

                    var totalAttachments = 0;
                    foreach (var s in skeletonData.Skins)
                        totalAttachments += s.Attachments.Count;
                    EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Total Attachments: " + totalAttachments, Icons.genericAttachment));
                }
            }
            using (new SpineInspectorUtility.BoxScope(false))
            {
                EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(SpineInspectorUtility.TempContent("Animations: " + skeletonData.Animations.Count, Icons.animation));

                using (new SpineInspectorUtility.IndentScope())
                {
                    this.bakeAnimations = EditorGUILayout.Toggle(SpineInspectorUtility.TempContent("Bake Animations", Icons.animationRoot), this.bakeAnimations);
                    using (new EditorGUI.DisabledScope(!this.bakeAnimations))
                    {
                        using (new SpineInspectorUtility.IndentScope())
                        {
                            this.bakeIK = EditorGUILayout.Toggle(SpineInspectorUtility.TempContent("Bake IK", Icons.constraintIK), this.bakeIK);
                            this.bakeEventOptions = (SendMessageOptions)EditorGUILayout.EnumPopup(SpineInspectorUtility.TempContent("Event Options", Icons.userEvent), this.bakeEventOptions);
                        }
                    }
                }
            }
            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(this.skinToBake) && UnityEngine.Event.current.type == EventType.Repaint)
                this.bakeSkin = skeletonData.FindSkin(this.skinToBake) ?? skeletonData.DefaultSkin;

            var prefabIcon = EditorGUIUtility.FindTexture("PrefabModel Icon");

            if (hasExtraSkins)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.so.FindProperty("skinToBake"));
                if (EditorGUI.EndChangeCheck())
                {
                    this.so.ApplyModifiedProperties();
                    this.Repaint();
                }

                if (SpineInspectorUtility.LargeCenteredButton(SpineInspectorUtility.TempContent(string.Format("Bake Skeleton with Skin ({0})", (this.bakeSkin == null ? "default" : this.bakeSkin.Name)), prefabIcon)))
                {
                    SkeletonBaker.BakeToPrefab(this.skeletonDataAsset, new ExposedList<Skin>(new[] { this.bakeSkin }), "", this.bakeAnimations, this.bakeIK, this.bakeEventOptions);
                }

                if (SpineInspectorUtility.LargeCenteredButton(SpineInspectorUtility.TempContent(string.Format("Bake All ({0} skins)", skeletonData.Skins.Count), prefabIcon)))
                {
                    SkeletonBaker.BakeToPrefab(this.skeletonDataAsset, skeletonData.Skins, "", this.bakeAnimations, this.bakeIK, this.bakeEventOptions);
                }
            }
            else
            {
                if (SpineInspectorUtility.LargeCenteredButton(SpineInspectorUtility.TempContent("Bake Skeleton", prefabIcon)))
                {
                    SkeletonBaker.BakeToPrefab(this.skeletonDataAsset, new ExposedList<Skin>(new[] { this.bakeSkin }), "", this.bakeAnimations, this.bakeIK, this.bakeEventOptions);
                }

            }

        }
    }
}
