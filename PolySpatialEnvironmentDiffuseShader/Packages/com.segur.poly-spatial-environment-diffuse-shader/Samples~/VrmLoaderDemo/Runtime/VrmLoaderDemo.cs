#if COM_VRMC_GLTF && COM_VRMC_VRM

using Segur.PolySpatialEnvironmentDiffuseShader.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Networking;
using UniVRM10;

// ReSharper disable once CheckNamespace
namespace Segur.PolySpatialEnvironmentDiffuseShader.Samples.VrmLoaderDemo.Runtime
{
    public class VrmLoaderDemo : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Please input url of VRM file")]
        private string url = "";

        private void Start()
        {
            StartCoroutine(LoadVrm());
        }

        /// <summary>
        /// Load VRM with PolySpatialEnvironmentDiffuseShader.
        /// To reduce dependencies on external libraries, this function uses coroutines to implement asynchronous processing.
        /// However, using UniTask would result in smarter code.
        /// </summary>
        private IEnumerator LoadVrm()
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("Please input Url of VrmLoader.");
                yield break;
            }

            using var req = UnityWebRequest.Get(url);
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                yield break;
            }

            var vrmBytes = req.downloadHandler.data;

            var task = Vrm10.LoadBytesAsync(
                vrmBytes,
                canLoadVrm0X: true,
                materialGenerator: GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset
                    ? new EnvironmentDiffuseMaterialDescriptorGenerator()
                    : null
            );
            yield return new WaitUntil(() => task.IsCompleted);

            var vrmInstance = task.Result;

            var vrmTransform = vrmInstance.transform;
            vrmTransform.SetParent(transform);
            vrmTransform.localPosition = Vector3.zero;
            vrmTransform.localRotation = Quaternion.identity;
            vrmTransform.localScale = Vector3.one;
        }
    }
}

#endif