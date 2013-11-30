using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

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


        internal static PropertyChangeRecorder CreatePropertyChangeRecorder<TBindable, TProperty>(TBindable bindable, Expression<Func<TBindable, TProperty>> propertyExpression)
            where TBindable : INotifyPropertyChanged
        {
            var recorder = new PropertyChangeRecorder();

            var compiledExpression = propertyExpression.Compile();
            var initialValue = compiledExpression(bindable);

            var propertyName = PropertyNameResolver.GetPropertyName(propertyExpression);
            bindable.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    recorder.NumberOfChanges++;
                    recorder.NewValues.Add(compiledExpression(bindable));
                }
            };

            return recorder;
        }

    }
}
