#Tidy Tabs for Visual Studio

_Based on [Tidy Tabs for Sublime Text](https://github.com/bradleyboy/TidyTabs-Sublime) by Brad Daily_


> Ever get to the end (middle?) of a day of coding only to find your window littered with tabs that are nearly impossible to search through visually? Find yourself rage-quitting all your tabs in disgust and starting over? No? Just me? Ok.

Tidy Tabs is a Visual Studio extension that keeps your document well organized by closing document tabs you are no longer using. 

- - -

## Installation

[Install from the Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/b80ab284-83f8-4022-bc78-95af126ba5f0)

---

## Usage

Close unused tabs with Tidy Tabs using the keyboard shortcut CTRL + ALT + ESC at any time.

By default Tidy Tabs will also close unused tabs whenever you save a document.

Tidy Tabs will never close the active document or any pinned tabs.

- - -

## Settings

The following default settings can be modified by opening *Tools -> Options -> Tidy Tabs* in Visual Studio:

***Close on Save*** (*default: true*)

If you only want Tidy Tabs to run when triggered with the keyboard shortcut set this to false.

***Timeout Minutes*** (*default: 10 minutes*)

Once a tab has not be viewed for this length of time Tidy Tabs will close it next time it is run. Tab timeouts are paused when you are outside of Visual Studio and resume when it becomes the active application.

***Open Tab Threshold*** (*default: 0*)

If this value is greater than 0 then Tidy Tabs will only attempt to close tabs when the number of open windows exceeds this value.

***Maximum Open Tabs*** (*default: 0*)

If this value is greater than 0 then Tidy Tabs will close the oldest tabs until the number of open tabs is equal to this value. The oldest tabs will be closed even if they have not exceeded the timeout value.

---

## Contribute

[Submit issues and Pull Requests at Github](https://github.com/davemckeown/TidyTabs-VisualStudio)
