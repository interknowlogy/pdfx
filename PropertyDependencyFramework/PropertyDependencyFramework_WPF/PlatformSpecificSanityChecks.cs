using System;
using System.Reflection;

namespace PropertyDependencyFramework
{
	public static class PlatformSpecificSanityChecks
	{
		public static void TryVerifyLastPropertySetterEquals(string expectedPropertySetterName)
		{
			if (GetLastPropertyGetterNameFromStackTrace() != expectedPropertySetterName)
				throw new InvalidOperationException("You have to be in the Property Getter of Property " + expectedPropertySetterName +
													 " in order to use this feature. You are in Property " + GetLastPropertyGetterNameFromStackTrace());
		}

		private static string GetLastPropertyGetterNameFromStackTrace()
		{
			var frames = new System.Diagnostics.StackTrace();
			for (var i = 0; i < frames.FrameCount; i++)
			{
				var frame = frames.GetFrame(i).GetMethod() as MethodInfo;
				if (frame != null)
					if (frame.IsSpecialName && frame.Name.StartsWith("get_"))
					{
						return frame.Name.Substring(4);
					}
			}
			throw new InvalidOperationException("GetLastPropertyGetterNameFromStackTrace can only be called from within a property getter");
		}

		public static void TryValidatePropertyHasSetter(Type type, string propertyName)
		{
			PropertyInfo typePropertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo declaringTypePropertyInfo = typePropertyInfo.DeclaringType.GetProperty(propertyName,
																								 BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			bool setterExists = declaringTypePropertyInfo.CanWrite;
			if (setterExists == false)
				throw new InvalidOperationException("Property " + propertyName + " is required to have a setter.");
		}
	}
}
