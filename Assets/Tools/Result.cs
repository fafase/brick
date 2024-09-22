using UnityEditor.VersionControl;
using UnityEngine;

namespace Tools
{

    public class Result<T>
    {
        public readonly bool IsSuccess;
        public readonly T Obj;
        public readonly string Message;

        public Result()
        {
            this.Obj = default(T);
            this.Message = string.Empty;
            IsSuccess = false;
        }

        public Result(T obj, string message, bool isSuccess)
        {
            this.Obj = obj;
            this.Message = message;
            IsSuccess = isSuccess;
            if(!IsSuccess && string.IsNullOrEmpty(message)) 
            {
                Debug.LogError("Missing message when failure");
            }
        }

        public static Result<T> Success() => new Result<T>(default(T), null, true);
        public static Result<T> Success(T obj) => new Result<T>(obj, null, true);
        public static Result<T> Failure(string message) => new Result<T>(default(T), message, false);

        public void CheckForDebug() 
        {
            if (!IsSuccess) 
            {
                Debug.Log(Message);
            }
        }
    }
}