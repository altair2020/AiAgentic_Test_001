$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Drawing

$root = "c:\_Dev\AiAgentic\AiAgentic_Test_001"
$sourceDir = Join-Path $root "SourceFaces"
$outBase = Join-Path $root "output\example_006\Assets\Resources"
$onlineDir = Join-Path $outBase "OnlineRefs"
$outDir = Join-Path $outBase "GeneratedFaces"
$metaPath = Join-Path $root "output\example_006\online_sources.json"

New-Item -ItemType Directory -Path $onlineDir -Force | Out-Null
New-Item -ItemType Directory -Path $outDir -Force | Out-Null
Get-ChildItem "$onlineDir\*.png" -ErrorAction SilentlyContinue | Remove-Item -Force
Get-ChildItem "$outDir\*.png" -ErrorAction SilentlyContinue | Remove-Item -Force

$seeds = @("atlas","nova","orion","lyra","vega","apollo","zenith","aurora","cosmos","titan")
$selectedOnline = @()
for ($i = 0; $i -lt 10; $i++) {
    $seed = $seeds[$i]
    $url = "https://api.dicebear.com/9.x/adventurer/png?seed=$seed&size=512"
    $name = "online_ref_{0:D2}.png" -f ($i + 1)
    $localPath = Join-Path $onlineDir $name
    Invoke-WebRequest -Uri $url -OutFile $localPath

    $selectedOnline += [PSCustomObject]@{
        title = "DiceBear Adventurer seed=$seed"
        pageUrl = "https://www.dicebear.com/styles/adventurer/"
        fileUrl = $url
        localFile = "Assets/Resources/OnlineRefs/$name"
        license = "DiceBear open-source avatar API"
        artist = "DiceBear"
    }
}

$sourceFaces = Get-ChildItem "$sourceDir\*.png" | Sort-Object Name
if ($sourceFaces.Count -eq 0) { throw "No SourceFaces PNG files found." }
$rand = New-Object System.Random 6006

function Render-Variant([string]$onlinePath,[string]$sourcePath,[string]$outPath,[int]$idx){
    $w = 384; $h = 615
    $online = [System.Drawing.Bitmap]::FromFile($onlinePath)
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    $dst = New-Object System.Drawing.Bitmap $w,$h
    $g = [System.Drawing.Graphics]::FromImage($dst)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    $bg = New-Object System.Drawing.Drawing2D.LinearGradientBrush (New-Object System.Drawing.Rectangle(0,0,$w,$h)), ([System.Drawing.Color]::FromArgb(18,24,44)), ([System.Drawing.Color]::FromArgb(7,11,20)), 90
    $g.FillRectangle($bg, 0,0,$w,$h)

    $onlineDst = New-Object System.Drawing.Rectangle 20, 20, 344, 575
    $sourceDst = New-Object System.Drawing.Rectangle 28, 10, 328, 590

    $iaOnline = New-Object System.Drawing.Imaging.ImageAttributes
    $onlineMatrix = New-Object System.Drawing.Imaging.ColorMatrix @(
      (,$(,(1.06,0,0,0,0))),
      (,$(,(0,1.04,0,0,0))),
      (,$(,(0,0,1.09,0,0))),
      (,$(,(0,0,0,1,0))),
      (,$(,(0.01,0.01,0.015,0,1)))
    )
    $iaOnline.SetColorMatrix($onlineMatrix)
    $g.DrawImage($online,$onlineDst,0,0,$online.Width,$online.Height,[System.Drawing.GraphicsUnit]::Pixel,$iaOnline)

    $iaSource = New-Object System.Drawing.Imaging.ImageAttributes
    $alpha = 0.22 + (($idx % 5) * 0.04)
    $sourceMatrix = New-Object System.Drawing.Imaging.ColorMatrix @(
      (,$(,(1,0,0,0,0))),
      (,$(,(0,1,0,0,0))),
      (,$(,(0,0,1,0,0))),
      (,$(,(0,0,0,$alpha,0))),
      (,$(,(0,0,0,0,1)))
    )
    $iaSource.SetColorMatrix($sourceMatrix)
    $g.DrawImage($source,$sourceDst,0,0,$source.Width,$source.Height,[System.Drawing.GraphicsUnit]::Pixel,$iaSource)

    $hud = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(40,255,255,255))
    $g.FillRectangle($hud, 12, 12, 180, 28)
    $font = New-Object System.Drawing.Font "Consolas",9,[System.Drawing.FontStyle]::Bold
    $textBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(225,235,245,255))
    $g.DrawString(("FACE MIX {0:D2}" -f $idx), $font, $textBrush, 18, 18)

    $pen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(180,210,230,255)),2
    $g.DrawRectangle($pen, 8, 8, $w - 16, $h - 16)

    $dst.Save($outPath,[System.Drawing.Imaging.ImageFormat]::Png)

    $pen.Dispose(); $textBrush.Dispose(); $font.Dispose(); $hud.Dispose()
    $iaSource.Dispose(); $iaOnline.Dispose(); $bg.Dispose()
    $g.Dispose(); $dst.Dispose(); $source.Dispose(); $online.Dispose()
}

$generated = @()
for ($i = 0; $i -lt 10; $i++) {
    $onlineFile = Join-Path $onlineDir ("online_ref_{0:D2}.png" -f ($i + 1))
    $sourceFile = $sourceFaces[$rand.Next(0,$sourceFaces.Count)].FullName
    $outName = "sourcefaces_online_gen_{0:D2}.png" -f ($i + 1)
    $outPath = Join-Path $outDir $outName

    Render-Variant -onlinePath $onlineFile -sourcePath $sourceFile -outPath $outPath -idx ($i + 1)

    $generated += [PSCustomObject]@{
        output = "Assets/Resources/GeneratedFaces/$outName"
        onlineRef = "Assets/Resources/OnlineRefs/online_ref_{0:D2}.png" -f ($i + 1)
        sourceBlend = [System.IO.Path]::GetFileName($sourceFile)
    }
}

[PSCustomObject]@{
    source_query = "DiceBear adventurer API seeds"
    selected_online = $selectedOnline
    generated = $generated
} | ConvertTo-Json -Depth 6 | Set-Content -Path $metaPath -Encoding UTF8

"done"
