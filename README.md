# AuthorizeAttribute Extended #

**AuthorizeAttribute Extended** separates 403 (Forbidden) error code from 401 (Unauthorized) error code based on authentication and/or authorisation result, while ASP.NET's AuthorizeAttribute class only returns 401 (Unauthorized) error code.


## Package Status ##

* **AuthorizeAttribute Extended** [![](https://img.shields.io/nuget/v/Aliencube.AuthorizeAttribute.Extended.svg)](https://www.nuget.org/packages/Aliencube.AuthorizeAttribute.Extended/) [![](https://img.shields.io/nuget/dt/Aliencube.AuthorizeAttribute.Extended.svg)](https://www.nuget.org/packages/Aliencube.AuthorizeAttribute.Extended/)


## Getting Started ##

**AuthorizeAttribute Extended** works the exactly same as the existing `AuthorizeAttribute` class on [ASP.NET MVC](http://www.nuget.org/packages/Microsoft.AspNet.Mvc).


### Add/Update `FilterConfig.cs` ###

* `App_Start\FilterConfig.cs` should include the `AuthorizeAttribute` class.
* The `using` alias MUST be used to avoid confusion from the `System.Web.Mvc.AuthorizeAttribute` class.

```csharp
using AuthorizeAttribute = Aliencube.AuthorizeAttribute.Extended.AuthorizeAttribute;
...

public class FilterConfig
{
  public static void RegisterGlobalFilters(GlobalFilterCollection filters)
  {
    filters.Add(new AuthorizeAttribute());
  }
}
```


### Add/Update `Global.asax.cs` ###

* `Global.asax.cs` should include the `FilterConfig` to activate the `AuthorizeAttribute` class.

```csharp
// Global.asax.cs
public class MvcApplication : System.Web.HttpApplication
{
  protected void Application_Start()
  {
    AreaRegistration.RegisterAllAreas();
    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
    RouteConfig.RegisterRoutes(RouteTable.Routes);
  }
}
```


### Add/Update Controllers ###

* The `using` alias MUST be used to avoid confusion from the `System.Web.Mvc.AuthorizeAttribute` class.

```csharp
using AuthorizeAttribute = Aliencube.AuthorizeAttribute.Extended.AuthorizeAttribute;
...

[Authorize]
public partial class AccountController : Controller
{
  ...

  [HttpPost]
  [AllowAnonymous]
  public virtual async Task<ActionResult> Login(LoginViewModel model)
  {
    ...

    return View();
  }

  [Authorize(Roles = "User")]
  public virtual async Task<ActionResult> MyProfile()
  {
    ...

    return View();
  }

  ...
}
```

## Contribution ##

Your contributions are always welcome! All your work should be done in your forked repository. Once you finish your work, please send us a pull request onto our `dev` branch for review.


## License ##

**AuthorizeAttribute Extended** is released under [MIT License](http://opensource.org/licenses/MIT)

> The MIT License (MIT)
>
> Copyright (c) 2014 [aliencube.org](http://aliencube.org)
> 
> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 
> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
