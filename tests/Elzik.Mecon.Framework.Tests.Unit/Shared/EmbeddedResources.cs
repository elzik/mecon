using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;
using Elzik.Mecon.Framework.Tests.Unit.Infrastructure.Plex;

namespace Elzik.Mecon.Framework.Tests.Unit.Shared
{
    public static class EmbeddedResources
    {
        internal static T GetXmlTestData<T>(string projectFilePath)
        {
            var dataStream = GetEmbeddedResourceStream(projectFilePath);

            var xmlSerialiser = new XmlSerializer(typeof(T));
            var data = (T)xmlSerialiser.Deserialize(dataStream);

            return data;
        }

        internal static T GetJsonTestData<T>(string projectFilePath)
        {
            var dataStream = GetEmbeddedResourceStream(projectFilePath);

            var data = JsonSerializer.Deserialize<T>(dataStream, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            return data;
        }

        private static Stream GetEmbeddedResourceStream(string projectFilePath)
        {
            var assemblyName = typeof(PlexUsersTests).Assembly.GetName().Name;
            var resourceStreamName = $"{assemblyName}.{projectFilePath.TrimStart('/').Replace('/', '.')}";

            var embeddedResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceStreamName);
            if (embeddedResourceStream == null)
            {
                throw new InvalidOperationException($"{resourceStreamName} embedded resource not found.");
            }

            return embeddedResourceStream;
        }
    }
}
