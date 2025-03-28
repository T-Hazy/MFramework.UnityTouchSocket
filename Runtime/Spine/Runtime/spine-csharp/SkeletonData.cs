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

using System;

namespace Spine
{

    /// <summary>Stores the setup pose and all of the stateless data for a skeleton.</summary>
    public class SkeletonData
    {
        internal string name;
        internal ExposedList<BoneData> bones = new ExposedList<BoneData>(); // Ordered parents first
        internal ExposedList<SlotData> slots = new ExposedList<SlotData>(); // Setup pose draw order.
        internal ExposedList<Skin> skins = new ExposedList<Skin>();
        internal Skin defaultSkin;
        internal ExposedList<EventData> events = new ExposedList<EventData>();
        internal ExposedList<Animation> animations = new ExposedList<Animation>();
        internal ExposedList<IkConstraintData> ikConstraints = new ExposedList<IkConstraintData>();
        internal ExposedList<TransformConstraintData> transformConstraints = new ExposedList<TransformConstraintData>();
        internal ExposedList<PathConstraintData> pathConstraints = new ExposedList<PathConstraintData>();
        internal float x, y, width, height;
        internal string version, hash;

        // Nonessential.
        internal float fps;
        internal string imagesPath, audioPath;

        public string Name { get { return this.name; } set { this.name = value; } }

        /// <summary>The skeleton's bones, sorted parent first. The root bone is always the first bone.</summary>
        public ExposedList<BoneData> Bones { get { return this.bones; } }

        public ExposedList<SlotData> Slots { get { return this.slots; } }

        /// <summary>All skins, including the default skin.</summary>
        public ExposedList<Skin> Skins { get { return this.skins; } set { this.skins = value; } }

        /// <summary>
        /// The skeleton's default skin.
        /// By default this skin contains all attachments that were not in a skin in Spine.
        /// </summary>
        /// <return>May be null.</return>
        public Skin DefaultSkin { get { return this.defaultSkin; } set { this.defaultSkin = value; } }

        public ExposedList<EventData> Events { get { return this.events; } set { this.events = value; } }
        public ExposedList<Animation> Animations { get { return this.animations; } set { this.animations = value; } }
        public ExposedList<IkConstraintData> IkConstraints { get { return this.ikConstraints; } set { this.ikConstraints = value; } }
        public ExposedList<TransformConstraintData> TransformConstraints { get { return this.transformConstraints; } set { this.transformConstraints = value; } }
        public ExposedList<PathConstraintData> PathConstraints { get { return this.pathConstraints; } set { this.pathConstraints = value; } }

        public float X { get { return this.x; } set { this.x = value; } }
        public float Y { get { return this.y; } set { this.y = value; } }
        public float Width { get { return this.width; } set { this.width = value; } }
        public float Height { get { return this.height; } set { this.height = value; } }
        /// <summary>The Spine version used to export this data, or null.</summary>
        public string Version { get { return this.version; } set { this.version = value; } }
        public string Hash { get { return this.hash; } set { this.hash = value; } }

        /// <summary>The path to the images directory as defined in Spine. Available only when nonessential data was exported. May be null</summary>
        public string ImagesPath { get { return this.imagesPath; } set { this.imagesPath = value; } }

        /// <summary>The path to the audio directory defined in Spine. Available only when nonessential data was exported. May be null.</summary>
        public string AudioPath { get { return this.audioPath; } set { this.audioPath = value; } }

        /// <summary>
        /// The dopesheet FPS in Spine. Available only when nonessential data was exported.</summary>
        public float Fps { get { return this.fps; } set { this.fps = value; } }

        // --- Bones.

        /// <summary>
        /// Finds a bone by comparing each bone's name.
        /// It is more efficient to cache the results of this method than to call it multiple times.</summary>
        /// <returns>May be null.</returns>
        public BoneData FindBone(string boneName)
        {
            if (boneName == null) throw new ArgumentNullException("boneName", "boneName cannot be null.");
            var bones = this.bones;
            var bonesItems = bones.Items;
            for (int i = 0, n = bones.Count; i < n; i++)
            {
                var bone = bonesItems[i];
                if (bone.name == boneName) return bone;
            }
            return null;
        }

