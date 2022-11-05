 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flowing.Triangulation
{
    internal enum PropertyConstants
    {
        Marked, FaceListIndex, Median, IncidentEdges, HeVertexIndex
    }

    internal class DynamicProperties
    {
        private Dictionary<PropertyConstants, object> _properties = new Dictionary<PropertyConstants, object>();
        public int Count { get { return _properties.Count; } }

        internal void AddProperty(PropertyConstants key, object value)
        {
            if (_properties.ContainsKey(key))
                _properties[key] = value;
            else
            {
                _properties.Add(key, value);
            }
        }

        internal bool ExistsKey(PropertyConstants key)
        {
            if (_properties.ContainsKey(key))
                return true;
            return false;
        }

        internal object GetValue(PropertyConstants key)
        {
            return _properties[key];
        }

        internal void ChangeValue(PropertyConstants key, object value)
        {
            if (!ExistsKey(key))
                throw new Exception("Key " + key + " was not found.");
            _properties[key] = value;
        }

        internal void Clear()
        {
            _properties.Clear();
        }

        internal void RemoveKey(PropertyConstants key)
        {
            _properties.Remove(key);
        }
    }
}
