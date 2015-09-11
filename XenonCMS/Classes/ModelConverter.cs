using System;
using System.Collections.Generic;
using System.Reflection;

namespace XenonCMS.Classes
{
    static public class ModelConverter
    {
        // Create and return a new destination model of the given type from the given source model
        static public T Convert<T>(object sourceModel)
        {
            var Result = (T)Activator.CreateInstance(typeof(T));
            Convert(sourceModel, Result);
            return Result;
        }

        // Create and return a new list of destination models of the given type from the given array of source models
        static public List<T> Convert<T>(object[] sourceModel)
        {
            var Result = new List<T>();
            for (int i = 0; i < sourceModel.Length; i++)
            {
                Result.Add((T)Activator.CreateInstance(typeof(T)));
                Convert(sourceModel[i], Result[i]);
            }
            return Result;
        }
        
        // Convert the given source model to the given destination model
        static public void Convert(object sourceModel, object destinationModel)
        {
            // Loop through viewmodel properties, applying any matching property values found in domainmodel
            PropertyInfo[] Properties = destinationModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo Property in Properties)
            {
                // Ensure we only look at read+write properties (read only helper properties should not be loaded from/saved to an ini)
                if ((Property.CanRead) && (Property.CanWrite))
                {
                    var SourceProperty = sourceModel.GetType().GetProperty(Property.Name);
                    if (SourceProperty != null)
                    {
                        Property.SetValue(destinationModel, SourceProperty.GetValue(sourceModel));
                    }
                }
            }
        }
    }
}