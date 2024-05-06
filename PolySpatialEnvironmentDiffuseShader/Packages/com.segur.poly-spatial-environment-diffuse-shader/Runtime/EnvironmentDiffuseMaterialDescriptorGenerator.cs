#if COM_VRMC_GLTF && COM_VRMC_VRM && COM_VRMC_VRMSHADERS

using System;
using System.Collections.Generic;
using UniGLTF;
using UnityEngine;
using UniVRM10;
using VRMShaders;

// This code is based on Packages/com.vrmc.vrm/Runtime/IO/Material/URP/Import/UrpVrm10MaterialDescriptorGenerator.cs:
// ReSharper disable once CheckNamespace
namespace Segur.PolySpatialEnvironmentDiffuseShader.Runtime
{
    /// <summary>
    /// VRM MaterialDescriptorGenerator for PolySpatial EnvironmentDiffuseShader
    /// This will be used on VisionOS
    /// </summary>
    public sealed class EnvironmentDiffuseMaterialDescriptorGenerator : IMaterialDescriptorGenerator
    {
        public const string ShaderName = "Segur/PolySpatial/EnvironmentDiffuse";

        private static readonly Shader TargetShader = Shader.Find(ShaderName);

        public MaterialDescriptor Get(GltfData data, int i)
        {
            // mtoon
            if (UrpVrm10MToonMaterialImporter.TryCreateParam(data, i, out var matDesc))
            {
                // Change MaterialDescriptor for MToon
                return ReplaceMaterialDescriptor(matDesc);
            }

            // unlit
            if (BuiltInGltfUnlitMaterialImporter.TryCreateParam(data, i, out matDesc))
            {
                return matDesc;
            }

            // pbr
            if (UrpGltfPbrMaterialImporter.TryCreateParam(data, i, out matDesc))
            {
                return matDesc;
            }

            // fallback
            Debug.LogWarning($"material: {i} out of range. fallback");
            return GenerateUrpGltfPbrMaterialDescriptor(i);
        }

        public MaterialDescriptor GetGltfDefault()
        {
            return UrpGltfDefaultMaterialImporter.CreateParam();
        }

        /// <summary>
        /// Transforms the provided material descriptor.
        /// </summary>
        /// <param name="inputMaterialDescriptor">The material descriptor to transform.</param>
        /// <returns>The transformed material descriptor.</returns>
        private static MaterialDescriptor ReplaceMaterialDescriptor(MaterialDescriptor inputMaterialDescriptor)
        {
            var outputMaterialDescriptor = new MaterialDescriptor(
                inputMaterialDescriptor.Name,
                TargetShader,
                null,
                ReplaceTextureSlots(inputMaterialDescriptor.TextureSlots),
                new Dictionary<string, float>(),
                ReplaceColors(inputMaterialDescriptor.Colors),
                new Dictionary<string, Vector4>(),
                new Action<Material>[] { });

            return outputMaterialDescriptor;
        }

        /// <summary>
        /// Transforms the provided dictionary of texture slots.
        /// If the key is "_MainTex", it is replaced with "_BaseMap".
        /// If the key is "_EmissionMap", it remains the same.
        /// Keys other than these are not transformed.
        /// </summary>
        /// <param name="inputTextureSlots">The dictionary of texture slots to transform.</param>
        /// <returns>The transformed dictionary of texture slots.</returns>
        private static IReadOnlyDictionary<string, TextureDescriptor> ReplaceTextureSlots(
            IReadOnlyDictionary<string, TextureDescriptor> inputTextureSlots)
        {
            var outputTextureSlots = new Dictionary<string, TextureDescriptor>();
            foreach (var currentTextureSlot in inputTextureSlots)
            {
                switch (currentTextureSlot.Key)
                {
                    case "_MainTex":
                        outputTextureSlots.Add("_BaseMap", currentTextureSlot.Value);
                        break;
                    case "_EmissionMap":
                        outputTextureSlots.Add("_EmissionMap", currentTextureSlot.Value);
                        break;
                }
            }

            return outputTextureSlots;
        }

        /// <summary>
        /// Transforms the provided dictionary of colors.
        /// If the key is "_Color", it is replaced with "_BaseColor".
        /// If the key is "_EmissionColor", it remains the same.
        /// Keys other than these are not transformed.
        /// </summary>
        /// <param name="inputColors">The dictionary of colors to transform.</param>
        /// <returns>The transformed dictionary of colors.</returns>
        private static IReadOnlyDictionary<string, Color> ReplaceColors(IReadOnlyDictionary<string, Color> inputColors)
        {
            var outputColors = new Dictionary<string, Color>();
            foreach (var currentColor in inputColors)
            {
                switch (currentColor.Key)
                {
                    case "_Color":
                        outputColors.Add("_BaseColor", currentColor.Value);
                        break;
                    case "_EmissionColor":
                        outputColors.Add("_EmissionColor", currentColor.Value);
                        break;
                }
            }

            return outputColors;
        }

        private static MaterialDescriptor GenerateUrpGltfPbrMaterialDescriptor(int i)
        {
            return new MaterialDescriptor(
                GltfMaterialImportUtils.ImportMaterialName(i, null),
                UrpGltfPbrMaterialImporter.Shader,
                null,
                new Dictionary<string, TextureDescriptor>(),
                new Dictionary<string, float>(),
                new Dictionary<string, Color>(),
                new Dictionary<string, Vector4>(),
                new Action<Material>[] { });
        }
    }
}

#endif