using System;

namespace PropertyDependencyFramework.DeclarativeAPI
{
	public interface IDependentPropertyExt
	{
		IDependentPropertyExt Depends(Action<IPropertyDependencyExt> deferredPropDependency);
	}
}