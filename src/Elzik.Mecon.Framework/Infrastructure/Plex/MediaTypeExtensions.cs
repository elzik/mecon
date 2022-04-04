﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elzik.Mecon.Framework.Domain;

namespace Elzik.Mecon.Framework.Infrastructure.Plex
{
    public static class MediaTypeExtensions
    {
        public static string ToPlexLibraryType(this MediaType mediaType)
        {
            return mediaType switch
            {
                MediaType.Movie => "movie",
                MediaType.TvShow => "show",
                _ => throw new InvalidOperationException($"MediaType {mediaType} " +
                                                         "cannot be converted to a Plex library type.")
            };
        }

        public static IEnumerable<string> ToPlexLibraryTypes(this IEnumerable<MediaType> mediaTypes)
        {
            return mediaTypes == null ? 
                Enum.GetValues<MediaType>().Select(type => type.ToPlexLibraryType()) : 
                mediaTypes.Select(type => type.ToPlexLibraryType());
        }
    }
}
