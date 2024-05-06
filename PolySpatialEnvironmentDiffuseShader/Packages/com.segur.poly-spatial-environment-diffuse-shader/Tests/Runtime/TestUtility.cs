using UnityEditor.PackageManager;

// ReSharper disable once CheckNamespace
namespace Segur.PolySpatialEnvironmentDiffuseShader.Tests.Runtime
{
    public static class TestUtility
    {
        private const string PackageName = "com.segur.poly-spatial-environment-diffuse-shader";


        public static string GetPackagePath()
        {
            var request = Client.List();
            while (!request.IsCompleted)
            {
            }

            foreach (var package in request.Result)
            {
                if (package.name == PackageName)
                {
                    return package.resolvedPath;
                }
            }

            return "";
        }
    }
}