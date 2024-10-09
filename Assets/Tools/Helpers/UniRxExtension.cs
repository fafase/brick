using System;
using TMPro;
using UniRx;

namespace Tools
{ 
    public static class UniRxExtension 
    {
        public static IDisposable SubscribeToText<T>(this IObservable<T> source, TextMeshProUGUI text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        }
    }
}
