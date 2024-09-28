using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Floater : MonoBehaviour
{
    [SerializeField] private List<FloaterAsset> m_assets;

    public IObservable<float> MoveWithArc(string assetName, Vector2 start, Vector2 end, float arcHeight = 200f, float duration = 1f) 
    {
        if (string.IsNullOrEmpty(assetName)) 
        {
            Debug.LogWarning("Empty asset name");
            return Observable.Empty<float>();
        }

        var asset = m_assets.Find(asset => asset.name.Equals(assetName));
        if(asset == null) 
        {
            Debug.LogWarning("No asset for that name");
            return Observable.Empty<float>();
        }
        GameObject obj = new GameObject("Floater_" + assetName);
        Image image = obj.AddComponent<Image>();
        image.sprite = asset.sprite;
        obj.transform.position = start; 
        return MoveWithArc(obj.GetComponent<RectTransform>(), start, end, arcHeight, duration);
    }


    public IObservable<float> MoveWithArc(RectTransform uiElement, Vector2 start, Vector2 end, float arcHeight = 200f, float duration = 1f)
    {
        AnimationCurve speedCurve = CreateCurve();
        return Observable.Create<float>(observer =>
        {
            float elapsedTime = 0f;
            var disposable = Observable.EveryUpdate()
                .TakeWhile(_ => elapsedTime < duration)
                .Subscribe(_ =>
                {
                    elapsedTime += Time.deltaTime;
                    float linearT = elapsedTime / duration;
                    float t = speedCurve.Evaluate(linearT);
                    Vector3 linearPosition = Vector3.Lerp(start, end, t);

                    Vector3 direction = (end - start).normalized;
                    Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward);
                    float arcOffset = Mathf.Sin(t * Mathf.PI) * arcHeight;
                    Vector3 curvedPosition = linearPosition + perpendicular * arcOffset;
                    uiElement.position = curvedPosition;
                    observer.OnNext(linearT);
                },
                () =>
                {
                    uiElement.position = end;
                    observer.OnNext(1);
                    observer.OnCompleted();
                });

            return Disposable.Create(() =>
            {
                disposable.Dispose();
            });
        });
    }

    private static AnimationCurve CreateCurve()
    {
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        Keyframe[] keyframes = curve.keys;
        keyframes[1].inTangent = 3f;
        keyframes[1].outTangent = 0f;
        return new AnimationCurve(keyframes);
    }

    [Serializable]
    public class FloaterAsset 
    {
        public string name;
        public Sprite sprite;
    }
}
 