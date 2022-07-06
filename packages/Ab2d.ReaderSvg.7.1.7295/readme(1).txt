-------------------------------------------------
-----         Ab2d.ReaderSvg Readme         -----
-------------------------------------------------

Ab2d.ReaderSvg is the ultimate svg file importer for WPF. 
The library can import DPI independent vector drawings
from svg files into WPF shapes or geometry objects.


Samples project:
https://github.com/ab4d/Ab2d.ReaderSvg.Wpf.Samples

Homepage:
https://www.ab4d.com/ReaderSvg.aspx

Online reference help:
https://www.ab4d.com/help/ReaderSvg/html/R_Project_Ab2d_ReaderSvg_Help.htm


ViewerSvg:
ViewerSvg is a svg to xaml converter application.
It is not part of this NuGet, but can be downloaded from
https://www.ab4d.com/Downloads.aspx or from Users Account web page (commercial users).
See more: https://www.ab4d.com/ViewerSvg.aspx


Supported platforms:
- .NET Framework 4.0+
- .NET Core 3.1


This version of Ab2d.ReaderSvg can be used as an evaluation and as a commercial version.

Evaluation usage:
On the first usage of the library, a dialog to start a 60-days evaluation is shown.
The evaluation version offers full functionality of the library but displays an evaluation
info dialog once a day and occasionally shows a "Ab2d.ReaderSvg evaluation" watermark text.
When the evaluation is expired, you can ask for evaluation extension or restart 
the evaluation period when a new version of the library is available.

You can see the prices of the library and purchase it on 
https://www.ab4d.com/Purchase.aspx#ReaderSvg

Commercial usage:
In case you have purchased a license, you can get the license parameters
from your User Account web page (https://www.ab4d.com/UserLogIn.aspx).
Then set the parametes with adding the following code before the library is used:

Ab2d.Licensing.ReaderSvg.LicenseHelper.SetLicense(licenseOwner: "[CompanyName]", 
                                                  licenseType: "[LicenseType]", 
                                                  license: "[LicenseText]");

Note that the version that is distributed as NuGet package uses a different licensing
mechanism then the commercial version that is distributed with a windows installer. 
Also the LicenseText that is used as a parameter to the SetLicense method is different 
then the license key used in the installer.