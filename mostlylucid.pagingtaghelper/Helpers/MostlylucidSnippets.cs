﻿using mostlylucid.pagingtaghelper.Components;

namespace mostlylucid.pagingtaghelper.Helpers;

    public static class MostlylucidSnippets
    {
        public static string Pagesizeonchange
            => GetString(nameof(Pagesizeonchange));
        
        
        public static string HTMXPageSizeChange => GetString(nameof(HTMXPageSizeChange));
        public static string PlainViewCSS => GetString(nameof(PlainViewCSS));
        public static string PlainViewMinCSS => GetString(nameof(PlainViewMinCSS));

        private static string GetString(string name)
        {
            var assembly = typeof(PagerTagHelper).Assembly;
            using var resource = assembly.GetManifestResourceStream(name);

            if (resource == null)
                throw new ArgumentException($"Resource \"{name}\" was not found.", nameof(name));
            
            using var reader = new StreamReader(resource);
            return reader.ReadToEnd();
        }
    }