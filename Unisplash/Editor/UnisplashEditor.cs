using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Unisplash))]
public class UnisplashEditor : Editor {

    [Serializable]
    private class UnsplashResponse
    {
        [Serializable]
        public class UnsplashUrlResponse
        {
            public string full;
            public string regular;
            public string small;
        }
        [Serializable]
        public class UnsplashUserResponse
        {
            public string name;
        }

        public string description;
        public UnsplashUrlResponse urls;
        public UnsplashUserResponse user;
    }

    private WWW wwwAPI, wwwImage;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh"))
        {
            RefreshImage();
        }
    }

    private void RefreshImage()
    {
        wwwAPI = new WWW("https://api.unsplash.com/photos/random?client_id=8dbfe94859c0d407f88c0f2b2ba5a856b1f35e86cd62490f4c1f9f958a70faa9");

        EditorApplication.update += Update;
    }
    private void Update()
    {
        if (wwwAPI != null && wwwAPI.isDone)
        {
            if (string.IsNullOrEmpty(wwwAPI.error) == false)
            {
                Debug.LogError(wwwAPI.error);
                EditorApplication.update -= Update;
            }
            else
            {

                var json = wwwAPI.text;
                var resp = JsonUtility.FromJson<UnsplashResponse>(json);
                var unisplash = (Unisplash)target;

                unisplash.fullURL = resp.urls.full;
                unisplash.smallURL = resp.urls.small;
                unisplash.regularURL = resp.urls.regular;
                unisplash.description = resp.description;
                unisplash.author = resp.user.name;

                wwwImage = new WWW(resp.urls.full);
            }

            wwwAPI = null;
        }
        if (wwwImage != null && wwwImage.isDone)
        {
            if (string.IsNullOrEmpty(wwwImage.error) == false)
            {
                Debug.LogError(wwwImage.error);
            }
            else
            {
                var texture = wwwImage.texture;
                var unisplash = (Unisplash)target;
                unisplash.GetComponent<UnityEngine.UI.RawImage>()
                    .texture = texture;

                var fitter = unisplash.GetComponent<UnityEngine.UI.AspectRatioFitter>();
                if (fitter != null)
                {
                    fitter.aspectRatio = (float)texture.width / texture.height;
                }
            }

            EditorApplication.update -= Update;
            wwwImage = null;
        }
    }
}
