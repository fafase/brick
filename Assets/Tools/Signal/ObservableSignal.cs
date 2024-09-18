using System;
using System.Collections.Generic;
using UniRx;

namespace Tools
{
    public static class ObservableSignal
    {
        private static Dictionary<Type, object> s_subjects = new Dictionary<Type, object>();

        public static IObservable<T> AsObservable<T>() where T : SignalData => GetSubject<T>().AsObservable();

        public static IDisposable Subscribe<T>(IObserver<T> observer) where T : SignalData 
        {
            lock (s_subjects)
            {
                return GetSubject<T>().Subscribe(observer);
            }
        }

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onSignalReceived) where T : SignalData 
        {
            lock (s_subjects)
            {
                return observable.Subscribe(onSignalReceived);
            }
        }

        public static void Broadcast<T>(T signal) where T : SignalData 
        {
            lock (s_subjects)
            {
                GetSubject<T>()?.OnNext(signal);
            }
        }

        public static void BroadcastComplete<T>(T signal) where T : SignalData 
        {
            lock (s_subjects)
            {
                Broadcast<T>(signal);
                Complete<T>(signal);
            }
        }

        public static void Complete<T>(T signal) where T : SignalData 
        {
            lock (s_subjects)
            {
                var subject = GetSubject<T>();
                subject.OnCompleted();
                subject.Dispose();
                s_subjects.Remove(typeof(T));
            }
        }

        private static Subject<T> GetSubject<T>() where T : SignalData
        {
            Type t = typeof(T);
            if (!s_subjects.ContainsKey(t))
            {
                s_subjects[t] = new Subject<T>();
            }
            return  (Subject<T>)s_subjects[t];
        }
    }
}
