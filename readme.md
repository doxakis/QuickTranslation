# QuickTranslation [![Build Status](https://travis-ci.org/doxakis/QuickTranslation.svg?branch=master)](https://travis-ci.org/doxakis/QuickTranslation) [![NuGet Status](https://badge.fury.io/nu/QuickTranslation.svg)](https://www.nuget.org/packages/QuickTranslation)
Embeddable translation in code, views, controllers, etc.

# Install from Nuget
To get the latest version:
```
Install-Package QuickTranslation
```

# Examples

## Basic usage

```csharp
var text = QuickTranslation.Prepare(@"
	# Comment.
	010: english1
	- fr: french1
	- es: spanish1
	
	020: english2
	030: english3
");

Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
Assert.AreEqual("english1", text["010"]);
Assert.AreEqual("english2", text["020"]);
Assert.AreEqual("english3", text["030"]);

Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");
Assert.AreEqual("french1", text["010"]);
Assert.AreEqual("english2", text["020"]);
Assert.AreEqual("english3", text["030"]);
```

## Embed in controller

```csharp
public class HomeController : Controller
{
	static QuickTranslation Translations = QuickTranslation.Prepare(@"
		MESSAGE_001: This is a message from the controller.
			- fr: Ceci est un message du contrôleur.
	");

	public IActionResult Index()
	{
		ViewBag.Message = Translations["MESSAGE_001"];
		return View();
	}
}
```

## Embed in view (.cshtml)

```csharp
@using QuickTranslationPack;
@using Newtonsoft.Json;

@{
	var text = QuickTranslation.Prepare(@"
		# Header.
		Title: Home Page
		- fr: Page d'accueil
		Welcome: Welcome {0},
		- fr: Bonjour {0},

		# Toggle language.
		Toggle_lang_desc: Changer à français
		- fr: Switch to english
		Toggle_lang_code: fr
		- fr: en
	");

	ViewData["Title"] = text["Title"];
}

<h1>@string.Format(text["Welcome"], "User1")</h1>
<pre>@JsonConvert.SerializeObject(text.GetTranslations(), Formatting.Indented)</pre>

<p>@Html.ActionLink(text["Toggle_lang_desc"], "Index", new { lang = text["Toggle_lang_code"] })</p>

<p>@ViewBag.Message</p>
```

# Copyright and license
Code released under the MIT license.
