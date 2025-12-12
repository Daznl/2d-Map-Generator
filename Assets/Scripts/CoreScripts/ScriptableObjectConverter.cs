using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ScriptableObjectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(ScriptableObject).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var instance = ScriptableObject.CreateInstance(objectType);
        serializer.Populate(reader, instance);
        return instance;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
