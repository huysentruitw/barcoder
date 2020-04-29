# Barcoder - Barcode Encoding Library

[![Build status](https://ci.appveyor.com/api/projects/status/x6dhc3m70nxj30mx/branch/master?svg=true)](https://ci.appveyor.com/project/huysentruitw/barcoder/branch/master)

Lightweight Barcode Encoding Library for .NET Framework, .NET Standard and .NET Core. Additional packages are available for rendering the generated barcode to SVG or an image (currently only PNG is supported).

Code ported from the GO project https://github.com/boombuler/barcode by [Florian Sundermann](https://github.com/boombuler).

Supported Barcode Types:

* 2 of 5
* Aztec Code
* Codabar
* Code 39
* Code 93
* Code 128
* Data Matrix (ECC 200)
* Data Matrix GS1
* EAN 8
* EAN 13
* PDF 417
* QR Code

## NuGet package

To install the main package:

    PM> Install-Package Barcoder

To install the SVG renderer:

    PM> Install-Package Barcoder.Renderer.Svg

To install the image renderer:

	PM> Install-Package Barcoder.Renderer.Image
	
## Usage - render to SVG

```csharp
var barcode = Code128Encoder.Encode("FOO/BAR/12345");
var renderer = new SvgRenderer();

using (var stream = new MemoryStream())
using (var reader = new StreamReader(stream))
{
    renderer.Render(barcode, stream);
    stream.Position = 0;

    string svg = reader.ReadToEnd();
    Console.WriteLine(svg);
}
```

## Usage - render to PNG, JPEG, GIF or BMP

Example for rendering to PNG:

```csharp
var barcode = QrEncoder.Encode("Hello World!");
var renderer = new ImageRenderer(imageFormat: ImageFormat.Png);

using (var stream = new FileStream("output.png", FileMode.Create))
{
    renderer.Render(barcode, stream);
}
```

Supported image formats can be found [here](/src/Barcoder.Renderer.Image/ImageFormat.cs)
