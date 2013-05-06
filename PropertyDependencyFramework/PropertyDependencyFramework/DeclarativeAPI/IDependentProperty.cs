using System;

namespace PropertyDependencyFramework
{
	public interface IDependentProperty
	{
		IDependentProperty Depends(Action<IPropertyDependency> deferredPropDependency);
	}
}