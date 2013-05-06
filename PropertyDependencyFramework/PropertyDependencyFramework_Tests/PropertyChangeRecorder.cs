using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyDependencyFramework_Tests
{
	public class PropertyChangeRecorder
	{
		public PropertyChangeRecorder()
		{
			NewValues = new List<object>();
		}

		public bool HasChanged
		{
			get { return NumberOfChanges > 0; }
		}

		public int NumberOfChanges { get; set; }

		public List<object> NewValues { get; private set; }
	}
}
