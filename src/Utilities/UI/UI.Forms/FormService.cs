using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Riverside.Cms.Utilities.UI.Forms
{
    public class FormService : IFormService
    {
        private string GetResourceText(Type resourceType, string resourceName)
        {
            ResourceManager manager = new ResourceManager(resourceType);
            return manager.GetString(resourceName);
        }

        private string GetResourceMessage<T>(PropertyInfo property) where T : ValidationAttribute
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(T), true);
            if (customAttributes != null && customAttributes.Length == 1)
            {
                T attribute = (T)customAttributes[0];
                return GetResourceText(attribute.ErrorMessageResourceType, attribute.ErrorMessageResourceName);
            }
            return null;
        }

        private string GetId(PropertyInfo property)
        {
            return char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
        }

        private string GetLabel(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (customAttributes != null && customAttributes.Length == 1)
                return ((DisplayAttribute)customAttributes[0]).GetName();
            return property.Name;
        }

        private bool GetRequired(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(RequiredAttribute), true);
            return customAttributes != null && customAttributes.Length == 1;
        }

        private int? GetMinLength(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(StringLengthAttribute), true);
            if (customAttributes != null && customAttributes.Length == 1)
                return ((StringLengthAttribute)customAttributes[0]).MinimumLength;
            return null;
        }

        private int? GetMaxLength(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(StringLengthAttribute), true);
            if (customAttributes != null && customAttributes.Length == 1)
                return ((StringLengthAttribute)customAttributes[0]).MaximumLength;
            return null;
        }

        private string GetPattern(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(RegularExpressionAttribute), true);
            if (customAttributes != null && customAttributes.Length == 1)
                return ((RegularExpressionAttribute)customAttributes[0]).Pattern;
            return null;
        }

        private int? GetRows(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(MultilineAttribute), true);
            if (customAttributes != null && customAttributes.Length == 1)
                return ((MultilineAttribute)customAttributes[0]).Rows;
            return null;
        }

        private IField GetTextField(PropertyInfo property, string id)
        {
            return new TextField
            {
                Id = id,
                Label = GetLabel(property),
                Pattern = GetPattern(property),
                Required = GetRequired(property),
                RequiredMessage = GetResourceMessage<RequiredAttribute>(property),
                MinLength = GetMinLength(property),
                MaxLength = GetMaxLength(property),
                LengthMessage = GetResourceMessage<StringLengthAttribute>(property),
                Rows = GetRows(property)
            };
        }

        private IField GetIntegerField(PropertyInfo property, string id)
        {
            return new IntegerField
            {
                Id = id,
                Label = GetLabel(property)
            };
        }

        private string GetEmptyOptionLabel(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(OptionAttribute), true);
            if (customAttributes != null && customAttributes.Length == 1)
            {
                OptionAttribute attribute = (OptionAttribute)customAttributes[0];
                if (attribute.EmptyOptionResourceName != null)
                    return GetResourceText(attribute.ResourceType, attribute.EmptyOptionResourceName);
            }
            return null;
        }

        private string GetNullOptionLabel(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(OptionAttribute), true);
            if (customAttributes != null && customAttributes.Length == 1)
            {
                OptionAttribute attribute = (OptionAttribute)customAttributes[0];
                if (attribute.NullOptionResourceName != null)
                    return GetResourceText(attribute.ResourceType, attribute.NullOptionResourceName);
            }
            return null;
        }

        private bool IsOption(PropertyInfo property)
        {
            object[] customAttributes = property.GetCustomAttributes(typeof(OptionAttribute), true);
            return customAttributes != null && customAttributes.Length == 1;
        }

        private IField GetOptionField(PropertyInfo property, string id)
        {
            return new OptionField
            {
                Id = id,
                Label = GetLabel(property),
                Required = GetRequired(property),
                RequiredMessage = GetResourceMessage<RequiredAttribute>(property),
                EmptyOptionLabel = GetEmptyOptionLabel(property),
                NullOptionLabel = GetNullOptionLabel(property)
            };
        }

        private IField GetDateTimeField(PropertyInfo property, string id)
        {
            return new DateTimeField
            {
                Id = id,
                Label = GetLabel(property)
            };
        }

        public IDictionary<string, IField> ListFields<T>()
        {
            Dictionary<string, IField> fields = new Dictionary<string, IField>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                string id = GetId(property);
                IField field = null;
                if (IsOption(property))
                    field = GetOptionField(property, id);
                else if (property.PropertyType == typeof(DateTime))
                    field = GetDateTimeField(property, id);
                else if (property.PropertyType == typeof(int))
                    field = GetIntegerField(property, id);
                else if (property.PropertyType == typeof(string))
                    field = GetTextField(property, id);
                if (field != null)
                    fields[id] = field;
            }
            return fields;
        }
    }
}
