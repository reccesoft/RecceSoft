using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecceSoft.Result
{
    public class Result
    {
        public const string DefaultOkMessage = "Operation Succeeded";
        public const string DefaultErrorMessage = "Operation did not complete successfully";
        public const string DefaultNoActionTakenMessage = "No Action taken";
        public const string DefaultGenericException = "An exception occurred";
        public const string DefaultErrorMessageDatabaseTransaction = "A database transaction error occurred";

        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; } = new();

        public Result()
        {
            IsSuccess = false;
            Messages = new() { DefaultNoActionTakenMessage };
        }

        public static Result Ok(string okMessage = null)
        {
            Result vtr = new();
            vtr.SetOk(okMessage);
            return vtr;
        }

        public static Result Error(string errorMessage = null)
        {
            Result vtr = new();
            vtr.SetError(errorMessage);
            return vtr;
        }

        public static Result NotFound(string itemName = null)
        {
            Result vtr = new();
            vtr.SetNotFound(itemName);
            return vtr;
        }

        public static Result OkOrNotFound<T>(T item, string itemNameNotFound = null, string okMessageIfItemIsFound = null)
        {
            Result vtr = new();
            vtr.SetOkOrNotFound(item, itemNameNotFound, okMessageIfItemIsFound);
            return vtr;
        }

        public static async Task<Result> OkOrDatabaseError(Func<Task<int>> performDatabaseTransaction, string databaseErrorMessage = null, string databaseSuccessMessage = null)
        {
            Result vtr = new();
            await vtr.SetOkOrDatabaseError(performDatabaseTransaction, databaseErrorMessage, databaseSuccessMessage);
            return vtr;
        }

        public void SetOk(List<string> messages = null)
        {
            IsSuccess = true;
            Messages = messages?.Any() ?? false ? messages : new() { DefaultOkMessage };
        }

        public void SetOk(string message)
        {
            message = String.IsNullOrWhiteSpace(message) ? DefaultOkMessage : message;
            SetOk(new List<string>() { message });
        }

        public void SetError(List<string> messages = null)
        {
            IsSuccess = false;
            Messages = messages?.Any() ?? false ? messages : new() { DefaultErrorMessage };
        }

        public void SetError(string message)
        {
            message = String.IsNullOrWhiteSpace(message) ? DefaultErrorMessage : message;
            SetError(new List<string>() { message });
        }

        public void SetNotFound(string itemName = null)
        {
            itemName = String.IsNullOrWhiteSpace(itemName) ? "Object Requested" : itemName;
            string message = $"{itemName} Not Found";
            IsSuccess = false;
            Messages = new() { message };
        }

        public void SetOkOrNotFound<T>(T item, string itemNameNotFound = null, string okMessageIfItemIsFound = null)
        {
            if (item == null)
            {
                SetNotFound(itemNameNotFound);
            }
            else
            {
                SetOk(okMessageIfItemIsFound);
            }
        }

        public async Task SetOkOrDatabaseError(Func<Task<int>> performDatabaseTransaction, string databaseErrorMessage = null, string databaseSuccessMessage = null)
        {
            if (string.IsNullOrWhiteSpace(databaseErrorMessage))
            {
                databaseErrorMessage = DefaultErrorMessageDatabaseTransaction;
            }

            if (performDatabaseTransaction == null)
            {
                SetError(databaseErrorMessage);
            }
            else
            {
                var rowsAffected = await performDatabaseTransaction();
                if (rowsAffected > 0)
                {
                    SetOk(databaseSuccessMessage);
                }
                else
                {
                    SetError(databaseErrorMessage);
                }
            }
        }

        

    }

    public class Result<T> : Result
    {
        public T ReturnedObject { get; set; }

        public Result() : base()
        {
        }



        public void SetOkAndAttachReturnedObject(T itemToReturn, List<string> messages = null)
        {
            SetOk(messages);
            ReturnedObject = itemToReturn;
        }

        public void SetOkAndAttachReturnedObject(T itemToReturn, string message)
        {
            SetOk(message);
            ReturnedObject = itemToReturn;
        }

        public void SetOkOrNotFoundAndAttachReturnedObject(T item, string itemNameNotFound = null, string okMessageIfItemIsFound = null)
        {
            ReturnedObject = item;
            SetOkOrNotFound(item, itemNameNotFound, okMessageIfItemIsFound);
        }

        public async Task SetOkOrNotFoundAndAttachReturnedObjectFromMethod<Y>(Y item, Func<Task<T>> getObjectToReturnIfObjectWasFound, string itemNameNotFound = null, string okMessageIfItemIsFound = null)
        {
            SetOkOrNotFound(item, itemNameNotFound, okMessageIfItemIsFound);
            if (IsSuccess && getObjectToReturnIfObjectWasFound != null)
            {
                ReturnedObject = await getObjectToReturnIfObjectWasFound();
            }
        }

        public async Task SetOkOrDatabaseErrorAndAttachReturnedObject(Func<Task<int>> performDatabaseTransaction, Func<Task<T>> getObjectToReturnOnlyWhenDatabaseTransactionSuccessful, string databaseErrorMessage = null, string databaseSuccessMessage = null)
        {
            await SetOkOrDatabaseError(performDatabaseTransaction, databaseErrorMessage, databaseSuccessMessage);
            if (IsSuccess && getObjectToReturnOnlyWhenDatabaseTransactionSuccessful != null)
            {
                ReturnedObject = await getObjectToReturnOnlyWhenDatabaseTransactionSuccessful();
            }
        }

        public static Result<T> OkAndAttachReturnedObject(T objectToReturn, string okMessage = null)
        {
            Result<T> vtr = new();
            vtr.SetOkAndAttachReturnedObject(objectToReturn, okMessage);
            return vtr;
        }


        public static Result<T> OkOrNotFoundAndAttachReturnedObject(T item, string itemNameNotFound = null, string okMessageIfItemIsFound = null)
        {
            Result<T> vtr = new();
            vtr.SetOkOrNotFoundAndAttachReturnedObject(item, itemNameNotFound, okMessageIfItemIsFound);
            return vtr;
        }

        public static async Task<Result<T>> OkOrNotFoundAndAttachReturnedObjectFromMethod<Y>(Y item, Func<Task<T>> getObjectToReturnIfObjectWasFound, string itemNameNotFound = null, string okMessageIfItemIsFound = null)
        {
            Result<T> vtr = new();
            await vtr.SetOkOrNotFoundAndAttachReturnedObjectFromMethod(item, getObjectToReturnIfObjectWasFound, itemNameNotFound, okMessageIfItemIsFound);
            return vtr;
        }

        public static async Task<Result<T>> OkOrDatabaseErrorAndAttachReturnedObject(Func<Task<int>> performDatabaseTransaction, Func<Task<T>> getObjectToReturnOnlyWhenDatabaseTransactionSuccessful, string databaseErrorMessage = null, string databaseSuccessMessage = null)
        {
            Result<T> vtr = new();
            await vtr.SetOkOrDatabaseErrorAndAttachReturnedObject(performDatabaseTransaction, getObjectToReturnOnlyWhenDatabaseTransactionSuccessful, databaseErrorMessage, databaseSuccessMessage);
            return vtr;
        }

      
    }
}
