Common use codes
===============
###Web
-----------

```
string appRootUrl = (HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath).TrimEnd('/');
```

###Creating web.template.config
-----------
1. open csproj file
2. add below script:
```
  <Target Name="BeforeBuild">
    <TransformXml Source="Web.Template.config" Transform="Web.$(Configuration).config" Destination="Web.config" />
  </Target>
```
###Copy config file from application project for unit test project
-----------
1. open project property
2. navigate to build option
3. copy below script to "post build"
```
copy "$(SolutionDir)SampleTest.NetAPI\bin\SampleTest.NetAPI.dll.config" $(ProjectDir)$(OutDir)$(TargetFileName).config"

```
