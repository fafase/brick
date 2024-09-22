using UnityEditor.VersionControl;
using UnityEngine;

namespace Tools
{
    public class Result 
    {
        public readonly bool IsSuccess;
        public readonly string Message;
        public Result(bool isSuccess, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public static Result Success() => new Result(true);
        public static Result Failure(string message) => new Result(false, message);
        public void CheckForDebug()
        {
            if (!IsSuccess)
            {
                Debug.Log(Message);
            }
        }
    }

    public class Result<T> : Result
    {
        public readonly T Obj;

        public Result() :base (false, string.Empty)
        {
            Obj = default(T);
        }

        public Result(T obj, bool isSuccess, string message) : base (isSuccess, message)
        {
            Obj = obj;
            if(!IsSuccess && string.IsNullOrEmpty(message)) 
            {
                Debug.LogError("Missing message when failure");
            }
        }

        public static Result<T> Success(T obj) => new Result<T>(obj, true, null);
        public new static Result<T> Failure(string message) => new Result<T>(default(T), false, message);
    }
}