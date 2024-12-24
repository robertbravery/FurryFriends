using System;
using Ardalis.GuardClauses;

namespace FurryFriends.Core.Common;

  public class ValidationResult<T>
  {
      private readonly bool _success;
      private readonly T? _value;
      private readonly string? _errorMessage;

      public static ValidationResult<T> CreateSuccess(T value)
      {
          return new ValidationResult<T>(true, value, null);
      }

      public static ValidationResult<T> CreateFailure(string errorMessage)
      {
          return new ValidationResult<T>(false, default(T), errorMessage);
      }

      private ValidationResult(bool success, T? value, string? errorMessage)
      {
          _success = success;
          _value = value;
          _errorMessage = errorMessage;
      }

      public bool Success => _success;

      public T Value
      {
          get
          {
              if (!_success)
                  throw new InvalidOperationException("Cannot access the value of an invalid ValidationResult.");
              return _value!;
          }
      }

      public string ErrorMessage
      {
          get
          {
              if (_success)
                  throw new InvalidOperationException("Cannot access the error message of a successful ValidationResult.");
              return _errorMessage!;
          }
      }
  }