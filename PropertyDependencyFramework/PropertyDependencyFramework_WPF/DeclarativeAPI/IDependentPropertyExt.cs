using System;

namespace PropertyDependencyFramework.DeclarativeAPI
{
	public interface IDependentPropertyExt
	{
		IDependentPropertyExt Depends(Action<IPropertyDependencyExt> deferredPropDependency);
	}

    public interface IDependentPropertyForTypeExt
    {
        IDependentPropertyForTypeExt Depends(Action<IPropertyDependencyForTypeExt> deferredPropDependency);
    }
}