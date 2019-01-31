Common use codes
===============
###Web
-----------

```
string appRootUrl = (HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath).TrimEnd('/');
```

