﻿using System.Collections.Generic;
using System.Net.Http.Headers;

class MetaData
{
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ResponseHeaders { get; }
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ContentHeaders { get; }

    public MetaData(IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, IEnumerable<KeyValuePair<string, IEnumerable<string>>> contentHeaders)
    {
        ResponseHeaders = responseHeaders;
        ContentHeaders = contentHeaders;
    }
}