using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PropertyDependencyFramework
{
	public static class PlatformSpecificSanityChecks
	{
		public static void TryVerifyLastPropertySetterEquals(string expectedPropertySetterName)
		{
			//Cannot easily validate in Windows Store App
		}
		
		public static void TryValidatePropertyHasSetter(Type type, string propertyName)
		{
			//Cannot easily validate in Windows Store App
		}
	}
}
