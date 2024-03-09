using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class EnvFileManager : MonoBehaviour
{
    public static EnvFileManager Instance { get; private set; }

    public Dictionary<string, string> envVariables;

    private string envFilePath = Path.Combine(Application.streamingAssetsPath, "file.env");


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        StartCoroutine(LoadEnvFile());
    }

    private IEnumerator LoadEnvFile()
    {
#if !UNITY_ANDROID
        string[] lines = File.ReadAllLines(envFilePath);
        if (lines.Length == 0)
        {
            Debug.LogError("Failed to load file.env");
            yield break;
        }
        envVariables = ReadEnvFile(lines);
#elif UNITY_ANDROID
        UnityWebRequest request = UnityWebRequest.Get(envFilePath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load file.env");
            yield break;
        }
        string text = request.downloadHandler.text;
        string[] lines = text.Split('\n');
        envVariables = ReadEnvFile(lines);
#endif
    }

    private Dictionary<string, string> ReadEnvFile(string[] lines)
    {
        Dictionary<string, string> env = new Dictionary<string, string>();

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            {   
                string[] keyValue = line.Split(new[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    // Replace double underscores with a single underscore
                    key = key.Replace("__", "_");

                    env[key] = value;
                }
            }
        }
        return env;
    }
}
