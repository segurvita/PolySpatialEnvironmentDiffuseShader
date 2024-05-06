#if COM_VRMC_GLTF && COM_VRMC_VRM && COM_VRMC_VRMSHADERS

using System;
using UnityEngine;
using UnityEngine.Rendering;
using VRMShaders.VRM10.MToon10.Runtime;

// This code is based on Packages/com.vrmc.vrmshaders/VRM10/MToon10/Runtime/MToonValidator.cs
// ReSharper disable once CheckNamespace
namespace Segur.PolySpatialEnvironmentDiffuseShader.Runtime
{
    /// <summary>
    /// Adjust parameters of PolySpatial EnvironmentDiffuseShader
    /// This will be used on VisionOS
    /// </summary>
    public sealed class EnvironmentDiffuseMaterialParameterAdjuster
    {
        private static readonly int Surface = Shader.PropertyToID("_Surface");
        private static readonly int AlphaClip = Shader.PropertyToID("_AlphaClip");
        private static readonly int AlphaClipThreshold = Shader.PropertyToID("_AlphaClipThreshold");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private static readonly int Cull = Shader.PropertyToID("_Cull");
        private static readonly int AlphaToMask = Shader.PropertyToID("_AlphaToMask");
        private static readonly int QueueOffset = Shader.PropertyToID("_QueueOffset");

        private readonly Material _material;

        public EnvironmentDiffuseMaterialParameterAdjuster(Material material)
        {
            _material = material;
        }

        public void Validate()
        {
            SetAlphaSettings();
            SetDoubleSidedSetting();

            // Refresh material
            _material.shader = EnvironmentDiffuseMaterialDescriptorGenerator.TargetShader;
        }

        private void SetAlphaSettings()
        {
            var alphaMode = (MToon10AlphaMode)_material.GetInt(MToon10Prop.AlphaMode);
            var zWriteMode = (MToon10TransparentWithZWriteMode)_material.GetInt(MToon10Prop.TransparentWithZWrite);
            var renderQueueOffset = _material.GetInt(MToon10Prop.RenderQueueOffsetNumber);
            var alphaCutoff = _material.GetFloat(MToon10Prop.AlphaCutoff);

            switch (alphaMode)
            {
                case MToon10AlphaMode.Opaque:
                    _material.SetOverrideTag(UnityRenderTag.Key, UnityRenderTag.OpaqueValue);
                    _material.SetInt(SrcBlend, (int)BlendMode.One);
                    _material.SetInt(DstBlend, (int)BlendMode.Zero);
                    _material.SetInt(ZWrite, (int)UnityZWriteMode.On);
                    _material.SetInt(AlphaToMask, (int)UnityAlphaToMaskMode.Off);
                    _material.SetInt(Surface, 0); // Set to Opaque
                    _material.SetInt(AlphaClip, 0); // Disable Alpha Clipping

                    renderQueueOffset = 0;
                    _material.renderQueue = (int)RenderQueue.Geometry;
                    break;
                case MToon10AlphaMode.Cutout:
                    _material.SetOverrideTag(UnityRenderTag.Key, UnityRenderTag.TransparentCutoutValue);
                    _material.SetInt(SrcBlend, (int)BlendMode.One);
                    _material.SetInt(DstBlend, (int)BlendMode.Zero);
                    _material.SetInt(ZWrite, (int)UnityZWriteMode.On);
                    _material.SetInt(AlphaToMask, (int)UnityAlphaToMaskMode.On);
                    _material.SetInt(Surface, 0); // Set to Opaque
                    _material.SetInt(AlphaClip, 1); // Enable Alpha Clipping
                    _material.SetFloat(AlphaClipThreshold, alphaCutoff);

                    renderQueueOffset = 0;
                    _material.renderQueue = (int)RenderQueue.AlphaTest;
                    break;
                case MToon10AlphaMode.Transparent when zWriteMode == MToon10TransparentWithZWriteMode.Off:
                    _material.SetOverrideTag(UnityRenderTag.Key, UnityRenderTag.TransparentValue);
                    _material.SetInt(SrcBlend, (int)BlendMode.SrcAlpha);
                    _material.SetInt(DstBlend, (int)BlendMode.OneMinusSrcAlpha);
                    _material.SetInt(ZWrite, (int)UnityZWriteMode.Off);
                    _material.SetInt(AlphaToMask, (int)UnityAlphaToMaskMode.Off);
                    _material.SetInt(Surface, 1); // Set to Transparent
                    _material.SetInt(AlphaClip, 0); // Disable Alpha Clipping

                    renderQueueOffset = Mathf.Clamp(renderQueueOffset, -9, 0);
                    _material.renderQueue = (int)RenderQueue.Transparent + renderQueueOffset;
                    break;
                case MToon10AlphaMode.Transparent when zWriteMode == MToon10TransparentWithZWriteMode.On:
                    _material.SetOverrideTag(UnityRenderTag.Key, UnityRenderTag.TransparentValue);
                    _material.SetInt(SrcBlend, (int)BlendMode.SrcAlpha);
                    _material.SetInt(DstBlend, (int)BlendMode.OneMinusSrcAlpha);
                    _material.SetInt(ZWrite, (int)UnityZWriteMode.On);
                    _material.SetInt(AlphaToMask, (int)UnityAlphaToMaskMode.Off);
                    _material.SetInt(Surface, 1); // Set to Transparent
                    _material.SetInt(AlphaClip, 0); // Disable Alpha Clipping

                    renderQueueOffset = Mathf.Clamp(renderQueueOffset, 0, +9);
                    _material.renderQueue = (int)RenderQueue.GeometryLast + 1 + renderQueueOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(alphaMode), alphaMode, null);
            }

            // Set after validation
            _material.SetInt(QueueOffset, renderQueueOffset);
        }

        private void SetDoubleSidedSetting()
        {
            var doubleSidedMode = (MToon10DoubleSidedMode)_material.GetInt(MToon10Prop.DoubleSided);

            switch (doubleSidedMode)
            {
                case MToon10DoubleSidedMode.Off:
                    _material.SetInt(Cull, (int)UnityCullMode.Back); // Cull Back, Render Front
                    break;
                case MToon10DoubleSidedMode.On:
                    _material.SetInt(Cull, (int)UnityCullMode.Off); // Cull Off, Render Both
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(doubleSidedMode), doubleSidedMode, null);
            }
        }
    }
}

#endif