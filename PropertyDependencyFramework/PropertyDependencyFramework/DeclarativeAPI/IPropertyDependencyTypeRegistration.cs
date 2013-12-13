using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework
{
    public interface IPropertyDependencyTypeRegistration
    {
        IPropertyDependencyTypeRegistration On<TSourceOwner, TProperty>(TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyPropertyChanged>> source, Expression<Func<TProperty>> sourceProperty);
    }
}