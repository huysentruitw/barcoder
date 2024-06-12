# Barcoder - Barcode Encoding Library

[![Build status](https://github.com/logstore/barcoder/actions/workflows/build-test-publish.yml/badge.svg?branch=master)](https://github.com/logstore/barcoder/actions/workflows/build-test-publish.yml?query=branch%3Amaster)

Lightweight Barcode Encoding Library for .NET Framework, .NET Standard and .NET Core. Additional packages are available for rendering the generated barcode to SVG or an image.

The Barcoder.v2 is a fork of library Barcoder by [Huysentruit](https://github.com/huysentruitw).

Supported Barcode Types:

* 2 of 5
* Aztec Code
* Codabar
* Code 39
* Code 93
* Code 128
* Code 128 GS1
* Data Matrix (ECC 200)
* Data Matrix GS1
* EAN 8
* EAN 13
* KIX (used by PostNL)  
* PDF 417
* QR Code
* RM4SC (Royal Mail 4 State Code)
* UPC A
* UPC E

## NuGet package

To install the [main package](https://www.nuget.org/packages/barcoder2):

    PM> Install-Package barcoder2

To install the SVG renderer:

    PM> Install-Package barcoder2.Renderer.Svg

To install the image renderer[^1]:

	PM> Install-Package barcoder2.Renderer.Image
	
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
var renderer = new ImageRenderer(new ImageRendererOptions { ImageFormat = ImageFormat.Png });

using (var stream = new FileStream("output.png", FileMode.Create))
{
    renderer.Render(barcode, stream);
}
```

Supported image formats can be found [here](/src/Barcoder.Renderer.Image/ImageFormat.cs)

[^1]: The `Barcoder.Renderer.Image` package depends on the cross-platform `SixLabors.ImageSharp.Drawing` library. So when using this package, also respect their [LICENSE](https://github.com/SixLabors/ImageSharp.Drawing/blob/master/LICENSE).
