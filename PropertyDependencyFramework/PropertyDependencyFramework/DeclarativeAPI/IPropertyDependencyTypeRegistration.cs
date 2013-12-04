using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework
{
    public interface IPropertyDependencyTypeRegistration
    {
        //TODO: New First Arg = "this" with matching type arg
        IPropertyDependencyTypeRegistration On<TSourceOwner, TSource, TProperty>(Expression<Func<TSourceOwner, TSource>> source, Expression<Func<TProperty>> sourceProperty)
            where TSource : INotifyPropertyChanged;
    }
}