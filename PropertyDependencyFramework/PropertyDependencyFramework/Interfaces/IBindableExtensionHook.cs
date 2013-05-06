namespace PropertyDependencyFramework
{
	public interface IBindableExtensionHook
	{
		void AfterUnregisterAllPropertyDependencies();
		void AfterOnPropertyChanged(string propertyName);
		void BeforeCachedValue(string propertyName);
	}
}