        /// <returns>-1 if the bone was not found.</returns>
        public int FindBoneIndex(string boneName)
        {
            if (boneName == null) throw new ArgumentNullException("boneName", "boneName cannot be null.");
            var bones = this.bones;
            var bonesItems = bones.Items;
            for (int i = 0, n = bones.Count; i < n; i++)
                if (bonesItems[i].name == boneName) return i;
            return -1;
        }

        // --- Slots.

        /// <returns>May be null.</returns>
        public SlotData FindSlot(string slotName)
        {
            if (slotName == null) throw new ArgumentNullException("slotName", "slotName cannot be null.");
            var slots = this.slots;
            for (int i = 0, n = slots.Count; i < n; i++)
            {
                var slot = slots.Items[i];
                if (slot.name == slotName) return slot;
            }
            return null;
        }

        /// <returns>-1 if the slot was not found.</returns>
        public int FindSlotIndex(string slotName)
        {
            if (slotName == null) throw new ArgumentNullException("slotName", "slotName cannot be null.");
            var slots = this.slots;
            for (int i = 0, n = slots.Count; i < n; i++)
                if (slots.Items[i].name == slotName) return i;
            return -1;
        }

        // --- Skins.

        /// <returns>May be null.</returns>
        public Skin FindSkin(string skinName)
        {
            if (skinName == null) throw new ArgumentNullException("skinName", "skinName cannot be null.");
            foreach (var skin in this.skins)
                if (skin.name == skinName) return skin;
            return null;
        }

        // --- Events.

        /// <returns>May be null.</returns>
        public EventData FindEvent(string eventDataName)
        {
            if (eventDataName == null) throw new ArgumentNullException("eventDataName", "eventDataName cannot be null.");
            foreach (var eventData in this.events)
                if (eventData.name == eventDataName) return eventData;
            return null;
        }

        // --- Animations.

        /// <returns>May be null.</returns>
        public Animation FindAnimation(string animationName)
        {
            if (animationName == null) throw new ArgumentNullException("animationName", "animationName cannot be null.");
            var animations = this.animations;
            for (int i = 0, n = animations.Count; i < n; i++)
            {
                var animation = animations.Items[i];
                if (animation.name == animationName) return animation;
            }
            return null;
        }

        // --- IK constraints.

        /// <returns>May be null.</returns>
        public IkConstraintData FindIkConstraint(string constraintName)
        {
            if (constraintName == null) throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
            var ikConstraints = this.ikConstraints;
            for (int i = 0, n = ikConstraints.Count; i < n; i++)
            {
                var ikConstraint = ikConstraints.Items[i];
                if (ikConstraint.name == constraintName) return ikConstraint;
            }
            return null;
        }

        // --- Transform constraints.

        /// <returns>May be null.</returns>
        public TransformConstraintData FindTransformConstraint(string constraintName)
        {
            if (constraintName == null) throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
            var transformConstraints = this.transformConstraints;
            for (int i = 0, n = transformConstraints.Count; i < n; i++)
            {
                var transformConstraint = transformConstraints.Items[i];
                if (transformConstraint.name == constraintName) return transformConstraint;
            }
            return null;
        }

        // --- Path constraints.

        /// <returns>May be null.</returns>
        public PathConstraintData FindPathConstraint(string constraintName)
        {
            if (constraintName == null) throw new ArgumentNullException("constraintName", "constraintName cannot be null.");
            var pathConstraints = this.pathConstraints;
            for (int i = 0, n = pathConstraints.Count; i < n; i++)
            {
                var constraint = pathConstraints.Items[i];
                if (constraint.name.Equals(constraintName)) return constraint;
            }
            return null;
        }

        /// <returns>-1 if the path constraint was not found.</returns>
        public int FindPathConstraintIndex(string pathConstraintName)
        {
            if (pathConstraintName == null) throw new ArgumentNullException("pathConstraintName", "pathConstraintName cannot be null.");
            var pathConstraints = this.pathConstraints;
            for (int i = 0, n = pathConstraints.Count; i < n; i++)
                if (pathConstraints.Items[i].name.Equals(pathConstraintName)) return i;
            return -1;
        }

        // ---

        public override string ToString()
        {
            return this.name ?? base.ToString();
        }
    }
}
