using System;

namespace PropertyDependencyFramework
{
    public interface IDependentPropertyTypeRegistration
    {
        IDependentPropertyTypeRegistration Depends(Action<IPropertyDependencyTypeRegistration> deferredPropDependency);
    }
}