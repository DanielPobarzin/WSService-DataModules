using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Domain.Entities
{
    public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
    {
        private readonly string _root = "Entity";
        private readonly IDictionary<string, object> _expandObject = null;

        public Entity()
        {
            _expandObject = new ExpandoObject();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_expandObject.TryGetValue(binder.Name, out object value))
            {
                result = value;
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _expandObject[binder.Name] = value;

            return true;
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(_root);

            while (!reader.Name.Equals(_root))
            {
                string typeContent;
                Type underlyingType;
                var name = reader.Name;

                reader.MoveToAttribute("type");
                typeContent = reader.ReadContentAsString();
                underlyingType = Type.GetType(typeContent);
                reader.MoveToContent();
                _expandObject[name] = reader.ReadElementContentAs(underlyingType, null);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in _expandObject.Keys)
            {
                var value = _expandObject[key];
                WriteLinksToXml(key, value, writer);
            }
        }

        private void WriteLinksToXml(string key, object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);
            writer.WriteString(value.ToString());
            writer.WriteEndElement();
        }

        public void Add(string key, object value)
        {
            _expandObject.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _expandObject.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _expandObject.Keys; }
        }

        public bool Remove(string key)
        {
            return _expandObject.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _expandObject.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return _expandObject.Values; }
        }

        public object this[string key]
        {
            get
            {
                return _expandObject[key];
            }
            set
            {
                _expandObject[key] = value;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _expandObject.Add(item);
        }

        public void Clear()
        {
            _expandObject.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _expandObject.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _expandObject.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _expandObject.Count; }
        }

        public bool IsReadOnly
        {
            get { return _expandObject.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _expandObject.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _expandObject.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}