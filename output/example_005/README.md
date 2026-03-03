# Example 005 - SourceFaces Derivatives

This output contains 10 newly generated portraits derived from images in the `SourceFaces` directory.

## Source Research Summary
- Source folder: `SourceFaces/`
- Total source images found: `137`
- Typical source size: `384x615`
- Naming pattern: `<Man|Woman><ID>_<Style>.png`
- Example styles: `AVest`, `FlightSuit`, `HelmetOff`, `HelmetOn`, `HelmetVisor`, `HelmetVisorGold`

## Generated Outputs
- `Assets/Resources/GeneratedFaces/sciface_gen_01_Man02_HelmetVisorGold.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_02_Man05_FlightSuit.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_03_Man08_HelmetOff.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_04_Man11_HelmetOn.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_05_Man13_AVest.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_06_Woman01_HelmetVisor.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_07_Woman03_FlightSuit.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_08_Woman06_HelmetOff.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_09_Woman08_HelmetOn.png`
- `Assets/Resources/GeneratedFaces/sciface_gen_10_Woman10_AVest.png`

All generated images are `512x512` PNG.

## Unity Load Example
```csharp
Sprite s = Resources.Load<Sprite>("GeneratedFaces/sciface_gen_01_Man02_HelmetVisorGold");
```
