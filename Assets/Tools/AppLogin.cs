using Cysharp.Threading.Tasks;
using System;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
//using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Tools
{
    public class AppLogin : MonoBehaviour
    {
        void Start()
        {
            InitializeServices()
                .Subscribe();
        }

        private IObservable<Unit> InitializeServices() 
        {
            return UnityServices.InitializeAsync().ToObservable()
                .ObserveOnMainThread()
                .Do(_ => Debug.Log("Initializing services"))
                .SelectMany(_ => InitializeAuthentication())
                .SelectMany(_ =>
                {
                    ILoader loader = GetComponent<ILoader>();
                    return loader.InitializeAll();
                })
                .DoOnCompleted(() => 
                    ObservableSignal.BroadcastComplete(new LoginSignalData()))
                .DoOnError(error => Debug.LogError(error));
        }

        private IObservable<Unit> InitializeAuthentication() 
        {
            PlayerAccountService.Instance.SignedIn += OnAccountSignedIn;
            Debug.Log($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

            Debug.Log(AuthenticationService.Instance.Profile);
            AuthenticationService.Instance.SignedIn += OnAuthSignedIn;
            AuthenticationService.Instance.SignedOut += OnSignedOut;
            AuthenticationService.Instance.SignInFailed += OnSignInFailed;

            return AuthenticationService.Instance
                .SignInAnonymouslyAsync()
                .ToObservable()
                .ObserveOnMainThread();
        }

        private void OnAccountSignedIn()
        {
            Debug.Log("Player Account Access token " + PlayerAccountService.Instance.AccessToken);
            AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }

        private void OnAuthSignedIn()
        {
            //Shows how to get a playerID
            Debug.Log($"PlayedID: {AuthenticationService.Instance.PlayerId}");

            //Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            const string successMessage = "Sign in anonymously succeeded!";
            Debug.Log(successMessage);
        }
        private void OnSignedOut() => Debug.Log("Signed Out!");


        private void OnSignInFailed(RequestFailedException exception)
            => Debug.LogError($"Sign in anonymously failed with error code: {exception.ErrorCode}");

        ////private void ApplyRemoteConfig(ConfigResponse response)
        ////{
        ////    switch (response.requestOrigin)
        ////    {
        ////        case ConfigOrigin.Default:
        ////            Debug.Log("No settings loaded this session and no local cache file exists; using default values.");
        ////            break;
        ////        case ConfigOrigin.Cached:
        ////            Debug.Log("No settings loaded this session; using cached values from a previous session.");
        ////            break;
        ////        case ConfigOrigin.Remote:
        ////            Debug.Log("New settings loaded this session; update values accordingly.");
        ////            break;
        ////    }
        ////}
    }
    public class LoginSignalData : SignalData
    {
    }
}
