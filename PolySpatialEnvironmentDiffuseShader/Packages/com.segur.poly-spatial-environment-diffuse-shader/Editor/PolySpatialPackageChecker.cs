using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Segur.PolySpatialEnvironmentDiffuseShader.Editor
{
    [InitializeOnLoad]
    public class PolySpatialPackageChecker
    {
        private const string PackageName = "com.unity.polyspatial";
        private const string ShaderName = "Segur/PolySpatial/EnvironmentDiffuse";

        static PolySpatialPackageChecker()
        {
            // Check package status on Unity Editor startup
            CheckPolySpatialPackage();

            // Subscribe to package events
            Events.registeringPackages += OnPackagesChanged;
        }

        private static void OnPackagesChanged(PackageRegistrationEventArgs args)
        {
            CheckPolySpatialPackage();
        }

        private static void CheckPolySpatialPackage()
        {
            // Check if the package is installed
            var usePolySpatial = IsPackageInstalled(PackageName);

            // Set the shader keyword value
            SetShaderKeyword(usePolySpatial);
        }

        private static bool IsPackageInstalled(string packageName)
        {
            var request = Client.List();
            while (!request.IsCompleted)
            {
            }

            return request.Result.Any(pkg => pkg.name == packageName);
        }

        private static void SetShaderKeyword(bool usePolySpatial)
        {
            // Load the shader
            var shader = Shader.Find(ShaderName);
            if (shader == null)
            {
                Debug.LogWarning($"Shader not found: {ShaderName}");
                return;
            }

            if (usePolySpatial)
            {
                Shader.EnableKeyword("_USE_POLYSPATIAL");
            }
            else
            {
                Shader.DisableKeyword("_USE_POLYSPATIAL");
            }
        }
    }
}