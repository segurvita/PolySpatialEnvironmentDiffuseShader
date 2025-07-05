#if COM_VRMC_GLTF

using Segur.PolySpatialEnvironmentDiffuseShader.Runtime;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UniGLTF;
using UniJSON;

// ReSharper disable once CheckNamespace
namespace Segur.PolySpatialEnvironmentDiffuseShader.Tests.Runtime
{
    public class EnvironmentDiffuseMaterialDescriptorGeneratorTest
    {
        private const string MaterialExtensionFilePath = "Tests/Runtime/TestAssets/MToonMaterialExtensionSample.json";
        private const string TextureExtensionFilePath = "Tests/Runtime/TestAssets/TextureExtensionSample.json";

        [Test]
        public void GetTestSimplePasses()
        {
            var gltfData = GenerateSampleGltfData();

            var materialGenerator = new EnvironmentDiffuseMaterialDescriptorGenerator();
            var materialDescriptor = materialGenerator.Get(gltfData, 0);

            Assert.That(materialDescriptor.Shader.name, Is.EqualTo(EnvironmentDiffuseMaterialDescriptorGenerator.ShaderName));

            Assert.That(materialDescriptor.TextureSlots.Count, Is.EqualTo(2));
            Assert.That(materialDescriptor.TextureSlots.ContainsKey("_MainTex"), Is.False);
            Assert.That(materialDescriptor.TextureSlots.ContainsKey("_BaseMap"), Is.True);
            Assert.That(materialDescriptor.TextureSlots.ContainsKey("_EmissionMap"), Is.True);

            Assert.That(materialDescriptor.FloatValues.Count, Is.GreaterThan(4));
            Assert.That(materialDescriptor.FloatValues["_AlphaMode"], Is.EqualTo(0));
            Assert.That(materialDescriptor.FloatValues["_TransparentWithZWrite"], Is.EqualTo(0));
            Assert.That(materialDescriptor.FloatValues["_Cutoff"], Is.EqualTo(0.5f));
            Assert.That(materialDescriptor.FloatValues["_DoubleSided"], Is.EqualTo(0));

            Assert.That(materialDescriptor.Colors.Count, Is.EqualTo(2));
            Assert.That(materialDescriptor.Colors.ContainsKey("_Color"), Is.False);
            Assert.That(materialDescriptor.Colors.ContainsKey("_BaseColor"), Is.True);
            Assert.That(materialDescriptor.Colors.ContainsKey("_EmissionColor"), Is.True);

            Assert.That(materialDescriptor.Vectors.Count, Is.EqualTo(0));
        }

        private static GltfData GenerateSampleGltfData()
        {
            var gltf = new glTF
            {
                images = GenerateSampleGltfImages(),
                textures = GenerateSampleGltfTextures(),
                materials = GenerateSampleGltfMaterials()
            };

            var gltfData = new GltfData("", "", gltf, null, null, null);
            return gltfData;
        }

        private static List<glTFImage> GenerateSampleGltfImages()
        {
            var baseImage = new glTFImage
            {
                bufferView = 0,
                mimeType = "image/png",
                name = "Base"
            };

            var emissionImage = new glTFImage
            {
                bufferView = 1,
                mimeType = "image/png",
                name = "Emission"
            };

            return new List<glTFImage>
            {
                baseImage,
                emissionImage
            };
        }

        private static List<glTFTexture> GenerateSampleGltfTextures()
        {
            var baseTexture = new glTFTexture
            {
                name = "Base",
                sampler = 0,
                source = 0
            };

            var emissionTexture = new glTFTexture
            {
                name = "Emission",
                sampler = 1,
                source = 1
            };

            return new List<glTFTexture>
            {
                baseTexture,
                emissionTexture
            };
        }

        private static List<glTFMaterial> GenerateSampleGltfMaterials()
        {
            var materialExtensionJsonFilePath = Path.Combine(TestUtility.GetPackagePath(), MaterialExtensionFilePath);
            var materialExtensionJsonText = File.ReadAllText(materialExtensionJsonFilePath);
            var materialExtensionJson = materialExtensionJsonText.ParseAsJson();

            var textureExtensionJsonFilePath = Path.Combine(TestUtility.GetPackagePath(), TextureExtensionFilePath);
            var textureExtensionJsonText = File.ReadAllText(textureExtensionJsonFilePath);
            var textureExtensionJson = textureExtensionJsonText.ParseAsJson();

            var extensions = new glTFExtensionImport(materialExtensionJson);
            var material = new glTFMaterial
            {
                extensions = extensions,
                alphaCutoff = 0.5f,
                alphaMode = "OPAQUE",
                doubleSided = false,
                emissiveFactor = new[] { 0f, 0f, 0f },
                emissiveTexture = new glTFMaterialEmissiveTextureInfo
                {
                    extensions = new glTFExtensionImport(textureExtensionJson),
                    index = 1,
                    texCoord = 0
                },
                name = "sample material",
                pbrMetallicRoughness = new glTFPbrMetallicRoughness
                {
                    baseColorFactor = new[] { 1f, 1f, 1f, 1f },
                    baseColorTexture = new glTFMaterialBaseColorTextureInfo()
                    {
                        extensions = new glTFExtensionImport(textureExtensionJson),
                        index = 0,
                        texCoord = 0
                    },
                    metallicFactor = 0,
                    roughnessFactor = 0
                }
            };

            return new List<glTFMaterial>
            {
                material
            };
        }
    }
}

#endif