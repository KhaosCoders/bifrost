﻿using System.Linq.Expressions;
using System.Reflection;

namespace Bifrost.Client.Utils.Validation;

public abstract class ValidationRule
{
    public abstract ValidationFault? Validate();
}

public class ValidationRule<T>(Expression<Func<T>> Accessor, Func<T, bool> Validator) : ValidationRule
{
    public string Name { get; } =
        Accessor.Body is MemberExpression member && member.Member is PropertyInfo property
            ? property.Name
            : throw new ArgumentException("Validation only possible on properties");

    public override ValidationFault? Validate()
    {
        try
        {
            T value = Accessor.Compile().Invoke();
            if (Validator(value))
            {
                return null;
            }

            return new ValidationFault(Name, "Validation failed");
        }
        catch (Exception ex)
        {
            return new ValidationFault(Name, ex.Message);
        }
    }
}
