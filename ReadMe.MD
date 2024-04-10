Herein you will find a WIP application in .net8 that implements a conversion of the [duo_api_csharp library](https://github.com/duosecurity/duo_api_csharp)

The Duo.cs file in duo_api_csharp to pass unit tests needed a slight modification when moved from .net framework to .net (core)

The HasProxyServer method was changed from

``` if (requestObject.Proxy != null)```  
to  
``` if (requestObject.Proxy != null && requestObject.Proxy.GetProxy(new Uri("http://localhost:8080")) != null)```