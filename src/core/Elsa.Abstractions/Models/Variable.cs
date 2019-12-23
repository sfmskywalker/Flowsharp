﻿using Elsa.Converters;
using Newtonsoft.Json;

namespace Elsa.Models
{
    public class Variable
    {
        public static Variable From(object output)
        {
            return output != null ? new Variable(output) : null;
        }
        
        public Variable()
        {
        }

        public Variable(object value)
        {
            Value = value;
        }
    
        [JsonConverter(typeof(TypeNameHandlingConverter))]
        public object? Value { get; set; }

        public T GetValue<T>() => (T)Value;
    }
}